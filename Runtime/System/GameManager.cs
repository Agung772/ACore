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
        public event Action OnUpdate;
        public RectTransform Canvas => canvas; [SerializeField] private RectTransform canvas;
        public RectTransform FrontCanvas => frontCanvas; [SerializeField] private RectTransform frontCanvas;
        public RectTransform CanvasPrefab => canvasPrefab; [SerializeField] private RectTransform canvasPrefab;
        
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
            OnUpdate?.Invoke();
        }

        private IEnumerator Initialize()
        {
            DontDestroyOnLoad(gameObject);
            SCENE.Initialize();
            GAME.Manager = this;
            GAME.Initialize();
            Debug.Log($"{nameof(ACore)}: Initialize");
            yield return GAME.InitializeCoroutine();
            Debug.Log($"{nameof(ACore)}: Initialize Coroutine");
            
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                var _scenePath = SceneUtility.GetScenePathByBuildIndex(1);
                var _sceneName = Path.GetFileNameWithoutExtension(_scenePath);
                SCENE.LoadScene(_sceneName);
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                STORAGE.Save();
            }
        }
    }
}

