using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using UABEANext4.AssetWorkspace;
using UABEANext4.Logic.Hierarchy;

namespace UABEANext4.Logic.Hierarchy
{
    public class TransferRecord
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public long SourcePathId { get; set; }
        public long TargetPathId { get; set; }
    }

    public class CrossFileCopier
    {
        private readonly Workspace _workspace;
        private readonly AssetsFileInstance _sourceFile;
        private readonly AssetsFileInstance _targetFile;
        private readonly Dictionary<long, long> _remapTable = new(); // Source PathID -> Target PathID
        private readonly Dictionary<int, int> _externalRemapTable = new(); // Source FileID -> Target FileID
        private readonly HashSet<long> _discoveredPathIds = new();
        private readonly long _rootTransformPathId;
        
        // Track which PathIDs were found to be truncated and require pure byte-level processing
        private readonly HashSet<long> _truncatedPathIds = new();
        
        // Bone name remapping: maps Transform name -> PathID in the TARGET file
        private readonly Dictionary<string, long> _targetBoneNameToPathId = new();
        private readonly Dictionary<string, long> _targetGameObjectNameToPathId = new();
        
        // Cache of source Transform PathID -> GameObject name (for name-based lookup)
        private readonly Dictionary<long, string> _sourceTransformNameCache = new();
        private readonly Dictionary<long, string> _sourceGameObjectNameCache = new();
        private readonly Dictionary<long, long> _compToGoMap = new(); // Component PathID -> GameObject PathID (Source)
        private readonly Dictionary<string, long> _targetScriptNameToPathId = new(); // MonoScript name -> Target PathID
        private readonly bool _copyChildren;
        
        // Report data for the transfer window
        private readonly List<TransferRecord> _transferRecords = new();

        public IReadOnlyList<TransferRecord> GetTransferRecords() => _transferRecords;

        public CrossFileCopier(Workspace workspace, AssetsFileInstance sourceFile, AssetsFileInstance targetFile, long rootTransformPathId, bool copyChildren = true)
        {
            _workspace = workspace;
            _sourceFile = sourceFile;
            _targetFile = targetFile;
            _rootTransformPathId = rootTransformPathId;
            _copyChildren = copyChildren;
        }

        public long CopyHierarchy()
        {
            if (_sourceFile == _targetFile)
                throw new InvalidOperationException("Source and target files cannot be the same for CrossFileCopier.");

            // ==============================================================
            // [注意] 以前在这里的脚本升级逻辑已经被 Universal 引擎完全取代
            // ==============================================================

            var rootTfmBf = _workspace.GetBaseField(_sourceFile, _rootTransformPathId);
            if (rootTfmBf == null) return 0;
            
            long rootGoPathId = rootTfmBf["m_GameObject"]["m_PathID"].AsLong;

            // 0. Build bone name lookup tables BEFORE discovery so we can use them for smart skipping
            BuildBoneNameLookups();

            // 1. Discover all source dependencies
            DiscoverDependencies(rootGoPathId);

            if (_discoveredPathIds.Count == 0) return 0;

            // 2. Allocate new Path IDs in target file
            AllocateTargetPathIds();

            // 4. Create assets in target (using raw bytes or baseField serialization)
            CreateInitialTargetAssets();

            // 5. Remap PPtrs on the target side
            RemapTargetAssets();

            // ==============================================================
            // [修复核心点] 将通用脚本升级移到这里！
            // 此时，旧的数据已经被成功拷贝到 _targetFile，且完成了 PPtr 映射。
            // 现在调用它，才能自动识别所有脚本类型并根据源文件 A 的蓝图进行强制洗脑升级。
            // ==============================================================
            ForceUpgradeAllScripts();
            
            // ==============================================================
            // [VRC 安全扫描修复] 垃圾回收与序号重排
            // 清理冗余的 TypeTree 字典和没用的 ScriptTypes，确保它们从 0 开始连续且无废弃项
            // ==============================================================
            CleanupUnusedMetadata();

            // ==============================================================
            // [最终兜底强力修复] 二次全字段扫描 PPtr 重映射
            // 确保在所有脚本洗脑和升级后，没有任何一个指针还残留着 A 文件的 PathID。
            // ==============================================================
            FinalSweepRemap();

            // 6. Flush appended resource data to the target bundle
            ApplyResSChanges();

            // 7. Register new assets in the AssetBundle's preload table
            UpdateAssetBundlePreloadTable();

            // Bug 2 Fix: 递归标记所有父级为脏 (确保顶层 Bundle 也能触发物理保存)
            var currentRoot = _workspace.FindWorkspaceItemByInstance(_targetFile);
            while (currentRoot != null)
            {
                _workspace.Dirty(currentRoot);
                currentRoot = currentRoot.Parent;
            }

            return _remapTable.TryGetValue(_rootTransformPathId, out long newTransformId) ? newTransformId : 0;
        }

        // ==================== PHASE 1: DISCOVERY ====================
        
        private void DiscoverDependencies(long startGoPathId)
        {
            var hierarchyQueue = new Queue<long>();
            var componentQueue = new Queue<long>(); 
            var dataQueue = new Queue<long>();
            
            hierarchyQueue.Enqueue(startGoPathId);
            
            // Unified Crawler Loop: Continue as long as ANY asset is discovered
            while (hierarchyQueue.Count > 0 || componentQueue.Count > 0 || dataQueue.Count > 0)
            {
                // Phase 1a: Walk the hierarchy tree (GameObjects + Transforms + their children)
                while (hierarchyQueue.Count > 0)
                {
                    long currentPathId = hierarchyQueue.Dequeue();
                    if (currentPathId == 0 || !_discoveredPathIds.Add(currentPathId)) continue;
                    
                    var info = _sourceFile.file.GetAssetInfo(currentPathId);
                    if (info == null) continue;
                    
                    AssetTypeValueField? bf;
                    try { bf = _workspace.Manager.GetBaseField(_sourceFile, info); }
                    catch { continue; }
                    if (bf == null) continue;
                    
                    if (info.TypeId == (int)AssetClassID.GameObject)
                    {
                        var components = bf["m_Component"]["Array"];
                        if (!components.IsDummy && components.Children != null)
                        {
                            foreach (var comp in components.Children)
                            {
                                var ptr = comp[comp.Children.Count - 1];
                                if (ptr["m_FileID"].AsInt == 0)
                                {
                                    long compPathId = ptr["m_PathID"].AsLong;
                                    if (compPathId != 0)
                                    {
                                        // Always record host association if not already present
                                        if (!_compToGoMap.ContainsKey(compPathId))
                                        {
                                            _compToGoMap[compPathId] = currentPathId;
                                        }

                                        if (!_discoveredPathIds.Contains(compPathId))
                                        {
                                            var compInfo = _sourceFile.file.GetAssetInfo(compPathId);
                                            if (compInfo != null)
                                            {
                                                if (compInfo.TypeId == (int)AssetClassID.Transform || 
                                                    compInfo.TypeId == (int)AssetClassID.RectTransform)
                                                {
                                                    hierarchyQueue.Enqueue(compPathId);
                                                }
                                                else
                                                {
                                                    _discoveredPathIds.Add(compPathId);
                                                    componentQueue.Enqueue(compPathId);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (info.TypeId == (int)AssetClassID.Transform || info.TypeId == (int)AssetClassID.RectTransform)
                    {
                        if (_copyChildren)
                        {
                            var children = bf["m_Children"]["Array"];
                            if (!children.IsDummy && children.Children != null)
                            {
                                foreach (var child in children.Children)
                                {
                                    if (child["m_FileID"].AsInt == 0)
                                    {
                                        long childPathId = child["m_PathID"].AsLong;
                                        if (childPathId != 0) hierarchyQueue.Enqueue(childPathId);
                                    }
                                }
                            }
                        }
                        
                        var go = bf["m_GameObject"];
                        if (!go.IsDummy && go["m_FileID"].AsInt == 0)
                        {
                            long goPathId = go["m_PathID"].AsLong;
                            if (goPathId != 0) hierarchyQueue.Enqueue(goPathId);
                        }
                    }
                }
                
                // Phase 1b: Scan components for data dependencies
                while (componentQueue.Count > 0)
                {
                    long compPathId = componentQueue.Dequeue();
                    var info = _sourceFile.file.GetAssetInfo(compPathId);
                    if (info == null) continue;
                    
                    AssetTypeValueField? bf = null;
                    try { bf = _workspace.Manager.GetBaseField(_sourceFile, info); }
                    catch { }
                    
                    if (bf == null || bf.IsDummy) 
                    {
                        _truncatedPathIds.Add(compPathId);
                        byte[] rawBytes = GetSourceAssetRawBytes(info);
                        if (rawBytes != null) ScanRawBytesForDataDependencies(rawBytes, dataQueue);
                        continue;
                    }
                    
                    ScanFieldForDataDependencies(bf, dataQueue);

                    try
                    {
                        byte[] mockWrite = bf.WriteToByteArray();
                        if (info.ByteSize - mockWrite.Length > 4)
                        {
                            _truncatedPathIds.Add(compPathId);
                            byte[] rawBytes = GetSourceAssetRawBytes(info);
                            if (rawBytes != null) ScanRawBytesForDataDependencies(rawBytes, dataQueue);
                        }
                    }
                    catch { }
                }
                
                // Phase 1c: Recursively discover data asset dependencies
                while (dataQueue.Count > 0)
                {
                    long dataPathId = dataQueue.Dequeue();
                    if (dataPathId == 0 || _discoveredPathIds.Contains(dataPathId)) continue;
                    
                    var info = _sourceFile.file.GetAssetInfo(dataPathId);
                    if (info == null) continue;
                    
                    if (info.TypeId == (int)AssetClassID.GameObject ||
                        info.TypeId == (int)AssetClassID.Transform ||
                        info.TypeId == (int)AssetClassID.RectTransform)
                    {
                        // [核心关键] 发现新的 host 物体：将其扔回 hierarchyQueue 进行完整探测
                        if (TryRemapByBoneName(dataPathId, info.TypeId == (int)AssetClassID.GameObject) != 0) continue;
                        hierarchyQueue.Enqueue(dataPathId);
                        continue;
                    }

                    if (info.TypeId == (int)AssetClassID.MonoBehaviour || info.TypeId == 21 || info.TypeId == 137)
                    {
                        try
                        {
                            var compGoBf = _workspace.Manager.GetBaseField(_sourceFile, info);
                            if (compGoBf != null)
                            {
                                long goPathId = compGoBf["m_GameObject"]["m_PathID"].AsLong;
                                if (goPathId != 0 && !_discoveredPathIds.Contains(goPathId))
                                {
                                    hierarchyQueue.Enqueue(goPathId);
                                }
                            }
                        }
                        catch { }
                    }

                    if (info.TypeId == (int)AssetClassID.MonoScript)
                    {
                        // [URGENT BUGFIX] Deduplicate BEFORE adding to _discoveredPathIds
                        string? name = GetAssetNameRaw(_sourceFile, dataPathId);
                        if (!string.IsNullOrEmpty(name) && _targetScriptNameToPathId.TryGetValue(name, out long targetId))
                        {
                            _remapTable[dataPathId] = targetId;
                            continue; // Skip discovery entirely for duplicated scripts
                        }
                    }

                    _discoveredPathIds.Add(dataPathId);
                    
                    AssetTypeValueField? bf = null;
                    try { bf = _workspace.Manager.GetBaseField(_sourceFile, info); }
                    catch { }
                    
                    if (bf == null || bf.IsDummy)
                    {
                        _truncatedPathIds.Add(dataPathId);
                        byte[] rawBytes = GetSourceAssetRawBytes(info);
                        if (rawBytes != null) ScanRawBytesForDataDependencies(rawBytes, dataQueue);
                        continue;
                    }
                    
                    ScanFieldForDataDependencies(bf, dataQueue);

                    try
                    {
                        byte[] mockWrite = bf.WriteToByteArray();
                        if (info.ByteSize - mockWrite.Length > 4)
                        {
                            _truncatedPathIds.Add(dataPathId);
                            byte[] rawBytes = GetSourceAssetRawBytes(info);
                            if (rawBytes != null) ScanRawBytesForDataDependencies(rawBytes, dataQueue);
                        }
                    }
                    catch { }
                }
            }
        }

        // ==================== BONE NAME LOOKUP ====================

        /// <summary>
        /// Build lookup tables mapping bone/Transform names to PathIDs in both files.
        /// This enables intelligent remapping of SkinnedMeshRenderer bone references
        /// to the target avatar's skeleton instead of zeroing them out.
        /// </summary>
        private void BuildBoneNameLookups()
        {
            _sourceTransformNameCache.Clear();
            _targetBoneNameToPathId.Clear();
            _sourceGameObjectNameCache.Clear();
            _targetGameObjectNameToPathId.Clear();
            _targetScriptNameToPathId.Clear();

            // Build source name caches (Binary-safe)
            foreach (var info in _sourceFile.file.Metadata.AssetInfos)
            {
                if (info.TypeId == (int)AssetClassID.Transform || info.TypeId == (int)AssetClassID.RectTransform)
                {
                    long goPathId = GetTransformHostGoPathIdRaw(_sourceFile, info.PathId);
                    string? name = GetAssetNameRaw(_sourceFile, goPathId);
                    if (name != null) _sourceTransformNameCache[info.PathId] = name;
                }
                else if (info.TypeId == (int)AssetClassID.GameObject)
                {
                    string? name = GetAssetNameRaw(_sourceFile, info.PathId);
                    if (name != null) _sourceGameObjectNameCache[info.PathId] = name;
                }
            }

            // Build target name→PathID lookups (Binary-safe)
            foreach (var info in _targetFile.file.Metadata.AssetInfos)
            {
                if (info.TypeId == (int)AssetClassID.Transform || info.TypeId == (int)AssetClassID.RectTransform)
                {
                    long goPathId = GetTransformHostGoPathIdRaw(_targetFile, info.PathId);
                    string? name = GetAssetNameRaw(_targetFile, goPathId);
                    if (name != null && !_targetBoneNameToPathId.ContainsKey(name))
                    {
                        _targetBoneNameToPathId[name] = info.PathId;
                    }
                }
                else if (info.TypeId == (int)AssetClassID.GameObject)
                {
                    string? name = GetAssetNameRaw(_targetFile, info.PathId);
                    if (name != null && !_targetGameObjectNameToPathId.ContainsKey(name))
                    {
                        _targetGameObjectNameToPathId[name] = info.PathId;
                    }
                }
                else if (info.TypeId == (int)AssetClassID.MonoScript)
                {
                    // Cache MonoScript names for deduplication (Binary safe for assets without TypeTrees)
                    string? name = GetAssetNameRaw(_targetFile, info.PathId);
                    if (!string.IsNullOrEmpty(name) && !_targetScriptNameToPathId.ContainsKey(name))
                    {
                        _targetScriptNameToPathId[name] = info.PathId;
                    }
                }
            }
        }

        private long GetTransformHostGoPathIdRaw(AssetsFileInstance fileInst, long transformPathId)
        {
            var info = fileInst.file.GetAssetInfo(transformPathId);
            if (info == null) return 0;
            byte[] raw = GetAssetRawBytes(fileInst, info);
            if (raw == null || raw.Length < 12) return 0;
            return BitConverter.ToInt64(raw, 4); // Host GO PathID
        }

        private string? GetAssetNameRaw(AssetsFileInstance fileInst, long pathId)
        {
            if (pathId == 0) return null;
            var info = fileInst.file.GetAssetInfo(pathId);
            if (info == null) return null;
            byte[] raw = GetAssetRawBytes(fileInst, info);
            if (raw == null || raw.Length < 4) return null;

            try
            {
                int len = BitConverter.ToInt32(raw, 0);
                if (len < 0 || len > 2048 || len + 4 > raw.Length) return null;
                return Encoding.UTF8.GetString(raw, 4, len);
            } catch { return null; }
        }

        /// <summary>
        /// Get the name of the GameObject that owns a Transform.
        /// </summary>
        private string? GetTransformGameObjectName(AssetsFileInstance fileInst, long transformPathId)
        {
            try
            {
                var tfmBf = _workspace.GetBaseField(fileInst, transformPathId);
                if (tfmBf == null) return null;

                long goPathId = tfmBf["m_GameObject"]["m_PathID"].AsLong;
                if (goPathId == 0) return null;

                var goBf = _workspace.GetBaseField(fileInst, goPathId);
                if (goBf == null) return null;

                return goBf["m_Name"].AsString;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Try to remap an undiscovered source PathID by finding a same-named bone in the target.
        /// Returns the target PathID if found, or 0 if no match.
        /// </summary>
        private long TryRemapByBoneName(long sourcePathId, bool isGameObject)
        {
            if (isGameObject)
            {
                if (_sourceGameObjectNameCache.TryGetValue(sourcePathId, out string name))
                {
                    if (_targetGameObjectNameToPathId.TryGetValue(name, out long result)) return result;
                }
            }
            else
            {
                if (_sourceTransformNameCache.TryGetValue(sourcePathId, out string name))
                {
                    if (_targetBoneNameToPathId.TryGetValue(name, out long result)) return result;
                }
            }
            return 0;
        }

        /// <summary>
        /// Scans a field tree for internal PPtrs (fileId==0) and enqueues referenced PathIDs
        /// that are NOT already discovered, filtering out Transform/GameObject.
        /// </summary>
        private void ScanFieldForDataDependencies(AssetTypeValueField field, Queue<long> queue)
        {
            if (field.Children == null) return;

            bool isPtr = field.Children.Count >= 2 &&
                         field.Children[0].TemplateField.Name == "m_FileID" &&
                         field.Children[1].TemplateField.Name == "m_PathID";

            if (isPtr)
            {
                int fileId = field.Children[0].AsInt;
                long pathId = field.Children[1].AsLong;

                if (fileId == 0 && pathId != 0 && !_discoveredPathIds.Contains(pathId))
                {
                    queue.Enqueue(pathId);
                }
                return;
            }

            foreach (var child in field.Children)
            {
                ScanFieldForDataDependencies(child, queue);
            }
        }

        /// <summary>
        /// Scans an asset's raw binary payload linearly for 12-byte blocks that look like (int fileId = 0, long pathId).
        /// Necessary because PPtrs inside unmapped MonoBehaviours are completely invisible to the structured parser
        /// if the target file has no TypeTree or has an obfuscated script.
        /// </summary>
        private void ScanRawBytesForDataDependencies(byte[] rawData, Queue<long> queue)
        {
            if (rawData == null || rawData.Length < 12) return;

            // We scan in 4-byte aligned steps, looking for an int32 '0' (m_FileID)
            // followed directly by an int64 'm_PathID'.
            for (int i = 0; i <= rawData.Length - 12; i += 4)
            {
                int fileId = BitConverter.ToInt32(rawData, i);
                
                if (fileId == 0)
                {
                    long pathId = BitConverter.ToInt64(rawData, i + 4);

                    // Plausibility check: PathID shouldn't be zero, and should exist in the source file.
                    if (pathId != 0 && !_discoveredPathIds.Contains(pathId))
                    {
                        // To prevent garbage matches (since zero is extremely common), 
                        // we must strictly verify the PathID actually exists in the source file.
                        var info = _sourceFile.file.GetAssetInfo(pathId);
                        if (info != null)
                        {
                            queue.Enqueue(pathId);
                        }
                    }
                }
            }
        }

        // ==================== PHASE 2: ALLOCATION ====================

        private void AllocateTargetPathIds()
        {
            long nextPathId = 1;
            if (_targetFile.file.Metadata.AssetInfos.Count > 0)
            {
                nextPathId = _targetFile.file.Metadata.AssetInfos.Max(a => a.PathId) + 1;
            }

            foreach (var sourcePathId in _discoveredPathIds)
            {
                _remapTable[sourcePathId] = nextPathId++;
            }
        }

        // ==================== PHASE 3: ASSET CREATION ====================

        private void CreateInitialTargetAssets()
        {
            var targetInfos = (System.Collections.ObjectModel.RangeObservableCollection<AssetFileInfo>)_targetFile.file.Metadata.AssetInfos;

            foreach (var sourcePathId in _discoveredPathIds)
            {
                long targetPathId = _remapTable[sourcePathId];
                
                var sourceInfo = _sourceFile.file.GetAssetInfo(sourcePathId);
                if (sourceInfo == null) continue;

                // Try to deserialize source baseField
                AssetTypeValueField? sourceBf = null;
                try { sourceBf = _workspace.Manager.GetBaseField(_sourceFile, sourceInfo); }
                catch { /* deserialization failed, will use raw bytes */ }

                byte[] rawDataFallback = null;
                ushort scriptIdx = 0xFFFF;
                if (sourceInfo.TypeId == (int)AssetClassID.MonoBehaviour)
                {
                    ushort sourceScriptIdx = sourceInfo.ScriptTypeIndex;
                    if (sourceBf != null && !sourceBf.IsDummy)
                    {
                        var scriptPtr = sourceBf["m_Script"];
                        if (scriptPtr != null && !scriptPtr.IsDummy)
                        {
                            scriptIdx = GetScriptIdxInTarget(scriptPtr["m_FileID"].AsInt, scriptPtr["m_PathID"].AsLong);
                            
                            if (scriptIdx != 0xFFFF)
                            {
                                var targetScriptType = _targetFile.file.Metadata.ScriptTypes[scriptIdx];
                                scriptPtr["m_FileID"].AsInt = targetScriptType.FileId;
                                scriptPtr["m_PathID"].AsLong = targetScriptType.PathId;
                            }

                            // Inject TypeTree so target file knows how to read this script
                            InjectTypeTree(sourceInfo.TypeId, sourceScriptIdx, scriptIdx);
                        }
                    }
                    else
                    {
                        // Fallback: Extract m_Script pointer directly from raw binary
                        rawDataFallback = GetSourceAssetRawBytes(sourceInfo);
                        if (rawDataFallback != null && rawDataFallback.Length >= 28)
                        {
                            int scriptFileId = BitConverter.ToInt32(rawDataFallback, 16);
                            long scriptPathId = BitConverter.ToInt64(rawDataFallback, 20);
                            scriptIdx = GetScriptIdxInTarget(scriptFileId, scriptPathId);
                            
                            if (scriptIdx != 0xFFFF)
                            {
                                var targetScriptType = _targetFile.file.Metadata.ScriptTypes[scriptIdx];
                                Buffer.BlockCopy(BitConverter.GetBytes(targetScriptType.FileId), 0, rawDataFallback, 16, 4);
                                Buffer.BlockCopy(BitConverter.GetBytes(targetScriptType.PathId), 0, rawDataFallback, 20, 8);
                            }

                            // Even if structured read failed, we might find a TypeTree in metadata to inject
                            InjectTypeTree(sourceInfo.TypeId, sourceScriptIdx, scriptIdx);
                        }
                    }
                }

                var baseInfo = AssetFileInfo.Create(
                    _targetFile.file,
                    targetPathId,
                    sourceInfo.TypeId,
                    scriptIdx,
                    _workspace.Manager.ClassDatabase,
                    false
                );

                if (baseInfo == null) continue;

                var newAsset = new AssetInst(_targetFile, baseInfo);

                // Track the transfer for the report
                string assetName = "";
                if (sourceBf != null && !sourceBf["m_Name"].IsDummy)
                    assetName = sourceBf["m_Name"].AsString;

                if (string.IsNullOrEmpty(assetName))
                {
                    if (sourceInfo.TypeId == (int)AssetClassID.GameObject)
                        assetName = GetAssetNameRaw(_sourceFile, sourcePathId) ?? "GameObject";
                    else if (sourceInfo.TypeId == (int)AssetClassID.Transform || sourceInfo.TypeId == (int)AssetClassID.RectTransform)
                        assetName = GetTransformGameObjectName(_sourceFile, sourcePathId) ?? "Transform";
                    else
                        assetName = "Untitled";
                }

                string typeName = ((AssetClassID)sourceInfo.TypeId).ToString();
                if (sourceInfo.TypeId == (int)AssetClassID.MonoBehaviour && scriptIdx != 0xFFFF)
                {
                    var scriptPtr = sourceBf?["m_Script"];
                    if (scriptPtr != null && !scriptPtr.IsDummy)
                    {
                        var scriptExt = _workspace.Manager.GetExtAsset(_targetFile, scriptPtr["m_FileID"].AsInt, scriptPtr["m_PathID"].AsLong);
                        if (scriptExt.baseField != null) typeName = scriptExt.baseField["m_Name"].AsString;
                    }
                }

                _transferRecords.Add(new TransferRecord
                {
                    Name = assetName,
                    Type = typeName,
                    SourcePathId = sourcePathId,
                    TargetPathId = targetPathId
                });

                if (sourceBf != null && !_truncatedPathIds.Contains(sourcePathId))
                {
                    // Use structured baseField serialization (preserves field structure)
                    newAsset.UpdateAssetDataAndRow(_workspace, sourceBf);
                }
                else
                {
                    // Truncated asset or no TypeTree: use raw bytes to preserve custom data
                    byte[] rawData = rawDataFallback ?? GetSourceAssetRawBytes(sourceInfo);
                    if (rawData != null && rawData.Length > 0)
                    {
                        newAsset.UpdateAssetDataAndRow(_workspace, rawData);
                    }
                }
                
                targetInfos.Add(newAsset);
            }
            
            _targetFile.file.GenerateQuickLookup();
        }

        private void FinalSweepRemap()
        {
            foreach (var targetPathId in _remapTable.Values)
            {
                var assetInst = _workspace.GetAssetInst(_targetFile, 0, targetPathId);
                if (assetInst == null) continue;

                var bf = _workspace.GetBaseField(assetInst);
                if (bf == null || bf.IsDummy) continue;

                if (DeepRemapField(bf))
                {
                    assetInst.UpdateAssetDataAndRow(_workspace, bf);
                }
            }
        }

        private bool DeepRemapField(AssetTypeValueField field)
        {
            bool modified = false;
            
            // 1. Universal Value-Based Remapping (Name-Agnostic)
            // Scans any field that contains a long/ulong value. If it matches a known source PathID, swap it.
            if (field.Value != null)
            {
                var vt = field.Value.ValueType;
                if (vt == AssetValueType.Int64 || vt == AssetValueType.UInt64)
                {
                    long val = field.Value.AsLong;
                    if (val != 0 && _remapTable.TryGetValue(val, out long targetPathId))
                    {
                        if (val != targetPathId)
                        {
                            field.Value.AsLong = targetPathId;
                            modified = true;
                        }
                    }
                }
            }

            if (field.Children == null || field.Children.Count == 0) return modified;

            // Security: Avoid remapping m_Script values as they are handled by specialized logic
            if (field.TemplateField.Name.Equals("m_Script", StringComparison.OrdinalIgnoreCase)) return modified;

            // 2. Structural PPtr Fallback (for bone name remapping which requires context)
            bool isPtr = field.Children.Count == 2 &&
                         (field.Children[0].Value?.ValueType == AssetValueType.Int32 || field.Children[0].Value?.ValueType == AssetValueType.UInt32) &&
                         (field.Children[1].Value?.ValueType == AssetValueType.Int64 || field.Children[1].Value?.ValueType == AssetValueType.UInt64);

            if (isPtr)
            {
                int fileId = field.Children[0].AsInt;
                long pathId = field.Children[1].AsLong;

                if (fileId == 0 && pathId != 0)
                {
                    // If universal check on the m_PathID child didn't result in a hit (not in remapTable),
                    // we try bone name recovery which requires the context of the PPtr field name.
                    if (!_remapTable.ContainsKey(pathId))
                    {
                        bool isGo = field.TemplateField.Name.Contains("GameObject", StringComparison.OrdinalIgnoreCase);
                        long remappedId = TryRemapByBoneName(pathId, isGo);
                        if (remappedId != 0 && pathId != remappedId)
                        {
                            field.Children[1].AsLong = remappedId;
                            modified = true;
                        }
                    }
                }
            }

            foreach (var child in field.Children)
            {
                if (DeepRemapField(child)) modified = true;
            }
            return modified;
        }

        // ==================== PHASE 4: REMAPPING ====================

        private void RemapTargetAssets()
        {
            foreach (var sourcePathId in _discoveredPathIds)
            {
                long targetPathId = _remapTable[sourcePathId];
                var targetAsset = _workspace.GetAssetInst(_targetFile, 0, targetPathId);
                if (targetAsset == null) continue;

                var targetBf = _workspace.GetBaseField(targetAsset);
                bool modified = false;

                if (targetBf != null && !targetBf.IsDummy)
                {
                    modified = RemapPPtrsInField(targetBf, sourcePathId);

                    if (AppendResourceData(targetBf, sourcePathId))
                    {
                        modified = true;
                    }

                    if (modified)
                    {
                        if (_truncatedPathIds.Contains(sourcePathId) && targetAsset.TypeId == (int)AssetClassID.MonoBehaviour)
                        {
                            byte[] remappedBytes = targetBf.WriteToByteArray();
                            var sourceInfo = _sourceFile.file.GetAssetInfo(sourcePathId);
                            byte[] originalRawBytes = GetSourceAssetRawBytes(sourceInfo);

                            if (originalRawBytes != null && remappedBytes.Length <= originalRawBytes.Length)
                            {
                                byte[] finalBytes = new byte[originalRawBytes.Length];
                                Buffer.BlockCopy(remappedBytes, 0, finalBytes, 0, remappedBytes.Length);
                                Buffer.BlockCopy(originalRawBytes, remappedBytes.Length, finalBytes, remappedBytes.Length, originalRawBytes.Length - remappedBytes.Length);
                                targetAsset.UpdateAssetDataAndRow(_workspace, finalBytes);
                            }
                            else { targetAsset.UpdateAssetDataAndRow(_workspace, targetBf); }
                        }
                        else { targetAsset.UpdateAssetDataAndRow(_workspace, targetBf); }
                    }
                }
                else if (_truncatedPathIds.Contains(sourcePathId) && targetAsset.TypeId == (int)AssetClassID.MonoBehaviour)
                {
                    var sourceInfo = _sourceFile.file.GetAssetInfo(sourcePathId);
                    if (sourceInfo != null) { CopyTruncatedMonoBehaviourSafe(targetAsset, sourceInfo); }
                }
            }

            var wsItem = _workspace.FindWorkspaceItemByInstance(_targetFile);
            if (wsItem != null) _workspace.Dirty(wsItem);
        }

        private bool CopyTruncatedMonoBehaviourSafe(AssetInst targetInst, AssetFileInfo sourceInfo)
        {
            byte[] sourceRawData = GetSourceAssetRawBytes(sourceInfo);
            if (sourceRawData == null || sourceRawData.Length < 28) return false;

            byte[] patchedBytes = new byte[sourceRawData.Length];
            Buffer.BlockCopy(sourceRawData, 0, patchedBytes, 0, sourceRawData.Length);

            int goFileId = BitConverter.ToInt32(patchedBytes, 0);
            long goPathId = BitConverter.ToInt64(patchedBytes, 4);
            if (goFileId == 0 && goPathId != 0 && _remapTable.TryGetValue(goPathId, out long newGoPathId))
            {
                Buffer.BlockCopy(BitConverter.GetBytes(newGoPathId), 0, patchedBytes, 4, 8);
            }

            int scriptFileId = BitConverter.ToInt32(patchedBytes, 16);
            long scriptPathId = BitConverter.ToInt64(patchedBytes, 20);
            int targetScriptFileId = scriptFileId;
            long targetScriptPathId = scriptPathId;

            if (scriptFileId == 0 && scriptPathId != 0 && _remapTable.TryGetValue(scriptPathId, out long newScriptPathId))
            {
                targetScriptPathId = newScriptPathId;
            }
            else if (scriptFileId != 0) { targetScriptFileId = MapExternalFileId(scriptFileId); }

            Buffer.BlockCopy(BitConverter.GetBytes(targetScriptFileId), 0, patchedBytes, 16, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(targetScriptPathId), 0, patchedBytes, 20, 8);

            int nameLength = 0;
            int scanStartIndex = 28;
            if (patchedBytes.Length >= 32)
            {
                nameLength = BitConverter.ToInt32(patchedBytes, 28);
                if (nameLength >= 0 && nameLength < 1024 && (32 + nameLength <= patchedBytes.Length))
                {
                    int alignedStringLength = (nameLength + 3) & ~3;
                    scanStartIndex = 32 + alignedStringLength; 
                }
            }

            for (int i = scanStartIndex; i <= patchedBytes.Length - 12; i += 4)
            {
                int fileId = BitConverter.ToInt32(patchedBytes, i);
                if (fileId == 0)
                {
                    long pathId = BitConverter.ToInt64(patchedBytes, i + 4);
                    if (pathId != 0)
                    {
                        if (_remapTable.TryGetValue(pathId, out long newPathId))
                        {
                            Buffer.BlockCopy(BitConverter.GetBytes(newPathId), 0, patchedBytes, i + 4, 8);
                            i += 8;
                        }
                        else
                        {
                            long remappedBoneId = TryRemapByBoneName(pathId, false);
                            if (remappedBoneId != 0)
                            {
                                Buffer.BlockCopy(BitConverter.GetBytes(remappedBoneId), 0, patchedBytes, i + 4, 8);
                                i += 8;
                            }
                        }
                    }
                }
            }

            targetInst.UpdateAssetDataAndRow(_workspace, patchedBytes);
            return true;
        }

        private bool RemapPPtrsInField(AssetTypeValueField field, long sourceAssetPathId)
        {
            bool modified = false;
            if (field.Children == null || field.Children.Count == 0) return false;

            // Name-agnostic PPtr identification: 2 children, (int/uint, long/ulong)
            // This ensures remapping works even on obfuscated or version-mismatched snapshots.
            bool isPtr = field.Children.Count == 2 &&
                         (field.Children[0].Value?.ValueType == AssetValueType.Int32 || field.Children[0].Value?.ValueType == AssetValueType.UInt32) &&
                         (field.Children[1].Value?.ValueType == AssetValueType.Int64 || field.Children[1].Value?.ValueType == AssetValueType.UInt64);

            if (isPtr)
            {
                // Identification by name for special rules, but remapping is structure-based
                string fieldName = field.TemplateField.Name;
                if (fieldName == "m_Script" || field.Children[0].TemplateField.Name.Equals("m_Script", StringComparison.OrdinalIgnoreCase)) 
                    return false; // Scripts are handled separately

                int fileId = field.Children[0].AsInt;
                long pathId = field.Children[1].AsLong;

                if (fileId != 0)
                {
                    int newFileId = MapExternalFileId(fileId);
                    if (newFileId != fileId)
                    {
                        field.Children[0].AsInt = newFileId;
                        return true;
                    }
                    return false;
                }

                if (fileId == 0 && pathId != 0)
                {
                    if (sourceAssetPathId == _rootTransformPathId && 
                        (field.TemplateField.Name == "m_Father" || field.Children[0].TemplateField.Name.Equals("m_Father", StringComparison.OrdinalIgnoreCase)))
                    {
                        field.Children[1].AsLong = 0;
                        return true;
                    }
                    if (field.TemplateField.Name == "m_CorrespondingSourceObject" || field.TemplateField.Name == "m_PrefabInstance")
                    {
                        field.Children[1].AsLong = 0;
                        return true;
                    }
                    if (_remapTable.TryGetValue(pathId, out long newPathId))
                    {
                        if (pathId != newPathId)
                        {
                            field.Children[1].AsLong = newPathId;
                            return true;
                        }
                    }
                    else
                    {
                        // Smart recovery for bones or GameObjects that weren't in the tree
                        bool isGo = field.TemplateField.Name.Contains("GameObject", StringComparison.OrdinalIgnoreCase);
                        long remappedId = TryRemapByBoneName(pathId, isGo);
                        if (remappedId != 0)
                        {
                            field.Children[1].AsLong = remappedId; 
                            return true;
                        }
                    }
                }
                return false;
            }

            foreach (var child in field.Children)
            {
                if (RemapPPtrsInField(child, sourceAssetPathId)) modified = true;
            }
            return modified;
        }

        private ushort GetScriptIdxInTarget(int sourceFileId, long sourcePathId)
        {
            var scriptExt = _workspace.Manager.GetExtAsset(_sourceFile, sourceFileId, sourcePathId);
            if (scriptExt.baseField == null || scriptExt.baseField.IsDummy) return 0xFFFF;
            string scriptName = scriptExt.baseField["m_Name"].AsString;
            
            for (int i = 0; i < _targetFile.file.Metadata.ScriptTypes.Count; i++)
            {
                var type = _targetFile.file.Metadata.ScriptTypes[i];
                if (IsScriptNamed(_targetFile, type.FileId, type.PathId, scriptName))
                {
                    var depInst = _targetFile;
                    if (type.FileId != 0) depInst = _targetFile.GetDependency(_workspace.Manager, type.FileId - 1);
                    if (depInst != null)
                    {
                        var targetScriptAsset = _workspace.GetAssetInst(depInst, 0, type.PathId);
                        if (targetScriptAsset != null) targetScriptAsset.UpdateAssetDataAndRow(_workspace, scriptExt.baseField);
                    }
                    return (ushort)i;
                }
            }

            var newPtr = new AssetPPtr();
            if (sourceFileId == 0 && _remapTable.TryGetValue(sourcePathId, out long newPathId)) { newPtr.FileId = 0; newPtr.PathId = newPathId; }
            else { newPtr.FileId = MapExternalFileId(sourceFileId); newPtr.PathId = sourcePathId; }
            _targetFile.file.Metadata.ScriptTypes.Add(newPtr);
            return (ushort)(_targetFile.file.Metadata.ScriptTypes.Count - 1);
        }

        private void InjectTypeTree(int typeId, ushort sourceScriptIdx, ushort targetScriptIdx)
        {
            if (typeId != (int)AssetClassID.MonoBehaviour && typeId >= 0) return;
            var sourceTtt = _sourceFile.file.Metadata.TypeTreeTypes.FirstOrDefault(t => t.TypeId == typeId && t.ScriptTypeIndex == sourceScriptIdx);
            if (sourceTtt == null || sourceTtt.Nodes == null || sourceTtt.Nodes.Count == 0) return;
            if (_targetFile.file.Metadata.TypeTreeTypes.Any(t => t.TypeId == typeId && t.ScriptTypeIndex == targetScriptIdx)) return;

            // 【彻底修复 另存为崩溃 (NullReferenceException) 的核心代码】
            var newTtt = new TypeTreeType
            {
                TypeId = typeId,
                IsRefType = sourceTtt.IsRefType,
                ScriptTypeIndex = targetScriptIdx,
                ScriptIdHash = sourceTtt.ScriptIdHash,
                TypeHash = sourceTtt.TypeHash,
                Nodes = new List<TypeTreeNode>(sourceTtt.Nodes),
                StringBufferBytes = sourceTtt.StringBufferBytes != null ? (byte[])sourceTtt.StringBufferBytes.Clone() : Array.Empty<byte>(),
                TypeDependencies = sourceTtt.TypeDependencies != null ? (int[])sourceTtt.TypeDependencies.Clone() : Array.Empty<int>() // 必须克隆防止为空
            };
            _targetFile.file.Metadata.TypeTreeTypes.Add(newTtt);
            _targetFile.file.Metadata.TypeTreeEnabled = true;
        }

        private int MapExternalFileId(int sourceFileId)
        {
            if (sourceFileId <= 0) return sourceFileId;
            if (_externalRemapTable.TryGetValue(sourceFileId, out int existingTgtFileId)) return existingTgtFileId;
            int sourceIdx = sourceFileId - 1;
            if (sourceIdx >= _sourceFile.file.Metadata.Externals.Count) return sourceFileId;

            var sourceExt = _sourceFile.file.Metadata.Externals[sourceIdx];
            string sourcePathName = Path.GetFileName(sourceExt.PathName);
            for (int i = 0; i < _targetFile.file.Metadata.Externals.Count; i++)
            {
                var targetExt = _targetFile.file.Metadata.Externals[i];
                if (Path.GetFileName(targetExt.PathName).Equals(sourcePathName, StringComparison.OrdinalIgnoreCase))
                {
                    _externalRemapTable[sourceFileId] = i + 1;
                    return i + 1;
                }
            }

            var newExt = new AssetsFileExternal { PathName = sourceExt.PathName, OriginalPathName = sourceExt.OriginalPathName, Type = sourceExt.Type, Guid = sourceExt.Guid };
            _targetFile.file.Metadata.Externals.Add(newExt);
            int newFileId = _targetFile.file.Metadata.Externals.Count;
            _externalRemapTable[sourceFileId] = newFileId;
            return newFileId;
        }

        private void UpdateAssetBundlePreloadTable()
        {
            AssetFileInfo? abInfo = null;
            foreach (var info in _targetFile.file.Metadata.AssetInfos) { if (info.TypeId == (int)AssetClassID.AssetBundle) { abInfo = info; break; } }
            if (abInfo == null) return;

            var abAsset = _workspace.GetAssetInst(_targetFile, 0, abInfo.PathId);
            if (abAsset == null) return;
            var abBf = _workspace.GetBaseField(abAsset);
            if (abBf == null) return;

            var preloadTable = abBf["m_PreloadTable"]["Array"];
            if (preloadTable.IsDummy) return;

            var newEntries = new List<AssetTypeValueField>();
            foreach (var sourcePathId in _discoveredPathIds)
            {
                if (_remapTable.TryGetValue(sourcePathId, out long targetPathId))
                {
                    var pptrTemplate = preloadTable.Children.Count > 0 ? ValueBuilder.DefaultValueFieldFromTemplate(preloadTable.Children[0].TemplateField) : null;
                    if (pptrTemplate != null) { pptrTemplate["m_FileID"].AsInt = 0; pptrTemplate["m_PathID"].AsLong = targetPathId; newEntries.Add(pptrTemplate); }
                }
            }
            if (newEntries.Count == 0) return;
            foreach (var entry in newEntries) { preloadTable.Children.Add(entry); }

            var container = abBf["m_Container"]["Array"];
            if (!container.IsDummy && container.Children != null)
            {
                foreach (var item in container.Children)
                {
                    var second = item["second"];
                    if (second != null && !second.IsDummy) { second["preloadSize"].AsInt = preloadTable.Children.Count; }
                }
            }
            abAsset.UpdateAssetDataAndRow(_workspace, abBf);
        }

        private Dictionary<string, MemoryStream> _targetResSStreams = new();
        private Dictionary<string, byte[]> _sourceResSCache = new();

        private MemoryStream GetTargetResSStream(string resName)
        {
            if (_targetResSStreams.TryGetValue(resName, out var ms)) return ms;
            ms = new MemoryStream();
            var bundleInst = _targetFile.parentBundle;
            if (bundleInst != null)
            {
                var dirInfo = bundleInst.file.BlockAndDirInfo.DirectoryInfos.FirstOrDefault(d => d.Name.Equals(resName, StringComparison.OrdinalIgnoreCase));
                if (dirInfo == null) dirInfo = bundleInst.file.BlockAndDirInfo.DirectoryInfos.FirstOrDefault(d => d.Name.EndsWith(".resS", StringComparison.OrdinalIgnoreCase));
                if (dirInfo != null)
                {
                    byte[] existingData = BundleHelper.LoadAssetDataFromBundle(bundleInst.file, dirInfo.Name);
                    if (existingData != null && existingData.Length > 0) { ms.Write(existingData, 0, existingData.Length); }
                }
            }
            long remainder = ms.Length % 16;
            if (remainder > 0) ms.Write(new byte[16 - remainder], 0, (int)(16 - remainder));
            _targetResSStreams[resName] = ms;
            return ms;
        }

        private void ApplyResSChanges()
        {
            var bundleInst = _targetFile.parentBundle;
            if (bundleInst == null || _targetResSStreams.Count == 0) return;
            foreach (var kvp in _targetResSStreams)
            {
                string resName = kvp.Key;
                byte[] newData = kvp.Value.ToArray();
                var dirInfo = bundleInst.file.BlockAndDirInfo.DirectoryInfos.FirstOrDefault(d => d.Name.Equals(resName, StringComparison.OrdinalIgnoreCase));
                if (dirInfo == null) dirInfo = bundleInst.file.BlockAndDirInfo.DirectoryInfos.FirstOrDefault(d => d.Name.EndsWith(".resS", StringComparison.OrdinalIgnoreCase));
                if (dirInfo != null)
                {
                    dirInfo.SetNewData(newData);
                    var bunItem = _workspace.FindWorkspaceItemByInstance(bundleInst);
                    if (bunItem != null)
                    {
                        var child = bunItem.Children.FirstOrDefault(c => c.Name.Equals(dirInfo.Name, StringComparison.OrdinalIgnoreCase));
                        if (child != null) _workspace.Dirty(child);
                    }
                }
            }
        }

        private bool AppendResourceData(AssetTypeValueField targetBf, long sourcePathId)
        {
            AssetTypeValueField? streamData = targetBf.Get("m_StreamData");
            if (streamData == null || streamData.IsDummy) streamData = targetBf.Get("m_Resource");
            if (streamData == null || streamData.IsDummy) return false;

            string pathField = streamData.Get("path") != null && !streamData.Get("path").IsDummy ? "path" : "m_Source";
            string offsetField = streamData.Get("offset") != null && !streamData.Get("offset").IsDummy ? "offset" : "m_Offset";
            string sizeField = streamData.Get("size") != null && !streamData.Get("size").IsDummy ? "size" : "m_Size";

            long size = streamData[sizeField].AsLong;
            string sourcePath = streamData[pathField].AsString;
            if (size <= 0 || string.IsNullOrEmpty(sourcePath)) return false;
            long sourceOffset = streamData[offsetField].AsLong;

            byte[]? rawData = ReadFromSourceResS(sourcePath, sourceOffset, size);
            if (rawData == null || rawData.Length == 0) return false;

            var targetBundle = _targetFile.parentBundle;
            if (targetBundle == null) return false;

            string assetsContainerName = _targetFile.name;
            string targetResSName = Path.GetFileNameWithoutExtension(assetsContainerName) + ".resS";
            var ms = GetTargetResSStream(targetResSName);
            ms.Seek(0, SeekOrigin.End);
            long newOffset = ms.Position;
            ms.Write(rawData, 0, rawData.Length);

            long remainder = ms.Length % 16;
            if (remainder > 0) ms.Write(new byte[16 - remainder], 0, (int)(16 - remainder));

            streamData[sizeField].AsLong = rawData.Length;
            streamData[offsetField].AsLong = newOffset;
            streamData[pathField].AsString = "archive:/" + assetsContainerName + "/" + targetResSName;

            var imgData = targetBf.Get("image data");
            if (imgData != null && !imgData.IsDummy) imgData.AsByteArray = Array.Empty<byte>();

            var vData = targetBf.Get("m_VertexData");
            if (vData != null && !vData.IsDummy)
            {
                var dSize = vData.Get("m_DataSize");
                if (dSize != null && !dSize.IsDummy) dSize.AsByteArray = Array.Empty<byte>();
            }
            return true;
        }

        private byte[]? ReadFromSourceResS(string path, long offset, long size)
        {
            if (size == 0 || string.IsNullOrEmpty(path)) return null;
            if (_sourceFile.parentBundle != null)
            {
                string archiveName = path.StartsWith("archive:/") ? path.Substring("archive:/".Length) : Path.GetFileName(path.TrimStart('/', '\\'));
                if (!_sourceResSCache.TryGetValue(archiveName, out byte[]? fullResSData))
                {
                    var bundle = _sourceFile.parentBundle.file;
                    var dirInfo = bundle.BlockAndDirInfo.DirectoryInfos.FirstOrDefault(i => i.Name.Equals(archiveName, StringComparison.OrdinalIgnoreCase) || i.Name.EndsWith(".resS", StringComparison.OrdinalIgnoreCase));
                    if (dirInfo != null) { fullResSData = BundleHelper.LoadAssetDataFromBundle(bundle, dirInfo.Name); _sourceResSCache[archiveName] = fullResSData ?? Array.Empty<byte>(); }
                    else { _sourceResSCache[archiveName] = Array.Empty<byte>(); }
                }
                if (fullResSData != null && fullResSData.Length > 0 && offset + size <= fullResSData.Length) { byte[] result = new byte[size]; Array.Copy(fullResSData, offset, result, 0, size); return result; }
            }
            var rootPath = Path.GetDirectoryName(_sourceFile.path);
            var fixedPath = path.StartsWith("archive:/") ? path.Substring("archive:/".Length) : path;
            if (!Path.IsPathRooted(fixedPath) && rootPath != null) fixedPath = Path.Combine(rootPath, fixedPath);
            if (File.Exists(fixedPath))
            {
                using var fs = File.OpenRead(fixedPath);
                if (offset + size <= fs.Length) { fs.Position = offset; var data = new byte[(int)size]; int bytesRead = fs.Read(data, 0, (int)size); if (bytesRead == (int)size) return data; }
            }
            return null;
        }

        public void ForceUpgradeAllScripts()
        {
            var templatesA = GetScriptTemplates(_sourceFile);
            var templatesB = GetScriptTemplates(_targetFile);

            var masterTemplates = new Dictionary<long, GoldenScriptTemplate>();
            var scriptsToUpgrade = new HashSet<long>();
            var injectMapFromA = new Dictionary<long, GoldenScriptTemplate>();

            foreach (var kvp in templatesA)
            {
                long pathId = kvp.Key;
                var tempA = kvp.Value;

                if (templatesB.TryGetValue(pathId, out var tempB))
                {
                    int nodesA = tempA.TypeTree.Nodes.Count;
                    int nodesB = tempB.TypeTree.Nodes.Count;

                    if (nodesA > nodesB)
                    {
                        masterTemplates[pathId] = tempA;
                        scriptsToUpgrade.Add(pathId);
                        injectMapFromA[pathId] = tempA;
                    }
                    else if (nodesB > nodesA)
                    {
                        masterTemplates[pathId] = tempB;
                        scriptsToUpgrade.Add(pathId);
                    }
                    else
                    {
                        bool dataSame = CompareMonoScripts(tempA.MonoScriptBaseField, tempB.MonoScriptBaseField);
                        if (!dataSame)
                        {
                            masterTemplates[pathId] = tempA;
                            scriptsToUpgrade.Add(pathId);
                            injectMapFromA[pathId] = tempA;
                        }
                        else
                        {
                            masterTemplates[pathId] = tempB;
                        }
                    }
                }
                else
                {
                    masterTemplates[pathId] = tempA;
                    scriptsToUpgrade.Add(pathId);
                    injectMapFromA[pathId] = tempA;
                }
            }

            if (masterTemplates.Count == 0) return;

            var reverseRemap = new Dictionary<long, long>();
            foreach (var kvp in _remapTable) reverseRemap[kvp.Value] = kvp.Key;

            var targetAssets = new List<UpgradeInfo>();
            foreach (var info in _targetFile.file.Metadata.AssetInfos)
            {
                if (info.TypeId == (int)AssetClassID.MonoBehaviour)
                {
                    var assetInst = _workspace.GetAssetInst(_targetFile, 0, info.PathId);
                    if (assetInst != null)
                    {
                        long pathId = 0;
                        var bf = _workspace.GetBaseField(assetInst);
                        if (bf != null && !bf.IsDummy)
                        {
                            pathId = bf["m_Script"]?["m_PathID"].AsLong ?? 0;
                        }
                        else
                        {
                            byte[] raw = GetTargetAssetRawBytes(info);
                            if (raw != null && raw.Length >= 28) pathId = BitConverter.ToInt64(raw, 20);
                        }

                        if (masterTemplates.ContainsKey(pathId))
                        {
                            targetAssets.Add(new UpgradeInfo { Info = info, ScriptPathId = pathId });
                        }
                    }
                }
            }

            if (targetAssets.Count == 0) return;

            var snapshotData = new Dictionary<long, AssetTypeValueField?>();
            foreach (var upInfo in targetAssets)
            {
                long targetPathId = upInfo.Info.PathId;
                if (reverseRemap.TryGetValue(targetPathId, out long sourcePathId))
                {
                    // [核心修复] 结构化重排快照 (Strongest Fix)
                    // 如果资产是从源文件 A 移动过来的，我们优先从 A 读取原始数据。
                    // 这样做是因为 A 拥有完整的 TypeTree，能保证 100% 正确读取所有字段（即使 B 是截断的）。
                    var sourceAsset = _workspace.GetAssetInst(_sourceFile, 0, sourcePathId);
                    if (sourceAsset != null)
                    {
                        var bf = _workspace.GetBaseField(sourceAsset);
                        if (bf != null && !bf.IsDummy)
                        {
                            // 在存储快照前，立即在内存中完成 PPtr 重映射（映射到 B 的 PathID 空间）。
                            // 这样在后面的 MergeMatchingFields 时，合并进去的指针就已经是正确的了。
                            RemapPPtrsInField(bf, sourcePathId);
                            snapshotData[targetPathId] = bf;
                        }
                    }
                }
                else
                {
                    // 资产原本就在 B 中，尝试从 B 读取原始数据
                    var targetAsset = _workspace.GetAssetInst(_targetFile, 0, targetPathId);
                    if (targetAsset != null)
                    {
                        var bf = _workspace.GetBaseField(targetAsset);
                        if (bf != null && !bf.IsDummy) snapshotData[targetPathId] = bf;
                    }
                }
            }

            var scriptAndTtIdxMap = new Dictionary<long, (ushort sIdx, ushort ttIdx)>();
            foreach (var kvp in injectMapFromA)
            {
                var indices = InjectGoldenScriptAndTypeTree(kvp.Value.SourceScriptIdx, kvp.Value.TypeTree);
                scriptAndTtIdxMap[kvp.Key] = indices;

                var targetScriptAsset = _workspace.GetAssetInst(_targetFile, 0, kvp.Key);
                if (targetScriptAsset != null)
                {
                    targetScriptAsset.UpdateAssetDataAndRow(_workspace, kvp.Value.MonoScriptBaseField);
                }
            }

            foreach (var kvp in masterTemplates)
            {
                if (!scriptAndTtIdxMap.ContainsKey(kvp.Key) && templatesB.TryGetValue(kvp.Key, out var tb))
                {
                    // Find existing TypeTree index in B
                    int ttIdx = _targetFile.file.Metadata.TypeTreeTypes.FindIndex(t => 
                        t.TypeId == (int)AssetClassID.MonoBehaviour && t.ScriptTypeIndex == tb.SourceScriptIdx);
                    scriptAndTtIdxMap[kvp.Key] = (tb.SourceScriptIdx, (ushort)ttIdx);
                }
            }

            foreach (var upInfo in targetAssets)
            {
                long targetPathId = upInfo.Info.PathId;
                var res = scriptAndTtIdxMap[upInfo.ScriptPathId];
                ushort targetScriptIdx = res.sIdx;
                ushort targetTtIdx = res.ttIdx;

                var assetInst = _workspace.GetAssetInst(_targetFile, 0, targetPathId);
                if (assetInst == null) continue;

                if (!scriptsToUpgrade.Contains(upInfo.ScriptPathId))
                {
                    upInfo.Info.ScriptTypeIndex = targetScriptIdx;
                    assetInst.ScriptTypeIndex = targetScriptIdx;
                    
                    // [修复] 即使不升级数据，也必须确保 TypeTree 索引正确（防止原来指向错误的字典）
                    if (targetTtIdx != 0xFFFF)
                    {
                        upInfo.Info.TypeIdOrIndex = targetTtIdx;
                        assetInst.TypeIdOrIndex = targetTtIdx;
                    }
                    continue;
                }

                var targetTree = _targetFile.file.Metadata.TypeTreeTypes[targetTtIdx];
                var newTemplate = new AssetTypeTemplateField();
                newTemplate.FromTypeTree(targetTree);

                upInfo.Info.ScriptTypeIndex = targetScriptIdx;
                assetInst.ScriptTypeIndex = targetScriptIdx;
                upInfo.Info.TypeIdOrIndex = targetTtIdx;
                assetInst.TypeIdOrIndex = targetTtIdx;

                var newBf = ValueBuilder.DefaultValueFieldFromTemplate(newTemplate);
                
                // [用户建议方案：A→B 记录追踪] 100% 绝对宿主连接
                // 彻底杜绝“孤儿组件”错误。不管是 A 的 PathID 还是名字。
                long targetGoFileId = 0, targetGoPathId = 0;
                if (reverseRemap.TryGetValue(targetPathId, out long sourcePathId))
                {
                    var sourceInfo = _sourceFile.file.GetAssetInfo(sourcePathId);
                    byte[] sourceRaw = GetSourceAssetRawBytes(sourceInfo);
                    if (sourceRaw != null && sourceRaw.Length >= 12)
                    {
                        int srcGoFileId = BitConverter.ToInt32(sourceRaw, 0);
                        long srcGoPathId = BitConverter.ToInt64(sourceRaw, 4);

                        // [核心强化建议] 优先使用 _compToGoMap 进行 100% 宿主重连
                        if (_compToGoMap.TryGetValue(sourcePathId, out long hostGoPathId))
                        {
                            srcGoFileId = 0;
                            srcGoPathId = hostGoPathId;
                        }

                        if (srcGoFileId == 0 && srcGoPathId != 0)
                        {
                            if (_remapTable.TryGetValue(srcGoPathId, out long newGoPathId))
                            {
                                targetGoPathId = newGoPathId;
                            }
                            else
                            {
                                targetGoPathId = TryRemapByBoneName(srcGoPathId, true);
                            }
                        }
                        else if (srcGoFileId != 0)
                        {
                            targetGoFileId = MapExternalFileId(srcGoFileId);
                            targetGoPathId = srcGoPathId;
                        }
                    }
                }
                else
                {
                    // 数据原本就在 B 中，尝试从 B 读取当前的宿主连接
                    byte[] targetRaw = GetTargetAssetRawBytes(upInfo.Info);
                    if (targetRaw != null && targetRaw.Length >= 12)
                    {
                        targetGoFileId = BitConverter.ToInt32(targetRaw, 0);
                        targetGoPathId = BitConverter.ToInt64(targetRaw, 4);
                    }
                }

                if (snapshotData.TryGetValue(targetPathId, out var originalBf) && originalBf != null)
                {
                    MergeMatchingFields(originalBf, newBf);
                }

                // [修复：极致强制重连]
                // 此时 newBf 已经完成了 Merge，我们现在无条件注入正确的 GameObject PathID。
                long forcedGoFileId = targetGoFileId;
                long forcedGoPathId = targetGoPathId;

                // 如果通过 raw 序列或 snapshot 没找到，再次尝试使用 ownership map 兜底 (Source A -> Target B)
                if (forcedGoPathId == 0 && reverseRemap.TryGetValue(targetPathId, out long sId))
                {
                    if (_compToGoMap.TryGetValue(sId, out long hId))
                    {
                        if (_remapTable.TryGetValue(hId, out long nId))
                        {
                            forcedGoFileId = 0;
                            forcedGoPathId = nId;
                        }
                    }
                }

                if (forcedGoPathId != 0)
                {
                    // [究极重连计划]
                    // 我们不仅看名字，还看索引，甚至看内容。
                    // 如果某个 PPtr 指向的是 A 模型的旧宿主 ID，或者它是第一个字段，我们就把它改成 B 模型的新宿主 ID。
                    for (int i = 0; i < newBf.Children.Count; i++)
                    {
                        var child = newBf.Children[i];
                        if (child.Children != null && child.Children.Count == 2)
                        {
                            bool isPtr = (child.Children[0].Value?.ValueType == AssetValueType.Int32 || child.Children[0].Value?.ValueType == AssetValueType.UInt32) &&
                                         (child.Children[1].Value?.ValueType == AssetValueType.Int64 || child.Children[1].Value?.ValueType == AssetValueType.UInt64);
                            
                            if (isPtr)
                            {
                                string name = child.TemplateField.Name;
                                long currentId = child.Children[1].AsLong;

                                if (name.Equals("m_GameObject", StringComparison.OrdinalIgnoreCase) || 
                                    i == 0 || 
                                    (reverseRemap.TryGetValue(targetPathId, out long sid) && _compToGoMap.TryGetValue(sid, out long oid) && currentId == oid))
                                {
                                    child.Children[0].AsInt = (int)forcedGoFileId;
                                    child.Children[1].AsLong = forcedGoPathId;
                                    break; 
                                }
                            }
                        }
                    }
                }

                var goldenScriptType = _targetFile.file.Metadata.ScriptTypes[targetScriptIdx];
                newBf["m_Script"]["m_FileID"].AsInt = goldenScriptType.FileId;
                newBf["m_Script"]["m_PathID"].AsLong = goldenScriptType.PathId;
                
                assetInst.UpdateAssetDataAndRow(_workspace, newBf);
            }
        }

        private Dictionary<long, GoldenScriptTemplate> GetScriptTemplates(AssetsFileInstance fileInst)
        {
            var temps = new Dictionary<long, GoldenScriptTemplate>();
            for (ushort i = 0; i < fileInst.file.Metadata.ScriptTypes.Count; i++)
            {
                var scriptType = fileInst.file.Metadata.ScriptTypes[i];
                var tree = fileInst.file.Metadata.TypeTreeTypes.FirstOrDefault(t => 
                    t.TypeId == (int)AssetClassID.MonoBehaviour && t.ScriptTypeIndex == i);
                
                if (tree == null) continue;

                var scriptExt = _workspace.Manager.GetExtAsset(fileInst, scriptType.FileId, scriptType.PathId);
                if (scriptExt.baseField == null || scriptExt.baseField.IsDummy) continue;

                temps[scriptType.PathId] = new GoldenScriptTemplate
                {
                    SourceScriptIdx = i,
                    TypeTree = tree,
                    MonoScriptBaseField = scriptExt.baseField
                };
            }
            return temps;
        }

        private bool CompareMonoScripts(AssetTypeValueField a, AssetTypeValueField b)
        {
            if (a == null || b == null) return false;
            try
            {
                return a["m_AssemblyName"].AsString == b["m_AssemblyName"].AsString &&
                       a["m_ClassName"].AsString == b["m_ClassName"].AsString &&
                       a["m_Namespace"].AsString == b["m_Namespace"].AsString;
            }
            catch { return false; }
        }

        private class GoldenScriptTemplate
        {
            public ushort SourceScriptIdx;
            public TypeTreeType TypeTree;
            public AssetTypeValueField MonoScriptBaseField;
        }

        private class UpgradeInfo
        {
            public AssetFileInfo Info;
            public long ScriptPathId;
        }

        private (ushort sIdx, ushort ttIdx) InjectGoldenScriptAndTypeTree(ushort sourceScriptIdx, TypeTreeType sourceTree)
        {
            var sourceType = _sourceFile.file.Metadata.ScriptTypes[sourceScriptIdx];
            ushort targetScriptIdx = GetScriptIdxInTarget(sourceType.FileId, sourceType.PathId);

            int ttIdx = _targetFile.file.Metadata.TypeTreeTypes.FindIndex(t => 
                t.TypeId == (int)AssetClassID.MonoBehaviour && t.ScriptTypeIndex == targetScriptIdx);

            if (ttIdx == -1)
            {
                // [核心防空指针修复] 必须克隆 TypeDependencies，防止保存打包时崩溃！
                var newTtt = new TypeTreeType
                {
                    TypeId = (int)AssetClassID.MonoBehaviour,
                    IsStrippedType = false,
                    ScriptTypeIndex = targetScriptIdx,
                    ScriptIdHash = sourceTree.ScriptIdHash,
                    TypeHash = sourceTree.TypeHash,
                    Nodes = new List<TypeTreeNode>(sourceTree.Nodes),
                    StringBufferBytes = sourceTree.StringBufferBytes != null ? (byte[])sourceTree.StringBufferBytes.Clone() : Array.Empty<byte>(),
                    TypeDependencies = sourceTree.TypeDependencies != null ? (int[])sourceTree.TypeDependencies.Clone() : Array.Empty<int>()
                };
                _targetFile.file.Metadata.TypeTreeTypes.Add(newTtt);
                _targetFile.file.Metadata.TypeTreeEnabled = true;
                ttIdx = _targetFile.file.Metadata.TypeTreeTypes.Count - 1;
            }
            else
            {
                var targetTree = _targetFile.file.Metadata.TypeTreeTypes[ttIdx];
                // 覆盖时同样必须保证 TypeDependencies 存在
                targetTree.Nodes = new List<TypeTreeNode>(sourceTree.Nodes);
                targetTree.StringBufferBytes = sourceTree.StringBufferBytes != null ? (byte[])sourceTree.StringBufferBytes.Clone() : Array.Empty<byte>();
                targetTree.ScriptIdHash = sourceTree.ScriptIdHash;
                targetTree.TypeHash = sourceTree.TypeHash;
                targetTree.TypeDependencies = sourceTree.TypeDependencies != null ? (int[])sourceTree.TypeDependencies.Clone() : Array.Empty<int>();
            }

            return (targetScriptIdx, (ushort)ttIdx);
        }

        private void MergeMatchingFields(AssetTypeValueField src, AssetTypeValueField dest)
        {
            if (src == null || dest == null) return;
            
            if (dest.Children.Count > 0 && !dest.TemplateField.IsArray)
            {
                foreach (var destChild in dest.Children)
                {
                    var srcChild = src[destChild.TemplateField.Name];
                    if (srcChild != null && !srcChild.IsDummy)
                    {
                        MergeMatchingFields(srcChild, destChild);
                    }
                }
            }
            else if (dest.TemplateField.IsArray && src.TemplateField.IsArray)
            {
                while (dest.Children.Count < src.Children.Count)
                {
                    dest.Children.Add(ValueBuilder.DefaultValueFieldFromTemplate(dest.TemplateField.Children[1]));
                }
                int count = Math.Min(src.Children.Count, dest.Children.Count);
                for (int i = 0; i < count; i++)
                {
                    MergeMatchingFields(src.Children[i], dest.Children[i]);
                }
            }
            else if (dest.Value != null && src.Value != null)
            {
                try
                {
                    switch (dest.TemplateField.ValueType)
                    {
                        case AssetValueType.Bool: dest.Value.AsBool = src.Value.AsBool; break;
                        case AssetValueType.Int8: dest.Value.AsSByte = src.Value.AsSByte; break;
                        case AssetValueType.UInt8: dest.Value.AsByte = src.Value.AsByte; break;
                        case AssetValueType.Int16: dest.Value.AsInt = src.Value.AsInt; break;
                        case AssetValueType.UInt16: dest.Value.AsUInt = src.Value.AsUInt; break;
                        case AssetValueType.Int32: dest.Value.AsInt = src.Value.AsInt; break;
                        case AssetValueType.UInt32: dest.Value.AsUInt = src.Value.AsUInt; break;
                        case AssetValueType.Int64: dest.Value.AsLong = src.Value.AsLong; break;
                        case AssetValueType.UInt64: dest.Value.AsULong = src.Value.AsULong; break;
                        case AssetValueType.Float: dest.Value.AsFloat = src.Value.AsFloat; break;
                        case AssetValueType.Double: dest.Value.AsDouble = src.Value.AsDouble; break;
                        case AssetValueType.String: dest.Value.AsString = src.Value.AsString; break;
                    }
                }
                catch { }
            }
        }

        private byte[] GetSourceAssetRawBytes(AssetFileInfo info) => GetAssetRawBytes(_sourceFile, info);
        private byte[] GetTargetAssetRawBytes(AssetFileInfo info) => GetAssetRawBytes(_targetFile, info);

        private byte[] GetAssetRawBytes(AssetsFileInstance fileInst, AssetFileInfo info)
        {
            if (info == null) return null;
            lock (fileInst.LockReader)
            {
                var reader = fileInst.file.Reader;
                reader.Position = info.GetAbsoluteByteOffset(fileInst.file);
                return reader.ReadBytes((int)info.ByteSize);
            }
        }

        private bool IsScriptNamed(AssetsFileInstance sourceFileInst, int fileId, long pathId, string expectedName)
        {
            var scriptExt = _workspace.Manager.GetExtAsset(sourceFileInst, fileId, pathId);
            if (scriptExt.baseField != null && !scriptExt.baseField.IsDummy)
            {
                return scriptExt.baseField["m_Name"].AsString == expectedName;
            }

            try
            {
                var depInst = sourceFileInst;
                if (fileId != 0) 
                    depInst = sourceFileInst.GetDependency(_workspace.Manager, fileId - 1);
                    
                if (depInst != null)
                {
                    var info = depInst.file.GetAssetInfo(pathId);
                    if (info != null)
                    {
                        lock (depInst.LockReader)
                        {
                            var reader = depInst.file.Reader;
                            reader.Position = info.GetAbsoluteByteOffset(depInst.file);
                            return reader.ReadCountStringInt32() == expectedName;
                        }
                    }
                }
            }
            catch { }

            return false;
        }

        private void CleanupUnusedMetadata()
        {
            var metadata = _targetFile.file.Metadata;
            
            // 1. 扫描并重排 TypeTreeTypes (TypeIdOrIndex)
            // 在启用 TypeTree 的资产文件中，TypeIdOrIndex 是指向 TypeTreeTypes 列表的真实索引
            var usedTypeTreeIndices = new SortedSet<int>();
            foreach (var info in metadata.AssetInfos)
            {
                usedTypeTreeIndices.Add((int)info.TypeIdOrIndex);
            }

            var typeTreeMapping = new Dictionary<int, int>();
            var newTypeTrees = new List<TypeTreeType>();
            int nextTtIdx = 0;
            foreach (int oldIdx in usedTypeTreeIndices)
            {
                if (oldIdx >= 0 && oldIdx < metadata.TypeTreeTypes.Count)
                {
                    typeTreeMapping[oldIdx] = nextTtIdx++;
                    newTypeTrees.Add(metadata.TypeTreeTypes[oldIdx]);
                }
            }

            // [必须步骤] 更新全模型资产的 TypeIdOrIndex 与缓存同步
            foreach (var info in metadata.AssetInfos)
            {
                if (typeTreeMapping.TryGetValue((int)info.TypeIdOrIndex, out int newTtIdx))
                {
                    // 这里原本使用了错误的 (uint) 强制转换导致报错，现直接赋值以适配 int 类型的 TypeIdOrIndex 字段
                    info.TypeIdOrIndex = (ushort)newTtIdx; // 使用 ushort 强制转换以适配 int
                    
                    var assetInst = _workspace.GetAssetInst(_targetFile, 0, info.PathId);
                    if (assetInst != null)
                    {
                        assetInst.TypeIdOrIndex = (ushort)newTtIdx;
                    }
                }
            }

            // 重设 Metadata 中的 TypeTree 列表
            metadata.TypeTreeTypes.Clear();
            foreach (var tt in newTypeTrees) metadata.TypeTreeTypes.Add(tt);

            // 2. 扫描并重排 ScriptTypes (ScriptTypeIndex)
            // 在重排后的 TypeTreeTypes 中，找出那些真正被引用的脚本指针 ScriptTypeIndex
            var usedScriptIndices = new SortedSet<int>();
            foreach (var tt in metadata.TypeTreeTypes)
            {
                if (tt.ScriptTypeIndex != 0xFFFF)
                {
                    usedScriptIndices.Add((int)tt.ScriptTypeIndex);
                }
            }

            var scriptMapping = new Dictionary<int, int>();
            var newScriptTypes = new List<AssetPPtr>();
            int nextScriptIdx = 0;
            foreach (int oldIdx in usedScriptIndices)
            {
                if (oldIdx >= 0 && oldIdx < metadata.ScriptTypes.Count)
                {
                    scriptMapping[oldIdx] = nextScriptIdx++;
                    newScriptTypes.Add(metadata.ScriptTypes[oldIdx]);
                }
            }

            // 同步更新 TypeTreeType 内部的脚本索引指向
            foreach (var tt in metadata.TypeTreeTypes)
            {
                if (tt.ScriptTypeIndex != 0xFFFF && scriptMapping.TryGetValue((int)tt.ScriptTypeIndex, out int mappedSIdx))
                {
                    tt.ScriptTypeIndex = (ushort)mappedSIdx;
                }
                else { tt.ScriptTypeIndex = 0xFFFF; }
            }

            // [必须步骤] 同步更新全模型资产的 ScriptTypeIndex 与缓存
            foreach (var info in metadata.AssetInfos)
            {
                if (info.ScriptTypeIndex != 0xFFFF)
                {
                    if (scriptMapping.TryGetValue((int)info.ScriptTypeIndex, out int mappedSIdx))
                    {
                        info.ScriptTypeIndex = (ushort)mappedSIdx;
                        
                        var assetInst = _workspace.GetAssetInst(_targetFile, 0, info.PathId);
                        if (assetInst != null)
                        {
                            assetInst.ScriptTypeIndex = (ushort)mappedSIdx;
                        }
                    }
                    else { info.ScriptTypeIndex = 0xFFFF; }
                }
            }

            // 重设 Metadata 中的脚本指针列表 (ScriptTypes)
            metadata.ScriptTypes.Clear();
            foreach (var st in newScriptTypes) metadata.ScriptTypes.Add(st);
        }
    }
}