#if GOOGLE_MOBILE

using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

namespace ACore.Google
{
    public class AdManager : GlobalBehaviour
    {
        private bool hasInitialize;
        private Dictionary<Type, AdBase> instances = new();

        public override void Initialize()
        {
            var _setting = Game.GetSO<ASettingData>();
            if (!_setting.isGooglePlay) return;
            
            var _googleSetting = _setting.googlePlay;
            if (_googleSetting.noAds) return;
            
            MobileAds.Initialize(_=> 
            {
                hasInitialize = true;
                instances = InstanceUtility.Create<AdBase>();
                foreach (var _google in instances.Values)
                {
                    _google.Initialize();
                }
            });
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