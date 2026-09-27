using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于处理 Unity 资产文件（保留英文原名：AssetsTools.NET）

using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展功能，提供额外类型与辅助方法（保留英文原名：AssetsTools.NET.Extra）

using System.ComponentModel;
// 引用组件模型命名空间，提供 INotifyPropertyChanged 与 PropertyChangedEventArgs（保留英文原名：System.ComponentModel）

using UABEANext4.Logic.Configuration;
// 引用项目内配置管理命名空间，用于读取设置（保留英文原名：UABEANext4.Logic.Configuration）

using UABEANext4.Util;
// 引用项目内工具/实用程序命名空间（保留英文原名：UABEANext4.Util）

namespace UABEANext4.AssetWorkspace;
// 定义命名空间 UABEANext4.AssetWorkspace，用于组织工作区相关类型（保留英文原名：UABEANext4.AssetWorkspace）

// assetfileinfo wrapper for extra info
// 注释：说明此类是对 AssetFileInfo 的包装，添加额外信息（保留英文原注释）

public class AssetInst : AssetFileInfo, INotifyPropertyChanged
// 定义公共类 AssetInst，继承自 AssetFileInfo 并实现 INotifyPropertyChanged（保留英文原名：AssetInst / AssetFileInfo / INotifyPropertyChanged）
{
    // 类体开始（AssetInst）

    public string? AssetName { get; set; }
    // 公共可空属性 AssetName：资产的显示名称或自定义名称（保留英文原名：AssetName）

    public string? DisplayContainer { get; set; }
    // 公共可空属性 DisplayContainer：用于显示的容器/路径信息（保留英文原名：DisplayContainer）

    public AssetsFileInstance FileInstance { get; }
    // 只读公共属性 FileInstance：指向包含此资产的 AssetsFileInstance（保留英文原名：FileInstance / AssetsFileInstance）

    public AssetClassID Type => (AssetClassID)TypeId;
    // 只读属性 Type：将基类的 TypeId 转换为枚举 AssetClassID（保留英文原名：Type / TypeId / AssetClassID）

    public AssetsFileReader FileReader => IsReplacerPreviewable
        ? new AssetsFileReader(Replacer.GetPreviewStream())
        : FileInstance.file.Reader;
    // 只读属性 FileReader：如果有 Replacer 的预览流则用它创建新的 AssetsFileReader，否则使用 FileInstance.file.Reader（保留英文原名：FileReader / IsReplacerPreviewable / Replacer / AssetsFileReader）

    public string FileName => FileInstance.name;
    // 只读属性 FileName：返回所属文件实例的名称（FileInstance.name）（保留英文原名：FileName / FileInstance.name）

    public long AbsoluteByteStart => IsReplacerPreviewable ? 0 : GetAbsoluteByteOffset(FileInstance.file);
    // 只读属性 AbsoluteByteStart：如果使用 Replacer 预览则返回 0，否则计算并返回在文件中的绝对字节偏移（保留英文原名：AbsoluteByteStart / GetAbsoluteByteOffset）

    public string ModifiedString => Replacer != null ? "*" : "";
    // 只读属性 ModifiedString：如果存在 Replacer（表示已修改）则返回 "*"，否则返回空字符串（保留英文原名：ModifiedString / Replacer）

    public uint ByteSizeModified => Replacer != null && Replacer.HasPreview()
        ? (uint)Replacer.GetPreviewStream().Length
        : ByteSize;
    // 只读属性 ByteSizeModified：如果 Replacer 有预览流则返回预览流长度，否则返回原始 ByteSize（保留英文原名：ByteSizeModified / Replacer / HasPreview / ByteSize）

    public string DisplayName => AssetNamer.GetFallbackName(this, AssetName);
    // 只读属性 DisplayName：通过 AssetNamer 获取显示名称（若 AssetName 为空则使用回退规则）（保留英文原名：DisplayName / AssetNamer.GetFallbackName）

    public string BundleName => FileInstance.parentBundle != null ? FileInstance.parentBundle.name : "";
    // 只读属性 BundleName：如果所属文件属于 bundle，则返回 bundle 名称，否则返回空字符串（保留英文原名：BundleName / parentBundle）

    public AssetInst(AssetsFileInstance parentFile, AssetFileInfo origInfo)
    // 构造函数 AssetInst：用父文件实例与原始 AssetFileInfo 初始化包装对象（保留英文原名：AssetInst / AssetsFileInstance / AssetFileInfo）
    {
        PathId = origInfo.PathId;
        // 将 PathId 复制自原始信息（保留英文原名：PathId / origInfo.PathId）

        ByteOffset = origInfo.ByteOffset;
        // 将 ByteOffset 复制自原始信息（保留英文原名：ByteOffset / origInfo.ByteOffset）

        ByteSize = origInfo.ByteSize;
        // 将 ByteSize 复制自原始信息（保留英文原名：ByteSize / origInfo.ByteSize）

        TypeIdOrIndex = origInfo.TypeIdOrIndex;
        // 将 TypeIdOrIndex 复制自原始信息（保留英文原名：TypeIdOrIndex / origInfo.TypeIdOrIndex）

        OldTypeId = origInfo.OldTypeId;
        // 将 OldTypeId 复制自原始信息（保留英文原名：OldTypeId / origInfo.OldTypeId）

        ScriptTypeIndex = origInfo.ScriptTypeIndex;
        // 将 ScriptTypeIndex 复制自原始信息（保留英文原名：ScriptTypeIndex / origInfo.ScriptTypeIndex）

        Stripped = origInfo.Stripped;
        // 将 Stripped 标志复制自原始信息（保留英文原名：Stripped / origInfo.Stripped）

        TypeId = origInfo.TypeId;
        // 将 TypeId 复制自原始信息（保留英文原名：TypeId / origInfo.TypeId）

        Replacer = origInfo.Replacer;
        // 将 Replacer（可能用于替换/修改资产）复制自原始信息（保留英文原名：Replacer / origInfo.Replacer）

        AssetName = "Unnamed asset";
        // 初始化 AssetName 为默认值 "Unnamed asset"（保留英文原名：AssetName）

        FileInstance = parentFile;
        // 将 FileInstance 设为传入的父文件实例（保留英文原名：FileInstance / parentFile）
    }

    public void UpdateAssetDataAndRow(Workspace workspace, AssetTypeValueField baseField)
    // 公共方法 UpdateAssetDataAndRow（重载）：接收 AssetTypeValueField 并将其写入字节数组后更新资产数据与 UI 行（保留英文原名：UpdateAssetDataAndRow / AssetTypeValueField）
    {
        UpdateAssetDataAndRow(workspace, baseField.WriteToByteArray());
        // 将 baseField 序列化为字节数组并调用另一个重载进行实际更新（保留英文原名：WriteToByteArray）
    }

    public void UpdateAssetDataAndRow(Workspace workspace, byte[] data)
    // 公共方法 UpdateAssetDataAndRow：用字节数组替换资产数据，更新显示名称、大小标记并标记工作区为脏（保留英文原名：UpdateAssetDataAndRow）
    {
        SetNewData(data);
        // 调用基类/替换逻辑将新数据设置为当前资产数据（保留英文原名：SetNewData）

        var maxNameLen = ConfigurationManager.Settings.ListingNameLength;
        // 从配置中读取显示名称的最大长度（保留英文原名：ConfigurationManager.Settings.ListingNameLength）

        AssetName = workspace.Namer.GetAssetName(this, true, maxNameLen);
        // 使用 Workspace 的 Namer 生成新的 AssetName（保留英文原名：workspace.Namer.GetAssetName）

        Update(nameof(DisplayName));
        // 触发属性变更通知，通知 UI DisplayName 已更新（保留英文原名：Update / DisplayName）

        Update(nameof(ByteSizeModified));
        // 触发属性变更通知，通知 UI ByteSizeModified 已更新（保留英文原名：ByteSizeModified）

        Update(nameof(ModifiedString));
        // 触发属性变更通知，通知 UI ModifiedString 已更新（保留英文原名：ModifiedString）

        workspace.Dirty(workspace.ItemLookup[FileInstance.name]);
        // 标记包含此资产的 WorkspaceItem 为已修改（Dirty），通过 FileInstance.name 在 ItemLookup 中查找对应项（保留英文原名：workspace.Dirty / ItemLookup）
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    // 事件 PropertyChanged：实现 INotifyPropertyChanged，用于通知属性变化（保留英文原名：PropertyChangedEventHandler / PropertyChanged）

    public void Update(string propertyName = "")
    // 公共方法 Update：触发 PropertyChanged 事件，参数为属性名（默认空字符串）（保留英文原名：Update）
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        // 如果有订阅者则调用事件，传入 PropertyChangedEventArgs（保留英文原名：PropertyChangedEventArgs / Invoke）
    }
    // 方法结束（Update）

}
// 类体结束（AssetInst）
