using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ACore
{
    public static class Storage
    {
        private const string FILE_NAME = "Save.txt";
        private static string PathFile => Path.Combine(Application.persistentDataPath, FILE_NAME);
        
        private static readonly JsonSerializerSettings jsonSettings = new() { TypeNameHandling = TypeNameHandling.Auto };
        private static Dictionary<Type, BaseStorage> storages = new();
        public static T Get<T>() where T : BaseStorage, new() => storages[typeof(T)] as T;

        public static void Initialize()
        {
            Create();
            Load();
            Application.quitting += Save;
        }

        private static void Create()
        {
            storages = InstanceUtility.Create<BaseStorage>();
            foreach (var _storage in storages.Values)
            {
                _storage.OnCreate();
            }
        }
        private static void Load()
        {
            if (File.Exists(PathFile))
            {
                try
                {
                    var _json = File.ReadAllBytes(PathFile);
                    var _decrypt = Encryption.Decrypt(_json);

                    var _raw = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(_decrypt, jsonSettings);
                    var _serializer = JsonSerializer.Create(jsonSettings);

                    foreach (var _key in storages.Keys.ToArray())
                    {
                        try
                        {
                            var _typeName = _key.AssemblyQualifiedName;
                            if (_typeName != null && _raw.TryGetValue(_typeName, out var _token))
                            {
                                var _storage = _token.ToObject(typeof(BaseStorage), _serializer) as BaseStorage;
                                if (_storage != null)
                                {
                                    storages[_key] = _storage;
                                }
                            }
                        }
                        catch (Exception _exInner)
                        {
                            Debug.LogWarning($"Skip error for storage type {_key.Name}: {_exInner.Message}");
                        }

                        storages[_key].OnLoad();
                    }
                }
                catch (Exception _ex)
                {
                    Debug.LogWarning($"Failed to load storage data: {_ex.Message}");
                    foreach (var _storage in storages.Values)
                    {
                        _storage.OnLoad();
                    }
                }
            }
            else
            {
                Debug.LogWarning("Storage data not found");
                foreach (var _storage in storages.Values)
                {
                    _storage.OnDefault();
                    _storage.OnLoad();
                }
            }
        }

        public static void Save()
        {
            if (storages == null) return;
            if (storages.Count == 0) return;
            
            foreach (var _key in storages.Keys.ToList())
            {
                storages[_key].OnSave();
            }
            
            var _json = JsonConvert.SerializeObject(storages, jsonSettings);
            var _encrypt = Encryption.Encrypt(_json);
            File.WriteAllBytes(PathFile, _encrypt);

            #if UNITY_EDITOR
            var _pathEditorSave = Path.Combine(Application.persistentDataPath, PathFile.Replace(".", "Editor."));
            File.WriteAllText(_pathEditorSave, _json);
            #endif

            Debug.Log($"Save Storage Data \n" +
                      $"Path : {PathFile}");
        }
    }
}

