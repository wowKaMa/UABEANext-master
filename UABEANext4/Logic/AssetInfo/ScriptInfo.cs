using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于访问 Unity 资产解析相关类型（保留英文原名：AssetsTools.NET）

using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展功能（保留英文原名：AssetsTools.NET.Extra）

using DynamicData;
// 引用 DynamicData 库，用于响应式集合与变更集（保留英文原名：DynamicData）

using DynamicData.Binding;
// 引用 DynamicData 的绑定扩展，用于将变更集绑定到 ObservableCollection（保留英文原名：DynamicData.Binding）

using System;
// 引用基础系统命名空间，提供常用类型（保留英文原名：System）

using System.Collections.ObjectModel;
// 引用可观察集合命名空间，提供 ObservableCollection 与 ReadOnlyObservableCollection（保留英文原名：System.Collections.ObjectModel）

using System.IO;
// 引用 IO 命名空间，用于文件路径处理（保留英文原名：System.IO）

using UABEANext4.AssetWorkspace;
// 引用项目内的资产工作区命名空间，提供 Workspace、AssetInst、AssetPPtr 等类型（保留英文原名：UABEANext4.AssetWorkspace）

namespace UABEANext4.Logic.AssetInfo;
// 定义命名空间 UABEANext4.Logic.AssetInfo，用于组织与资产信息显示相关的类型（保留英文原名：UABEANext4.Logic.AssetInfo）

public class ScriptInfo
// 定义公共类 ScriptInfo（保留英文原名：ScriptInfo），用于收集并展示文件中可用的脚本类型信息
{
    // 类体开始（ScriptInfo）

    public ObservableCollection<AssetPPtr> Scripts { get; set; } = [];
    // 公共属性 Scripts（ObservableCollection<AssetPPtr>）：保存文件中脚本类型的指针列表（AssetPPtr），并初始化为空集合
    // 说明：AssetPPtr 表示资产指针（file id + path id），用于引用脚本类型条目

    public ReadOnlyObservableCollection<string> ScriptsDisplay { get; init; }
    // 公共只读初始化属性 ScriptsDisplay（ReadOnlyObservableCollection<string>）：用于绑定到 UI 的脚本显示字符串集合（init 表示只能在初始化时赋值）

    public ScriptInfo(Workspace workspace, AssetsFileInstance fileInst)
    // 构造函数 ScriptInfo(Workspace workspace, AssetsFileInstance fileInst)：从指定的文件实例（fileInst）和工作区（workspace）构建脚本信息
    {
        // 构造体开始

        var scripts = fileInst.file.Metadata.ScriptTypes;
        // 从文件实例的元数据（Metadata）中获取 ScriptTypes 列表（原名：scripts），这是文件中记录的脚本类型指针集合

        foreach (var script in scripts)
        {
            Scripts.Add(script);
        }
        // 将从元数据读取到的每个脚本指针添加到本实例的 Scripts 集合中

        Func<AssetPPtr, int, string> scriptsNameTransFac = (pptr, idx) =>
        {
            AssetTypeValueField? scriptBf = workspace.GetBaseField(fileInst, pptr.FileId, pptr.PathId);
            if (scriptBf == null)
            {
                if (pptr.FileId == 0)
                {
                    return $"{idx} - {fileInst.name}/{pptr.PathId}";
                }
                else
                {
                    string fileName = fileInst.file.Metadata.Externals[pptr.FileId - 1].PathName;
                    return $"{idx} - {Path.GetFileName(fileName)}/{pptr.PathId}";
                }
            }

            string nameSpace = scriptBf["m_Namespace"].AsString;
            string className = scriptBf["m_ClassName"].AsString;

            string fullName;
            if (nameSpace != "")
                fullName = $"{nameSpace}.{className}";
            else
                fullName = className;

            return $"{idx} - {fullName}";
        };
        // 定义一个转换函数 scriptsNameTransFac（Func<AssetPPtr,int,string>）：
        // - 输入：AssetPPtr（pptr）和索引（idx）
        // - 作用：尝试通过 workspace.GetBaseField 读取脚本的 BaseField（scriptBf），
        //   如果无法读取（scriptBf 为 null），则根据 pptr.FileId 决定显示为 "索引 - 文件名/PathId"（若 FileId==0 使用当前 fileInst.name，否则从 Externals 取文件名）；
        //   如果能读取，则从 BaseField 中取 "m_Namespace" 与 "m_ClassName" 字段拼接为完整类名（namespace.class），并返回格式化字符串 "索引 - 完整类名"。
        // 说明：这个函数用于把 AssetPPtr 转换为可显示的字符串（供 UI 列表使用）

        Scripts
            .ToObservableChangeSet()
            .Transform(scriptsNameTransFac)
            .Bind(out var scriptsItems)
            .DisposeMany()
            .Subscribe();
        // 使用 DynamicData 对 Scripts 集合创建变更集（ToObservableChangeSet）：
        // - Transform(scriptsNameTransFac)：将每个 AssetPPtr 转换为字符串（使用上面定义的转换函数）
        // - Bind(out var scriptsItems)：将转换后的结果绑定到一个本地变量 scriptsItems（这是一个 IObservableList/ObservableCollection 的包装）
        // - DisposeMany()：在元素被移除时处理其可能的可释放资源（此处为惯用链）
        // - Subscribe()：订阅变更集以使绑定生效
        // 结果：scriptsItems 将包含与 Scripts 对应的字符串集合，随 Scripts 的变更自动更新

        ScriptsDisplay = scriptsItems!;
        // 将绑定得到的 scriptsItems 赋值给只读属性 ScriptsDisplay（使用 null-forgiving 操作符 ! 假定 scriptsItems 非空）
        // 这样外部可以通过 ScriptsDisplay 读取用于显示的脚本名称列表
    }
    // 构造体结束

    private ScriptInfo()
    {
        ScriptsDisplay = new ReadOnlyObservableCollection<string>(new ObservableCollection<string>());
    }
    // 私有无参构造函数：用于创建一个空的 ScriptInfo 实例（例如作为默认值），并将 ScriptsDisplay 初始化为一个空的只读集合

    public static ScriptInfo Empty { get; } = new()
    {
        Scripts = [],
        ScriptsDisplay = new ReadOnlyObservableCollection<string>(new ObservableCollection<string>())
    };
    // 公共静态只读属性 Empty：提供一个空的 ScriptInfo 单例作为默认值
    // - Scripts 初始化为空集合
    // - ScriptsDisplay 初始化为一个空的只读集合
}
// 类体结束（ScriptInfo）
