using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UABEANext4.AssetWorkspace;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Avalonia.Threading;
using System.Text;
using System.IO;
using UABEANext4.ViewModels;

namespace UABEANext4.ViewModels.Tools
{
    /// <summary>
    /// 变形修复工具视图模型。
    /// 用于通过层级结构和名称匹配两个 VRCA 模型，并同步旋转和网格数据。
    /// </summary>
    public partial class DeformationRepairViewModel : ViewModelBase
    {
        public Workspace Workspace { get; }

        [ObservableProperty] private ObservableCollection<IDataSourceItem> _availableFiles = new();
        [ObservableProperty] private IDataSourceItem? _sourceFile;
        [ObservableProperty] private IDataSourceItem? _targetFile;

        [ObservableProperty] private ObservableCollection<DeformationMatchNode> _matchRoots = new();
        [ObservableProperty] private ObservableCollection<DeformationMatchNode> _differenceList = new();
        [ObservableProperty] private bool _isWorking;
        [ObservableProperty] private string _progressText = "等待开始...";
        [ObservableProperty] private double _progressValue;
        [ObservableProperty] private string _logText = string.Empty;

        public DeformationRepairViewModel(Workspace workspace)
        {
            Workspace = workspace;
            LoadFiles();
        }

        private void LoadFiles()
        {
            AvailableFiles.Clear();
            var assetsFiles = WorkspaceItem.GetAssetsFileWorkspaceItems(Workspace.RootItems);
            foreach (var item in assetsFiles)
            {
                if (item.Object is AssetsFileInstance afi)
                {
                    AvailableFiles.Add(new FileSourceItem { Name = afi.name, Instance = afi });
                }
            }
        }

        /// <summary>
        /// 开始执行层级结构匹配逻辑。
        /// </summary>
        [RelayCommand]
        private async Task StartMatching()
        {
            if (SourceFile == null || TargetFile == null)
            {
                AppendLog("错误: 请先选择源文件和目标文件。");
                return;
            }

            IsWorking = true;
            ProgressText = "正在匹配层级结构...";
            MatchRoots.Clear();
            DifferenceList.Clear();
            LogText = string.Empty;

            await Task.Run(() =>
            {
                // 分别构建源文件和目标文件的层级树
                var sourceNodes = BuildHierarchy(SourceFile.Instance);
                var targetNodes = BuildHierarchy(TargetFile.Instance);

                // 执行递归匹配逻辑
                var roots = MatchHierarchies(sourceNodes, targetNodes, "");
                
                var diffs = new List<DeformationMatchNode>();
                CollectDifferences(roots, diffs);

                Dispatcher.UIThread.Post(() =>
                {
                    foreach (var root in roots)
                    {
                        MatchRoots.Add(root);
                    }
                    foreach (var diff in diffs)
                    {
                        DifferenceList.Add(diff);
                    }
                    AppendLog($"层级匹配完成。共发现 {CountNodes(roots)} 个匹配对象，其中 {diffs.Count} 个存在差异。");
                });
            });

            IsWorking = false;
            ProgressText = "匹配完成。";
        }

        [RelayCommand]
        private void SelectAll() => SetAllChecked(MatchRoots, true);

        [RelayCommand]
        private void DeselectAll() => SetAllChecked(MatchRoots, false);

        /// <summary>
        /// 执行数据修复逻辑。
        /// 仅处理被选中的节点。
        /// </summary>
        [RelayCommand]
        private async Task ExecuteRepair()
        {
            // 收集所有已勾选的节点
            var selectedNodes = new List<DeformationMatchNode>();
            CollectSelectedNodes(MatchRoots, selectedNodes);

            if (selectedNodes.Count == 0)
            {
                AppendLog("错误: 未勾选任何修复项。");
                return;
            }

            IsWorking = true;
            ProgressValue = 0;
            int count = 0;
            int total = selectedNodes.Count;

            await Task.Run(() =>
            {
                var dirtyFiles = new HashSet<AssetsFileInstance>();

                foreach (var node in selectedNodes)
                {
                    if (node.SourceAsset == null || node.TargetAsset == null) continue;

                    try
                    {
                        bool repaired = false;
                        if (node.Type == "[Transform]")
                        {
                            // 修复 Transform 旋转
                            repaired = RepairTransform(node.SourceAsset, node.TargetAsset);
                        }
                        else if (node.Type == "[Mesh]")
                        {
                            // 修复 Mesh 网格数据
                            repaired = RepairMesh(node.SourceAsset, node.TargetAsset);
                        }

                        if (repaired)
                        {
                            dirtyFiles.Add(node.TargetAsset.FileInstance);
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"[错误] 修复 {node.Path} 失败: {ex.Message}");
                    }

                    count++;
                    var progress = (double)count / total;
                    Dispatcher.UIThread.Post(() =>
                    {
                        ProgressValue = progress;
                        ProgressText = $"正在执行修复... ({count}/{total})";
                    });
                }

                // 将有变动的文件标记为已脏 (Dirty)，以便后续保存
                foreach (var file in dirtyFiles)
                {
                    var wsItem = Workspace.FindWorkspaceItemByInstance(file);
                    if (wsItem != null)
                    {
                        Dispatcher.UIThread.Post(() => Workspace.Dirty(wsItem));
                    }
                }

                AppendLog($"修复完成。共处理 {count} 个项。已标记 {dirtyFiles.Count} 个文件为已修改。");
            });

            IsWorking = false;
            ProgressText = "修复执行完毕。";
        }

        private bool RepairTransform(AssetInst source, AssetInst target)
        {
            var sourceBf = Workspace.GetBaseField(source);
            var targetBf = Workspace.GetBaseField(target);

            if (sourceBf == null || targetBf == null) return false;

            var sourceRotation = sourceBf["m_LocalRotation"];
            var targetRotation = targetBf["m_LocalRotation"];

            targetRotation["x"].AsFloat = sourceRotation["x"].AsFloat;
            targetRotation["y"].AsFloat = sourceRotation["y"].AsFloat;
            targetRotation["z"].AsFloat = sourceRotation["z"].AsFloat;
            targetRotation["w"].AsFloat = sourceRotation["w"].AsFloat;

            target.UpdateAssetDataAndRow(Workspace, targetBf);
            return true;
        }

        private bool RepairMesh(AssetInst source, AssetInst target)
        {
            // Direct raw data copy as requested
            byte[]? sourceData;
            lock (source.FileInstance.LockReader)
            {
                source.FileReader.Position = source.AbsoluteByteStart;
                sourceData = source.FileReader.ReadBytes((int)source.ByteSize);
            }

            if (sourceData == null || sourceData.Length == 0) return false;

            target.UpdateAssetDataAndRow(Workspace, sourceData);
            return true;
        }

        private void AppendLog(string text)
        {
            Dispatcher.UIThread.Post(() => LogText += $"[{DateTime.Now:HH:mm:ss}] {text}\n");
        }

        private Dictionary<long, HierarchyInfo> BuildHierarchy(AssetsFileInstance file)
        {
            var transformInfos = new Dictionary<long, AssetFileInfo>();
            foreach (var info in file.file.Metadata.AssetInfos)
            {
                if (info.TypeId == (int)AssetClassID.Transform || info.TypeId == (int)AssetClassID.RectTransform)
                    transformInfos[info.PathId] = info;
            }

            var idToNode = new Dictionary<long, HierarchyInfo>();
            
            foreach (var kvp in transformInfos)
            {
                try
                {
                    var tfmBf = Workspace.Manager.GetBaseField(file, kvp.Value);
                    if (tfmBf == null) continue;

                    long goPathId = tfmBf["m_GameObject"]["m_PathID"].AsLong;
                    string name = "(unknown)";
                    AssetInst? goInst = null;
                    if (goPathId != 0)
                    {
                        goInst = Workspace.GetAssetInst(file, 0, goPathId);
                        if (goInst != null)
                        {
                            var goBf = Workspace.GetBaseField(goInst);
                            name = goBf?["m_Name"].AsString ?? name;
                        }
                    }

                    long fatherId = tfmBf["m_Father"]["m_PathID"].AsLong;
                    
                    // Also find SkinnedMeshRenderer if it exists on the same GameObject
                    AssetInst? meshInst = null;
                    if (goInst != null)
                    {
                        var goBf = Workspace.GetBaseField(goInst);
                        var comps = goBf?["m_Component.Array"];
                        if (comps != null)
                        {
                            foreach (var cRef in comps.Children)
                            {
                                var cPtr = cRef["component"];
                                var cInst = Workspace.GetAssetInst(file, cPtr);
                                if (cInst != null && (cInst.TypeId == (int)AssetClassID.SkinnedMeshRenderer || cInst.TypeId == (int)AssetClassID.MeshFilter))
                                {
                                    var compBf = Workspace.GetBaseField(cInst);
                                    var mPtr = compBf?["m_Mesh"];
                                    if (mPtr != null && !mPtr.IsDummy)
                                    {
                                        meshInst = Workspace.GetAssetInst(file, mPtr);
                                    }
                                }
                            }
                        }
                    }

                    idToNode[kvp.Key] = new HierarchyInfo 
                    { 
                        Name = name, 
                        PathId = kvp.Key, 
                        ParentId = fatherId,
                        TransformAsset = Workspace.GetAssetInst(file, 0, kvp.Key),
                        MeshAsset = meshInst
                    };
                }
                catch { }
            }

            return idToNode;
        }

        private List<DeformationMatchNode> MatchHierarchies(Dictionary<long, HierarchyInfo> source, Dictionary<long, HierarchyInfo> target, string parentPath)
        {
            var sourceRoots = source.Values.Where(n => n.ParentId == 0).ToList();
            var targetRoots = target.Values.Where(n => n.ParentId == 0).ToList();

            AppendLog($"源文件根节点数量: {sourceRoots.Count}, 目标文件根节点数量: {targetRoots.Count}");

            var matchedRoots = new List<DeformationMatchNode>();

            // 修复：不再尝试匹配根节点（如 CAB-xxx），而是直接匹配它们底下的子节点
            foreach (var sRoot in sourceRoots)
            {
                var sChildren = source.Values.Where(n => n.ParentId == sRoot.PathId).ToList();
                var tChildren = target.Values.SelectMany(tr => target.Values.Where(n => n.ParentId == tr.PathId)).ToList(); 
                // 注意：上面 targetRoots 可能有多个（虽然通常只有一个 CAB），我们尝试在所有根的子节点中寻找匹配
                
                var tAllTopChildren = new List<HierarchyInfo>();
                foreach (var tr in targetRoots)
                {
                    tAllTopChildren.AddRange(target.Values.Where(n => n.ParentId == tr.PathId));
                }

                foreach (var sc in sChildren)
                {
                    var tc = tAllTopChildren.FirstOrDefault(t => t.Name == sc.Name);
                    if (tc != null)
                    {
                        var node = MatchNode(sc, tc, source, target, "");
                        matchedRoots.Add(node);
                    }
                }
            }

            return matchedRoots;
        }

        private DeformationMatchNode MatchNode(HierarchyInfo s, HierarchyInfo t, Dictionary<long, HierarchyInfo> source, Dictionary<long, HierarchyInfo> target, string parentPath)
        {
            string currentPath = string.IsNullOrEmpty(parentPath) ? s.Name : $"{parentPath}/{s.Name}";
            
            var node = new DeformationMatchNode
            {
                Name = s.Name,
                Type = "[Transform]",
                Path = currentPath,
                SourceAsset = s.TransformAsset,
                TargetAsset = t.TransformAsset,
                DiffOverview = GetTransformDiff(s.TransformAsset, t.TransformAsset)
            };

            // Mesh Match
            if (s.MeshAsset != null && t.MeshAsset != null)
            {
                var meshDiff = GetMeshDiff(s.MeshAsset, t.MeshAsset);
                var meshNode = new DeformationMatchNode
                {
                    Name = s.MeshAsset.DisplayName,
                    Type = "[Mesh]",
                    Path = currentPath + "/[Mesh]",
                    SourceAsset = s.MeshAsset,
                    TargetAsset = t.MeshAsset,
                    DiffOverview = meshDiff,
                    HasDifference = !meshDiff.Contains("一致") && !meshDiff.Contains("N/A")
                };
                node.Children.Add(meshNode);
            }

            // Transform logic (checking if rotation diff makes it "different")
            var transDiff = GetTransformDiff(s.TransformAsset, t.TransformAsset);
            node.DiffOverview = transDiff;
            node.HasDifference = !transDiff.Contains("无差异") && !transDiff.Contains("N/A");

            // Children match
            var sChildren = source.Values.Where(n => n.ParentId == s.PathId).ToList();
            var tChildren = target.Values.Where(n => n.ParentId == t.PathId).ToList();

            foreach (var sc in sChildren)
            {
                var tc = tChildren.FirstOrDefault(tc => tc.Name == sc.Name);
                if (tc != null)
                {
                    node.Children.Add(MatchNode(sc, tc, source, target, currentPath));
                }
            }

            return node;
        }

        private string GetTransformDiff(AssetInst? s, AssetInst? t)
        {
            if (s == null || t == null) return "N/A";
            try
            {
                var sBf = Workspace.GetBaseField(s);
                var tBf = Workspace.GetBaseField(t);
                if (sBf != null && tBf != null)
                {
                    var sRot = sBf["m_LocalRotation"];
                    var tRot = tBf["m_LocalRotation"];
                    
                    float sx = sRot["x"].AsFloat, sy = sRot["y"].AsFloat, sz = sRot["z"].AsFloat, sw = sRot["w"].AsFloat;
                    float tx = tRot["x"].AsFloat, ty = tRot["y"].AsFloat, tz = tRot["z"].AsFloat, tw = tRot["w"].AsFloat;

                    float dx = Math.Abs(sx - tx);
                    float dy = Math.Abs(sy - ty);
                    float dz = Math.Abs(sz - tz);
                    float dw = Math.Abs(sw - tw);

                    string sVal = $"({sx:F3}, {sy:F3}, {sz:F3}, {sw:F3})";
                    string tVal = $"({tx:F3}, {ty:F3}, {tz:F3}, {tw:F3})";

                    if (dx + dy + dz + dw < 0.0001f) return $"无差异: {sVal}";
                    return $"A {sVal} vs B {tVal}";
                }
            } catch { }
            return "Unknown";
        }

        private string GetMeshDiff(AssetInst? s, AssetInst? t)
        {
            if (s == null || t == null) return "N/A";
            
            byte[]? sData = null;
            byte[]? tData = null;

            try
            {
                lock (s.FileInstance.LockReader)
                {
                    s.FileReader.Position = s.AbsoluteByteStart;
                    sData = s.FileReader.ReadBytes((int)s.ByteSize);
                }
                lock (t.FileInstance.LockReader)
                {
                    t.FileReader.Position = t.AbsoluteByteStart;
                    tData = t.FileReader.ReadBytes((int)t.ByteSize);
                }

                if (sData != null && tData != null)
                {
                    if (sData.Length != tData.Length)
                        return $"大小差异: {sData.Length} vs {tData.Length} bytes";

                    bool identical = true;
                    for (int i = 0; i < sData.Length; i++)
                    {
                        if (sData[i] != tData[i])
                        {
                            identical = false;
                            break;
                        }
                    }

                    if (identical) return $"内容一致 ({sData.Length} bytes)";
                    return "大小一致但内容不同 (Data mismatch)";
                }
            }
            catch { }

            return "Unknown";
        }

        private int CountNodes(IEnumerable<DeformationMatchNode> nodes)
        {
            int count = nodes.Count();
            foreach (var node in nodes) count += CountNodes(node.Children);
            return count;
        }

        private void SetAllChecked(IEnumerable<DeformationMatchNode> nodes, bool value)
        {
            foreach (var node in nodes)
            {
                node.IsChecked = value;
                SetAllChecked(node.Children, value);
            }
        }

        private void CollectSelectedNodes(IEnumerable<DeformationMatchNode> nodes, List<DeformationMatchNode> result)
        {
            foreach (var node in nodes)
            {
                if (node.IsChecked) result.Add(node);
                CollectSelectedNodes(node.Children, result);
            }
        }

        private void CollectDifferences(IEnumerable<DeformationMatchNode> nodes, List<DeformationMatchNode> result)
        {
            foreach (var node in nodes)
            {
                if (node.HasDifference) result.Add(node);
                CollectDifferences(node.Children, result);
            }
        }

        private class HierarchyInfo
        {
            public string Name { get; set; } = string.Empty;
            public long PathId { get; set; }
            public long ParentId { get; set; }
            public AssetInst? TransformAsset { get; set; }
            public AssetInst? MeshAsset { get; set; }
        }
    }

    public partial class DeformationMatchNode : ObservableObject
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public AssetInst? SourceAsset { get; set; }
        public AssetInst? TargetAsset { get; set; }
        [ObservableProperty] private bool _isChecked = true;
        [ObservableProperty] private string _diffOverview = string.Empty;
        [ObservableProperty] private bool _hasDifference;
        public ObservableCollection<DeformationMatchNode> Children { get; } = new();

        public string DisplayPath => string.IsNullOrEmpty(Path) ? Name : Path;
    }

    public interface IDataSourceItem
    {
        string Name { get; }
        AssetsFileInstance Instance { get; }
    }

    public class FileSourceItem : IDataSourceItem
    {
        public string Name { get; set; } = string.Empty;
        public required AssetsFileInstance Instance { get; init; }
        public override string ToString() => Name;
    }
}
