using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ACore
{
    public class GameData
    {
        private Dictionary<Type, BaseStorage> storages = new();
        public T Get<T>() where T : BaseStorage, new() => storages[typeof(T)] as T;
        private readonly JsonSerializerSettings jsonSettings = new() { TypeNameHandling = TypeNameHandling.Auto };
        
        public GameData(string json) { SetJSON(json); }
        
        public string GetJSON()
        {
            return JsonConvert.SerializeObject(storages, jsonSettings);
        }

        public void SetJSON(string json)
        {
            try
            {
                storages = InstanceUtility.Create<BaseStorage>();
                foreach (var _storage in storages.Values)
                {
                    _storage.OnCreate();
                }
                
                var _raw = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(json, jsonSettings);
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

        public void New()
        {
            foreach (var _storage in storages.Values)
            {
                _storage.OnDefault();
                _storage.OnLoad();
            }
        }
        
        public void Save()
        {
            foreach (var _storage in storages.Values)
            {
                _storage.OnSave();
            }
        }
    }
}
