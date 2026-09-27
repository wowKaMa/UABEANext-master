using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UABEANext4.Interfaces;
using UABEANext4.AssetWorkspace;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using AssetsTools.NET.Texture;
using Avalonia.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Avalonia.Threading;
using System.IO;
using SixLabors.ImageSharp.Processing;

namespace UABEANext4.ViewModels.Dialogs
{
    public partial class TextureSelectionItem : ObservableObject
    {
        public AssetInst Asset { get; }
        public string Name { get; }
        public string FileName { get; }
        [ObservableProperty] private Bitmap? _thumbnail;

        public TextureSelectionItem(AssetInst asset)
        {
            Asset = asset;
            Name = asset.AssetName ?? "Unnamed Texture";
            FileName = asset.FileInstance.name;
        }
    }

    public partial class SelectTextureViewModel : ObservableObject, IDialogAware<AssetInst>
    {
        public string Title => "选择贴图 (Select Texture 2D)";
        public int Width => 600;
        public int Height => 500;

        public event Action<AssetInst?>? RequestClose;

        private Workspace _workspace;
        private List<TextureSelectionItem> _allTextures;
        [ObservableProperty] private ObservableCollection<TextureSelectionItem> _textures;
        [ObservableProperty] private TextureSelectionItem? _selectedTexture;
        [ObservableProperty] private string _searchText = string.Empty;

        public SelectTextureViewModel(Workspace workspace, List<AssetInst> textureAssets)
        {
            _workspace = workspace;
            _allTextures = textureAssets.Select(a => new TextureSelectionItem(a)).ToList();
            _textures = new ObservableCollection<TextureSelectionItem>(_allTextures);
            
            // Start background thumbnail loading
            Task.Run(LoadThumbnails);
        }

        private void LoadThumbnails()
        {
            foreach (var item in _allTextures)
            {
                if (item.Thumbnail != null) continue;

                try
                {
                    var baseField = _workspace.GetBaseField(item.Asset);
                    if (baseField == null) continue;

                    var texFile = TextureFile.ReadTextureFile(baseField);
                    if (texFile == null) continue;

                    // Load at small size for thumbnail
                    byte[] encTextureData = texFile.FillPictureData(item.Asset.FileInstance);
                    byte[] decBytes = texFile.DecodeTextureRaw(encTextureData);

                    if (decBytes != null)
                    {
                        using (var image = Image.LoadPixelData<Bgra32>(decBytes, texFile.m_Width, texFile.m_Height))
                        {
                            // Resize to thumbnail size
                            int thumbSize = 64;
                            image.Mutate(x => x.Resize(new SixLabors.ImageSharp.Size(thumbSize, thumbSize)));

                            using (var ms = new MemoryStream())
                            {
                                image.SaveAsPng(ms);
                                ms.Position = 0;
                                var bitmap = new Bitmap(ms);
                                Dispatcher.UIThread.Post(() => item.Thumbnail = bitmap);
                            }
                        }
                    }
                }
                catch
                {
                    // Ignore errors for individual thumbnails
                }
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Textures = new ObservableCollection<TextureSelectionItem>(_allTextures);
            }
            else
            {
                var filtered = _allTextures.Where(t => 
                    t.Name.Contains(value, StringComparison.OrdinalIgnoreCase) || 
                    t.FileName.Contains(value, StringComparison.OrdinalIgnoreCase)
                ).ToList();
                Textures = new ObservableCollection<TextureSelectionItem>(filtered);
            }
        }

        [RelayCommand]
        private void Confirm()
        {
            RequestClose?.Invoke(SelectedTexture?.Asset);
        }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose?.Invoke(null);
        }
    }
}
