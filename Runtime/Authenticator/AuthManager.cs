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
                Debug.Log("[Auth] Initialization completed");
                return;
            }

            _result = await LoginGoogle();
            if (_result.IsSuccess)
                Debug.Log("[Auth] Initialization completed");
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
                {
                    Debug.LogError($"[Auth] Failed to load game data: {_gameResult.Error}");
                    return new NetworkResult(_gameResult.Error);
                }

                var _saveResult = await SupabaseManager.SaveData(
                    STORAGE.GetJSON()
                );

                if (!_saveResult.IsSuccess)
                    Debug.LogError($"[Auth] Failed to create game data: {_saveResult.Error}");

                return _saveResult;
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
                {
                    Debug.LogError($"[Auth] Failed to load game data: {_gameResult.Error}");
                    return new NetworkResult(_gameResult.Error);
                }

                var _saveResult = await SupabaseManager.SaveData(
                    STORAGE.GetJSON()
                );

                if (!_saveResult.IsSuccess)
                    Debug.LogError($"[Auth] Failed to create game data: {_saveResult.Error}");

                return _saveResult;
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
                var _user = SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                {
                    Debug.Log("[Auth] No active session found.");
                    return new NetworkResult("No active session found.");
                }

                return await InitializeGameData();
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
                return new NetworkResult("Supabase client is not initialized.");

            if (!await NETWORK.IsConnection().WithTimeout(10))
                return new NetworkResult("No internet connection.");

            if (isAuthenticating)
                return new NetworkResult("Authentication is already in progress.");

            isAuthenticating = true;

            try
            {
                var _settings = GAME.GetSO<ASettingData>();

                if (_settings == null)
                    return new NetworkResult("ASettingData is null.");

                if (_settings.supabase == null)
                    return new NetworkResult("Supabase settings are null.");

                string _webClientId = _settings.supabase.webClientID;

                if (string.IsNullOrWhiteSpace(_webClientId))
                    return new NetworkResult("Google Web Client ID is empty.");

                GoogleSignIn.Configuration =
                    new GoogleSignInConfiguration
                    {
                        WebClientId = _webClientId,
                        RequestIdToken = true,
                        RequestEmail = true
                    };

                var _googleUser =
                    await GoogleSignIn.DefaultInstance.SignIn();

                if (_googleUser == null)
                    return new NetworkResult("Google authentication returned a null user.");

                if (string.IsNullOrEmpty(_googleUser.IdToken))
                    return new NetworkResult("Google authentication returned an empty ID token.");

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

        private async Task<NetworkResult> LoginGoogleInternal(string _idToken)
        {
            if (string.IsNullOrEmpty(_idToken))
                return new NetworkResult("Google ID token is empty.");

            try
            {
                await SupabaseManager.Client.Auth.SignInWithIdToken(
                    Supabase.Gotrue.Constants.Provider.Google,
                    _idToken
                );

                var _user = SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                    return new NetworkResult("Google authentication succeeded but Supabase user is null.");

                return await InitializeFirstGameData();
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
                return new NetworkResult<bool>("Supabase client is not initialized.");

            if (!await NETWORK.IsConnection().WithTimeout(10))
                return new NetworkResult<bool>("No internet connection.");

            try
            {
                await SupabaseManager.Client.Auth.SignOut();
                return new NetworkResult<bool>(true);
            }
            catch (Exception _e)
            {
                Debug.LogError($"[Auth] Sign out failed: {_e.Message}");
                return new NetworkResult<bool>(_e.Message);
            }
        }

        private void HandleAuthException(Exception _e)
        {
            if (_e == null)
            {
                Debug.LogError("[Auth] Unknown authentication error.");
                return;
            }

            var _message = _e.Message;

            if (_message.Contains("over_email_send_rate_limit"))
            {
                Debug.LogError("[Auth] Supabase email rate limit exceeded.");
                return;
            }

            if (_message.Contains("Invalid API key"))
            {
                Debug.LogError("[Auth] Supabase API key is invalid.");
                return;
            }

            if (_message.Contains("invalid_credentials"))
            {
                Debug.LogError("[Auth] Editor authentication failed: invalid credentials.");
                return;
            }

            Debug.LogError($"[Auth] Authentication error: {_message}");
        }
    }
}

#endif