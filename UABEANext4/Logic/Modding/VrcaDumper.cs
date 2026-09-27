using System.Collections.Generic;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Linq;

namespace UABEANext4.Logic.Modding
{
    public class VrcaDumper
    {
        private AssetsManager _manager;
        private AssetsFileInstance _fileInst;
        private UnityVersion _version;

        public VrcaDumper(AssetsManager manager, AssetsFileInstance fileInst)
        {
            _manager = manager;
            _fileInst = fileInst;
            _version = new UnityVersion(fileInst.file.Metadata.UnityVersion);
        }

        public VrcaDump Dump()
        {
            var dump = new VrcaDump();
            dump.Meta.UnityVersion = _fileInst.file.Metadata.UnityVersion;

            var scriptNames = new Dictionary<long, string>();

            foreach (var info in _fileInst.file.Metadata.AssetInfos)
            {
                if (info.TypeId != (int)AssetClassID.MonoBehaviour) continue;

                var baseField = _manager.GetBaseField(_fileInst, info);
                if (baseField == null || baseField["m_Script"].IsDummy) continue;

                long scriptPid = baseField["m_Script"]["m_PathID"].AsLong;
                if (!scriptNames.TryGetValue(scriptPid, out string? scriptName))
                {
                    var scriptExt = _manager.GetExtAsset(_fileInst, baseField["m_Script"]);
                    if (scriptExt.baseField != null)
                    {
                        scriptName = scriptExt.baseField["m_Name"].AsString;
                        scriptNames[scriptPid] = scriptName;
                    }
                }

                if (scriptName == "VRCExpressionParameters")
                {
                    DumpParameters(info, dump.Parameters);
                    // Use the first one found or prioritize if we see descriptor later
                    if (dump.Meta.ParametersPathId == 0) dump.Meta.ParametersPathId = info.PathId;
                }
                else if (scriptName == "VRCExpressionsMenu")
                {
                    var ext = new AssetExternal { info = info, baseField = baseField };
                    RecursivelyDumpMenu(ext, dump.Menus);
                }
                else if (scriptName == "VRCAvatarDescriptor")
                {
                    var menuRef = baseField["expressionsMenu"];
                    if (menuRef != null && !menuRef.IsDummy)
                        dump.Meta.RootMenuPathId = menuRef["m_PathID"].AsLong;
                    
                    var paramsRef = baseField["expressionParameters"];
                    if (paramsRef != null && !paramsRef.IsDummy)
                        dump.Meta.ParametersPathId = paramsRef["m_PathID"].AsLong;
                }
            }

            return dump;
        }

        private void DumpParameters(AssetFileInfo info, List<VrcParameter> list)
        {
            var baseField = _manager.GetBaseField(_fileInst, info);
            if (baseField == null) return;
            
            var paramsField = baseField["parameters"];
            var paramsArray = paramsField["Array"];
            if (paramsArray.IsDummy) paramsArray = paramsField;

            foreach (var p in paramsArray.Children)
            {
                if (p == null || p.TemplateField.IsArray) continue;
                string name = (p["name"].IsDummy || p["name"].Value == null ? "Unknown" : p["name"].AsString);
                
                // Avoid duplicates by name
                if (list.Any(existing => existing.Name == name)) continue;

                list.Add(new VrcParameter
                {
                    Name = name,
                    Type = (p["valueType"].IsDummy || p["valueType"].Value == null ? 0 : p["valueType"].AsInt),
                    DefaultVal = (p["defaultValue"].IsDummy || p["defaultValue"].Value == null ? 0f : p["defaultValue"].AsFloat),
                    Saved = (!p["saved"].IsDummy && p["saved"].Value != null && p["saved"].AsBool),
                    Synced = (!p["networkSynced"].IsDummy && p["networkSynced"].Value != null && p["networkSynced"].AsBool)
                });
            }
        }

        private void RecursivelyDumpMenu(AssetExternal asset, Dictionary<string, VrcMenu> menus)
        {
            if (asset.baseField == null) return;
            string pathIdKey = asset.info.PathId.ToString();
            if (menus.ContainsKey(pathIdKey)) return; // Already dumped

            var nameField = asset.baseField["m_Name"];
            if (nameField.IsDummy) nameField = asset.baseField["name"];
            
            var menu = new VrcMenu { Name = (nameField.IsDummy || nameField.Value == null ? "Unknown Menu" : nameField.AsString) };
            var controlsField = asset.baseField["controls"];
            var controlsArray = controlsField["Array"];
            if (controlsArray.IsDummy) controlsArray = controlsField;
            
            foreach (var c in controlsArray.Children)
            {
                if (c == null || c.TemplateField.IsArray) continue;
                var ctrl = new VrcControl
                {
                    Name = (c["name"].IsDummy || c["name"].Value == null ? "Control" : c["name"].AsString),
                    Type = (c["type"].IsDummy || c["type"].Value == null ? 0 : c["type"].AsInt),
                    Parameter = (c["parameter"].IsDummy || c["parameter"]["name"].IsDummy || c["parameter"]["name"].Value == null ? "" : c["parameter"]["name"].AsString),
                    Value = (c["value"].IsDummy || c["value"].Value == null ? 0f : c["value"].AsFloat),
                    Style = (c["style"].IsDummy || c["style"].Value == null ? 0 : c["style"].AsInt)
                };

                // Icon
                var iconRef = c["icon"];
                if (iconRef != null && !iconRef.IsDummy) ctrl.IconPathId = iconRef["m_PathID"].AsLong;

                // SubMenu
                var subMenuRef = c["subMenu"];
                if (subMenuRef != null && !subMenuRef.IsDummy)
                {
                    long subId = subMenuRef["m_PathID"].AsLong;
                    if (subId != 0)
                    {
                        ctrl.SubMenuPathId = subId;
                        // For non-recursive scan, we don't MUST recurse here if we scan all assets anyway,
                        // but recursing ensures we follow the tree if some are in other files.
                        // However, VRCA is usually self-contained.
                        if (!menus.ContainsKey(subId.ToString()))
                        {
                            var subAsset = _manager.GetExtAsset(_fileInst, subMenuRef);
                            if (subAsset.baseField != null)
                                RecursivelyDumpMenu(subAsset, menus);
                        }
                    }
                }

                menu.Controls.Add(ctrl);
            }

            menus[pathIdKey] = menu;
        }
    }
}
