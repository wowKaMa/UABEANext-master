using AssetsTools.NET; // 引用 AssetsTools.NET 库，用于处理 Unity 资产文件的底层 API（保留原始英文名称 AssetsTools.NET）

using AssetsTools.NET.Extra; // 引用 AssetsTools.NET 的扩展功能（保留原始英文名称 AssetsTools.NET.Extra）

using CommunityToolkit.Mvvm.ComponentModel; // 引用 CommunityToolkit.Mvvm 的组件模型，提供 ObservableProperty 等 MVVM 特性（保留原始英文名称 CommunityToolkit.Mvvm.ComponentModel）

using DynamicData; // 引用 DynamicData 库，用于响应式集合与数据流（保留原始英文名称 DynamicData）

using System; // 引用基础系统命名空间，提供基本类型与工具（保留原始英文名称 System）

using System.Collections.Generic; // 引用泛型集合命名空间（List、Dictionary 等）（保留原始英文名称 System.Collections.Generic）

using System.Collections.ObjectModel; // 引用可观察集合类型（ObservableCollection），用于 UI 绑定（保留原始英文名称 System.Collections.ObjectModel）

using System.Linq; // 引用 LINQ 扩展方法，用于集合查询与转换（保留原始英文名称 System.Linq）

using UABEANext4.AssetWorkspace; // 引用项目的资产工作区命名空间（Workspace、AssetInst 等）（保留原始英文名称 UABEANext4.AssetWorkspace）

using UABEANext4.Interfaces; // 引用项目接口命名空间，包含 IDialogAware 接口（保留原始英文名称 UABEANext4.Interfaces）

namespace UABEANext4.ViewModels.Dialogs; // 定义命名空间 UABEANext4.ViewModels.Dialogs，用于组织对话框相关的视图模型类（保留原始英文名称）

public partial class SelectTypeFilterViewModel : ViewModelBase, IDialogAware<IEnumerable<TypeFilterTypeEntry>?> // 定义部分类 SelectTypeFilterViewModel，继承 ViewModelBase 并实现 IDialogAware，返回类型为 IEnumerable<TypeFilterTypeEntry>?（保留原始英文名称）
{
    [ObservableProperty] // 特性：由 CommunityToolkit 自动生成属性与通知（将生成 FilterTypes 属性的封装）
    public ObservableCollection<TypeFilterTypeEntry> _filterTypes = []; // 字段：FilterTypes 的后备字段，保存类型过滤项集合（初始化为空集合）

    public string Title => "选择类型过滤器 (Select Type Filter)"; // 属性：对话框标题，中文显示并在括号保留英文原名（Title）

    public int Width => 300; // 属性：对话框宽度（像素）（Width）

    public int Height => 500; // 属性：对话框高度（像素）（Height）

    public event Action<IEnumerable<TypeFilterTypeEntry>?>? RequestClose; // 事件：请求关闭对话框时触发，参数为选中的过滤类型集合或 null（RequestClose）

    public SelectTypeFilterViewModel(List<TypeFilterTypeEntry> filterTypes) // 构造函数：接收初始的类型过滤项列表（SelectTypeFilterViewModel）
    {
        FilterTypes.AddRange(filterTypes); // 将传入的 filterTypes 添加到 FilterTypes 集合（初始化 UI 数据源）
    }

    public void SelectAll() // 方法：将所有过滤项标记为已选（SelectAll）
    {
        foreach (var filterType in FilterTypes) // 遍历 FilterTypes 集合中的每一项
        {
            filterType.IsSelected = true; // 将当前过滤项的 IsSelected 设为 true（全选）
        }
    }

    public void DeselectAll() // 方法：取消选择所有过滤项（DeselectAll）
    {
        foreach (var filterType in FilterTypes) // 遍历 FilterTypes 集合中的每一项
        {
            filterType.IsSelected = false; // 将当前过滤项的 IsSelected 设为 false（全不选）
        }
    }

    public void Accept() // 方法：确认选择并关闭对话框，返回选中的过滤类型（Accept）
    {
        bool includeAllMonoBehaviours = false; // 局部变量：是否包含所有 MonoBehaviour（默认 false）

        var monoBehaviourType = FilterTypes.FirstOrDefault(ft => ft.TypeId == 0x72 && ft.ScriptRef is null); // 查找 TypeId 为 0x72（MonoBehaviour）且 ScriptRef 为 null 的条目（表示“所有 MonoBehaviour”项）
        if (monoBehaviourType is not null) // 如果找到了该条目
            includeAllMonoBehaviours = monoBehaviourType.IsSelected; // 根据该条目的 IsSelected 决定是否包含所有 MonoBehaviour

        IEnumerable<TypeFilterTypeEntry> filteredTypes; // 局部变量：保存最终要返回的过滤类型集合
        if (includeAllMonoBehaviours) // 如果包含所有 MonoBehaviour
            filteredTypes = FilterTypes.Where(ft => ft.IsSelected || ft.ScriptRef is not null); // 返回所有被选中或有 ScriptRef（具体脚本）的项
        else
            filteredTypes = FilterTypes.Where(ft => ft.IsSelected); // 否则仅返回被选中的项

        RequestClose?.Invoke(filteredTypes); // 触发 RequestClose 事件并传递过滤后的集合（关闭对话框并返回结果）
    }

    public void Cancel() // 方法：取消并关闭对话框（Cancel）
    {
        RequestClose?.Invoke(null); // 触发 RequestClose 事件并传递 null（表示取消）
    }

    public static List<TypeFilterTypeEntry> MakeTypeFilterTypes(Workspace workspace, IList<AssetInst> assets) // 静态方法：根据 Workspace 和资产列表生成类型过滤项集合（MakeTypeFilterTypes）
    {
        var uniqueTypeIds = new HashSet<int>(); // 集合：保存唯一的类型 ID（非 MonoBehaviour 或无脚本绑定的脚本）
        var uniqueMonoIdPairs = new Dictionary<AssetsFileInstance, HashSet<ushort>>(); // 字典：对每个文件实例保存其出现的 MonoBehaviour 脚本索引集合
        var uniqueScriptPtrs = new HashSet<AssetPPtr>(); // 集合：保存唯一的脚本指针（AssetPPtr），用于去重跨文件的脚本引用
        foreach (var asset in assets) // 遍历传入的资产列表
        {
            var typeId = asset.TypeId; // 获取资产的 TypeId（整数）
            // if a non-script or a script with no script binding, treat as regular type
            // 注释：如果是非脚本或脚本但没有脚本绑定，则视为普通类型
            if (typeId != (int)AssetClassID.MonoBehaviour && typeId >= 0) // 如果不是 MonoBehaviour 且 TypeId 非负
            {
                uniqueTypeIds.Add(typeId); // 将该 TypeId 加入唯一集合
            }
            else // 否则处理 MonoBehaviour 或特殊情况
            {
                var assetFileInst = asset.FileInstance; // 获取资产所属的文件实例
                var scriptIndex = asset.GetScriptIndex(assetFileInst.file); // 获取该资产在文件中的脚本索引（如果有）
                if (scriptIndex == ushort.MaxValue) // 如果脚本索引无效（表示没有脚本绑定）
                {
                    uniqueTypeIds.Add(typeId); // 将原始 TypeId 加入唯一集合（作为普通类型处理）
                }
                else
                {
                    // a bit slow but I don't know what else to do
                    // 注释：下面的处理可能较慢，但用于收集每个文件中出现的脚本索引
                    if (!uniqueMonoIdPairs.TryGetValue(assetFileInst, out HashSet<ushort>? value)) // 如果字典中还没有该文件的条目
                        uniqueMonoIdPairs[assetFileInst] = value = []; // 创建一个新的 HashSet 并加入字典

                    uniqueTypeIds.Add(0x72); // 将 MonoBehaviour 的通用 TypeId (0x72) 加入唯一类型集合
                    value.Add(scriptIndex); // 将脚本索引加入该文件对应的集合
                }
            }
        }

        var filterTypes = new List<TypeFilterTypeEntry>(); // 列表：用于保存生成的 TypeFilterTypeEntry 项
        foreach (var uniqueTypeId in uniqueTypeIds) // 遍历所有唯一的类型 ID
        {
            filterTypes.Add(TypeFilterTypeEntry.FromTypeId(uniqueTypeId)); // 使用 FromTypeId 创建条目并加入列表
        }

        // deduplicate mono ids by converting them to global pptrs
        // 注释：通过将文件内的脚本索引转换为全局 AssetPPtr 来对 MonoBehaviour 脚本去重
        foreach (var uniqueMonoIdPair in uniqueMonoIdPairs) // 遍历每个文件实例及其脚本索引集合
        {
            var uniqueMonoIdFile = uniqueMonoIdPair.Key; // 当前文件实例
            var uniqueMonoIds = uniqueMonoIdPair.Value; // 该文件中出现的脚本索引集合
            foreach (var uniqueMonoId in uniqueMonoIds) // 遍历每个脚本索引
            {
                var scriptPtr = uniqueMonoIdFile.file.Metadata.ScriptTypes[uniqueMonoId]; // 从文件的 Metadata.ScriptTypes 获取对应的 AssetPPtr（脚本指针）
                scriptPtr.SetFilePathFromFile(workspace.Manager, uniqueMonoIdFile); // 设置脚本指针的文件路径以便全局唯一性比较

                // SetFilePathFromFile changes hash method to file name
                // rather than file id, so this is fine.
                // 注释：SetFilePathFromFile 会改变哈希方法以文件名为依据，这里是可接受的
                uniqueScriptPtrs.Add(scriptPtr); // 将脚本指针加入去重集合
            }
        }

        foreach (var uniqueScriptPtr in uniqueScriptPtrs) // 遍历所有唯一的脚本指针
        {
            var scriptTypeRef = GetAssetsFileScriptInfo(workspace.Manager, uniqueScriptPtr); // 通过全局脚本指针获取脚本的详细信息（AssetTypeReference）
            if (scriptTypeRef is not null) // 如果成功获取到脚本信息
            {
                filterTypes.Add(TypeFilterTypeEntry.FromTypeReference(scriptTypeRef)); // 使用 FromTypeReference 创建条目并加入 filterTypes 列表
            }
        }

        var filterTypesSorted = filterTypes // 对生成的过滤类型列表进行排序
            .OrderBy(a => a.ScriptRef is not null) // 先按是否有 ScriptRef 排序（无 ScriptRef 的先或后，取决于布尔排序）
            .ThenBy(a => a.DisplayText) // 再按显示文本字母序排序
            .ToList(); // 转换为列表

        return filterTypesSorted; // 返回排序后的过滤类型列表
    }

    // get script info from global assetpptr rather than script index
    // 注释：下面的方法通过全局 AssetPPtr 获取脚本信息，而不是依赖文件内的脚本索引
    private static AssetTypeReference? GetAssetsFileScriptInfo(AssetsManager manager, AssetPPtr assetPtr) // 私有静态方法：根据 AssetPPtr 获取 AssetTypeReference（GetAssetsFileScriptInfo）
    {
        if (string.IsNullOrEmpty(assetPtr.FilePath)) // 如果 AssetPPtr 的 FilePath 为空或 null
            return null; // 返回 null（无法定位文件）

        AssetTypeValueField msBaseField; // 局部变量：用于保存反序列化得到的 baseField
        try
        {
            var fileInst = manager.FileLookup[AssetsManager.GetFileLookupKey(assetPtr.FilePath)]; // 通过 manager 的 FileLookup 根据文件路径查找文件实例
            msBaseField = manager.GetExtAsset(fileInst, 0, assetPtr.PathId).baseField; // 使用 GetExtAsset 获取扩展资产并读取 baseField
            if (msBaseField == null) // 如果 baseField 为 null
                return null; // 返回 null
        }
        catch // 捕获任何异常（例如文件未找到或解析失败）
        {
            return null; // 返回 null 表示无法获取脚本信息
        }

        AssetTypeValueField assemblyNameField = msBaseField["m_AssemblyName"]; // 从 baseField 中读取 m_AssemblyName 字段
        AssetTypeValueField nameSpaceField = msBaseField["m_Namespace"]; // 读取 m_Namespace 字段
        AssetTypeValueField classNameField = msBaseField["m_ClassName"]; // 读取 m_ClassName 字段
        if (assemblyNameField.IsDummy || nameSpaceField.IsDummy || classNameField.IsDummy) // 如果任一字段为占位（IsDummy）
            return null; // 返回 null（字段不可用）

        string assemblyName = assemblyNameField.AsString; // 将 assemblyNameField 转为字符串
        string nameSpace = nameSpaceField.AsString; // 将 nameSpaceField 转为字符串
        string className = classNameField.AsString; // 将 classNameField 转为字符串

        AssetTypeReference info = new AssetTypeReference(className, nameSpace, assemblyName); // 使用类名、命名空间、程序集名构造 AssetTypeReference
        return info; // 返回构造好的脚本类型引用信息
    }
} // SelectTypeFilterViewModel 类结束

public partial class TypeFilterTypeEntry : ObservableObject // 定义部分类 TypeFilterTypeEntry，继承自 ObservableObject（用于 UI 绑定与属性通知）
{
    public required string DisplayText { get; set; } // 必需属性：显示文本（DisplayText），用于在 UI 中显示类型名称

    public required int TypeId { get; set; } // 必需属性：类型 ID（TypeId），例如 AssetClassID 枚举值或 0x72 表示 MonoBehaviour

    public required AssetTypeReference? ScriptRef { get; set; } // 必需属性：可选的脚本引用（ScriptRef），当表示具体脚本时不为 null

    [ObservableProperty] // 特性：自动生成 IsSelected 属性的封装与通知
    public bool _isSelected = false; // 字段：IsSelected 的后备字段，表示该过滤项是否被选中（默认 false）

    public static TypeFilterTypeEntry FromTypeId(int typeId) // 静态工厂方法：根据 TypeId 创建 TypeFilterTypeEntry（FromTypeId）
    {
        return new TypeFilterTypeEntry // 返回新创建的实例
        {
            TypeId = typeId, // 设置 TypeId
            DisplayText = Enum.GetName(typeof(AssetClassID), typeId) // 尝试将枚举名作为显示文本
                ?? $"Unknown #{typeId}", // 如果无法解析枚举名则使用 "Unknown #id" 作为显示文本
            ScriptRef = null // ScriptRef 为 null（表示非具体脚本类型）
        };
    }

    public static TypeFilterTypeEntry FromTypeReference(AssetTypeReference reference) // 静态工厂方法：根据脚本引用创建 TypeFilterTypeEntry（FromTypeReference）
    {
        return new TypeFilterTypeEntry // 返回新创建的实例
        {
            TypeId = 0x72, // 对于脚本引用，TypeId 使用 0x72（MonoBehaviour）
            DisplayText = reference.Namespace != "" // 构造显示文本：如果有命名空间则显示 "MB Namespace.ClassName"，否则 "MB ClassName"
                ? $"MB {reference.Namespace}.{reference.ClassName}"
                : $"MB {reference.ClassName}",
            ScriptRef = reference // 保存脚本引用到 ScriptRef
        };
    }

    public override bool Equals(object? obj) // 重写 Equals：用于判断两个 TypeFilterTypeEntry 是否相等（基于 TypeId 与 ScriptRef）
    {
        if (obj is not TypeFilterTypeEntry otherObj) // 如果 obj 不是 TypeFilterTypeEntry
            return false; // 返回 false

        if (ScriptRef is null) // 如果当前对象的 ScriptRef 为 null（表示非脚本类型）
        {
            if (otherObj.ScriptRef is null) // 如果另一个对象的 ScriptRef 也为 null
                return TypeId == otherObj.TypeId; // 则比较 TypeId 是否相等
        }
        else if (ScriptRef is not null) // 如果当前对象有 ScriptRef（表示具体脚本）
        {
            if (otherObj.ScriptRef is not null) // 如果另一个对象也有 ScriptRef
                return ScriptRef.Equals(otherObj.ScriptRef) && TypeId == otherObj.TypeId; // 比较 ScriptRef 与 TypeId 是否都相等
        }

        return false; // 其他情况返回 false
    }

    public override int GetHashCode() // 重写 GetHashCode：基于 TypeId 与 ScriptRef 生成哈希码
    {
        return HashCode.Combine(TypeId, ScriptRef); // 使用 HashCode.Combine 组合 TypeId 与 ScriptRef
    }

    public override string ToString() // 重写 ToString：用于在 UI 或调试时显示条目的文本
    {
        return DisplayText; // 返回 DisplayText 作为字符串表示
    }
}
