namespace UABEANext4.Logic.Mesh
// 定义命名空间 UABEANext4.Logic.Mesh，用于组织与网格（mesh）相关的枚举和类型（保留英文原名：UABEANext4.Logic.Mesh）

{
    public enum VertexChannelFormat
    // 定义公共枚举 VertexChannelFormat，表示顶点通道的原始数据通道格式（保留英文原名：VertexChannelFormat）
    {
        Float,
        // 枚举成员 Float：表示通道以 32 位浮点数存储（保留英文原名：Float）

        Float16,
        // 枚举成员 Float16：表示通道以 16 位半精度浮点数存储（保留英文原名：Float16）

        Color,
        // 枚举成员 Color：表示通道以颜色格式存储（通常为 4 字节 RGBA 或归一化格式）（保留英文原名：Color）

        Byte,
        // 枚举成员 Byte：表示通道以单字节（8 位）整数存储（保留英文原名：Byte）

        UInt32
        // 枚举成员 UInt32：表示通道以 32 位无符号整数存储（保留英文原名：UInt32）
    }

    public enum VertexFormatV2
    // 定义公共枚举 VertexFormatV2，表示统一的顶点分量格式（较新的或统一后的格式集合）（保留英文原名：VertexFormatV2）
    {
        Float,
        // 每分量 32 位浮点（Float）

        Float16,
        // 每分量 16 位半精度浮点（Float16）

        UNorm8,
        // 每分量 8 位无符号归一化（0..255 映射到 0.0..1.0）（UNorm8）

        SNorm8,
        // 每分量 8 位有符号归一化（-128..127 映射到 -1.0..1.0）（SNorm8）

        UNorm16,
        // 每分量 16 位无符号归一化（UNorm16）

        SNorm16,
        // 每分量 16 位有符号归一化（SNorm16）

        UInt8,
        // 每分量 8 位无符号整数（UInt8）

        SInt8,
        // 每分量 8 位有符号整数（SInt8）

        UInt16,
        // 每分量 16 位无符号整数（UInt16）

        SInt16,
        // 每分量 16 位有符号整数（SInt16）

        UInt32,
        // 每分量 32 位无符号整数（UInt32）

        SInt32
        // 每分量 32 位有符号整数（SInt32）
    }

    public enum VertexFormatV1
    // 定义公共枚举 VertexFormatV1，表示旧版本 Unity 使用的顶点格式编码（保留英文原名：VertexFormatV1）
    {
        Float,
        // 32 位浮点（Float）

        Float16,
        // 16 位半精度浮点（Float16）

        Color,
        // 颜色格式（Color）

        UNorm8,
        // 8 位无符号归一化（UNorm8）

        SNorm8,
        // 8 位有符号归一化（SNorm8）

        UNorm16,
        // 16 位无符号归一化（UNorm16）

        SNorm16,
        // 16 位有符号归一化（SNorm16）

        UInt8,
        // 8 位无符号整数（UInt8）

        SInt8,
        // 8 位有符号整数（SInt8）

        UInt16,
        // 16 位无符号整数（UInt16）

        SInt16,
        // 16 位有符号整数（SInt16）

        UInt32,
        // 32 位无符号整数（UInt32）

        SInt32
        // 32 位有符号整数（SInt32）
    }

    public enum ChannelTypeV3
    // 定义公共枚举 ChannelTypeV3，表示 Unity 较新版本（V3）中通道的语义类型（保留英文原名：ChannelTypeV3）
    {
        Vertex,
        // 通道表示顶点位置（Vertex）

        Normal,
        // 通道表示法线（Normal）

        Tangent,
        // 通道表示切线（Tangent）

        Color,
        // 通道表示顶点颜色（Color）

        TexCoord0,
        // 第 0 个纹理坐标通道（TexCoord0）

        TexCoord1,
        // 第 1 个纹理坐标通道（TexCoord1）

        TexCoord2,
        // 第 2 个纹理坐标通道（TexCoord2）

        TexCoord3,
        // 第 3 个纹理坐标通道（TexCoord3）

        TexCoord4,
        // 第 4 个纹理坐标通道（TexCoord4）

        TexCoord5,
        // 第 5 个纹理坐标通道（TexCoord5）

        TexCoord6,
        // 第 6 个纹理坐标通道（TexCoord6）

        TexCoord7,
        // 第 7 个纹理坐标通道（TexCoord7）

        BlendWeight,
        // 骨骼混合权重通道（BlendWeight），用于蒙皮（skinning）

        BlendIndices,
        // 骨骼索引通道（BlendIndices），用于蒙皮（skinning）
    }

    public enum ChannelTypeV2
    // 定义公共枚举 ChannelTypeV2，表示 Unity 较旧版本（V2）中通道的语义类型（保留英文原名：ChannelTypeV2）
    {
        Vertex,
        // 顶点位置（Vertex）

        Normal,
        // 法线（Normal）

        Color,
        // 颜色（Color）

        TexCoord0,
        // 纹理坐标 0（TexCoord0）

        TexCoord1,
        // 纹理坐标 1（TexCoord1）

        TexCoord2,
        // 纹理坐标 2（TexCoord2）

        TexCoord3,
        // 纹理坐标 3（TexCoord3）

        Tangent,
        // 切线（Tangent）
    }

    public enum ChannelTypeV1
    // 定义公共枚举 ChannelTypeV1，表示更旧版本（V1）中通道的语义类型（保留英文原名：ChannelTypeV1）
    {
        Vertex,
        // 顶点位置（Vertex）

        Normal,
        // 法线（Normal）

        Color,
        // 颜色（Color）

        TexCoord0,
        // 纹理坐标 0（TexCoord0）

        TexCoord1,
        // 纹理坐标 1（TexCoord1）

        Tangent,
        // 切线（Tangent）
    }
}
