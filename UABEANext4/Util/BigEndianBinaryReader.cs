using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace UABEANext4.Util
{
    /// <summary>
    /// 支持大端序 (Big-Endian) 读取的 BinaryReader 扩展工具类。
    /// UnityFS Header 所有的数值字段都是大端序。
    /// </summary>
    public class BigEndianBinaryReader : BinaryReader
    {
        public BigEndianBinaryReader(Stream input) : base(input) { }
        public BigEndianBinaryReader(Stream input, Encoding encoding) : base(input, encoding) { }
        public BigEndianBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : base(input, encoding, leaveOpen) { }

        public override short ReadInt16() => BinaryPrimitives.ReadInt16BigEndian(ReadBytes(2));
        public override ushort ReadUInt16() => BinaryPrimitives.ReadUInt16BigEndian(ReadBytes(2));
        public override int ReadInt32() => BinaryPrimitives.ReadInt32BigEndian(ReadBytes(4));
        public override uint ReadUInt32() => BinaryPrimitives.ReadUInt32BigEndian(ReadBytes(4));
        public override long ReadInt64() => BinaryPrimitives.ReadInt64BigEndian(ReadBytes(8));
        public override ulong ReadUInt64() => BinaryPrimitives.ReadUInt64BigEndian(ReadBytes(8));
        public override float ReadSingle() => BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32BigEndian(ReadBytes(4)));
        public override double ReadDouble() => BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64BigEndian(ReadBytes(8)));

        /// <summary>
        /// 读取以空字符结尾的字符串 (\0)。
        /// </summary>
        public string ReadNullTerminatedString()
        {
            var sb = new StringBuilder();
            byte b;
            int count = 0;
            while ((b = ReadByte()) != 0 && count < 4096)
            {
                sb.Append((char)b);
                count++;
            }
            return sb.ToString();
        }

        public uint ReadUInt32BigEndian() => ReadUInt32();
        public ulong ReadUInt64BigEndian() => ReadUInt64();
    }
}
