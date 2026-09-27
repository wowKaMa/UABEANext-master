using AssetsTools.NET;
// 引用 AssetsTools.NET 库（AssetsTools.NET），用于访问 Unity 资产解析相关类型（例如 AssetTypeValueField）

namespace UABEANext4.Logic.Mesh
// 定义命名空间 UABEANext4.Logic.Mesh（UABEANext4.Logic.Mesh），用于组织与网格（mesh）相关的逻辑类型
{
    public class Channel
    // 定义公共类 Channel（Channel），表示网格顶点通道的描述信息（如流索引、偏移、格式、维度）
    {
        public byte stream;
        // 公共字段 stream（stream）：表示该通道所属的数据流索引（一个字节），用于区分不同 vertex stream

        public byte offset;
        // 公共字段 offset（offset）：表示该通道在流内的字节偏移（一个字节），用于定位通道数据起始位置

        public byte format;
        // 公共字段 format（format）：表示该通道的数据格式编码（一个字节），需结合 Unity 版本解析为具体格式

        public byte dimension;
        // 公共字段 dimension（dimension）：表示该通道的分量数量或维度（一个字节），低 4 位通常表示分量数

        public Channel(AssetTypeValueField field)
        // 构造函数 Channel(AssetTypeValueField field)（Channel）：从 AssetTypeValueField（field）中读取并初始化通道字段
        {
            stream = field["stream"].AsByte;
            // 从 field 的 "stream" 子字段读取字节值并赋给 stream（使用 AssetTypeValueField 的 AsByte 访问器）

            offset = field["offset"].AsByte;
            // 从 field 的 "offset" 子字段读取字节值并赋给 offset（表示通道在流中的偏移）

            format = field["format"].AsByte;
            // 从 field 的 "format" 子字段读取字节值并赋给 format（表示通道的编码格式）

            dimension = field["dimension"].AsByte;
            // 从 field 的 "dimension" 子字段读取字节值并赋给 dimension（表示通道的分量数或维度）
        }
    }
}
