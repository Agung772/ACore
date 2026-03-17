using System.Collections.Generic;
using UnityEngine;

namespace ACore
{
    public static class LeanTweenWithID
    { 
        private static readonly Dictionary<GameObject, Dictionary<string, List<LTDescr>>> TweensWithID = new();

        private static void AddTween(GameObject key, string id, LTDescr tween)
        {
            if (key == null) return;

            if (!TweensWithID.TryGetValue(key, out var _dict))
            {
                _dict = new Dictionary<string, List<LTDescr>>();
                TweensWithID[key] = _dict;
            }

            if (!_dict.TryGetValue(id, out var _list))
            {
                _list = new List<LTDescr>();
                _dict[id] = _list;
            }

            _list.Add(tween);

            tween.setOnComplete(() =>
            {
                RemoveTween(key, id, tween);
            });
        }

        private static void RemoveTween(GameObject key, string id, LTDescr tween)
        {
            if (!TweensWithID.TryGetValue(key, out var _dict)) return;
            if (!_dict.TryGetValue(id, out var _list)) return;

            _list.Remove(tween);

            if (_list.Count == 0)
                _dict.Remove(id);

            if (_dict.Count == 0)
                TweensWithID.Remove(key);
        }

        public static LTDescr SetID(this LTDescr tween, string id)
        {
            var _go = LeanTween.descr(tween.uniqueId)?.trans?.gameObject;
            AddTween(_go, id, tween);
            return tween;
        }

        public static void LeanCancel(this GameObject go, string id)
        {
            if (!TweensWithID.TryGetValue(go, out var _dict)) return;
            if (!_dict.TryGetValue(id, out var _list)) return;

            foreach (var _tween in _list)
            {
                if (_tween != null)
                    LeanTween.cancel(_tween.uniqueId);
            }

            _dict.Remove(id);

            if (_dict.Count == 0)
                TweensWithID.Remove(go);
        }

        public static void LeanCancelAll(this GameObject go)
        {
            if (!TweensWithID.TryGetValue(go, out var _dict)) return;

            foreach (var _pair in _dict)
            {
                foreach (var _tween in _pair.Value)
                {
                    if (_tween != null)
                        LeanTween.cancel(_tween.uniqueId);
                }
            }

            TweensWithID.Remove(go);
        }

        public static bool IsTweening(this GameObject go, string id)
        {
            return TweensWithID.TryGetValue(go, out var _dict) &&
                   _dict.TryGetValue(id, out var _list) &&
                   _list.Count > 0;
        }
    }
}