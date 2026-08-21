#if SUPABASE

using System;
using System.IO;
using Newtonsoft.Json;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using UnityEngine;

namespace ACore
{
    public class UnitySessionHandler : IGotrueSessionPersistence<Session>
    {
        private const string FileName = "gotrue.cache";
        private static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

        public void SaveSession(Session session)
        {
            if (session == null)
            {
                DestroySession();
                return;
            }

            try
            {
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(session));
            }
            catch (Exception _e)
            {
                Debug.LogError($"[Supabase] Failed to save session: {_e}");
                throw;
            }
        }

        public Session LoadSession()
        {
            if (!File.Exists(FilePath))
                return null;

            try
            {
                var _json = File.ReadAllText(FilePath);
                return string.IsNullOrEmpty(_json) ? null : JsonConvert.DeserializeObject<Session>(_json);
            }
            catch (Exception _e)
            {
                Debug.LogError($"[Supabase] Failed to load session: {_e}");
                return null;
            }
        }

        public void DestroySession()
        {
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
}

#endif