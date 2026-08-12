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
                Debug.Log("Supabase Setup...");

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

                Debug.Log("Supabase Client Created");

                await Client.InitializeAsync();

                Debug.Log("Supabase Ready");

                return new NetworkResult();
            }
            catch (Exception _e)
            {
                Client = null;
                return new NetworkResult($"Supabase Setup Error: {_e}");
            }
        }

        public static async Task<NetworkResult<GameDatabase>> GetPlayerData()
        {
            if (Client == null)
                return new NetworkResult<GameDatabase>("Supabase Client belum siap.");

            if (!await NETWORK.IsConnection())
                return new NetworkResult<GameDatabase>("No Internet");

            try
            {
                var _user = Client.Auth.CurrentUser;

                if (_user == null)
                    return new NetworkResult<GameDatabase>("User belum login.");

                var _response = await Client
                    .From<GameDatabase>()
                    .Where(x => x.Id == _user.Id)
                    .Get();

                if (_response.Models == null || _response.Models.Count == 0)
                    return new NetworkResult<GameDatabase>("PlayerData tidak ditemukan.");

                return new NetworkResult<GameDatabase>(_response.Models[0]);
            }
            catch (Exception _e)
            {
                return new NetworkResult<GameDatabase>(_e.Message);
            }
        }

        public static async Task<NetworkResult> SavePlayerData()
        {
            return await SavePlayerData(STORAGE.GetJSON());
        }
        
        public static async Task<NetworkResult> SavePlayerData(string gameData)
        {
            Debug.Log("Start Saving Game Data to server");
            
            if (Client == null)
                return new NetworkResult("Supabase Client belum siap.");

            if (!await NETWORK.IsConnection())
                return new NetworkResult("No Internet");

            try
            {
                var _user = Client.Auth.CurrentUser;

                if (_user == null)
                    return new NetworkResult("User belum login.");

                var _playerData = new GameDatabase
                {
                    Id = _user.Id,
                    GameData = gameData
                };

                await Client
                    .From<GameDatabase>()
                    .Upsert(_playerData);

                Debug.Log("Successfully saved the game data to server.");

                return new NetworkResult();
            }
            catch (Exception _e)
            {
                Debug.Log($"Failed to save the game data to server. {_e}");
                return new NetworkResult(_e.Message);
            }
        }

        public static async Task<NetworkResult<DateTime>> GetTime()
        {
            if (Client == null)
                return new NetworkResult<DateTime>("Supabase Client belum siap.");

            if (!await NETWORK.IsConnection())
                return new NetworkResult<DateTime>("No Internet");

            try
            {
                var _response = await Client
                    .Rpc(
                        "get_server_time",
                        new Dictionary<string, object>()
                    )
                    .WithTimeout(5f);

                return new NetworkResult<DateTime>(
                    DateTime.Parse(_response.Content).ToUniversalTime()
                );
            }
            catch (Exception _e)
            {
                return new NetworkResult<DateTime>(
                    $"Supabase Get Time Error: {_e}"
                );
            }
        }
    }
}

#endif