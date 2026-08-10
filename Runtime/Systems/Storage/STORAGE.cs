using System.IO;
using UnityEngine;

namespace ACore
{
    public static class STORAGE
    {
        private const string FILE_NAME = "Save.txt";
        private static string PathFile => Path.Combine(Application.persistentDataPath, FILE_NAME);
        private static GameData data = new();
        public static T Get<T>() where T : BaseStorage, new() => data.Get<T>();
        public static string GetJSON() => data.GetJSON();
        public static void SetJSON(string json) => data.SetJSON(json);
        
        public static void Initialize()
        {
            Create();
            Load();
            Application.quitting += Save;
        }

        private static void Create()
        {
            data.Initialize();
        }
        
        private static void Load()
        {
            if (File.Exists(PathFile))
            {
                var _json = File.ReadAllBytes(PathFile);
                var _decrypt = Encryption.Decrypt(_json);
                data.SetJSON(_decrypt);
            }
            else
            {
                Debug.LogWarning("Storage data not found");
                data.New();
            }
        }

        public static void Save()
        {
            if (data == null) return;

            var _json = data.GetJSON();
            var _encrypt = Encryption.Encrypt(_json);
            File.WriteAllBytes(PathFile, _encrypt);

#if UNITY_EDITOR
            var _pathEditorSave = Path.Combine(Application.persistentDataPath, PathFile.Replace(".", "Editor."));
            File.WriteAllText(_pathEditorSave, _json);
#endif

            Debug.Log($"Save Storage Data \n" +
                      $"Path : {PathFile}");
        }

        public static void TryReplace(string json)
        {
            var _newStorages = new GameData(json);
            TryReplace(_newStorages);
        }
        
        public static void TryReplace(GameData newData)
        {
            if (newData.Get<MetaStorage>().lastSave > data.Get<MetaStorage>().lastSave)
            {
                data = newData;
            }
        }
    }
}

