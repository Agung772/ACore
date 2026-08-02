#if SUPABASE

using System;
using System.Collections;
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

    private const int InitializeTimeout = 15;

    public override IEnumerator InitializeCoroutine()
    {
        Task<NetworkResult> setupTask = Setup();

        float timer = 0f;

        while (!setupTask.IsCompleted && timer < InitializeTimeout)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        if (!setupTask.IsCompleted)
        {
            Client = null;
            Debug.LogError("Supabase initialization timeout.");
            yield break;
        }

        NetworkResult result = setupTask.Result;

        if (!result.IsSuccess)
        {
            Client = null;
            Debug.LogError(result.Error);
            yield break;
        }

        Debug.Log("Supabase Ready");
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

            Task initializeTask = Client.InitializeAsync();

            Task completedTask = await Task.WhenAny(
                initializeTask,
                Task.Delay(TimeSpan.FromSeconds(InitializeTimeout))
            );

            if (completedTask != initializeTask)
            {
                Client = null;
                return new NetworkResult("Supabase InitializeAsync timeout.");
            }

            await initializeTask;

            return new NetworkResult();
        }
        catch (Exception e)
        {
            Client = null;
            return new NetworkResult($"Supabase Setup Error: {e}");
        }
    }

    public async void SendPlayerData(
        string username,
        int level,
        int coin,
        string gameData)
    {
        if (Client == null)
        {
            Debug.LogError("Supabase Client belum siap.");
            return;
        }

        try
        {
            var user = Client.Auth.CurrentUser;

            if (user == null)
            {
                Debug.LogError("User belum login.");
                return;
            }

            var playerData = new PlayerData
            {
                Id = user.Id,
                GameData = gameData
            };

            await Client
                .From<PlayerData>()
                .Upsert(playerData);

            Debug.Log("PlayerData berhasil dikirim ke Supabase.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Gagal mengirim PlayerData: {e}");
        }
    }
}

}

#endif
