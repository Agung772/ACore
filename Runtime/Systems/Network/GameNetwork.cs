using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace ACore
{
    public static class GameNetwork
    {
        private const string TEST_URL = "https://www.google.com/generate_204";
        private const float TIMEOUT_SECONDS = 5f;
        private const float SLOW_THRESHOLD_SECONDS = 3f;
    
        public static async Task<bool> IsInternetConnection()
        {
            var _request = UnityWebRequest.Get(TEST_URL);
            _request.timeout = (int)TIMEOUT_SECONDS;

            var _stopwatch = Stopwatch.StartNew();
            var _operation = _request.SendWebRequest();

            while (!_operation.isDone)
                await Task.Yield();

            _stopwatch.Stop();

            if (_request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Internet not available: " + _request.error);
                return false;
            }

            var _duration = (float)_stopwatch.Elapsed.TotalSeconds;
            var _isSlow = _duration > SLOW_THRESHOLD_SECONDS;

            if (_isSlow)
                Debug.LogWarning($"Slow internet: {_duration:F2} second.");
            else
                Debug.Log($"Fast internet: {_duration:F2} second.");

            return !_isSlow;
        }
    }
}