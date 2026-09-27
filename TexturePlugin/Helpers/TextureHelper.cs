using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于处理 Unity 资产文件（保留英文原名：AssetsTools.NET）

using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展命名空间，提供额外辅助类型与方法（保留英文原名：AssetsTools.NET.Extra）

using AssetsTools.NET.Texture;
// 引用 AssetsTools.NET 的纹理处理命名空间，提供 TextureFile、TextureFormat 等（保留英文原名：AssetsTools.NET.Texture）

using UABEANext4.AssetWorkspace;
// 引用项目的工作区命名空间，提供 Workspace、AssetInst 等类型（保留英文原名：UABEANext4.AssetWorkspace）

using UABEANext4.Logic.AssetInfo;
// 引用项目中与资产信息相关的逻辑命名空间（保留英文原名：UABEANext4.Logic.AssetInfo）

namespace TexturePlugin.Helpers;
// 定义命名空间 TexturePlugin.Helpers，用于组织纹理相关的辅助方法（保留英文原名：TexturePlugin.Helpers）

public static class TextureHelper
// 定义静态公共类 TextureHelper，包含若干纹理处理的辅助静态方法（保留英文原名：TextureHelper）
{
    // 类体开始（TextureHelper）

    public static AssetTypeValueField? GetByteArrayTexture(Workspace workspace, AssetInst tex)
    // 静态方法 GetByteArrayTexture：从 AssetInst 获取可作为字节数组读取的 AssetTypeValueField（保留英文原名：GetByteArrayTexture / AssetTypeValueField）
    {
        var textureTemp = workspace.GetTemplateField(tex);
        // 使用 Workspace 的 GetTemplateField 获取纹理的模板字段（未实例化的字段结构）（保留英文原名：textureTemp / workspace.GetTemplateField）

        var image_data = textureTemp.Children.FirstOrDefault(f => f.Name == "image data");
        // 在模板字段的子字段中查找名为 "image data" 的字段（保留英文原名：image_data / FirstOrDefault）

        if (image_data == null)
            return null;
        // 如果没有找到 "image data" 字段则返回 null（保留英文原名：image_data）

        image_data.ValueType = AssetValueType.ByteArray;
        // 将该字段的值类型设置为 ByteArray，以便后续以字节数组方式读取（保留英文原名：ValueType / AssetValueType.ByteArray）

        var m_PlatformBlob = textureTemp.Children.FirstOrDefault(f => f.Name == "m_PlatformBlob");
        // 在模板字段中查找名为 "m_PlatformBlob" 的字段（保留英文原名：m_PlatformBlob）

        if (m_PlatformBlob != null)
        {
            var m_PlatformBlob_Array = m_PlatformBlob.Children[0];
            m_PlatformBlob_Array.ValueType = AssetValueType.ByteArray;
        }
        // 如果存在 m_PlatformBlob，则将其数组子项的值类型也设置为 ByteArray（保留英文原名：m_PlatformBlob_Array / ValueType）

        AssetTypeValueField baseField;
        // 声明局部变量 baseField，用于保存最终实例化后的字段（保留英文原名：baseField）

        lock (tex.FileInstance.LockReader)
        {
            baseField = textureTemp.MakeValue(tex.FileReader, tex.AbsoluteByteStart);
        }
        // 在锁定文件读取器的上下文中，用模板字段的 MakeValue 方法实例化字段（从文件读取实际数据），以避免并发读取冲突（保留英文原名：LockReader / MakeValue / AbsoluteByteStart）

        return baseField;
        // 返回实例化后的 AssetTypeValueField（保留英文原名：baseField）
    }

    public static byte[]? GetRawTextureBytes(TextureFile texFile, AssetsFileInstance inst)
    // 静态方法 GetRawTextureBytes：尝试从 TextureFile 的 streamData 中读取原始图片字节（保留英文原名：GetRawTextureBytes / TextureFile）
    {
        var rootPath = Path.GetDirectoryName(inst.path);
        // 获取 assets 文件所在目录的根路径（保留英文原名：rootPath / inst.path）

        if (texFile.m_StreamData.size != 0 && texFile.m_StreamData.path != string.Empty)
        {
            string fixedStreamPath = texFile.m_StreamData.path;
            if (inst.parentBundle == null && fixedStreamPath.StartsWith("archive:/"))
            {
                fixedStreamPath = Path.GetFileName(fixedStreamPath);
            }
            if (!Path.IsPathRooted(fixedStreamPath) && rootPath != null)
            {
                fixedStreamPath = Path.Combine(rootPath, fixedStreamPath);
            }
            if (File.Exists(fixedStreamPath))
            {
                using Stream stream = File.OpenRead(fixedStreamPath);
                stream.Position = (long)texFile.m_StreamData.offset;
                texFile.pictureData = new byte[texFile.m_StreamData.size];
                stream.Read(texFile.pictureData, 0, (int)texFile.m_StreamData.size);
            }
            else
            {
                return null;
            }
        }
        // 如果 TextureFile 指定了外部流（m_StreamData），则构建正确的路径（处理 archive:/ 前缀与相对路径），并从该流中读取 pictureData；若文件不存在则返回 null（保留英文原名：m_StreamData / pictureData）

        return texFile.pictureData;
        // 返回 TextureFile 中的 pictureData（可能是从流读取或已存在的字节数组）（保留英文原名：pictureData）
    }

    public static byte[]? GetPlatformBlob(AssetTypeValueField texBaseField)
    // 静态方法 GetPlatformBlob：从已实例化的纹理字段中提取 m_PlatformBlob（平台特定数据）字节数组（保留英文原名：GetPlatformBlob）
    {
        var m_PlatformBlob = texBaseField["m_PlatformBlob"];
        // 直接从 baseField 读取 m_PlatformBlob 字段（保留英文原名：m_PlatformBlob）

        if (!m_PlatformBlob.IsDummy)
        {
            return m_PlatformBlob["Array"].AsByteArray;
        }
        // 如果 m_PlatformBlob 不是占位（非 Dummy），则返回其 Array 子字段的字节数组表示（保留英文原名：IsDummy / AsByteArray）

        return null;
        // 否则返回 null（保留英文原名：return null）
    }

    public static bool IsPo2(int n)
    // 静态方法 IsPo2：判断给定整数是否为 2 的幂（power of two）（保留英文原名：IsPo2）
    {
        return n > 0 && (n & n - 1) == 0;
    }
    // 使用位运算快速判断 n 是否为 2 的幂（保留英文原名：位运算技巧）

    // assuming width and height are po2
    // 注释：假设宽度和高度为 2 的幂（保留英文原注释）

    public static int GetMaxMipCount(int width, int height)
    // 静态方法 GetMaxMipCount：计算给定尺寸下最大 mipmap 级数（保留英文原名：GetMaxMipCount）
    {
        int widthMipCount = (int)Math.Log2(width) + 1;
        int heightMipCount = (int)Math.Log2(height) + 1;
        // 分别计算宽度和高度对应的 mip 级数（保留英文原名：Math.Log2）

        // if the texture is 512x1024 for example, select the height (1024)
        // I guess the width would stay 1 while the height resizes down
        // 注释：说明在非方形纹理时选择较大维度的 mip 级数（保留英文原注释）

        return Math.Max(widthMipCount, heightMipCount);
        // 返回宽高中较大的 mip 级数作为最大 mip count（保留英文原名：Math.Max）
    }

    public static void SwizzleOptIn(TextureFile texture, AssetsFile file)
    // 静态方法 SwizzleOptIn：根据目标平台（例如 Switch）为 TextureFile 启用 swizzle（保留英文原名：SwizzleOptIn / SwizzleType）
    {
        // note: this means "swizzle if it seems enabled" not "always enable swizzle"
        // for switch, if platformblob isn't present, this value is pretty much ignored
        // 注释：说明此方法仅在看起来需要时启用 swizzle，而不是强制总是启用（保留英文原注释）

        if (file.Metadata.TargetPlatform == (uint)BuildTarget.Switch)
        {
            texture.swizzleType = SwizzleType.Switch;
        }
        // 如果文件的目标平台是 Switch，则将 texture 的 swizzleType 设置为 Switch（保留英文原名：Metadata.TargetPlatform / BuildTarget.Switch / swizzleType）
    }
}
// 类体结束（TextureHelper）
