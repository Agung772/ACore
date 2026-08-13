#if SUPABASE

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Supabase;

namespace ACore
{
    public class SupabaseManager : GlobalBehaviour
    {
        public static Client Client { get; private set; }

        public override async Task InitializeAsync() => await Setup().WithTimeout(5);

        private async Task<NetworkResult> Setup()
        {
            try
            {
                Debug.Log("[Supabase] Initializing...");

                var _setting = GAME.GetSO<ASettingData>().supabase;

                Client = new Client(
                    _setting.url,
                    _setting.key,
                    new SupabaseOptions
                    {
                        AutoRefreshToken = true,
                        AutoConnectRealtime = false
                    }
                );

                Debug.Log("[Supabase] Client created.");

                await Client.InitializeAsync();

                Debug.Log("[Supabase] Initialization completed.");

                return new NetworkResult();
            }
            catch (Exception _e)
            {
                Client = null;
                Debug.LogError($"[Supabase] Initialization failed: {_e}");
                return new NetworkResult($"Supabase initialization failed: {_e.Message}");
            }
        }

        public static async Task<NetworkResult<GameDatabase>> GetData()
        {
            Debug.Log("[Supabase] get game data...");

            if (Client == null)
            {
                Debug.LogError("[Supabase] Cannot fetch game data: client is not initialized.");
                return new NetworkResult<GameDatabase>("Supabase client is not initialized.");
            }

            if (!await NETWORK.IsConnection())
            {
                Debug.LogWarning("[Supabase] Cannot fetch game data: no internet connection.");
                return new NetworkResult<GameDatabase>("No internet connection.");
            }

            try
            {
                var _user = Client.Auth.CurrentUser;

                if (_user == null)
                {
                    Debug.LogWarning("[Supabase] Cannot fetch game data: user is not authenticated.");
                    return new NetworkResult<GameDatabase>("User is not authenticated.");
                }

                var _response = await Client
                    .From<GameDatabase>()
                    .Where(x => x.Id == _user.Id)
                    .Get();

                if (_response.Models == null || _response.Models.Count == 0)
                {
                    Debug.LogWarning($"[Supabase] Game data not found for user: {_user.Id}");
                    return new NetworkResult<GameDatabase>("Game data not found.");
                }

                Debug.Log("[Supabase] Game data get successfully.");

                return new NetworkResult<GameDatabase>(_response.Models[0]);
            }
            catch (Exception _e)
            {
                Debug.LogError($"[Supabase] Failed to fetch game data: {_e}");
                return new NetworkResult<GameDatabase>(_e.Message);
            }
        }

        public static async Task<NetworkResult> SaveData()
        {
            return await SaveData(STORAGE.GetJSON());
        }

        public static async Task<NetworkResult> SaveData(string _gameData)
        {
            Debug.Log("[Supabase] Saving game data...");

            if (Client == null)
            {
                Debug.LogError("[Supabase] Cannot save game data: client is not initialized.");
                return new NetworkResult("Supabase client is not initialized.");
            }

            if (!await NETWORK.IsConnection())
            {
                Debug.LogWarning("[Supabase] Cannot save game data: no internet connection.");
                return new NetworkResult("No internet connection.");
            }

            try
            {
                var _user = Client.Auth.CurrentUser;

                if (_user == null)
                {
                    Debug.LogWarning("[Supabase] Cannot save game data: user is not authenticated.");
                    return new NetworkResult("User is not authenticated.");
                }

                var _gameDatabase = new GameDatabase
                {
                    Id = _user.Id,
                    GameData = _gameData
                };

                await Client
                    .From<GameDatabase>()
                    .Upsert(_gameDatabase);

                Debug.Log("[Supabase] Game data saved successfully.");

                return new NetworkResult();
            }
            catch (Exception _e)
            {
                Debug.LogError($"[Supabase] Failed to save game data: {_e}");
                return new NetworkResult(_e.Message);
            }
        }

        public static async Task<NetworkResult<DateTime>> GetTime()
        {
            Debug.Log("[Supabase] Fetching server time...");

            if (Client == null)
            {
                Debug.LogError("[Supabase] Cannot fetch server time: client is not initialized.");
                return new NetworkResult<DateTime>("Supabase client is not initialized.");
            }

            if (!await NETWORK.IsConnection())
            {
                Debug.LogWarning("[Supabase] Cannot fetch server time: no internet connection.");
                return new NetworkResult<DateTime>("No internet connection.");
            }

            try
            {
                var _response = await Client
                    .Rpc(
                        "get_server_time",
                        new Dictionary<string, object>()
                    )
                    .WithTimeout(5f);

                var _serverTime = DateTime.Parse(_response.Content).ToUniversalTime();

                Debug.Log($"[Supabase] Server time received: {_serverTime:O}");

                return new NetworkResult<DateTime>(_serverTime);
            }
            catch (Exception _e)
            {
                Debug.LogError($"[Supabase] Failed to fetch server time: {_e}");
                return new NetworkResult<DateTime>($"Failed to fetch server time: {_e.Message}");
            }
        }
    }
}

#endif