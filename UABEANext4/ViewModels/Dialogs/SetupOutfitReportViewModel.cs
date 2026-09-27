using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UABEANext4.Interfaces;
using UABEANext4.Logic;
using UABEANext4.Logic.Hierarchy;
using UABEANext4.ViewModels.Tools;

namespace UABEANext4.ViewModels.Dialogs
{
    public partial class SetupOutfitReportViewModel : ObservableObject, IDialogAware
    {
        public string Title => "MA 服装设置诊断报告 (Setup Outfit Diagnostic Report)";
        public int Width => 1050;
        public int Height => 780;

        private readonly SetupOutfitReport _report;

        [ObservableProperty]
        private string _searchText = "";

        [ObservableProperty]
        private string _statusText = "";

        [ObservableProperty]
        private string _statusColor = "#4CAF50";

        [ObservableProperty]
        private string _summaryText = "";

        [ObservableProperty]
        private ObservableCollection<ReportSection> _sections = new();

        public SetupOutfitReportViewModel(SetupOutfitReport report)
        {
            _report = report;
            StatusText = report.Success ? "✅ 成功 (Success)" : "❌ 失败 (Failed)";
            StatusColor = report.Success ? "#4CAF50" : "#F44336";
            SummaryText = report.ResultMessage;
            BuildSections();
        }

        partial void OnSearchTextChanged(string value)
        {
            BuildSections();
        }

        private void BuildSections()
        {
            var sections = new List<ReportSection>();
            var filter = SearchText?.Trim() ?? "";

            // Section 1: FindBones
            var bonesItems = new List<ReportRow>
            {
                new("头像根节点 (Avatar Root)", _report.Bones.AvatarRootName, $"PathID: {_report.Bones.AvatarRootPathId}", _report.Bones.AvatarRootItem),
                new("头像骨架 (Avatar Armature)", _report.Bones.AvatarArmatureName, $"PathID: {_report.Bones.AvatarArmaturePathId}"),
                new("头像 Hips", _report.Bones.AvatarHipsName, $"PathID: {_report.Bones.AvatarHipsPathId}", _report.Bones.AvatarHipsItem),
                new("服装 Hips", _report.Bones.OutfitHipsName, $"PathID: {_report.Bones.OutfitHipsPathId}", _report.Bones.OutfitHipsItem),
                new("服装骨架 (Outfit Armature)", _report.Bones.OutfitArmatureName, $"PathID: {_report.Bones.OutfitArmaturePathId}"),
                new("头像 Animator", _report.Bones.HasAvatarAnimator ? "✅ 已找到" : "❌ 未找到", $"Humanoid 骨骼数: {_report.Bones.AvatarHumanoidBoneCount}"),
                new("服装 Animator", _report.Bones.HasOutfitAnimator ? "✅ 已找到" : "❌ 未找到", $"Humanoid 骨骼数: {_report.Bones.OutfitHumanoidBoneCount}"),
                new("推断前缀 (Prefix)", string.IsNullOrEmpty(_report.Prefix) ? "(无)" : $"\"{_report.Prefix}\"", ""),
                new("推断后缀 (Suffix)", string.IsNullOrEmpty(_report.Suffix) ? "(无)" : $"\"{_report.Suffix}\"", ""),
            };
            AddSection(sections, "🦴 骨骼发现 (FindBones)", bonesItems, filter);

            // Section 2: Bone Mappings
            var mappingItems = _report.BoneMappings.Select(m =>
                new ReportRow(
                    $"{"".PadLeft(m.Depth * 2, '·')}{m.OutfitBoneName}",
                    $"→ {m.AvatarBoneName}",
                    $"[{m.MatchMethod}] O:{m.OutfitPathId} A:{m.AvatarPathId}",
                    m.OutfitItem,
                    "✅"
                )).ToList();
            AddSection(sections, $"🔗 骨骼映射 (Bone Mappings) — {_report.BoneMappings.Count} 条", mappingItems, filter);

            // Section 3: Bone Renames
            var renameItems = _report.BoneRenames
                .Where(r => r.Changed)
                .Select(r => new ReportRow(
                    r.OldName,
                    $"→ {r.NewName}",
                    $"PathID: {r.PathId}",
                    r.Item,
                    "✏️"
                )).ToList();
            int unchangedCount = _report.BoneRenames.Count(r => !r.Changed);
            string renameSuffix = unchangedCount > 0 ? $" (未改动: {unchangedCount})" : "";
            AddSection(sections, $"✏️ 骨骼重命名 (Rename) — {renameItems.Count} 处改动{renameSuffix}", renameItems, filter);

            // Section 4: Reparents
            var reparentItems = _report.Reparents.Select(r =>
                new ReportRow(
                    $"{r.BoneName}",
                    $"{r.OldParentName} → {r.NewParentName}",
                    r.Success ? $"PathID: {r.BonePathId}" : $"❌ {r.FailReason}",
                    r.BoneItem,
                    r.Success ? "✅" : "❌"
                )).ToList();
            AddSection(sections, $"📎 重新挂载 (Reparent) — {_report.SuccessfulReparents}/{_report.TotalMappings} 成功", reparentItems, filter);

            // Section 5: Mesh Settings
            var meshItems = _report.MeshSyncs.Select(m =>
                new ReportRow(
                    m.GameObjectName,
                    $"RootBone: {m.RootBoneName}  Anchor: {m.ProbeAnchorName}",
                    $"Bounds C({m.BoundsCenter.X:F3},{m.BoundsCenter.Y:F3},{m.BoundsCenter.Z:F3}) E({m.BoundsExtent.X:F3},{m.BoundsExtent.Y:F3},{m.BoundsExtent.Z:F3})",
                    m.Item,
                    "🎨"
                )).ToList();
            AddSection(sections, $"🎨 网格同步 (Mesh Settings) — {meshItems.Count} 个 SMR", meshItems, filter);

            // Section 6: A-Pose Fix
            var aposeItems = _report.APoseFixes.Select(a =>
            {
                string detail;
                if (a.Applied)
                    detail = $"旋转修正: ({a.OldRotation.X:F4},{a.OldRotation.Y:F4},{a.OldRotation.Z:F4},{a.OldRotation.W:F4}) → ({a.NewRotation.X:F4},{a.NewRotation.Y:F4},{a.NewRotation.Z:F4},{a.NewRotation.W:F4})";
                else
                    detail = a.SkipReason;
                return new ReportRow(
                    $"{a.BoneName}",
                    $"Avatar: {a.AvatarBoneName} / Outfit: {a.OutfitBoneName}",
                    detail,
                    a.OutfitItem,
                    a.Applied ? "✅" : "⏭️"
                );
            }).ToList();
            AddSection(sections, $"💪 A-Pose 修复 — {_report.APoseFixes.Count(a => a.Applied)} 处应用", aposeItems, filter);

            // Section 7: Unmatched Bones
            var unmatchedItems = new List<ReportRow>();
            foreach (var u in _report.UnmatchedOutfitBones)
                unmatchedItems.Add(new ReportRow($"[服装] {u.BoneName}", $"PathID: {u.PathId}", "未匹配 (Unmatched)", u.Item, "⚠️"));
            foreach (var u in _report.UnmatchedAvatarBones)
                unmatchedItems.Add(new ReportRow($"[头像] {u.BoneName}", $"PathID: {u.PathId}", "未匹配 (Unmatched)", u.Item, "⚠️"));
            AddSection(sections, $"⚠️ 未匹配骨骼 — 服装:{_report.UnmatchedOutfitBones.Count} / 头像:{_report.UnmatchedAvatarBones.Count}", unmatchedItems, filter);

            // Section 7.5: Added Bones (unmatched bones copied to avatar)
            var addedItems = _report.AddedBones.Select(a =>
                new ReportRow(
                    a.BoneName,
                    $"→ {a.AvatarParentName}",
                    a.Success ? $"子骨骼数: {a.ChildCount}  PathID: {a.BonePathId}" : $"❌ {a.FailReason}",
                    a.BoneItem,
                    a.Success ? "📦" : "❌"
                )).ToList();
            if (addedItems.Count > 0)
                AddSection(sections, $"📦 新增骨骼 (Added Bones) — {_report.AddedBones.Count(a => a.Success)}/{_report.AddedBones.Count} 成功", addedItems, filter);

            // Section 7.8: Merged Bones (matched bones with references redirected)
            var mergedItems = _report.MergedBones.Select(m =>
                new ReportRow(
                    $"{m.OutfitBoneName} → {m.AvatarBoneName}",
                    m.Deleted ? "已删除 (Deleted)" : "保留 (Kept)",
                    $"Outfit PID: {m.OutfitTransformPathId} → Avatar PID: {m.AvatarTransformPathId}",
                    m.AvatarItem,
                    m.Deleted ? "🔗" : "⚠️"
                )).ToList();
            if (mergedItems.Count > 0)
                AddSection(sections, $"🔗 合并骨骼 (Merged Bones) — {_report.SuccessfulMerges} 个合并, {_report.DeletedAssetsCount} 资产已删除", mergedItems, filter);

            // Section 8: Warnings & Armature Rename
            var warnItems = new List<ReportRow>();
            foreach (var w in _report.Warnings)
                warnItems.Add(new ReportRow("⚠️ 警告", w, "", statusIcon: "⚠️"));
            if (_report.ArmatureRename != null)
                warnItems.Add(new ReportRow("骨架重命名 (Armature Rename)", $"{_report.ArmatureRename.OldName} → {_report.ArmatureRename.NewName}", $"PathID: {_report.ArmatureRename.PathId}", statusIcon: "✏️"));
            if (warnItems.Count > 0)
                AddSection(sections, $"📢 警告与备注 — {warnItems.Count} 条", warnItems, filter);

            Sections = new ObservableCollection<ReportSection>(sections);
        }

        private void AddSection(List<ReportSection> sections, string header, List<ReportRow> items, string filter)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                items = items.Where(r =>
                    r.Label.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    r.Value.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                    r.Detail.Contains(filter, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }
            sections.Add(new ReportSection(header, items));
        }

        [RelayCommand]
        public void NavigateToItem(ReportRow? row)
        {
            if (row?.NavigationTarget == null) return;
            if (row.NavigationTarget.Asset != null)
            {
                var asset = row.NavigationTarget.Asset;
                
                // 触发 Hierarchy 树展开和定位 (Trigger Hierarchy tree expand & select)
                WeakReferenceMessenger.Default.Send(new RequestSceneViewMessage(asset));
                
                // 触发资源和属性面板刷新为选中状态 (Trigger Inspector/Asset view update)
                WeakReferenceMessenger.Default.Send(new AssetsSelectedMessage(new List<AssetWorkspace.AssetInst> { asset }));
                WeakReferenceMessenger.Default.Send(new RequestVisitAssetMessage(asset));
            }
        }
    }

    public class ReportSection
    {
        public string Header { get; }
        public List<ReportRow> Rows { get; }
        public string DisplayHeader => $"{Header}";
        public bool HasRows => Rows.Count > 0;

        public ReportSection(string header, List<ReportRow> rows)
        {
            Header = header;
            Rows = rows;
        }
    }

    public class ReportRow
    {
        public string Label { get; }
        public string Value { get; }
        public string Detail { get; }
        public HierarchyItem? NavigationTarget { get; }
        public string StatusIcon { get; }
        public bool CanNavigate => NavigationTarget != null;

        public ReportRow(string label, string value, string detail, HierarchyItem? navigationTarget = null, string statusIcon = "ℹ️")
        {
            Label = label;
            Value = value;
            Detail = detail;
            NavigationTarget = navigationTarget;
            StatusIcon = statusIcon;
        }
    }
}
