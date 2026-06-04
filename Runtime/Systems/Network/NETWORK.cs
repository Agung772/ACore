using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ACore
{
    public static class NETWORK
    {
        private const string TestURL = "https://www.google.com/generate_204";
        private const float TimeoutSeconds = 5f;
        private const float SlowThresholdSeconds = 3f;

        public static async Task<bool> IsConnection()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return false;
            
            using var _request = UnityWebRequest.Get(TestURL);

            _request.timeout = (int)TimeoutSeconds;

            var _stopwatch = Stopwatch.StartNew();
            var _operation = _request.SendWebRequest();

            while (!_operation.isDone)
                await Task.Yield();

            _stopwatch.Stop();

            if (_request.result != UnityWebRequest.Result.Success)
                return false;

            var _duration = (float)_stopwatch.Elapsed.TotalSeconds;
            return _duration <= SlowThresholdSeconds;
        }
    }
}