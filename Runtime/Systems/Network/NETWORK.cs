using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ACore
{
    public static class Network
    {
        private const string TestURL = "https://www.google.com/generate_204";
        private const string TimeURL = "https://google.com";
        
        public static async Task<bool> IsConnection()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return false;

            try
            {
                using var _request = UnityWebRequest.Get(TestURL);

                var _operation = _request.SendWebRequest();

                await _operation.WithTimeout(5);

                return _request.result == UnityWebRequest.Result.Success;
            }
            catch
            {
                return false;
            }
        }


        public static async Task<NetworkResult<DateTime>> GetTime()
        {
            try
            {
                using var _request = UnityWebRequest.Head(TimeURL);

                var _operation = _request.SendWebRequest();

                await _operation.WithTimeout(5);

                if (_request.result != UnityWebRequest.Result.Success)
                {
                    return new NetworkResult<DateTime>(
                        _request.error
                    );
                }
                
                var _date = _request.GetResponseHeader("Date");

                if (string.IsNullOrEmpty(_date))
                {
                    return new NetworkResult<DateTime>(
                        "Date header not found."
                    );
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
                return new NetworkResult<DateTime>(
                    _exception.Message
                );
            }
        }
    }
}