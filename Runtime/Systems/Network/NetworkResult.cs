namespace ACore
{
    public class NetworkResult
    {
        public bool IsSuccess { get; }
        public string Error { get; }
    
        public NetworkResult()
        {
            IsSuccess = true;
        }
    
        public NetworkResult(string error)
        {
            IsSuccess = false;
            Error = error;
        }
    }
    
    public class NetworkResult<T> : NetworkResult
    {
        public T Value { get; }
    
        public NetworkResult(T value)
        {
            Value = value;
        }
    
        public NetworkResult(string error) : base(error) { }
    }
}