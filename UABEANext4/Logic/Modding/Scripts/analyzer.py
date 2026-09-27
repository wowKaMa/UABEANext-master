import json
import sys
import os

# Try to use fuzzywuzzy, fallback to simple matching if not available
try:
    from fuzzywuzzy import fuzz
except ImportError:
    class fuzz:
        @staticmethod
        def ratio(a, b):
            return 100 if a.lower() == b.lower() else 0

def analyze(source_path, target_path, output_path):
    with open(source_path, 'r', encoding='utf-8') as f:
        source = json.load(f)
    with open(target_path, 'r', encoding='utf-8') as f:
        target = json.load(f)

    plan = {
        "stats": {"new_assets": 0, "new_params": 0},
        "actions": []
    }

    # 1. Analyze Parameters
    src_params = {p['name']: p for p in source.get('parameters', [])}
    tgt_params = {p['name']: p for p in target.get('parameters', [])}
    
    src_params_pid = source.get('meta', {}).get('parameters_path_id', 0)
    tgt_params_pid = target.get('meta', {}).get('parameters_path_id', 0)

    for name, p in src_params.items():
        if name not in tgt_params:
            plan["actions"].append({
                "type": "ADD_PARAM",
                "source_path_id": src_params_pid,
                "target_path_id": tgt_params_pid,
                "data": p
            })
            plan["stats"]["new_params"] += 1
        elif p['type'] != tgt_params[name]['type'] or p['saved'] != tgt_params[name]['saved']:
            plan["actions"].append({
                "type": "MODIFY_PARAM",
                "source_path_id": src_params_pid,
                "target_path_id": tgt_params_pid,
                "data": p
            })

    # 2. Analyze Menus with Robust Matching
    src_menus = source.get('menus', {})
    tgt_menus = target.get('menus', {})
    
    def get_menu_fingerprint(menu):
        controls = sorted([f"{c['name']}:{c['type']}" for c in menu.get('controls', [])])
        return "|".join(controls)

    matched_tgt_ids = set()

    # Pre-calculate fingerprints
    src_data = {sid: {"name": m.get('name', 'Unknown'), "fingerprint": get_menu_fingerprint(m), "json": m} 
                for sid, m in src_menus.items()}
    tgt_data = {tid: {"name": m.get('name', 'Unknown'), "fingerprint": get_menu_fingerprint(m), "json": m} 
                for tid, m in tgt_menus.items()}

    for sid, sinfo in src_data.items():
        s_name = sinfo["name"]
        s_fingerprint = sinfo["fingerprint"]
        
        best_match_score = -1
        best_tid = None
        
        for tid, tinfo in tgt_data.items():
            if tid in matched_tgt_ids: continue
            
            t_name = tinfo["name"]
            t_fingerprint = tinfo["fingerprint"]
            
            # 1. Name Similarity
            name_score = fuzz.ratio(s_name, t_name)
            
            # 2. Structure Similarity
            struct_score = fuzz.ratio(s_fingerprint, t_fingerprint)
            
            # Weighted average with Boost for Exact Name
            total_score = (name_score * 0.5) + (struct_score * 0.5)
            if name_score == 100:
                total_score += 20 # Exact Name Boost
            
            if total_score > best_match_score:
                best_match_score = total_score
                best_tid = tid
        
        # Threshold (Adjusted)
        if best_match_score < 50:
             plan["actions"].append({
                "type": "COPY_ASSET",
                "source_path_id": int(sid),
                "reason": f"New/Unique Menu: {s_name}"
            })
             plan["stats"]["new_assets"] += 1
        else:
            matched_tgt_ids.add(best_tid)
            tinfo = tgt_data[best_tid]
            
            # Check for changes
            if s_fingerprint != tinfo["fingerprint"]:
                plan["actions"].append({
                    "type": "MODIFY_ASSET",
                    "source_path_id": int(sid),
                    "target_path_id": int(best_tid),
                    "reason": f"Modified Menu: {s_name} (Similarity: {best_match_score:.1f}%)"
                })
            else:
                # Even if structure same, check deeper
                if json.dumps(sinfo["json"], sort_keys=True) != json.dumps(tinfo["json"], sort_keys=True):
                    plan["actions"].append({
                        "type": "MODIFY_ASSET",
                        "source_path_id": int(sid),
                        "target_path_id": int(best_tid),
                        "reason": f"Modified Content: {s_name}"
                    })

    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(plan, f, indent=2)

if __name__ == "__main__":
    if len(sys.argv) < 4:
        print("Usage: analyzer.py <source.json> <target.json> <output.json>")
    else:
        analyze(sys.argv[1], sys.argv[2], sys.argv[3])
