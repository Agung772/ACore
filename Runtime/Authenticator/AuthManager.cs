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

        public override async Task PostInitializeAsync() => await Setup().WithTimeout(5);

        private async Task Setup()
        {
            while (SupabaseManager.Client == null)
                await Task.Delay(100);

            if (!await NETWORK.IsConnection())
            {
                Debug.LogError("[Auth] Initialization failed: no internet connection.");
                return;
            }

#if UNITY_EDITOR

            var _result = await LoginEditorAccount();

            if (!_result.IsSuccess)
                Debug.LogError($"[Auth] Editor authentication failed: {_result.Error}");

#else

            var _result = await RestoreSession();

            if (_result.IsSuccess)
            {
                Debug.Log("[Auth] Existing session restored successfully.");
                return;
            }

            Debug.Log($"[Auth] No valid session found: {_result.Error}");

            _result = await LoginGoogle();

            if (!_result.IsSuccess)
            {
                Debug.LogError($"[Auth] Google authentication failed: {_result.Error}");
                return;
            }

            Debug.Log("[Auth] Google authentication completed successfully.");

#endif
        }

        private async Task<NetworkResult> InitializeGameData()
        {
            Debug.Log("[Auth] Initializing game data...");

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

            Debug.Log("[Auth] No game data found. Creating initial game data...");

            var _saveResult = await SupabaseManager.SaveData(STORAGE.GetJSON());

            if (_saveResult.IsSuccess)
                Debug.Log("[Auth] Initial game data created successfully.");

            return _saveResult;
        }

#if UNITY_EDITOR

        private async Task<NetworkResult> LoginEditorAccount()
        {
            if (isAuthenticating)
                return new NetworkResult("Authentication is already in progress.");

            isAuthenticating = true;

            try
            {
                Debug.Log("[Auth] Authenticating with editor account...");

                var _user = SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                {
                    await SupabaseManager.Client.Auth.SignIn(editorEmail, editorPassword);
                    _user = SupabaseManager.Client.Auth.CurrentUser;
                }

                if (_user == null)
                    return new NetworkResult("Editor authentication succeeded but user is null.");

                Debug.Log($"[Auth] Editor authentication successful. User ID: {_user.Id}");

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
                Debug.Log("[Auth] Checking for existing session...");

                var _user = SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                    return new NetworkResult("No active session found.");

                Debug.Log($"[Auth] Existing session found. User ID: {_user.Id}");

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

            if (!await NETWORK.IsConnection())
                return new NetworkResult("No internet connection.");

            if (isAuthenticating)
                return new NetworkResult("Authentication is already in progress.");

            isAuthenticating = true;

            try
            {
                Debug.Log("[Auth] Starting Google authentication...");

                GoogleSignIn.Configuration = new GoogleSignInConfiguration
                {
                    WebClientId = GAME.GetSO<ASettingData>().supabase.clintID,
                    RequestIdToken = true,
                    RequestEmail = true
                };

                var _googleUser = await GoogleSignIn.DefaultInstance.SignIn();

                if (_googleUser == null)
                    return new NetworkResult("Google authentication returned a null user.");

                if (string.IsNullOrEmpty(_googleUser.IdToken))
                    return new NetworkResult("Google authentication returned an empty ID token.");

                Debug.Log($"[Auth] Google authentication successful. Email: {_googleUser.Email}");

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
            try
            {
                Debug.Log("[Auth] Signing in to Supabase with Google ID token...");

                await SupabaseManager.Client.Auth.SignInWithIdToken(
                    Supabase.Gotrue.Constants.Provider.Google,
                    _idToken
                );

                var _user = SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                    return new NetworkResult("Google authentication succeeded but Supabase user is null.");

                Debug.Log($"[Auth] Supabase Google authentication successful. User ID: {_user.Id}");

                return await InitializeGameData();
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

            if (!await NETWORK.IsConnection())
                return new NetworkResult<bool>("No internet connection.");

            try
            {
                Debug.Log("[Auth] Signing out...");

                await SupabaseManager.Client.Auth.SignOut();

                Debug.Log("[Auth] Sign out completed successfully.");

                return new NetworkResult<bool>(true);
            }
            catch (Exception _e)
            {
                Debug.LogError($"[Auth] Sign out failed: {_e}");
                return new NetworkResult<bool>(_e.Message);
            }
        }

        private void HandleAuthException(Exception _e)
        {
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
                Debug.LogError("[Auth] Editor authentication failed: invalid email or password.");
                return;
            }

            Debug.LogError($"[Auth] Authentication error: {_message}");
        }
    }
}

#endif