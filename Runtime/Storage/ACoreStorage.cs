using UnityEngine;

namespace ACore
{
    public class ACoreStorage : BaseStorage
    {
        public string language;

        public override void OnLoad()
        {
            if (string.IsNullOrEmpty(language))
            {
                language = Localize.GetDefault();
            }
            
            
            Localize.Initialize();
            Debug.Log(        Localize.GetText("test"));
        }
    }
}
