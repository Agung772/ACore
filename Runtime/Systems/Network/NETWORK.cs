using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ACore
{
    public static class NETWORK
    {
        private const string TestURL = "https://www.google.com/generate_204";
        private const int TimeoutSeconds = 5;
        private const float SlowThresholdSeconds = 3f;

        public static async Task<bool> IsConnection()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return false;
            
            using var _request = UnityWebRequest.Get(TestURL);

            _request.timeout = TimeoutSeconds;

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
        
        public static async Task<NetworkResult<DateTime>> GetTime()
        {
            try
            {
                using var _request = UnityWebRequest.Head("https://google.com");

                _request.timeout = TimeoutSeconds;

                var _operation = _request.SendWebRequest();

                while (!_operation.isDone)
                {
                    await Task.Yield();
                }

                if (_request.result != UnityWebRequest.Result.Success)
                {
                    return new NetworkResult<DateTime>(_request.error);
                }

                var _date = _request.GetResponseHeader("Date");

                if (string.IsNullOrEmpty(_date))
                {
                    return new NetworkResult<DateTime>("Date header not found.");
                }

                return new NetworkResult<DateTime>(
                    DateTime.ParseExact(
                        _date,
                        "r",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal
                    )
                );
            }
            catch (Exception _exception)
            {
                return new NetworkResult<DateTime>(_exception.Message);
            }
        }
    }
}