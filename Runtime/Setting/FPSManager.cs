using System;
using UnityEngine;

namespace ACore
{
    public static class FPSManager
    {
        public static void Set(int value)
        {
            Application.targetFrameRate = value;
        }
        
        public static void Set(FPSLimit fps)
        {
            Application.targetFrameRate = fps.ToValue();
        }
        
        public static int ToValue(this FPSLimit fps)
        {
            return fps switch
            {
                FPSLimit.Auto => GetDefault(),
                FPSLimit.FPS30 => 30,
                FPSLimit.FPS60 => 60,
                FPSLimit.FPS90 => 90,
                FPSLimit.FPS120 => 120,
                _ => GetDefault()
            };
        }

        public static int GetDefault()
        {
            var _hz = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
            _hz = Mathf.Clamp(_hz, 30, 120);
            return _hz;
        }
    }
}
