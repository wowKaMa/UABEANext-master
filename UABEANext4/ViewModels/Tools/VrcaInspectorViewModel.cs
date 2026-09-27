using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UABEANext4.Interfaces;
using UABEANext4.Logic.UnityFS;
using UABEANext4.Services;
using UABEANext4.Util;
using Avalonia.Platform.Storage;

namespace UABEANext4.ViewModels.Tools
{
    public partial class VrcaInspectorViewModel : ObservableObject, IDialogAware
    {
        public string Title => "VRCA 深度结构审查器 (VRCA Deep Inspector)";
        public int Width => 900;
        public int Height => 700;


        [ObservableProperty]
        private string _filePath = "";

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private UnityFSParser.AnalysisReport? _report;

        public IAsyncRelayCommand SelectFileCommand { get; }
        public IAsyncRelayCommand AnalyzeCommand { get; }

        public VrcaInspectorViewModel()
        {
            SelectFileCommand = new AsyncRelayCommand(SelectFile);
            AnalyzeCommand = new AsyncRelayCommand(Analyze);
        }

        private async Task SelectFile()
        {
            var storageProvider = StorageService.GetStorageProvider();
            if (storageProvider == null) return;

            var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择 VRCA/VRCW 文件",
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Unity Bundle Files (*.vrca;*.vrcw;*.unity3d)") { Patterns = new[] { "*.vrca", "*.vrcw", "*.unity3d", "*" } }
                },
                AllowMultiple = false
            });

            if (result.Count > 0 && result[0] != null)
            {
                FilePath = result[0]!.Path.LocalPath; // Use LocalPath for IO.File
                await Analyze();
            }
        }

        private async Task Analyze()
        {
            if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath)) return;

            IsBusy = true;
            try
            {
                var parser = new UnityFSParser();
                Report = await parser.AnalyzeFileAsync(FilePath);
            }
            catch (Exception ex)
            {
                await MessageBoxUtil.ShowDialog("分析失败", $"发生错误: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
