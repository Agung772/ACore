#if GOOGLE_MOBILE

using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using UnityEngine;


namespace ACore.Google
{
    public class PlayGamesManager : GlobalBehaviour
    { 
        private bool hasInitialize;
        public event Action OnInitialize;
        private Dictionary<Type, PlayGamesBase> instances = new();

        public override IEnumerator InitializeCoroutine()
        {
            if (!GAME.GetSO<ASettingData>().isGooglePlay) yield break;
            PlayGamesPlatform.Activate();
            
            var _isCompleted = false;
            Social.localUser.Authenticate(success =>
            {
                if (success)
                {
                    hasInitialize = true;
                    instances = InstanceUtility.Create<PlayGamesBase>();
                    foreach (var _google in instances.Values)
                    {
                        _google.Initialize();
                    }
                    OnInitialize?.Invoke();
                    Debug.Log("[Google Play Games] Authentication successfully");
                }
                else
                {
                    Debug.Log("[Google Play Games] Authentication failed");
                }

                _isCompleted = true;
            });
            
            yield return new WaitUntil(() => _isCompleted);
        }
        
        public bool IsActive<T>() where T : PlayGamesBase
        {
            return hasInitialize;
        }
        
        public T Get<T>() where T : PlayGamesBase
        {
            if (instances.TryGetValue(typeof(T), out var _instance))
            {
                return (T)_instance;
            }

            return null;
        }
        
        public bool TryGet<T>(out T instance) where T : PlayGamesBase
        {
            if (hasInitialize)
            {
                var _instance = Get<T>();
                if (_instance != null)
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