#if SUPABASE

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace ACore
{
    public class AuthManager : GlobalBehaviour
    {
        private string editorEmail = "test@gmail.com";
        private string editorPassword = "test123";

        private bool isAuthenticating;

        public override async Task PostInitializeAsync()
        {
            await InitializeAuthentication().WithTimeout(5);
        }

        private async Task InitializeAuthentication()
        {
            while (SupabaseManager.Client == null) await Task.Delay(100);

            if (!await Network.IsConnection())
            {
                Debug.LogError("Tidak ada koneksi internet.");
                return;
            }

#if UNITY_EDITOR
            await LoginEditorAccount();
#else
            await RestoreSession();
#endif
        }

#if UNITY_EDITOR

        private async Task<NetworkResult> LoginEditorAccount()
        {
            if (isAuthenticating)
                return new NetworkResult(
                    "Already authenticating."
                );

            isAuthenticating = true;

            try
            {
                var _user =
                    SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                {
                    await SupabaseManager.Client.Auth.SignIn(
                        editorEmail,
                        editorPassword
                    );

                    _user =
                        SupabaseManager.Client.Auth.CurrentUser;
                }

                if (_user == null)
                {
                    return new NetworkResult(
                        "Login Editor gagal. User null."
                    );
                }

                return await InitializePlayerData(
                    _user.Id
                );
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);

                return new NetworkResult(
                    _e.Message
                );
            }
            finally
            {
                isAuthenticating = false;
            }
        }

#endif

        private async Task<NetworkResult> RestoreSession()
        {
            try
            {
                var _user =
                    SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                {
                    return new NetworkResult(
                        "Session tidak ditemukan. Silakan login dengan Google."
                    );
                }

                return await InitializePlayerData(
                    _user.Id
                );
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);

                return new NetworkResult(
                    _e.Message
                );
            }
        }

        public async Task<NetworkResult> LoginGoogle(
            string idToken)
        {
            if (SupabaseManager.Client == null)
            {
                return new NetworkResult(
                    "Supabase Client belum siap."
                );
            }

            if (!await Network.IsConnection())
            {
                return new NetworkResult(
                    "No Internet"
                );
            }

            if (string.IsNullOrEmpty(idToken))
            {
                return new NetworkResult(
                    "Google ID Token kosong."
                );
            }

            if (isAuthenticating)
            {
                return new NetworkResult(
                    "Already authenticating."
                );
            }

            isAuthenticating = true;

            try
            {
                await SupabaseManager.Client.Auth
                    .SignInWithIdToken(
                        Supabase.Gotrue.Constants.Provider.Google,
                        idToken
                    );

                var _user =
                    SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                {
                    return new NetworkResult(
                        "Google login berhasil tetapi User null."
                    );
                }

                return await InitializePlayerData(
                    _user.Id
                );
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);

                return new NetworkResult(
                    _e.Message
                );
            }
            finally
            {
                isAuthenticating = false;
            }
        }

        private async Task<NetworkResult> InitializePlayerData(
            string userId)
        {
            var _playerResult =
                await GetPlayerData();

            if (_playerResult.IsSuccess)
                return new NetworkResult();

            if (_playerResult.Error !=
                "PlayerData tidak ditemukan.")
            {
                return new NetworkResult(
                    _playerResult.Error
                );
            }

            return await SavePlayerData(
                userId,
                STORAGE.GetJSON
            );
        }

        public async Task<NetworkResult<PlayerData>> GetPlayerData()
        {
            if (SupabaseManager.Client == null)
            {
                return new NetworkResult<PlayerData>(
                    "Supabase Client belum siap."
                );
            }

            if (!await Network.IsConnection())
            {
                return new NetworkResult<PlayerData>(
                    "No Internet"
                );
            }

            try
            {
                var _user =
                    SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                {
                    return new NetworkResult<PlayerData>(
                        "User belum login."
                    );
                }

                var _response =
                    await SupabaseManager.Client
                        .From<PlayerData>()
                        .Where(x => x.Id == _user.Id)
                        .Get();

                if (_response.Models == null ||
                    _response.Models.Count == 0)
                {
                    return new NetworkResult<PlayerData>(
                        "PlayerData tidak ditemukan."
                    );
                }

                return new NetworkResult<PlayerData>(
                    _response.Models[0]
                );
            }
            catch (Exception _e)
            {
                return new NetworkResult<PlayerData>(
                    _e.Message
                );
            }
        }

        public async Task<NetworkResult> SavePlayerData(
            string gameData)
        {
            if (SupabaseManager.Client == null)
            {
                return new NetworkResult(
                    "Supabase Client belum siap."
                );
            }

            if (!await Network.IsConnection())
            {
                return new NetworkResult(
                    "No Internet"
                );
            }

            try
            {
                var _user =
                    SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                {
                    return new NetworkResult(
                        "User belum login."
                    );
                }

                return await SavePlayerData(
                    _user.Id,
                    gameData
                );
            }
            catch (Exception _e)
            {
                return new NetworkResult(
                    _e.Message
                );
            }
        }

        private async Task<NetworkResult> SavePlayerData(
            string userId,
            string gameData)
        {
            try
            {
                var _playerData = new PlayerData
                {
                    Id = userId,
                    GameData = gameData
                };

                await SupabaseManager.Client
                    .From<PlayerData>()
                    .Upsert(_playerData);

                return new NetworkResult();
            }
            catch (Exception _e)
            {
                return new NetworkResult(
                    _e.Message
                );
            }
        }

        public async Task<NetworkResult<bool>> Logout()
        {
            if (SupabaseManager.Client == null)
            {
                return new NetworkResult<bool>(
                    "Supabase Client belum siap."
                );
            }

            if (!await Network.IsConnection())
            {
                return new NetworkResult<bool>(
                    "No Internet"
                );
            }

            try
            {
                await SupabaseManager.Client.Auth.SignOut();

                return new NetworkResult<bool>(true);
            }
            catch (Exception _e)
            {
                return new NetworkResult<bool>(
                    _e.Message
                );
            }
        }

        private void HandleAuthException(Exception _e)
        {
            string _message = _e.Message;

            if (_message.Contains(
                "over_email_send_rate_limit"))
            {
                Debug.LogError(
                    "Supabase Email Rate Limit terkena."
                );

                return;
            }

            if (_message.Contains(
                "Invalid API key"))
            {
                Debug.LogError(
                    "Supabase API Key tidak valid."
                );

                return;
            }

            if (_message.Contains(
                "invalid_credentials"))
            {
                Debug.LogError(
                    "Login Editor gagal. Email atau password salah."
                );

                return;
            }

            Debug.LogError(
                $"Supabase Auth Error:\n{_message}"
            );
        }
    }
}

#endif