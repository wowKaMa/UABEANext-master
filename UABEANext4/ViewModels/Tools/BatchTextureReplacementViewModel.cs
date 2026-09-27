using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetsTools.NET.Texture;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UABEANext4.AssetWorkspace;
using UABEANext4.Logic;
using UABEANext4.Services;
using UABEANext4.Util;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using BCnEncoder.Encoder;
using BCnEncoder.Shared;
using Avalonia.Threading;

namespace UABEANext4.ViewModels.Tools
{
    public partial class BatchTextureReplacementViewModel : ViewModelBase
    {
        private readonly Workspace _workspace;
        private Dictionary<string, byte[]> _rawResourceFiles = new(StringComparer.OrdinalIgnoreCase);
        private Dictionary<string, byte[]> _newResSData = new(StringComparer.OrdinalIgnoreCase);
        private List<ResourceLink> _allResourceLinks = new();

        [ObservableProperty]
        private string _replacementFolderPath = string.Empty;

        [ObservableProperty]
        private ObservableCollection<TextureReplacementItem> _textureItems = new();

        [ObservableProperty]
        private string _statusText = "准备就绪 (Ready)";

        [ObservableProperty]
        private double _progressValue;

        [ObservableProperty]
        private bool _isProcessing;

        // Progress breakdown
        [ObservableProperty] private int _processedCount;
        [ObservableProperty] private int _totalCount;

        public BatchTextureReplacementViewModel(Workspace workspace)
        {
            _workspace = workspace;
            // Auto-scan on load
            ScanWorkspace();
        }
        
        // Constructor for XAML preview
        public BatchTextureReplacementViewModel() 
        { 
             _workspace = new Workspace();
        }

        [RelayCommand]
        private async Task BrowseReplacementFolder()
        {
            var storageProvider = StorageService.GetStorageProvider();
            if (storageProvider is null) return;

            var result = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "选择替换文件夹 (Select Replacement Folder)"
            });

            if (result.Count > 0)
            {
                ReplacementFolderPath = result[0].Path.LocalPath;
                ScanForReplacements();
            }
        }

        private void ScanWorkspace()
        {
            StatusText = "正在扫描已加载资产... (Scanning loaded assets...)";
            IsProcessing = true;
            TextureItems.Clear();
            _rawResourceFiles.Clear();

            Task.Run(() =>
            {
                try
                {
                    var newItems = new List<TextureReplacementItem>();
                    var loadedBundles = new HashSet<BundleFileInstance>();

                    // 1. Scan loaded bundles for resources
                    foreach (var rootItem in _workspace.RootItems)
                    {
                        if (rootItem.Object is BundleFileInstance bunInst)
                        {
                            loadedBundles.Add(bunInst);
                            foreach (var dir in bunInst.file.BlockAndDirInfo.DirectoryInfos)
                            {
                                if (dir.Name.EndsWith(".resS", StringComparison.OrdinalIgnoreCase) ||
                                    dir.Name.EndsWith(".resource", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Load raw data from bundle
                                    // Make sure BundleHelper is available or use implicit knowledge
                                    // Assuming BundleHelper is available as seen in other files
                                    if (!_rawResourceFiles.ContainsKey(dir.Name))
                                    {
                                        _rawResourceFiles[dir.Name] = BundleHelper.LoadAssetDataFromBundle(bunInst.file, dir.Name);
                                    }
                                }
                            }
                        }
                    }

                    // 2. Scan Assets for ALL external resource references (Strict follow tihuan.txt)
                    var allLinks = new List<ResourceLink>();
                    var assetItems = WorkspaceItem.GetAssetsFileWorkspaceItems(_workspace.RootItems);
                    foreach (var item in assetItems)
                    {
                        if (item.Object is not AssetsFileInstance asInst) continue;

                        foreach (var info in asInst.file.AssetInfos)
                        {
                            var assetInst = _workspace.GetAssetInst(asInst, 0, info.PathId);
                            if (assetInst == null) continue;

                            var baseField = _workspace.GetBaseField(assetInst);
                            if (baseField == null) continue;

                            ProbeResourceLinks(assetInst, baseField, _rawResourceFiles, allLinks);
                            
                            // If it's a Texture2D, also add to UI list
                            if (info.TypeId == (int)AssetClassID.Texture2D)
                            {
                                var uiItem = CreateUiItem(assetInst, baseField);
                                if (uiItem != null) newItems.Add(uiItem);
                            }
                        }
                    }

                    _allResourceLinks = allLinks;

                    Avalonia.Threading.Dispatcher.UIThread.Post(() => {
                        foreach(var item in newItems) TextureItems.Add(item);
                        StatusText = $"发现 {TextureItems.Count} 个贴图 (Found {TextureItems.Count} textures)";
                        IsProcessing = false;
                        
                        // Auto-scan if folder is already selected
                        if (!string.IsNullOrEmpty(ReplacementFolderPath))
                        {
                            ScanForReplacements();
                        }
                    });
                }
                catch (Exception ex)
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => {
                        StatusText = $"Error: {ex.Message}";
                        IsProcessing = false;
                    });
                }
            });
        }
               private TextureReplacementItem? CreateUiItem(AssetInst assetInst, AssetTypeValueField field)
        {
            string name = field["m_Name"].AsString;
            long offset = 0;
            long size = 0;
            string resName = "";

            var streamData = field.Get("m_StreamData");
            if (streamData != null && !streamData.IsDummy && streamData["path"].AsString.Length > 0)
            {
                resName = Path.GetFileName(streamData["path"].AsString);
                offset = streamData["offset"].AsLong;
                size = streamData["size"].AsLong;
            }
            else
            {
                var resData = field.Get("m_Resource");
                if (resData != null && !resData.IsDummy && resData["m_Source"].AsString.Length > 0)
                {
                    resName = Path.GetFileName(resData["m_Source"].AsString);
                    offset = resData["m_Offset"].AsLong;
                    size = resData["m_Size"].AsLong;
                }
            }

            if (string.IsNullOrEmpty(resName)) return null;

            return new TextureReplacementItem
            {
                Asset = assetInst,
                Name = name,
                PathId = assetInst.PathId,
                ResFileName = resName,
                OriginalOffset = offset,
                OriginalSize = size,
                OriginalFormat = (TextureFormat)field["m_TextureFormat"].AsInt,
                Status = "Pending"
            };
        }

        private void ProbeResourceLinks(AssetInst assetInst, AssetTypeValueField field, Dictionary<string, byte[]> validResFiles, List<ResourceLink> collector)
        {
            var streamData = field.Get("m_StreamData");
            if (streamData != null && !streamData.IsDummy)
            {
                AddResourceLinkIfValid(assetInst, field, streamData, "path", "offset", "size", validResFiles, collector);
            }
            else
            {
                var resData = field.Get("m_Resource");
                if (resData != null && !resData.IsDummy)
                {
                    AddResourceLinkIfValid(assetInst, field, resData, "m_Source", "m_Offset", "m_Size", validResFiles, collector);
                }
            }
        }

        private void AddResourceLinkIfValid(AssetInst assetInst, AssetTypeValueField baseField, AssetTypeValueField container, string pName, string oName, string sName, Dictionary<string, byte[]> validResFiles, List<ResourceLink> collector)
        {
            string fullPath = container.Get(pName).AsString;
            string resName = Path.GetFileName(fullPath);
            long size = container.Get(sName).AsLong;
            long offset = container.Get(oName).AsLong;

            if (validResFiles.ContainsKey(resName) && size > 0)
            {
                collector.Add(new ResourceLink
                {
                    AssetInst = assetInst,
                    BaseField = baseField,
                    DataContainer = container,
                    ResFileName = resName,
                    OldOffset = offset,
                    OldSize = size,
                    OffsetFieldName = oName,
                    SizeFieldName = sName
                });
            }
        }

        private void ScanForReplacements()
        {
            if (string.IsNullOrEmpty(ReplacementFolderPath) || !Directory.Exists(ReplacementFolderPath)) return;

            int foundCount = 0;
            foreach (var item in TextureItems)
            {
                string pngPath = Path.Combine(ReplacementFolderPath, item.Name + ".png");
                string jpgPath = Path.Combine(ReplacementFolderPath, item.Name + ".jpg");

                if (File.Exists(pngPath))
                {
                    item.ReplacementPath = pngPath;
                    item.Status = "Found (PNG)";
                    item.HasReplacement = true;
                    foundCount++;
                }
                else if (File.Exists(jpgPath))
                {
                    item.ReplacementPath = jpgPath;
                    item.Status = "Found (JPG)";
                    item.HasReplacement = true;
                    foundCount++;
                }
                else
                {
                    item.ReplacementPath = null;
                    item.Status = "Not Found (未找到)";
                    item.HasReplacement = false;
                }
            }
            StatusText = $"找到 {foundCount} 个替换文件 (Found {foundCount} replacements)";
        }

        [RelayCommand]
        private async Task ProcessReplacements()
        {
            var itemsToProcess = TextureItems.Where(x => x.HasReplacement).ToList();
            if (itemsToProcess.Count == 0)
            {
                StatusText = "No replacements to process.";
                return;
            }

            IsProcessing = true;
            ProgressValue = 0;
            ProcessedCount = 0;
            TotalCount = itemsToProcess.Count;

            await Task.Run(() =>
            {
                try
                {
                    // 1. Process Textures (Image Processing & Encoding)
                    foreach (var item in itemsToProcess)
                    {
                        try
                        {
                            Avalonia.Threading.Dispatcher.UIThread.Post(() => StatusText = $"Encoding {item.Name}...");

                            ProcessSingleTexture(item);
                            
                            Avalonia.Threading.Dispatcher.UIThread.Post(() => 
                            {
                                item.Status = "Encoded (已编码)";
                                ProcessedCount++;
                                ProgressValue = (double)ProcessedCount / TotalCount * 0.5; // First 50%
                            });
                        }
                        catch (Exception ex)
                        {
                            Avalonia.Threading.Dispatcher.UIThread.Post(() => item.Status = $"Error: {ex.Message}");
                        }
                    }

                    // 2. Rebuild Resources
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => StatusText = "Rebuilding Resources... (重建资源...)");
                    RebuildResources(itemsToProcess);

                    // 3. Apply Changes to Workspace
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => StatusText = "Applying Changes... (应用更改...)");
                    ApplyChanges(itemsToProcess);

                    Avalonia.Threading.Dispatcher.UIThread.Post(() => {
                        StatusText = "更改已应用! 请通过主菜单保存文件 (Changes Applied! Please SAVE the file via main menu)";
                        ProgressValue = 1.0;
                    });

                }
                catch (Exception ex)
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => StatusText = $"Error: {ex.Message}");
                }
                finally
                {
                    IsProcessing = false;
                }
            });
        }

        private void RebuildResources(List<TextureReplacementItem> processedItems)
        {
            var groups = _allResourceLinks.GroupBy(r => r.ResFileName);
            bool anyChange = false;

            foreach (var group in groups)
            {
                string resName = group.Key;
                if (!_rawResourceFiles.ContainsKey(resName)) continue;

                byte[] fullResData = _rawResourceFiles[resName];
                StreamBuilder builder = new StreamBuilder();

                // Sort by old offset to preserve order (Stictly follow tihuan.txt)
                var sortedLinks = group.OrderBy(x => x.OldOffset).ToList();

                foreach (var link in sortedLinks)
                {
                    byte[] dataToSave;
                    var replacedItem = processedItems.FirstOrDefault(p => p.Asset == link.AssetInst);

                    if (replacedItem != null && replacedItem.NewData != null)
                    {
                        dataToSave = replacedItem.NewData;
                        anyChange = true;

                        // Match tihuan.txt ProcessTextureReplacement logic
                        SetFieldValue(link.BaseField, "m_Width", replacedItem.NewWidth);
                        SetFieldValue(link.BaseField, "m_Height", replacedItem.NewHeight);
                        SetFieldValue(link.BaseField, "m_MipCount", replacedItem.NewMipCount);
                        SetFieldValue(link.BaseField, "m_TextureFormat", replacedItem.NewFormat);

                        var compSize = link.BaseField.Get("m_CompleteImageSize");
                        if (compSize != null && !compSize.IsDummy) compSize.AsInt = dataToSave.Length;

                        // Match tihuan.txt: "清空“image data”数组以强制从 .resS 读取"
                        var imgData = link.BaseField.Get("image data");
                        if (imgData != null && !imgData.IsDummy) imgData.Value.AsByteArray = Array.Empty<byte>();
                    }
                    else
                    {
                        // Load original data
                        if (link.OldOffset + link.OldSize <= fullResData.Length)
                        {
                            dataToSave = new byte[link.OldSize];
                            Array.Copy(fullResData, link.OldOffset, dataToSave, 0, link.OldSize);
                        }
                        else
                        {
                            dataToSave = Array.Empty<byte>();
                        }
                    }

                    // Write to new stream with 16-byte alignment (Strict follow tihuan.txt)
                    long newOffset = builder.WriteAligned(dataToSave);
                    long newSize = dataToSave.Length;

                    if (newOffset != link.OldOffset || newSize != link.OldSize) anyChange = true;

                    // Update metadata
                    link.DataContainer.Get(link.OffsetFieldName).AsLong = newOffset;
                    link.DataContainer.Get(link.SizeFieldName).AsLong = newSize;

                    // Persist changes to AssetInfo
                    link.AssetInst.SetNewData(link.BaseField);

                    // Update UI
                    Avalonia.Threading.Dispatcher.UIThread.Post(() => link.AssetInst.UpdateAssetDataAndRow(_workspace, link.BaseField));
                }

                _newResSData[resName] = builder.Stream.ToArray();
            }
        }

        private void ApplyChanges(List<TextureReplacementItem> processedItems)
        {
             // 1. Update Asset Bundle Directory Infos with new .resS data
             foreach (var kvp in _newResSData)
             {
                 string resName = kvp.Key;
                 byte[] newData = kvp.Value;

                 foreach (var rootItem in _workspace.RootItems)
                 {
                     if (rootItem.Object is BundleFileInstance bunInst)
                     {
                        var dirInfo = bunInst.file.BlockAndDirInfo.DirectoryInfos.FirstOrDefault(d => d.Name.Equals(resName, StringComparison.OrdinalIgnoreCase));
                        if (dirInfo != null)
                        {
                            dirInfo.SetNewData(newData);
                             // Need to mark the Bundle WorkspaceItem child as dirty
                             var bunItem = _workspace.FindWorkspaceItemByInstance(bunInst);
                             if (bunItem != null)
                             {
                                 var child = bunItem.Children.FirstOrDefault(c => c.Name.Equals(resName, StringComparison.OrdinalIgnoreCase));
                                 if (child != null) _workspace.Dirty(child);
                             }
                        }
                     }
                 }
             }

             // Note: Metadata (Width, Height, Format, Offsets) for all assets 
             // were already updated in the RebuildResources loop to ensure consistency.
        }

        private void ProcessSingleTexture(TextureReplacementItem item)
        {
            if (item.ReplacementPath == null) return;

            // Match tihuan.txt ProcessTextureReplacement
            using (var image = Image.Load<Bgra32>(item.ReplacementPath))
            {
                // Match flipping logic: "x.Rotate(RotateMode.Rotate180); x.Flip(FlipMode.Horizontal);"
                image.Mutate(x =>
                {
                    x.Rotate(RotateMode.Rotate180);
                    x.Flip(FlipMode.Horizontal);
                });

                var encoder = new BcEncoder();
                var baseField = _workspace.GetBaseField(item.Asset);
                if (baseField == null) return;
                
                int m_MipCount = baseField["m_MipCount"].AsInt;
                int m_TextureFormat = baseField["m_TextureFormat"].AsInt;
                
                encoder.OutputOptions.GenerateMipMaps = m_MipCount > 1;
                encoder.OutputOptions.Quality = CompressionQuality.Balanced;
                encoder.OutputOptions.Format = MapUnityFormatToBCn((TextureFormat)m_TextureFormat);

                byte[] pixelBytes = new byte[image.Width * image.Height * 4];
                image.CopyPixelDataTo(pixelBytes);

                byte[][] mipData = encoder.EncodeToRawBytes(pixelBytes, image.Width, image.Height, BCnEncoder.Encoder.PixelFormat.Bgra32);

                using (MemoryStream ms = new MemoryStream())
                {
                    foreach (var level in mipData) ms.Write(level, 0, level.Length);
                    item.NewData = ms.ToArray();
                }

                // Update metadata props for later use
                TextureFormat originalFmt = (TextureFormat)m_TextureFormat;
                int targetFmt = (int)originalFmt;
                if (originalFmt == TextureFormat.DXT1Crunched) targetFmt = (int)TextureFormat.DXT1;
                if (originalFmt == TextureFormat.DXT5Crunched) targetFmt = (int)TextureFormat.DXT5;
                if (originalFmt == TextureFormat.ETC_RGB4Crunched) targetFmt = (int)TextureFormat.ETC_RGB4;
                if (originalFmt == TextureFormat.ETC2_RGBA8Crunched) targetFmt = (int)TextureFormat.ETC2_RGBA8;

                item.NewWidth = image.Width;
                item.NewHeight = image.Height;
                item.NewMipCount = mipData.Length;
                item.NewFormat = targetFmt;
            }
        }

        private static CompressionFormat MapUnityFormatToBCn(TextureFormat format)
        {
             switch (format)
            {
                case TextureFormat.DXT1:
                case TextureFormat.DXT1Crunched:
                    return CompressionFormat.Bc1;
                case TextureFormat.DXT5:
                case TextureFormat.DXT5Crunched:
                    return CompressionFormat.Bc3;
                case TextureFormat.BC7:
                    return CompressionFormat.Bc7;
                case TextureFormat.BC5:
                    return CompressionFormat.Bc5;
                case TextureFormat.BC4:
                    return CompressionFormat.Bc4;
                default:
                    return CompressionFormat.Bc3;
            }
        }
        
        private static void SetFieldValue(AssetTypeValueField baseField, string name, int value)
        {
            var field = baseField.Get(name);
            if (field != null && !field.IsDummy) field.AsInt = value;
        }

        // Inner Classes (Strict follow tihuan.txt)
        public class ResourceLink
        {
            public required AssetInst AssetInst;
            public required AssetTypeValueField BaseField;
            public required AssetTypeValueField DataContainer;
            public required string ResFileName;
            public long OldOffset;
            public long OldSize;
            public required string OffsetFieldName;
            public required string SizeFieldName;
            
            public long NewOffset;
            public long NewSize;
        }

        public class StreamBuilder
        {
            public MemoryStream Stream = new MemoryStream();
            public long WriteAligned(byte[] data)
            {
                long padding = (16 - (Stream.Position % 16)) % 16;
                if (padding > 0) Stream.Write(new byte[padding], 0, (int)padding);
                long startPos = Stream.Position;
                Stream.Write(data, 0, data.Length);
                return startPos;
            }
        }
    }
    
    public partial class TextureReplacementItem : ObservableObject
    {
        public required AssetInst Asset { get; set; }
        public required string Name { get; set; }
        public long PathId { get; set; }
        public required string ResFileName { get; set; }
        public long OriginalOffset { get; set; }
        public long OriginalSize { get; set; }
        public TextureFormat OriginalFormat { get; set; }
        
        [ObservableProperty]
        private string _status = "";
        
        [ObservableProperty]
        private string? _replacementPath;
        
        [ObservableProperty]
        private bool _hasReplacement;
        
        // Temp storage for new data
        public byte[]? NewData { get; set; }
        public int NewWidth { get; set; }
        public int NewHeight { get; set; }
        public int NewMipCount { get; set; }
        public int NewFormat { get; set; }
        public long NewOffset { get; set; }
        public long NewSize { get; set; }
    }
}
