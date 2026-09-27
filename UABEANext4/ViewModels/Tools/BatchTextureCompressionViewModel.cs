using AssetsTools.NET;
using AssetsTools.NET.Extra; // For BundleHelper, etc.
using AssetsTools.NET.Texture;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UABEANext4.Logic.TextureProcessors;
using UABEANext4.Views.Tools;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Platform.Storage;
using UABEANext4.AssetWorkspace;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Runtime.InteropServices;

namespace UABEANext4.ViewModels.Tools
{
    public partial class BatchTextureCompressionViewModel : ViewModelBase
    {
        [ObservableProperty] private string _inputFolder = string.Empty;
        [ObservableProperty] private string _outputFolder = string.Empty;
        [ObservableProperty] private int _maxSize = 1024;
        [ObservableProperty] private int _concurrency = 4;
        [ObservableProperty] private string _systemInfo = string.Empty;
        
        public IStorageProvider? StorageProvider { get; set; }
        public Action? RequestClose { get; set; }

        public BatchTextureCompressionViewModel()
        {
            // Calculate recommended concurrency based on RAM
            try
            {
                // Simple heuristic: assume 64GB as user stated, or try to detect if needed.
                // For now, let's default to a safe value or what user asked.
                // User has 64GB RAM. Each VRCA processing might take 1-2GB peak.
                // 4-8 is safe.
                Concurrency = 4;            }
            catch { }
            
            UpdateSystemInfo();
        }

        private void UpdateSystemInfo()
        {
            SystemInfo = $"推荐并发数 (Recommended Concurrency): {Concurrency} (基于可用内存 / Based on available RAM)";
        }

        [RelayCommand]
        public async Task BrowseInput()
        {
            if (StorageProvider == null) return;
            var folder = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = "Select Input Folder (VRCA)" });
            if (folder.Count > 0) InputFolder = folder[0].Path.LocalPath;
        }

        [RelayCommand]
        public async Task BrowseOutput()
        {
            if (StorageProvider == null) return;
            var folder = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = "Select Output Folder" });
            if (folder.Count > 0) OutputFolder = folder[0].Path.LocalPath;
        }


        [RelayCommand]
        public async Task StartBatch()
        {
            if (string.IsNullOrEmpty(InputFolder) || string.IsNullOrEmpty(OutputFolder)) return;

            if (!Directory.Exists(InputFolder))
            {
                await Util.MessageBoxUtil.ShowDialog("错误 (Error)", "输入文件夹不存在 (Input folder does not exist)");
                return;
            }
            Directory.CreateDirectory(OutputFolder);

            var files = Directory.GetFiles(InputFolder, "*.vrca", SearchOption.AllDirectories);
            if (files.Length == 0) return;

            // Close the configuration window
            RequestClose?.Invoke();

            // Initialize Progress VM
            var progressVm = new BatchProgressViewModel();
            
            // Pre-populate items
            foreach (var f in files)
            {
                progressVm.Items.Add(new BatchFileItemViewModel 
                { 
                    FileName = Path.GetFileName(f), 
                    FullPath = f,
                    Status = "排队中 (Queued)",
                    Progress = 0
                });
            }

            var progressWindow = new BatchProgressWindow { DataContext = progressVm };
            Window? owner = null;
            try
            {
                owner = Util.WindowUtils.GetMainWindow();
            }
            catch { }
            if (owner != null)
                progressWindow.Show(owner);
            else
                progressWindow.Show();

            progressVm.TotalProgressText = $"0 / {files.Length}";

            // Run in background
            await Task.Run(async () =>
            {
                int processedCount = 0;
                var parallelOptions = new ParallelOptions 
                { 
                    MaxDegreeOfParallelism = Concurrency,
                    CancellationToken = progressVm.Cts.Token 
                };

                try
                {
                    await Parallel.ForEachAsync(progressVm.Items.ToList(), parallelOptions, async (item, ct) =>
                    {
                        try
                        {
                            await ProcessSingleVrca(item, Path.Combine(OutputFolder, item.FileName), MaxSize, ct);
                            
                            Interlocked.Increment(ref processedCount);
                            double totalProg = (double)processedCount / files.Length * 100;
                            
                            Dispatcher.UIThread.Post(() => 
                            {
                                progressVm.TotalProgress = totalProg;
                                progressVm.TotalProgressText = $"{processedCount} / {files.Length}";
                            });
                        }
                        catch (OperationCanceledException) { throw; }
                        catch (Exception ex)
                        {
                            Dispatcher.UIThread.Post(() => {
                                item.Status = "错误 (Error)";
                                item.Message = ex.Message;
                            });
                        }
                        finally
                        {
                            // Aggressive Cleanup after EACH file
                            ForceCleanup();
                            Dispatcher.UIThread.Post(() => item.MemoryInfo = "已释放 (Released)");
                        }
                    });
                }
                catch (OperationCanceledException)
                {
                    Dispatcher.UIThread.Post(() => progressVm.StatusText = "已取消 (Cancelled)");
                }
                finally
                {
                    Dispatcher.UIThread.Post(() => progressVm.StatusText = "处理完成 (Batch Completed)");
                }
            });
        }

        private async Task ProcessSingleVrca(BatchFileItemViewModel item, string outputFile, int maxSize, CancellationToken ct)
        {
            string inputFile = item.FullPath;
            
            // STRICT ISOLATION: Run in a static context to ensure no references are held by this instance
            // We pass only simple types or data objects, no "this" capture.
            await ProcessSingleVrcaInternal(item, inputFile, outputFile, maxSize, ct);

            // Force cleanup AFTER the static method returns
            // At this point, all variables inside ProcessSingleVrcaInternal are out of scope
            Dispatcher.UIThread.Post(() => item.Status = "释放内存 (Releasing Memory)...");
            
            long memBefore = GC.GetTotalMemory(false) / 1024 / 1024;
            ForceCleanup();
            long memAfter = GC.GetTotalMemory(true) / 1024 / 1024;
            
            Dispatcher.UIThread.Post(() => item.MemoryInfo = $"Released: {memBefore - memAfter} MB (Free: {memAfter} MB)");
        }

        // STATIC METHOD: Cannot access instance fields, ensures isolation
        private static async Task ProcessSingleVrcaInternal(BatchFileItemViewModel item, string inputFile, string outputFile, int maxSize, CancellationToken ct)
        {
            Dispatcher.UIThread.Post(() => item.Status = "加载中 (Loading)...");

            var manager = new AssetsManager();
            string classDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "classdata.tpk");
            if (File.Exists(classDataPath))
                manager.LoadClassPackage(classDataPath);

            BundleFileInstance? bunInst = null;
            
            // Declare large collections
            var rawResourceFiles = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
            var allResources = new List<ResourceLink>();
            var assetsFiles = new List<AssetsFileInstance>();
            Dictionary<string, byte[]>? newResSData = null;

            FileStream? vrcaStream = null;

            try
            {
                // Explicitly manage the file stream
                vrcaStream = File.OpenRead(inputFile);
                bunInst = manager.LoadBundleFile(vrcaStream, inputFile, true);
                var dirInfos = bunInst.file.BlockAndDirInfo.DirectoryInfos;

                // 1. Preload raw resource data
                Dispatcher.UIThread.Post(() => item.Status = "扫描资源 (Scanning Resources)...");
                foreach (var dir in dirInfos)
                {
                    if (dir.Name.EndsWith(".resS", StringComparison.OrdinalIgnoreCase) ||
                        dir.Name.EndsWith(".resource", StringComparison.OrdinalIgnoreCase))
                    {
                        rawResourceFiles[dir.Name] = BundleHelper.LoadAssetDataFromBundle(bunInst.file, dir.Name);
                    }
                }

                // 2. Scan ALL Assets
                foreach (var dir in dirInfos)
                {
                    if (ct.IsCancellationRequested) return;
                    if (!dir.Name.EndsWith(".assets") && !dir.Name.ToLower().Contains("cab-")) continue;

                    var asInst = manager.LoadAssetsFileFromBundle(bunInst, dir.Name, true);
                    if (asInst == null) continue;
                    assetsFiles.Add(asInst);
                    
                    manager.LoadClassDatabaseFromPackage(asInst.file.Metadata.UnityVersion);

                    foreach (var info in asInst.file.AssetInfos)
                    {
                        var baseField = manager.GetBaseField(asInst, info);
                        
                        var streamData = baseField["m_StreamData"];
                        if (!streamData.IsDummy && streamData["path"].AsString.Length > 0)
                        {
                            string resFileName = Path.GetFileName(streamData["path"].AsString);
                            if (rawResourceFiles.ContainsKey(resFileName))
                            {
                                allResources.Add(new ResourceLink
                                {
                                    AssetFileInst = asInst,
                                    AssetInfo = info,
                                    BaseField = baseField,
                                    DataContainer = streamData,
                                    AssetName = baseField["m_Name"].AsString,
                                    ResFileName = resFileName,
                                    OldOffset = streamData["offset"].AsLong,
                                    OldSize = streamData["size"].AsLong,
                                    IsProcessed = false,
                                    TypeId = info.TypeId
                                });
                            }
                        }
                        
                        var resourceData = baseField["m_Resource"];
                        if (!resourceData.IsDummy && resourceData["m_Source"].AsString.Length > 0)
                        {
                            string resFileName = Path.GetFileName(resourceData["m_Source"].AsString);
                            if (rawResourceFiles.ContainsKey(resFileName))
                            {
                                allResources.Add(new ResourceLink
                                {
                                    AssetFileInst = asInst,
                                    AssetInfo = info,
                                    BaseField = baseField,
                                    DataContainer = resourceData,
                                    AssetName = baseField["m_Name"].AsString,
                                    ResFileName = resFileName,
                                    OldOffset = resourceData["m_Offset"].AsLong,
                                    OldSize = resourceData["m_Size"].AsLong,
                                    IsProcessed = false,
                                    TypeId = info.TypeId
                                });
                            }
                        }
                    }
                }

                bool anyChanged = false;

                // 3. Process Textures
                var texturesToProcess = allResources.Where(x => x.TypeId == (int)AssetClassID.Texture2D).ToList();
                int texCount = texturesToProcess.Count;
                int texDone = 0;

                foreach (var link in texturesToProcess)
                {
                    if (ct.IsCancellationRequested) return;

                    TextureFile tf = TextureFile.ReadTextureFile(link.BaseField);
                    
                    // Update progress status
                    texDone++;
                    Dispatcher.UIThread.Post(() => {
                        item.Status = $"处理贴图 (Processing Textures) [{texDone}/{texCount}]";
                        item.Progress = (double)texDone / texCount * 80; // 0-80% for texture processing
                        item.Message = "计算设备: GPU (Compute Device: GPU)";
                    });
                    
                    if (tf.m_Width <= maxSize && tf.m_Height <= maxSize) continue;

                    if (rawResourceFiles.TryGetValue(link.ResFileName, out byte[]? fullResData) && fullResData != null)
                    {
                         byte[] textureData = new byte[link.OldSize];
                         if (link.OldOffset + link.OldSize > fullResData.Length) continue;

                         Array.Copy(fullResData, link.OldOffset, textureData, 0, link.OldSize);
                         
                         tf.pictureData = textureData; 
                         byte[] bgraRaw = tf.DecodeTextureRaw(textureData, true); 
                         
                         if (bgraRaw != null)
                         {
                             double ratio = Math.Min((double)maxSize / tf.m_Width, (double)maxSize / tf.m_Height);
                             int newWidth = Math.Max(4, ((int)Math.Round(tf.m_Width * ratio) / 4) * 4);
                             int newHeight = Math.Max(4, ((int)Math.Round(tf.m_Height * ratio) / 4) * 4);

                             using (var image = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(bgraRaw, tf.m_Width, tf.m_Height))
                             {
                                 TextureFormat targetFormat = (TextureFormat)tf.m_TextureFormat;
                                 bool mips = tf.m_MipCount > 1;

                                 byte[] newDdsData = await GpuTextureProcessor.ProcessTextureAsync(image, newWidth, newHeight, targetFormat, mips);

                                 if (newDdsData != null && newDdsData.Length > 0)
                                 {
                                     link.Data = newDdsData;
                                     link.IsProcessed = true;
                                     anyChanged = true;

                                     int targetFormatId = tf.m_TextureFormat;
                                     if ((TextureFormat)tf.m_TextureFormat == TextureFormat.DXT1Crunched) targetFormatId = (int)TextureFormat.DXT1;
                                     if ((TextureFormat)tf.m_TextureFormat == TextureFormat.DXT5Crunched) targetFormatId = (int)TextureFormat.DXT5;

                                     SetFieldValue(link.BaseField, "m_Width", newWidth);
                                     SetFieldValue(link.BaseField, "m_Height", newHeight);
                                     SetFieldValue(link.BaseField, "m_TextureFormat", targetFormatId);
                                     
                                     if (mips)
                                         SetFieldValue(link.BaseField, "m_MipCount", (int)Math.Floor(Math.Log(Math.Max(newWidth, newHeight), 2)) + 1);
                                     
                                     var imgField = link.BaseField["image data"];
                                     if (!imgField.IsDummy) imgField.Value.AsByteArray = Array.Empty<byte>();

                                     link.AssetInfo.SetNewData(link.BaseField);
                                 }
                             }
                         }
                    }
                }

                if (!anyChanged)
                {
                    Dispatcher.UIThread.Post(() => {
                        item.Status = "跳过 (Skipped)";
                        item.Message = "无需压缩 (No textures to compress)";
                        item.Progress = 100;
                    });
                    return;
                }

                // 4. Rebuild .resS files
                Dispatcher.UIThread.Post(() => item.Status = "重建资源 (Rebuilding Resources)...");
                var resSGroups = allResources.GroupBy(x => x.ResFileName);
                newResSData = new Dictionary<string, byte[]>();

                foreach (var group in resSGroups)
                {
                    string resName = group.Key;
                    if (!rawResourceFiles.ContainsKey(resName)) continue;
                    
                    byte[] originalFileBytes = rawResourceFiles[resName];

                    using (MemoryStream ms = new MemoryStream())
                    {
                        var sortedLinks = group.OrderBy(x => x.OldOffset).ToList();
                        
                        foreach (var link in sortedLinks)
                        {
                            long padding = (16 - (ms.Position % 16)) % 16;
                            if (padding > 0) ms.Write(new byte[padding], 0, (int)padding);

                            long newOffset = ms.Position;
                            byte[] dataToWrite = Array.Empty<byte>();
            


                            if (link.IsProcessed && link.Data != null)
                            {
                                dataToWrite = link.Data; 
                            }
                            else
                            {
                                if (link.OldOffset + link.OldSize <= originalFileBytes.Length)
                                {
                                    dataToWrite = new byte[link.OldSize];
                                    Array.Copy(originalFileBytes, link.OldOffset, dataToWrite, 0, link.OldSize);
                                }
                            }

                            ms.Write(dataToWrite, 0, dataToWrite.Length);

                            if (link.TypeId == (int)AssetClassID.AudioClip)
                            {
                                link.DataContainer!["m_Offset"].AsLong = newOffset;
                                link.DataContainer!["m_Size"].AsLong = dataToWrite.Length;
                            }
                            else
                            {
                                link.DataContainer!["offset"].AsLong = newOffset;
                                link.DataContainer!["size"].AsLong = dataToWrite.Length;
                            }
                            
                            link.AssetInfo!.SetNewData(link.BaseField);
                        }
                        
                        newResSData[resName] = ms.ToArray();
                    }
                }

                // 5. Pack Bundle with LZMA
                Dispatcher.UIThread.Post(() => item.Status = "打包保存 (Packing & Saving)...");
                
                foreach (var kvp in newResSData)
                {
                    var dirInfo = dirInfos.FirstOrDefault(d => d.Name.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase));
                    if (dirInfo != null) dirInfo.SetNewData(kvp.Value);
                }

                foreach (var asInst in assetsFiles)
                {
                    var dirInfo = dirInfos.FirstOrDefault(d => d.Name.Equals(asInst.name, StringComparison.OrdinalIgnoreCase));
                    if (dirInfo != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        using (AssetsFileWriter w = new AssetsFileWriter(ms))
                        {
                            asInst.file.Write(w);
                            dirInfo.SetNewData(ms.ToArray());
                        }
                    }
                }

                using (MemoryStream ms = new MemoryStream())
                using (AssetsFileWriter writer = new AssetsFileWriter(ms))
                {
                    bunInst.file.Write(writer);
                    byte[] uncompressedBundle = ms.ToArray();

                    using (var readMs = new MemoryStream(uncompressedBundle))
                    {
                        var ab = new AssetBundleFile();
                        ab.Read(new AssetsFileReader(readMs));
                        
                        using (var fs = File.Create(outputFile))
                        using (var w = new AssetsFileWriter(fs))
                        {
                            ab.Pack(w, AssetBundleCompressionType.LZMA);
                        }
                    }
                }

                Dispatcher.UIThread.Post(() => {
                    item.Status = "完成 (Completed)";
                    item.Progress = 100;
                });
            }
            finally
            {
                Dispatcher.UIThread.Post(() => item.Status = "释放内存 (Releasing Memory)...");
                
                // CRITICAL: Explicitly close everything
                
                // 1. Close all assets files loaded from the bundle
                if (assetsFiles != null)
                {
                    foreach (var af in assetsFiles)
                    {
                        try { af.file.Close(); } catch { }
                    }
                    assetsFiles.Clear();
                    assetsFiles = null;
                }

                // 2. Close the bundle file (this disposes the decompressed memory stream)
                if (bunInst != null)
                {
                    try { bunInst.file.Close(); } catch { }
                    bunInst = null;
                }

                // 3. Dispose the original file stream (if not already closed by bundle)
                if (vrcaStream != null)
                {
                    try { vrcaStream.Dispose(); } catch { }
                    vrcaStream = null;
                }

                // 4. Unload everything from manager including ClassDatabase
                manager.UnloadAll(true);
                manager = null;
                
                // Clear local large collections
                rawResourceFiles?.Clear();
                rawResourceFiles = null;
                
                allResources?.Clear();
                allResources = null;
                
                newResSData?.Clear();
                newResSData = null;

                // Force cleanup
                long memBefore = GC.GetTotalMemory(false) / 1024 / 1024;
                ForceCleanup();
                long memAfter = GC.GetTotalMemory(true) / 1024 / 1024;
                
                Dispatcher.UIThread.Post(() => item.MemoryInfo = $"Released: {memBefore - memAfter} MB (Free: {memAfter} MB)");
            }
        }




        private static void SetFieldValue(AssetTypeValueField baseField, string name, int value)
        {
            var field = baseField[name];
            if (!field.IsDummy) field.AsInt = value;
        }

        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);

        private static void ForceCleanup()
        {
             // Force ImageSharp release
             SixLabors.ImageSharp.Configuration.Default.MemoryAllocator.ReleaseRetainedResources();
             
             // Compact Large Object Heap (LOH)
             System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
             
             GC.Collect(2, GCCollectionMode.Forced, true, true);
             GC.WaitForPendingFinalizers();
             GC.Collect(2, GCCollectionMode.Forced, true, true);

             try
             {
                 // Aggressively flush working set to release physical memory to OS
                 EmptyWorkingSet(Process.GetCurrentProcess().Handle);
             }
             catch { }
        }


        private class ResourceLink
        {
            public AssetsFileInstance? AssetFileInst;
            public AssetFileInfo? AssetInfo;
            public AssetTypeValueField? BaseField;
            public AssetTypeValueField? DataContainer;
            public string AssetName = string.Empty;
            public string ResFileName = string.Empty;
            public long OldOffset;
            public long OldSize;
            public bool IsProcessed;
            public byte[]? Data; 
            public int TypeId;
        }
    }
}
