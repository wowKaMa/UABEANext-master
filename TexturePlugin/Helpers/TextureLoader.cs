using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于处理 Unity 资产文件（保留英文原名：AssetsTools.NET）

using AssetsTools.NET.Extra;
// 引用 AssetsTools.NET 的扩展命名空间，提供额外辅助类型与方法（保留英文原名：AssetsTools.NET.Extra）

using AssetsTools.NET.Texture;
// 引用 AssetsTools.NET 的纹理处理命名空间，提供 TextureFile、TextureFormat 等（保留英文原名：AssetsTools.NET.Texture）

using Avalonia;
// 引用 Avalonia 框架的核心命名空间（保留英文原名：Avalonia）

using Avalonia.Media.Imaging;
// 引用 Avalonia 的位图/图像类型命名空间，提供 Bitmap、WriteableBitmap 等（保留英文原名：Avalonia.Media.Imaging）

using Avalonia.Platform;
// 引用 Avalonia 平台抽象命名空间，提供像素格式、锁定帧缓冲等（保留英文原名：Avalonia.Platform）

using SkiaSharp;
// 引用 SkiaSharp 图形库，用于像素操作、绘制与图像处理（保留英文原名：SkiaSharp）

using System.Runtime.InteropServices;
// 引用运行时互操作服务，用于内存拷贝与 Marshal 操作（保留英文原名：System.Runtime.InteropServices）

using UABEANext4.AssetWorkspace;
// 引用项目的工作区命名空间，提供 Workspace、AssetInst 等类型（保留英文原名：UABEANext4.AssetWorkspace）

using UABEANext4.Logic.Mesh;
// 引用项目的网格逻辑命名空间，提供 MeshObj 等类型（保留英文原名：UABEANext4.Logic.Mesh）

namespace TexturePlugin.Helpers;
// 定义命名空间 TexturePlugin.Helpers，用于组织纹理加载相关的辅助类（保留英文原名：TexturePlugin.Helpers）

public class TextureLoader
// 定义公共类 TextureLoader：负责从 AssetInst 解码/裁剪/转换纹理以供预览或导出（保留英文原名：TextureLoader）
{
    // 类体开始（TextureLoader）

    private readonly Dictionary<AssetInst, SKImage> _spriteImageCache = [];
    // 私有只读字段 _spriteImageCache：缓存已解码的 sprite 对应的 SKImage，键为 AssetInst（保留英文原名：_spriteImageCache / SKImage）

    private readonly Queue<AssetInst> _spriteImageQueue = new();
    // 私有只读字段 _spriteImageQueue：用于记录缓存插入顺序以便淘汰最旧项（保留英文原名：_spriteImageQueue / Queue）

    private readonly SpriteAtlasLookup _spriteAtlasLookup = new();
    // 私有只读字段 _spriteAtlasLookup：用于查找和缓存 SpriteAtlas 数据（保留英文原名：_spriteAtlasLookup / SpriteAtlasLookup）

    private readonly Dictionary<AssetsFileInstance, Dictionary<string, AssetInst>> _nameToSpriteAtlasLookup = [];
    // 私有只读字段 _nameToSpriteAtlasLookup：按文件实例缓存 atlas 名称到 AssetInst 的映射（保留英文原名：_nameToSpriteAtlasLookup / AssetsFileInstance）

    // todo: this should be configurable
    // 注释：TODO：缓存大小等应可配置（保留英文原注释）

    public const int DEFAULT_MAX_SPRITE_IMAGE_CACHE_SIZE = 10;
    // 公共常量：默认 sprite 图像缓存的最大条目数（保留英文原名：DEFAULT_MAX_SPRITE_IMAGE_CACHE_SIZE）

    public Bitmap? GetSpriteAvaloniaBitmap(
        Workspace workspace, AssetInst asset,
        bool fullCrop, out TextureFormat format)
    // 公共方法 GetSpriteAvaloniaBitmap：返回 Avalonia 可用的 Bitmap（WriteableBitmap），用于 UI 预览（保留英文原名：GetSpriteAvaloniaBitmap / Bitmap）
    {
        SKBitmap? skBitmap = GetSpriteSkBitmap(workspace, asset, fullCrop, out format);
        // 调用内部方法 GetSpriteSkBitmap 获取 Skia 的 SKBitmap，并输出纹理格式（保留英文原名：GetSpriteSkBitmap / SKBitmap）

        if (skBitmap == null)
        {
            return null;
        }
        // 如果无法获取 SKBitmap 则返回 null（保留英文原名：skBitmap）

        var croppedByteSize = skBitmap.Width * skBitmap.Height * 4;
        // 计算裁剪后图像的字节大小（假设 4 字节/像素，BGRA/RGBA32）（保留英文原名：croppedByteSize）

        var bitmap = new WriteableBitmap(new PixelSize(skBitmap.Width, skBitmap.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Unpremul);
        // 创建 Avalonia 的 WriteableBitmap，像素格式为 Bgra8888，分辨率 96 DPI（保留英文原名：WriteableBitmap / PixelFormat.Bgra8888）

        using var croppedPixels = skBitmap.PeekPixels();
        // 使用 Skia 的 PeekPixels 获取像素访问对象（保留英文原名：PeekPixels）

        using var frameBuffer = bitmap.Lock();
        // 锁定 WriteableBitmap 的帧缓冲以便写入像素（保留英文原名：bitmap.Lock / frameBuffer）

        {
            var destByteSize = frameBuffer.RowBytes * frameBuffer.Size.Height;
            // 计算目标缓冲区的字节大小（每行字节数 * 高度）（保留英文原名：destByteSize / RowBytes）

            unsafe
            {
                // marshal.copy can't do native -> native so we have to do this unsafe copy
                // 注释：说明 Marshal.Copy 无法直接做本机指针到本机指针的拷贝，因此使用 unsafe 的内存拷贝（保留英文原注释）

                Buffer.MemoryCopy(croppedPixels.GetPixels().ToPointer(), frameBuffer.Address.ToPointer(), destByteSize, croppedByteSize);
                // 使用 Buffer.MemoryCopy 将 Skia 像素内存直接复制到 Avalonia 的帧缓冲（保留英文原名：Buffer.MemoryCopy）
            }
        }

        skBitmap.Dispose();
        // 释放 SKBitmap（保留英文原名：skBitmap.Dispose）

        return bitmap;
        // 返回构建好的 Avalonia Bitmap（保留英文原名：bitmap）
    }

    public byte[]? GetSpriteRawBytes(
        Workspace workspace, AssetInst asset,
        bool fullCrop, out TextureFormat format,
        out int width, out int height)
    // 公共方法 GetSpriteRawBytes：返回裁剪后的原始像素字节数组（RGBA32），并输出格式与宽高（保留英文原名：GetSpriteRawBytes）
    {
        SKBitmap? skBitmap = GetSpriteSkBitmap(workspace, asset, fullCrop, out format);
        // 复用 GetSpriteSkBitmap 获取 SKBitmap（保留英文原名：GetSpriteSkBitmap）

        if (skBitmap == null)
        {
            width = 0;
            height = 0;
            return null;
        }
        // 若无法获取则设置宽高为 0 并返回 null（保留英文原名：width / height）

        width = skBitmap.Width;
        // 设置输出宽度（保留英文原名：width）

        height = skBitmap.Height;
        // 设置输出高度（保留英文原名：height）

        byte[] outData = new byte[width * height * 4];
        // 分配输出字节数组（假设 4 字节/像素）（保留英文原名：outData）

        using (var croppedPixels = skBitmap.PeekPixels())
        {
            Marshal.Copy(croppedPixels.GetPixels(), outData, 0, outData.Length);
        }
        // 使用 Marshal.Copy 将 Skia 像素复制到托管字节数组（保留英文原名：Marshal.Copy）

        skBitmap.Dispose();
        // 释放 SKBitmap（保留英文原名：skBitmap.Dispose）

        return outData;
        // 返回像素字节数组（保留英文原名：outData）
    }

    // format output is only for error messages. the byte output is rgba32.
    // 注释：说明 format 仅用于错误信息，实际返回的字节为 RGBA32（保留英文原注释）

    public SKBitmap? GetSpriteSkBitmap(Workspace workspace, AssetInst asset, bool fullCrop, out TextureFormat format)
    // 公共方法 GetSpriteSkBitmap：核心方法，返回 Skia 的 SKBitmap（裁剪/蒙版/翻转/旋转后），并输出纹理格式（保留英文原名：GetSpriteSkBitmap / SKBitmap）
    {
        format = 0;
        // 初始化输出格式为 0（保留英文原名：format）

        var spriteBf = workspace.GetBaseField(asset);
        // 获取 sprite 的 BaseField（反序列化后的字段结构）（保留英文原名：spriteBf / workspace.GetBaseField）

        if (spriteBf == null)
        {
            return null;
        }
        // 如果无法读取 sprite 字段则返回 null（保留英文原名：spriteBf）

        var renderData = spriteBf["m_RD"];
        // 读取 sprite 的渲染数据字段 m_RD（保留英文原名：renderData / "m_RD"）

        var spriteAtlas = GetSpriteAtlas(workspace, asset, spriteBf);
        // 尝试获取关联的 SpriteAtlas 数据（保留英文原名：GetSpriteAtlas / spriteAtlas）

        AssetPPtr texturePtr;
        // 声明局部变量 texturePtr（保留英文原名：AssetPPtr / texturePtr）

        if (spriteAtlas != null)
        {
            texturePtr = spriteAtlas.texture;
        }
        else
        {
            texturePtr = AssetPPtr.FromField(renderData["texture"]);
            if (texturePtr.IsNull())
            {
                return null;
            }
        }
        // 如果存在 atlas 则使用 atlas 的 texture 指针，否则从 renderData 中读取 texture 指针并验证（保留英文原名：spriteAtlas / AssetPPtr.FromField）

        var textureAsset = workspace.GetAssetInst(asset.FileInstance, texturePtr.FileId, texturePtr.PathId);
        // 根据 texturePtr 在工作区中获取对应的 AssetInst（保留英文原名：GetAssetInst / texturePtr.FileId / texturePtr.PathId）

        if (textureAsset == null)
        {
            return null;
        }
        // 如果找不到对应的纹理资产则返回 null（保留英文原名：textureAsset）

        // we use skia so we can crop, then convert to avalonia bitmap at the end
        // 注释：说明使用 Skia 进行裁剪与绘制，然后转换为 Avalonia Bitmap（保留英文原注释）

        SKImage baseImage;
        // 声明 baseImage（SKImage）变量用于后续绘制（保留英文原名：SKImage / baseImage）

        if (_spriteImageCache.TryGetValue(textureAsset, out var cachedBitmap))
        {
            baseImage = cachedBitmap;
        }
        else
        {
            var textureEditBf = TextureHelper.GetByteArrayTexture(workspace, textureAsset);
            var texture = TextureFile.ReadTextureFile(textureEditBf);
            format = (TextureFormat)texture.m_TextureFormat;

            TextureHelper.SwizzleOptIn(texture, textureAsset.FileInstance.file);

            var encTextureData = texture.FillPictureData(textureAsset.FileInstance);
            var textureData = texture.DecodeTextureRaw(encTextureData);
            if (textureData == null)
            {
                return null;
            }

            var baseBitmap = new SKBitmap(texture.m_Width, texture.m_Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            using var basePixels = baseBitmap.PeekPixels();
            var basePixelsSpan = basePixels.GetPixelSpan<byte>();
            MemoryExtensions.CopyTo(textureData, basePixelsSpan);

            baseImage = SKImage.FromBitmap(baseBitmap);

            // just like the lz4 block decoder, this only pulls whichever item
            // was added earliest since we can't reset the position of elements
            // with a stock .net queue
            // 注释：说明缓存淘汰策略：使用队列按插入顺序淘汰最早的项（保留英文原注释）

            if (_spriteImageQueue.Count >= DEFAULT_MAX_SPRITE_IMAGE_CACHE_SIZE)
            {
                var lastKey = _spriteImageQueue.Dequeue();
                var lastValue = _spriteImageCache[lastKey];
                lastValue.Dispose();
                _spriteImageCache.Remove(lastKey);
            }
            // 如果缓存已满，出队最早的键并释放对应的 SKImage，然后从字典中移除（保留英文原名：Dequeue / Dispose / Remove）

            _spriteImageCache[textureAsset] = baseImage;
            _spriteImageQueue.Enqueue(textureAsset);
            // 将新解码的 baseImage 缓存并将键入队以记录顺序（保留英文原名：Enqueue）
        }

        var pixelsToUnits = spriteBf["m_PixelsToUnits"].AsFloat;
        // 读取 sprite 的像素到单位转换比例（m_PixelsToUnits）（保留英文原名：pixelsToUnits）

        var pivot = spriteBf["m_Pivot"];
        // 读取 sprite 的 pivot 字段（保留英文原名：pivot）

        var pivotX = pivot["x"].AsFloat;
        // 读取 pivot 的 x 分量（保留英文原名：pivotX）

        var pivotY = pivot["y"].AsFloat;
        // 读取 pivot 的 y 分量（保留英文原名：pivotY）

        var rect = spriteBf["m_Rect"];
        // 读取 sprite 的 rect 字段（保留英文原名：rect）

        var rectWidth = rect["width"].AsFloat;
        // 读取 rect 的宽度（保留英文原名：rectWidth）

        var rectHeight = rect["height"].AsFloat;
        // 读取 rect 的高度（保留英文原名：rectHeight）

        float textureRectOffsetX, textureRectOffsetY;
        // 声明纹理矩形偏移量变量（保留英文原名：textureRectOffsetX / textureRectOffsetY）

        float textureRectX, textureRectY, textureRectWidth, textureRectHeight;
        // 声明纹理矩形位置与尺寸变量（保留英文原名：textureRectX / textureRectY / textureRectWidth / textureRectHeight）

        uint settingsRaw;
        // 声明 settingsRaw（原始设置位掩码）变量（保留英文原名：settingsRaw）

        if (spriteAtlas != null)
        {
            textureRectX = spriteAtlas.textureRectX;
            textureRectY = spriteAtlas.textureRectY;
            textureRectWidth = spriteAtlas.textureRectWidth;
            textureRectHeight = spriteAtlas.textureRectHeight;

            textureRectOffsetX = spriteAtlas.textureRectOffsetX;
            textureRectOffsetY = spriteAtlas.textureRectOffsetY;

            settingsRaw = spriteAtlas.settingsRaw;
        }
        else
        {
            var textureRect = renderData["textureRect"];
            textureRectX = (float)Math.Floor(textureRect["x"].AsFloat);
            textureRectY = (float)Math.Floor(textureRect["y"].AsFloat);
            textureRectWidth = (float)Math.Ceiling(textureRect["width"].AsFloat);
            textureRectHeight = (float)Math.Ceiling(textureRect["height"].AsFloat);

            var textureRectOffset = renderData["textureRectOffset"];
            textureRectOffsetX = textureRectOffset["x"].AsFloat;
            textureRectOffsetY = textureRectOffset["y"].AsFloat;

            settingsRaw = renderData["settingsRaw"].AsUInt;
        }
        // 如果存在 atlas 则从 atlas 中读取纹理矩形与偏移，否则从 renderData 中读取并做向下/向上取整处理（保留英文原名：Math.Floor / Math.Ceiling）

        // todo
        // 注释：TODO：可能还有其他设置需要处理（保留英文原注释）

        var flipX = (settingsRaw & 4) != 0;
        // 从 settingsRaw 位掩码中判断是否需要水平翻转（保留英文原名：flipX）

        var flipY = (settingsRaw & 8) != 0;
        // 从 settingsRaw 位掩码中判断是否需要垂直翻转（保留英文原名：flipY）

        var rot90 = (settingsRaw & 16) != 0;
        // 从 settingsRaw 位掩码中判断是否需要旋转 90 度（保留英文原名：rot90）

        // full crop: bounded by the sprite texture rect
        // regular crop: bounded by the sprite's working area
        // 注释：说明 fullCrop 与常规裁剪的区别（保留英文原注释）

        SKBitmap croppedBitmap;
        // 声明将要创建的裁剪后位图（保留英文原名：croppedBitmap）

        if (fullCrop)
            croppedBitmap = new SKBitmap((int)Math.Round(textureRectWidth), (int)Math.Round(textureRectHeight));
        else
            croppedBitmap = new SKBitmap((int)Math.Round(rectWidth), (int)Math.Round(rectHeight));
        // 根据 fullCrop 决定裁剪位图的尺寸（保留英文原名：SKBitmap / Math.Round）

        var version = asset.FileInstance.file.Metadata.UnityVersion;
        // 读取所属文件的 Unity 版本字符串（保留英文原名：UnityVersion）

        var mesh = new MeshObj(asset.FileInstance, renderData, new UnityVersion(version));
        // 使用 MeshObj 构造器根据 renderData 构建网格对象（保留英文原名：MeshObj / UnityVersion）

        if (mesh.Vertices.Length % 3 != 0)
        {
            return null;
        }
        // 验证网格顶点数量是否为 3 的倍数（每个三角形 3 个顶点），否则返回 null（保留英文原名：mesh.Vertices）

        using (var canvas = new SKCanvas(croppedBitmap))
        {
            canvas.Clear(SKColors.Transparent);
            using (var path = new SKPath())
            {
                var offX = rectWidth * pivotX;
                var offY = rectHeight * pivotY;
                if (fullCrop)
                {
                    offX -= textureRectOffsetX;
                    offY -= textureRectOffsetY;
                }

                for (var i = 0; i < mesh.Indices.Length; i += 3)
                {
                    var pointAIdx = mesh.Indices[i] * 3;
                    var pointBIdx = mesh.Indices[i + 1] * 3;
                    var pointCIdx = mesh.Indices[i + 2] * 3;
                    var pointA = new SKPoint(
                        mesh.Vertices[pointAIdx] * pixelsToUnits + offX,
                        mesh.Vertices[pointAIdx + 1] * pixelsToUnits + offY
                    );
                    var pointB = new SKPoint(
                        mesh.Vertices[pointBIdx] * pixelsToUnits + offX,
                        mesh.Vertices[pointBIdx + 1] * pixelsToUnits + offY
                    );
                    var pointC = new SKPoint(
                        mesh.Vertices[pointCIdx] * pixelsToUnits + offX,
                        mesh.Vertices[pointCIdx + 1] * pixelsToUnits + offY
                    );
                    var points = new SKPoint[] { pointA, pointB, pointC };
                    path.AddPoly(points);
                }
                canvas.ClipPath(path);

                float xOff;
                float yOff;
                if (flipX)
                {
                    canvas.Translate(croppedBitmap.Width, 0);
                    canvas.Scale(-1, 1);
                    xOff = fullCrop
                        ? -textureRectX
                        : -textureRectX + rectWidth - textureRectWidth - textureRectOffsetX;
                }
                else
                {
                    xOff = fullCrop
                        ? -textureRectX
                        : -textureRectX + textureRectOffsetX;
                }

                if (flipY)
                {
                    canvas.Translate(0, croppedBitmap.Height);
                    canvas.Scale(1, -1);
                    yOff = fullCrop
                        ? -textureRectY
                        : -textureRectY + rectHeight - textureRectHeight - textureRectOffsetY;
                }
                else
                {
                    yOff = fullCrop
                        ? -textureRectY
                        : -textureRectY + textureRectOffsetY;
                }
                // todo: rot90

                canvas.DrawImage(baseImage, xOff, yOff);
            }
        }
        // 使用 SkiaCanvas 在裁剪位图上绘制：构建裁剪路径（基于网格索引与顶点），设置翻转/偏移，然后将 baseImage 绘制到画布上（保留英文原名：SKCanvas / SKPath / DrawImage）

        return croppedBitmap;
        // 返回裁剪并绘制好的 SKBitmap（保留英文原名：croppedBitmap）
    }

    private SpriteAtlasData? GetSpriteAtlas(Workspace workspace, AssetInst asset, AssetTypeValueField spriteBf)
    // 私有方法 GetSpriteAtlas：尝试解析并返回与 sprite 关联的 SpriteAtlasData（保留英文原名：GetSpriteAtlas / SpriteAtlasData）
    {
        var spriteAtlas = spriteBf["m_SpriteAtlas"];
        // 读取 sprite 字段中的 m_SpriteAtlas（保留英文原名：spriteAtlas）

        var spriteAtlasPtr = AssetPPtr.FromField(spriteAtlas);
        // 将字段解析为 AssetPPtr（保留英文原名：AssetPPtr.FromField / spriteAtlasPtr）

        if (spriteAtlasPtr.IsNull())
        {
            var atlasTags = spriteBf["m_AtlasTags.Array"];
            if (atlasTags.Children.Count == 0)
            {
                // nothing we can do. there's no reference to an atlas/texture anywhere.
                return null;
            }

            // in some games, m_SpriteAtlas is not set, but a SpriteAtlas in the same
            // file references this sprite. m_AtlasTags has a list of atlas names.
            // I am not sure why this list would have multiple entries. this field may
            // have more than one only in an editor project.
            // 注释：说明某些情况下 m_SpriteAtlas 未设置，但 m_AtlasTags 列表包含 atlas 名称（保留英文原注释）

            var atlasTag = atlasTags[0].AsString;
            // 取第一个 atlas 标签作为候选（保留英文原名：atlasTag）

            // we're going to assume the sprite atlas is always in the same file.
            // it would probably be good to do a last resort option, but tbd on that.
            // 注释：假设 atlas 在同一文件中（保留英文原注释）

            var atlasNameLookup = GetSpriteAtlasNameLookup(workspace, asset.FileInstance);
            // 获取当前文件中 atlas 名称到 AssetInst 的查找表（保留英文原名：GetSpriteAtlasNameLookup）

            if (!atlasNameLookup.TryGetValue(atlasTag, out var atlasAsset))
            {
                // nothing we can do. give up.
                return null;
            }

            spriteAtlasPtr = new AssetPPtr(0, atlasAsset.PathId);
        }

        spriteAtlasPtr.SetFilePathFromFile(workspace.Manager, asset.FileInstance);
        // 为 spriteAtlasPtr 设置文件路径信息（保留英文原名：SetFilePathFromFile）

        var key = SpriteAtlasLookup.MakeRenderKeyGuid(spriteBf["m_RenderDataKey"]["first"]);
        // 生成用于查找 atlas 渲染数据的 key（保留英文原名：MakeRenderKeyGuid / m_RenderDataKey）

        var atlasData = _spriteAtlasLookup.GetAtlasData(spriteAtlasPtr, key);
        // 尝试从 _spriteAtlasLookup 获取已缓存的 atlas 数据（保留英文原名：GetAtlasData）

        if (atlasData != null)
        {
            return atlasData;
        }
        // 如果缓存命中则直接返回（保留英文原名：atlasData）

        var spriteAtlasBf = workspace.GetBaseField(asset.FileInstance, spriteAtlasPtr.FileId, spriteAtlasPtr.PathId);
        // 否则从工作区读取 atlas 的 BaseField（保留英文原名：GetBaseField / spriteAtlasPtr）

        if (spriteAtlasBf == null)
        {
            return null;
        }
        // 如果无法读取 atlas 字段则返回 null（保留英文原名：spriteAtlasBf）

        _spriteAtlasLookup.AddSpriteAtlas(spriteAtlasPtr, spriteAtlasBf);
        // 将读取到的 atlas 字段添加到 _spriteAtlasLookup 缓存中（保留英文原名：AddSpriteAtlas）

        return _spriteAtlasLookup.GetAtlasData(spriteAtlasPtr, key);
        // 再次从缓存中获取并返回 atlas 数据（保留英文原名：GetAtlasData）
    }

    private Dictionary<string, AssetInst> GetSpriteAtlasNameLookup(Workspace workspace, AssetsFileInstance fileInstance)
    // 私有方法 GetSpriteAtlasNameLookup：为指定文件构建 atlas 名称到 AssetInst 的映射并缓存（保留英文原名：GetSpriteAtlasNameLookup）
    {
        if (_nameToSpriteAtlasLookup.TryGetValue(fileInstance, out var nameLookup))
        {
            return nameLookup;
        }
        // 如果缓存中已有则直接返回（保留英文原名：_nameToSpriteAtlasLookup）

        var atlasNameLookup = new Dictionary<string, AssetInst>();
        // 新建字典用于存放 atlas 名称到 AssetInst 的映射（保留英文原名：atlasNameLookup）

        foreach (var atlasInf in fileInstance.file.GetAssetsOfType(AssetClassID.SpriteAtlas))
        {
            var atlasAsset = workspace.GetAssetInst(fileInstance, 0, atlasInf.PathId);
            if (atlasAsset is null)
                continue;

            var atlasBf = workspace.GetBaseField(atlasAsset);
            if (atlasBf is null)
                continue;

            var atlasTag = atlasBf["m_Tag"].AsString;
            atlasNameLookup[atlasTag] = atlasAsset;
        }
        // 遍历文件中所有 SpriteAtlas 类型的资产，读取其 m_Tag 并建立名称到 AssetInst 的映射（保留英文原名：GetAssetsOfType / m_Tag）

        _nameToSpriteAtlasLookup[fileInstance] = atlasNameLookup;
        // 将构建好的映射缓存到 _nameToSpriteAtlasLookup（保留英文原名：_nameToSpriteAtlasLookup）

        return atlasNameLookup;
        // 返回映射（保留英文原名：atlasNameLookup）
    }

    public static Bitmap? GetTexture2DBitmap(Workspace workspace, AssetInst asset, out TextureFormat format)
    // 公共静态方法 GetTexture2DBitmap：直接从 Texture2D 资产生成 Avalonia Bitmap（保留英文原名：GetTexture2DBitmap）
    {
        var textureEditBf = TextureHelper.GetByteArrayTexture(workspace, asset);
        // 获取纹理的 baseField（字节数组字段）（保留英文原名：TextureHelper.GetByteArrayTexture）

        var texture = TextureFile.ReadTextureFile(textureEditBf);
        // 解析 baseField 为 TextureFile（保留英文原名：TextureFile.ReadTextureFile）

        format = (TextureFormat)texture.m_TextureFormat;
        // 输出纹理格式（保留英文原名：m_TextureFormat）

        TextureHelper.SwizzleOptIn(texture, asset.FileInstance.file);
        // 根据文件或配置对 texture 应用 swizzle（颜色通道重排）选项（保留英文原名：SwizzleOptIn）

        var encTextureData = texture.FillPictureData(asset.FileInstance);
        // 获取编码后的纹理数据（保留英文原名：FillPictureData）

        // rare, but sometimes we see large textures with 0 texture data size
        // 注释：说明极少数情况下会遇到编码数据长度为 0 的情况（保留英文原注释）

        if (encTextureData.Length == 0 || (texture.m_Width == 0 && texture.m_Height == 0))
        {
            return null;
        }
        // 如果编码数据为空或纹理尺寸为 0x0，则返回 null（保留英文原名：encTextureData / m_Width / m_Height）

        var textureData = texture.DecodeTextureRaw(encTextureData);
        // 解码原始纹理数据为像素字节数组（保留英文原名：DecodeTextureRaw）

        if (textureData == null)
        {
            return null;
        }
        // 如果解码失败则返回 null（保留英文原名：textureData）

        var bitmap = new WriteableBitmap(new PixelSize(texture.m_Width, texture.m_Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Unpremul);
        // 创建 Avalonia 的 WriteableBitmap 用于返回（保留英文原名：WriteableBitmap）

        using (var frameBuffer = bitmap.Lock())
        {
            Marshal.Copy(textureData, 0, frameBuffer.Address, textureData.Length);
        }
        // 将解码后的像素数据复制到 WriteableBitmap 的帧缓冲（保留英文原名：Marshal.Copy / frameBuffer.Address）

        return bitmap;
        // 返回生成的 Bitmap（保留英文原名：bitmap）
    }

    public void Cleanup()
    // 公共方法 Cleanup：释放缓存与资源（保留英文原名：Cleanup）
    {
        foreach (var bitmap in _spriteImageCache.Values)
        {
            bitmap.Dispose();
        }
        // 释放缓存中所有 SKImage（保留英文原名：Dispose）

        _spriteImageCache.Clear();
        // 清空缓存字典（保留英文原名：Clear）

        _spriteImageQueue.Clear();
        // 清空缓存队列（保留英文原名：Clear）

        _spriteAtlasLookup.Clear();
        // 清空 atlas 查找缓存（保留英文原名：_spriteAtlasLookup.Clear）

        _nameToSpriteAtlasLookup.Clear();
        // 清空按文件的 atlas 名称查找缓存（保留英文原名：_nameToSpriteAtlasLookup.Clear）
    }
}
// 类体结束（TextureLoader）
