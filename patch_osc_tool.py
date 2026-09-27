
import os
import re

file_path = r"e:\UABEANext-master\UABEANext-master\UABEANext4\ViewModels\Tools\OscToolViewModel.cs"

with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

# Match the line with flexible whitespace and potential Chinese characters
pattern = re.compile(r'SelectedAssetChannels\s*=\s*\$".*channels.*";')
match = pattern.search(content)

if not match:
    print("Target not found with regex!")
    # Try even simpler
    pattern = re.compile(r'SelectedAssetChannels\s*=\s*.*;')
    match = pattern.search(content)
    if not match:
        exit(1)

target_full_line = match.group(0)
print(f"Found target: {target_full_line}")

replacement = target_full_line + r'''

                        // Heuristics for ShellProtector
                        string assetName = value.AssetName.ToLower();
                        bool isEncrypted = assetName.Contains("_encrypt") || 
                                         assetName.Contains("_encode") || 
                                         assetName.Contains("_encrypttex") ||
                                         assetName.Contains("_encrypt2");

                        if (isEncrypted)
                        {
                            // ShellProtector 2.x uses Linear color space by default for its encrypted textures.
                            // Applying Gamma 0.45 correction restores it to sRGB.
                            SelectedGammaModeIndex = 2; // Linear -> sRGB (Gamma 0.45)
                            
                            // _encrypt2 and _Encode are usually RGBA32 (ID 4) and often need BGR swap in Unity libraries
                            if (texFile.m_TextureFormat == 4 || texFile.m_TextureFormat == 5)
                            {
                                SwapBgr = true;
                            }
                            else
                            {
                                SwapBgr = false;
                            }
                        }'''

new_content = content.replace(target_full_line, replacement)

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(new_content)

print("Patch applied successfully!")
