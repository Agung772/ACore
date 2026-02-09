using UnityEngine;

namespace ACore
{
    public class ACoreStorage : BaseStorage
    {
        public string language;
        public FPSLimit FPS;

        public override void OnDefault()
        {
            var _setting = Game.GetSO<ASettingData>();
            language = string.IsNullOrEmpty(_setting.language) ? Localize.GetDefault() : _setting.language;
            FPS = _setting.FPS;
        }

        public override void OnLoad()
        {
            Localize.Initialize();
            FPSManager.Set(FPS);
        }
    }
}
