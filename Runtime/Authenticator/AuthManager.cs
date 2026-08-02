#if SUPABASE

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace ACore
{
    public class AuthManager : GlobalBehaviour
    {
        [Header("Editor Account")]
        private string editorEmail = "test@gmail.com";
        private string editorPassword = "test123";

        private bool isAuthenticating;

        public override IEnumerator PostInitializeCoroutine()
        {
#if UNITY_EDITOR
            var _account = LoginEditorAccount();
            yield return new WaitUntil(() => _account.IsCompleted);
            
            var _currentUser = SupabaseManager.Client.Auth.CurrentUser;
            
            SavePlayerData(_currentUser.Id, STORAGE.GetJSON);
#endif
        }

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
                {
                    Debug.LogError("Tidak ada koneksi internet.");
                    return new NetworkResult("No Internet");
                }

                Debug.Log("Supabase Client siap.");

                var _currentUser =
                    SupabaseManager.Client.Auth.CurrentUser;

                if (_currentUser != null)
                {
                    Debug.Log(
                        $"Session ditemukan: {_currentUser.Id}"
                    );

                    var _saveResult =
                        await SavePlayerData(_currentUser.Id);

                    if (!_saveResult.IsSuccess)
                    {
                        Debug.LogError(
                            $"Gagal menyimpan PlayerData: {_saveResult.Error}"
                        );

                        return new NetworkResult(
                            _saveResult.Error
                        );
                    }

                    return new NetworkResult();
                }

                Debug.Log(
                    $"Login Editor Account: {editorEmail}"
                );

                await SupabaseManager.Client.Auth.SignIn(
                    editorEmail,
                    editorPassword
                );

                _currentUser =
                    SupabaseManager.Client.Auth.CurrentUser;

                if (_currentUser == null)
                {
                    const string error =
                        "Login berhasil tetapi CurrentUser null.";

                    Debug.LogError(error);

                    return new NetworkResult(error);
                }

                Debug.Log(
                    $"Login berhasil: {_currentUser.Id}"
                );

                var _result =
                    await SavePlayerData(_currentUser.Id);

                if (!_result.IsSuccess)
                {
                    Debug.LogError(
                        $"Gagal menyimpan PlayerData: {_result.Error}"
                    );

                    return new NetworkResult(
                        _result.Error
                    );
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

        public async Task<NetworkResult<bool>> CreateAccount(
            string email,
            string password)
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
                Debug.Log(
                    $"Membuat akun: {email}"
                );

                var _session =
                    await SupabaseManager.Client.Auth.SignUp(
                        email,
                        password
                    );

                if (_session == null)
                {
                    return new NetworkResult<bool>(
                        "SignUp berhasil tetapi session null. Kemungkinan Email Confirmation aktif."
                    );
                }

                var _user =
                    SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                {
                    return new NetworkResult<bool>(
                        "Account dibuat tetapi User null."
                    );
                }

                Debug.Log(
                    $"Account berhasil dibuat: {_user.Id}"
                );

                var _saveResult =
                    await SavePlayerData(_user.Id);

                if (!_saveResult.IsSuccess)
                {
                    return new NetworkResult<bool>(
                        _saveResult.Error
                    );
                }

                return new NetworkResult<bool>(true);
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);

                return new NetworkResult<bool>(
                    _e.Message
                );
            }
        }

        private async Task<NetworkResult> SavePlayerData(
            string userId, string gameData = "")
        {
            if (SupabaseManager.Client == null)
            {
                return new NetworkResult<bool>(
                    "Supabase Client belum siap."
                );
            }

            try
            {
                PlayerData _playerData = new PlayerData
                {
                    Id = userId,
                    GameData = gameData
                };

                await SupabaseManager.Client
                    .From<PlayerData>()
                    .Upsert(_playerData);

                Debug.Log("PlayerData berhasil disimpan.");
                Debug.Log($"User ID : {userId}");

                return new NetworkResult();
            }
            catch (Exception _e)
            {
                return new NetworkResult(
                    _e.Message
                );
            }
        }

        private void HandleAuthException(Exception e)
        {
            string _message = e.Message;

            if (_message.Contains("over_email_send_rate_limit"))
            {
                Debug.LogError(
                    "Supabase Email Rate Limit terkena. Tunggu sampai rate limit reset."
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

        public async Task<NetworkResult<bool>> Login(
            string email,
            string password)
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
                await SupabaseManager.Client.Auth.SignIn(
                    email,
                    password
                );

                var _user =
                    SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                {
                    return new NetworkResult<bool>(
                        "Login gagal. User null."
                    );
                }

                Debug.Log(
                    $"Login berhasil: {_user.Id}"
                );

                var _saveResult =
                    await SavePlayerData(_user.Id);

                if (!_saveResult.IsSuccess)
                {
                    return new NetworkResult<bool>(
                        _saveResult.Error
                    );
                }

                return new NetworkResult<bool>(true);
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);

                return new NetworkResult<bool>(
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

                Debug.Log("Logout berhasil.");

                return new NetworkResult<bool>(true);
            }
            catch (Exception _e)
            {
                Debug.LogError(
                    $"Logout gagal:\n{_e.Message}"
                );

                return new NetworkResult<bool>(
                    _e.Message
                );
            }
        }
    }
}

#endif