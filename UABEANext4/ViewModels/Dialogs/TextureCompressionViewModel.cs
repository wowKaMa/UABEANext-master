using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetsTools.NET.Texture;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BCnEncoder.Encoder;
using BCnEncoder.Shared;
using SixLabors.ImageSharp.PixelFormats;
using UABEANext4.AssetWorkspace;
using UABEANext4.ViewModels;
using UABEANext4.Util;
using Avalonia.Threading;
using Avalonia.Controls;
using UABEANext4.Logic.TextureProcessors;

namespace UABEANext4.ViewModels.Dialogs;

public partial class TextureCompressionEntry : ObservableObject
{
    public AssetInst Asset { get; }
    public string Name { get; }
    public string Format { get; }
    public long OldOffset { get; }
    public long OldSize { get; }
    
    [ObservableProperty]
    private long _newOffset;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplaySize))]
    private long _size;

    public string DisplaySize => $"{Size / 1024.0:F2} KB";

    public bool IsTexture { get; }
    public string ResSFile { get; }

    [ObservableProperty]
    private string _status = "Pending";

    [ObservableProperty]
    private string _errorMessage = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasIssue))]
    private bool _isOverlap;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasIssue))]
    private bool _isFailed;

    public bool HasIssue => IsOverlap || IsFailed;

    public int NewWidth { get; set; }
    public int NewHeight { get; set; }
    public int NewMipCount { get; set; }
    public int NewFormat { get; set; }
    public byte[]? ProcessedData { get; set; }

    public TextureCompressionEntry(AssetInst asset, string name, string format, long oldOffset, long size, bool isTexture, string resSFile)
    {
        Asset = asset;
        Name = name;
        Format = format;
        OldOffset = oldOffset;
        OldSize = size;
        NewOffset = oldOffset;
        Size = size;
        IsTexture = isTexture;
        ResSFile = resSFile;
    }
}

public partial class TextureCompressionViewModel : ViewModelBase
{
    private readonly Workspace? _workspace;
    
    [ObservableProperty]
    private ObservableCollection<TextureCompressionEntry> _entries = new();

    [ObservableProperty]
    private ObservableCollection<TextureCompressionEntry> _issueEntries = new();

    [ObservableProperty]
    private bool _isScanning;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private int _maxTextureSize = 1024;

    [ObservableProperty]
    private bool _isGpuAccelerationEnabled = true;

    private bool _gpuErrorShown;
    private readonly object _gpuErrorLock = new();

    private Dictionary<string, byte[]> _newResSData = new();
    private Dictionary<string, byte[]> _oldResSData = new();
    private static readonly ConcurrentDictionary<AssetsFileInstance, object> _fileLocks = new();

    public TextureCompressionViewModel(Workspace? workspace = null)
    {
        _workspace = workspace;
        // If workspace is null, we are likely in standalone/batch mode
    }

    [RelayCommand]
    public async Task ScanAssets()
    {
        if (_workspace == null)
        {
            StatusText = "No workspace loaded. Only Batch Mode is available.";
            return;
        }

        IsScanning = true;
        StatusText = "Scanning for resource links...";
        Entries.Clear();

        await Task.Run(() =>
        {
            var newEntries = new ConcurrentBag<TextureCompressionEntry>();
            
            var assetsFiles = WorkspaceItem.GetAssetsFileWorkspaceItems(_workspace.RootItems);

            Parallel.ForEach(assetsFiles, item =>
            {
                if (item.Object is not AssetsFileInstance asInst) return;

                foreach (var info in asInst.file.AssetInfos)
                {
                    // 仅扫描贴图（Texture2D 和 Cubemap）
                    if (info.TypeId != (int)AssetClassID.Texture2D && info.TypeId != (int)AssetClassID.Cubemap) continue;

                    AssetTypeValueField baseField;
                    try { baseField = _workspace.Manager.GetBaseField(asInst, info); } catch { continue; }
                    if (baseField == null) continue;

                    // 贴图使用 m_StreamData 存储外部资源引用
                    AssetTypeValueField targetData = baseField["m_StreamData"];
                    if (targetData.IsDummy) continue; // 忽略嵌入式贴图

                    var offset = targetData["offset"].AsLong;
                    var size = targetData["size"].AsLong;
                    var path = targetData["path"].AsString;

                    if (size > 0 && !string.IsNullOrEmpty(path))
                    {
                        var asset = _workspace.GetAssetInst(asInst, 0, info.PathId);
                        
                        string formatStr = "Unknown";
                        if (!baseField["m_TextureFormat"].IsDummy)
                            formatStr = ((TextureFormat)baseField["m_TextureFormat"].AsInt).ToString();
                        
                        newEntries.Add(new TextureCompressionEntry(
                            asset,
                            asset.AssetName ?? "Unknown",
                            formatStr,
                            offset,
                            size,
                            true, // 已确定是贴图
                            path
                        ));
                    }
                }
            });

            var sorted = newEntries.OrderBy(e => e.ResSFile).ThenBy(e => e.OldOffset).ToList();
            Dispatcher.UIThread.Post(() => {
                foreach (var entry in sorted) Entries.Add(entry);
                StatusText = $"Found {Entries.Count} resource references.";
            });
        });

        IsScanning = false;
    }

    [RelayCommand]
    public async Task StartCompression()
    {
        if (Entries.Count == 0)
        {
            StatusText = "No assets to compress. Please scan first.";
            return;
        }

        IsProcessing = true;
        StatusText = "Processing textures...";
        _gpuErrorShown = false;

        await Task.Run(() =>
        {
            StatusText = "Pre-loading resource data from bundle...";
            LoadAllResources();

            int total = Entries.Count;
            int current = 0;

            int maxThreads = Math.Min(Environment.ProcessorCount, total);
            if (maxThreads < 1) maxThreads = 1;

            var partitioner = Partitioner.Create(Entries, EnumerablePartitionerOptions.NoBuffering);

            Parallel.ForEach(partitioner, new ParallelOptions { MaxDegreeOfParallelism = maxThreads }, entry =>
            {
                try
                {
                    byte[] data;
                    if (entry.IsTexture)
                    {
                        data = ProcessTexture(entry);
                    }
                    else
                    {
                        data = ReadOriginalResource(entry);
                    }

                    if (data.Length == 0 && entry.Size > 0)
                        throw new Exception("Returned empty data for non-empty source");

                    entry.ProcessedData = data;
                    
                    int progress = Interlocked.Increment(ref current);
                    var mode = IsGpuAccelerationEnabled ? "GPU" : "CPU";
                    var msg = $"[{progress}/{total}] Processing ({mode}): {entry.Name} (Offset: {entry.OldOffset}, Size: {entry.OldSize / 1024.0:F2} KB)";
                    Dispatcher.UIThread.Post(() => StatusText = msg, DispatcherPriority.Background);
                }
                catch (Exception ex)
                {
                    entry.Status = "Error";
                    entry.ErrorMessage = ex.Message;
                    entry.IsFailed = true;

                    if (IsGpuAccelerationEnabled && !_gpuErrorShown)
                    {
                        lock (_gpuErrorLock)
                        {
                            if (!_gpuErrorShown)
                            {
                                _gpuErrorShown = true;
                                Dispatcher.UIThread.Post(async () => {
                                    await MessageBoxUtil.ShowDialog("GPU Processing Error", 
                                        $"An error occurred during GPU processing (shown only once):\n\nAsset: {entry.Name}\nError: {ex.Message}");
                                });
                            }
                        }
                    }
                    
                    try
                    {
                        entry.ProcessedData = ReadOriginalResource(entry);
                        entry.Status = "Failed (Original Kept)";
                    }
                    catch
                    {
                        entry.Status = "Critical Error";
                        entry.ProcessedData = Array.Empty<byte>();
                    }
                }
            });

            StatusText = "Finalizing offsets and building resource streams (Stage 2/2)...";
            _oldResSData.Clear(); // 提前释放原始大块内存
            _newResSData.Clear();

            var groups = Entries.GroupBy(e => e.ResSFile).ToList();
            foreach (var group in groups)
            {
                var resSName = group.Key;
                var sortedEntries = group.OrderBy(e => e.OldOffset).ToList();
                
                using var newResSStream = new MemoryStream();
                var updatesToApply = new List<(TextureCompressionEntry entry, long offset, int size, string status)>();
                
                foreach (var entry in sortedEntries)
                {
                    byte[] data = entry.ProcessedData ?? Array.Empty<byte>();
                    
                    long newOffset = (newResSStream.Position + 15) & ~15;
                    byte[] padding = new byte[newOffset - newResSStream.Position];
                    if (padding.Length > 0) newResSStream.Write(padding, 0, padding.Length);

                    newResSStream.Write(data, 0, data.Length);

                    string successStatus = entry.Status;
                    if (successStatus == "Pending") successStatus = "Success";

                    updatesToApply.Add((entry, newOffset, data.Length, successStatus));
                    entry.ProcessedData = null;

                    if (updatesToApply.Count >= 100)
                    {
                        var chunk = updatesToApply.ToList();
                        updatesToApply.Clear();
                        Dispatcher.UIThread.Post(() => ApplyEntryUpdates(chunk), DispatcherPriority.Background);
                    }
                }

                if (updatesToApply.Count > 0)
                {
                    var lastChunk = updatesToApply.ToList();
                    Dispatcher.UIThread.Post(() => ApplyEntryUpdates(lastChunk), DispatcherPriority.Background);
                }

                _newResSData[resSName] = newResSStream.ToArray();
            }

            ValidateResults();
        });

        IsProcessing = false;
        StatusText = "Process complete.";
    }

    private void ApplyEntryUpdates(List<(TextureCompressionEntry entry, long offset, int size, string status)> updates)
    {
        foreach (var update in updates)
        {
            update.entry.NewOffset = update.offset;
            update.entry.Size = update.size;
            update.entry.Status = update.status;
            update.entry.IsFailed = !update.status.Contains("Success");
            if (update.status == "Success" && update.entry.IsTexture && update.entry.NewWidth > 0)
            {
                update.entry.ErrorMessage = $"Compressed to {((TextureFormat)update.entry.NewFormat)} ({update.entry.NewWidth}x{update.entry.NewHeight})";
            }
        }
    }

    private void ValidateResults()
    {
        var issues = new List<TextureCompressionEntry>();
        foreach (var entry in Entries) entry.IsOverlap = false;

        var groups = Entries.GroupBy(e => e.ResSFile);
        foreach (var group in groups)
        {
            var offsetGroups = group.GroupBy(e => e.NewOffset).Where(g => g.Count() > 1);
            foreach (var overlapGroup in offsetGroups)
            {
                foreach (var entry in overlapGroup)
                {
                    entry.IsOverlap = true;
                    if (!issues.Contains(entry)) issues.Add(entry);
                }
            }
        }

        foreach (var entry in Entries)
        {
            if (entry.IsFailed && !issues.Contains(entry)) issues.Add(entry);
        }

        Dispatcher.UIThread.Post(() => {
            IssueEntries.Clear();
            foreach (var issue in issues) IssueEntries.Add(issue);
        });
    }

    private void LoadAllResources()
    {
        _oldResSData.Clear();
        var assetsFiles = WorkspaceItem.GetAssetsFileWorkspaceItems(_workspace.RootItems);
        foreach (var item in assetsFiles)
        {
            if (item.Object is not AssetsFileInstance asInst) continue;
            if (asInst.parentBundle == null) continue;

            var bun = asInst.parentBundle.file;
            foreach (var dir in bun.BlockAndDirInfo.DirectoryInfos)
            {
                if (dir.Name.EndsWith(".resS", StringComparison.OrdinalIgnoreCase) ||
                    dir.Name.EndsWith(".resource", StringComparison.OrdinalIgnoreCase))
                {
                    string resName = dir.Name;
                    if (!_oldResSData.ContainsKey(resName))
                    {
                        var data = BundleHelper.LoadAssetDataFromBundle(bun, resName);
                        if (data != null)
                            _oldResSData[resName] = data;
                    }
                }
            }
        }
    }

    private byte[] ReadOriginalResource(TextureCompressionEntry entry)
    {
        string resName = Path.GetFileName(entry.ResSFile);
        if (_oldResSData.TryGetValue(resName, out var resData))
        {
            if (entry.OldOffset + entry.OldSize <= resData.Length)
            {
                byte[] data = new byte[entry.OldSize];
                Array.Copy(resData, entry.OldOffset, data, 0, entry.OldSize);
                return data;
            }
        }

        var inst = entry.Asset.FileInstance;
        var rootPath = Path.GetDirectoryName(inst.path);
        string fixedPath = entry.ResSFile;

        if (inst.parentBundle == null && fixedPath.StartsWith("archive:/"))
            fixedPath = Path.GetFileName(fixedPath);
        
        if (!Path.IsPathRooted(fixedPath) && rootPath != null)
            fixedPath = Path.Combine(rootPath, fixedPath);

        if (File.Exists(fixedPath))
        {
            using var fs = File.OpenRead(fixedPath);
            fs.Position = entry.OldOffset;
            byte[] data = new byte[entry.OldSize];
            fs.Read(data, 0, data.Length);
            return data;
        }
        
        return Array.Empty<byte>();
    }

    private byte[] ProcessTexture(TextureCompressionEntry entry)
    {
        if (IsGpuAccelerationEnabled)
        {
            return ProcessTextureGpu(entry);
        }
        else
        {
            return ProcessTextureCpu(entry);
        }
    }

    private byte[] ProcessTextureCpu(TextureCompressionEntry entry)
    {
        TextureFile tex;
        byte[] encTextureData;
        
        var fileLock = _fileLocks.GetOrAdd(entry.Asset.FileInstance, _ => new object());

        lock (fileLock)
        {
            var baseField = _workspace.GetBaseField(entry.Asset);
            if (baseField == null) return Array.Empty<byte>();

            tex = TextureFile.ReadTextureFile(baseField);
            
            if (tex.m_Width <= MaxTextureSize && tex.m_Height <= MaxTextureSize)
                return ReadOriginalResource(entry);

            encTextureData = tex.FillPictureData(entry.Asset.FileInstance);
        }

        byte[] decBytes = tex.DecodeTextureRaw(encTextureData);
        if (decBytes == null) return ReadOriginalResource(entry);

        double ratio = Math.Min((double)MaxTextureSize / tex.m_Width, (double)MaxTextureSize / tex.m_Height);
        int newWidth = Math.Max(4, ((int)Math.Round(tex.m_Width * ratio) / 4) * 4);
        int newHeight = Math.Max(4, ((int)Math.Round(tex.m_Height * ratio) / 4) * 4);

        using (var image = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(decBytes, tex.m_Width, tex.m_Height))
        {
            image.Mutate(x => x.Resize(newWidth, newHeight, KnownResamplers.Lanczos3));

            BcEncoder encoder = new BcEncoder();
            encoder.OutputOptions.Quality = CompressionQuality.BestQuality;
            encoder.OutputOptions.GenerateMipMaps = (tex.m_MipCount > 1);
            encoder.OutputOptions.Format = MapUnityFormatToBCn((TextureFormat)tex.m_TextureFormat);

            byte[] pixelBytes = new byte[newWidth * newHeight * 4];
            image.CopyPixelDataTo(pixelBytes);

            byte[][] mipData = encoder.EncodeToRawBytes(pixelBytes, newWidth, newHeight, BCnEncoder.Encoder.PixelFormat.Bgra32);

            using (MemoryStream ms = new MemoryStream())
            {
                foreach (var level in mipData) ms.Write(level, 0, level.Length);
                
                int targetFormatID = tex.m_TextureFormat;
                if ((TextureFormat)tex.m_TextureFormat == TextureFormat.DXT1Crunched) targetFormatID = (int)TextureFormat.DXT1;
                if ((TextureFormat)tex.m_TextureFormat == TextureFormat.DXT5Crunched) targetFormatID = (int)TextureFormat.DXT5;

                entry.NewWidth = newWidth;
                entry.NewHeight = newHeight;
                entry.NewMipCount = mipData.Length;
                entry.NewFormat = targetFormatID;

                return ms.ToArray();
            }
        }
    }

    private byte[] ProcessTextureGpu(TextureCompressionEntry entry)
    {
        // GPU implemented using texconv.exe
        if (!GpuTextureProcessor.IsAvailable())
        {
            if (!_gpuErrorShown)
            {
                lock (_gpuErrorLock)
                {
                    if (!_gpuErrorShown)
                    {
                        _gpuErrorShown = true;
                        Dispatcher.UIThread.Post(async () => {
                            await MessageBoxUtil.ShowDialog("Error", "texconv.exe not found in Tools folder. Please install it to use GPU acceleration.");
                        });
                        IsGpuAccelerationEnabled = false;
                    }
                }
            }
            return ProcessTextureCpu(entry);
        }

        TextureFile tex;
        byte[] encTextureData;
        
        var fileLock = _fileLocks.GetOrAdd(entry.Asset.FileInstance, _ => new object());

        lock (fileLock)
        {
            var baseField = _workspace.GetBaseField(entry.Asset);
            if (baseField == null) return Array.Empty<byte>();

            tex = TextureFile.ReadTextureFile(baseField);
            
            if (tex.m_Width <= MaxTextureSize && tex.m_Height <= MaxTextureSize)
                return ReadOriginalResource(entry);

            encTextureData = tex.FillPictureData(entry.Asset.FileInstance);
        }

        byte[] decBytes = tex.DecodeTextureRaw(encTextureData);
        if (decBytes == null) return ReadOriginalResource(entry);

        double ratio = Math.Min((double)MaxTextureSize / tex.m_Width, (double)MaxTextureSize / tex.m_Height);
        int newWidth = Math.Max(4, ((int)Math.Round(tex.m_Width * ratio) / 4) * 4);
        int newHeight = Math.Max(4, ((int)Math.Round(tex.m_Height * ratio) / 4) * 4);

        using (var image = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(decBytes, tex.m_Width, tex.m_Height))
        {
            // We can let texconv resize, but we need to pass the original image first.
            // Wait, GpuTextureProcessor takes Image<Bgra32>.
            // We should let GpuTextureProcessor handle saving the file and calling texconv with resize args.
            
            // However, our CPU implementation resizes using Lanczos3 in ImageSharp first.
            // The user asked for "highest quality".
            // DirectTex's texconv also has high quality resizing (e.g. -if FANT/CUBIC).
            // Let's pass the original image to GpuTextureProcessor and let it resize and compress.
            
            // Note: ProcessTextureAsync is async but we are in a Parallel.ForEach which is synchronous blocking.
            // We need to run it synchronously here.
            
            var task = GpuTextureProcessor.ProcessTextureAsync(image, newWidth, newHeight, (TextureFormat)tex.m_TextureFormat, tex.m_MipCount > 1);
            byte[] processedBytes = task.GetAwaiter().GetResult();
            
            // We need to update entry with new dimensions and format
            // But GpuTextureProcessor returns raw bytes.
            // We need to know what format it chose.
            // It uses targetFormat passed in.
            
            int targetFormatID = tex.m_TextureFormat;
            if ((TextureFormat)tex.m_TextureFormat == TextureFormat.DXT1Crunched) targetFormatID = (int)TextureFormat.DXT1;
            if ((TextureFormat)tex.m_TextureFormat == TextureFormat.DXT5Crunched) targetFormatID = (int)TextureFormat.DXT5;

            entry.NewWidth = newWidth;
            entry.NewHeight = newHeight;
            // Mip count? If we asked for mips, texconv generates full chain.
            // Calculate mip count based on size
            int mipCount = 1;
            if (tex.m_MipCount > 1)
            {
                int w = newWidth;
                int h = newHeight;
                while (w > 1 || h > 1)
                {
                    w = Math.Max(1, w / 2);
                    h = Math.Max(1, h / 2);
                    mipCount++;
                }
            }
            entry.NewMipCount = mipCount;
            entry.NewFormat = targetFormatID;

            return processedBytes;
        }
    }

    private void SetFieldValue(AssetTypeValueField baseField, string name, int value)
    {
        var field = baseField[name];
        if (!field.IsDummy) field.AsInt = value;
    }

    private CompressionFormat MapUnityFormatToBCn(TextureFormat format)
    {
        return format switch
        {
            TextureFormat.DXT1 => CompressionFormat.Bc1,
            TextureFormat.DXT1Crunched => CompressionFormat.Bc1,
            TextureFormat.DXT5 => CompressionFormat.Bc3,
            TextureFormat.DXT5Crunched => CompressionFormat.Bc3,
            TextureFormat.BC7 => CompressionFormat.Bc7,
            TextureFormat.BC5 => CompressionFormat.Bc5,
            TextureFormat.BC4 => CompressionFormat.Bc4,
            _ => CompressionFormat.Bc3
        };
    }
    [RelayCommand]
    public async Task ApplyChanges()
    {
        long memBefore = GC.GetTotalMemory(false);
        StatusText = "Applying changes and updating asset trees...";
        
        await Task.Run(() => {
            var updateBatch = new List<(TextureCompressionEntry entry, AssetTypeValueField field)>();
            
            foreach (var entry in Entries)
            {
                if (_workspace == null) continue;
                var baseField = _workspace.GetBaseField(entry.Asset);
                if (baseField == null) continue;

                AssetTypeValueField targetData = baseField["m_StreamData"];
                if (targetData.IsDummy) continue;

                targetData["offset"].AsLong = entry.NewOffset;
                targetData["size"].AsLong = entry.Size;
                
                if (entry.IsTexture && entry.NewWidth > 0)
                {
                    SetFieldValue(baseField, "m_Width", entry.NewWidth);
                    SetFieldValue(baseField, "m_Height", entry.NewHeight);
                    SetFieldValue(baseField, "m_MipCount", entry.NewMipCount);
                    SetFieldValue(baseField, "m_TextureFormat", entry.NewFormat);

                    var imgDataField = baseField["image data"];
                    if (!imgDataField.IsDummy) imgDataField.Value.AsByteArray = Array.Empty<byte>();
                }

                // Apply to bundle info
                if (entry.Asset.FileInstance.parentBundle != null)
                {
                    var bunInst = entry.Asset.FileInstance.parentBundle;
                    var bundleItem = _workspace.FindWorkspaceItemByInstance(bunInst);
                    if (bundleItem != null)
                    {
                        string resFileName = Path.GetFileName(entry.ResSFile);
                        var resSItem = bundleItem.Children.FirstOrDefault(c => c.Name.Equals(resFileName, StringComparison.OrdinalIgnoreCase));
                        if (resSItem != null && resSItem.Object is AssetBundleDirectoryInfo info)
                        {
                            if (_newResSData.TryGetValue(entry.ResSFile, out var newData))
                            {
                                info.SetNewData(newData);
                                Dispatcher.UIThread.Post(() => _workspace.Dirty(resSItem));
                            }
                        }
                    }
                }

                updateBatch.Add((entry, baseField));
                if (updateBatch.Count >= 100)
                {
                    var batch = updateBatch;
                    updateBatch = new List<(TextureCompressionEntry entry, AssetTypeValueField field)>(); // Start fresh list for closure
                    Dispatcher.UIThread.Post(() => {
                        foreach(var b in batch) b.entry.Asset.UpdateAssetDataAndRow(_workspace!, b.field);
                        batch.Clear(); // Explicitly clear batch internal references
                    }, DispatcherPriority.Background);
                }
            }

            if (updateBatch.Count > 0)
            {
                var batch = updateBatch;
                Dispatcher.UIThread.Post(() => {
                    foreach(var b in batch) b.entry.Asset.UpdateAssetDataAndRow(_workspace!, b.field);
                    batch.Clear();
                }, DispatcherPriority.Background);
            }
        });

        // Break all local references to heavy data
        _newResSData.Clear();
        _oldResSData.Clear();
        _newResSData = new Dictionary<string, byte[]>();
        _oldResSData = new Dictionary<string, byte[]>();

        StatusText = "Yielding to UI thread for final rendering...";
        
        // Wait for ALL UI tasks to clear. Using a long delay plus priority check.
        await Task.Delay(500);
        var uiClearTcs = new TaskCompletionSource<bool>();
        Dispatcher.UIThread.Post(() => uiClearTcs.SetResult(true), DispatcherPriority.Background);
        await uiClearTcs.Task;

        StatusText = "Performing deep memory compaction (may take a moment)...";

        // Deep detached cleanup
        _ = Task.Run(async () => 
        {
            await Task.Delay(1500); // Wait for the method's state machine to definitely clear

            for (int i = 0; i < 5; i++)
            {
                SixLabors.ImageSharp.Configuration.Default.MemoryAllocator.ReleaseRetainedResources();
                System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
                
                // Collect everything including LOH
                GC.Collect(2, GCCollectionMode.Forced, true, true);
                GC.WaitForPendingFinalizers();
                GC.Collect(2, GCCollectionMode.Forced, true, true);
                
                await Task.Delay(500); // Longer wait to let OS and LOH settle
            }

            long memAfter = GC.GetTotalMemory(true);
            double releasedMB = Math.Max(0, (memBefore - memAfter) / 1024.0 / 1024.0);
            
            Dispatcher.UIThread.Post(() => {
                StatusText = $"Applied! Released {releasedMB:F2} MB. Please SAVE the file to finalize memory.";
            });
        });
    }

    [RelayCommand]
    public void OpenBatchDialog()
    {
        var vm = new UABEANext4.ViewModels.Tools.BatchTextureCompressionViewModel();
        var win = new UABEANext4.Views.Tools.BatchTextureCompressionWindow
        {
            DataContext = vm
        };
        Window? owner = null;
        try
        {
            owner = Util.WindowUtils.GetMainWindow();
        }
        catch { }
        if (owner != null)
            win.Show(owner);
        else
            win.Show();
    }
}

