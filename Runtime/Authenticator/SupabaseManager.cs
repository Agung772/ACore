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
                        AutoConnectRealtime = false,
                        SessionHandler = new UnitySessionHandler()
                    }
                );

                await Client.InitializeAsync();
                Client.Auth.LoadSession();
                await Client.Auth.RetrieveSessionAsync();

                Debug.Log($"[Supabase] Initialization completed. User: {Client.Auth.CurrentUser?.Id}");

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
                return new NetworkResult<GameDatabase>("Supabase client is not initialized.");

            if (!await NETWORK.IsConnection())
                return new NetworkResult<GameDatabase>("No internet connection.");

            try
            {
                var _user = Client.Auth.CurrentUser;

                if (_user == null)
                    return new NetworkResult<GameDatabase>("User is not authenticated.");

                var _response = await Client.From<GameDatabase>().Where(x => x.Id == _user.Id).Get();

                if (_response.Models == null || _response.Models.Count == 0)
                    return new NetworkResult<GameDatabase>("Game data not found.");

                Debug.Log("[Supabase] Game data get successfully.");

                return new NetworkResult<GameDatabase>(_response.Models[0]);
            }
            catch (Exception _e)
            {
                Debug.LogError($"[Supabase] Failed to fetch game data: {_e}");
                return new NetworkResult<GameDatabase>(_e.Message);
            }
        }

        public static async Task<NetworkResult> SaveData() => await SaveData(STORAGE.GetJSON());

        public static async Task<NetworkResult> SaveData(string gameData)
        {
            Debug.Log("[Supabase] Saving game data...");

            if (Client == null)
                return new NetworkResult("Supabase client is not initialized.");

            if (!await NETWORK.IsConnection())
                return new NetworkResult("No internet connection.");

            try
            {
                var _user = Client.Auth.CurrentUser;

                if (_user == null)
                    return new NetworkResult("User is not authenticated.");

                await Client.From<GameDatabase>().Upsert(new GameDatabase
                {
                    Id = _user.Id,
                    GameData = gameData
                });

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
                return new NetworkResult<DateTime>("Supabase client is not initialized.");

            if (!await NETWORK.IsConnection())
                return new NetworkResult<DateTime>("No internet connection.");

            try
            {
                var _response = await Client.Rpc("get_server_time", new Dictionary<string, object>()).WithTimeout(5f);
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