using ACore.Google;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACore
{
    [CreateAssetMenu(menuName = "ACore/Setting", fileName = "ASetting")]
    public class ASettingData : ScriptableObjectAuto
    {
        [TabGroup("General")] public string language;
        [TabGroup("General")] public FPSLimit FPS;
        
        [TabGroup("Google Play")] public bool isGooglePlay;
        [TabGroup("Google Play"), ShowIf(nameof(isGooglePlay)), HideLabel] 
        public GooglePlaySetting googlePlay;
    }
}
