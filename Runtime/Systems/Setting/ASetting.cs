using System.Collections;
using System.IO;
using ACore.Google;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ACore
{
    [CreateAssetMenu(menuName = "ACore/Setting", fileName = "ASetting")]
    public class ASetting : ScriptableObjectAuto
    {
        [TabGroup("General")] public bool autoSave = true;
        [TabGroup("General")] public string language;
        [TabGroup("General")] public FPSLimit FPS;
        
        [TabGroup("Google Play")] public bool isGooglePlay;
        [TabGroup("Google Play"), ShowIf(nameof(isGooglePlay)), HideLabel] 
        public GooglePlaySetting googlePlay;
        
        [TabGroup("Supabase")] public bool isSupabase;
        [TabGroup("Supabase"), ShowIf(nameof(isSupabase)), HideLabel] 
        public SupabaseSetting supabase;

        public virtual IEnumerator FirstScene()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                var _scenePath = SceneUtility.GetScenePathByBuildIndex(1);
                var _sceneName = Path.GetFileNameWithoutExtension(_scenePath);
                yield return SCENE.LoadCoroutine(_sceneName);
            }
        }
    }
}
