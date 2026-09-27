using AssetsTools.NET;
// 引用 AssetsTools.NET 库，用于处理 Unity 资产文件（保留英文原名：AssetsTools.NET）

namespace TexturePlugin.Helpers;
// 定义命名空间 TexturePlugin.Helpers，用于组织与纹理相关的辅助类（保留英文原名：TexturePlugin.Helpers）

public class SpriteAtlasLookup
// 定义公共类 SpriteAtlasLookup：用于缓存与查找 SpriteAtlas 渲染数据（保留英文原名：SpriteAtlasLookup）
{
    // 类体开始（SpriteAtlasLookup）

    public readonly Dictionary<AssetPPtr, Dictionary<GUID128, SpriteAtlasData>> _lookup = [];
    // 公共只读字段 _lookup：字典，键为 AssetPPtr（表示 atlas 引用），值为另一个字典（键为 GUID128，值为 SpriteAtlasData），用于缓存 atlas 渲染数据（保留英文原名：_lookup / AssetPPtr / GUID128 / SpriteAtlasData）

    public SpriteAtlasData? GetAtlasData(AssetPPtr atlasPtr, GUID128 key)
    // 公共方法 GetAtlasData：根据 atlas 指针（atlasPtr）和渲染键（key）查找并返回对应的 SpriteAtlasData（保留英文原名：GetAtlasData / AssetPPtr / GUID128）
    {
        if (_lookup.TryGetValue(atlasPtr, out var atlasLookup))
        {
            if (atlasLookup.TryGetValue(key, out var atlasData))
            {
                return atlasData;
            }
        }
        return null;
    }
    // 逻辑说明：先尝试从 _lookup 中取出 atlasPtr 对应的内部字典（atlasLookup），若存在再尝试用 key 取出 atlasData，若都成功则返回 atlasData，否则返回 null（保留英文原名：TryGetValue / atlasLookup / atlasData）

    public void AddSpriteAtlas(AssetPPtr atlasPtr, AssetTypeValueField atlasBf)
    // 公共方法 AddSpriteAtlas：将解析得到的 atlasBf（AssetTypeValueField）中的渲染数据解析并加入缓存（保留英文原名：AddSpriteAtlas / AssetTypeValueField）
    {
        if (_lookup.ContainsKey(atlasPtr))
        {
            return;
        }
        // 如果缓存中已经存在该 atlasPtr，则直接返回（保留英文原名：ContainsKey）

        var pairs = atlasBf["m_RenderDataMap.Array"];
        // 从 atlas 的字段（atlasBf）中读取 m_RenderDataMap.Array（包含若干键值对），并赋给 pairs（保留英文原名：atlasBf / "m_RenderDataMap.Array" / pairs）

        _lookup[atlasPtr] = [];
        // 在 _lookup 中为该 atlasPtr 初始化一个空的内部字典（保留英文原名：_lookup / atlasPtr）

        foreach (var pair in pairs)
        {
            var guidField = pair["first.first"];
            var key = MakeRenderKeyGuid(guidField);
            var value = new SpriteAtlasData(pair["second"]);
            _lookup[atlasPtr][key] = value;
        }
    }
    // 逻辑说明：遍历 pairs（每个 pair 表示一条渲染数据映射），从 pair 中取出 guid 字段（pair["first.first"]），用 MakeRenderKeyGuid 生成 GUID128 作为 key，再用 pair["second"] 构造 SpriteAtlasData 作为 value，最后将 key/value 存入 _lookup[atlasPtr]（保留英文原名：foreach / guidField / MakeRenderKeyGuid / SpriteAtlasData）

    public void Clear()
    // 公共方法 Clear：清空缓存（保留英文原名：Clear）
    {
        _lookup.Clear();
    }
    // 逻辑说明：调用字典的 Clear 方法移除所有缓存项（保留英文原名：Clear）

    public static GUID128 MakeRenderKeyGuid(AssetTypeValueField field)
    // 公共静态方法 MakeRenderKeyGuid：从给定的字段（field）构造一个 GUID128（保留英文原名：MakeRenderKeyGuid / GUID128 / AssetTypeValueField）
    {
        return new GUID128()
        {
            data0 = field[0].AsUInt,
            data1 = field[1].AsUInt,
            data2 = field[2].AsUInt,
            data3 = field[3].AsUInt,
        };
    }
    // 逻辑说明：从 field 的前四个子字段读取无符号整数字段（AsUInt），分别赋给 GUID128 的 data0..data3，返回该 GUID128（保留英文原名：data0 / AsUInt）
}
// SpriteAtlasLookup 类结束（SpriteAtlasLookup）

public class SpriteAtlasData
// 定义公共类 SpriteAtlasData：表示单个 atlas 渲染数据项（保留英文原名：SpriteAtlasData）
{
    // 类体开始（SpriteAtlasData）

    public AssetPPtr texture;
    // 公共字段 texture：指向 atlas 使用的主纹理（保留英文原名：texture / AssetPPtr）

    public AssetPPtr alphaTexture;
    // 公共字段 alphaTexture：指向 alpha 纹理（若有）（保留英文原名：alphaTexture / AssetPPtr）

    // todo: make these vectors
    // 注释：TODO：可以把下面的多个 float 合并为向量类型以便管理（保留英文原注释）

    public float textureRectX;
    // 公共字段 textureRectX：纹理矩形的 X 坐标（保留英文原名：textureRectX）

    public float textureRectY;
    // 公共字段 textureRectY：纹理矩形的 Y 坐标（保留英文原名：textureRectY）

    public float textureRectWidth;
    // 公共字段 textureRectWidth：纹理矩形的宽度（保留英文原名：textureRectWidth）

    public float textureRectHeight;
    // 公共字段 textureRectHeight：纹理矩形的高度（保留英文原名：textureRectHeight）

    public float textureRectOffsetX;
    // 公共字段 textureRectOffsetX：纹理矩形的 X 偏移（保留英文原名：textureRectOffsetX）

    public float textureRectOffsetY;
    // 公共字段 textureRectOffsetY：纹理矩形的 Y 偏移（保留英文原名：textureRectOffsetY）

    public float atlasRectOffsetX;
    // 公共字段 atlasRectOffsetX：atlas 矩形的 X 偏移（保留英文原名：atlasRectOffsetX）

    public float atlasRectOffsetY;
    // 公共字段 atlasRectOffsetY：atlas 矩形的 Y 偏移（保留英文原名：atlasRectOffsetY）

    public float uvTransformX;
    // 公共字段 uvTransformX：UV 变换矩阵的 X 分量（保留英文原名：uvTransformX）

    public float uvTransformY;
    // 公共字段 uvTransformY：UV 变换矩阵的 Y 分量（保留英文原名：uvTransformY）

    public float uvTransformZ;
    // 公共字段 uvTransformZ：UV 变换矩阵的 Z 分量（保留英文原名：uvTransformZ）

    public float uvTransformW;
    // 公共字段 uvTransformW：UV 变换矩阵的 W 分量（保留英文原名：uvTransformW）

    public float downscaleMultiplier;
    // 公共字段 downscaleMultiplier：缩放倍率（用于 atlas 缩放或降采样）（保留英文原名：downscaleMultiplier）

    public uint settingsRaw;
    // 公共字段 settingsRaw：原始设置位掩码（包含翻转/旋转等标志）（保留英文原名：settingsRaw）

    public SpriteAtlasData(AssetTypeValueField field)
    // 构造函数 SpriteAtlasData：从 AssetTypeValueField（field）解析并填充所有字段（保留英文原名：SpriteAtlasData / AssetTypeValueField）
    {
        texture = AssetPPtr.FromField(field["texture"]);
        // 从 field["texture"] 解析 AssetPPtr 并赋给 texture（保留英文原名：AssetPPtr.FromField / "texture"）

        alphaTexture = AssetPPtr.FromField(field["alphaTexture"]);
        // 从 field["alphaTexture"] 解析 AssetPPtr 并赋给 alphaTexture（保留英文原名："alphaTexture"）

        var textureRect = field["textureRect"];
        // 读取 textureRect 子字段（保留英文原名：textureRect）

        textureRectX = textureRect["x"].AsFloat;
        // 从 textureRect 的 x 子字段读取浮点值并赋给 textureRectX（保留英文原名：AsFloat）

        textureRectY = textureRect["y"].AsFloat;
        // 从 textureRect 的 y 子字段读取浮点值并赋给 textureRectY（保留英文原名：AsFloat）

        textureRectWidth = textureRect["width"].AsFloat;
        // 从 textureRect 的 width 子字段读取浮点值并赋给 textureRectWidth（保留英文原名：AsFloat）

        textureRectHeight = textureRect["height"].AsFloat;
        // 从 textureRect 的 height 子字段读取浮点值并赋给 textureRectHeight（保留英文原名：AsFloat）

        var textureRectOffset = field["textureRectOffset"];
        // 读取 textureRectOffset 子字段（保留英文原名：textureRectOffset）

        textureRectOffsetX = textureRectOffset["x"].AsFloat;
        // 从 textureRectOffset 的 x 子字段读取浮点值并赋给 textureRectOffsetX（保留英文原名：AsFloat）

        textureRectOffsetY = textureRectOffset["y"].AsFloat;
        // 从 textureRectOffset 的 y 子字段读取浮点值并赋给 textureRectOffsetY（保留英文原名：AsFloat）

        var atlasRectOffset = field["atlasRectOffset"];
        // 读取 atlasRectOffset 子字段（保留英文原名：atlasRectOffset）

        atlasRectOffsetX = atlasRectOffset["x"].AsFloat;
        // 从 atlasRectOffset 的 x 子字段读取浮点值并赋给 atlasRectOffsetX（保留英文原名：AsFloat）

        atlasRectOffsetY = atlasRectOffset["y"].AsFloat;
        // 从 atlasRectOffset 的 y 子字段读取浮点值并赋给 atlasRectOffsetY（保留英文原名：AsFloat）

        var uvTransform = field["uvTransform"];
        // 读取 uvTransform 子字段（保留英文原名：uvTransform）

        uvTransformX = uvTransform["x"].AsFloat;
        // 从 uvTransform 的 x 子字段读取浮点值并赋给 uvTransformX（保留英文原名：AsFloat）

        uvTransformY = uvTransform["y"].AsFloat;
        // 从 uvTransform 的 y 子字段读取浮点值并赋给 uvTransformY（保留英文原名：AsFloat）

        uvTransformZ = uvTransform["z"].AsFloat;
        // 从 uvTransform 的 z 子字段读取浮点值并赋给 uvTransformZ（保留英文原名：AsFloat）

        uvTransformW = uvTransform["w"].AsFloat;
        // 从 uvTransform 的 w 子字段读取浮点值并赋给 uvTransformW（保留英文原名：AsFloat）

        downscaleMultiplier = field["downscaleMultiplier"].AsFloat;
        // 从 field 的 downscaleMultiplier 子字段读取浮点值并赋给 downscaleMultiplier（保留英文原名：downscaleMultiplier）

        settingsRaw = field["settingsRaw"].AsUInt;
        // 从 field 的 settingsRaw 子字段读取无符号整数并赋给 settingsRaw（保留英文原名：settingsRaw / AsUInt）
    }
    // 构造函数结束（SpriteAtlasData）
}
// SpriteAtlasData 类结束（SpriteAtlasData）
