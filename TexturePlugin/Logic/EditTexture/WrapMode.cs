namespace TexturePlugin.Logic.EditTexture;
// 定义命名空间 TexturePlugin.Logic.EditTexture，用于组织与编辑纹理相关的逻辑（保留英文原名：TexturePlugin.Logic.EditTexture）

public enum WrapMode
// 定义公共枚举 WrapMode，表示纹理的环绕/重复模式（保留英文原名：WrapMode）
{
    // 枚举体开始（WrapMode）

    Repeat,
    // 枚举成员 Repeat：表示纹理在超出 UV 范围时重复平铺（重复环绕模式）（保留英文原名：Repeat）

    Clamp,
    // 枚举成员 Clamp：表示纹理在超出 UV 范围时夹紧到边缘像素（边缘拉伸/夹紧模式）（保留英文原名：Clamp）

    Mirror,
    // 枚举成员 Mirror：表示纹理在超出 UV 范围时镜像翻转重复（交替镜像平铺模式）（保留英文原名：Mirror）

    MirrorOnce
    // 枚举成员 MirrorOnce：表示纹理只在第一次超出时镜像一次，之后通常保持边缘（单次镜像模式）（保留英文原名：MirrorOnce）
}
// 枚举体结束（WrapMode）
