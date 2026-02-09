#if GOOGLE_MOBILE

using System;
using System.Collections.Generic;
using GooglePlayGames;
using UnityEngine;


namespace ACore.Google
{
    public class PlayGamesManager : GlobalBehaviour
    { 
        private bool hasInitialize;
        private int maxTryAgain = 3;
        private int countTryAgain;
        public event Action OnInitialize;
        private Dictionary<Type, PlayGamesBase> instances = new();

        public override void Initialize()
        {
            if (!Game.GetSO<ASettingData>().isGooglePlay) return;
            
            PlayGamesPlatform.Activate();
            RequestPlayGames(() =>
            {
                hasInitialize = true;
                instances = InstanceUtility.Create<PlayGamesBase>();
                foreach (var _google in instances.Values)
                {
                    _google.Initialize();
                }
                OnInitialize?.Invoke();
            });
        }

        private void RequestPlayGames(Action onComplete)
        {
            Social.localUser.Authenticate(success =>
            {
                if (success)
                {
                    onComplete.Invoke();
                    Debug.Log("Login with Google Play games successful.");
                }
                else
                {
                    if (countTryAgain < maxTryAgain)
                    {
                        countTryAgain++;
                        Game.Manager.gameObject.LeanDelayedCall(1f, () =>
                        {
                            RequestPlayGames(onComplete);
                        });
                    }
 
                    Debug.Log("Login Unsuccessful.");
                }
            });
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