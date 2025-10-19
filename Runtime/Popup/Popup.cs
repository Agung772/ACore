using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ACore
{
    public class Popup : GlobalBehaviour
    {
        public Dictionary<Type, PopupBehaviour> active = new();
        public Dictionary<Type, PopupBehaviour> resources = new();

        public override void Initialize()
        {
            var _popups = Resources.LoadAll<PopupBehaviour>("");
            foreach (var _popup in _popups)
            {
                resources.Add(_popup.GetType(), _popup);
            }
        }

        public T Show<T>() where T : PopupBehaviour
        {
            var _prefab = resources[typeof(T)];
            var _popup = SpawnPopup(_prefab);
            _popup.Initialize();
            if (_popup is not MultiPopupBehaviour) active.Add(typeof(T), _popup);
            return (T)_popup;
        }

        private PopupBehaviour SpawnPopup(PopupBehaviour prefab)
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

        public void RemoveOnLoaded(bool withGlobal = false)
        {
            foreach (var _popup in active.Values.ToArray())
            {
                if (withGlobal || !_popup.isGlobal)
                {
                    Remove(_popup);
                }
            }
        }
        
        public bool Remove<T>() where T : PopupBehaviour 
        {
            if (TryGet<T>(out var _popup))
            {
                _popup.Close(); 
                return true;
            }
            
            return false;
        }
        
        public bool Remove(PopupBehaviour popup) 
        {
            if (active.ContainsKey(popup.GetType()))
            {
                popup.Close();
                return true;
            }
            
            return false;
        }
        
        public bool IsActive<T>() where T : PopupBehaviour
        {
            return active.ContainsKey(typeof(T));
        }

        public bool TryGet<T>(out T popup) where T : PopupBehaviour
        {
            if (IsActive<T>())
            {
                popup = active[typeof(T)] as T;
                return true;
            }
            
            popup = null;
            return false;
        }
        
        public T Get<T>() where T : PopupBehaviour
        {
            TryGet<T>(out var _popup);
            return _popup;
        }
    }
}

