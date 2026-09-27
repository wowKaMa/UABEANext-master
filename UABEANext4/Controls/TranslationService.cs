using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Concurrent;

namespace UABEANext4.Util
{
    /// <summary>
    /// 極致強化版翻譯服務 - 專為 Unity 數據結構設計
    /// </summary>
    public static class TranslationService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        // 內存緩存：L1 Cache
        private static readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // 並發控制器：限制瞬時網絡請求，防止被封 IP
        private static readonly SemaphoreSlim _networkLock = new SemaphoreSlim(3, 3);

        // --- 1. 專業術語保護字典 ---
        private static readonly Dictionary<string, string> TermProtection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "GameObject", "遊戲對象" },
            { "Transform", "變換" },
            { "Mesh", "網格" },
            { "Renderer", "渲染器" },
            { "PhysBone", "物理骨骼" },
            { "Avatar", "化身" },
            { "Material", "材質球" },
            { "Texture", "貼圖" },
            { "Component", "組件" },
            { "Collider", "碰撞體" },
            { "Animator", "動畫控制器" }
        };

        /// <summary>
        /// 執行翻譯：輸入原始變量名，返回簡體中文
        /// </summary>
        public static async Task<string> TranslateAsync(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            // A. 跳過不需要翻譯的技術數據 (GUID, PathID, 純數字)
            if (IsTechnicalData(input)) return input;

            // B. 智能分詞與規範化 (m_sAlpha -> Static Alpha)
            string normalizedText = SmartNormalize(input);

            // C. 檢查術語保護
            if (TermProtection.TryGetValue(normalizedText, out var protectedResult)) return protectedResult;

            // D. 檢查緩存
            if (_cache.TryGetValue(normalizedText, out var cachedResult)) return cachedResult;

            // E. 執行雲端異步翻譯
            return await FetchCloudTranslation(normalizedText, input);
        }

        private static async Task<string> FetchCloudTranslation(string normalizedText, string original, int retry = 2)
        {
            await _networkLock.WaitAsync();
            try
            {
                // 使用 Google Translate Free 接口 (簡體中文)
                string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl=zh-CN&dt=t&q={HttpUtility.UrlEncode(normalizedText)}";

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                var response = await _httpClient.GetStringAsync(url, cts.Token);

                // 解析 JSON 返回值
                var match = Regex.Match(response, @"\""([^\""]+)\""");
                if (match.Success)
                {
                    string translated = match.Groups[1].Value;

                    // 清理翻譯結果中的多餘字符
                    translated = Regex.Replace(translated, @"[？\?]", "").Trim();

                    _cache.TryAdd(normalizedText, translated);
                    return translated;
                }
            }
            catch
            {
                if (retry > 0)
                {
                    await Task.Delay(300); // 失敗重試延遲
                    return await FetchCloudTranslation(normalizedText, original, retry - 1);
                }
            }
            finally
            {
                _networkLock.Release();
            }

            return original; // 所有嘗試失敗則返回原文
        }

        /// <summary>
        /// 核心糾錯分詞引擎：將代碼變量轉化為人類語言
        /// </summary>
        private static string SmartNormalize(string text)
        {
            string res = text;

            // 1. 剝離 Unity 字段前綴
            if (res.StartsWith("m_")) res = res.Substring(2);
            if (res.StartsWith("_")) res = res.Substring(1);

            // 2. 識別特定的縮寫邏輯 (例如 sAlpha -> Static Alpha)
            res = Regex.Replace(res, @"^s([A-Z])", "Static $1");

            // 3. 處理駝峰命名與數字分離
            // 效果：IsPreProcessed -> Is Pre Processed / Texture2D -> Texture 2D
            res = Regex.Replace(res, @"([a-z0-9])([A-Z])", "$1 $2");
            res = Regex.Replace(res, @"([A-Z])([A-Z][a-z])", "$1 $2");
            res = Regex.Replace(res, @"([a-zA-Z])([0-9])", "$1 $2");

            // 4. 處理連接符
            res = res.Replace("_", " ").Replace(".", " ");

            // 5. 修正過度拆分的專有名詞
            res = res.Replace("Mip Map", "Mipmap")
                     .Replace("Phys Bone", "PhysBone")
                     .Replace("RGBA", "RGBA ")
                     .Replace("VRC", "VRC ");

            return Regex.Replace(res, @"\s+", " ").Trim();
        }

        /// <summary>
        /// 技術數據過濾邏輯
        /// </summary>
        private static bool IsTechnicalData(string input)
        {
            // 十六進制、長路徑 ID、GUID、或純數字
            if (input.StartsWith("0x")) return true;
            if (input.Length > 16 && Regex.IsMatch(input, @"[0-9a-fA-F]{16,}")) return true;
            if (long.TryParse(input, out _)) return true;
            if (input.Contains("[") && input.Contains("]")) return true; // 數組下標
            return false;
        }
    }
}