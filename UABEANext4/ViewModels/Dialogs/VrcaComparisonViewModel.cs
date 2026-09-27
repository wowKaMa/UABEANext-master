using System;
using AvaloniaEdit.Document;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UABEANext4.AssetWorkspace;
using UABEANext4.Logic.ImportExport;
using UABEANext4.Util;
using CommunityToolkit.Mvvm.Messaging;
using UABEANext4.Logic;
using UABEANext4.ViewModels;

namespace UABEANext4.ViewModels.Dialogs
{
    public partial class VrcaComparisonViewModel : ViewModelBase
    {
        private readonly Workspace _workspace;

        [ObservableProperty] private TextDocument _sourceDump = new();
        [ObservableProperty] private TextDocument _targetDump = new();
        [ObservableProperty] private TextDocument _resultDump = new();

        // Diff line markers
        public HashSet<int> SourceDiffLines { get; } = new();
        public HashSet<int> TargetDiffLines { get; } = new();
        public HashSet<int> ResultDiffLines { get; } = new();

        [ObservableProperty] private long _sourcePathId;
        [ObservableProperty] private long _targetPathId;
        
        [ObservableProperty] private AssetsFileInstance _sourceFile;
        [ObservableProperty] private AssetsFileInstance _targetFile;

        public VrcaComparisonViewModel(Workspace workspace, AssetsFileInstance sourceFile, long sourcePathId, AssetsFileInstance targetFile, long targetPathId)
        {
            _workspace = workspace;
            _sourceFile = sourceFile;
            _sourcePathId = sourcePathId;
            _targetFile = targetFile;
            _targetPathId = targetPathId;

            _ = InitializeDumps();
        }

        private async Task InitializeDumps()
        {
            var srcStr = await Task.Run(() => GetDumpText(SourceFile, SourcePathId));
            var tgtStr = await Task.Run(() => GetDumpText(TargetFile, TargetPathId));
            
            SourceDump = new TextDocument(srcStr);
            TargetDump = new TextDocument(tgtStr);
            ResultDump = new TextDocument(srcStr); // Assuming applied for now

            CalculateDiffs();
        }

        private void CalculateDiffs()
        {
            var srcLines = SourceDump.Text.Split('\n');
            var tgtLines = TargetDump.Text.Split('\n');

            int max = Math.Max(srcLines.Length, tgtLines.Length);
            for (int i = 0; i < max; i++)
            {
                string? s = i < srcLines.Length ? srcLines[i]?.Trim() : null;
                string? t = i < tgtLines.Length ? tgtLines[i]?.Trim() : null;

                if (s != t)
                {
                    if (i < srcLines.Length) SourceDiffLines.Add(i + 1);
                    if (i < tgtLines.Length) TargetDiffLines.Add(i + 1);
                    ResultDiffLines.Add(i + 1);
                }
            }
        }

        private string GetDumpText(AssetsFileInstance fileInst, long pathId)
        {
            if (pathId == 0) return "(N/A)";
            
            var baseField = _workspace.GetBaseField(fileInst, pathId);
            if (baseField == null) return "Failed to deserialize asset.";

            using var ms = new MemoryStream();
            var exporter = new AssetExport(ms);
            exporter.DumpTextAsset(baseField);
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        [RelayCommand]
        private void JumpToSource()
        {
            NavigateToAsset(SourceFile, SourcePathId);
        }

        [RelayCommand]
        private void JumpToTarget()
        {
            NavigateToAsset(TargetFile, TargetPathId);
        }

        private void NavigateToAsset(AssetsFileInstance fileInst, long pathId)
        {
             if (pathId == 0) return;
             
             // Ensure we are working with an asset instance from the correct file
             var asset = _workspace.GetAssetInst(fileInst, 0, pathId);
             if (asset != null)
             {
                 // RequestVisitAssetMessage is the one handled by MainViewModel to locate/jump to asset
                 WeakReferenceMessenger.Default.Send(new RequestVisitAssetMessage(asset));
             }
             else
             {
                 _ = MessageBoxUtil.ShowDialog("Navigation Error", $"Could not find asset with Path ID {pathId} in the selected file.", MessageBoxType.OK);
             }
        }
    }
}
