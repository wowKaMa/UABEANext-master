using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于处理 Unity 资产文件的底层 API（保留原名 AssetsTools.NET）

using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展功能（保留原名 AssetsTools.NET.Extra）

using CommunityToolkit.Mvvm.ComponentModel;
// 引用 CommunityToolkit.Mvvm 的组件模型，提供 ObservableProperty 等 MVVM 特性（保留原名 CommunityToolkit.Mvvm.ComponentModel）

using System;
// 引用基础系统命名空间，提供基本类型与工具（保留原名 System）

using System.Buffers.Binary;
// 引用二进制缓冲区工具，用于按大小端写入数值（保留原名 System.Buffers.Binary）

using System.Collections.Generic;
// 引用泛型集合命名空间（List、Dictionary 等）（保留原名 System.Collections.Generic）

using System.Collections.ObjectModel;
// 引用可观察集合类型（ObservableCollection），用于 UI 绑定（保留原名 System.Collections.ObjectModel）

using System.IO;
// 引用 IO 操作命名空间，用于流与文件读写（保留原名 System.IO）

using System.Linq;
// 引用 LINQ 扩展方法，用于集合查询与转换（保留原名 System.Linq）

using System.Text;
// 引用文本编码与处理命名空间（Encoding 等）（保留原名 System.Text）

using System.Threading;
// 引用线程与同步原语（CancellationToken、Interlocked 等）（保留原名 System.Threading）

using System.Threading.Tasks;
// 引用异步任务支持（Task、async/await）（保留原名 System.Threading.Tasks）

using UABEANext4.AssetWorkspace;
// 引用项目的资产工作区命名空间（Workspace、AssetsFileInstance 等）（保留原名 UABEANext4.AssetWorkspace）

using UABEANext4.Interfaces;
// 引用项目接口命名空间，包含 IDialogAware 等接口（保留原名 UABEANext4.Interfaces）

using UABEANext4.Util;
// 引用项目工具类命名空间（MessageBoxUtil、PathUtils 等）（保留原名 UABEANext4.Util）

namespace UABEANext4.ViewModels.Dialogs;
// 定义命名空间 UABEANext4.ViewModels.Dialogs（保留原名），组织对话框相关的视图模型类

public partial class AssetDataSearchViewModel : ViewModelBase, IDialogAware<string?>
// 定义部分类 AssetDataSearchViewModel，继承 ViewModelBase 并实现 IDialogAware<string?>（对话框返回 string?），类名保留原名
{
    [ObservableProperty]
    public string _searchText = "";
    // 可观察字段：搜索文本的后备字段（生成的属性名为 SearchText），默认空字符串

    [ObservableProperty]
    public AssetDataSearchKind _searchKind = AssetDataSearchKind.Bytes;
    // 可观察字段：搜索类型的后备字段（生成的属性名为 SearchKind），默认使用 Bytes（字节搜索）

    [ObservableProperty]
    public ObservableCollection<string> _searchResults = [];
    // 可观察字段：搜索结果集合的后备字段（生成的属性名为 SearchResults），初始化为空集合

    private readonly Workspace _workspace;
    // 私有只读字段：保存传入的 Workspace 实例，用于访问文件与进度控制

    private readonly List<AssetsFileInstance> _items;
    // 私有只读字段：要搜索的文件实例列表

    public string Title => "十六进制搜索（开发中） (Search hex (WIP))";
    // 对话框标题（UI 显示中文并在括号保留英文原名）

    public int Width => 350;
    // 对话框宽度（像素）

    public int Height => 400;
    // 对话框高度（像素）

    public event Action<string?>? RequestClose;
    // 事件：请求关闭对话框并返回字符串（例如选中结果或 null）

    [Obsolete("This constructor is for the designer only and should not be used directly.", true)]
    public AssetDataSearchViewModel()
    // 设计器专用构造函数（标记为过时以避免运行时使用）
    {
        _workspace = new();
        // 为设计器创建一个占位 Workspace 实例

        _items = [];
        // 为设计器初始化空的文件实例列表
    }

    public AssetDataSearchViewModel(Workspace workspace, List<AssetsFileInstance> items)
    // 运行时构造函数：接收 Workspace 与要搜索的文件实例列表
    {
        _workspace = workspace;
        // 保存传入的 Workspace 实例

        _items = items;
        // 保存传入的文件实例列表
    }

    public async Task BtnSearch_Click()
    // 异步方法：当用户点击“搜索”按钮时调用，执行并显示搜索结果
    {
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Min(Environment.ProcessorCount - 1, 4)
        };
        // 配置并行选项：最大并行度为 CPU 核心数减一且上限为 4，避免占满全部核心

        byte[]? searchBytes = GetSearchBytes();
        // 将用户输入根据 SearchKind 转换为要搜索的字节序列

        if (searchBytes is null)
        {
            await MessageBoxUtil.ShowDialog("输入无效 (Invalid input)", "对于当前搜索类型，输入无效。");
            return;
        }
        // 如果转换失败，弹出错误对话框并返回

        _workspace.SetProgressThreadSafe(0f, "正在搜索文件... (Searching files...)");
        // 在工作区上设置进度为 0 并显示提示（线程安全）

        await Task.Run(() =>
        {
            _workspace.ModifyMutex.WaitOne();
            // 获取工作区的互斥锁，确保对共享状态的独占访问

            _workspace.ProgressValue = 0;
            // 重置进度值

            SearchResults.Clear();
            // 清空之前的搜索结果集合

            int currentCount = 0;
            // 当前已处理文件计数

            int itemCount = _items.Count;
            // 总文件数

            Parallel.ForEach(_items, options, (fileInst, state, index) =>
            {
                // 并行遍历每个文件实例，使用上面配置的并行选项

                // this is always an ObservableCollection for us which means
                // we have to use the special extension BinarySearch method.
                var assetInfos = (ObservableCollection<AssetFileInfo>)fileInst.file.AssetInfos;
                // 将文件的 AssetInfos 强制转换为 ObservableCollection<AssetFileInfo>，以便后续使用 BinarySearch 扩展

                foreach (long pos in FindAllSubstringsInStream(fileInst.AssetsStream, searchBytes))
                {
                    // 在文件流中查找所有匹配的字节位置，遍历每个匹配位置

                    int searchIdx = assetInfos.BinarySearch(
                        new AssetFileInfo()
                        {
                            ByteOffset = pos - fileInst.file.Header.DataOffset,
                        },
                        (i, j) => i.ByteOffset.CompareTo(j.ByteOffset)
                    );
                    // 在 assetInfos 中二分查找包含该字节偏移的 AssetFileInfo，比较依据是 ByteOffset

                    if (searchIdx == -1)
                    {
                        // didn't find anything? string was probably found outside of an asset.
                        // let's just put the address so the user can look for it themselves.
                        if (fileInst.parentBundle is not null)
                            SearchResults.Add($"{fileInst.parentBundle.name} {fileInst.name} @ {pos:x2}");
                        else
                            SearchResults.Add($"{fileInst.name} @ {pos:x2}");
                    }
                    else
                    {
                        AssetFileInfo? info = (searchIdx < 0)
                            ? assetInfos[~searchIdx - 1]
                            : assetInfos[searchIdx];
                        // 根据 BinarySearch 的返回值计算实际对应的 AssetFileInfo（处理负值插入点的情况）

                        var name = (info is AssetInst asset)
                            ? asset.DisplayName
                            : $"{info.TypeId} asset";
                        // 如果 info 是 AssetInst 则使用其 DisplayName，否则用类型 ID 描述

                        if (fileInst.parentBundle is not null)
                            SearchResults.Add($"{fileInst.parentBundle.name} {fileInst.name} -> {name}, {info.PathId}");
                        else
                            SearchResults.Add($"{fileInst.name} -> {name}, {info.PathId}");
                        // 将格式化的结果加入 SearchResults 集合（包含文件名、资产名与 PathId）
                    }

                }

                var currentCountNow = Interlocked.Increment(ref currentCount);
                // 原子递增已处理文件计数

                _workspace.SetProgressThreadSafe((float)currentCountNow / itemCount, $"正在搜索文件 {fileInst.name}... (Searching file {fileInst.name}...)");
                // 更新进度显示，显示当前正在处理的文件名（线程安全）
            });
            _workspace.SetProgressThreadSafe(1f, "完成 (Done)");
            // 并行循环结束后将进度设为 100% 并显示完成

            _workspace.ModifyMutex.ReleaseMutex();
            // 释放工作区互斥锁
        });
    }

    public void BtnCancel_Click()
    // 方法：当用户点击“取消”按钮时调用，关闭对话框并返回 null
    {
        RequestClose?.Invoke(null);
    }

    private byte[]? GetSearchBytes()
    // 私有方法：根据 SearchKind 将 SearchText 转换为要搜索的字节数组，失败返回 null
    {
        var bigEndian = _items.Count > 0 && _items[0].file.Header.Endianness;
        // 判断字节序：如果有文件则以第一个文件的 Endianness 为准（true 表示大端）

        byte[] searchBytes;
        switch (SearchKind)
        {
            case AssetDataSearchKind.Bytes:
            {
                searchBytes = Convert.FromHexString(SearchText.Replace(" ", ""));
                break;
            }
            // 如果是字节搜索（Hex），去掉空格并将十六进制字符串转换为字节数组

            case AssetDataSearchKind.Text:
            {
                searchBytes = Encoding.UTF8.GetBytes(SearchText);
                break;
            }
            // 如果是文本搜索，将文本按 UTF-8 编码为字节数组

            case AssetDataSearchKind.Signed4Byte:
            {
                if (!int.TryParse(SearchText, out int searchInt))
                    return null;

                searchBytes = new byte[4];
                if (bigEndian)
                    BinaryPrimitives.WriteInt32BigEndian(searchBytes, searchInt);
                else
                    BinaryPrimitives.WriteInt32LittleEndian(searchBytes, searchInt);

                break;
            }
            // 有符号 4 字节整数：解析为 int 并按目标字节序写入 4 字节数组

            case AssetDataSearchKind.Signed8Byte:
            {
                if (!long.TryParse(SearchText, out long searchLong))
                    return null;

                searchBytes = new byte[8];
                if (bigEndian)
                    BinaryPrimitives.WriteInt64BigEndian(searchBytes, searchLong);
                else
                    BinaryPrimitives.WriteInt64LittleEndian(searchBytes, searchLong);

                break;
            }
            // 有符号 8 字节整数：解析为 long 并按目标字节序写入 8 字节数组

            case AssetDataSearchKind.Unsigned4Byte:
            {
                if (!uint.TryParse(SearchText, out uint searchUint))
                    return null;

                searchBytes = new byte[4];
                if (bigEndian)
                    BinaryPrimitives.WriteUInt32BigEndian(searchBytes, searchUint);
                else
                    BinaryPrimitives.WriteUInt32LittleEndian(searchBytes, searchUint);

                break;
            }
            // 无符号 4 字节整数：解析为 uint 并按目标字节序写入 4 字节数组

            case AssetDataSearchKind.Unsigned8Byte:
            {
                if (!ulong.TryParse(SearchText, out ulong searchUlong))
                    return null;

                searchBytes = new byte[8];
                if (bigEndian)
                    BinaryPrimitives.WriteUInt64BigEndian(searchBytes, searchUlong);
                else
                    BinaryPrimitives.WriteUInt64LittleEndian(searchBytes, searchUlong);

                break;
            }
            // 无符号 8 字节整数：解析为 ulong 并按目标字节序写入 8 字节数组

            case AssetDataSearchKind.Float4Byte:
            {
                if (!float.TryParse(SearchText, out float searchFloat))
                    return null;

                searchBytes = new byte[4];
                if (bigEndian)
                    BinaryPrimitives.WriteSingleBigEndian(searchBytes, searchFloat);
                else
                    BinaryPrimitives.WriteSingleLittleEndian(searchBytes, searchFloat);

                break;
            }
            // 4 字节浮点数：解析为 float 并按目标字节序写入 4 字节数组

            case AssetDataSearchKind.Float8Byte:
            {
                if (!double.TryParse(SearchText, out double searchDouble))
                    return null;

                searchBytes = new byte[8];
                if (bigEndian)
                    BinaryPrimitives.WriteDoubleBigEndian(searchBytes, searchDouble);
                else
                    BinaryPrimitives.WriteDoubleLittleEndian(searchBytes, searchDouble);

                break;
            }
            // 8 字节浮点数：解析为 double 并按目标字节序写入 8 字节数组

            default:
            {
                return null;
            }
        }

        return searchBytes;
        // 返回构造好的字节数组
    }

    private static IEnumerable<long> FindAllSubstringsInStream(Stream fs, byte[] patternBytes)
    // 私有静态方法：在给定流中分块查找所有匹配的字节序列，返回每个匹配的绝对位置（long）
    {
        const int ChunkSize = 65536;
        // 每次读取的块大小（64KB）

        int patternLength = patternBytes.Length;
        // 模式字节长度

        int overlap = patternLength > 1 ? patternLength - 1 : 0;
        // 为了处理跨块匹配，需要保留的重叠字节数（模式长度 - 1）

        byte[] buffer = new byte[ChunkSize];
        // 读取缓冲区

        long currentPosition = 0;
        // 当前块在流中的绝对起始位置

        int bytesRead;
        // 本次读取的字节数

        fs.Position = 0;
        // 从流头开始读取

        while ((bytesRead = fs.Read(buffer, 0, ChunkSize)) > 0)
        {
            int indexInChunk;
            int searchStart = 0;
            while ((indexInChunk = IndexOfBytes(buffer, patternBytes, searchStart)) != -1)
            {
                long absolutePosition = currentPosition + indexInChunk;
                // 计算匹配在整个流中的绝对位置

                yield return absolutePosition;
                // 产出一个匹配位置

                searchStart = indexInChunk + patternLength;
                // 在当前块中继续从匹配后的位置搜索

                if (searchStart >= bytesRead)
                {
                    break;
                }
            }

            if (bytesRead == ChunkSize && fs.Position < fs.Length)
            {
                fs.Seek(-overlap, SeekOrigin.Current);
                // 如果还有后续数据，则回退 overlap 字节以保留跨块匹配的可能性
            }

            currentPosition += bytesRead - overlap;
            // 更新下一块的绝对起始位置（减去 overlap，因为已回退）
        }
    }

    public static int IndexOfBytes(byte[] buffer, byte[] pattern, int start = 0)
    // 公共静态方法：在 buffer 中从 start 开始查找 pattern 的首次出现，返回相对索引（若未找到返回 -1）
    {
        if (buffer == null || pattern == null || pattern.Length == 0) return -1;
        // 参数校验：空或空模式返回 -1

        if (start < 0 || start > buffer.Length - pattern.Length) return -1;
        // 起始位置越界检查

        var span = buffer.AsSpan(start);
        // 使用 Span 提高切片性能

        for (int i = 0; i <= span.Length - pattern.Length; i++)
        {
            if (span.Slice(i, pattern.Length).SequenceEqual(pattern))
                return i + start;
            // 如果在当前位置匹配则返回全局索引
        }
        return -1;
        // 未找到返回 -1
    }

    public enum AssetDataSearchKind
    // 枚举：表示支持的搜索类型（AssetDataSearchKind 保留原名）
    {
        Bytes,
        // 以十六进制字节序列搜索（Bytes）

        Text,
        // 以 UTF-8 文本搜索（Text）

        Signed4Byte,
        // 有符号 4 字节整数搜索（Signed4Byte）

        Signed8Byte,
        // 有符号 8 字节整数搜索（Signed8Byte）

        Unsigned4Byte,
        // 无符号 4 字节整数搜索（Unsigned4Byte）

        Unsigned8Byte,
        // 无符号 8 字节整数搜索（Unsigned8Byte）

        Float4Byte,
        // 4 字节浮点数搜索（Float4Byte）

        Float8Byte
        // 8 字节浮点数搜索（Float8Byte）
    }
}
