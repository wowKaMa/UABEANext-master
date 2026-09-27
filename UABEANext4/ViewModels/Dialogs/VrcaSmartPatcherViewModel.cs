using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using UABEANext4.AssetWorkspace;
using UABEANext4.Logic;
using UABEANext4.Logic.Modding;
using Avalonia.Media;
using UABEANext4.Util;
using UABEANext4.Views.Dialogs;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using UABEANext4.ViewModels;
using Avalonia.Controls;

namespace UABEANext4.ViewModels.Dialogs
{
    public partial class VrcaSmartPatcherViewModel : ViewModelBase
    {
        internal readonly Workspace _workspace;

        [ObservableProperty] private AssetsFileInstance? _sourceFile;
        [ObservableProperty] private AssetsFileInstance? _targetFile;
        [ObservableProperty] private bool _isAnalyzing;
        [ObservableProperty] private ObservableCollection<MatchPairViewModel> _differences = new();
        [ObservableProperty] private ObservableCollection<AssetsFileInstance> _availableFiles = new();

        public VrcaSmartPatcherViewModel(Workspace workspace)
        {
            _workspace = workspace;
            LoadAvailableFiles();
        }

        private void LoadAvailableFiles()
        {
            var files = new List<AssetsFileInstance>();
            foreach (var rootItem in _workspace.RootItems)
            {
                GatherAssetsFilesRecursive(rootItem, files);
            }
            
            AvailableFiles.Clear();
            foreach (var f in files.Distinct()) AvailableFiles.Add(f);
        }

        private void GatherAssetsFilesRecursive(WorkspaceItem item, List<AssetsFileInstance> files)
        {
            if (item.Object is AssetsFileInstance afi)
            {
                files.Add(afi);
            }
            foreach (var child in item.Children)
            {
                GatherAssetsFilesRecursive(child, files);
            }
        }

        [RelayCommand]
        private async Task AnalyzeDifferences()
        {
            if (SourceFile == null || TargetFile == null) return;

            IsAnalyzing = true;
            try
            {
                // 1. Export both sides to JSON
                var sourceExporter = new VrcaDataExporter(_workspace.Manager, SourceFile);
                var targetExporter = new VrcaDataExporter(_workspace.Manager, TargetFile);
                
                string sourceJson = sourceExporter.Export();
                string targetJson = targetExporter.Export();

                string tempPath = Path.Combine(Path.GetTempPath(), "UABEANext_Modding");
                if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);

                string sourceFile = Path.Combine(tempPath, "source_dump.json");
                string targetFile = Path.Combine(tempPath, "target_dump.json");
                string planFile = Path.Combine(tempPath, "patch_plan.json");

                await File.WriteAllTextAsync(sourceFile, sourceJson);
                await File.WriteAllTextAsync(targetFile, targetJson);

                // 2. Call Python Analyzer
                string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logic", "Modding", "Scripts", "analyzer.py");
                if (!File.Exists(scriptPath)) 
                    scriptPath = @"e:\UABEANext-master\UABEANext-master\UABEANext4\Logic\Modding\Scripts\analyzer.py";

                bool success = await RunPythonScript("python", scriptPath, sourceFile, targetFile, planFile);
                if (!success)
                {
                    success = await RunPythonScript("python3", scriptPath, sourceFile, targetFile, planFile);
                }

                if (!success)
                {
                    await MessageBoxUtil.ShowDialog("Error", "Failed to run Python analyzer. Please ensure Python is installed and added to PATH.", MessageBoxType.OK);
                    return;
                }

                // 3. Parse Plan and Populate Differences
                if (File.Exists(planFile))
                {
                    string planJson = await File.ReadAllTextAsync(planFile);
                    var plan = JsonConvert.DeserializeObject<dynamic>(planJson);
                    
                    Differences.Clear();
                    if (plan != null && plan["actions"] != null)
                    {
                        foreach (var action in plan["actions"])
                        {
                            if (action == null) continue;
                            
                            string? type = (string?)action["type"];
                            if (type == "COPY_ASSET")
                            {
                                Differences.Add(new MatchPairViewModel(this) {
                                    SourceName = (string?)action["reason"] ?? "Unknown Asset",
                                    Status = "New",
                                    AssetType = "Menu",
                                    SourcePathId = (long?)action["source_path_id"] ?? 0,
                                    TargetPathId = 0
                                });
                            }
                            else if (type == "MODIFY_ASSET")
                            {
                                var match = new MatchPairViewModel(this) {
                                    SourceName = (string?)action["reason"] ?? "Modified Asset",
                                    Status = "Modified",
                                    AssetType = "Menu",
                                    SourcePathId = (long?)action["source_path_id"] ?? 0,
                                    TargetPathId = (long?)action["target_path_id"] ?? 0
                                };
                                if (match.TargetPathId != 0)
                                {
                                    var asset = _workspace.GetAssetInst(TargetFile, 0, match.TargetPathId);
                                    if (asset != null) match.TargetName = asset.AssetName;
                                }
                                Differences.Add(match);
                            }
                            else if (type == "ADD_PARAM" || type == "MODIFY_PARAM")
                            {
                                var match = new MatchPairViewModel(this) {
                                    SourceName = (string?)action["data"]?["name"] ?? "Unknown Parameter",
                                    Status = type == "ADD_PARAM" ? "New" : "Modified",
                                    AssetType = "Parameter",
                                    SourcePathId = (long?)action["source_path_id"] ?? 0,
                                    TargetPathId = (long?)action["target_path_id"] ?? 0
                                };
                                if (match.TargetPathId != 0)
                                {
                                    var asset = _workspace.GetAssetInst(TargetFile, 0, match.TargetPathId);
                                    if (asset != null) match.TargetName = asset.AssetName;
                                }
                                Differences.Add(match);
                            }
                        }
                    }

                    if (Differences.Count == 0)
                    {
                         await MessageBoxUtil.ShowDialog("Analysis Info", "No differences found between source and target.", MessageBoxType.OK);
                    }
                }
                else
                {
                    await MessageBoxUtil.ShowDialog("Error", "Patch plan file was not generated.", MessageBoxType.OK);
                }
            }
            catch (Exception ex)
            {
                await MessageBoxUtil.ShowDialog("Analysis Exception", $"An error occurred during analysis: {ex}", MessageBoxType.OK);
            }
            finally
            {
                IsAnalyzing = false;
            }
        }

        [RelayCommand]
        public async Task ApplyPatch()
        {
            if (SourceFile == null || TargetFile == null)
            {
                await MessageBoxUtil.ShowDialog("Error", "Please select source and target files.", MessageBoxType.OK);
                return;
            }
            
            string tempPath = Path.Combine(Path.GetTempPath(), "UABEANext_Modding");
            string planFile = Path.Combine(tempPath, "patch_plan.json");
            
            if (!File.Exists(planFile))
            {
                await MessageBoxUtil.ShowDialog("Error", "Patch plan not found. Please analyze differences first.", MessageBoxType.OK);
                return;
            }

            try
            {
                var executor = new PatchExecutor(_workspace, SourceFile, TargetFile);
                string instructions = await File.ReadAllTextAsync(planFile);
                
                int actionCount = await Task.Run(() => executor.Execute(instructions));
                
                var targetItem = _workspace.FindWorkspaceItemByInstance(TargetFile);
                if (targetItem != null) _workspace.Dirty(targetItem);

                await MessageBoxUtil.ShowDialog("Patch Applied", 
                    $"Successfully applied {actionCount} changes to {TargetFile.name}.\n\n" +
                    "NOTE: Changes are currently IN-MEMORY. You must click 'Save' in the main window to write them to the file.", 
                    MessageBoxType.OK);

                Differences.Clear();
            }
            catch (Exception ex)
            {
                await MessageBoxUtil.ShowDialog("Patch Error", $"Failed to apply patch:\n{ex.Message}\n{ex.StackTrace}", MessageBoxType.OK);
            }
        }
        private async Task<bool> RunPythonScript(string cmd, string scriptPath, string sourceFile, string targetFile, string planFile)
        {
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = $"\"{scriptPath}\" \"{sourceFile}\" \"{targetFile}\" \"{planFile}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = System.Diagnostics.Process.Start(startInfo);
                if (process == null) return false;

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0)
                {
                    if (!string.IsNullOrEmpty(error) || !string.IsNullOrEmpty(output))
                    {
                         await MessageBoxUtil.ShowDialog("Python Error", $"Analyzer failed with exit code {process.ExitCode}.\nError: {error}\nOutput: {output}", MessageBoxType.OK);
                         return true; 
                    }
                    return false; 
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public partial class MatchPairViewModel : ObservableObject
    {
        private readonly VrcaSmartPatcherViewModel _parent;

        [ObservableProperty] private string _sourceName = "";
        [ObservableProperty] private string _targetName = "";
        [ObservableProperty] private string _status = "New"; 
        [ObservableProperty] private string _assetType = "Menu";
        [ObservableProperty] private string _sourceJsonContent = "";
        [ObservableProperty] private string _targetJsonContent = "";
        
        [ObservableProperty] private long _sourcePathId;
        [ObservableProperty] private long _targetPathId;

        public MatchPairViewModel(VrcaSmartPatcherViewModel parent)
        {
            _parent = parent;
        }

        public bool IsNewAsset => TargetPathId == 0;
        
        public IBrush StatusColor => Status switch
        {
            "Match" => new SolidColorBrush(Color.Parse("#4EC9B0")),
            "Modified" => new SolidColorBrush(Color.Parse("#DCDCAA")),
            "New" => new SolidColorBrush(Color.Parse("#569CD6")),
            "Conflict" => new SolidColorBrush(Color.Parse("#F44747")),
            _ => Brushes.Gray
        };

        [RelayCommand]
        private void ViewDifferences()
        {
            var vm = new VrcaComparisonViewModel(_parent._workspace, _parent.SourceFile!, SourcePathId, _parent.TargetFile!, TargetPathId);
            var win = new VrcaComparisonWindow(vm);
            
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
}
