using UnityEngine;

namespace ACore
{
    using System.Collections.Generic;
    using System.IO;

    public static class LocalizeUtility
    {
        public static Dictionary<string, string> ParseCsv(this TextAsset asset, string language)
        {
            var _dict = new Dictionary<string, string>();
            if (!asset) return _dict;
            if (string.IsNullOrEmpty(asset.text)) return _dict;

            using var _reader = new StringReader(asset.text);
            var _header = _reader.ReadLine()?.Split(',');
            if (_header == null) return _dict;

            var _langIndex = FindLanguageIndex(_header, language);
            if (_langIndex == -1)
                _langIndex = FindLanguageIndex(_header, "en");

            string _line;
            while ((_line = _reader.ReadLine()) != null)
            {
                var _parts = SplitCsv(_line);
                if (_parts.Length <= _langIndex || string.IsNullOrEmpty(_parts[0])) continue;
                _dict[_parts[0].Trim()] = _parts[_langIndex].Trim();
            }

            return _dict;
        }

        private static int FindLanguageIndex(string[] header, string lang)
        {
            return System.Array.FindIndex(header, h => h.Trim().ToLower() == lang.ToLower());
        }

        private static string[] SplitCsv(string line)
        {
            var _result = new List<string>();
            var _inQuotes = false;
            var _current = "";

            foreach (var _c in line)
            {
                if (_c == '"') _inQuotes = !_inQuotes;
                else if (_c == ',' && !_inQuotes)
                {
                    _result.Add(_current);
                    _current = "";
                }
                else _current += _c;
            }

            _result.Add(_current);
            return _result.ToArray();
        }
    }

}
