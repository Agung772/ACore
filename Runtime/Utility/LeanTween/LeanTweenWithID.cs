using System.Collections.Generic;
using UnityEngine;

namespace ACore
{
    public static class LeanTweenWithID
    {
        private static readonly Dictionary<GameObject, Dictionary<string, List<LTDescr>>> TweensWithID = new();

        private static void AddTween(GameObject key, string id, LTDescr tween)
        {
            if (key == null || tween == null) return;

            if (!TweensWithID.TryGetValue(key, out var dict))
            {
                dict = new Dictionary<string, List<LTDescr>>();
                TweensWithID[key] = dict;
            }

            if (!dict.TryGetValue(id, out var list))
            {
                list = new List<LTDescr>();
                dict[id] = list;
            }

            list.Add(tween);

            tween.setOnComplete(() =>
            {
                RemoveTween(key, id, tween);
            });
        }

        private static void RemoveTween(GameObject key, string id, LTDescr tween)
        {
            if (key == null || tween == null) return;

            if (!TweensWithID.TryGetValue(key, out var dict)) return;
            if (!dict.TryGetValue(id, out var list)) return;

            list.Remove(tween);

            if (list.Count == 0)
                dict.Remove(id);

            if (dict.Count == 0)
                TweensWithID.Remove(key);
        }

        public static LTDescr SetID(this LTDescr tween, string id)
        {
            if (tween == null) return null;

            var go = LeanTween.descr(tween.uniqueId)?.trans?.gameObject;

            if (go != null)
                AddTween(go, id, tween);

            return tween;
        }

        public static void LeanCancel(this GameObject go, string id)
        {
            if (go == null) return;

            if (!TweensWithID.TryGetValue(go, out var dict)) return;
            if (!dict.TryGetValue(id, out var list)) return;

            var temp = list.ToArray();

            foreach (var tween in temp)
            {
                if (tween != null)
                {
                    LeanTween.cancel(tween.uniqueId);
                }
            }

            dict.Remove(id);

            if (dict.Count == 0)
                TweensWithID.Remove(go);
        }

        public static void LeanCancelAll(this GameObject go)
        {
            if (go == null) return;

            if (!TweensWithID.TryGetValue(go, out var dict)) return;

            var tweenList = new List<LTDescr>();

            foreach (var pair in dict)
            {
                tweenList.AddRange(pair.Value);
            }

            foreach (var tween in tweenList)
            {
                if (tween != null)
                {
                    LeanTween.cancel(tween.uniqueId);
                }
            }

            TweensWithID.Remove(go);
        }

        public static bool IsTweening(this GameObject go, string id)
        {
            return go != null &&
                   TweensWithID.TryGetValue(go, out var dict) &&
                   dict.TryGetValue(id, out var list) &&
                   list.Count > 0;
        }
    }
}