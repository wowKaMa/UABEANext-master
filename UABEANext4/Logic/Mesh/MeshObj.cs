using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace UABEANext4.Logic.Mesh;

public class MeshObj
{
    public uint[] Indices = []; // Changed to uint to support large meshes
    public List<Channel> Channels = [];
    public float[] Vertices = [];
    public float[] Normals = [];
    public float[] Tangents = [];
    public float[] Colors = [];
    public float[][] UVs = new float[8][];

    public Vector3 MinBounds = new(float.MaxValue);
    public Vector3 MaxBounds = new(float.MinValue);

    public string DebugStatus { get; private set; } = "Not initialized";

    public MeshObj() { }

    public MeshObj(AssetsFileInstance fileInst, AssetTypeValueField baseField, UnityVersion version)
    {
        Read(fileInst, baseField, version);
    }

    private void Read(AssetsFileInstance fileInst, AssetTypeValueField baseField, UnityVersion version)
    {
        try 
        {
            ReadIndicesData(baseField);
            ReadChannels(baseField);
            ReadVertexData(fileInst, baseField, version);
            
            if (Vertices.Length == 0)
                DebugStatus = "Extraction failed: No vertices found";
            else
                DebugStatus = $"Success: {Vertices.Length/3} verts, {Indices.Length/3} tris";
        }
        catch (Exception ex)
        {
            DebugStatus = $"Error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"[VRCA Preview] CRITICAL: Mesh parsing failed: {ex}");
        }
    }

    private void ReadIndicesData(AssetTypeValueField baseField)
    {
        var bufferField = baseField["m_IndexBuffer.Array"];
        if (bufferField.IsDummy) return;

        var indicesData = bufferField.AsByteArray;
        if (indicesData.Length == 0) return;

        var format = baseField["m_IndexFormat"].AsInt; // 0 = 16-bit, 1 = 32-bit
        if (format == 0)
        {
            Indices = new uint[indicesData.Length / 2];
            for (var i = 0; i < indicesData.Length; i += 2)
                Indices[i / 2] = (ushort)(indicesData[i + 1] << 8 | indicesData[i]);
        }
        else
        {
            Indices = new uint[indicesData.Length / 4];
            for (var i = 0; i < indicesData.Length; i += 4)
                Indices[i / 4] = BitConverter.ToUInt32(indicesData, i);
        }
    }

    private void ReadChannels(AssetTypeValueField baseField)
    {
        var channelFields = baseField["m_VertexData"]["m_Channels.Array"];
        if (channelFields.IsDummy) return;

        Channels = new List<Channel>();
        foreach (var channelField in channelFields)
        {
            Channels.Add(new Channel(channelField));
        }
    }

    private List<int> GetStreamLengths(UnityVersion version)
    {
        if (Channels.Count == 0) return [];

        var streamCount = Channels.Max(c => c.stream) + 1;
        var streamLengths = new List<int>();
        for (var i = 0; i < streamCount; i++)
        {
            var maxEndOffset = 0;
            foreach (var channel in Channels.Where(c => c.stream == i))
            {
                var format = ToVertexFormatV2(channel.format, version);
                var size = GetFormatSize(format);
                var endOffset = channel.offset + (channel.dimension & 0xf) * size;
                maxEndOffset = Math.Max(maxEndOffset, endOffset);
            }
            streamLengths.Add((maxEndOffset + 3) & ~3); // 4-byte align
        }
        return streamLengths;
    }

    private static int GetFormatSize(VertexFormatV2 format) => format switch
    {
        VertexFormatV2.Float => 4, VertexFormatV2.Float16 => 2,
        VertexFormatV2.UNorm8 or VertexFormatV2.SNorm8 or VertexFormatV2.UInt8 or VertexFormatV2.SInt8 => 1,
        VertexFormatV2.UNorm16 or VertexFormatV2.SNorm16 or VertexFormatV2.UInt16 or VertexFormatV2.SInt16 => 2,
        VertexFormatV2.UInt32 or VertexFormatV2.SInt32 => 4,
        _ => 4
    };

    private static VertexFormatV2 ToVertexFormatV2(int format, UnityVersion version)
    {
        if (version.major >= 2019) return (VertexFormatV2)format;
        if (version.major >= 2017) return (VertexFormatV1)format switch
        {
            VertexFormatV1.Float => VertexFormatV2.Float,
            VertexFormatV1.Float16 => VertexFormatV2.Float16,
            VertexFormatV1.Color or VertexFormatV1.UNorm8 => VertexFormatV2.UNorm8,
            _ => (VertexFormatV2)format
        };
        return (VertexChannelFormat)format switch
        {
            VertexChannelFormat.Float => VertexFormatV2.Float,
            VertexChannelFormat.Float16 => VertexFormatV2.Float16,
            _ => VertexFormatV2.Float
        };
    }

    private static byte[] GetVertexData(AssetsFileInstance fileInst, AssetTypeValueField baseField)
    {
        var streamData = baseField["m_StreamData"];
        if (!streamData.IsDummy && streamData["size"].AsUInt > 0)
        {
            var offset = streamData["offset"].AsUInt;
            var size = streamData["size"].AsUInt;
            var path = streamData["path"].AsString;
            
            if (fileInst.parentBundle != null)
            {
                var archiveName = Path.GetFileName(path.TrimStart('/', '\\'));
                if (path.StartsWith("archive:/")) archiveName = path.Substring(9);
                
                var bundle = fileInst.parentBundle.file;
                var info = bundle.BlockAndDirInfo.DirectoryInfos.FirstOrDefault(i => 
                    i.Name.Equals(archiveName, StringComparison.OrdinalIgnoreCase) || 
                    i.Name.EndsWith(archiveName, StringComparison.OrdinalIgnoreCase));

                if (info != null)
                {
                    lock (bundle.DataReader)
                    {
                        bundle.DataReader.Position = info.Offset + offset;
                        return bundle.DataReader.ReadBytes((int)size);
                    }
                }
            }

            var rootPath = Path.GetDirectoryName(fileInst.path);
            var fixedPath = path;
            if (path.StartsWith("archive:/")) fixedPath = Path.GetFileName(fixedPath);
            if (!Path.IsPathRooted(fixedPath) && rootPath != null) fixedPath = Path.Combine(rootPath, fixedPath);
            
            if (File.Exists(fixedPath))
            {
                using var fs = File.OpenRead(fixedPath);
                fs.Position = offset;
                var data = new byte[size];
                fs.ReadExactly(data);
                return data;
            }
        }
        
        var meshDataField = baseField["m_VertexData"]["m_DataSize"];
        return !meshDataField.IsDummy ? meshDataField.AsByteArray : [];
    }

    private void ReadVertexData(AssetsFileInstance fileInst, AssetTypeValueField baseField, UnityVersion version)
    {
        var vertexCount = (int)baseField["m_VertexData"]["m_VertexCount"].AsUInt;
        if (vertexCount == 0) return;

        var vertexData = GetVertexData(fileInst, baseField);
        if (vertexData.Length == 0) return;

        var streamLengths = GetStreamLengths(version);
        var startPos = 0;

        for (var strIdx = 0; strIdx < streamLengths.Count; strIdx++)
        {
            var streamLength = streamLengths[strIdx];
            if (streamLength == 0) continue;

            for (var chnIdx = 0; chnIdx < Channels.Count; chnIdx++)
            {
                var channel = Channels[chnIdx];
                if (channel.stream != strIdx) continue;

                var dimension = channel.dimension & 0xf;
                var format = ToVertexFormatV2(channel.format, version);
                var compSize = GetFormatSize(format);
                var elementSize = compSize * dimension;
                var offset = channel.offset + startPos;

                if (offset + vertexCount * streamLength > vertexData.Length) continue;

                var data = new byte[elementSize * vertexCount];
                for (var i = 0; i < vertexCount; i++)
                    Buffer.BlockCopy(vertexData, offset + i * streamLength, data, i * elementSize, elementSize);

                bool isInt = format is VertexFormatV2.UInt8 or VertexFormatV2.SInt8 or VertexFormatV2.UInt16 or VertexFormatV2.SInt16 or VertexFormatV2.UInt32 or VertexFormatV2.SInt32;
                if (isInt) SetCorrectArray(ConvertIntArray(data, format), null!, chnIdx, version);
                else SetCorrectArray(null!, ConvertFloatArray(data, format), chnIdx, version);
            }
            startPos += streamLength * vertexCount;
        }
        CalculateBounds();
    }

    private void CalculateBounds()
    {
        if (Vertices.Length == 0) return;
        var min = new Vector3(float.MaxValue);
        var max = new Vector3(float.MinValue);
        for (int i = 0; i < Vertices.Length; i += 3)
        {
            var v = new Vector3(Vertices[i], Vertices[i + 1], Vertices[i + 2]);
            min = Vector3.Min(min, v);
            max = Vector3.Max(max, v);
        }
        MinBounds = min; MaxBounds = max;
    }

    private void SetCorrectArray(int[]? intItems, float[]? floatItems, int chnIdx, UnityVersion version)
    {
        if (version.major >= 2018)
        {
            var type = (ChannelTypeV3)chnIdx;
            switch (type)
            {
                case ChannelTypeV3.Vertex: Vertices = floatItems!; break;
                case ChannelTypeV3.Normal: Normals = floatItems!; break;
                case ChannelTypeV3.Tangent: Tangents = floatItems!; break;
                case ChannelTypeV3.Color: Colors = floatItems!; break;
                case >= ChannelTypeV3.TexCoord0 and <= ChannelTypeV3.TexCoord7: UVs[(int)type - (int)ChannelTypeV3.TexCoord0] = floatItems!; break;
            }
        }
        else
        {
            var type = (ChannelTypeV2)chnIdx;
            switch (type)
            {
                case ChannelTypeV2.Vertex: Vertices = floatItems!; break;
                case ChannelTypeV2.Normal: Normals = floatItems!; break;
                case ChannelTypeV2.Color: Colors = floatItems!; break;
                case >= ChannelTypeV2.TexCoord0 and <= ChannelTypeV2.TexCoord3: UVs[(int)type - (int)ChannelTypeV2.TexCoord0] = floatItems!; break;
                case ChannelTypeV2.Tangent: Tangents = floatItems!; break;
            }
        }
    }

    private static int[] ConvertIntArray(byte[] data, VertexFormatV2 format)
    {
        var size = GetFormatSize(format);
        var count = data.Length / size;
        var items = new int[count];
        for (var i = 0; i < count; i++) items[i] = format switch {
            VertexFormatV2.UInt8 or VertexFormatV2.SInt8 => data[i],
            VertexFormatV2.UInt16 or VertexFormatV2.SInt16 => BitConverter.ToUInt16(data, i * 2),
            VertexFormatV2.UInt32 or VertexFormatV2.SInt32 => BitConverter.ToInt32(data, i * 4),
            _ => 0
        };
        return items;
    }

    private static float[] ConvertFloatArray(byte[] data, VertexFormatV2 format)
    {
        var size = GetFormatSize(format);
        var count = data.Length / size;
        var items = new float[count];
        for (var i = 0; i < count; i++) items[i] = format switch {
            VertexFormatV2.Float => BitConverter.ToSingle(data, i * 4),
            VertexFormatV2.Float16 => (float)BitConverter.UInt16BitsToHalf(BitConverter.ToUInt16(data, i * 2)),
            VertexFormatV2.UNorm8 => data[i] / 255.0f,
            VertexFormatV2.SNorm8 => Math.Max((sbyte)data[i] / 127.0f, -1.0f),
            VertexFormatV2.UNorm16 => BitConverter.ToUInt16(data, i * 2) / 65535.0f,
            VertexFormatV2.SNorm16 => Math.Max(BitConverter.ToInt16(data, i * 2) / 32767.0f, -1.0f),
            _ => 0.0f
        };
        return items;
    }
}
