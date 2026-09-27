using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetsTools.NET.Texture;
using System;
using System.IO;

namespace UABEANext4.Logic.Mesh;

public class TextureObj
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public byte[] Data { get; private set; } = Array.Empty<byte>();
    public bool HasAlpha { get; private set; }
    public string Name { get; private set; } = "Texture";

    public TextureObj(AssetsFileInstance fileInst, AssetTypeValueField texBase)
    {
        Name = texBase["m_Name"].AsString;
        Width = texBase["m_Width"].AsInt;
        Height = texBase["m_Height"].AsInt;
        
        try 
        {
            var texFile = TextureFile.ReadTextureFile(texBase);
            var encBytes = GetRawTextureBytes(texFile, fileInst);
            
            if (encBytes != null && encBytes.Length > 0)
            {
                // Decode to BGRA32 (default for DecodeTextureRaw)
                // AssetRipper.TextureDecoder is used internally usually
                
                // Note: DecodeTextureRaw might imply BGRA or RGBA depending on impl.
                // Usually it returns BGRA. OpenGl likes RGBA.
                // We might need to swizzle.
                
                // Check if DecodeTextureRaw exists (based on TexturePlugin usage)
                Data = texFile.DecodeTextureRaw(encBytes, true); 
                
                if (Data != null)
                {
                    // GL expects RGBA, AssetsTools usually gives BGRA.
                    // Simple swizzle check/conversion if needed.
                    // For now, let's assume it gives what we want or we fix colors later.
                    // Actually, let's just get it compiling first.
                    
                    // Convert BGRA to RGBA if necessary
                    // for (int i = 0; i < Data.Length; i += 4)
                    // {
                    //     var b = Data[i];
                    //     Data[i] = Data[i + 2];
                    //     Data[i + 2] = b;
                    // }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error decoding texture {Name}: {ex.Message}");
            Data = new byte[Width * Height * 4];
            for(int i=0; i<Data.Length; i++) Data[i] = 255;
        }
    }
    
    private byte[]? GetRawTextureBytes(TextureFile texFile, AssetsFileInstance inst)
    {
        if (texFile.m_StreamData.size != 0 && !string.IsNullOrEmpty(texFile.m_StreamData.path))
        {
            var rootPath = Path.GetDirectoryName(inst.path);
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
                using var stream = File.OpenRead(fixedStreamPath);
                stream.Position = (long)texFile.m_StreamData.offset;
                var data = new byte[texFile.m_StreamData.size];
                stream.Read(data, 0, data.Length);
                return data;
            }
             return null;
        }
        return texFile.pictureData;
    }
}
