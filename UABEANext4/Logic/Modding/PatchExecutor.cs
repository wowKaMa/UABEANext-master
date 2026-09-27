using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using Newtonsoft.Json;
using UABEANext4.AssetWorkspace;
using UABEANext4.Logic;
using CommunityToolkit.Mvvm.Messaging;

namespace UABEANext4.Logic.Modding
{
    public class PatchExecutor
    {
        private readonly Workspace _workspace;
        private readonly AssetsFileInstance _sourceFile;
        private readonly AssetsFileInstance _targetFile;
        private readonly Dictionary<long, long> _remapTable = new();

        public PatchExecutor(Workspace workspace, AssetsFileInstance source, AssetsFileInstance target)
        {
            _workspace = workspace;
            _sourceFile = source;
            _targetFile = target;
        }

        public int Execute(string instructionsJson)
        {
            var plan = JsonConvert.DeserializeObject<dynamic>(instructionsJson);
            if (plan == null || plan.actions == null) return 0;

            int actionCount = 0;
            // 1. Process Actions
            foreach (var action in plan.actions)
            {
                string type = (string)action.type;
                try
                {
                    if (type == "COPY_ASSET")
                    {
                        CopyAsset((long)action.source_path_id);
                        actionCount++;
                    }
                    else if (type == "MODIFY_ASSET")
                    {
                        ModifyAsset((long)action.source_path_id, (long)action.target_path_id);
                        actionCount++;
                    }
                    else if (type == "ADD_PARAM")
                    {
                        AddParameter(action.data);
                        actionCount++;
                    }
                    else if (type == "MODIFY_PARAM")
                    {
                        ModifyParameter(action.data);
                        actionCount++;
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue with other actions
                    Console.WriteLine($"Error processing patch action {type}: {ex.Message}");
                }
            }

            // 2. Notify UI
            WeakReferenceMessenger.Default.Send(new AssetFileModifiedMessage(_targetFile));

            return actionCount;
        }

        private void CopyAsset(long sourcePathId)
        {
            if (sourcePathId == 0) return;
            if (_remapTable.ContainsKey(sourcePathId)) return;

            var ext = _workspace.Manager.GetExtAsset(_sourceFile, 0, sourcePathId);
            if (ext.baseField == null) return;

            // 1. Remap internal references in the data (Recursive)
            RemapReferencesToTarget(ext.baseField);

            // 2. Create in target
            long nextPathId = 1;
            if (_targetFile.file.Metadata.AssetInfos.Count > 0)
                nextPathId = _targetFile.file.Metadata.AssetInfos.Max(a => a.PathId) + 1;
            
            ushort scriptIdx = 0xFFFF;
            if (ext.info.TypeId == (int)AssetClassID.MonoBehaviour)
            {
                scriptIdx = GetScriptIdxInTarget(ext.baseField["m_Script"]);
            }

            var baseInfo = AssetFileInfo.Create(
                _targetFile.file, 
                nextPathId, 
                ext.info.TypeId, 
                scriptIdx,
                _workspace.Manager.ClassDatabase, 
                false
            );

            if (baseInfo == null) return;

            // Use AssetInst to wrap it so it's ready for UI
            var newAsset = new AssetInst(_targetFile, baseInfo);
            newAsset.UpdateAssetDataAndRow(_workspace, ext.baseField);
            
            var infos = (System.Collections.ObjectModel.RangeObservableCollection<AssetFileInfo>)_targetFile.file.Metadata.AssetInfos;
            infos.Add(newAsset);

            _remapTable[sourcePathId] = nextPathId;
        }

        private void ModifyAsset(long sourcePathId, long targetPathId)
        {
            if (sourcePathId == 0 || targetPathId == 0) return;

            var sourceExt = _workspace.Manager.GetExtAsset(_sourceFile, 0, sourcePathId);
            var targetAsset = _workspace.GetAssetInst(_targetFile, 0, targetPathId);
            
            if (sourceExt.baseField == null || targetAsset == null) return;

            // Remap references to target
            RemapReferencesToTarget(sourceExt.baseField);

            // Apply data
            targetAsset.UpdateAssetDataAndRow(_workspace, sourceExt.baseField);
        }

        private void RemapReferencesToTarget(AssetTypeValueField field)
        {
            if (field.Children == null) return;
            
            // Detect PPtr
            bool isPtr = field.Children.Count >= 2 && 
                         field.Children[0].TemplateField.Name == "m_FileID" && 
                         field.Children[1].TemplateField.Name == "m_PathID";
            
            if (isPtr)
            {
                long pathId = field.Children[1].AsLong;
                int fileId = field.Children[0].AsInt;

                if (pathId != 0 && fileId == 0) // Internal reference to source file
                {
                    // Check if we already copied/remapped this
                    if (!_remapTable.TryGetValue(pathId, out long newPathId))
                    {
                        // Optimization: For menus, we can try to match by name if it's already in target?
                        // But for now, let's just copy if it's missing from remap table.
                        CopyAsset(pathId);
                        _remapTable.TryGetValue(pathId, out newPathId);
                    }
                    
                    if (newPathId != 0)
                        field.Children[1].AsLong = newPathId;
                }
                return;
            }

            foreach (var child in field.Children)
            {
                RemapReferencesToTarget(child);
            }
        }

        private ushort GetScriptIdxInTarget(AssetTypeValueField sourceScriptPtr)
        {
            var scriptExt = _workspace.Manager.GetExtAsset(_sourceFile, sourceScriptPtr);
            if (scriptExt.baseField == null) return 0xFFFF;

            string scriptName = scriptExt.baseField["m_Name"].AsString;
            
            // Find this script in target file
            foreach (var type in _targetFile.file.Metadata.ScriptTypes)
            {
                // In AT.NET 3, we need to resolve the script to get its name
                // This is a bit slow, but necessary for correctness
                // (Ideally we'd have a script cache like VrcaDumper)
                var targetScriptExt = _workspace.Manager.GetExtAsset(_targetFile, 0, type.PathId);
                if (targetScriptExt.baseField != null && targetScriptExt.baseField["m_Name"].AsString == scriptName)
                {
                    // Found it
                    return (ushort)_targetFile.file.Metadata.ScriptTypes.IndexOf(type);
                }
            }

            return 0xFFFF;
        }

        private void AddParameter(dynamic data)
        {
            var asset = GetAssetByScript("VRCExpressionParameters");
            if (asset == null) return;

            var baseField = _workspace.GetBaseField(asset);
            if (baseField == null) return;

            var arrayField = baseField["parameters"]["Array"];
            if (arrayField.IsDummy) arrayField = baseField["parameters"];

            if (arrayField.TemplateField.Children.Count < 2) return;

            var template = arrayField.TemplateField.Children[1]; // Element template
            var newParam = ValueBuilder.DefaultValueFieldFromTemplate(template);
            
            newParam["name"].AsString = (string)data.name;
            newParam["valueType"].AsInt = (int)data.type;
            newParam["defaultValue"].AsFloat = (float)data.default_val;
            newParam["saved"].AsBool = data.saved ?? true;
            newParam["networkSynced"].AsBool = data.synced ?? true;

            arrayField.Children.Add(newParam);
            asset.UpdateAssetDataAndRow(_workspace, baseField);
        }

        private void ModifyParameter(dynamic data)
        {
            var asset = GetAssetByScript("VRCExpressionParameters");
            if (asset == null) return;

            var baseField = _workspace.GetBaseField(asset);
            if (baseField == null) return;

            var arrayField = baseField["parameters"]["Array"];
            if (arrayField.IsDummy) arrayField = baseField["parameters"];

            string name = (string)data.name;
            var param = arrayField.Children.FirstOrDefault(c => c["name"].AsString == name);
            if (param != null)
            {
                param["valueType"].AsInt = (int)data.type;
                param["defaultValue"].AsFloat = (float)data.default_val;
                param["saved"].AsBool = data.saved ?? true;
                param["networkSynced"].AsBool = data.synced ?? true;
                
                asset.UpdateAssetDataAndRow(_workspace, baseField);
            }
        }

        private AssetInst? GetAssetByScript(string scriptName)
        {
            foreach (var info in _targetFile.file.Metadata.AssetInfos)
            {
                if (info.TypeId == (int)AssetClassID.MonoBehaviour)
                {
                    var asset = _workspace.GetAssetInst(_targetFile, 0, info.PathId);
                    if (asset == null) continue;

                    var baseField = _workspace.GetBaseField(asset);
                    if (baseField == null) continue;
                    
                    var scriptRef = baseField["m_Script"];
                    if (scriptRef.IsDummy) continue;
                    
                    var scriptExt = _workspace.Manager.GetExtAsset(_targetFile, scriptRef);
                    if (scriptExt.baseField != null && scriptExt.baseField["m_Name"].AsString == scriptName)
                    {
                        return asset;
                    }
                }
            }
            return null;
        }
    }
}
