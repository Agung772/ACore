using System.Collections.Generic;
using UnityEngine;

namespace ACore
{
    public static class LOCALIZE
    {
        private static readonly Dictionary<string, string> GlobalText = new();
        private static readonly Dictionary<TextAsset, Dictionary<string, string>> AssetCache = new();
        
        public static void Initialize()
        {
            GlobalText.Clear();
            AssetCache.Clear();
            
            var _localize = Resources.Load<TextAsset>("Localize");
            if (_localize == null) return;
            var _parses = _localize.ParseCsv(STORAGE.Get<ACoreStorage>().language);
            foreach (var _parse in _parses)
            {
                GlobalText[_parse.Key] = _parse.Value;
            }
        }

        /// <summary>
        /// Contoh CSV:
        /// HELLO_NAME,Hello {0}
        ///
        /// Contoh pemanggilan:
        /// Localize.GetText("HELLO_NAME", "Agung")
        /// -> "Hello Agung"
        ///
        /// Placeholder menggunakan format:
        /// {0} = parameter pertama
        /// {1} = parameter kedua
        /// </summary>
        public static string GetText(string key, params object[] placeholder)
        {
            if (GlobalText.TryGetValue(key, out var _val))
            {
                if (placeholder != null && placeholder.Length > 0)
                    return string.Format(_val, placeholder);

                return _val;
            }

            return key;
        }
        
        public static string GetText(this TextAsset asset, string key, params object[] placeholder)
        {
            if (asset == null) return key;

            if (!AssetCache.TryGetValue(asset, out var _dict))
            {
                _dict = asset.ParseCsv(STORAGE.Get<ACoreStorage>().language);
                AssetCache[asset] = _dict;
            }

            if (_dict.TryGetValue(key, out var _val))
            {
                if (placeholder != null && placeholder.Length > 0)
                    return string.Format(_val, placeholder);

                return _val;
            }

            return key;
        }
        
        public static string GetDefault()
        {
            return Application.systemLanguage switch
            {
                SystemLanguage.Indonesian => "id",
                _ => "en"
            };
        }
    }
}
