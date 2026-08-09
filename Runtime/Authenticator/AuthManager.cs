#if SUPABASE

using System;
using System.Threading.Tasks;
using UnityEngine;

#if !UNITY_EDITOR
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

namespace ACore
{
    public class AuthManager : GlobalBehaviour
    {
#if UNITY_EDITOR

        private string editorEmail = "test@gmail.com";
        private string editorPassword = "test123";

#endif

        private bool isAuthenticating;

        public override async Task PostInitializeAsync()
        {
            await Setup().WithTimeout(5);

            if (SupabaseManager.Client?.Auth?.CurrentUser != null)
                await SavePlayerData(STORAGE.JSON);
        }

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

            await LoginEditorAccount();

#else

            var _restoreResult = await RestoreSession();

            if (_restoreResult.IsSuccess)
            {
                Debug.Log("Supabase session berhasil di-restore.");
                return;
            }

            Debug.Log(
                "Supabase session tidak ditemukan. " +
                "Mencoba Google Play Games Silent Sign-In."
            );

            var _silentResult = await SilentSignIn();

            if (_silentResult.IsSuccess)
            {
                Debug.Log(
                    "Google Play Games Silent Sign-In berhasil."
                );

                return;
            }

            Debug.Log(
                "Google Play Games Silent Sign-In gagal."
            );

            Debug.Log(
                "User perlu Interactive Sign-In."
            );

#endif
        }

        private void ReplaceData()
        {
            
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
                        "Session tidak ditemukan."
                    );
                }

                Debug.Log(
                    $"Supabase session ditemukan: {_user.Id}"
                );

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

#if !UNITY_EDITOR

        public async Task<NetworkResult> SilentSignIn()
        {
            if (isAuthenticating)
            {
                return new NetworkResult("Already authenticating.");
            }

            if (SupabaseManager.Client == null)
            {
                return new NetworkResult("Supabase Client belum siap.");
            }

            if (!await NETWORK.IsConnection())
            {
                return new NetworkResult("No Internet");
            }

            isAuthenticating = true;

            try
            {
                Debug.Log(
                    "Google Play Games Silent Sign-In..."
                );

                var _result = 
                    await AuthenticatePlayGames();

                if (!_result.IsSuccess)
                    return _result;

                string _playerId =
                    GetPlayGamesPlayerId();

                if (string.IsNullOrEmpty(_playerId))
                {
                    return new NetworkResult(
                        "Google Play Games berhasil login " +
                        "tetapi Player ID kosong."
                    );
                }

                Debug.Log(
                    $"Google Play Games Player ID: {_playerId}"
                );

                return new NetworkResult();
            }
            catch (Exception _e)
            {
                Debug.LogWarning(
                    $"Google Play Games Silent Sign-In gagal: {_e.Message}"
                );

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

#if !UNITY_EDITOR

        public async Task<NetworkResult> InteractiveSignIn()
        {
            if (isAuthenticating)
            {
                return new NetworkResult(
                    "Already authenticating."
                );
            }

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

            isAuthenticating = true;

            try
            {
                Debug.Log(
                    "Memulai Google Play Games Interactive Sign-In..."
                );

                var _result =
                    await InteractivePlayGames();

                if (!_result.IsSuccess)
                    return _result;

                string _playerId =
                    GetPlayGamesPlayerId();

                if (string.IsNullOrEmpty(_playerId))
                {
                    return new NetworkResult(
                        "Google Play Games berhasil login " +
                        "tetapi Player ID kosong."
                    );
                }

                Debug.Log(
                    $"Google Play Games Player ID: {_playerId}"
                );

                return new NetworkResult();
            }
            catch (Exception _e)
            {
                Debug.LogError(
                    $"Google Play Games Interactive Sign-In gagal: {_e.Message}"
                );

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

#if !UNITY_EDITOR

        private Task<NetworkResult> AuthenticatePlayGames()
        {
            var _task =
                new TaskCompletionSource<NetworkResult>();

            try
            {
                PlayGamesPlatform.Instance.Authenticate(
                    _status =>
                    {
                        if (_status == SignInStatus.Success)
                        {
                            _task.SetResult(
                                new NetworkResult()
                            );

                            return;
                        }

                        _task.SetResult(
                            new NetworkResult(
                                $"PGS Silent Sign-In gagal: {_status}"
                            )
                        );
                    }
                );
            }
            catch (Exception _e)
            {
                _task.SetResult(
                    new NetworkResult(
                        _e.Message
                    )
                );
            }

            return _task.Task;
        }

#endif

#if !UNITY_EDITOR

        private Task<NetworkResult> InteractivePlayGames()
        {
            var _task =
                new TaskCompletionSource<NetworkResult>();

            try
            {
                PlayGamesPlatform.Instance.ManuallyAuthenticate(
                    _status =>
                    {
                        if (_status == SignInStatus.Success)
                        {
                            _task.SetResult(
                                new NetworkResult()
                            );

                            return;
                        }

                        _task.SetResult(
                            new NetworkResult(
                                $"PGS Interactive Sign-In gagal: {_status}"
                            )
                        );
                    }
                );
            }
            catch (Exception _e)
            {
                _task.SetResult(
                    new NetworkResult(
                        _e.Message
                    )
                );
            }

            return _task.Task;
        }

#endif

#if !UNITY_EDITOR

        private string GetPlayGamesPlayerId()
        {
            try
            {
                return PlayGamesPlatform.Instance.GetUserId();
            }
            catch (Exception _e)
            {
                Debug.LogError(
                    $"Gagal mengambil Player ID: {_e.Message}"
                );

                return null;
            }
        }

        public string GetPlayGamesPlayerName()
        {
            try
            {
                return PlayGamesPlatform.Instance.GetUserDisplayName();
            }
            catch (Exception _e)
            {
                Debug.LogError(
                    $"Gagal mengambil Player Name: {_e.Message}"
                );

                return null;
            }
        }

#endif

        public async Task<NetworkResult> LoginGoogle(
            string idToken)
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
                return await LoginGoogleInternal(
                    idToken
                );
            }
            finally
            {
                isAuthenticating = false;
            }
        }

        private async Task<NetworkResult> LoginGoogleInternal(
            string idToken)
        {
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

                Debug.Log(
                    $"Supabase Google Login berhasil: {_user.Id}"
                );

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
                STORAGE.JSON
            );
        }

        public async Task<NetworkResult<GameDatabase>>
            GetPlayerData()
        {
            if (SupabaseManager.Client == null)
            {
                return new NetworkResult<GameDatabase>(
                    "Supabase Client belum siap."
                );
            }

            if (!await NETWORK.IsConnection())
            {
                return new NetworkResult<GameDatabase>(
                    "No Internet"
                );
            }

            try
            {
                var _user =
                    SupabaseManager.Client.Auth.CurrentUser;

                if (_user == null)
                {
                    return new NetworkResult<GameDatabase>(
                        "User belum login."
                    );
                }

                var _response =
                    await SupabaseManager.Client
                        .From<GameDatabase>()
                        .Where(x => x.Id == _user.Id)
                        .Get();

                if (_response.Models == null ||
                    _response.Models.Count == 0)
                {
                    return new NetworkResult<GameDatabase>(
                        "PlayerData tidak ditemukan."
                    );
                }

                return new NetworkResult<GameDatabase>(
                    _response.Models[0]
                );
            }
            catch (Exception _e)
            {
                return new NetworkResult<GameDatabase>(
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

            if (!await NETWORK.IsConnection())
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
                var _playerData = new GameDatabase
                {
                    Id = userId,
                    GameData = gameData
                };

                await SupabaseManager.Client
                    .From<GameDatabase>()
                    .Upsert(_playerData);

                Debug.Log(
                    "Successfully saved the game data to server."
                );

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

            if (!await NETWORK.IsConnection())
            {
                return new NetworkResult<bool>(
                    "No Internet"
                );
            }

            try
            {
                await SupabaseManager.Client.Auth.SignOut();

                return new NetworkResult<bool>(
                    true
                );
            }
            catch (Exception _e)
            {
                return new NetworkResult<bool>(
                    _e.Message
                );
            }
        }

        private void HandleAuthException(
            Exception _e)
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
                    "Login Editor gagal. " +
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
