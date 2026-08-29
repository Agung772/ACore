using System;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace ACore
{
    public class MetaStorage : BaseStorage
    {
        // Account
        public DateTime createdAt;
        public DateTime lastLogin;
        public DateTime lastSave;
        public int loginCount;

        // Device
        public string deviceID;
        public string deviceModel;
        public string deviceName;
        public string deviceManufacturer;
        public string operatingSystem;
        public string operatingSystemVersion;
        public int androidApiLevel;

        // App
        public string appVersion;
        public string buildNumber;
        public string deviceLanguage;
        public string timeZone;

        // Gameplay
        public double totalPlayTime;

        public override void OnDefault()
        {
            base.OnDefault();
            createdAt = DateTime.UtcNow;
        }

        public override void OnLoad()
        {
            base.OnLoad();
            GAME.Manager.OnUpdate1s -= OnPlayTimeTick;
            GAME.Manager.OnUpdate1s += OnPlayTimeTick;
            RefreshMetadata();
            Login();
        }

        private void OnPlayTimeTick()
        {
            AddPlayTime(1);
        }
        
        private void RefreshMetadata()
        {
            deviceID = SystemInfo.deviceUniqueIdentifier;
            deviceModel = SystemInfo.deviceModel;
            deviceName = SystemInfo.deviceName;
            operatingSystem = SystemInfo.operatingSystem;
            deviceLanguage = Application.systemLanguage.ToString();
            timeZone = TimeZoneInfo.Local.Id;
            
            appVersion = Application.version;
            buildNumber = Application.buildGUID;

#if UNITY_ANDROID && !UNITY_EDITOR
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                androidApiLevel = version.GetStatic<int>("SDK_INT");
                operatingSystemVersion = version.GetStatic<string>("RELEASE");
            }

            using (var build = new AndroidJavaClass("android.os.Build"))
            {
                deviceManufacturer = build.GetStatic<string>("MANUFACTURER");
            }
#else
            androidApiLevel = -1;
            operatingSystemVersion = Environment.OSVersion.VersionString;
            deviceManufacturer = "Unknown";
#endif
        }

        private void Login()
        {
            lastLogin = DateTime.UtcNow;
            loginCount++;
        }

        public override void OnSave()
        {
            base.OnSave();
            lastSave = DateTime.UtcNow;
        }

        private void AddPlayTime(double seconds)
        {
            totalPlayTime += seconds;
        }
    }
}