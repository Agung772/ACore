using UnityEngine;

namespace ACore
{
    public class ACoreStorage : BaseStorage
    {
        public string language;
        public FPSLimit FPS;

        public override void OnDefault()
        {
            var _setting = GAME.GetSO<ASettingData>();
            language = string.IsNullOrEmpty(_setting.language) ? LOCALIZE.GetDefault() : _setting.language;
            FPS = _setting.FPS;
        }

        public override void OnLoad()
        {
            LOCALIZE.Initialize();
            FPSManager.Set(FPS);
        }
    }
}
