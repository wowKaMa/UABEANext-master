using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using AssetsTools.NET;
using AssetsTools.NET.Texture;

namespace UABEANext4.Logic.TextureProcessors
{
    public static class GpuTextureProcessor
    {
        private static string _texConvPath = FindTexConv();

        private static string FindTexConv()
        {
            // 1. Check direct path in base directory (Deployment)
            string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", "texconv.exe");
            if (File.Exists(localPath)) return localPath;

            // 2. Recursive search upwards from BaseDirectory (Development)
            // This handles cases where we are running in bin/Debug/net8.0 and Tools is in project root.
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null)
            {
                string target = Path.Combine(dir.FullName, "Tools", "texconv.exe");
                if (File.Exists(target)) return target;
                dir = dir.Parent;
            }
            
            // 3. Fallback: check hardcoded specific path if user specified one (e.g. e:\UABEANext-master\...)
            // Just widely search upwards from current dir too as fallback
            try 
            {
                dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                while (dir != null)
                {
                    string target = Path.Combine(dir.FullName, "Tools", "texconv.exe");
                    if (File.Exists(target)) return target;
                    dir = dir.Parent;
                }
            }
            catch {}

            return "";
        }

        public static bool IsAvailable()
        {
            if (string.IsNullOrEmpty(_texConvPath))
            {
                _texConvPath = FindTexConv();
            }
            return !string.IsNullOrEmpty(_texConvPath) && File.Exists(_texConvPath);
        }

        public static async Task<byte[]> ProcessTextureAsync(string inputFile, int width, int height, TextureFormat targetFormat, bool generateMipMaps)
        {
            if (!IsAvailable())
                throw new FileNotFoundException("texconv.exe not found. Please ensure it is in the Tools folder.");

            string tempDir = Path.Combine(Path.GetTempPath(), "UABEA_TexConv_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            // If input is already a file path, we use it directly. 
            // If it's an image object, we would save it first.
            // Simplified for batch: we assume input is a file (png/tga)
            
            string outputFileName = Path.GetFileNameWithoutExtension(inputFile) + ".dds";
            string outputFile = Path.Combine(tempDir, outputFileName);
            
            try
            {
                string formatArg = GetTexConvFormat(targetFormat);
                
                // Construct arguments
                string args = $"-w {width} -h {height} -m {(generateMipMaps ? 0 : 1)} -f {formatArg} -if CUBIC -y -o \"{tempDir}\" \"{inputFile}\"";

                var startInfo = new ProcessStartInfo
                {
                    FileName = _texConvPath,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = tempDir
                };

                using (var process = Process.Start(startInfo))
                {
                    if (process == null) throw new Exception("Failed to start texconv.exe");

                    // Read output streams asynchronously
                    var stdoutTask = process.StandardOutput.ReadToEndAsync();
                    var stderrTask = process.StandardError.ReadToEndAsync();

                    using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2)))
                    {
                        try
                        {
                            await process.WaitForExitAsync(cts.Token);
                        }
                        catch (OperationCanceledException)
                        {
                            process.Kill();
                            throw new Exception("texconv timed out after 2 minutes.");
                        }
                    }

                    string stdout = await stdoutTask;
                    string stderr = await stderrTask;

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"texconv failed with exit code {process.ExitCode}.\nStdOut: {stdout}\nStdErr: {stderr}");
                    }
                }

                // Texconv might output as .DDS or .dds, search for it
                var files = Directory.GetFiles(tempDir, "*.dds");
                if (files.Length == 0)
                    throw new FileNotFoundException("texconv output file not found.");
                
                outputFile = files[0];

                byte[] ddsBytes = await File.ReadAllBytesAsync(outputFile);
                return StripDdsHeader(ddsBytes);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    try { Directory.Delete(tempDir, true); } catch { } 
                }
            }
        }

        // Overload for ImageSharp image (used by single file UI)
        public static async Task<byte[]> ProcessTextureAsync(Image<Bgra32> image, int width, int height, TextureFormat targetFormat, bool generateMipMaps)
        {
             string tempDir = Path.Combine(Path.GetTempPath(), "UABEA_TexConv_Img_" + Guid.NewGuid().ToString());
             Directory.CreateDirectory(tempDir);
             string inputFile = Path.Combine(tempDir, "temp_input.png");
             
             try
             {
                 await image.SaveAsPngAsync(inputFile);
                 return await ProcessTextureAsync(inputFile, width, height, targetFormat, generateMipMaps);
             }
             finally
             {
                 if (Directory.Exists(tempDir))
                 {
                     try { Directory.Delete(tempDir, true); } catch { }
                 }
             }
        }

        private static string GetTexConvFormat(TextureFormat format)
        {
            return format switch
            {
                TextureFormat.DXT1 => "BC1_UNORM",
                TextureFormat.DXT1Crunched => "BC1_UNORM", 
                TextureFormat.DXT5 => "BC3_UNORM",
                TextureFormat.DXT5Crunched => "BC3_UNORM",
                TextureFormat.BC7 => "BC7_UNORM",
                TextureFormat.R8 => "R8_UNORM",
                TextureFormat.RGB24 => "RGB24", // Supported by texconv, but might need special handling if not packed
                TextureFormat.RGBA32 => "R8G8B8A8_UNORM",
                _ => "BC3_UNORM"
            };
        }

        private static byte[] StripDdsHeader(byte[] ddsBytes)
        {
            if (ddsBytes.Length < 128) return ddsBytes;

            int headerSize = 128;
            uint fourCC = BitConverter.ToUInt32(ddsBytes, 84);
            // DX10 header check
            if (fourCC == 0x30315844) headerSize += 20;

            if (ddsBytes.Length <= headerSize) return Array.Empty<byte>();

            byte[] rawData = new byte[ddsBytes.Length - headerSize];
            Array.Copy(ddsBytes, headerSize, rawData, 0, rawData.Length);
            return rawData;
        }
    }
}
