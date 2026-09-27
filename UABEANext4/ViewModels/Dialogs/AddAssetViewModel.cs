using AssetsTools.NET; // 引用 AssetsTools.NET 库，用于处理 Unity 资产文件的底层 API（保留原名 AssetsTools.NET）

using AssetsTools.NET.Extra; // 引用 AssetsTools.NET 的扩展功能（保留原名 AssetsTools.NET.Extra）

using AvaloniaEdit.Utils; // 引用 AvaloniaEdit 的工具类（例如 DispatcherTimer 等）（保留原名 AvaloniaEdit.Utils）

using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型，提供 ObservableProperty 等 MVVM 特性（保留原名 CommunityToolkit.Mvvm.ComponentModel）

using System; // 引用基础系统命名空间，提供基本类型与异常等（保留原名 System）

using System.Collections.Generic; // 引用泛型集合命名空间，提供 List、Dictionary 等（保留原名 System.Collections.Generic）

using System.Collections.ObjectModel; // 引用可观察集合类型（ObservableCollection），用于 UI 绑定（保留原名 System.Collections.ObjectModel）

using System.ComponentModel.DataAnnotations; // 引用数据注解，用于属性验证（保留原名 System.ComponentModel.DataAnnotations）

using System.Linq; // 引用 LINQ 扩展方法，用于集合查询与转换（保留原名 System.Linq）

using System.Threading.Tasks; // 引用异步任务支持（Task、async/await）（保留原名 System.Threading.Tasks）

using UABEANext4.AssetWorkspace; // 引用项目的资产工作区命名空间（Workspace、AssetsFileInstance 等）（保留原名 UABEANext4.AssetWorkspace）

using UABEANext4.Interfaces; // 引用项目接口命名空间，包含 IDialogAware 等接口（保留原名 UABEANext4.Interfaces）

using UABEANext4.Util; // 引用项目工具类命名空间（MessageBoxUtil、AssetHelper 等）（保留原名 UABEANext4.Util）

namespace UABEANext4.ViewModels.Dialogs; // 定义命名空间 UABEANext4.ViewModels.Dialogs（保留原名）

public partial class AddAssetViewModel : ViewModelBaseValidator, IDialogAware<AddAssetResult> // 定义部分类 AddAssetViewModel，继承自 ViewModelBaseValidator 并实现 IDialogAware<AddAssetResult>（保留原名）
{
    private Workspace _workspace; // 私有字段：保存传入的 Workspace 实例，用于访问管理器与文件（_workspace）

    private Dictionary<AssetsFileInstance, List<string>> _scriptLookup; // 私有字段：文件实例到脚本描述列表的查找表（_scriptLookup）

    [ObservableProperty] // 特性：由 CommunityToolkit 自动生成公开属性 Files（以及变更通知）
    public List<AssetsFileInstance> _files = new(); // 后备字段：Files 的初始值为空列表（用于下拉选择文件）

    [ObservableProperty] // 特性：由 CommunityToolkit 自动生成公开属性 Scripts（以及变更通知）
    public ObservableCollection<string> _scripts = new(); // 后备字段：Scripts 的初始值为空可观察集合（用于显示脚本选项）

    [ObservableProperty] // 特性：生成 SelectedFile 属性的后备字段与通知
    public AssetsFileInstance? _selectedFile; // 后备字段：当前选中的文件实例（可为空）

    [ObservableProperty] // 特性：生成 PathIdString 属性并启用验证
    [CustomValidation(typeof(AddAssetViewModel), nameof(ValidatePathId))] // 特性：对 PathIdString 使用自定义验证方法 ValidatePathId
    public string _pathIdString = ""; // 后备字段：PathId 的字符串表示（用于用户输入）

    [ObservableProperty] // 特性：生成 TypeNameOrId 属性并启用验证
    [CustomValidation(typeof(AddAssetViewModel), nameof(ValidateTypeNameOrId))] // 特性：对 TypeNameOrId 使用自定义验证方法 ValidateTypeNameOrId
    public string _typeNameOrId = ""; // 后备字段：类型名或类型 ID 的文本输入

    [ObservableProperty] // 特性：生成 SelectedScriptIndex 属性
    public int _selectedScriptIndex = 0; // 后备字段：选中的脚本索引（默认 0）

    [ObservableProperty] // 特性：生成 IsScript 属性
    public bool _isScript = false; // 后备字段：指示当前类型是否为脚本（用于显示脚本选择控件）

    public string Title => "添加资产 (Add Asset)"; // 对话框标题：中文显示“添加资产”，括号中保留英文原名（Title）

    public int Width => 300; // 对话框宽度（像素）（Width）

    public int Height => 170; // 对话框高度（像素）（Height）

    public event Action<AddAssetResult?>? RequestClose; // 事件：请求关闭对话框并返回 AddAssetResult 或 null（RequestClose）

    public AddAssetViewModel(Workspace workspace, List<AssetsFileInstance> fileInsts) // 构造函数：接收 Workspace 与文件实例列表用于初始化
    {
        _workspace = workspace; // 保存传入的 Workspace 实例到私有字段

        _scriptLookup = new(); // 初始化脚本查找表为空字典

        foreach (var fileInst in fileInsts) // 遍历传入的每个文件实例
        {
            Files.Add(fileInst); // 将文件实例加入 Files 列表（用于 UI 下拉）

            var scriptList = new List<string>(); // 为当前文件准备脚本描述列表

            // this will skip script references that won't match. that's probably fine
            // since later analysis requires we know what we have loaded.
            // 注释：作者说明：这里会跳过无法匹配的脚本引用，这是可以接受的

            var scriptInfos = AssetHelper.GetAssetsFileScriptInfos(workspace.Manager, fileInst); // 获取该文件的脚本信息字典（键为索引或标识，值为 AssetTypeReference）

            foreach (var scriptInfo in scriptInfos) // 遍历脚本信息字典
            {
                scriptList.Add($"{scriptInfo.Key} - {GetTypeRefFullName(scriptInfo.Value!)}"); // 将脚本索引与脚本全名拼接为描述并加入列表
            }

            _scriptLookup[fileInst] = scriptList; // 将该文件对应的脚本描述列表保存到查找表
        }

        if (fileInsts.Count > 0) // 如果传入的文件列表非空
        {
            SelectedFile = fileInsts.First(); // 默认选择第一个文件实例
        }
    }

    public static ValidationResult? ValidatePathId(string pathIdStr, ValidationContext context) // 静态方法：验证 PathIdString 的有效性（ValidatePathId）
    {
        if (context.ObjectInstance is not AddAssetViewModel vm) // 从 ValidationContext 获取视图模型实例
        {
            throw new Exception("View model not found"); // 如果无法获取则抛出异常
        }

        if (!long.TryParse(pathIdStr, out var pathId)) // 尝试将字符串解析为 long
        {
            return new("Path ID must be a long"); // 解析失败则返回验证错误（英文原文）
        }

        if (pathId == 0) // PathId 不能为 0
        {
            return new("Zero is not a valid path ID"); // 返回验证错误（英文原文）
        }

        var info = vm.SelectedFile?.file.GetAssetInfo(pathId); // 在选中文件中查找是否已有相同 PathId 的资产
        if (info != null) // 如果已存在
        {
            return new("Path ID already exists in file"); // 返回验证错误（英文原文）
        }

        return ValidationResult.Success; // 验证通过
    }

    public static ValidationResult? ValidateTypeNameOrId(string typeStr, ValidationContext context) // 静态方法：验证类型名或 ID（ValidateTypeNameOrId）
    {
        if (context.ObjectInstance is not AddAssetViewModel vm) // 获取视图模型实例
        {
            throw new Exception("View model not found"); // 未找到则抛出异常
        }

        if (!vm.TryParseTypeId(typeStr, false, out _, out _)) // 尝试解析类型（不创建模板）
        {
            return new("Class ID must be an int or a valid class name"); // 解析失败返回验证错误（英文原文）
        }

        return ValidationResult.Success; // 验证通过
    }

    // yes, we double parse to enable/disable the script field. oh well.
    // 注释：作者说明：确实会重复解析以决定是否启用脚本字段

    partial void OnTypeNameOrIdChanged(string value) // 部分方法：当 TypeNameOrId 改变时由生成代码调用
    {
        if (TryParseTypeId(value, false, out _, out var typeId)) // 尝试解析类型 ID（不创建模板）
        {
            IsScript = typeId < 0 || typeId == (int)AssetClassID.MonoBehaviour; // 如果 typeId 小于 0 或等于 MonoBehaviour，则视为脚本类型
        }
        else
        {
            IsScript = false; // 解析失败则不是脚本
        }
    }

    partial void OnSelectedFileChanged(AssetsFileInstance? value) // 部分方法：当 SelectedFile 改变时由生成代码调用
    {
        if (value == null) // 如果新值为 null
        {
            return; // 直接返回
        }

        Scripts.Clear(); // 清空当前 Scripts 列表

        if (_scriptLookup.TryGetValue(value, out var scripts)) // 如果查找表中存在该文件的脚本列表
        {
            Scripts.AddRange(scripts); // 将脚本描述加入 Scripts（用于 UI 显示）
            SelectedScriptIndex = -1; // 先设置为 -1（触发变更）
            SelectedScriptIndex = 0; // 再设置为 0（默认选中第一个）
        }
    }

    private bool TryParseTypeId(string typeIdText, bool creating, out AssetTypeTemplateField? tempField, out int typeId) // 私有方法：尝试解析类型 ID 或类型名，可能基于 TypeTree 或 ClassDatabase
    {
        if (SelectedFile == null) // 如果没有选中文件
        {
            tempField = null; // 无模板字段
            typeId = -1; // 无效 typeId
            return false; // 返回失败
        }

        if (SelectedFile.file.Metadata.TypeTreeEnabled) // 如果文件启用了 TypeTree（较新 Unity 版本）
        {
            if (!TryParseTypeIdByTypeTree(SelectedFile, typeIdText, creating, out tempField, out typeId)) // 先尝试通过 TypeTree 解析
            {
                if (!TryParseTypeIdByClassDatabase(typeIdText, creating, out tempField, out typeId)) // 若失败再尝试通过 ClassDatabase 解析
                {
                    return false; // 两者都失败则返回 false
                }
            }
        }
        else // 如果没有 TypeTree
        {
            if (!TryParseTypeIdByClassDatabase(typeIdText, creating, out tempField, out typeId)) // 仅通过 ClassDatabase 解析
            {
                tempField = null; // 失败则清空
                typeId = -1; // 无效
                return false; // 返回失败
            }
        }

        return true; // 解析成功
    }

    private bool TryParseTypeIdByClassDatabase(string typeIdText, bool creating, out AssetTypeTemplateField? tempField, out int typeId) // 私有方法：通过 ClassDatabase 解析类型
    {
        tempField = null; // 初始化输出参数

        ClassDatabaseFile? cldb = (ClassDatabaseFile?)_workspace.Manager.ClassDatabase; // 从 Workspace.Manager 获取 ClassDatabase
        if (cldb == null) // 如果没有 ClassDatabase
        {
            typeId = -1; // 无效
            return false; // 返回失败
        }

        ClassDatabaseType cldbType; // 局部变量：ClassDatabaseType
        bool needsTypeId; // 标记：是否需要从类型名推断 typeId

        if (int.TryParse(typeIdText, out typeId)) // 如果输入可以解析为整数
        {
            cldbType = cldb.FindAssetClassByID(typeId); // 通过 ID 查找类型
            needsTypeId = false; // 不需要再推断 ID
        }
        else
        {
            cldbType = cldb.FindAssetClassByName(typeIdText); // 通过名称查找类型
            needsTypeId = true; // 需要从找到的类型获取 ID
        }

        if (cldbType == null) // 如果未找到类型
        {
            return false; // 返回失败
        }

        if (needsTypeId) // 如果需要设置 typeId
        {
            typeId = cldbType.ClassId; // 从 ClassDatabaseType 获取 ClassId
        }

        if (creating) // 如果调用方要求创建模板字段
        {
            tempField = new AssetTypeTemplateField(); // 创建模板字段实例
            tempField.FromClassDatabase(cldb, cldbType); // 从 ClassDatabase 填充模板字段
        }
        return true; // 成功解析
    }

    private bool TryParseTypeIdByTypeTree(AssetsFileInstance file, string typeIdText, bool creating, out AssetTypeTemplateField? tempField, out int typeId) // 私有方法：通过 TypeTree 解析类型
    {
        tempField = null; // 初始化输出参数

        AssetsFileMetadata meta = file.file.Metadata; // 获取文件的 Metadata
        TypeTreeType ttType; // 局部变量：TypeTreeType
        bool needsTypeId; // 标记：是否需要从名称推断 ID

        if (int.TryParse(typeIdText, out typeId)) // 如果输入为整数
        {
            ttType = meta.FindTypeTreeTypeByID(typeId); // 通过 ID 查找 TypeTreeType
            needsTypeId = false; // 不需要推断
        }
        else
        {
            ttType = meta.FindTypeTreeTypeByName(typeIdText); // 通过名称查找 TypeTreeType
            needsTypeId = true; // 需要推断 ID
        }

        if (ttType == null) // 如果未找到
        {
            return false; // 返回失败
        }

        if (needsTypeId) // 如果需要设置 typeId
        {
            typeId = ttType.TypeId; // 从 TypeTreeType 获取 TypeId
        }

        if (creating) // 如果需要创建模板字段
        {
            tempField = new AssetTypeTemplateField(); // 创建模板字段
            tempField.FromTypeTree(ttType); // 从 TypeTree 填充模板字段
        }
        return true; // 成功解析
    }

    private static string GetTypeRefFullName(AssetTypeReference typeRef) // 私有静态方法：获取 AssetTypeReference 的完整类型名（命名空间.类名）
    {
        var nameSpace = typeRef.Namespace; // 获取命名空间
        var className = typeRef.ClassName; // 获取类名

        if (nameSpace != "") // 如果命名空间非空
        {
            return $"{nameSpace}.{className}"; // 返回 "命名空间.类名"
        }
        else
        {
            return className; // 否则仅返回类名
        }
    }

    public async void BtnOk_Click() // 公共方法：当用户点击“确定”按钮时调用，验证并构造 AddAssetResult 返回
    {
        if (SelectedFile == null) // 如果没有选中文件
        {
            await ShowInvalidOptionsBox(); // 显示无效选项提示
            return; // 退出
        }

        var pathIdSuccess = long.TryParse(PathIdString, out var pathId); // 尝试解析 PathIdString 为 long
        if (!pathIdSuccess) // 解析失败
        {
            await ShowInvalidOptionsBox(); // 显示错误
            return; // 退出
        }

        var typeIdSuccess = TryParseTypeId(TypeNameOrId, true, out var tempField, out var typeId); // 尝试解析类型并创建模板（creating = true）
        if (!typeIdSuccess) // 解析失败
        {
            await ShowInvalidOptionsBox(); // 显示错误
            return; // 退出
        }

        var scriptIndex = typeId < 0 || typeId == (int)AssetClassID.MonoBehaviour ? (ushort)SelectedScriptIndex : ushort.MaxValue; // 如果是脚本类型则使用选中的脚本索引，否则使用 ushort.MaxValue 表示无脚本
        var result = new AddAssetResult(SelectedFile, pathId, typeId, scriptIndex, tempField); // 构造 AddAssetResult 实例
        RequestClose?.Invoke(result); // 触发 RequestClose 事件并传回结果，关闭对话框
    }

    private async Task ShowInvalidOptionsBox() // 私有异步方法：显示“无效选项”错误对话框
    {
        await MessageBoxUtil.ShowDialog("错误 (Error)", "提供的选项无效。 (Invalid options provided.)"); // 弹出对话框（中文 + 英文原文）
    }

    public void BtnCancel_Click() // 公共方法：当用户点击“取消”按钮时调用
    {
        RequestClose?.Invoke(null); // 触发 RequestClose 并传回 null（表示取消）
    }
} // AddAssetViewModel 类结束

public class AddAssetResult // 公共类：封装添加资产操作的结果数据
{
    public AssetsFileInstance File { get; } // 只读属性：目标文件实例（File）

    public AssetTypeTemplateField? TempField { get; } // 只读属性：可选的模板字段（TempField），用于创建新资产时的默认值

    public long PathId { get; } // 只读属性：资产的 PathId（PathId）

    public int TypeId { get; } // 只读属性：资产的类型 ID（TypeId）

    public ushort ScriptIndex { get; } // 只读属性：脚本索引（ScriptIndex），如果不是脚本则为 ushort.MaxValue

    public AddAssetResult( // 构造函数：初始化 AddAssetResult 的所有字段
        AssetsFileInstance file, long pathId, int typeId,
        ushort scriptIndex, AssetTypeTemplateField? tempField = null)
    {
        File = file; // 赋值 File
        PathId = pathId; // 赋值 PathId
        TypeId = typeId; // 赋值 TypeId
        ScriptIndex = scriptIndex; // 赋值 ScriptIndex
        TempField = tempField; // 赋值 TempField（可为 null）
    }
} // AddAssetResult 类结束
