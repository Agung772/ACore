#if SUPABASE

using System;
using System.Threading.Tasks;
using UnityEngine;
using Google;

namespace ACore
{
    public class AuthManager : GlobalBehaviour
    {
#if UNITY_EDITOR
        private string editorEmail = "test@gmail.com";
        private string editorPassword = "test123";
#endif

        private bool isAuthenticating;

        public override async Task PostInitializeAsync() => await Setup().WithTimeout(5);

        private async Task Setup()
        {
            while (SupabaseManager.Client == null)
                await Task.Delay(100);

            if (!await NETWORK.IsConnection())
            {
                Debug.LogError("Tidak ada koneksi internet.");
                return;
            }

#if UNITY_EDITOR

            var _result = await LoginEditorAccount();

            if (!_result.IsSuccess)
                Debug.LogError($"Editor Auth gagal: {_result.Error}");

#else

            var _result = await RestoreSession();

            if (_result.IsSuccess)
            {
                Debug.Log("Supabase session berhasil di-restore.");
                return;
            }

            Debug.Log($"Supabase session tidak ditemukan: {_result.Error}");

            _result = await LoginGoogle();

            if (!_result.IsSuccess)
            {
                Debug.LogError($"Google Auth gagal: {_result.Error}");
                return;
            }

            Debug.Log("Google Auth berhasil.");

#endif
        }

        private async Task<NetworkResult> InitializePlayerData()
        {
            var _playerResult = await SupabaseManager.GetPlayerData();

            if (_playerResult.IsSuccess)
            {
                STORAGE.TryReplace(_playerResult.Value.GameData);
                return new NetworkResult();
            }

            if (_playerResult.Error != "PlayerData tidak ditemukan.")
                return new NetworkResult(_playerResult.Error);

            return await SupabaseManager.SavePlayerData(STORAGE.GetJSON());
        }

#if UNITY_EDITOR

        private async Task<NetworkResult> LoginEditorAccount()
        {
            if (isAuthenticating)
                return new NetworkResult("Already authenticating.");

            isAuthenticating = true;

            try
            {
                var _user = SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                {
                    await SupabaseManager.Client.Auth.SignIn(editorEmail, editorPassword);
                    _user = SupabaseManager.Client.Auth.CurrentUser;
                }

                if (_user == null)
                    return new NetworkResult("Login Editor gagal. User null.");

                Debug.Log($"Supabase Editor Login berhasil: {_user.Id}");

                return await InitializePlayerData();
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

        private async Task<NetworkResult> RestoreSession()
        {
            try
            {
                var _user = SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                    return new NetworkResult("Session tidak ditemukan.");

                Debug.Log($"Supabase session ditemukan: {_user.Id}");

                return await InitializePlayerData();
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);
                return new NetworkResult(_e.Message);
            }
        }

        public async Task<NetworkResult> LoginGoogle()
        {
            if (SupabaseManager.Client == null)
                return new NetworkResult("Supabase Client belum siap.");

            if (!await NETWORK.IsConnection())
                return new NetworkResult("No Internet");

            if (isAuthenticating)
                return new NetworkResult("Already authenticating.");

            isAuthenticating = true;

            try
            {
                GoogleSignIn.Configuration = new GoogleSignInConfiguration
                {
                    WebClientId = GAME.GetSO<ASettingData>().supabase.clintID,
                    RequestIdToken = true,
                    RequestEmail = true
                };

                var _googleUser = await GoogleSignIn.DefaultInstance.SignIn();

                if (_googleUser == null)
                    return new NetworkResult("Google User null.");

                if (string.IsNullOrEmpty(_googleUser.IdToken))
                    return new NetworkResult("Google ID Token kosong.");

                Debug.Log($"Google Login berhasil: {_googleUser.Email}");
                Debug.Log($"Google ID Token: {_googleUser.IdToken}");

                return await LoginGoogleInternal(_googleUser.IdToken);
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

        private async Task<NetworkResult> LoginGoogleInternal(string idToken)
        {
            try
            {
                await SupabaseManager.Client.Auth.SignInWithIdToken(Supabase.Gotrue.Constants.Provider.Google, idToken);

                var _user = SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                    return new NetworkResult("Google login berhasil tetapi User null.");

                Debug.Log($"Supabase Google Login berhasil: {_user.Id}");

                return await InitializePlayerData();
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);
                return new NetworkResult(_e.Message);
            }
        }

        public async Task<NetworkResult<bool>> Logout()
        {
            if (SupabaseManager.Client == null)
                return new NetworkResult<bool>("Supabase Client belum siap.");

            if (!await NETWORK.IsConnection())
                return new NetworkResult<bool>("No Internet");

            try
            {
                await SupabaseManager.Client.Auth.SignOut();
                return new NetworkResult<bool>(true);
            }
            catch (Exception _e)
            {
                return new NetworkResult<bool>(_e.Message);
            }
        }

        private void HandleAuthException(Exception _e)
        {
            string _message = _e.Message;

            if (_message.Contains("over_email_send_rate_limit"))
            {
                Debug.LogError("Supabase Email Rate Limit terkena.");
                return;
            }

            if (_message.Contains("Invalid API key"))
            {
                Debug.LogError("Supabase API Key tidak valid.");
                return;
            }

            if (_message.Contains("invalid_credentials"))
            {
                Debug.LogError("Login Editor gagal. Email atau password salah.");
                return;
            }

            Debug.LogError($"Supabase Auth Error:\n{_message}");
        }
    }
}

#endif