using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using UABEANext4.AssetWorkspace;

namespace UABEANext4.Logic.Hierarchy
{
    /// <summary>
    /// Result of an AnimationClip transfer, including detailed diagnostic info.
    /// </summary>
    public class TransferResult
    {
        public int ClipsTransferred { get; set; }
        public int TotalBindingHashes { get; set; }
        public List<string> MatchedInTarget { get; set; } = new();
        public List<string> MissingFromTarget { get; set; } = new();
        public List<string> NotFoundInSource { get; set; } = new();
        public List<string> CreatedInTarget { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public int SourcePathCount { get; set; }
        public int TargetPathCount { get; set; }
    }

    /// <summary>
    /// Handles transferring AnimationClip assets between VRCA files.
    /// Only transfers missing GameObjects identified by CRC-32 path hash comparison.
    /// Does NOT copy components, materials, textures, meshes, or any other dependencies.
    /// </summary>
    public class AnimationClipCopier
    {
        private readonly Workspace _workspace;
        private readonly AssetsFileInstance _sourceFile;
        private readonly AssetsFileInstance _targetFile;

        public AnimationClipCopier(Workspace workspace, AssetsFileInstance sourceFile, AssetsFileInstance targetFile)
        {
            _workspace = workspace;
            _sourceFile = sourceFile;
            _targetFile = targetFile;
        }

        /// <summary>
        /// Transfer one or more AnimationClip assets from source to target file.
        /// Returns a detailed TransferResult with diagnostic info.
        /// </summary>
        public TransferResult TransferAnimationClips(List<long> clipPathIds)
        {
            var result = new TransferResult();

            // Step 1: Build CRC-32 hash → path tables for BOTH files
            var sourceCrcToPath = BuildHierarchyPathTable(_sourceFile);
            var targetCrcToPath = BuildHierarchyPathTable(_targetFile);
            var targetCrcSet = new HashSet<uint>(targetCrcToPath.Keys);

            result.SourcePathCount = sourceCrcToPath.Count;
            result.TargetPathCount = targetCrcToPath.Count;

            // Step 2: Collect ALL path hashes from ALL clips
            var allReferencedHashes = new HashSet<uint>();
            var clipDataMap = new Dictionary<long, (AssetFileInfo info, AssetTypeValueField? bf)>();

            foreach (long clipPathId in clipPathIds)
            {
                var sourceInfo = _sourceFile.file.GetAssetInfo(clipPathId);
                if (sourceInfo == null) continue;
                if (sourceInfo.TypeId != (int)AssetClassID.AnimationClip) continue;

                AssetTypeValueField? clipBf;
                try { clipBf = _workspace.Manager.GetBaseField(_sourceFile, sourceInfo); }
                catch { clipBf = null; }

                clipDataMap[clipPathId] = (sourceInfo, clipBf);

                if (clipBf != null)
                    ExtractBindingHashes(clipBf, allReferencedHashes);
            }

            result.TotalBindingHashes = allReferencedHashes.Count;

            // Step 3: Compare each hash against target B
            var missingHashes = new HashSet<uint>();
            foreach (var hash in allReferencedHashes)
            {
                if (hash == 0) continue;

                if (targetCrcSet.Contains(hash))
                {
                    // Found in target B - no need to import
                    string targetPath = targetCrcToPath.TryGetValue(hash, out var tp) ? tp : "?";
                    result.MatchedInTarget.Add($"[{hash}] → {targetPath}");
                }
                else if (sourceCrcToPath.ContainsKey(hash))
                {
                    // Missing from B but found in A - need to import
                    string sourcePath = sourceCrcToPath[hash];
                    result.MissingFromTarget.Add($"[{hash}] → {sourcePath}");
                    missingHashes.Add(hash);
                }
                else
                {
                    // Not found in either file
                    result.NotFoundInSource.Add($"[{hash}] → (not found in source A either)");
                }
            }

            // Step 4: Transfer missing objects
            if (missingHashes.Count > 0)
            {
                TransferMissingObjects(missingHashes, sourceCrcToPath, targetCrcToPath, result);
            }

            // Step 5: Copy each AnimationClip asset itself
            foreach (long clipPathId in clipPathIds)
            {
                if (!clipDataMap.TryGetValue(clipPathId, out var data)) continue;
                try
                {
                    CopyAnimationClipAsset(data.info, data.bf);
                    result.ClipsTransferred++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Failed to copy clip {clipPathId}: {ex.Message}");
                }
            }

            return result;
        }

        /// <summary>
        /// Extract all CRC-32 path hashes from an AnimationClip's bindings.
        /// </summary>
        private void ExtractBindingHashes(AssetTypeValueField clipBf, HashSet<uint> hashes)
        {
            var bindingConstant = clipBf["m_ClipBindingConstant"];
            if (bindingConstant != null && !bindingConstant.IsDummy)
            {
                var genericBindings = bindingConstant["genericBindings"]["Array"];
                if (genericBindings != null && !genericBindings.IsDummy)
                {
                    foreach (var binding in genericBindings.Children)
                    {
                        uint pathHash = binding["path"].AsUInt;
                        if (pathHash != 0)
                            hashes.Add(pathHash);
                    }
                }
            }

            string[] curveArrayNames = { "m_FloatCurves", "m_RotationCurves", "m_PositionCurves",
                                          "m_ScaleCurves", "m_EulerCurves", "m_PPtrCurves" };
            foreach (var curveName in curveArrayNames)
            {
                var curveArray = clipBf[curveName]?["Array"];
                if (curveArray != null && !curveArray.IsDummy)
                {
                    foreach (var curve in curveArray.Children)
                    {
                        var pathField = curve["path"];
                        if (pathField != null && !pathField.IsDummy)
                        {
                            string path = pathField.AsString;
                            if (!string.IsNullOrEmpty(path))
                            {
                                uint hash = Crc32Helper.ComputeCrc32(path);
                                hashes.Add(hash);
                            }
                        }
                    }
                }
            }
        }

        // ===================== HIERARCHY PATH TABLE =====================

        /// <summary>
        /// Build CRC-32 hash → hierarchy path string for all objects.
        /// Also populates the reverse mapping (path → TransformPathID).
        /// </summary>
        private Dictionary<uint, string> BuildHierarchyPathTable(AssetsFileInstance fileInst)
        {
            var result = new Dictionary<uint, string>();
            var transformInfos = new Dictionary<long, AssetFileInfo>();

            foreach (var info in fileInst.file.Metadata.AssetInfos)
            {
                if (info.TypeId == (int)AssetClassID.Transform || info.TypeId == (int)AssetClassID.RectTransform)
                    transformInfos[info.PathId] = info;
            }

            var parentToChildren = new Dictionary<long, List<long>>();
            var transformToName = new Dictionary<long, string>();
            var roots = new List<long>();

            foreach (var kvp in transformInfos)
            {
                try
                {
                    var tfmBf = _workspace.Manager.GetBaseField(fileInst, kvp.Value);
                    if (tfmBf == null) continue;

                    long fatherPathId = tfmBf["m_Father"]["m_PathID"].AsLong;

                    if (fatherPathId == 0)
                        roots.Add(kvp.Key);
                    else
                    {
                        if (!parentToChildren.ContainsKey(fatherPathId))
                            parentToChildren[fatherPathId] = new List<long>();
                        parentToChildren[fatherPathId].Add(kvp.Key);
                    }

                    long goPathId = tfmBf["m_GameObject"]["m_PathID"].AsLong;
                    if (goPathId != 0)
                    {
                        var goInfo = fileInst.file.GetAssetInfo(goPathId);
                        if (goInfo != null)
                        {
                            try
                            {
                                var goBf = _workspace.Manager.GetBaseField(fileInst, goInfo);
                                if (goBf != null)
                                    transformToName[kvp.Key] = goBf["m_Name"].AsString;
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            foreach (long rootTfm in roots)
            {
                if (parentToChildren.TryGetValue(rootTfm, out var children))
                {
                    foreach (long child in children)
                        BuildPathsDFS(child, "", parentToChildren, transformToName, result);
                }
            }

            return result;
        }

        private void BuildPathsDFS(long transformPathId, string parentPath,
            Dictionary<long, List<long>> parentToChildren,
            Dictionary<long, string> transformToName,
            Dictionary<uint, string> result)
        {
            if (!transformToName.TryGetValue(transformPathId, out string? name) || name == null)
                return;

            string fullPath = string.IsNullOrEmpty(parentPath) ? name : parentPath + "/" + name;
            uint hash = Crc32Helper.ComputeCrc32(fullPath);

            if (!result.ContainsKey(hash))
                result[hash] = fullPath;

            if (parentToChildren.TryGetValue(transformPathId, out var children))
            {
                foreach (long child in children)
                    BuildPathsDFS(child, fullPath, parentToChildren, transformToName, result);
            }
        }

        // ===================== TRANSFER MISSING OBJECTS =====================

        /// <summary>
        /// For each missing CRC-32 hash, find the object name in source A,
        /// create a BARE GameObject + Transform in target B (no components, no data),
        /// and link it into the correct parent in B's hierarchy.
        /// </summary>
        private void TransferMissingObjects(HashSet<uint> missingHashes,
            Dictionary<uint, string> sourceCrcToPath, Dictionary<uint, string> targetCrcToPath, TransferResult diagnostics)
        {
            // Build path→TransformPathID lookups for both files
            var sourcePathToTransform = BuildPathToTransformLookup(_sourceFile);
            var targetPathToTransform = BuildPathToTransformLookup(_targetFile);

            // Collect all paths that need to be created (including intermediate parents)
            var pathsToCreate = new HashSet<string>();
            foreach (uint hash in missingHashes)
            {
                if (!sourceCrcToPath.TryGetValue(hash, out string? fullPath) || fullPath == null)
                    continue;

                // Walk up the path to find the deepest existing ancestor in target
                string currentPath = fullPath;
                while (!string.IsNullOrEmpty(currentPath))
                {
                    if (targetPathToTransform.ContainsKey(currentPath))
                        break; // This ancestor already exists in target

                    pathsToCreate.Add(currentPath);

                    int lastSlash = currentPath.LastIndexOf('/');
                    if (lastSlash > 0)
                        currentPath = currentPath.Substring(0, lastSlash);
                    else
                        break; // Top-level node
                }
            }

            if (pathsToCreate.Count == 0) return;

            // Sort by depth (shallowest first) so parents are created before children
            var sortedPaths = pathsToCreate.OrderBy(p => p.Count(c => c == '/')).ToList();

            foreach (string path in sortedPaths)
            {
                // Skip if already created in a previous iteration
                if (targetPathToTransform.ContainsKey(path)) continue;
                // Must exist in source
                if (!sourcePathToTransform.ContainsKey(path))
                {
                    diagnostics.Errors.Add($"Path '{path}' not found in source Transform lookup");
                    continue;
                }

                try
                {
                    // Get the object name (last segment of path)
                    string objectName = path;
                    int lastSlash = path.LastIndexOf('/');
                    if (lastSlash >= 0)
                        objectName = path.Substring(lastSlash + 1);

                    // Find the parent Transform PathID in the TARGET
                    long targetParentTransformId = 0;
                    if (lastSlash > 0)
                    {
                        string parentPath = path.Substring(0, lastSlash);
                        if (targetPathToTransform.TryGetValue(parentPath, out long parentId))
                            targetParentTransformId = parentId;
                        else
                            diagnostics.Errors.Add($"Parent '{parentPath}' not found in target for '{path}'");
                    }
                    else
                    {
                        // Top-level: parent is the root Transform
                        targetParentTransformId = FindRootTransform(_targetFile);
                    }

                    if (targetParentTransformId == 0)
                    {
                        diagnostics.Errors.Add($"No parent found for '{path}', skipping");
                        continue;
                    }

                    // Create a bare GameObject + Transform in target
                    long newTransformId = CreateBareGameObject(objectName, targetParentTransformId);
                    if (newTransformId != 0)
                    {
                        targetPathToTransform[path] = newTransformId;
                        uint crc = Crc32Helper.ComputeCrc32(path);
                        diagnostics.CreatedInTarget.Add($"[{crc}] → {path} (TransformID={newTransformId})");
                    }
                    else
                    {
                        diagnostics.Errors.Add($"CreateBareGameObject failed for '{path}'");
                    }
                }
                catch (Exception ex)
                {
                    diagnostics.Errors.Add($"Error creating '{path}': {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Find the root Transform (m_Father.m_PathID == 0) in a file.
        /// </summary>
        private long FindRootTransform(AssetsFileInstance fileInst)
        {
            foreach (var info in fileInst.file.Metadata.AssetInfos)
            {
                if (info.TypeId != (int)AssetClassID.Transform && info.TypeId != (int)AssetClassID.RectTransform)
                    continue;

                try
                {
                    var tfmBf = _workspace.Manager.GetBaseField(fileInst, info);
                    if (tfmBf == null) continue;
                    long fatherId = tfmBf["m_Father"]["m_PathID"].AsLong;
                    if (fatherId == 0) return info.PathId;
                }
                catch { }
            }
            return 0;
        }

        /// <summary>
        /// Create a minimal GameObject + Transform pair in the target file.
        /// Clones the source file's Transform+GameObject structure and modifies the fields.
        /// Links the new Transform as a child of parentTransformId.
        /// Returns the new Transform's PathID.
        /// </summary>
        private long CreateBareGameObject(string name, long parentTransformId)
        {
            // Find a source Transform+GameObject to clone the structure from
            var sourceTemplateTfm = FindTemplateTransformInfo(_sourceFile);
            if (sourceTemplateTfm == null) return 0;

            var sourceTfmBf = _workspace.Manager.GetBaseField(_sourceFile, sourceTemplateTfm);
            if (sourceTfmBf == null) return 0;

            long sourceGoPathId = sourceTfmBf["m_GameObject"]["m_PathID"].AsLong;
            var sourceGoInfo = _sourceFile.file.GetAssetInfo(sourceGoPathId);
            if (sourceGoInfo == null) return 0;

            var sourceGoBf = _workspace.Manager.GetBaseField(_sourceFile, sourceGoInfo);
            if (sourceGoBf == null) return 0;

            // Allocate two new PathIDs
            long maxPathId = 0;
            foreach (var info in _targetFile.file.Metadata.AssetInfos)
            {
                if (info.PathId > maxPathId) maxPathId = info.PathId;
            }
            long goPathId = maxPathId + 1;
            long tfmPathId = maxPathId + 2;

            // --- Modify the cloned Transform ---
            sourceTfmBf["m_GameObject"]["m_FileID"].AsInt = 0;
            sourceTfmBf["m_GameObject"]["m_PathID"].AsLong = goPathId;
            sourceTfmBf["m_Father"]["m_FileID"].AsInt = 0;
            sourceTfmBf["m_Father"]["m_PathID"].AsLong = parentTransformId;

            // Clear children array
            var childrenArr = sourceTfmBf["m_Children"]["Array"];
            if (childrenArr != null && !childrenArr.IsDummy && childrenArr.Children != null)
                childrenArr.Children.Clear();

            // Identity transform
            sourceTfmBf["m_LocalRotation"]["x"].AsFloat = 0f;
            sourceTfmBf["m_LocalRotation"]["y"].AsFloat = 0f;
            sourceTfmBf["m_LocalRotation"]["z"].AsFloat = 0f;
            sourceTfmBf["m_LocalRotation"]["w"].AsFloat = 1f;
            sourceTfmBf["m_LocalPosition"]["x"].AsFloat = 0f;
            sourceTfmBf["m_LocalPosition"]["y"].AsFloat = 0f;
            sourceTfmBf["m_LocalPosition"]["z"].AsFloat = 0f;
            sourceTfmBf["m_LocalScale"]["x"].AsFloat = 1f;
            sourceTfmBf["m_LocalScale"]["y"].AsFloat = 1f;
            sourceTfmBf["m_LocalScale"]["z"].AsFloat = 1f;

            // Serialize Transform
            var tfmInfo = AssetFileInfo.Create(
                _targetFile.file, tfmPathId, (int)AssetClassID.Transform, 0xFFFF,
                _workspace.Manager.ClassDatabase, false);
            var tfmAsset = new AssetInst(_targetFile, tfmInfo);
            tfmAsset.UpdateAssetDataAndRow(_workspace, sourceTfmBf);

            // --- Modify the cloned GameObject ---
            sourceGoBf["m_Name"].AsString = name;
            sourceGoBf["m_IsActive"].AsUInt = 1;

            // Set m_Component to only contain our new Transform
            var compArray = sourceGoBf["m_Component"]["Array"];
            if (compArray != null && !compArray.IsDummy && compArray.Children != null && compArray.Children.Count > 0)
            {
                // Keep the first component entry as template, modify it, remove the rest
                var firstComp = compArray.Children[0];
                var pptr = firstComp[firstComp.Children.Count - 1];
                pptr["m_FileID"].AsInt = 0;
                pptr["m_PathID"].AsLong = tfmPathId;

                // Remove all other components
                while (compArray.Children.Count > 1)
                    compArray.Children.RemoveAt(compArray.Children.Count - 1);
            }

            // Serialize GameObject
            var goInfo = AssetFileInfo.Create(
                _targetFile.file, goPathId, (int)AssetClassID.GameObject, 0xFFFF,
                _workspace.Manager.ClassDatabase, false);
            var goAsset = new AssetInst(_targetFile, goInfo);
            goAsset.UpdateAssetDataAndRow(_workspace, sourceGoBf);

            // --- Add both to the target file ---
            if (_targetFile.file.Metadata.AssetInfos is System.Collections.ObjectModel.RangeObservableCollection<AssetFileInfo> targetInfos)
            {
                targetInfos.Add(goAsset);
                targetInfos.Add(tfmAsset);
            }
            else
            {
                _targetFile.file.Metadata.AssetInfos.Add(goAsset);
                _targetFile.file.Metadata.AssetInfos.Add(tfmAsset);
            }

            _targetFile.file.GenerateQuickLookup();

            // --- Link to parent: add this Transform to parent's m_Children ---
            var parentInst = _workspace.GetAssetInst(_targetFile, 0, parentTransformId);
            if (parentInst != null)
            {
                var parentBf = _workspace.GetBaseField(parentInst);
                if (parentBf != null)
                {
                    var parentChildren = parentBf["m_Children"]["Array"];
                    if (parentChildren != null && !parentChildren.IsDummy && parentChildren.Children != null)
                    {
                        // Create a new PPtr<Transform> entry using ValueBuilder
                        // which properly initializes all Value objects
                        AssetTypeValueField? newChildRef = null;

                        if (parentChildren.Children.Count > 0)
                        {
                            // Clone from an existing child entry (safest)
                            newChildRef = ValueBuilder.DefaultValueFieldFromTemplate(
                                parentChildren.Children[0].TemplateField);
                        }
                        else
                        {
                            // No existing children - use m_Father PPtr as structural template
                            var fatherField = parentBf["m_Father"];
                            if (fatherField != null && !fatherField.IsDummy)
                            {
                                newChildRef = ValueBuilder.DefaultValueFieldFromTemplate(
                                    fatherField.TemplateField);
                            }
                        }

                        if (newChildRef != null)
                        {
                            newChildRef["m_FileID"].AsInt = 0;
                            newChildRef["m_PathID"].AsLong = tfmPathId;
                            parentChildren.Children.Add(newChildRef);
                            parentInst.UpdateAssetDataAndRow(_workspace, parentBf);
                        }
                    }
                }
            }

            // Add to preload table
            AddToPreloadTable(goPathId);
            AddToPreloadTable(tfmPathId);

            return tfmPathId;
        }

        private AssetFileInfo? FindTemplateTransformInfo(AssetsFileInstance fileInst)
        {
            foreach (var info in fileInst.file.Metadata.AssetInfos)
            {
                if (info.TypeId == (int)AssetClassID.Transform)
                    return info;
            }
            return null;
        }

        // ===================== PATH → TRANSFORM LOOKUP =====================

        private Dictionary<string, long> BuildPathToTransformLookup(AssetsFileInstance fileInst)
        {
            var result = new Dictionary<string, long>();
            var transformInfos = new Dictionary<long, AssetFileInfo>();

            foreach (var info in fileInst.file.Metadata.AssetInfos)
            {
                if (info.TypeId == (int)AssetClassID.Transform || info.TypeId == (int)AssetClassID.RectTransform)
                    transformInfos[info.PathId] = info;
            }

            var parentToChildren = new Dictionary<long, List<long>>();
            var transformToName = new Dictionary<long, string>();
            var roots = new List<long>();

            foreach (var kvp in transformInfos)
            {
                try
                {
                    var tfmBf = _workspace.Manager.GetBaseField(fileInst, kvp.Value);
                    if (tfmBf == null) continue;

                    long fatherPathId = tfmBf["m_Father"]["m_PathID"].AsLong;

                    if (fatherPathId == 0)
                        roots.Add(kvp.Key);
                    else
                    {
                        if (!parentToChildren.ContainsKey(fatherPathId))
                            parentToChildren[fatherPathId] = new List<long>();
                        parentToChildren[fatherPathId].Add(kvp.Key);
                    }

                    long goPathId = tfmBf["m_GameObject"]["m_PathID"].AsLong;
                    if (goPathId != 0)
                    {
                        var goInfo = fileInst.file.GetAssetInfo(goPathId);
                        if (goInfo != null)
                        {
                            try
                            {
                                var goBf = _workspace.Manager.GetBaseField(fileInst, goInfo);
                                if (goBf != null)
                                    transformToName[kvp.Key] = goBf["m_Name"].AsString;
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            foreach (long root in roots)
            {
                if (parentToChildren.TryGetValue(root, out var children))
                {
                    foreach (long child in children)
                        BuildPathToTransformDFS(child, "", parentToChildren, transformToName, result);
                }
            }

            return result;
        }

        private void BuildPathToTransformDFS(long transformPathId, string parentPath,
            Dictionary<long, List<long>> parentToChildren,
            Dictionary<long, string> transformToName,
            Dictionary<string, long> result)
        {
            if (!transformToName.TryGetValue(transformPathId, out string? name) || name == null)
                return;

            string fullPath = string.IsNullOrEmpty(parentPath) ? name : parentPath + "/" + name;
            if (!result.ContainsKey(fullPath))
                result[fullPath] = transformPathId;

            if (parentToChildren.TryGetValue(transformPathId, out var children))
            {
                foreach (long child in children)
                    BuildPathToTransformDFS(child, fullPath, parentToChildren, transformToName, result);
            }
        }

        // ===================== COPY ANIMATIONCLIP ASSET =====================

        private void CopyAnimationClipAsset(AssetFileInfo sourceInfo, AssetTypeValueField? clipBf)
        {
            // Check if target already has an AnimationClip with the same name
            if (clipBf != null)
            {
                string clipName = clipBf["m_Name"]?.AsString ?? "";
                if (!string.IsNullOrEmpty(clipName))
                {
                    foreach (var existingInfo in _targetFile.file.Metadata.AssetInfos)
                    {
                        if (existingInfo.TypeId == (int)AssetClassID.AnimationClip)
                        {
                            try
                            {
                                var existingBf = _workspace.Manager.GetBaseField(_targetFile, existingInfo);
                                if (existingBf != null && existingBf["m_Name"]?.AsString == clipName)
                                    return; // Already exists, skip
                            }
                            catch { }
                        }
                    }
                }
            }

            long newPathId = 1;
            if (_targetFile.file.Metadata.AssetInfos.Count > 0)
                newPathId = _targetFile.file.Metadata.AssetInfos.Max(a => a.PathId) + 1;

            var targetInfo = AssetFileInfo.Create(
                _targetFile.file, newPathId, sourceInfo.TypeId, 0xFFFF,
                _workspace.Manager.ClassDatabase, false);

            var newAsset = new AssetInst(_targetFile, targetInfo);

            if (clipBf != null)
            {
                RemapClipPPtrs(clipBf);
                newAsset.UpdateAssetDataAndRow(_workspace, clipBf);
            }
            else
            {
                byte[]? rawData = GetSourceAssetRawBytes(sourceInfo);
                if (rawData != null)
                    newAsset.UpdateAssetDataAndRow(_workspace, rawData);
            }

            if (_targetFile.file.Metadata.AssetInfos is System.Collections.ObjectModel.RangeObservableCollection<AssetFileInfo> targetInfos)
                targetInfos.Add(newAsset);
            else
                _targetFile.file.Metadata.AssetInfos.Add(newAsset);

            _targetFile.file.GenerateQuickLookup();
            AddToPreloadTable(newPathId);

            var wsItem = _workspace.FindWorkspaceItemByInstance(_targetFile);
            if (wsItem != null) _workspace.Dirty(wsItem);
        }

        private void RemapClipPPtrs(AssetTypeValueField field)
        {
            if (field.Children == null) return;

            bool isPtr = field.Children.Count >= 2 &&
                         field.Children[0].TemplateField.Name == "m_FileID" &&
                         field.Children[1].TemplateField.Name == "m_PathID";

            if (isPtr)
            {
                int fileId = field.Children[0].AsInt;
                long pathId = field.Children[1].AsLong;

                if (fileId != 0)
                {
                    int sourceIdx = fileId - 1;
                    if (sourceIdx < _sourceFile.file.Metadata.Externals.Count)
                    {
                        var sourceExt = _sourceFile.file.Metadata.Externals[sourceIdx];
                        string sourcePathName = Path.GetFileName(sourceExt.PathName);

                        bool found = false;
                        for (int i = 0; i < _targetFile.file.Metadata.Externals.Count; i++)
                        {
                            var targetExt = _targetFile.file.Metadata.Externals[i];
                            if (Path.GetFileName(targetExt.PathName).Equals(sourcePathName, StringComparison.OrdinalIgnoreCase))
                            {
                                field.Children[0].AsInt = i + 1;
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            var newExt = new AssetsFileExternal
                            {
                                PathName = sourceExt.PathName,
                                OriginalPathName = sourceExt.OriginalPathName,
                                Type = sourceExt.Type,
                                Guid = sourceExt.Guid
                            };
                            _targetFile.file.Metadata.Externals.Add(newExt);
                            field.Children[0].AsInt = _targetFile.file.Metadata.Externals.Count;
                        }
                    }
                }
                else if (pathId != 0)
                {
                    field.Children[1].AsLong = 0;
                }
                return;
            }

            foreach (var child in field.Children)
                RemapClipPPtrs(child);
        }

        private byte[]? GetSourceAssetRawBytes(AssetFileInfo info)
        {
            try
            {
                var reader = _sourceFile.file.Reader;
                long absOffset = info.GetAbsoluteByteOffset(_sourceFile.file);
                lock (reader)
                {
                    reader.Position = absOffset;
                    return reader.ReadBytes((int)info.ByteSize);
                }
            }
            catch { return null; }
        }

        // ===================== PRELOAD TABLE =====================

        private void AddToPreloadTable(long pathId)
        {
            AssetFileInfo? abInfo = null;
            foreach (var info in _targetFile.file.Metadata.AssetInfos)
            {
                if (info.TypeId == (int)AssetClassID.AssetBundle)
                {
                    abInfo = info;
                    break;
                }
            }
            if (abInfo == null) return;

            var abAsset = _workspace.GetAssetInst(_targetFile, 0, abInfo.PathId);
            if (abAsset == null) return;

            var abBf = _workspace.GetBaseField(abAsset);
            if (abBf == null) return;

            var preloadTable = abBf["m_PreloadTable"]["Array"];
            if (preloadTable.IsDummy || preloadTable.Children.Count == 0) return;

            var template = ValueBuilder.DefaultValueFieldFromTemplate(preloadTable.Children[0].TemplateField);
            if (template == null) return;

            template["m_FileID"].AsInt = 0;
            template["m_PathID"].AsLong = pathId;
            preloadTable.Children.Add(template);

            var container = abBf["m_Container"]["Array"];
            if (!container.IsDummy && container.Children != null)
            {
                foreach (var item in container.Children)
                {
                    var second = item["second"];
                    if (second != null && !second.IsDummy)
                        second["preloadSize"].AsInt = preloadTable.Children.Count;
                }
            }

            abAsset.UpdateAssetDataAndRow(_workspace, abBf);
        }
    }
}
