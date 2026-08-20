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

        public static void RestratScene(Action<float> onProgress = null, bool removeAllPopup = false, Action onComplete = null)
        {
            LoadScene(GAME.CurrentScene, onProgress, removeAllPopup, onComplete);
        }
        
        public static void LoadScene(string sceneName, Action<float> onProgress = null, bool removeAllPopup = false, Action onComplete = null)
        {
            GAME.Manager.StartCoroutine(LoadSceneCoroutine(sceneName, onProgress, removeAllPopup, onComplete));
        }

        public static IEnumerator LoadSceneCoroutine(string sceneName, Action<float> onProgress = null, bool removeAllPopup = false, Action onComplete = null)
        {
            OBJECT.RemoveOnLoaded(removeAllPopup);
            var _async = SceneManager.LoadSceneAsync(sceneName);

            GAME.CurrentScene = sceneName;
            while (_async.isDone)
            {
                var _progress = Mathf.Clamp01(_async.progress / 0.9f);
                onProgress?.Invoke(_progress);
                yield return null;
            }

            var _isCompleted = false;
            _async.completed += _ => _isCompleted = true;
            yield return new WaitUntil(() => _isCompleted);
            
            onComplete?.Invoke();
        }
    }
}

