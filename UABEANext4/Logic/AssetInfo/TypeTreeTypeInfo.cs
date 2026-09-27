using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于访问 Unity 资产解析相关类型（保留英文原名：AssetsTools.NET）

using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展功能（保留英文原名：AssetsTools.NET.Extra）

using System.Diagnostics.CodeAnalysis;
// 引用诊断/注解命名空间，用于 [MaybeNullWhen] 等可空性注解（保留英文原名：System.Diagnostics.CodeAnalysis）

using System.Text;
// 引用文本处理命名空间，提供 StringBuilder 等（保留英文原名：System.Text）

namespace UABEANext4.Logic.AssetInfo;
// 定义命名空间 UABEANext4.Logic.AssetInfo，用于组织与资产类型信息显示相关的类型（保留英文原名：UABEANext4.Logic.AssetInfo）

public class TypeTreeTypeInfo
// 定义公共类 TypeTreeTypeInfo（保留英文原名：TypeTreeTypeInfo），用于封装并显示 TypeTreeType 的可读信息
{
    // 类体开始（TypeTreeTypeInfo）

    public TypeTreeType TtType { get; }
    // 只读公共属性 TtType（TypeTreeType）：保存底层的类型树类型对象（保留英文原名：TtType / TypeTreeType）

    public string Name { get; set; }
    // 公共属性 Name（string）：表示类型的名称，可读写（保留英文原名：Name）

    public string ScriptName { get; set; }
    // 公共属性 ScriptName（string）：表示关联脚本/类名（如果有），可读写（保留英文原名：ScriptName）

    public int TypeId => TtType.TypeId;
    // 只读计算属性 TypeId（int）：从 TtType 获取类型 ID（保留英文原名：TypeId），用于显示或比较

    public uint ScriptId { get; set; }
    // 公共属性 ScriptId（uint）：表示脚本索引或 ID，可读写（保留英文原名：ScriptId）

    public bool IsRef => TtType.IsRefType;
    // 只读计算属性 IsRef（bool）：指示该类型是否为引用类型（保留英文原名：IsRef / IsRefType）

    public string TypeHash => !TtType.TypeHash.IsZero() ? TtType.TypeHash.ToString() : string.Empty;
    // 只读计算属性 TypeHash（string）：如果 TypeHash 非零则返回其字符串表示，否则返回空字符串（保留英文原名：TypeHash / IsZero）

    public string MonoHash => !TtType.ScriptIdHash.IsZero() ? TtType.ScriptIdHash.ToString() : string.Empty;
    // 只读计算属性 MonoHash（string）：如果 ScriptIdHash 非零则返回其字符串表示，否则返回空字符串（保留英文原名：MonoHash / ScriptIdHash）

    public TypeTreeTypeInfo(AssetsManager manager, AssetsFileInstance fileInst, TypeTreeType ttType)
    // 构造函数 TypeTreeTypeInfo(AssetsManager manager, AssetsFileInstance fileInst, TypeTreeType ttType)
    // 用于根据提供的 manager、fileInst 与 ttType 初始化 TypeTreeTypeInfo 实例（保留英文原名：TypeTreeTypeInfo）
    {
        // 构造函数体开始

        TtType = ttType;
        // 将传入的 ttType 赋值给只读属性 TtType（保留英文原名：TtType）

        Name = GetTypeName(manager, fileInst, ttType) ?? "UNKNOWN";
        // 调用私有静态方法 GetTypeName 获取类型名；若返回 null 则使用 "UNKNOWN" 作为回退（保留英文原名：GetTypeName）

        if (GetScriptIndexAndName(manager, fileInst, ttType, out ushort index, out string? name))
        {
            ScriptName = name;
            ScriptId = index;
        }
        else
        {
            ScriptName = "UNKNOWN";
            ScriptId = ushort.MaxValue;
        }
        // 调用私有静态方法 GetScriptIndexAndName 尝试获取脚本索引与名称：
        // 如果成功则将 ScriptName 与 ScriptId 设为返回值；
        // 如果失败则将 ScriptName 设为 "UNKNOWN" 并将 ScriptId 设为 ushort.MaxValue（保留英文原名：GetScriptIndexAndName / ushort.MaxValue）
    }
    // 构造函数体结束

    public override string ToString()
    // 重写 ToString() 方法以返回该类型信息的可读字符串表示（保留英文原名：ToString）
    {
        // 方法体开始

        StringBuilder sb = new StringBuilder();
        // 创建 StringBuilder（sb）用于高效构建输出字符串（保留英文原名：StringBuilder）

        sb.Append(Name);
        // 将 Name 追加到字符串构建器中（保留英文原名：Name）

        if (ScriptName != string.Empty)
        {
            sb.Append(' ');
            sb.Append(ScriptName);
        }
        // 如果 ScriptName 非空，则在名称后追加空格与 ScriptName（保留英文原名：ScriptName）

        if (ScriptId != ushort.MaxValue)
        {
            sb.Append(" (0x");
            sb.Append(TypeId.ToString("x"));
            sb.Append('/');
            sb.Append(ScriptId.ToString("d4"));
            sb.Append(')');
        }
        else
        {
            sb.Append(" (0x");
            sb.Append(TypeId.ToString("x"));
            sb.Append(')');
        }
        // 根据 ScriptId 是否为回退值（ushort.MaxValue）决定输出格式：
        // 如果有有效 ScriptId，则输出 "(0x{TypeId:x}/{ScriptId:d4})"；
        // 否则只输出 "(0x{TypeId:x})"（保留英文原名：TypeId / ScriptId / ToString 格式）

        if (IsRef)
        {
            sb.Append(" REF");
        }
        // 如果 IsRef 为 true，则在末尾追加 " REF" 标记（保留英文原名：IsRef）

        return sb.ToString();
        // 返回构建好的字符串表示（保留英文原名：ToString）
    }
    // 方法体结束

    private static string? GetTypeName(AssetsManager manager, AssetsFileInstance fileInst, TypeTreeType ttType)
    // 私有静态方法 GetTypeName：尝试从 ClassDatabase 或 TypeTree 中获取类型名称，返回字符串或 null（保留英文原名：GetTypeName）
    {
        // 方法体开始

        var cldb = manager.ClassDatabase;
        // 从 AssetsManager 获取 ClassDatabase（cldb），用于在没有 TypeTree 的情况下查找类型名（保留英文原名：ClassDatabase）

        var metadata = fileInst.file.Metadata;
        // 获取当前 AssetsFileInstance 的 Metadata（保留英文原名：Metadata）

        if (!metadata.TypeTreeEnabled && cldb != null)
        {
            // use class database
            var cldbType = cldb.FindAssetClassByID(ttType.TypeId);
            if (cldbType == null)
            {
                return null;
            }

            return cldb.GetString(cldbType.Name);
        }
        // 如果文件的 TypeTree 未启用且 ClassDatabase 可用，则使用 ClassDatabase 根据 TypeId 查找类型名：
        // 若找不到则返回 null，否则通过 cldb.GetString 返回名称（保留英文原名：TypeTreeEnabled / FindAssetClassByID / GetString）

        else
        {
            // use type tree and read the first field, if it exists
            var ttNodes = ttType.Nodes;
            if (ttNodes.Count == 0)
            {
                return null;
            }

            var baseField = ttType.Nodes[0];
            return baseField.GetTypeString(ttType.StringBufferBytes);
        }
        // 否则使用 TypeTree：读取 ttType 的节点列表（Nodes），如果为空返回 null；
        // 否则取第一个节点（baseField）并调用 GetTypeString（传入 StringBufferBytes）以获取类型名（保留英文原名：Nodes / GetTypeString / StringBufferBytes）
    }
    // 方法体结束

    private static bool GetScriptIndexAndName(
        AssetsManager manager, AssetsFileInstance fileInst, TypeTreeType ttType,
        [MaybeNullWhen(false)] out ushort index,
        [MaybeNullWhen(false)] out string name)
    // 私有静态方法 GetScriptIndexAndName：尝试解析 ttType 的脚本索引（ScriptTypeIndex）并获取脚本名称，返回是否成功（保留英文原名：GetScriptIndexAndName）
    {
        // 方法体开始

        index = ushort.MaxValue;
        // 将 out 参数 index 初始化为回退值 ushort.MaxValue（表示无效或未找到）（保留英文原名：index）

        name = null;
        // 将 out 参数 name 初始化为 null（保留英文原名：name）

        if (ttType.ScriptTypeIndex != 0xffff)
        {
            var scriptInfo = AssetHelper.GetAssetsFileScriptInfo(manager, fileInst, ttType.ScriptTypeIndex);
            var scriptName = scriptInfo?.ClassName;
            if (scriptName == null)
            {
                // null because there is a name but we don't know what it is
                return false;
            }

            index = ttType.ScriptTypeIndex;
            name = scriptName;
            return true;
        }
        else
        {
            // empty because there isn't one and it looks better blank
            name = string.Empty;
            return true;
        }
        // 逻辑说明：
        // 如果 ttType.ScriptTypeIndex 不是 0xffff（表示存在脚本索引），则通过 AssetHelper.GetAssetsFileScriptInfo 获取脚本信息并读取 ClassName：
        //   如果 scriptName 为 null（表示存在索引但无法解析名称），返回 false（失败）；
        //   否则将 index 与 name 设为解析到的值并返回 true（成功）。
        // 如果 ScriptTypeIndex 为 0xffff（表示没有脚本），将 name 设为空字符串并返回 true（表示“成功但为空”以便上层显示为空）。
    }
    // 方法体结束

}
// 类体结束（TypeTreeTypeInfo）
