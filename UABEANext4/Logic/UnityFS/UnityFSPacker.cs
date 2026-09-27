using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using UABEANext4.AssetWorkspace;
using UABEANext4.Util;
using SevenZip;

namespace UABEANext4.Logic.UnityFS
{
    public static class UnityFSPacker
    {
        private const int CHUNK_SIZE = 33554432; // 32MB (0x2000000)

        private static T GetPropertyValue<T>(object obj, string name)
        {
            if (obj == null) return default!;
            var prop = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop != null) return (T)prop.GetValue(obj)!;
            var field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field != null) return (T)field.GetValue(obj)!;
            return default!;
        }

        /// <summary>
        /// 手动分块 LZMA 压缩 (32MB Chunks)，完全模拟官方 VRCA 结构。
        /// </summary>
        public static void PackChunkedLzma(BundleFileInstance bunInst, Stream outputStream)
        {
            if (bunInst == null) throw new ArgumentNullException(nameof(bunInst));
            if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));

            AssetBundleFile bundle = bunInst.file;

            // 1. 提取所有数据并应用内存中的修改 (Replacers)
            // 我们需要获取完整的解压流，以进行 32MB 的重新分块
            List<ManualDirectoryInfo> updatedNodes;
            byte[] fullUncompressedData = GetFullDecompressedDataWithReplacers(bundle, out updatedNodes);

            // 2. 32MB 分块并执行 LZMA 压缩
            List<byte[]> compressedChunks = new List<byte[]>();
            List<ManualBlockInfo> newBlockInfos = new List<ManualBlockInfo>();

            for (int i = 0; i < fullUncompressedData.Length; i += CHUNK_SIZE)
            {
                int currentSize = Math.Min(CHUNK_SIZE, fullUncompressedData.Length - i);
                byte[] chunk = new byte[currentSize];
                Buffer.BlockCopy(fullUncompressedData, i, chunk, 0, currentSize);

                using (MemoryStream msu = new MemoryStream(chunk))
                using (MemoryStream msc = new MemoryStream())
                {
                    // 使用 7zip LZMA 编码器
                    SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
                    SevenZip.CoderPropID[] propIDs = {
                        SevenZip.CoderPropID.DictionarySize, SevenZip.CoderPropID.PosStateBits, SevenZip.CoderPropID.LitContextBits,
                        SevenZip.CoderPropID.LitPosBits, SevenZip.CoderPropID.Algorithm, SevenZip.CoderPropID.NumFastBytes,
                        SevenZip.CoderPropID.MatchFinder, SevenZip.CoderPropID.EndMarker
                    };
                    
                    // 【终极黄金合规参数：确保跨平台无错解压与 VRChat 验证通过】
                    object[] properties = { 
                        (int)(1 << 19), // DictionarySize: 512 KB (强制要求，既过认证又省内存)
                        (int)2,         // PosStateBits
                        (int)3,         // LitContextBits
                        (int)0,         // LitPosBits
                        (int)2,         // Algorithm
                        (int)128,       // NumFastBytes: 官方高压缩标准配置
                        "bt4",          // MatchFinder
                        false           // EndMarker (关闭流结束标记以保证严格定长)
                    };
                    encoder.SetCoderProperties(propIDs, properties);

                    // 【终极真相：官方 Chunked LZMA 规范确实保留了 5 字节 Header】
                    // 在 UnityFS 现代多块分块体系中，每个独立的物理 Block 数据开头都包含了 5 字节属性头。
                    // 这样解析库 (如 AssetsTools.NET) 才能直接从中读取属性并完成 Decoder 的独立握手。
                    encoder.WriteCoderProperties(msc);
                    encoder.Code(msu, msc, currentSize, -1, null);

                    byte[] compressedData = msc.ToArray();
                    compressedChunks.Add(compressedData);

                    newBlockInfos.Add(new ManualBlockInfo()
                    {
                        UncompressedSize = (uint)currentSize,
                        CompressedSize = (uint)compressedData.Length,
                        Flags = 0x41 // LZMA + Streamed
                    });
                }
            }

// 3. 构建 BlocksInfo (数据字典)
            byte[] uncompressedBlocksInfo;
            using (MemoryStream ms = new MemoryStream())
            using (AssetsFileWriter biWriter = new AssetsFileWriter(ms))
            {
                biWriter.BigEndian = true;
                
                // 【完美终极修复 6：内存硬读法 (彻底无视所有类型和命名报错)】
                byte[] finalHash = new byte[16];
                
                // 注意这里：我们将泛型指定为 object，绝对不会再触发 InvalidCastException
                object hashObj = GetPropertyValue<object>(bundle.BlockAndDirInfo, "Hash"); 
                
                if (hashObj is byte[] bArr && bArr.Length >= 16)
                {
                    // 如果它是字节数组，直接拷贝
                    Buffer.BlockCopy(bArr, 0, finalHash, 0, 16);
                }
                else if (hashObj != null)
                {
                    // 如果底层是 Hash128 结构体，不管它里面的变量叫 data0 还是 hash0
                    // 我们直接用 C# 底层指针，把它物理内存里的 16 个字节“生抠”出来！
                    int size = System.Runtime.InteropServices.Marshal.SizeOf(hashObj);
                    if (size >= 16)
                    {
                        IntPtr ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
                        System.Runtime.InteropServices.Marshal.StructureToPtr(hashObj, ptr, false);
                        System.Runtime.InteropServices.Marshal.Copy(ptr, finalHash, 0, 16);
                        System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);
                    }
                }
                
                biWriter.Write(finalHash); // 完美写入官方 16 字节原始 Hash
                
                // ========================================================
                // 写入数据块字典
                biWriter.Write((uint)newBlockInfos.Count);
                foreach (var block in newBlockInfos)
                {
                    biWriter.Write(block.UncompressedSize);
                    biWriter.Write(block.CompressedSize);
                    biWriter.Write(block.Flags);
                }

                // 写入 DirectoryInfo (Nodes)
                biWriter.Write((uint)updatedNodes.Count);
                foreach (var node in updatedNodes)
                {
                    biWriter.Write(node.Offset);
                    biWriter.Write(node.Size);
                    biWriter.Write(node.Flags);
                    WriteNullTerminatedString(biWriter, node.Name);
                }
                
                uncompressedBlocksInfo = ms.ToArray();
            }

            // BlocksInfo 采用 LZ4 压缩以匹配 VRCA 常见标准 (Flags 0x43 or 0xC3)
            byte[] compressedBlocksInfo = LZ4EncodeHC(uncompressedBlocksInfo);

// 4. 回写完整的 UnityFS 文件
                using (AssetsFileWriter writer = new AssetsFileWriter(outputStream))
                {
                    writer.BigEndian = true;

                    // --- Header ---
                    WriteNullTerminatedString(writer, "UnityFS");
                    writer.Write(bundle.Header.Version);

                    // 【终极修复 1：完美还原官方指纹字符串 (结合你的安全反射方法)】
                    // 首先安全地从原文件中提取真实的构建版本号
                    string originalRev = GetPropertyValue<string>(bundle.Header, "EngineRevision") 
                                      ?? GetPropertyValue<string>(bundle.Header, "UnityRevision") 
                                      ?? "2022.3.22f1";

                    // 官方标准要求第一个字符串固定为 "5.x.x" (代表大版本架构)
                    // 第二个字符串才是真实的构建版本号
                    string engineVer = "5.x.x"; 
                    string engineRev = originalRev;

                    WriteNullTerminatedString(writer, engineVer);
                    WriteNullTerminatedString(writer, engineRev);

                    long archiveSizePos = writer.Position;
                    writer.Write((long)0); // 占位符：总文件大小

                    writer.Write((uint)compressedBlocksInfo.Length);
                    writer.Write((uint)uncompressedBlocksInfo.Length);

                    // 【终极修复 2：还原官方 Archive Flags 为 0x43】
                    // 彻底移除 0x80，意味着字典必须放在文件头部
                    uint archiveFlags = 0x43; 
                    writer.Write(archiveFlags);

                    // Header 16 字节对齐计算
                    if (bundle.Header.Version >= 7)
                    {
                        long curPos = writer.Position;
                        int padding = (int)((16 - (curPos % 16)) % 16);
                        if (padding > 0)
                        {
                            writer.Write(new byte[padding]);
                        }
                    }

                    // 【终极修复 3：写入顺序对调】
                    // 字典写在头部，紧跟在 Header 对齐之后，完美回到 0x40 偏移
                    writer.Write(compressedBlocksInfo);

                    // --- Data Blocks ---
                    // 顺序连续写入所有的物理数据块 (LZMA Chunks)
                    foreach (var chunkData in compressedChunks)
                    {
                        writer.Write(chunkData);
                    }

                    // --- Finalize ---
                    long finalSize = writer.Position;
                    writer.Position = archiveSizePos;
                    writer.Write(finalSize); // 覆写真实的总文件大小

                    writer.Flush();
                }
        }

        private static byte[] GetFullDecompressedDataWithReplacers(AssetBundleFile bundle, out List<ManualDirectoryInfo> updatedNodes)
        {
            updatedNodes = new List<ManualDirectoryInfo>();
            using (MemoryStream ms = new MemoryStream())
            {
                var dirInfos = bundle.BlockAndDirInfo.DirectoryInfos;
                long currentOffset = 0;
                foreach (var dir in dirInfos)
                {
                    // BundleHelper.LoadAssetDataFromBundle 内部会处理所有 Pending 的 Replacer (修改后的 AssetsFile 等)
                    byte[] data = BundleHelper.LoadAssetDataFromBundle(bundle, dir.Name);
                    if (data == null) continue;

                    updatedNodes.Add(new ManualDirectoryInfo
                    {
                        Name = dir.Name,
                        Offset = currentOffset,
                        Size = data.Length,
                        Flags = dir.Flags
                    });

                    ms.Write(data, 0, data.Length);
                    currentOffset += data.Length;
                }
                return ms.ToArray();
            }
        }

// 【终极修复 4：使用 LZ4HC 完美对齐官方 BlocksInfo 压缩率】
        private static byte[] LZ4EncodeHC(byte[] input)
        {
            int maxOutputSize = input.Length + (input.Length / 255) + 16;
            byte[] output = new byte[maxOutputSize];
            
            // 调用 LZ4ps 的 HC (High Compression) 方法
            // 这将消除你之前多出来的 10 字节差异，实现完美压缩
            int encodedSize = LZ4ps.LZ4Codec.Encode32HC(input, 0, input.Length, output, 0, output.Length);
            
            byte[] finalOutput = new byte[encodedSize];
            Buffer.BlockCopy(output, 0, finalOutput, 0, encodedSize);
            return finalOutput;
        }

        private static void WriteNullTerminatedString(AssetsFileWriter writer, string str)
        {
            if (string.IsNullOrEmpty(str)) str = "";
            byte[] bytes = Encoding.UTF8.GetBytes(str + "\0");
            writer.Write(bytes);
        }

        // 内部结构体，方便手动管理
        private struct ManualBlockInfo
        {
            public uint UncompressedSize;
            public uint CompressedSize;
            public ushort Flags;
        }

        private struct ManualDirectoryInfo
        {
            public long Offset;
            public long Size;
            public uint Flags;
            public string Name;
        }
    }
}
