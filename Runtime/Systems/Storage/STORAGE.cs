using System.IO;
using UnityEngine;

namespace ACore
{
    public static class STORAGE
    {
        private const string FILE_NAME = "Save.txt";
        private static string PathFile => Path.Combine(Application.persistentDataPath, FILE_NAME);
        private static GameData data;
        public static T Get<T>() where T : BaseStorage, new() => data.Get<T>();
        public static string GetJSON() => data.GetJSON();
        public static void SetJSON(string json) => data.SetJSON(json);
        
        public static void Initialize()
        {
            Load();
            Application.quitting += Save;
        }
        
        private static void Load()
        {
            if (File.Exists(PathFile))
            {
                var _json = File.ReadAllBytes(PathFile);
                var _decrypt = Encryption.Decrypt(_json);
                data = new GameData(_decrypt);
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
            
            data.Save();

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

        public static bool TryReplace(string json)
        {
            var _newStorages = new GameData(json);
            return TryReplace(_newStorages);
        }
        
        public static bool TryReplace(GameData newData)
        {
            if (newData.Get<MetaStorage>().lastSave > data.Get<MetaStorage>().lastSave)
            {
                data = newData;
                return true;
            }

            return false;
        }

        public static void Replace(string json)
        {
            var _newStorages = new GameData(json);
            Replace(_newStorages);
        }
        public static void Replace(GameData newData)
        {
            data = newData;
        }
    }
}

