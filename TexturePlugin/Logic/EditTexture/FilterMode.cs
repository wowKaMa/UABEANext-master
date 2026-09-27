namespace TexturePlugin.Logic.EditTexture;
// 定义命名空间 TexturePlugin.Logic.EditTexture，用于组织与编辑纹理相关的逻辑（保留英文原名：TexturePlugin.Logic.EditTexture）

public enum FilterMode
// 定义公共枚举 FilterMode，表示纹理的过滤模式（保留英文原名：FilterMode）
{
    // 枚举体开始（FilterMode）

    Point,
    // 枚举成员 Point：表示点采样（最近邻）过滤模式，像素不进行插值，适合像素风格或需要精确像素边界的场景（保留英文原名：Point）

    Bilinear,
    // 枚举成员 Bilinear：表示双线性过滤模式，对相邻 2×2 像素做线性插值以平滑缩放，常用于一般纹理缩放（保留英文原名：Bilinear）

    Trilinear
    // 枚举成员 Trilinear：表示三线性过滤模式，在双线性基础上对不同 mip 级别之间再做线性插值，能进一步减少纹理切换时的突变（保留英文原名：Trilinear）

}
// 枚举体结束（FilterMode）
