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
            Debug.Log("========== [Auth] POST INITIALIZE START ==========");

            try
            {
                await Setup().WithTimeout(5);
            }
            catch (Exception _e)
            {
                Debug.LogError("========== [Auth] POST INITIALIZE EXCEPTION ==========");
                Debug.LogException(_e);
            }

            Debug.Log("========== [Auth] POST INITIALIZE END ==========");
        }

        private async Task Setup()
        {
            Debug.Log("========== [Auth] SETUP START ==========");

            Debug.Log($"[Auth] Platform: {Application.platform}");
            Debug.Log($"[Auth] Unity Version: {Application.unityVersion}");
            Debug.Log($"[Auth] Application Identifier: {Application.identifier}");
            Debug.Log($"[Auth] Application Version: {Application.version}");
            Debug.Log($"[Auth] Device Model: {SystemInfo.deviceModel}");
            Debug.Log($"[Auth] Device Name: {SystemInfo.deviceName}");
            Debug.Log($"[Auth] Operating System: {SystemInfo.operatingSystem}");

            Debug.Log("[Auth] Waiting for Supabase client...");

            int _waitCount = 0;

            while (SupabaseManager.Client == null)
            {
                await Task.Delay(100);

                _waitCount++;

                if (_waitCount % 10 == 0)
                    Debug.Log($"[Auth] Still waiting for Supabase client... ({_waitCount * 100} ms)");
            }

            Debug.Log("[Auth] Supabase client initialized.");

            Debug.Log("[Auth] Checking internet connection...");

            bool _hasConnection = await NETWORK.IsConnection();

            Debug.Log($"[Auth] Internet connection result: {_hasConnection}");

            if (!_hasConnection)
            {
                Debug.LogError("[Auth] Initialization failed: no internet connection.");
                return;
            }

#if UNITY_EDITOR

            Debug.Log("[Auth] Running in UNITY_EDITOR mode.");
            Debug.Log("[Auth] Starting editor authentication...");

            var _result = await LoginEditorAccount();

            Debug.Log($"[Auth] Editor authentication result. Success: {_result.IsSuccess}");

            if (!_result.IsSuccess)
                Debug.LogError($"[Auth] Editor authentication failed: {_result.Error}");

#else

            Debug.Log("[Auth] Running in Android/device mode.");

            Debug.Log("[Auth] Checking for existing Supabase session...");

            var _result = await RestoreSession();

            Debug.Log($"[Auth] RestoreSession result. Success: {_result.IsSuccess}");

            if (_result.IsSuccess)
            {
                Debug.Log("[Auth] Existing session restored successfully.");
                return;
            }

            Debug.Log($"[Auth] No valid session found: {_result.Error}");

            Debug.Log("[Auth] Starting Google authentication...");

            _result = await LoginGoogle();

            Debug.Log($"[Auth] LoginGoogle result. Success: {_result.IsSuccess}");

            if (!_result.IsSuccess)
            {
                Debug.LogError($"[Auth] Google authentication failed: {_result.Error}");
                return;
            }

            Debug.Log("[Auth] Google authentication completed successfully.");

#endif

            Debug.Log("========== [Auth] SETUP END ==========");
        }

        private async Task<NetworkResult> InitializeGameData()
        {
            Debug.Log("========== [Auth] INITIALIZE GAME DATA START ==========");

            try
            {
                Debug.Log("[Auth] Requesting game data from Supabase...");

                var _gameResult = await SupabaseManager.GetData();

                Debug.Log($"[Auth] GetData completed. Success: {_gameResult.IsSuccess}");
                Debug.Log($"[Auth] GetData error: {_gameResult.Error}");

                if (_gameResult.IsSuccess)
                {
                    Debug.Log("[Auth] Game data found.");

                    STORAGE.TryReplace(_gameResult.Value.GameData);

                    Debug.Log("[Auth] Local game data replaced successfully.");

                    return new NetworkResult();
                }

                if (_gameResult.Error != "Game data not found.")
                {
                    Debug.LogError($"[Auth] Failed to load game data: {_gameResult.Error}");

                    return new NetworkResult(_gameResult.Error);
                }

                Debug.Log("[Auth] No game data found. Creating initial game data...");

                var _json = STORAGE.GetJSON();

                Debug.Log($"[Auth] Local game data JSON length: {_json?.Length ?? 0}");

                var _saveResult = await SupabaseManager.SaveData(_json);

                Debug.Log($"[Auth] Initial SaveData completed. Success: {_saveResult.IsSuccess}");
                Debug.Log($"[Auth] Initial SaveData error: {_saveResult.Error}");

                if (_saveResult.IsSuccess)
                    Debug.Log("[Auth] Initial game data created successfully.");

                return _saveResult;
            }
            catch (Exception _e)
            {
                Debug.LogError("[Auth] InitializeGameData exception:");
                Debug.LogException(_e);

                return new NetworkResult(_e.Message);
            }
            finally
            {
                Debug.Log("========== [Auth] INITIALIZE GAME DATA END ==========");
            }
        }

#if UNITY_EDITOR

        private async Task<NetworkResult> LoginEditorAccount()
        {
            if (isAuthenticating)
                return new NetworkResult("Authentication is already in progress.");

            isAuthenticating = true;

            Debug.Log("========== [Auth] EDITOR LOGIN START ==========");

            try
            {
                Debug.Log("[Auth] Authenticating with editor account...");

                var _user = SupabaseManager.Client.Auth.CurrentUser;

                Debug.Log($"[Auth] Current Supabase user exists: {_user != null}");

                if (_user == null)
                {
                    Debug.Log("[Auth] No current user. Calling Supabase SignIn...");

                    await SupabaseManager.Client.Auth.SignIn(
                        editorEmail,
                        editorPassword
                    );

                    Debug.Log("[Auth] Supabase SignIn returned.");

                    _user = SupabaseManager.Client.Auth.CurrentUser;

                    Debug.Log($"[Auth] User after SignIn exists: {_user != null}");
                }

                if (_user == null)
                    return new NetworkResult(
                        "Editor authentication succeeded but user is null."
                    );

                Debug.Log($"[Auth] Editor authentication successful.");
                Debug.Log($"[Auth] User ID: {_user.Id}");
                Debug.Log($"[Auth] User Email: {_user.Email}");

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

                Debug.Log("========== [Auth] EDITOR LOGIN END ==========");
            }
        }

#endif

        private async Task<NetworkResult> RestoreSession()
        {
            Debug.Log("========== [Auth] RESTORE SESSION START ==========");

            try
            {
                Debug.Log("[Auth] Reading Supabase.CurrentUser...");

                var _user = SupabaseManager.Client.Auth.CurrentUser;

                Debug.Log($"[Auth] CurrentUser exists: {_user != null}");

                if (_user == null)
                {
                    Debug.Log("[Auth] No active Supabase session.");

                    return new NetworkResult("No active session found.");
                }

                Debug.Log("[Auth] Existing session found.");
                Debug.Log($"[Auth] User ID: {_user.Id}");
                Debug.Log($"[Auth] User Email: {_user.Email}");

                return await InitializeGameData();
            }
            catch (Exception _e)
            {
                HandleAuthException(_e);

                return new NetworkResult(_e.Message);
            }
            finally
            {
                Debug.Log("========== [Auth] RESTORE SESSION END ==========");
            }
        }

        public async Task<NetworkResult> LoginGoogle()
        {
            Debug.Log("==================================================");
            Debug.Log("========== [Auth] GOOGLE LOGIN START ==========");
            Debug.Log("==================================================");

            if (SupabaseManager.Client == null)
            {
                Debug.LogError("[Auth] Supabase client is NULL.");

                return new NetworkResult(
                    "Supabase client is not initialized."
                );
            }

            Debug.Log("[Auth] Supabase client exists.");

            Debug.Log("[Auth] Checking internet connection...");

            bool _hasConnection = await NETWORK.IsConnection();

            Debug.Log($"[Auth] Internet connection: {_hasConnection}");

            if (!_hasConnection)
            {
                Debug.LogError("[Auth] Google login aborted: no internet.");

                return new NetworkResult(
                    "No internet connection."
                );
            }

            if (isAuthenticating)
            {
                Debug.LogWarning(
                    "[Auth] Google login aborted: authentication already in progress."
                );

                return new NetworkResult(
                    "Authentication is already in progress."
                );
            }

            isAuthenticating = true;

            try
            {
                Debug.Log("---------- [Auth] GOOGLE CONFIGURATION ----------");

                var _settings = GAME.GetSO<ASettingData>();

                Debug.Log($"[Auth] ASettingData exists: {_settings != null}");

                if (_settings == null)
                {
                    Debug.LogError("[Auth] ASettingData is NULL.");

                    return new NetworkResult(
                        "ASettingData is null."
                    );
                }

                Debug.Log($"[Auth] Supabase settings exists: {_settings.supabase != null}");

                if (_settings.supabase == null)
                {
                    Debug.LogError("[Auth] Supabase settings are NULL.");

                    return new NetworkResult(
                        "Supabase settings are null."
                    );
                }

                string _webClientId = _settings.supabase.clintID;

                Debug.Log($"[Auth] WebClientId exists: {!string.IsNullOrEmpty(_webClientId)}");
                Debug.Log($"[Auth] WebClientId length: {_webClientId?.Length ?? 0}");

                // Jangan print Client ID penuh kalau tidak diperlukan.
                if (!string.IsNullOrEmpty(_webClientId))
                {
                    Debug.Log(
                        $"[Auth] WebClientId prefix: " +
                        $"{_webClientId.Substring(0, Math.Min(20, _webClientId.Length))}..."
                    );
                }

                Debug.Log($"[Auth] Application.identifier: {Application.identifier}");
                Debug.Log($"[Auth] Application.version: {Application.version}");
                Debug.Log($"[Auth] Platform: {Application.platform}");

                Debug.Log("---------- [Auth] CREATING GOOGLE CONFIGURATION ----------");

                GoogleSignIn.Configuration = new GoogleSignInConfiguration
                {
                    WebClientId = _webClientId,
                    RequestIdToken = true,
                    RequestEmail = true
                };

                Debug.Log("[Auth] GoogleSignIn.Configuration assigned.");
                Debug.Log($"[Auth] RequestIdToken: {GoogleSignIn.Configuration.RequestIdToken}");
                Debug.Log($"[Auth] RequestEmail: {GoogleSignIn.Configuration.RequestEmail}");
                Debug.Log(
                    $"[Auth] Configuration WebClientId exists: " +
                    $"{!string.IsNullOrEmpty(GoogleSignIn.Configuration.WebClientId)}"
                );

                Debug.Log("---------- [Auth] GOOGLE SIGN-IN INSTANCE ----------");

                var _googleInstance = GoogleSignIn.DefaultInstance;

                Debug.Log($"[Auth] GoogleSignIn.DefaultInstance exists: {_googleInstance != null}");

                if (_googleInstance == null)
                {
                    Debug.LogError("[Auth] GoogleSignIn.DefaultInstance is NULL.");

                    return new NetworkResult(
                        "Google Sign-In instance is null."
                    );
                }

                Debug.Log("---------- [Auth] CALLING GOOGLE SIGN-IN ----------");

                Debug.Log("[Auth] Opening Google Sign-In UI...");

                var _googleUser = await _googleInstance.SignIn();

                Debug.Log("[Auth] Google Sign-In Task completed.");

                Debug.Log($"[Auth] Google user is NULL: {_googleUser == null}");

                if (_googleUser == null)
                {
                    Debug.LogError(
                        "[Auth] Google authentication returned a NULL user."
                    );

                    return new NetworkResult(
                        "Google authentication returned a null user."
                    );
                }

                Debug.Log("---------- [Auth] GOOGLE USER RESULT ----------");

                Debug.Log($"[Auth] Google User ID exists: {!string.IsNullOrEmpty(_googleUser.UserId)}");
                Debug.Log($"[Auth] Google User Email: {_googleUser.Email}");
                Debug.Log($"[Auth] Google User DisplayName: {_googleUser.DisplayName}");
                Debug.Log($"[Auth] Google User GivenName: {_googleUser.GivenName}");
                Debug.Log($"[Auth] Google User FamilyName: {_googleUser.FamilyName}");
                Debug.Log($"[Auth] Google User ImageUrl: {_googleUser.ImageUrl}");

                Debug.Log(
                    $"[Auth] Google ID Token exists: " +
                    $"{!string.IsNullOrEmpty(_googleUser.IdToken)}"
                );

                Debug.Log(
                    $"[Auth] Google ID Token length: " +
                    $"{_googleUser.IdToken?.Length ?? 0}"
                );

                // Jangan pernah print ID Token.
                Debug.Log("[Auth] Google ID Token value intentionally NOT logged.");

                if (string.IsNullOrEmpty(_googleUser.IdToken))
                {
                    Debug.LogError("[Auth] Google returned an EMPTY ID token.");

                    return new NetworkResult(
                        "Google authentication returned an empty ID token."
                    );
                }

                Debug.Log("[Auth] Google authentication successful.");
                Debug.Log("[Auth] Passing Google ID token to Supabase...");

                return await LoginGoogleInternal(_googleUser.IdToken);
            }
            catch (GoogleSignIn.SignInException _googleException)
            {
                Debug.LogError("==================================================");
                Debug.LogError("========== [Auth] GOOGLE SIGN-IN EXCEPTION ==========");
                Debug.LogError("==================================================");

                Debug.LogError(
                    $"[Auth] Exception Type: {_googleException.GetType().FullName}"
                );

                Debug.LogError(
                    $"[Auth] Status: {_googleException.Status}"
                );

                Debug.LogError(
                    $"[Auth] Status Code: {(int)_googleException.Status}"
                );

                Debug.LogError(
                    $"[Auth] Message: {_googleException.Message}"
                );

                Debug.LogError(
                    $"[Auth] ToString: {_googleException}"
                );

                Debug.LogError(
                    $"[Auth] StackTrace: {_googleException.StackTrace}"
                );

                if (_googleException.InnerException != null)
                {
                    Debug.LogError(
                        $"[Auth] Inner Exception Type: " +
                        $"{_googleException.InnerException.GetType().FullName}"
                    );

                    Debug.LogError(
                        $"[Auth] Inner Exception Message: " +
                        $"{_googleException.InnerException.Message}"
                    );

                    Debug.LogError(
                        $"[Auth] Inner Exception: " +
                        $"{_googleException.InnerException}"
                    );
                }

                Debug.LogError("---------- [Auth] GOOGLE DEBUG CONTEXT ----------");
                Debug.LogError($"[Auth] Application.identifier: {Application.identifier}");
                Debug.LogError($"[Auth] Application.version: {Application.version}");
                Debug.LogError($"[Auth] Platform: {Application.platform}");
                Debug.LogError($"[Auth] Device Model: {SystemInfo.deviceModel}");
                Debug.LogError($"[Auth] Operating System: {SystemInfo.operatingSystem}");

                var _settings = GAME.GetSO<ASettingData>();

                if (_settings != null && _settings.supabase != null)
                {
                    string _clientId = _settings.supabase.clintID;

                    Debug.Log(
                        $"[Auth] WebClientId exists: " +
                        $"{!string.IsNullOrEmpty(_clientId)}"
                    );

                    Debug.Log(
                        $"[Auth] WebClientId length: " +
                        $"{_clientId?.Length ?? 0}"
                    );

                    if (!string.IsNullOrEmpty(_clientId))
                    {
                        Debug.Log(
                            $"[Auth] WebClientId prefix: " +
                            $"{_clientId.Substring(0, Math.Min(20, _clientId.Length))}..."
                        );
                    }
                }
                else
                {
                    Debug.LogError(
                        "[Auth] Unable to inspect ASettingData.supabase."
                    );
                }

                Debug.LogError("==================================================");

                HandleAuthException(_googleException);

                return new NetworkResult(
                    $"Google Sign-In failed. " +
                    $"Status: {_googleException.Status} " +
                    $"({_googleException.Status.GetHashCode()}) | " +
                    $"Message: {_googleException.Message}"
                );
            }
            catch (Exception _e)
            {
                Debug.LogError("==================================================");
                Debug.LogError("========== [Auth] GENERAL GOOGLE EXCEPTION ==========");
                Debug.LogError("==================================================");

                Debug.LogError(
                    $"[Auth] Exception Type: {_e.GetType().FullName}"
                );

                Debug.LogError(
                    $"[Auth] Message: {_e.Message}"
                );

                Debug.LogError(
                    $"[Auth] ToString: {_e}"
                );

                Debug.LogError(
                    $"[Auth] StackTrace: {_e.StackTrace}"
                );

                if (_e.InnerException != null)
                {
                    Debug.LogError(
                        $"[Auth] Inner Exception Type: " +
                        $"{_e.InnerException.GetType().FullName}"
                    );

                    Debug.LogError(
                        $"[Auth] Inner Exception Message: " +
                        $"{_e.InnerException.Message}"
                    );

                    Debug.LogError(
                        $"[Auth] Inner Exception: " +
                        $"{_e.InnerException}"
                    );
                }

                Debug.LogError("==================================================");

                HandleAuthException(_e);

                return new NetworkResult(_e.Message);
            }
            finally
            {
                isAuthenticating = false;

                Debug.Log("========== [Auth] GOOGLE LOGIN END ==========");
            }
        }

        private async Task<NetworkResult> LoginGoogleInternal(string _idToken)
        {
            Debug.Log("========== [Auth] SUPABASE GOOGLE LOGIN START ==========");

            try
            {
                Debug.Log("[Auth] Preparing Supabase SignInWithIdToken...");

                Debug.Log(
                    $"[Auth] ID Token exists: {!string.IsNullOrEmpty(_idToken)}"
                );

                Debug.Log(
                    $"[Auth] ID Token length: {_idToken?.Length ?? 0}"
                );

                // Token value sengaja tidak dilog.
                Debug.Log("[Auth] ID Token value intentionally NOT logged.");

                Debug.Log(
                    $"[Auth] Supabase client exists: " +
                    $"{SupabaseManager.Client != null}"
                );

                Debug.Log("[Auth] Provider: Google");

                Debug.Log("[Auth] Calling Supabase Auth.SignInWithIdToken...");

                await SupabaseManager.Client.Auth.SignInWithIdToken(
                    Supabase.Gotrue.Constants.Provider.Google,
                    _idToken
                );

                Debug.Log(
                    "[Auth] Supabase SignInWithIdToken completed."
                );

                var _user = SupabaseManager.Client.Auth.CurrentUser;

                Debug.Log(
                    $"[Auth] Supabase CurrentUser exists: {_user != null}"
                );

                if (_user == null)
                {
                    Debug.LogError(
                        "[Auth] Google authentication succeeded " +
                        "but Supabase user is NULL."
                    );

                    return new NetworkResult(
                        "Google authentication succeeded but Supabase user is null."
                    );
                }

                Debug.Log(
                    "[Auth] Supabase Google authentication successful."
                );

                Debug.Log($"[Auth] Supabase User ID: {_user.Id}");
                Debug.Log($"[Auth] Supabase User Email: {_user.Email}");

                return await InitializeGameData();
            }
            catch (Exception _e)
            {
                Debug.LogError("========== [Auth] SUPABASE GOOGLE LOGIN EXCEPTION ==========");

                Debug.LogError(
                    $"[Auth] Exception Type: {_e.GetType().FullName}"
                );

                Debug.LogError(
                    $"[Auth] Message: {_e.Message}"
                );

                Debug.LogError(
                    $"[Auth] ToString: {_e}"
                );

                Debug.LogError(
                    $"[Auth] StackTrace: {_e.StackTrace}"
                );

                if (_e.InnerException != null)
                {
                    Debug.LogError(
                        $"[Auth] Inner Exception Type: " +
                        $"{_e.InnerException.GetType().FullName}"
                    );

                    Debug.LogError(
                        $"[Auth] Inner Exception Message: " +
                        $"{_e.InnerException.Message}"
                    );

                    Debug.LogError(
                        $"[Auth] Inner Exception: " +
                        $"{_e.InnerException}"
                    );
                }

                HandleAuthException(_e);

                return new NetworkResult(_e.Message);
            }
            finally
            {
                Debug.Log("========== [Auth] SUPABASE GOOGLE LOGIN END ==========");
            }
        }

        public async Task<NetworkResult<bool>> Logout()
        {
            Debug.Log("========== [Auth] LOGOUT START ==========");

            if (SupabaseManager.Client == null)
            {
                Debug.LogError("[Auth] Cannot logout: Supabase client is NULL.");

                return new NetworkResult<bool>(
                    "Supabase client is not initialized."
                );
            }

            bool _hasConnection = await NETWORK.IsConnection();

            Debug.Log($"[Auth] Internet connection: {_hasConnection}");

            if (!_hasConnection)
            {
                Debug.LogError("[Auth] Logout aborted: no internet.");

                return new NetworkResult<bool>(
                    "No internet connection."
                );
            }

            try
            {
                Debug.Log("[Auth] Signing out from Supabase...");

                await SupabaseManager.Client.Auth.SignOut();

                Debug.Log("[Auth] Sign out completed successfully.");

                return new NetworkResult<bool>(true);
            }
            catch (Exception _e)
            {
                Debug.LogError("========== [Auth] LOGOUT EXCEPTION ==========");

                Debug.LogError($"[Auth] Exception Type: {_e.GetType().FullName}");
                Debug.LogError($"[Auth] Message: {_e.Message}");
                Debug.LogError($"[Auth] Exception: {_e}");

                return new NetworkResult<bool>(_e.Message);
            }
            finally
            {
                Debug.Log("========== [Auth] LOGOUT END ==========");
            }
        }

        private void HandleAuthException(Exception _e)
        {
            if (_e == null)
            {
                Debug.LogError("[Auth] HandleAuthException received NULL exception.");
                return;
            }

            var _message = _e.Message ?? string.Empty;

            Debug.LogError("========== [Auth] HANDLE EXCEPTION ==========");

            Debug.LogError($"[Auth] Exception Type: {_e.GetType().FullName}");
            Debug.LogError($"[Auth] Message: {_message}");

            if (_e.InnerException != null)
            {
                Debug.LogError(
                    $"[Auth] Inner Exception: " +
                    $"{_e.InnerException.GetType().FullName}"
                );

                Debug.LogError(
                    $"[Auth] Inner Message: " +
                    $"{_e.InnerException.Message}"
                );
            }

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
                    "[Auth] Editor authentication failed: " +
                    "invalid email or password."
                );

                return;
            }

            Debug.LogError(
                $"[Auth] Authentication error: {_message}"
            );

            Debug.LogError("========== [Auth] HANDLE EXCEPTION END ==========");
        }
    }
}

#endif