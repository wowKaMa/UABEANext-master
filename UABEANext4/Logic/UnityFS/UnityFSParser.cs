using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UABEANext4.Util;
using AssetsTools.NET;

namespace UABEANext4.Logic.UnityFS
{
    public class UnityFSParser
    {
        public class ValidationItem
        {
            public string Label { get; set; } = "";
            public string Expected { get; set; } = "";
            public string Actual { get; set; } = "";
            public bool IsError { get; set; }
            public string Description { get; set; } = "";
        }

        public class NodeInfo
        {
            public string Path { get; set; } = "";
            public long Offset { get; set; }
            public long Size { get; set; }
        }

        public class AnalysisReport
        {
            public List<ValidationItem> HeaderItems { get; } = new();
            public List<ValidationItem> BlockItems { get; } = new();
            public List<ValidationItem> NodeItems { get; } = new();
            public List<ValidationItem> DeepScanItems { get; } = new();
            public bool HasCriticalError => HeaderItems.Any(i => i.IsError) || BlockItems.Any(i => i.IsError) || NodeItems.Any(i => i.IsError);
        }

        public class DataBlockInfo
        {
            public uint CompressedSize { get; set; }
            public uint UncompressedSize { get; set; }
            public ushort Flags { get; set; }
            
            // 底层净荷分析扩展属性
            public uint PayloadHeaderOverhead { get; set; }
            public uint PureCompressedDataSize { get; set; }
            public uint DictionaryOverhead => 10; 
            
            // 物理连续性偏移量 (十进制)
            public long StartOffset { get; set; }
            public long EndOffset { get; set; }

            // 全局虚拟解压流坐标 (用于与 Node 进行区间交叉比对)
            public long DecompressedOffset { get; set; }
        }

        public async Task<AnalysisReport> AnalyzeFileAsync(string filePath)
        {
            return await Task.Run(() =>
            {
                var report = new AnalysisReport();
                try
                {
                    using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var reader = new BigEndianBinaryReader(fs);

                    // Phase 1: Header
                    AnalyzeHeader(fs, reader, report, out var blocksInfoPos, out var blocksInfoCompressedSize, out var blocksInfoUncompressedSize, out var archiveFlags, out var version, out var headerEndPos);

                    if (report.HasCriticalError) return report;

                    // Phase 2: 解析 BlocksInfo (暂时不输出块明细，等待节点解析完毕后进行交叉比对)
                    byte[] decompressedBlocksInfo = AnalyzeBlocksInfo(fs, reader, report, blocksInfoPos, blocksInfoCompressedSize, blocksInfoUncompressedSize, archiveFlags, version, headerEndPos, out var dataBlocks);

                    if (report.HasCriticalError) return report;

                    // Phase 3: 解析 Nodes
                    AnalyzeNodes(decompressedBlocksInfo, report, dataBlocks, out var nodes);

                    // Phase 2.1: 执行物理块与逻辑节点的区间交叉比对，并渲染到面板
                    MapBlocksToNodesAndRender(report, dataBlocks, nodes);

                    // Phase 2.5: PipelineManager Blueprint ID
                    ScanBlueprintId(fs, report, dataBlocks, archiveFlags, version, headerEndPos, blocksInfoCompressedSize);

                    // Phase 4: CAB Deep Sniffing
                    AnalyzeCAB(fs, report, nodes, dataBlocks, archiveFlags, version, headerEndPos, blocksInfoCompressedSize);
                }
                catch (Exception ex)
                {
                    report.HeaderItems.Add(new ValidationItem
                    {
                        Label = "整体解析异常",
                        IsError = true,
                        Actual = "Exception",
                        Description = ex.Message
                    });
                }
                return report;
            });
        }

        private void AnalyzeHeader(FileStream fs, BigEndianBinaryReader reader, AnalysisReport report, out long blocksInfoPos, out uint blocksInfoCompressedSize, out uint blocksInfoUncompressedSize, out uint archiveFlags, out uint version, out long headerEndPos)
        {
            blocksInfoPos = 0;
            blocksInfoCompressedSize = 0;
            blocksInfoUncompressedSize = 0;
            archiveFlags = 0;
            version = 0;
            headerEndPos = 0;

            string magic = reader.ReadNullTerminatedString();
            bool magicOk = magic == "UnityFS";
            report.HeaderItems.Add(new ValidationItem
            {
                Label = "Magic String",
                Expected = "UnityFS",
                Actual = magic,
                IsError = !magicOk,
                Description = magicOk ? "文件格式识别成功" : "无效的 UnityFS 文件标识"
            });

            version = reader.ReadUInt32();
            report.HeaderItems.Add(new ValidationItem
            {
                Label = "Format Version",
                Actual = version.ToString(),
                Description = "UnityFS 格式版本"
            });

            string unityVer = reader.ReadNullTerminatedString();
            string unityRev = reader.ReadNullTerminatedString();
            report.HeaderItems.Add(new ValidationItem
            {
                Label = "Unity Version",
                Actual = $"{unityVer} ({unityRev})",
                Description = "生成此文件的 Unity 版本"
            });

            long fileSize = reader.ReadInt64();
            long actualSize = fs.Length;
            bool sizeOk = fileSize == actualSize;
            report.HeaderItems.Add(new ValidationItem
            {
                Label = "File Size",
                Expected = fileSize.ToString("N0"),
                Actual = actualSize.ToString("N0"),
                IsError = !sizeOk,
                Description = sizeOk ? "文件大小校验一致" : "物理文件大小与 Header 记录不符"
            });

            blocksInfoCompressedSize = reader.ReadUInt32();
            blocksInfoUncompressedSize = reader.ReadUInt32();
            archiveFlags = reader.ReadUInt32();
            
            headerEndPos = reader.BaseStream.Position;

            if (blocksInfoCompressedSize > 0x7FFFFFFF || blocksInfoUncompressedSize > 0x7FFFFFFF)
            {
                report.HeaderItems.Add(new ValidationItem
                {
                    Label = "BlocksInfo 大小异常",
                    IsError = true,
                    Actual = $"{blocksInfoCompressedSize} / {blocksInfoUncompressedSize}",
                    Description = "数据字典大小超出 2GB 限制或数值无效"
                });
                return;
            }

            report.HeaderItems.Add(new ValidationItem
            {
                Label = "Archive Flags",
                Actual = $"0x{archiveFlags:X8}",
                Description = $"压缩类型: {archiveFlags & 0x3F}, 标志位: {(archiveFlags & 0xC0):X2}"
            });

            if ((archiveFlags & 0x80) != 0)
            {
                blocksInfoPos = fileSize - blocksInfoCompressedSize;
            }
            else
            {
                blocksInfoPos = headerEndPos;
                if (version >= 7)
                {
                    blocksInfoPos = (blocksInfoPos + 15) & ~15;
                }
            }

            report.HeaderItems.Add(new ValidationItem
            {
                Label = "BlocksInfo Offset",
                Actual = $"0x{blocksInfoPos:X}",
                Description = $"{( (archiveFlags & 0x80) != 0 ? "在文件尾部" : "在文件头部 (对齐后)" )}"
            });
        }

        private byte[] AnalyzeBlocksInfo(FileStream fs, BigEndianBinaryReader reader, AnalysisReport report, long pos, uint cSize, uint uSize, uint archiveFlags, uint version, long headerEndPos, out List<DataBlockInfo> dataBlocks)
        {
            dataBlocks = new List<DataBlockInfo>();
            if (pos < 0 || pos > fs.Length)
            {
                report.BlockItems.Add(new ValidationItem { Label = "定位失败", IsError = true, Description = $"BlocksInfo 偏移 0x{pos:X} 无效" });
                return Array.Empty<byte>();
            }

            byte[] uncompressedData = new byte[uSize];
            try
            {
                fs.Position = pos;
                byte[] compressedData = new byte[cSize];
                int read = fs.Read(compressedData, 0, (int)cSize);
                if (read != (int)cSize) throw new Exception("读取 BlocksInfo 数据不完整");

                uint compressionType = archiveFlags & 0x3F;
                if (compressionType == 0)
                {
                    Buffer.BlockCopy(compressedData, 0, uncompressedData, 0, (int)uSize);
                }
                else if (compressionType == 1)
                {
                    using var msc = new MemoryStream(compressedData);
                    using var msu = new MemoryStream(uncompressedData);
                    SevenZipHelper.StreamDecompressLzma(msc, msu, (long)cSize, (long)uSize);
                }
                else
                {
                    LZ4ps.LZ4Codec.Decode32(compressedData, 0, (int)cSize, uncompressedData, 0, (int)uSize, true);
                }

                report.BlockItems.Add(new ValidationItem
                {
                    Label = "BlocksInfo 解压",
                    Expected = uSize.ToString("N0"),
                    Actual = uncompressedData.Length.ToString("N0"),
                    IsError = false,
                    Description = "数据字典解压成功"
                });
            }
            catch (Exception ex)
            {
                report.BlockItems.Add(new ValidationItem
                {
                    Label = "BlocksInfo 解压失败",
                    IsError = true,
                    Actual = "Fail",
                    Description = ex.Message
                });
                return Array.Empty<byte>();
            }

            using var ms = new MemoryStream(uncompressedData);
            using var biReader = new BigEndianBinaryReader(ms);

            biReader.ReadBytes(16); // hash
            uint blockCount = biReader.ReadUInt32();
            if (blockCount > 100000) throw new Exception("数据块数量异常过多");

            report.BlockItems.Add(new ValidationItem
            {
                Label = "数据块总数",
                Actual = blockCount.ToString(),
                Description = "UnityFS 内部存储的压缩数据块数量"
            });

            long totalUncompressedSize = 0;
            long totalCompressedSize = 0;
            long sizeLzma = 0;
            long sizeLz4 = 0;
            long sizeLz4hc = 0;
            long sizeUncompressed = 0;
            long totalPureCompressedData = 0;

            // 获取绝对物理偏移游标
            long currentPhysicalOffset = GetBlocksDataStartOffset(archiveFlags, version, headerEndPos, cSize);
            // 获取虚拟解压流游标 (用于后续与 Nodes 交集计算)
            long currentDecompressedOffset = 0;

            for (int i = 0; i < blockCount; i++)
            {
                uint bUncompressedSize = biReader.ReadUInt32();
                uint bCompressedSize = biReader.ReadUInt32();
                ushort bFlags = biReader.ReadUInt16();

                if (bCompressedSize > 0x7FFFFFFF || bUncompressedSize > 0x7FFFFFFF)
                    throw new Exception("单个数据块大小异常");

                uint compType = (uint)(bFlags & 0x3F);
                uint payloadHeader = 0;
                if (compType == 1) payloadHeader = 5; 
                
                uint pureData = bCompressedSize >= payloadHeader ? bCompressedSize - payloadHeader : 0;

                if (compType == 0) sizeUncompressed += bCompressedSize;
                else if (compType == 1) sizeLzma += bCompressedSize;
                else if (compType == 2) sizeLz4 += bCompressedSize;
                else if (compType == 3) sizeLz4hc += bCompressedSize;

                totalUncompressedSize += bUncompressedSize;
                totalCompressedSize += bCompressedSize;
                totalPureCompressedData += pureData;

                long blockStart = currentPhysicalOffset;
                long blockEnd = blockStart + bCompressedSize - 1;

                dataBlocks.Add(new DataBlockInfo
                {
                    CompressedSize = bCompressedSize,
                    UncompressedSize = bUncompressedSize,
                    Flags = bFlags,
                    PayloadHeaderOverhead = payloadHeader,
                    PureCompressedDataSize = pureData,
                    StartOffset = blockStart,
                    EndOffset = blockEnd,
                    DecompressedOffset = currentDecompressedOffset
                });

                currentPhysicalOffset += bCompressedSize;
                currentDecompressedOffset += bUncompressedSize;
            }

            report.BlockItems.Add(new ValidationItem
            {
                Label = "📦 数据块总大小 (解压后)",
                Actual = $"{totalUncompressedSize:N0} 字节",
                Description = $"所有数据块解压后的总体积 (压缩前物理文件占比: {totalCompressedSize:N0} 字节)"
            });

            report.BlockItems.Add(new ValidationItem
            {
                Label = "📊 压缩格式物理空间占用",
                Actual = $"纯压缩净荷: {totalPureCompressedData:N0} 字节",
                Description = $"LZMA: {sizeLzma:N0} | LZ4: {sizeLz4:N0} | LZ4HC: {sizeLz4hc:N0} | 结构化开销总计: {(blockCount * 10) + (sizeLzma > 0 ? (blockCount * 5) : 0)} 字节"
            });

            // 注：区块详细列表生成已移至 MapBlocksToNodesAndRender 方法
            return uncompressedData;
        }

        private void MapBlocksToNodesAndRender(AnalysisReport report, List<DataBlockInfo> dataBlocks, List<NodeInfo> nodes)
        {
            // 循环遍历每一个数据块，进行物理与逻辑的关联映射
            for (int i = 0; i < dataBlocks.Count; i++)
            {
                var block = dataBlocks[i];
                uint compType = (uint)(block.Flags & 0x3F);
                string compName = compType switch { 0 => "None", 1 => "LZMA", 2 => "LZ4", 3 => "LZ4HC", _ => "Unknown" };

                // 计算当前数据块在“全局虚拟解压流”中的管辖区间
                long blockDecompStart = block.DecompressedOffset;
                long blockDecompEnd = blockDecompStart + block.UncompressedSize - 1;

                // 核心：区间交集算法。找出所有与当前 Block 存在交集的 Nodes
                var overlappingNodes = nodes.Where(n => 
                    n.Offset <= blockDecompEnd && 
                    (n.Offset + n.Size - 1) >= blockDecompStart
                ).Select(n => Path.GetFileName(n.Path)).ToList();

                string nodeStr = overlappingNodes.Count > 0 ? string.Join(", ", overlappingNodes) : "无分配数据 (间隙/碎片)";
                
                // 如果一个块里包含了太多极小的碎片文件，截断文本以保证 UI 美观
                if (nodeStr.Length > 60) 
                {
                    nodeStr = nodeStr.Substring(0, 57) + "...";
                }

                report.BlockItems.Add(new ValidationItem
                {
                    Label = $"Block #{i}",
                    Actual = $"物理偏移: {block.StartOffset:N0} -> {block.EndOffset:N0} (压: {block.CompressedSize:N0} B)",
                    // 在 Description 中完美融合 节点归属信息 与 解压算法统计
                    Description = $"[关联节点: {nodeStr}] | 解压: {block.UncompressedSize:N0} B | 算法: {compName} | 纯数据: {block.PureCompressedDataSize:N0} B"
                });

                if (block.UncompressedSize < block.CompressedSize && compType != 0)
                {
                    report.BlockItems.Add(new ValidationItem
                    {
                        Label = $"Block #{i} 结构风险",
                        IsError = true,
                        Expected = $">= {block.CompressedSize}",
                        Actual = block.UncompressedSize.ToString(),
                        Description = "解压后体积反而减小，可能存在数据损坏或非标准压缩"
                    });
                }
            }
        }

        private void ScanBlueprintId(FileStream fs, AnalysisReport report, List<DataBlockInfo> dataBlocks, uint archiveFlags, uint version, long headerEndPos, uint blocksInfoCompressedSize)
        {
            long currentCompressedOffset = GetBlocksDataStartOffset(archiveFlags, version, headerEndPos, blocksInfoCompressedSize);
            long currentDecompressedOffset = 0;

            byte[] pattern = Encoding.ASCII.GetBytes("avtr_");
            int matchIndex = 0;
            long foundOffset = -1;
            string foundId = null;
            int foundStartBlock = -1;
            int foundEndBlock = -1;

            List<byte> idBytes = new List<byte>();
            int idLengthRead = 0;
            const int expectedIdLength = 41; 

            try
            {
                for (int b = 0; b < dataBlocks.Count; b++)
                {
                    var block = dataBlocks[b];
                    byte[] uncompressed = DecompressSingleBlock(fs, currentCompressedOffset, block);

                    for (int i = 0; i < uncompressed.Length; i++)
                    {
                        if (foundId == null)
                        {
                            if (idLengthRead > 0)
                            {
                                idBytes.Add(uncompressed[i]);
                                idLengthRead++;
                                if (idLengthRead == expectedIdLength)
                                {
                                    foundId = Encoding.ASCII.GetString(idBytes.ToArray());
                                    foundEndBlock = b;
                                    break;
                                }
                            }
                            else
                            {
                                if (uncompressed[i] == pattern[matchIndex])
                                {
                                    matchIndex++;
                                    if (matchIndex == pattern.Length)
                                    {
                                        foundOffset = currentDecompressedOffset + i - pattern.Length + 1;
                                        foundStartBlock = b;
                                        idBytes.AddRange(pattern);
                                        idLengthRead = pattern.Length;
                                        matchIndex = 0;
                                    }
                                }
                                else
                                {
                                    if (matchIndex > 0)
                                    {
                                        int backtrack = matchIndex;
                                        matchIndex = 0;
                                        if (i >= backtrack) i -= backtrack; 
                                    }
                                }
                            }
                        }
                    }

                    currentDecompressedOffset += block.UncompressedSize;
                    currentCompressedOffset += block.CompressedSize;

                    if (foundId != null) break;
                }

                if (foundId != null)
                {
                    long foundEndOffset = foundOffset + expectedIdLength - 1; 

                    string blockMsg = foundStartBlock == foundEndBlock
                        ? $"存在于 Block #{foundStartBlock} 内"
                        : $"跨块分布: 头部在 Block #{foundStartBlock}, 尾部在 Block #{foundEndBlock}";

                    report.BlockItems.Add(new ValidationItem
                    {
                        Label = "🔑 PipelineManager 蓝图 ID",
                        Actual = foundId,
                        IsError = false,
                        Description = $"{blockMsg} (解压流十进制偏移范围: {foundOffset:N0} -> {foundEndOffset:N0})"
                    });
                }
                else
                {
                    report.BlockItems.Add(new ValidationItem
                    {
                        Label = "PipelineManager 蓝图 ID",
                        IsError = true,
                        Actual = "未嗅探到",
                        Description = "在所有解压数据块中均未找到 avtr_ 标识符，资产可能被混淆或不包含蓝图"
                    });
                }
            }
            catch (Exception ex)
            {
                report.BlockItems.Add(new ValidationItem
                {
                    Label = "蓝图 ID 扫描异常",
                    IsError = true,
                    Actual = "Error",
                    Description = $"深度扫描过程中解压出错: {ex.Message}"
                });
            }
        }

        private void AnalyzeNodes(byte[] decompressedBlocksInfo, AnalysisReport report, List<DataBlockInfo> dataBlocks, out List<NodeInfo> nodes)
        {
            nodes = new List<NodeInfo>();
            if (decompressedBlocksInfo == null || decompressedBlocksInfo.Length < 20) return;

            using var ms = new MemoryStream(decompressedBlocksInfo);
            using var reader = new BigEndianBinaryReader(ms);
            
            reader.ReadBytes(16);
            uint blockCount = reader.ReadUInt32();
            if (blockCount > 100000) return;
            for (int i = 0; i < blockCount; i++)
            {
                reader.ReadUInt32(); reader.ReadUInt32(); reader.ReadUInt16();
            }

            uint nodeCount = reader.ReadUInt32();
            if (nodeCount > 100000) return;

            for (int i = 0; i < nodeCount; i++)
            {
                long offset = reader.ReadInt64();
                long size = reader.ReadInt64();
                uint flags = reader.ReadUInt32();
                string path = reader.ReadNullTerminatedString();

                nodes.Add(new NodeInfo { Path = path, Offset = offset, Size = size });
                
                report.NodeItems.Add(new ValidationItem
                {
                    Label = $"节点: {Path.GetFileName(path)}",
                    Actual = $"Offset: 0x{offset:X}, Size: {size:N0}",
                    Description = $"路径: {path}"
                });
            }

            if (nodes.Count == 0) return;

            var sortedNodes = nodes.OrderBy(n => n.Offset).ToList();
            long currentOffset = 0;
            for (int i = 0; i < sortedNodes.Count; i++)
            {
                var node = sortedNodes[i];
                if (node.Offset > currentOffset)
                {
                    report.NodeItems.Add(new ValidationItem
                    {
                        Label = "数据间隙 (Gap)",
                        IsError = true,
                        Expected = currentOffset.ToString(),
                        Actual = node.Offset.ToString(),
                        Description = $"在 {node.Path} 之前发现缝隙"
                    });
                }
                currentOffset = node.Offset + node.Size;
            }

            long totalUncompressedDataSize = dataBlocks.Sum(b => (long)b.UncompressedSize);
            if (currentOffset == totalUncompressedDataSize)
            {
                report.NodeItems.Add(new ValidationItem
                {
                    Label = "节点连续性",
                    Actual = "✅ 完美连续",
                    Description = "所有资产节点逻辑排列紧密，无缝隙或重叠"
                });
            }
        }

        private void AnalyzeCAB(FileStream fs, AnalysisReport report, List<NodeInfo> nodes, List<DataBlockInfo> dataBlocks, uint archiveFlags, uint version, long headerEndPos, uint blocksInfoCompressedSize)
        {
            var cabNodes = nodes.Where(n => n.Path.Contains("CAB-")).ToList();
            foreach (var node in cabNodes)
            {
                try 
                {
                    byte[] headerBuffer = ReadDecompressedData(fs, node.Offset, 20, dataBlocks, archiveFlags, version, headerEndPos, blocksInfoCompressedSize);
                    if (headerBuffer.Length >= 20)
                    {
                        report.DeepScanItems.Add(new ValidationItem
                        {
                            Label = $"资产嗅探: {Path.GetFileName(node.Path)}",
                            Actual = $"Size: {node.Size:N0}",
                            Description = $"成功提取头部数据({headerBuffer.Length} bytes)，路径: {node.Path}"
                        });
                    }
                    else
                    {
                        report.DeepScanItems.Add(new ValidationItem
                        {
                            Label = $"嗅探失败: {Path.GetFileName(node.Path)}",
                            IsError = true,
                            Description = $"提取截断，仅读到 {headerBuffer.Length} 字节"
                        });
                    }
                } 
                catch (Exception ex)
                {
                    report.DeepScanItems.Add(new ValidationItem
                    {
                        Label = $"异常终止: {Path.GetFileName(node.Path)}",
                        IsError = true,
                        Description = $"解码或越界报错: {ex.Message}"
                    });
                }
            }
        }

        private byte[] ReadDecompressedData(FileStream fs, long offset, int size, List<DataBlockInfo> dataBlocks, uint archiveFlags, uint version, long headerEndPos, uint blocksInfoCompressedSize)
        {
            long currentDecompressedOffset = 0;
            long currentCompressedOffset = GetBlocksDataStartOffset(archiveFlags, version, headerEndPos, blocksInfoCompressedSize);
            
            byte[] result = new byte[size];
            int bytesRead = 0;
            long targetOffset = offset; 

            foreach (var block in dataBlocks)
            {
                if (bytesRead >= size) break;

                long blockEndOffset = currentDecompressedOffset + block.UncompressedSize;

                if (targetOffset >= currentDecompressedOffset && targetOffset < blockEndOffset)
                {
                    byte[] uncompressed = DecompressSingleBlock(fs, currentCompressedOffset, block);

                    long offsetInBlock = targetOffset - currentDecompressedOffset;
                    int availableInBlock = (int)(block.UncompressedSize - offsetInBlock);
                    int toRead = Math.Min(size - bytesRead, availableInBlock);
                    
                    Buffer.BlockCopy(uncompressed, (int)offsetInBlock, result, bytesRead, toRead);
                    
                    bytesRead += toRead;
                    targetOffset += toRead;
                }
                
                currentDecompressedOffset += block.UncompressedSize;
                currentCompressedOffset += block.CompressedSize;
            }

            if (bytesRead < size) Array.Resize(ref result, bytesRead);
            return result;
        }

        private long GetBlocksDataStartOffset(uint archiveFlags, uint version, long headerEndPos, uint blocksInfoCompressedSize)
        {
            if ((archiveFlags & 0x80) != 0)
            {
                long dataPos = headerEndPos;
                if (version >= 7) dataPos = (dataPos + 15) & ~15;
                return dataPos;
            }
            else
            {
                long blocksInfoStartPos = headerEndPos;
                if (version >= 7) blocksInfoStartPos = (blocksInfoStartPos + 15) & ~15;
                return blocksInfoStartPos + blocksInfoCompressedSize;
            }
        }

        private byte[] DecompressSingleBlock(FileStream fs, long compressedOffset, DataBlockInfo block)
        {
            fs.Position = compressedOffset;
            byte[] compressed = new byte[block.CompressedSize];
            fs.Read(compressed, 0, (int)block.CompressedSize);
            
            byte[] uncompressed = new byte[block.UncompressedSize];
            uint compType = (uint)(block.Flags & 0x3F);
            
            if (compType == 0) 
            {
                Buffer.BlockCopy(compressed, 0, uncompressed, 0, (int)block.UncompressedSize);
            }
            else if (compType == 1) 
            {
                using var msc = new MemoryStream(compressed);
                using var msu = new MemoryStream(uncompressed);
                SevenZipHelper.StreamDecompressLzma(msc, msu, block.CompressedSize, block.UncompressedSize);
            }
            else 
            {
                LZ4ps.LZ4Codec.Decode32(compressed, 0, (int)block.CompressedSize, uncompressed, 0, (int)block.UncompressedSize, true);
            }
            return uncompressed;
        }

        public static class SevenZipHelper
        {
            public static void StreamDecompressLzma(Stream inStream, Stream outStream, long inSize, long outSize)
            {
                SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();
                byte[] properties = new byte[5];
                if (inStream.Read(properties, 0, 5) != 5)
                    throw new Exception("输入的 LZMA 流已截断，无法读取配置头 (Stream Position Error)");
                decoder.SetDecoderProperties(properties);
                decoder.Code(inStream, outStream, inSize - 5, outSize, null);
            }
        }
    }
}