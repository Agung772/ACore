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
        private static bool autoSave = true;
        
        public static void Initialize()
        {
            Load();
            if (GAME.TryGetSO<ASetting>(out var _so))
            {
                autoSave = _so.autoSave;
            }
            Application.quitting += TrySave;
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
                Debug.LogWarning("[Storage] local game data not found");
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

#if SUPABASE
            SupabaseManager.SaveData();
#endif
            Debug.Log($"[Storage] Saving local game data \n" +
                      $"Path : {PathFile}");
        }

        public static void TrySave()
        {
            if (autoSave)
            {
                Save();
            }
        }

        public static bool TryReplace(string json)
        {
            var _newStorages = new GameData();
            _newStorages.SetJSON(json);
            return TryReplace(_newStorages);
        }
        
        public static bool TryReplace(GameData newData)
        {
            if (newData.Get<MetaStorage>().lastSave > data.Get<MetaStorage>().lastSave)
            {
                Debug.Log("[Storage] Successfully try replaced game data to local");
                data = newData;
                return true;
            }

            Debug.Log("[Storage] Try replace failed: new game data is older or equal to local data");
            return false;
        }

        public static void Replace(string json)
        {
            data.SetJSON(json);
            Debug.Log("[Storage] Successfully replaced game data to local");
        }
    }
}

