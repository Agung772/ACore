using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ACore
{
    [DefaultExecutionOrder(-1000)]
    public class GameManager : MonoBehaviour
    {
        /// <summary>Normal Update</summary>
        public event Action OnUpdate;
        
        /// <summary>0.1 Second</summary>
        public event Action OnUpdate100ms;
        private float timer100ms;
        
        /// <summary>0.25 Second</summary>
        public event Action OnUpdate250ms;
        private float timer250ms;
        
        /// <summary>0.5 Second</summary>
        public event Action OnUpdate500ms;
        private float timer500ms;
        
        /// <summary>1 Second</summary>
        public event Action OnUpdate1s;
        private float timer1s;
        
        /// <summary>5 Seconds</summary>
        public event Action OnUpdate5s;
        private float timer5s;
        
        
        #if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnPlay()
        {
            Debug.Log($"{nameof(ACore)}: Play");
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                Debug.Log($"{nameof(ACore)}: Create Manager");
                Instantiate(Resources.Load<GameManager>("GameManager"));
            }
        }
        #endif

        private void Awake()
        {
            StartCoroutine(Initialize());
        }

        private void Update()
        {
            UpdateTick();
        }

        private IEnumerator Initialize()
        {
            Debug.Log($"[{nameof(ACore)}] Start Booting...");
            
            DontDestroyOnLoad(gameObject);
            GAME.Manager = this;
            SCENE.Initialize();
            OBJECT.Initialize();
            STORAGE.Initialize();

            if (OBJECT.TryShow<BootingPopup>(out var _popup))
                yield return GAME.Initialize(_popup.Setup);
            else
                yield return GAME.Initialize();
            
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                var _scenePath = SceneUtility.GetScenePathByBuildIndex(1);
                var _sceneName = Path.GetFileNameWithoutExtension(_scenePath);
                yield return SCENE.LoadSceneCoroutine(_sceneName);
            }

            if (_popup != null)
            {
                _popup.Remove();
            }
            
            Debug.Log($"[{nameof(ACore)}] Booting Completed");
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // Auto Save
                //STORAGE.Save();
            }
        }

        private void UpdateTick()
        {
            OnUpdate?.Invoke();
            
            var _dt = Time.deltaTime;
            timer100ms += _dt;
            timer250ms += _dt;
            timer500ms += _dt;
            timer1s += _dt;
            timer5s += _dt;

            if (timer100ms >= 0.1f)
            {
                timer100ms -= 0.1f;
                OnUpdate100ms?.Invoke();
            }

            if (timer250ms >= 0.25f)
            {
                timer250ms -= 0.25f;
                OnUpdate250ms?.Invoke();
            }

            if (timer500ms >= 0.5f)
            {
                timer500ms -= 0.5f;
                OnUpdate500ms?.Invoke();
            }

            if (timer1s >= 1f)
            {
                timer1s -= 1f;
                OnUpdate1s?.Invoke();
            }

            if (timer5s >= 5f)
            {
                timer5s -= 5f;
                OnUpdate5s?.Invoke();
            }
        }
    }
}

