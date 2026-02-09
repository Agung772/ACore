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
            if (string.IsNullOrEmpty(_setting.language))
            {
                language = Localize.GetDefault();
            }
            
            FPS = _setting.FPS;
        }

        public override void OnLoad()
        {
            Localize.Initialize();
            FPSManager.Set(FPS);
        }
    }
}
