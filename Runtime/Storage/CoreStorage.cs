using System;

namespace ACore
{
    public class CoreStorage : BaseStorage
    {
        public DateTime lastSaveTime;
        public DateTime localSaveTime;

        void t()
        {
            if (lastSaveTime > localSaveTime)
            {
                
            }
        }
    }
}
