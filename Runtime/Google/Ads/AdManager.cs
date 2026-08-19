#if GOOGLE_MOBILE

using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

namespace ACore.Google
{
    public class AdManager : GlobalBehaviour
    {
        private bool hasInitialize;
        private Dictionary<Type, AdBase> instances = new();
        
        public override IEnumerator InitializeCoroutine()
        {
            var _setting = GAME.GetSO<ASettingData>();
            if (!_setting.isGooglePlay) yield break;
            if (_setting.googlePlay.noAds) yield break;
            
            var _isCompleted = false;
            MobileAds.Initialize(_=> 
            {
                instances = InstanceUtility.Create<AdBase>();
                foreach (var _google in instances.Values)
                {
                    _google.Initialize();
                }

                hasInitialize = true;
                _isCompleted = true;
                Debug.Log("[AdMob] Initialize successfully");
            });
            
            yield return new WaitUntil(() => _isCompleted);
        }

        public bool IsActive<T>() where T : AdBase
        {
            var _instance = Get<T>();
            return _instance != null && _instance.CanShow();
        }
        
        public T Get<T>() where T : AdBase
        {
            if (instances.TryGetValue(typeof(T), out var _instance))
            {
                return (T)_instance;
            }

            return null;
        }
        
        public bool TryGet<T>(out T instance) where T : AdBase
        {
            if (hasInitialize)
            {
                var _instance = Get<T>();
                if (_instance != null && _instance.CanShow())
                {
                    instance = _instance;
                    return true;
                }
            }

            instance = null;
            return false;
        }
    }
}

#endif