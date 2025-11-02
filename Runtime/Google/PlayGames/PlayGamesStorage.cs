#if GOOGLE_MOBILE

using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Threading.Tasks;
using GooglePlayGames.BasicApi;

namespace ACore.Google
{
    public class PlayGamesStorage : PlayGamesBase
    {
        private const string SaveSlotName = "SaveData";

        public static async Task<bool> TrySaveAsync(byte[] json)
        {
            if (!Social.localUser.authenticated)
            {
                Debug.LogWarning("TrySaveAsync failed: Not logged in to Google Play");
                return false;
            }

            var _openResult = await OpenSavedGameAsync(SaveSlotName);
            if (_openResult.status != SavedGameRequestStatus.Success)
            {
                Debug.LogWarning($"TrySaveAsync failed: Open slot error ({_openResult.status})");
                return false;
            }

            var _update = new SavedGameMetadataUpdate.Builder()
                .WithUpdatedDescription("Saved at " + DateTime.Now)
                .Build();

            var _commitStatus = await CommitUpdateAsync(_openResult.metadata, _update, json);
            var _success = _commitStatus == SavedGameRequestStatus.Success;

            Debug.Log(_success ? "Cloud Save success" : "Cloud Save failed");
            return _success;
        }

        public static async Task<(bool success, byte[] json)> TryLoadAsync()
        {
            if (!Social.localUser.authenticated)
            {
                Debug.LogWarning("TryLoadAsync failed: Not logged in to Google Play");
                return (false, null);
            }

            var _openResult = await OpenSavedGameAsync(SaveSlotName);
            if (_openResult.status != SavedGameRequestStatus.Success)
            {
                Debug.LogWarning($"TryLoadAsync failed: Open slot error ({_openResult.status})");
                return (false, null);
            }

            var _readResult = await ReadBinaryDataAsync(_openResult.metadata);
            if (_readResult.status != SavedGameRequestStatus.Success)
            {
                Debug.LogWarning($"TryLoadAsync failed: Read error ({_readResult.status})");
                return (false, null);
            }
            
            Debug.Log("Cloud Load success");
            return (true, _readResult.data);
        }
        
        private static Task<(SavedGameRequestStatus status, ISavedGameMetadata metadata)> OpenSavedGameAsync(string name)
        {
            var _tcs = new TaskCompletionSource<(SavedGameRequestStatus, ISavedGameMetadata)>();

            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(
                name,
                DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseMostRecentlySaved,
                (status, metadata) => _tcs.SetResult((status, metadata))
            );

            return _tcs.Task;
        }

        private static Task<SavedGameRequestStatus> CommitUpdateAsync(ISavedGameMetadata metadata, SavedGameMetadataUpdate update, byte[] data)
        {
            var _tcs = new TaskCompletionSource<SavedGameRequestStatus>();

            PlayGamesPlatform.Instance.SavedGame.CommitUpdate(
                metadata, update, data,
                (status, meta) => _tcs.SetResult(status)
            );

            return _tcs.Task;
        }

        private static Task<(SavedGameRequestStatus status, byte[] data)> ReadBinaryDataAsync(ISavedGameMetadata metadata)
        {
            var _tcs = new TaskCompletionSource<(SavedGameRequestStatus, byte[])>();

            PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(
                metadata, (status, data) => _tcs.SetResult((status, data))
            );

            return _tcs.Task;
        }
    }
}

#endif