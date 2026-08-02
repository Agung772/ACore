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

        public override IEnumerator PostInitializeCoroutine()
        {
#if UNITY_EDITOR
            var _account = LoginEditorAccount();
            yield return new WaitUntil(() => _account.IsCompleted);
#endif

            yield return null;
        }

#if UNITY_EDITOR
        private async Task<NetworkResult> LoginEditorAccount()
        {
            if (isAuthenticating)
                return new NetworkResult("Already authenticating.");

            isAuthenticating = true;

            try
            {
                while (SupabaseManager.Client == null)
                    await Task.Delay(100);

                if (!await NETWORK.IsConnection())
                    return new NetworkResult("No Internet");

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
                    return new NetworkResult(
                        "Login gagal. User null."
                    );

                var _playerResult =
                    await GetPlayerData();

                if (!_playerResult.IsSuccess)
                {
                    var _saveResult =
                        await SavePlayerData(
                            _user.Id,
                            STORAGE.GetJSON
                        );

                    if (!_saveResult.IsSuccess)
                        return _saveResult;
                }

                return new NetworkResult();
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);
                return new NetworkResult(_e.Message);
            }
            finally
            {
                isAuthenticating = false;
            }
        }
#endif

        public async Task<NetworkResult> CreateAccount(
            string email,
            string password)
        {
            if (SupabaseManager.Client == null)
            {
                return new NetworkResult(
                    "Supabase Client belum siap."
                );
            }

            if (!await NETWORK.IsConnection())
            {
                return new NetworkResult(
                    "No Internet"
                );
            }

            try
            {
                var _session =
                    await SupabaseManager.Client.Auth.SignUp(
                        email,
                        password
                    );

                if (_session == null)
                {
                    return new NetworkResult(
                        "SignUp berhasil tetapi session null. Kemungkinan Email Confirmation aktif."
                    );
                }

                var _user =
                    SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                {
                    return new NetworkResult(
                        "Account dibuat tetapi User null."
                    );
                }

                var _saveResult =
                    await SavePlayerData(
                        _user.Id,
                        STORAGE.GetJSON
                    );

                if (!_saveResult.IsSuccess)
                {
                    return new NetworkResult(
                        _saveResult.Error
                    );
                }

                return new NetworkResult();
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);

                return new NetworkResult(
                    _e.Message
                );
            }
        }

        public async Task<NetworkResult<PlayerData>> GetPlayerData()
        {
            if (SupabaseManager.Client == null)
            {
                return new NetworkResult<PlayerData>(
                    "Supabase Client belum siap."
                );
            }

            if (!await NETWORK.IsConnection())
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

        private async Task<NetworkResult> SavePlayerData(
            string userId,
            string gameData)
        {
            if (SupabaseManager.Client == null)
            {
                return new NetworkResult(
                    "Supabase Client belum siap."
                );
            }

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

        public async Task<NetworkResult> Login(
            string email,
            string password)
        {
            if (SupabaseManager.Client == null)
            {
                return new NetworkResult(
                    "Supabase Client belum siap."
                );
            }

            if (!await NETWORK.IsConnection())
            {
                return new NetworkResult(
                    "No Internet"
                );
            }

            try
            {
                await SupabaseManager.Client.Auth.SignIn(
                    email,
                    password
                );

                var _user =
                    SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                {
                    return new NetworkResult(
                        "Login gagal. User null."
                    );
                }

                var _playerResult =
                    await GetPlayerData();

                if (!_playerResult.IsSuccess)
                {
                    return new NetworkResult(
                        _playerResult.Error
                    );
                }

                return new NetworkResult();
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);

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

            if (!await NETWORK.IsConnection())
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

            if (_message.Contains("over_email_send_rate_limit"))
            {
                Debug.LogError(
                    "Supabase Email Rate Limit terkena."
                );

                return;
            }

            if (_message.Contains("Invalid API key"))
            {
                Debug.LogError(
                    "Supabase API Key tidak valid."
                );

                return;
            }

            if (_message.Contains("email_address_invalid"))
            {
                Debug.LogError(
                    "Format email ditolak oleh Supabase."
                );

                return;
            }

            if (_message.Contains("invalid_credentials"))
            {
                Debug.LogError(
                    "Email atau password salah."
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