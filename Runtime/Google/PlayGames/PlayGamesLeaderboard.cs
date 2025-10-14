#if GOOGLE_MOBILE

using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

namespace ACore.Google
{
    public class PlayGamesLeaderboard : PlayGamesBase
    {
        public void ReportScore(long skor)
        {
            Social.ReportScore(skor, Game.Get<PlayGamesManager>().Setting.leaderboardID, success =>
            {
                Debug.Log(success ? "Skor terkirim" : "Gagal kirim skor");
            });
        }
        
        public void Show()
        {
            Social.ShowLeaderboardUI();
        }
        
        public async Task<long> GetScore()
        {
            var _tcs = new TaskCompletionSource<long>();

            PlayGamesPlatform.Instance.LoadScores(
                Game.Get<PlayGamesManager>().Setting.leaderboardID,
                LeaderboardStart.PlayerCentered,
                1,
                LeaderboardCollection.Public,
                LeaderboardTimeSpan.AllTime,
                data =>
                {
                    if (data.Status == ResponseStatus.Success && data.PlayerScore != null)
                    {
                        _tcs.TrySetResult(data.PlayerScore.value);
                    }
                    else
                    {
                        Debug.LogWarning($"Gagal ambil skor player. Status: {data.Status}");
                        _tcs.TrySetResult(0);
                    }
                });

            return await _tcs.Task;
        }
    }
}

#endif