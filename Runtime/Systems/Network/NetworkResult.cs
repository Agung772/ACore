using System;

namespace ACore
{
    [Serializable]
    public class NetworkResult<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }

        public NetworkResult(T value)
        {
            IsSuccess = true;
            Value = value;
        }

        public NetworkResult(string error)
        {
            IsSuccess = false;
            Error = error;
        }
    }
}