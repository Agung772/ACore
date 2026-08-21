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
        private const string editorEmail = "test@gmail.com";
        private const string editorPassword = "test123";
#endif

        private bool isAuthenticating;
        private static bool googleConfigured;

        public override async Task PostInitializeAsync()
        {
            try
            {
                await Setup();
            }
            catch (Exception _e)
            {
                Debug.LogError($"[Auth] Initialization failed: {_e.Message}");
            }

            SCENE.OnLoaded += STORAGE.Save;
        }

        private async Task Setup()
        {
            Debug.Log("[Auth] Initializing...");

            while (SupabaseManager.Client == null)
                await Task.Delay(100);

            if (!await NETWORK.IsConnection().WithTimeout(10))
            {
                Debug.LogError("[Auth] No internet connection.");
                return;
            }

#if UNITY_EDITOR
            var _result = await LoginEditorAccount();

            if (_result.IsSuccess)
                Debug.Log("[Auth] Initialization completed");
            else
                Debug.LogError($"[Auth] Editor authentication failed: {_result.Error}");
#else
            var _result = await RestoreSession();

            if (_result.IsSuccess)
            {
                Debug.Log("[Auth] Supabase session restored.");
                return;
            }

            Debug.Log($"[Auth] Supabase session restore failed: {_result.Error}");

            _result = await LoginGoogleSilently();

            if (_result.IsSuccess)
            {
                Debug.Log("[Auth] Google silent authentication completed.");
                return;
            }

            Debug.Log($"[Auth] Google silent authentication failed: {_result.Error}");

            _result = await LoginGoogle();

            if (_result.IsSuccess)
                Debug.Log("[Auth] Google authentication completed.");
            else
                Debug.LogError($"[Auth] Google authentication failed: {_result.Error}");
#endif
        }

        private async Task<NetworkResult> InitializeFirstGameData()
        {
            try
            {
                var _gameResult = await SupabaseManager.GetData();

                if (_gameResult.IsSuccess)
                {
                    STORAGE.Replace(_gameResult.Value.GameData);
                    return new NetworkResult();
                }

                if (_gameResult.Error != "Game data not found.")
                    return new NetworkResult(_gameResult.Error);

                return await SupabaseManager.SaveData(STORAGE.GetJSON());
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);
                return new NetworkResult(_e.Message);
            }
        }

        private async Task<NetworkResult> InitializeGameData()
        {
            try
            {
                var _gameResult = await SupabaseManager.GetData();

                if (_gameResult.IsSuccess)
                {
                    STORAGE.TryReplace(_gameResult.Value.GameData);
                    return new NetworkResult();
                }

                if (_gameResult.Error != "Game data not found.")
                    return new NetworkResult(_gameResult.Error);

                return await SupabaseManager.SaveData(STORAGE.GetJSON());
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);
                return new NetworkResult(_e.Message);
            }
        }

#if UNITY_EDITOR
        private async Task<NetworkResult> LoginEditorAccount()
        {
            if (isAuthenticating)
                return new NetworkResult("Authentication is already in progress.");

            isAuthenticating = true;

            try
            {
                var _user = SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                {
                    await SupabaseManager.Client.Auth.SignIn(
                        editorEmail,
                        editorPassword
                    );

                    _user = SupabaseManager.Client.Auth.CurrentUser;
                }

                if (_user == null)
                    return new NetworkResult(
                        "Editor authentication succeeded but user is null."
                    );

                return await InitializeGameData();
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
                if (SupabaseManager.Client == null)
                    return new NetworkResult(
                        "Supabase client is not initialized."
                    );

                var _session = SupabaseManager.Client.Auth.CurrentSession;
                var _user = SupabaseManager.Client.Auth.CurrentUser;

                if (_session == null || _user == null)
                {
                    Debug.Log("[Auth] No active Supabase session.");
                    return new NetworkResult("No active session found.");
                }

                Debug.Log($"[Auth] Supabase session restored: {_user.Id}");

                return await InitializeGameData();
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);
                return new NetworkResult(_e.Message);
            }
        }

        private NetworkResult ConfigureGoogleSignIn()
        {
            if (googleConfigured)
                return new NetworkResult();

            var _settings = GAME.GetSO<ASettingData>();

            if (_settings == null)
                return new NetworkResult("ASettingData is null.");

            if (_settings.supabase == null)
                return new NetworkResult("Supabase settings are null.");

            var _webClientId = _settings.supabase.webClientID;

            if (string.IsNullOrWhiteSpace(_webClientId))
                return new NetworkResult("Google Web Client ID is empty.");

            try
            {
                GoogleSignIn.Configuration = new GoogleSignInConfiguration
                {
                    WebClientId = _webClientId,
                    RequestIdToken = true,
                    RequestEmail = true
                };

                googleConfigured = true;

                return new NetworkResult();
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);
                return new NetworkResult(_e.Message);
            }
        }

        private async Task<NetworkResult> LoginGoogleSilently()
        {
            if (SupabaseManager.Client == null)
                return new NetworkResult(
                    "Supabase client is not initialized."
                );

            if (!await NETWORK.IsConnection().WithTimeout(10))
                return new NetworkResult("No internet connection.");

            if (isAuthenticating)
                return new NetworkResult(
                    "Authentication is already in progress."
                );

            isAuthenticating = true;

            try
            {
                var _configurationResult = ConfigureGoogleSignIn();

                if (!_configurationResult.IsSuccess)
                    return _configurationResult;

                GoogleSignInUser _googleUser;

                try
                {
                    _googleUser =
                        await GoogleSignIn.DefaultInstance.SignInSilently();
                }
                catch (Exception _e)
                {
                    return new NetworkResult(
                        $"Google silent sign-in failed: {_e.Message}"
                    );
                }

                if (_googleUser == null)
                    return new NetworkResult(
                        "Google silent sign-in returned null user."
                    );

                if (string.IsNullOrEmpty(_googleUser.IdToken))
                    return new NetworkResult(
                        "Google silent sign-in returned an empty ID token."
                    );

                Debug.Log(
                    $"[Auth] Google silent sign-in succeeded: {_googleUser.Email}"
                );

                return await LoginGoogleInternal(
                    _googleUser.IdToken,
                    false
                );
            }
            finally
            {
                isAuthenticating = false;
            }
        }

        public async Task<NetworkResult> LoginGoogle()
        {
            if (SupabaseManager.Client == null)
                return new NetworkResult(
                    "Supabase client is not initialized."
                );

            if (!await NETWORK.IsConnection().WithTimeout(10))
                return new NetworkResult("No internet connection.");

            if (isAuthenticating)
                return new NetworkResult(
                    "Authentication is already in progress."
                );

            isAuthenticating = true;

            try
            {
                var _configurationResult = ConfigureGoogleSignIn();

                if (!_configurationResult.IsSuccess)
                    return _configurationResult;

                GoogleSignInUser _googleUser;

                try
                {
                    _googleUser =
                        await GoogleSignIn.DefaultInstance.SignIn();
                }
                catch (Exception _e)
                {
                    HandleAuthException(_e);
                    return new NetworkResult(_e.Message);
                }

                if (_googleUser == null)
                    return new NetworkResult(
                        "Google authentication returned a null user."
                    );

                if (string.IsNullOrEmpty(_googleUser.IdToken))
                    return new NetworkResult(
                        "Google authentication returned an empty ID token."
                    );

                Debug.Log(
                    $"[Auth] Google interactive sign-in succeeded: {_googleUser.Email}"
                );

                return await LoginGoogleInternal(
                    _googleUser.IdToken,
                    true
                );
            }
            finally
            {
                isAuthenticating = false;
            }
        }

        private async Task<NetworkResult> LoginGoogleInternal(
            string idToken,
            bool isInteractive)
        {
            if (string.IsNullOrEmpty(idToken))
                return new NetworkResult(
                    "Google ID token is empty."
                );

            try
            {
                await SupabaseManager.Client.Auth.SignInWithIdToken(
                    Supabase.Gotrue.Constants.Provider.Google,
                    idToken
                );

                var _user = SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                    return new NetworkResult(
                        "Google authentication succeeded but Supabase user is null."
                    );

                Debug.Log(
                    $"[Auth] Supabase Google login succeeded: {_user.Id}"
                );

                return isInteractive
                    ? await InitializeFirstGameData()
                    : await InitializeGameData();
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
                return new NetworkResult<bool>(
                    "Supabase client is not initialized."
                );

            if (!await NETWORK.IsConnection().WithTimeout(10))
                return new NetworkResult<bool>(
                    "No internet connection."
                );

            try
            {
                await SupabaseManager.Client.Auth.SignOut();

                return new NetworkResult<bool>(true);
            }
            catch (Exception _e)
            {
                Debug.LogError(
                    $"[Auth] Sign out failed: {_e.Message}"
                );

                return new NetworkResult<bool>(_e.Message);
            }
        }

        private void HandleAuthException(Exception e)
        {
            if (e == null)
            {
                Debug.LogError(
                    "[Auth] Unknown authentication error."
                );

                return;
            }

            var _message = e.Message;

            if (_message.Contains("over_email_send_rate_limit"))
            {
                Debug.LogError(
                    "[Auth] Supabase email rate limit exceeded."
                );

                return;
            }

            if (_message.Contains("Invalid API key"))
            {
                Debug.LogError(
                    "[Auth] Supabase API key is invalid."
                );

                return;
            }

            if (_message.Contains("invalid_credentials"))
            {
                Debug.LogError(
                    "[Auth] Editor authentication failed: invalid credentials."
                );

                return;
            }

            if (_message.Contains("DefaultInstance already created"))
            {
                Debug.LogError(
                    "[Auth] Google Sign-In DefaultInstance was already created."
                );

                return;
            }

            Debug.LogError(
                $"[Auth] Authentication error: {_message}"
            );
        }
    }
}

#endif