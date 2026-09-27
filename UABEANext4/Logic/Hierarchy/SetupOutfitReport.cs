using System.Collections.Generic;
using System.Numerics;

namespace UABEANext4.Logic.Hierarchy;

public class SetupOutfitReport
{
    public bool Success { get; set; }
    public string ResultMessage { get; set; } = "";
    public string OutfitName { get; set; } = "";
    public string Prefix { get; set; } = "";
    public string Suffix { get; set; } = "";
    public int TotalMappings { get; set; }
    public int SuccessfulReparents { get; set; }
    public int SuccessfulMerges { get; set; }
    public int DeletedAssetsCount { get; set; }

    public FindBonesResult Bones { get; set; } = new();
    public List<BoneMappingEntry> BoneMappings { get; set; } = new();
    public List<BoneRenameEntry> BoneRenames { get; set; } = new();
    public List<ReparentEntry> Reparents { get; set; } = new();
    public List<MeshSyncEntry> MeshSyncs { get; set; } = new();
    public List<APoseFixEntry> APoseFixes { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<UnmatchedBoneEntry> UnmatchedOutfitBones { get; set; } = new();
    public List<UnmatchedBoneEntry> UnmatchedAvatarBones { get; set; } = new();
    public List<AddedBoneEntry> AddedBones { get; set; } = new();
    public List<MergedBoneEntry> MergedBones { get; set; } = new();
    public ArmatureRenameInfo? ArmatureRename { get; set; }
}

public class FindBonesResult
{
    public string AvatarRootName { get; set; } = "";
    public long AvatarRootPathId { get; set; }
    public string AvatarHipsName { get; set; } = "";
    public long AvatarHipsPathId { get; set; }
    public string OutfitHipsName { get; set; } = "";
    public long OutfitHipsPathId { get; set; }
    public string AvatarArmatureName { get; set; } = "";
    public long AvatarArmaturePathId { get; set; }
    public string OutfitArmatureName { get; set; } = "";
    public long OutfitArmaturePathId { get; set; }
    public bool HasAvatarAnimator { get; set; }
    public bool HasOutfitAnimator { get; set; }
    public int AvatarHumanoidBoneCount { get; set; }
    public int OutfitHumanoidBoneCount { get; set; }
    public HierarchyItem? AvatarRootItem { get; set; }
    public HierarchyItem? AvatarHipsItem { get; set; }
    public HierarchyItem? OutfitHipsItem { get; set; }
}

public class BoneMappingEntry
{
    public string OutfitBoneName { get; set; } = "";
    public string AvatarBoneName { get; set; } = "";
    public string MatchMethod { get; set; } = "";
    public long OutfitPathId { get; set; }
    public long AvatarPathId { get; set; }
    public int Depth { get; set; }
    public HierarchyItem? OutfitItem { get; set; }
    public HierarchyItem? AvatarItem { get; set; }
}

public class BoneRenameEntry
{
    public string OldName { get; set; } = "";
    public string NewName { get; set; } = "";
    public long PathId { get; set; }
    public bool Changed { get; set; }
    public HierarchyItem? Item { get; set; }
}

public class ReparentEntry
{
    public string BoneName { get; set; } = "";
    public string OldParentName { get; set; } = "";
    public string NewParentName { get; set; } = "";
    public long BonePathId { get; set; }
    public long NewParentPathId { get; set; }
    public bool Success { get; set; }
    public string FailReason { get; set; } = "";
    public HierarchyItem? BoneItem { get; set; }
    public HierarchyItem? NewParentItem { get; set; }
}

public class MeshSyncEntry
{
    public string GameObjectName { get; set; } = "";
    public long GameObjectPathId { get; set; }
    public long RootBonePathId { get; set; }
    public string RootBoneName { get; set; } = "";
    public long ProbeAnchorPathId { get; set; }
    public string ProbeAnchorName { get; set; } = "";
    public Vector3 BoundsCenter { get; set; }
    public Vector3 BoundsExtent { get; set; }
    public HierarchyItem? Item { get; set; }
}

public class APoseFixEntry
{
    public string BoneName { get; set; } = "";
    public string AvatarBoneName { get; set; } = "";
    public string OutfitBoneName { get; set; } = "";
    public bool Applied { get; set; }
    public string SkipReason { get; set; } = "";
    public Quaternion OldRotation { get; set; }
    public Quaternion NewRotation { get; set; }
    public HierarchyItem? OutfitItem { get; set; }
}

public class UnmatchedBoneEntry
{
    public string BoneName { get; set; } = "";
    public long PathId { get; set; }
    public HierarchyItem? Item { get; set; }
}

public class ArmatureRenameInfo
{
    public string OldName { get; set; } = "";
    public string NewName { get; set; } = "";
    public long PathId { get; set; }
}

public class AddedBoneEntry
{
    public string BoneName { get; set; } = "";
    public long BonePathId { get; set; }
    public string AvatarParentName { get; set; } = "";
    public long AvatarParentPathId { get; set; }
    public bool Success { get; set; }
    public string FailReason { get; set; } = "";
    public int ChildCount { get; set; }
    public HierarchyItem? BoneItem { get; set; }
    public HierarchyItem? AvatarParentItem { get; set; }
}

public class MergedBoneEntry
{
    public string OutfitBoneName { get; set; } = "";
    public long OutfitTransformPathId { get; set; }
    public string AvatarBoneName { get; set; } = "";
    public long AvatarTransformPathId { get; set; }
    public bool Deleted { get; set; }
    public int RedirectedRefCount { get; set; }
    public HierarchyItem? OutfitItem { get; set; }
    public HierarchyItem? AvatarItem { get; set; }
}
