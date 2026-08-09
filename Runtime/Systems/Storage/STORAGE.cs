using System.IO;
using UnityEngine;

namespace ACore
{
    public static class STORAGE
    {
        private const string FILE_NAME = "Save.txt";
        private static string PathFile => Path.Combine(Application.persistentDataPath, FILE_NAME);
        private static GameStorages storages = new();
        public static T Get<T>() where T : BaseStorage, new() => storages.Get<T>();
        
        public static void Initialize()
        {
            Create();
            Load();
            Application.quitting += Save;
        }

        private static void Create()
        {
            storages.Initialize();
        }
        
        private static void Load()
        {
            if (File.Exists(PathFile))
            {
                var _json = File.ReadAllBytes(PathFile);
                var _decrypt = Encryption.Decrypt(_json);
                storages.SetJSON(_decrypt);
            }
            else
            {
                Debug.LogWarning("Storage data not found");
                storages.New();
            }
        }

        public static void Save()
        {
            if (storages == null) return;

            var _json = storages.GetJSON();
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
            var _newStorages = new GameStorages(json);
            TryReplace(_newStorages);
        }
        
        public static void TryReplace(GameStorages newStorages)
        {
            if (newStorages.Get<MetaStorage>().lastSave > storages.Get<MetaStorage>().lastSave)
            {
                storages = newStorages;
            }
        }
    }
}

