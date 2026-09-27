using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型，提供 ObservableProperty 等 MVVM 特性（保留原名 CommunityToolkit.Mvvm.ComponentModel）

using System; // 引用基础系统命名空间，提供 Action 等委托类型（保留原名 System）

using System.Collections.Generic; // 引用泛型集合命名空间，提供 List<T> 等集合类型（保留原名 System.Collections.Generic）

using System.ComponentModel; // 引用用于属性变更通知的接口（INotifyPropertyChanged）（保留原名 System.ComponentModel）

using System.IO; // 引用文件与目录操作（File、Directory 等）（保留原名 System.IO）

using System.Linq; // 引用 LINQ 扩展方法，用于集合查询（保留原名 System.Linq）

using UABEANext4.AssetWorkspace; // 引用项目的资产工作区命名空间（Workspace、AssetInst 等）（保留原名 UABEANext4.AssetWorkspace）

using UABEANext4.Interfaces; // 引用项目接口命名空间，包含 IDialogAware 等（保留原名 UABEANext4.Interfaces）

using UABEANext4.Logic.Configuration; // 引用配置管理相关命名空间（ConfigurationManager 等）（保留原名 UABEANext4.Logic.Configuration）

using UABEANext4.Util; // 引用工具类（FileUtils、PathUtils、AssetNamer 等）（保留原名 UABEANext4.Util）

namespace UABEANext4.ViewModels.Dialogs; // 定义命名空间 UABEANext4.ViewModels.Dialogs，用于组织对话框相关的视图模型类（保留原名）

public partial class BatchImportViewModel : ViewModelBase, IDialogAware<List<ImportBatchInfo>?> // 定义部分类 BatchImportViewModel，继承 ViewModelBase 并实现 IDialogAware，返回 List<ImportBatchInfo>?（类名保留原名）
{
    private string _directory; // 私有字段：保存要导入的目标目录路径（_directory）

    private bool _ignoreListEvents; // 私有字段：用于在程序内部更新列表索引时忽略列表选择事件（防止循环触发）

    public List<ImportBatchDataGridItem> DataGridItems { get; set; } // 公共属性：绑定到 UI 的数据网格项集合（DataGridItems）

    [ObservableProperty] // 特性：自动生成属性与通知（将生成 MatchingFilesItems 属性）
    public List<string> _matchingFilesItems; // 字段：匹配到的文件名列表（MatchingFilesItems 的后备字段）

    [ObservableProperty] // 特性：自动生成属性与通知（将生成 DataGridSelectedItem 属性）
    public object? _dataGridSelectedItem; // 字段：当前数据网格选中的项（DataGridSelectedItem 的后备字段）

    [ObservableProperty] // 特性：自动生成属性与通知（将生成 MatchingFilesSelectedIndex 属性）
    public int _matchingFilesSelectedIndex; // 字段：匹配文件列表中当前选中索引（MatchingFilesSelectedIndex 的后备字段）

    public string Title => "批量导入 (Batch Import)"; // 属性：对话框标题，中文显示并在括号保留英文原名（Title）

    public int Width => 700; // 属性：对话框宽度（像素）（Width）

    public int Height => 350; // 属性：对话框高度（像素）（Height）

    public event Action<List<ImportBatchInfo>?>? RequestClose; // 事件：请求关闭对话框并返回导入信息列表或 null（RequestClose）

    [Obsolete("This constructor is for the designer only and should not be used directly.", true)] // 标记：此构造函数仅供设计器使用，不应直接调用（保留英文说明）
    public BatchImportViewModel() // 无参构造函数（仅供设计器）
    {
        _directory = string.Empty; // 初始化目录为空字符串，避免 null

        DataGridItems = new List<ImportBatchDataGridItem>(); // 初始化数据网格项集合为空列表

        MatchingFilesItems = new List<string>(); // 初始化匹配文件列表为空列表
    }

    public BatchImportViewModel(Workspace workspace, List<AssetInst> selection, string directory,
        List<string> extensions) // 构造函数：运行时使用，接收工作区、选中资产、目录与扩展名列表
    {
        _directory = directory; // 保存传入的目录路径到私有字段

        var anyExtension = extensions.Contains("*"); // 判断是否包含通配符 "*"（表示接受任意扩展名）

        List<string> filesInDir; // 局部变量：保存目录下的文件路径列表
        if (!anyExtension) // 如果不是任意扩展名
            filesInDir = FileUtils.GetFilesInDirectory(directory, extensions); // 使用 FileUtils 获取指定扩展名的文件
        else
            filesInDir = Directory.GetFiles(directory).ToList(); // 否则获取目录下所有文件

        List<ImportBatchDataGridItem> gridItems = new(); // 创建临时列表用于收集网格项
        int maxNameLen = ConfigurationManager.Settings.ExportNameLength; // 从配置读取导出文件名的最大长度

        foreach (var asset in selection) // 遍历传入的选中资产集合
        {
            var assetName = workspace.Namer.GetAssetName(asset, true, maxNameLen); // 使用工作区的命名器生成资产名（考虑长度限制）
            assetName = AssetNamer.GetFallbackName(asset, assetName); // 如果生成失败则使用回退命名

            var gridItem = new ImportBatchDataGridItem( // 创建一个新的网格项，包装 ImportBatchInfo
                new ImportBatchInfo(
                    asset, Path.GetFileName(asset.FileInstance.path), assetName, asset.PathId)
            );

            List<string> matchingFiles; // 局部变量：保存与该资产匹配的文件名（不含路径）
            if (!anyExtension) // 如果不是任意扩展名
            {
                matchingFiles = filesInDir
                    .Where(f => extensions.Any(x => f.EndsWith(gridItem.GetMatchName(x)))) // 通过扩展名和匹配规则筛选文件
                    .Select(f => Path.GetFileName(f)!).ToList(); // 取文件名并转换为列表
            }
            else // 如果是任意扩展名
            {
                matchingFiles = filesInDir
                    .Where(f => PathUtils.GetFilePathWithoutExtension(f).EndsWith(gridItem.GetMatchName("*"))) // 使用不带扩展名的路径进行匹配
                    .Select(f => Path.GetFileName(f)!).ToList(); // 取文件名并转换为列表
            }

            gridItem.MatchingFiles = matchingFiles; // 将匹配到的文件名列表赋给网格项
            gridItem.SelectedIndex = matchingFiles.Count > 0 ? 0 : -1; // 如果有匹配则默认选中第一个，否则设为 -1（未选中）
            if (gridItem.MatchingFiles.Count > 0) // 仅当存在匹配文件时才将该网格项加入最终列表
                gridItems.Add(gridItem); // 将网格项加入临时集合
        }

        DataGridItems = gridItems; // 将构建好的网格项集合赋值给公开属性，供 UI 绑定
        MatchingFilesItems = new List<string>(); // 初始化匹配文件显示列表为空（等待用户选择某一行时填充）
    }

    partial void OnDataGridSelectedItemChanged(object? value) // 部分方法：当 DataGridSelectedItem 由生成的 ObservableProperty 改变时被调用
    {
        if (value is ImportBatchDataGridItem gridItem) // 如果选中项是 ImportBatchDataGridItem
        {
            MatchingFilesItems = gridItem.MatchingFiles; // 将该行的匹配文件列表赋给 MatchingFilesItems（用于显示右侧列表）
            if (gridItem.SelectedIndex != -1) // 如果该行已有选中索引
            {
                //there's gotta be a better way to do this .-. oh well
                // 注释：作者自嘲，这里用标志位避免事件循环
                _ignoreListEvents = true; // 设置标志以忽略随后触发的列表选择事件
                MatchingFilesSelectedIndex = gridItem.SelectedIndex; // 将右侧列表的选中索引同步为该行的 SelectedIndex
                _ignoreListEvents = false; // 取消忽略标志
            }
        }
    }

    partial void OnMatchingFilesSelectedIndexChanged(int value) // 部分方法：当 MatchingFilesSelectedIndex 改变时被调用
    {
        if (DataGridSelectedItem is ImportBatchDataGridItem gridItem && !_ignoreListEvents) // 如果当前有选中网格项且不在忽略事件阶段
        {
            gridItem.SelectedIndex = value; // 将网格项的 SelectedIndex 更新为右侧列表的选中索引
        }
    }

    public void BtnOk_Click() // 公共方法：当用户点击“确定”按钮时调用，收集所有已选择的导入信息并关闭对话框
    {
        List<ImportBatchInfo> importInfos = new List<ImportBatchInfo>(); // 创建列表用于收集要返回的 ImportBatchInfo
        foreach (ImportBatchDataGridItem gridItem in DataGridItems) // 遍历所有网格项
        {
            if (gridItem.SelectedIndex != -1) // 仅处理已选择匹配文件的项
            {
                ImportBatchInfo importInfo = gridItem.importInfo; // 获取该项对应的 ImportBatchInfo
                importInfo.ImportFile = Path.Combine(_directory, gridItem.MatchingFiles[gridItem.SelectedIndex]); // 将选中的文件名与目录拼接为完整路径并赋值给 ImportFile
                importInfos.Add(importInfo); // 将该导入信息加入返回列表
            }
        }

        RequestClose?.Invoke(importInfos); // 触发 RequestClose 事件并传回收集到的导入信息列表（关闭对话框）
    }

    public void BtnCancel_Click() // 公共方法：当用户点击“取消”按钮时调用
    {
        RequestClose?.Invoke(null); // 触发 RequestClose 事件并传回 null（表示取消）
    }
} // BatchImportViewModel 类结束

public class ImportBatchInfo // 公共类：表示单个导入任务的信息（ImportBatchInfo）
{
    public readonly AssetInst Asset; // 只读字段：目标资产实例（Asset）
    public readonly string AssetFile; // 只读字段：资产所属文件名（AssetFile）
    public readonly string AssetName; // 只读字段：资产显示名称（AssetName）
    public readonly long PathId; // 只读字段：资产在文件中的 PathId（PathId）
    public string? ImportFile; // 可变字段：用户选择的导入文件完整路径（ImportFile）

    public ImportBatchInfo(AssetInst asset, string assetFile, string assetName, long pathId) // 构造函数：初始化 ImportBatchInfo
    {
        Asset = asset; // 赋值 Asset
        AssetFile = assetFile; // 赋值 AssetFile
        AssetName = assetName; // 赋值 AssetName
        PathId = pathId; // 赋值 PathId
    }
} // ImportBatchInfo 类结束

public class ImportBatchDataGridItem : INotifyPropertyChanged // 公共类：用于数据网格显示的包装项，实现属性变更通知（ImportBatchDataGridItem）
{
    public event PropertyChangedEventHandler? PropertyChanged; // 事件：属性变更通知（PropertyChanged）

    public ImportBatchInfo importInfo; // 字段：关联的 ImportBatchInfo（importInfo）

    public List<string> MatchingFiles = new(); // 字段：该项匹配到的文件名列表（MatchingFiles）

    public int SelectedIndex; // 字段：该项在 MatchingFiles 中的选中索引（SelectedIndex）

    public string Description => importInfo.AssetName; // 只读属性：用于 UI 显示的描述（资产名称）（Description）

    public string File => importInfo.AssetFile; // 只读属性：用于 UI 显示的文件名（File）

    public long PathId => importInfo.PathId; // 只读属性：用于 UI 显示的 PathId（PathId）

    public ImportBatchDataGridItem(ImportBatchInfo importInfo) // 构造函数：用 ImportBatchInfo 初始化网格项
    {
        this.importInfo = importInfo; // 保存传入的 importInfo 到字段
    }

    public string GetMatchName(string ext) // 公共方法：根据扩展名构造匹配规则字符串（GetMatchName）
    {
        if (ext != "*") // 如果扩展名不是通配符
            return $"-{File}-{PathId}.{ext}"; // 返回带扩展名的匹配后缀格式，例如 "-File-123.ext"

        return $"-{File}-{PathId}"; // 如果是通配符，返回不带扩展名的匹配后缀
    }

    public void Update(string propertyName = "") // 公共方法：触发属性变更通知以刷新 UI（Update）
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // 如果有订阅者则触发 PropertyChanged 事件
    }
} // ImportBatchDataGridItem 类结束
