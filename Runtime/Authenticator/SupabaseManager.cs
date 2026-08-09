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

        private const string Url = "https://uqmurtxybeatlfgkbuuc.supabase.co";
        private const string AnonKey = "sb_publishable_XZdKyPz5_vNPded-qId-uA_icHWMDJj";

        public override async Task InitializeAsync()
        {
            await Setup().WithTimeout(5);
        }

        private async Task<NetworkResult> Setup()
        {
            try
            {
                Debug.Log("Supabase Setup...");

                Client = new Client(
                    Url,
                    AnonKey,
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


        public async void SendPlayerData(string username, string gameData)
        {
            if (Client == null)
            {
                Debug.LogError("Supabase Client belum siap.");
                return;
            }

            try
            {
                var _user = Client.Auth.CurrentUser;

                if (_user == null)
                {
                    Debug.LogError("User belum login.");
                    return;
                }

                var _playerData = new GameDatabase
                {
                    Id = _user.Id,
                    GameData = gameData
                };

                await Client
                    .From<GameDatabase>()
                    .Upsert(_playerData);

                Debug.Log("PlayerData berhasil dikirim ke Supabase.");
            }
            catch (Exception _e)
            {
                Debug.LogError($"Gagal mengirim PlayerData: {_e}");
            }
        }


        public async Task<NetworkResult<DateTime>> GetTime()
        {
            try
            {
                var _response = await Client
                    .Rpc(
                        "get_server_time",
                        new Dictionary<string, object>()
                    )
                    .WithTimeout(5f);

                return new NetworkResult<DateTime>(
                    DateTime.Parse(_response.Content)
                        .ToUniversalTime()
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