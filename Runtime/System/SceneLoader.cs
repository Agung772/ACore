using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ACore
{
    public class SceneLoader : GlobalBehaviour
    {
        public event Action OnLoaded;
        public event Action OnUnloaded;

        public override void Initialize()
        {
            Game.CurrentScene = SceneManager.GetActiveScene().name;
            SceneManager.sceneLoaded += (_, _) => OnLoaded?.Invoke();
            SceneManager.sceneUnloaded += _ => OnUnloaded?.Invoke();
        }

        public void RestratScene(Action<float> onProgress = null, bool removeAllPopup = false, Action onComplete = null)
        {
            LoadScene(Game.CurrentScene, onProgress, removeAllPopup, onComplete);
        }
        
        public void LoadScene(string sceneName, Action<float> onProgress = null, bool removeAllPopup = false, Action onComplete = null)
        {
            Game.Manager.StartCoroutine(LoadSceneAsync(sceneName, onProgress, removeAllPopup, onComplete));
        }

        private IEnumerator LoadSceneAsync(string sceneName, Action<float> onProgress = null, bool removeAllPopup = false, Action onComplete = null)
        {
            Game.Get<Popup>().RemoveOnLoaded(removeAllPopup);
            var _async = SceneManager.LoadSceneAsync(sceneName);

            while (_async.isDone)
            {
                var _progress = Mathf.Clamp01(_async.progress / 0.9f);
                onProgress?.Invoke(_progress);
                yield return null;
            }
            
            Game.CurrentScene = sceneName;

            var _isCompleted = false;
            _async.completed += _ => _isCompleted = true;
            yield return new WaitUntil(() => _isCompleted);
            
            onComplete?.Invoke();
        }
    }
}

