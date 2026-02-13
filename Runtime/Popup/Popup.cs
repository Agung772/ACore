using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ACore
{
    public static class Popup
    {
        public static readonly Dictionary<Type, PopupBehaviour> Active = new();
        public static readonly Dictionary<Type, PopupBehaviour> Resources = new();

        public static void Initialize()
        {
            var _popups = UnityEngine.Resources.LoadAll<PopupBehaviour>("");
            foreach (var _popup in _popups)
            {
                Resources.Add(_popup.GetType(), _popup);
            }
        }

        public static T Show<T>() where T : PopupBehaviour
        {
            var _prefab = (T)Resources[typeof(T)];
            return Show(_prefab);
        }

        public static T Show<T>(T prefab) where T : PopupBehaviour
        {
            var _popup = SpawnPopup(prefab);
            _popup.Initialize();

            if (_popup is not MultiPopupBehaviour
                && _popup is not WorldPopupBehaviour)
            {
                Active.Add(_popup.GetType(), _popup);
            }

            return (T)_popup;
        }
        
        public static bool TryShow<T>(out T popup) where T : PopupBehaviour
        {
            if (!IsActive<T>())
            {
                popup = Show<T>(); 
                return true;
            }

            popup = null;
            return false;
        }

        private static PopupBehaviour SpawnPopup(PopupBehaviour prefab)
        {
            switch (prefab)
            {
                case WorldPopupBehaviour:
                    return Object.Instantiate(prefab).GetComponent<PopupBehaviour>();
                case TouchEffect:
                    return Object.Instantiate(prefab, Game.Manager.FrontCanvas);
            }

            if (prefab.setOrder)
            {
                var _canvas = Object.Instantiate(Game.Manager.CanvasPrefab, Game.Manager.transform);
                _canvas.GetComponent<Canvas>().sortingOrder = prefab.sortOrder;
                var _popup = Object.Instantiate(prefab, _canvas);
                _popup.onClose += () => Object.Destroy(_canvas.gameObject);
                return _popup;
            }
            
            return Object.Instantiate(prefab, Game.Manager.Canvas);
        }

        public static void RemoveOnLoaded(bool withGlobal = false)
        {
            foreach (var _popup in Active.Values.ToArray())
            {
                if (withGlobal || !_popup.isGlobal)
                {
                    Remove(_popup);
                }
            }
        }
        
        public static bool Remove<T>() where T : PopupBehaviour 
        {
            if (TryGet<T>(out var _popup))
            {
                _popup.OnClose(); 
                return true;
            }
            
            return false;
        }
        
        public static bool Remove(PopupBehaviour popup) 
        {
            if (Active.ContainsKey(popup.GetType()))
            {
                popup.OnClose();
                return true;
            }
            
            return false;
        }
        
        public static bool IsActive<T>() where T : PopupBehaviour
        {
            return Active.ContainsKey(typeof(T));
        }

        public static bool TryGet<T>(out T popup) where T : PopupBehaviour
        {
            if (IsActive<T>())
            {
                popup = Active[typeof(T)] as T;
                return true;
            }
            
            popup = null;
            return false;
        }
        
        public static T Get<T>() where T : PopupBehaviour
        {
            TryGet<T>(out var _popup);
            return _popup;
        }
    }
}

