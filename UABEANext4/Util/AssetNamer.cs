using AssetsTools.NET; // 引用 AssetsTools.NET 库，用于读取和操作 Unity 资产文件（保留英文原名 AssetsTools.NET）
using AssetsTools.NET.Extra; // 引用 AssetsTools.NET 的扩展功能（保留英文原名 AssetsTools.NET.Extra）
using System; // 引用基础系统命名空间（保留英文原名 System）
using System.Collections.Concurrent; // 引用并发集合命名空间（保留英文原名 System.Collections.Concurrent）
using System.IO; // 引用 IO 操作命名空间（保留英文原名 System.IO）
using System.Runtime.CompilerServices; // 引用运行时编译器服务命名空间（保留英文原名 System.Runtime.CompilerServices）
using UABEANext4.AssetWorkspace; // 引用项目的资产工作区命名空间（保留英文原名 UABEANext4.AssetWorkspace）

namespace UABEANext4.Util; // 定义命名空间 UABEANext4.Util（保留英文原名）

public class AssetNamer // 定义公共类 AssetNamer：用于根据 AssetInst 生成可读名称（保留英文原名 AssetNamer）
{
    private readonly Workspace _workspace; // 私有只读字段 _workspace：保存传入的 Workspace 实例（用于访问 Manager 等）

    // cache for optimization
    private readonly ConcurrentDictionary<AssetsFileInstance, NameReadOptimization> _gameObjectNro = []; // 私有并发字典：缓存每个文件的 GameObject 名称读取优化策略（_gameObjectNro）
    private readonly ConcurrentDictionary<AssetsFileInstance, NameReadOptimization> _monoBehaviourNro = []; // 私有并发字典：缓存每个文件的 MonoBehaviour 名称读取优化策略（_monoBehaviourNro）

    public AssetNamer(Workspace workspace) // 构造函数：接收 Workspace 并保存到字段
    {
        _workspace = workspace; // 将传入的 workspace 赋值给私有字段 _workspace
    }

    public string? GetAssetName(AssetInst asset, bool usePrefix, int maxLen) // 公共方法：获取资产的显示名称（可能为 null）
    {
        GetDisplayName(asset, usePrefix, maxLen, out var assetName, out _); // 调用 GetDisplayName 填充 assetName（忽略 typeName）
        return assetName; // 返回 assetName
    }

    public string GetAssetTypeName(AssetInst asset, bool usePrefix, int maxLen) // 公共方法：获取资产的类型名称（字符串）
    {
        GetDisplayName(asset, usePrefix, maxLen, out _, out var typeName); // 调用 GetDisplayName 填充 typeName（忽略 assetName）
        return typeName; // 返回 typeName
    }

    public void GetDisplayName(AssetInst asset, bool usePrefix, int maxLen, out string? assetName, out string typeName) // 核心方法：根据 asset 填充 assetName 与 typeName
    {
        assetName = null; // 初始化输出参数 assetName 为 null

        var manager = _workspace.Manager; // 获取 Workspace 的 Manager（用于模板/引用管理）
        var fileInst = asset.FileInstance; // 获取资产所属的文件实例

        AssetTypeTemplateField? tempBaseField; // 声明可空的模板字段变量 tempBaseField
        try
        {
            tempBaseField = manager.GetTemplateBaseField(fileInst, asset, AssetReadFlags.SkipMonoBehaviourFields); // 尝试从 manager 获取基础模板字段，跳过 MonoBehaviour 字段以加速
        }
        catch
        {
            // if this is a monobehaviour, retry from the cldb (this may not work either)
            try
            {
                tempBaseField = fileInst.file.Metadata.TypeTreeEnabled
                    ? manager.GetTemplateBaseField(fileInst, asset, AssetReadFlags.ForceFromCldb) // 如果启用了 TypeTree，则尝试强制从 ClassDatabase 获取模板
                    : null; // don't even try if typetree types don't exist
            }
            catch
            {
                // give up
                tempBaseField = null; // 如果仍失败，则放弃并将 tempBaseField 设为 null
            }
        }

        if (tempBaseField == null) // 如果没有模板字段（无法解析类型结构）
        {
            var maybeClassId = (AssetClassID)asset.TypeId; // 将 asset.TypeId 转为 AssetClassID 枚举尝试获取名称
            typeName = Enum.GetName(maybeClassId) ?? $"Type ID 0x{asset.TypeId:x}"; // 如果能从枚举获取名称则使用，否则使用 "Type ID 0x..." 形式
        }
        else
        {
            typeName = tempBaseField.Type; // 如果有模板字段，则使用模板字段的 Type 字段作为类型名
        }

        if (tempBaseField != null && tempBaseField.Children.Count > 0) // 如果模板存在且有子字段
        {
            try
            {
                var firstChild = tempBaseField.Children[0]; // 获取第一个子字段（通常可能是 m_Name）
                if (firstChild.Name == "m_Name" && firstChild.Type == "string" && asset.TypeId != (int)AssetClassID.Shader) // 如果第一个字段是字符串类型的 m_Name 且不是 Shader 类型
                {
                    GetLockObjAndReader(asset, out object lockObj, out AssetsFileReader reader, out long pos); // 获取用于读取的锁对象、读取器与起始位置

                    lock (lockObj) // 在锁内读取以保证线程安全
                    {
                        reader.Position = pos; // 将读取器位置设置为资产的起始位置
                        assetName = reader.ReadCountStringInt32(); // 读取以 int32 长度前缀的字符串（m_Name）
                    }

                    if (assetName != string.Empty) // 如果读取到非空名称
                    {
                        TrimAssetName(ref assetName, maxLen); // 裁剪名称到最大长度
                        return; // 返回（已成功获取名称）
                    }
                }

                var nro = NameReadOptimization.Unchecked; // 初始化名称读取优化策略为 Unchecked（未检查）
                var refMan = manager.GetRefTypeManager(fileInst); // 获取引用类型管理器（用于迭代复杂字段）
                if (asset.TypeId == (int)AssetClassID.GameObject) // 如果资产类型是 GameObject（游戏对象）
                {
                    if (!_gameObjectNro.TryGetValue(fileInst, out nro)) // 尝试从缓存获取该文件的 GameObject 优化策略
                        _gameObjectNro[fileInst] = nro = NameReadOptimization.Unchecked; // 如果没有则初始化为 Unchecked

                    if (nro == NameReadOptimization.Unchecked) // 如果尚未决定优化策略
                    {
                        _gameObjectNro[fileInst] = nro = GetGameObjectNro(tempBaseField); // 通过模板分析决定策略并缓存
                    }

                    if (nro == NameReadOptimization.UseOptimized) // 如果策略为 UseOptimized（可用快速读取路径）
                    {
                        var headerVer = fileInst.file.Header.Version; // 获取文件头版本
                        GetLockObjAndReader(asset, out object lockObj, out AssetsFileReader reader, out long pos); // 获取锁、读取器与位置

                        lock (lockObj) // 在锁内读取
                        {
                            reader.Position = pos; // 定位到资产起始位置
                            int size = reader.ReadInt32(); // 读取组件数组的大小（int）
                            int componentSize = headerVer >= 17 ? 0x0c : 0x10; // 根据 header 版本决定每个组件条目的大小
                            reader.Position += size * componentSize; // 跳过组件数组数据
                            reader.Position += 4; // 跳过额外的 4 字节（通常是某个计数或对齐）
                            assetName = usePrefix
                                ? $"游戏对象 (GameObject) {reader.ReadCountStringInt32()}" // 如果需要前缀，则返回 "游戏对象 (GameObject) <name>"
                                : reader.ReadCountStringInt32(); // 否则仅返回名称字符串
                        }

                        if (assetName != string.Empty) // 如果读取到非空名称
                        {
                            TrimAssetName(ref assetName, maxLen); // 裁剪名称
                            return; // 返回
                        }
                    }
                }
                else if (asset.TypeId == (int)AssetClassID.MonoBehaviour || asset.TypeId < 0) // 如果是 MonoBehaviour（脚本）或负数类型（旧式脚本）
                {
                    if (!_monoBehaviourNro.TryGetValue(fileInst, out nro)) // 尝试从缓存获取 MonoBehaviour 的优化策略
                        _monoBehaviourNro[fileInst] = nro = NameReadOptimization.Unchecked; // 如果没有则初始化为 Unchecked

                    if (nro == NameReadOptimization.Unchecked) // 如果尚未决定策略
                    {
                        _monoBehaviourNro[fileInst] = nro = GetMonoBehaviourNro(tempBaseField); // 通过模板分析决定策略并缓存
                    }

                    if (nro == NameReadOptimization.UseOptimized) // 如果可以使用优化读取
                    {
                        GetLockObjAndReader(asset, out object lockObj, out AssetsFileReader reader, out long pos); // 获取锁、读取器与位置

                        lock (lockObj) // 在锁内读取
                        {
                            reader.Position = pos + 0x1c; // 定位到 m_Name 的快速偏移（0x1c）
                            assetName = reader.ReadCountStringInt32(); // 读取名称字符串
                        }

                        if (assetName == string.Empty) // 如果读取为空
                        {
                            assetName = GetMonoBehaviourNameFast(asset); // 尝试使用快速方法获取 MonoBehaviour 名称
                        }
                        if (assetName != string.Empty) // 如果最终得到非空名称
                        {
                            TrimAssetName(ref assetName, maxLen); // 裁剪名称
                            return; // 返回
                        }
                    }
                }
                else if (asset.TypeId == (int)AssetClassID.Shader) // 如果是 Shader 类型
                {
                    GetLockObjAndReader(asset, out object lockObj, out AssetsFileReader reader, out long pos); // 获取锁、读取器与位置

                    lock (lockObj) // 在锁内使用迭代器读取复杂结构
                    {
                        reader.Position = pos; // 定位到资产起始位置
                        var iterator = new AssetTypeValueIterator(tempBaseField, reader, refMan); // 创建 AssetTypeValueIterator 用于遍历字段

                        // skip first name
                        iterator.ReadNext(); // 跳过第一个字段（通常是某个名称）
                        iterator.ReadNext(); // 再跳过一个字段

                        while (iterator.ReadNext()) // 继续读取直到结束
                        {
                            if (iterator.TempField.Name == "m_Name" && iterator.TempField.Type == "string" && iterator.TempFieldStack[1].Name == "m_ParsedForm") // 如果找到 m_Name 且其父字段是 m_ParsedForm
                            {
                                var valueField = iterator.ReadValueField(); // 读取值字段
                                assetName = valueField.AsString; // 将其作为名称
                                break; // 跳出循环
                            }
                        }
                    }

                    nro = NameReadOptimization.UseOptimized; // 对 Shader 强制标记为 UseOptimized（因为上面已使用迭代器找到名称）
                }

                if (nro == NameReadOptimization.Unchecked) // 如果策略仍为 Unchecked（未能确定）
                {
                    assetName = $"{asset.Type} #{asset.PathId}"; // 使用默认回退格式："<Type> #<PathId>"
                }
                else if (nro == NameReadOptimization.UseIterator) // 如果策略为 UseIterator（需要使用迭代器逐字段查找）
                {
                    GetLockObjAndReader(asset, out object lockObj, out AssetsFileReader reader, out long pos); // 获取锁、读取器与位置

                    lock (lockObj) // 在锁内使用迭代器查找 m_Name 字段
                    {
                        reader.Position = pos; // 定位到资产起始位置
                        var iterator = new AssetTypeValueIterator(tempBaseField, reader, refMan); // 创建迭代器
                        while (iterator.ReadNext()) // 遍历字段
                        {
                            if (iterator.TempField.Name == "m_Name" && iterator.TempField.Type == "string") // 找到 m_Name 且类型为 string
                            {
                                var valueField = iterator.ReadValueField(); // 读取值字段
                                assetName = valueField.AsString; // 取出字符串值作为名称
                                break; // 跳出循环
                            }
                        }
                    }
                }
            }
            catch
            {
                assetName = null; // 如果任何读取过程抛出异常，则将 assetName 设为 null（安全回退）
            }
        }

        if (string.IsNullOrEmpty(assetName)) // 如果最终没有得到有效名称
        {
            assetName = $"{asset.Type} #{asset.PathId}"; // 使用回退名称 "<Type> #<PathId>"
        }

        TrimAssetName(ref assetName, maxLen); // 裁剪名称到最大长度（如果需要）
    }

    // not very fast but w/e at least it's stable
    public string GetMonoBehaviourNameFast(AssetInst asset) // 公共方法：快速尝试获取 MonoBehaviour 的名称（不保证总是成功）
    {
        var manager = _workspace.Manager; // 获取 Manager

        // allow negative monobehaviours (old style) but not positive non-monobehaviours
        if (asset.Type != AssetClassID.MonoBehaviour && asset.TypeId >= 0) // 如果 asset.Type 不是 MonoBehaviour 且 TypeId 为非负（表示不是脚本）
            return string.Empty; // 返回空字符串（不适用）

        try
        {
            // get script index but skip any monobehaviours with index 0xffff (not sure why these happen)
            var scriptIdx = asset.GetScriptIndex(asset.FileInstance.file); // 获取脚本索引
            if (scriptIdx == ushort.MaxValue) // 如果索引为 0xffff（表示无效或特殊情况）
                return "Mono行为 (MonoBehaviour)"; // 返回中文显示 "Mono行为 (MonoBehaviour)" 作为回退名称

            var scriptPtr = asset.FileInstance.file.Metadata.ScriptTypes[scriptIdx]; // 获取脚本指针信息
            var fileInst = asset.FileInstance; // 初始文件实例为当前文件

            if (scriptPtr.FileId != 0) // 如果脚本指针引用了其他文件（FileId != 0）
                fileInst = fileInst.GetDependency(manager, scriptPtr.FileId - 1); // 获取依赖文件实例
            if (fileInst == null) // 如果无法获取依赖文件
                return "Mono行为 (MonoBehaviour)"; // 返回回退名称

            AssetFileInfo? info = fileInst.file.GetAssetInfo(scriptPtr.PathId); // 在依赖文件中查找脚本资产信息
            if (info == null) // 如果找不到脚本资产信息
                return "Mono行为 (MonoBehaviour)"; // 返回回退名称

            // AssetNamer might double lock this, but that's okay
            lock (fileInst.LockReader) // 锁定文件读取器以保证线程安全
            {
                var reader = fileInst.file.Reader; // 获取读取器
                reader.Position = info.GetAbsoluteByteOffset(fileInst.file); // 定位到脚本资产的绝对字节偏移
                return reader.ReadCountStringInt32(); // 读取并返回脚本名称字符串
            }
        }
        catch
        {
            return string.Empty; // 出现异常时返回空字符串
        }
    }

    public static string GetFallbackName(AssetInst asset, string? name) // 公共静态方法：根据可能的 name 返回回退名称（如果 name 为 null 则使用 "<Type> #<PathId>"）
    {
        return name ?? $"{asset.Type} #{asset.PathId}"; // 如果 name 非空则返回 name，否则返回回退格式
    }

    public static string GetAssetFileName(Workspace workspace, AssetInst asset, string ext, int maxNameLen) // 公共静态方法：生成导出文件名（包含 workspace.Namer 生成的名称、源文件名与 PathId）
    {
        var assetName = workspace.Namer.GetAssetName(asset, false, maxNameLen); // 使用 workspace 的 Namer 获取资产名称（不使用前缀）
        assetName = GetFallbackName(asset, assetName); // 使用回退名称确保非空
        assetName = PathUtils.ReplaceInvalidPathChars(assetName); // 替换文件名中非法字符
        return $"{assetName}-{Path.GetFileName(asset.FileInstance.path)}-{asset.PathId}{ext}"; // 返回格式化的文件名：<assetName>-<源文件名>-<PathId><ext>
    }

    public static string GetAssetFileName(AssetInst asset, string assetNameOverride, string ext) // 重载：使用外部提供的 assetNameOverride 生成文件名
    {
        string assetName = PathUtils.ReplaceInvalidPathChars(assetNameOverride); // 替换非法字符
        return $"{assetName}-{Path.GetFileName(asset.FileInstance.path)}-{asset.PathId}{ext}"; // 返回格式化文件名
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // 方法内联提示：建议编译器尽可能内联此方法以提高性能
    private static void GetLockObjAndReader(
        AssetInst asset, out object lockObj, out AssetsFileReader reader, out long pos) // 私有静态方法：根据 asset 决定用于读取的锁对象、读取器与起始位置
    {
        var fileInst = asset.FileInstance; // 获取资产所属的文件实例
        if (asset.IsReplacerPreviewable) // 如果 asset 是可预览的替换器（Replacer）
        {
            var stream = asset.Replacer.GetPreviewStream(); // 获取替换器的预览流
            lockObj = stream; // 使用流作为锁对象
            reader = new AssetsFileReader(stream); // 为该流创建新的 AssetsFileReader（独立于文件的 Reader）
            pos = 0; // 起始位置为 0（流的开头）
        }
        else
        {
            lockObj = fileInst.LockReader; // 否则使用文件实例的 LockReader 作为锁对象
            reader = asset.FileInstance.file.Reader; // 使用文件的共享 Reader
            pos = asset.AbsoluteByteStart; // 起始位置为资产在文件中的绝对字节偏移
        }
    }

    private static NameReadOptimization GetGameObjectNro(AssetTypeTemplateField tempField) // 私有静态方法：分析 GameObject 模板字段以决定名称读取优化策略
    {
        var b = tempField.Children; // 获取模板字段的子字段集合
        if (b.Count < 3) // 如果子字段少于 3 个
            return NameReadOptimization.UseIterator; // 返回 UseIterator（需要迭代器逐字段查找）

        var b_m_Component = b[0]; // 第 0 个子字段通常是 m_Component
        if (b_m_Component.Type != "vector") // 如果类型不是 vector
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        if (b_m_Component.Children.Count != 1) // 如果 m_Component 的子字段数量不是 1
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_Component_Array = b_m_Component[0]; // 获取 m_Component 的第一个子字段（数组描述）
        if (b_m_Component_Array.Type != "Array") // 如果类型不是 Array
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        if (b_m_Component_Array.Children.Count != 2) // 如果数组描述的子字段数量不是 2（size 与 data）
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_Component_Array_size = b_m_Component_Array[0]; // 数组的 size 字段
        if (b_m_Component_Array_size.Type != "int") // 如果 size 类型不是 int
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_Component_Array_data = b_m_Component_Array[1]; // 数组的 data 字段
        if (b_m_Component_Array_data.Type != "ComponentPair") // 如果 data 类型不是 ComponentPair
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        if (b_m_Component_Array_data.Children.Count != 1) // 如果 ComponentPair 的子字段数量不是 1
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_Component_Array_data_component = b_m_Component_Array_data[0]; // 获取 ComponentPair 的 component 字段
        if (b_m_Component_Array_data_component.Type != "PPtr<Component>") // 如果 component 类型不是 PPtr<Component>
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        if (b_m_Component_Array_data_component.Children.Count != 2) // 如果 component 的子字段数量不是 2（FileID 与 PathID）
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_Component_Array_data_component_m_FileID = b_m_Component_Array_data_component[0]; // component 的 FileID 字段
        if (b_m_Component_Array_data_component_m_FileID.Type != "int") // 如果 FileID 类型不是 int
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_Component_Array_data_component_m_PathID = b_m_Component_Array_data_component[1]; // component 的 PathID 字段
        if (b_m_Component_Array_data_component_m_PathID.Type != "SInt64") // 如果 PathID 类型不是 SInt64
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_Layer = b[1]; // 第 1 个子字段通常是 m_Layer
        if (b_m_Layer.Type != "unsigned int") // 如果 m_Layer 类型不是 unsigned int
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_Name = b[2]; // 第 2 个子字段通常是 m_Name
        if (b_m_Name.Type != "string") // 如果 m_Name 类型不是 string
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        return NameReadOptimization.UseOptimized; // 如果所有检查都通过，则返回 UseOptimized（可以使用快速读取路径）
    }

    private static NameReadOptimization GetMonoBehaviourNro(AssetTypeTemplateField tempField) // 私有静态方法：分析 MonoBehaviour 模板字段以决定名称读取优化策略
    {
        var b = tempField.Children; // 获取子字段集合
        if (b.Count < 4) // 如果子字段少于 4 个
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_GameObject = b[0]; // 第 0 个子字段通常是 m_GameObject
        if (b_m_GameObject.Type != "PPtr<GameObject>") // 如果类型不是 PPtr<GameObject>
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        if (b_m_GameObject.Children.Count != 2) // 如果 m_GameObject 的子字段数量不是 2（FileID 与 PathID）
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_GameObject_m_FileID = b_m_GameObject[0]; // m_GameObject 的 FileID 字段
        if (b_m_GameObject_m_FileID.Type != "int") // 如果 FileID 类型不是 int
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_GameObject_m_PathID = b_m_GameObject[1]; // m_GameObject 的 PathID 字段
        if (b_m_GameObject_m_PathID.Type != "SInt64") // 如果 PathID 类型不是 SInt64
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_Enabled = b[1]; // 第 1 个子字段通常是 m_Enabled
        if (b_m_Enabled.Type != "UInt8") // 如果 m_Enabled 类型不是 UInt8
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_Script = b[2]; // 第 2 个子字段通常是 m_Script
        if (b_m_Script.Type != "PPtr<MonoScript>") // 如果 m_Script 类型不是 PPtr<MonoScript>
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        if (b_m_Script.Children.Count != 2) // 如果 m_Script 的子字段数量不是 2（FileID 与 PathID）
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_Script_m_FileID = b_m_Script[0]; // m_Script 的 FileID 字段
        if (b_m_Script_m_FileID.Type != "int") // 如果 FileID 类型不是 int
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_Script_m_PathID = b_m_Script[1]; // m_Script 的 PathID 字段
        if (b_m_Script_m_PathID.Type != "SInt64") // 如果 PathID 类型不是 SInt64
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        var b_m_Name = b[3]; // 第 3 个子字段通常是 m_Name
        if (b_m_Name.Type != "string") // 如果 m_Name 类型不是 string
            return NameReadOptimization.UseIterator; // 返回 UseIterator

        return NameReadOptimization.UseOptimized; // 所有检查通过则返回 UseOptimized
    }

    private static void TrimAssetName(ref string name, int maxLen) // 私有静态方法：裁剪名称到最大长度
    {
        if (name.Length > maxLen) // 如果名称长度超过最大长度
        {
            name = name[..maxLen]; // 使用范围切片保留前 maxLen 个字符
        }
    }

    private enum NameReadOptimization // 私有枚举：表示名称读取的优化策略
    {
        Unchecked, // 未检查（Unchecked）
        UseOptimized, // 使用优化路径（UseOptimized）
        UseIterator // 使用迭代器逐字段查找（UseIterator）
    }
}
