namespace AudioPlugin;
// 定义命名空间：AudioPlugin（用于组织与音频导出相关的类型；保留英文原名：AudioPlugin）

public enum CompressionFormat
// 定义公共枚举：CompressionFormat（表示音频压缩格式的枚举；保留英文原名：CompressionFormat）
{
    // 枚举体开始（CompressionFormat）

    PCM,
    // 枚举成员 PCM：表示未压缩的脉冲编码调制音频（通常对应 .wav PCM 格式；保留英文原名：PCM）

    Vorbis,
    // 枚举成员 Vorbis：表示使用 Vorbis 编码的音频（通常对应 .ogg 容器；保留英文原名：Vorbis）

    ADPCM,
    // 枚举成员 ADPCM：表示自适应差分脉冲编码调制（常见于某些游戏音频；保留英文原名：ADPCM）

    MP3,
    // 枚举成员 MP3：表示使用 MPEG-1/2 Audio Layer III 压缩的音频（常见的有损格式；保留英文原名：MP3）

    VAG,
    // 枚举成员 VAG：表示 PlayStation 等平台使用的专有音频格式（通常标记为 VAG；保留英文原名：VAG）

    HEVAG,
    // 枚举成员 HEVAG：表示 VAG 的高效/扩展变体（专有格式，常见于某些平台；保留英文原名：HEVAG）

    XMA,
    // 枚举成员 XMA：表示 Xbox 平台使用的专有音频压缩格式（保留英文原名：XMA）

    AAC,
    // 枚举成员 AAC：表示高级音频编码（Advanced Audio Coding），常见于流媒体与移动平台（保留英文原名：AAC）

    GCADPCM,
    // 枚举成员 GCADPCM：表示 GameCube/Wii 使用的 ADPCM 变体（Nintendo 专用 ADPCM，常对应 .wav 输出时需特殊处理；保留英文原名：GCADPCM）

    ATRAC9
    // 枚举成员 ATRAC9：表示索尼平台使用的 ATRAC9 压缩格式（专有格式，通常需要专门解码器；保留英文原名：ATRAC9）

}
// 枚举体结束（CompressionFormat）
