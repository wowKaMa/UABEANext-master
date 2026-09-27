using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于处理 Unity 资产文件（保留英文原名：AssetsTools.NET）

using UABEANext4.AssetWorkspace;
// 引用工作区相关命名空间，提供 Workspace、AssetInst 等类型与方法（保留英文原名：UABEANext4.AssetWorkspace）

namespace FontPlugin;
// 定义命名空间 FontPlugin，用于组织字体插件相关类型（保留英文原名：FontPlugin）

public static class FontHelper
// 定义静态公共类 FontHelper，包含与字体资产处理相关的辅助方法（保留英文原名：FontHelper）
{
    // 类体开始（FontHelper）

    public static AssetTypeValueField? GetByteArrayFont(Workspace workspace, AssetInst asset)
    // 静态方法 GetByteArrayFont：从给定的 AssetInst 获取可作为字节数组读取的 AssetTypeValueField（保留英文原名：GetByteArrayFont / AssetTypeValueField / Workspace / AssetInst）
    {
        // 方法体开始（GetByteArrayFont）

        AssetTypeTemplateField? fontTemp = workspace.GetTemplateField(asset);
        // 使用 Workspace.GetTemplateField 获取字体资产的模板字段（未实例化的字段结构），赋值给 fontTemp（保留英文原名：AssetTypeTemplateField / GetTemplateField / fontTemp）

        if (fontTemp == null)
            return null;
        // 如果无法获取模板字段（fontTemp 为 null），则返回 null（保留英文原名：fontTemp）

        AssetTypeTemplateField? fontData = fontTemp.Children.FirstOrDefault(f => f.Name == "m_FontData");
        // 在模板字段的子字段中查找名为 "m_FontData" 的字段并赋给 fontData（保留英文原名：fontData / FirstOrDefault / "m_FontData"）

        if (fontData == null)
            return null;
        // 如果没有找到 m_FontData 字段则返回 null（保留英文原名：fontData）

        // m_FontData.Array
        // 注释：提示下一行处理的是 m_FontData 的数组子项（保留英文原注释：m_FontData.Array）

        fontData.Children[0].ValueType = AssetValueType.ByteArray;
        // 将 m_FontData 数组第一个子项的 ValueType 设置为 ByteArray，以便后续以字节数组方式读取字体数据（保留英文原名：ValueType / AssetValueType.ByteArray）

        AssetTypeValueField baseField;
        // 声明局部变量 baseField，用于保存实例化后的字段（保留英文原名：AssetTypeValueField / baseField）

        lock (asset.FileInstance.LockReader)
        // 锁定文件读取器（LockReader）以避免并发读取冲突（保留英文原名：LockReader / lock）
        {
            baseField = fontTemp.MakeValue(asset.FileReader, asset.AbsoluteByteStart);
            // 在锁定上下文中调用模板字段的 MakeValue 方法，从文件读取实际数据并实例化为 AssetTypeValueField（保留英文原名：MakeValue / FileReader / AbsoluteByteStart / baseField）
        }

        return baseField;
        // 返回实例化后的 AssetTypeValueField（包含 m_FontData 的字节数组），供调用方使用（保留英文原名：baseField）
    }

    public static bool IsDataOtf(byte[] byteData)
    // 静态方法 IsDataOtf：检查给定字节数组是否为 OTF（OpenType）字体文件的文件头（保留英文原名：IsDataOtf / byte[]）
    {
        // 方法体开始（IsDataOtf）

        return byteData[0] == 0x4f &&
            byteData[1] == 0x54 &&
            byteData[2] == 0x54 &&
            byteData[3] == 0x4f;
        // 通过比较前四个字节是否等于 ASCII "OTTO"（0x4F 0x54 0x54 0x4F）来判断是否为 OTF 文件头，返回布尔结果（保留英文原名："OTTO" / 0x4f）
    }
}
// 类体结束（FontHelper）
