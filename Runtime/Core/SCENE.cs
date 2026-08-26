using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ACore
{
    public static class SCENE
    {
        public static event Action OnLoaded;
        public static event Action OnUnloaded;

        public static void Initialize()
        {
            GAME.CurrentScene = SceneManager.GetActiveScene().name;
            SceneManager.sceneLoaded += (_, _) => OnLoaded?.Invoke();
            SceneManager.sceneUnloaded += _ => OnUnloaded?.Invoke();
        }

        public static void Restart(LoadSceneSetting setting = null)
        {
            Load(GAME.CurrentScene, setting);
        }
        
        public static void Load(string sceneName, LoadSceneSetting setting = null)
        {
            GAME.Manager.StartCoroutine(LoadCoroutine(sceneName, setting));
        }

        public static void LoadWithFade(string sceneName, LoadSceneSetting setting = null)
        {
            if (setting == null) setting = new LoadSceneSetting();
            
            var _onComplete = setting.onComplete;
            var _popup = OBJECT.Show<BlackscreenPopup>();
            _popup.In(setting.fadeInInstant, () =>
            {
                setting.onComplete = () =>
                {
                    _popup.Out(setting.fadeOutInstant, () =>
                    {
                        _popup.Remove();
                        _onComplete?.Invoke();
                    });
                };
                Load(sceneName, setting);
            });
        }
        
        public static IEnumerator LoadCoroutine(string sceneName, LoadSceneSetting setting = null)
        {
            if (setting == null) setting = new LoadSceneSetting();
            
            OBJECT.RemoveOnLoaded(setting.removeAllPopup);
            var _async = SceneManager.LoadSceneAsync(sceneName);

            GAME.CurrentScene = sceneName;
            while (!_async.isDone)
            {
                setting.onProgress?.Invoke(Mathf.Clamp01(_async.progress / 0.9f));
                yield return null;
            }

            var _isCompleted = false;
            _async.completed += _ => _isCompleted = true;
            yield return new WaitUntil(() => _isCompleted);
            
            setting.onComplete?.Invoke();
        }
    }
}