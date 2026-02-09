using UnityEngine;

namespace ACore
{
    public class ACoreStorage : BaseStorage
    {
        public string language;
        public int fps;

        public override void OnDefault()
        {
            var _setting = Game.GetSO<ASettingData>();
            if (string.IsNullOrEmpty(_setting.language))
            {
                language = Localize.GetDefault();
            }
            
            fps = _setting.FPS.ToValue();
        }

        public override void OnLoad()
        {
            Localize.Initialize();
            FPSManager.Set(fps);
        }
    }
}
