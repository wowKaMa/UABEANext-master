using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace UABEANext4.Logic.Hierarchy;

/// <summary>
/// Bone name matching logic ported from Modular Avatar's HeuristicBoneMapper.
/// Used for Setup Outfit to match outfit bone names to avatar bone names.
/// </summary>
public static class BoneNameMatcher
{
    // Ported from MA HeuristicBoneMapper.cs (MIT Licensed)
    // Original: https://github.com/HhotateA/AvatarModifyTools
    // Additional patterns from: https://github.com/Azukimochi/BoneRenamer
    public static readonly string[][] BoneNamePatterns = new[]
    {
        new[] { "Hips", "Hip", "pelvis" },
        new[] { "LeftUpperLeg", "UpperLeg_Left", "UpperLeg_L", "Leg_Left", "Leg_L", "ULeg_L", "Left leg", "LeftUpLeg", "UpLeg.L", "Thigh_L" },
        new[] { "RightUpperLeg", "UpperLeg_Right", "UpperLeg_R", "Leg_Right", "Leg_R", "ULeg_R", "Right leg", "RightUpLeg", "UpLeg.R", "Thigh_R" },
        new[] { "LeftLowerLeg", "LowerLeg_Left", "LowerLeg_L", "Knee_Left", "Knee_L", "LLeg_L", "Left knee", "LeftLeg", "leg_L", "shin.L" },
        new[] { "RightLowerLeg", "LowerLeg_Right", "LowerLeg_R", "Knee_Right", "Knee_R", "LLeg_R", "Right knee", "RightLeg", "leg_R", "shin.R" },
        new[] { "LeftFoot", "Foot_Left", "Foot_L", "Ankle_L", "Foot.L.001", "Left ankle", "heel.L", "heel" },
        new[] { "RightFoot", "Foot_Right", "Foot_R", "Ankle_R", "Foot.R.001", "Right ankle", "heel.R", "heel" },
        new[] { "Spine", "spine01" },
        new[] { "Chest", "Bust", "spine02", "upper_chest" },
        new[] { "Neck" },
        new[] { "Head" },
        new[] { "LeftShoulder", "Shoulder_Left", "Shoulder_L" },
        new[] { "RightShoulder", "Shoulder_Right", "Shoulder_R" },
        new[] { "LeftUpperArm", "UpperArm_Left", "UpperArm_L", "Arm_Left", "Arm_L", "UArm_L", "Left arm", "UpperLeftArm" },
        new[] { "RightUpperArm", "UpperArm_Right", "UpperArm_R", "Arm_Right", "Arm_R", "UArm_R", "Right arm", "UpperRightArm" },
        new[] { "LeftLowerArm", "LowerArm_Left", "LowerArm_L", "LArm_L", "Left elbow", "LeftForeArm", "Elbow_L", "forearm_L", "ForArm_L" },
        new[] { "RightLowerArm", "LowerArm_Right", "LowerArm_R", "LArm_R", "Right elbow", "RightForeArm", "Elbow_R", "forearm_R", "ForArm_R" },
        new[] { "LeftHand", "Hand_Left", "Hand_L", "Left wrist", "Wrist_L" },
        new[] { "RightHand", "Hand_Right", "Hand_R", "Right wrist", "Wrist_R" },
        new[] { "LeftToes", "Toes_Left", "Toe_Left", "ToeIK_L", "Toes_L", "Toe_L", "Foot.L.002", "Left Toe", "LeftToeBase" },
        new[] { "RightToes", "Toes_Right", "Toe_Right", "ToeIK_R", "Toes_R", "Toe_R", "Foot.R.002", "Right Toe", "RightToeBase" },
        new[] { "LeftEye", "Eye_Left", "Eye_L" },
        new[] { "RightEye", "Eye_Right", "Eye_R" },
        new[] { "Jaw" },
        new[] { "LeftThumbProximal", "ProximalThumb_Left", "ProximalThumb_L", "Thumb1_L", "ThumbFinger1_L", "LeftHandThumb1", "Thumb Proximal.L", "Thunb1_L", "finger01_01_L" },
        new[] { "LeftThumbIntermediate", "IntermediateThumb_Left", "IntermediateThumb_L", "Thumb2_L", "ThumbFinger2_L", "LeftHandThumb2", "Thumb Intermediate.L", "Thunb2_L", "finger01_02_L" },
        new[] { "LeftThumbDistal", "DistalThumb_Left", "DistalThumb_L", "Thumb3_L", "ThumbFinger3_L", "LeftHandThumb3", "Thumb Distal.L", "Thunb3_L", "finger01_03_L" },
        new[] { "LeftIndexProximal", "ProximalIndex_Left", "ProximalIndex_L", "Index1_L", "IndexFinger1_L", "LeftHandIndex1", "Index Proximal.L", "finger02_01_L", "f_index.01.L" },
        new[] { "LeftIndexIntermediate", "IntermediateIndex_Left", "IntermediateIndex_L", "Index2_L", "IndexFinger2_L", "LeftHandIndex2", "Index Intermediate.L", "finger02_02_L", "f_index.02.L" },
        new[] { "LeftIndexDistal", "DistalIndex_Left", "DistalIndex_L", "Index3_L", "IndexFinger3_L", "LeftHandIndex3", "Index Distal.L", "finger02_03_L", "f_index.03.L" },
        new[] { "LeftMiddleProximal", "ProximalMiddle_Left", "ProximalMiddle_L", "Middle1_L", "MiddleFinger1_L", "LeftHandMiddle1", "Middle Proximal.L", "finger03_01_L", "f_middle.01.L" },
        new[] { "LeftMiddleIntermediate", "IntermediateMiddle_Left", "IntermediateMiddle_L", "Middle2_L", "MiddleFinger2_L", "LeftHandMiddle2", "Middle Intermediate.L", "finger03_02_L", "f_middle.02.L" },
        new[] { "LeftMiddleDistal", "DistalMiddle_Left", "DistalMiddle_L", "Middle3_L", "MiddleFinger3_L", "LeftHandMiddle3", "Middle Distal.L", "finger03_03_L", "f_middle.03.L" },
        new[] { "LeftRingProximal", "ProximalRing_Left", "ProximalRing_L", "Ring1_L", "RingFinger1_L", "LeftHandRing1", "Ring Proximal.L", "finger04_01_L", "f_ring.01.L" },
        new[] { "LeftRingIntermediate", "IntermediateRing_Left", "IntermediateRing_L", "Ring2_L", "RingFinger2_L", "LeftHandRing2", "Ring Intermediate.L", "finger04_02_L", "f_ring.02.L" },
        new[] { "LeftRingDistal", "DistalRing_Left", "DistalRing_L", "Ring3_L", "RingFinger3_L", "LeftHandRing3", "Ring Distal.L", "finger04_03_L", "f_ring.03.L" },
        new[] { "LeftLittleProximal", "ProximalLittle_Left", "ProximalLittle_L", "Little1_L", "LittleFinger1_L", "LeftHandPinky1", "Little Proximal.L", "finger05_01_L", "f_pinky.01.L", "Pinky1.L" },
        new[] { "LeftLittleIntermediate", "IntermediateLittle_Left", "IntermediateLittle_L", "Little2_L", "LittleFinger2_L", "LeftHandPinky2", "Little Intermediate.L", "finger05_02_L", "f_pinky.02.L", "Pinky2.L" },
        new[] { "LeftLittleDistal", "DistalLittle_Left", "DistalLittle_L", "Little3_L", "LittleFinger3_L", "LeftHandPinky3", "Little Distal.L", "finger05_03_L", "f_pinky.03.L", "Pinky3.L" },
        new[] { "RightThumbProximal", "ProximalThumb_Right", "ProximalThumb_R", "Thumb1_R", "ThumbFinger1_R", "RightHandThumb1", "Thumb Proximal.R", "Thunb1_R", "finger01_01_R" },
        new[] { "RightThumbIntermediate", "IntermediateThumb_Right", "IntermediateThumb_R", "Thumb2_R", "ThumbFinger2_R", "RightHandThumb2", "Thumb Intermediate.R", "Thunb2_R", "finger01_02_R" },
        new[] { "RightThumbDistal", "DistalThumb_Right", "DistalThumb_R", "Thumb3_R", "ThumbFinger3_R", "RightHandThumb3", "Thumb Distal.R", "Thunb3_R", "finger01_03_R" },
        new[] { "RightIndexProximal", "ProximalIndex_Right", "ProximalIndex_R", "Index1_R", "IndexFinger1_R", "RightHandIndex1", "Index Proximal.R", "finger02_01_R", "f_index.01.R" },
        new[] { "RightIndexIntermediate", "IntermediateIndex_Right", "IntermediateIndex_R", "Index2_R", "IndexFinger2_R", "RightHandIndex2", "Index Intermediate.R", "finger02_02_R", "f_index.02.R" },
        new[] { "RightIndexDistal", "DistalIndex_Right", "DistalIndex_R", "Index3_R", "IndexFinger3_R", "RightHandIndex3", "Index Distal.R", "finger02_03_R", "f_index.03.R" },
        new[] { "RightMiddleProximal", "ProximalMiddle_Right", "ProximalMiddle_R", "Middle1_R", "MiddleFinger1_R", "RightHandMiddle1", "Middle Proximal.R", "finger03_01_R", "f_middle.01.R" },
        new[] { "RightMiddleIntermediate", "IntermediateMiddle_Right", "IntermediateMiddle_R", "Middle2_R", "MiddleFinger2_R", "RightHandMiddle2", "Middle Intermediate.R", "finger03_02_R", "f_middle.02.R" },
        new[] { "RightMiddleDistal", "DistalMiddle_Right", "DistalMiddle_R", "Middle3_R", "MiddleFinger3_R", "RightHandMiddle3", "Middle Distal.R", "finger03_03_R", "f_middle.03.R" },
        new[] { "RightRingProximal", "ProximalRing_Right", "ProximalRing_R", "Ring1_R", "RingFinger1_R", "RightHandRing1", "Ring Proximal.R", "finger04_01_R", "f_ring.01.R" },
        new[] { "RightRingIntermediate", "IntermediateRing_Right", "IntermediateRing_R", "Ring2_R", "RingFinger2_R", "RightHandRing2", "Ring Intermediate.R", "finger04_02_R", "f_ring.02.R" },
        new[] { "RightRingDistal", "DistalRing_Right", "DistalRing_R", "Ring3_R", "RingFinger3_R", "RightHandRing3", "Ring Distal.R", "finger04_03_R", "f_ring.03.R" },
        new[] { "RightLittleProximal", "ProximalLittle_Right", "ProximalLittle_R", "Little1_R", "LittleFinger1_R", "RightHandPinky1", "Little Proximal.R", "finger05_01_R", "f_pinky.01.R", "Pinky1.R" },
        new[] { "RightLittleIntermediate", "IntermediateLittle_Right", "IntermediateLittle_R", "Little2_R", "LittleFinger2_R", "RightHandPinky2", "Little Intermediate.R", "finger05_02_R", "f_pinky.02.R", "Pinky2.R" },
        new[] { "RightLittleDistal", "DistalLittle_Right", "DistalLittle_R", "Little3_R", "LittleFinger3_R", "RightHandPinky3", "Little Distal.R", "finger05_03_R", "f_pinky.03.R", "Pinky3.R" },
        new[] { "UpperChest", "UChest" },
    };

    // Normalized name → list of bone group indices (each group = one logical bone)
    public static readonly Dictionary<string, List<int>> NormalizedNameToGroup;
    // Bone group index → list of all normalized names for that group
    public static readonly Dictionary<int, List<string>> GroupToNames;
    // All known bone names (normalized)
    public static readonly HashSet<string> AllBoneNames;

    static BoneNameMatcher()
    {
        NormalizedNameToGroup = new Dictionary<string, List<int>>();
        GroupToNames = new Dictionary<int, List<string>>();
        AllBoneNames = new HashSet<string>();

        var pat_end_side = new Regex(@"[_\.]([LR])$", RegexOptions.IgnoreCase);

        for (int i = 0; i < BoneNamePatterns.Length; i++)
        {
            var namesInGroup = new List<string>();
            foreach (var name in BoneNamePatterns[i])
            {
                // Register base name
                RegisterNameForBone(NormalizeName(name), i, namesInGroup);

                // Side pattern: Handle "Bone.L" -> "L.Bone" (VRM style)
                var match = pat_end_side.Match(name);
                if (match.Success)
                {
                    var altName = name.Substring(0, name.Length - 2);
                    altName = match.Groups[1] + "." + altName;
                    RegisterNameForBone(NormalizeName(altName), i, namesInGroup);
                }
                else
                {
                    // VRM pattern: "C." + name for non-sided bones
                    var altName = "C." + name;
                    RegisterNameForBone(NormalizeName(altName), i, namesInGroup);
                }
            }
            GroupToNames[i] = namesInGroup;
        }
    }

    private static void RegisterNameForBone(string normalizedName, int groupIdx, List<string> namesInGroup)
    {
        AllBoneNames.Add(normalizedName);
        if (!namesInGroup.Contains(normalizedName)) namesInGroup.Add(normalizedName);

        if (!NormalizedNameToGroup.TryGetValue(normalizedName, out var groups))
        {
            groups = new List<int>();
            NormalizedNameToGroup[normalizedName] = groups;
        }
        if (!groups.Contains(groupIdx)) groups.Add(groupIdx);
    }

    /// <summary>
    /// Normalize a bone name for matching. Strips numbers, spaces, dots, underscores and lowercases.
    /// Ported from MA HeuristicBoneMapper.NormalizeName.
    /// </summary>
    public static string NormalizeName(string name)
    {
        name = name.ToLowerInvariant();
        name = Regex.Replace(name, @"^bone_|[0-9 ._]", "");
        return name;
    }

    /// <summary>
    /// Check if a name looks like a known bone name (after normalization).
    /// </summary>
    public static bool IsBoneName(string name)
    {
        return AllBoneNames.Contains(NormalizeName(name));
    }

    /// <summary>
    /// Try to find a matching avatar bone name for a given outfit bone name.
    /// Strips prefix/suffix, normalizes, and looks up in the bone pattern table.
    /// Returns the matched avatar bone name, or null if no match.
    /// </summary>
    public static string FindMatch(string outfitBoneName, string prefix, string suffix,
                                   Dictionary<string, string> avatarNameMap)
    {
        // 1. Strip prefix/suffix to get the core name
        var coreName = outfitBoneName;
        if (!string.IsNullOrEmpty(prefix) && coreName.StartsWith(prefix))
            coreName = coreName.Substring(prefix.Length);
        if (!string.IsNullOrEmpty(suffix) && coreName.EndsWith(suffix))
            coreName = coreName.Substring(0, coreName.Length - suffix.Length);

        // 2. Direct name match in avatar
        if (avatarNameMap.TryGetValue(coreName, out var directMatch))
            return directMatch;

        // 3. Normalized name lookup via bone pattern table
        var normalizedCore = NormalizeName(coreName);
        if (NormalizedNameToGroup.TryGetValue(normalizedCore, out var groups))
        {
            foreach (var groupIdx in groups)
            {
                foreach (var altName in GroupToNames[groupIdx])
                {
                    // Check if any avatar bone has this normalized name
                    foreach (var kvp in avatarNameMap)
                    {
                        if (NormalizeName(kvp.Key) == altName)
                            return kvp.Value;
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// [MA PARITY] Infer the prefix and suffix by comparing the names of the avatar's hips 
    /// with the child of the outfit armature.
    /// Ported from ModularAvatarMergeArmature.InferPrefixSuffix.
    /// </summary>
    public static (string prefix, string suffix) InferPrefixSuffix(
        string avatarHipsName,
        HierarchyItem outfitArmature)
    {
        // MA requires that the attached object has exactly one child (presumably the hips)
        if (outfitArmature.Children.Count != 1) return ("", "");

        var mergeHips = outfitArmature.Children[0];
        var mergeName = mergeHips.Name;
        var baseName = avatarHipsName;

        var candidates = new List<(string p, string s, int matches)>();

        // 1. Classic substring match
        int prefixIndex = mergeName.IndexOf(baseName, StringComparison.InvariantCulture);
        if (prefixIndex >= 0)
        {
            string p = mergeName.Substring(0, prefixIndex);
            string s = mergeName.Substring(prefixIndex + baseName.Length);
            candidates.Add((p, s, CountMatches(outfitArmature, p, s)));
        }

        // 2. Heuristic match (Fuzzy)
        var hipPatterns = BoneNamePatterns[0]; // Hips is index 0
        foreach (var hipNameCandidate in hipPatterns.OrderByDescending(p => p.Length))
        {
            int pIndex = mergeName.IndexOf(hipNameCandidate, StringComparison.InvariantCultureIgnoreCase);
            if (pIndex >= 0)
            {
                string p = mergeName.Substring(0, pIndex);
                string s = mergeName.Substring(pIndex + hipNameCandidate.Length);
                
                int matches = CountHeuristicMatches(mergeHips, p, s);
                // MA: candidate.matches = (candidate.matches + 1) / 2
                candidates.Add((p, s, (matches + 1) / 2));
                break;
            }
        }

        // Select candidate with most matches
        var selected = candidates.OrderByDescending(c => c.matches).FirstOrDefault();
        string finalPrefix = selected.p ?? "";
        string finalSuffix = selected.s ?? "";

        // VRM workaround
        if (finalPrefix == "J_Bip_C_") finalPrefix = "J_Bip_";

        return (finalPrefix, finalSuffix);
    }

    private static int CountMatches(HierarchyItem root, string prefix, string suffix)
    {
        int count = 0;
        foreach (var child in root.Flatten())
        {
            if (child.Name.StartsWith(prefix) && child.Name.EndsWith(suffix) && child.Name.Length > prefix.Length + suffix.Length)
            {
                var core = child.Name.Substring(prefix.Length, child.Name.Length - prefix.Length - suffix.Length);
                if (IsBoneName(core)) count++;
            }
        }
        return count;
    }

    private static int CountHeuristicMatches(HierarchyItem root, string prefix, string suffix)
    {
        int count = 1;
        Walk(root);
        return count;

        void Walk(HierarchyItem item)
        {
            foreach (var child in item.Children)
            {
                if (child.Name.StartsWith(prefix) && child.Name.EndsWith(suffix) && child.Name.Length > prefix.Length + suffix.Length)
                {
                    var core = child.Name.Substring(prefix.Length, child.Name.Length - prefix.Length - suffix.Length);
                    if (IsBoneName(core))
                    {
                        count++;
                        Walk(child);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Build a map of boneName → originalName for quick lookup from a list of child names.
    /// Key is the actual bone name, value is the same name (for direct lookup).
    /// </summary>
    public static Dictionary<string, string> BuildNameMap(IEnumerable<string> names)
    {
        var map = new Dictionary<string, string>();
        foreach (var name in names)
        {
            map[name] = name;
        }
        return map;
    }
}
