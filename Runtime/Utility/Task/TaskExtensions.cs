using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ACore
{
    public static class TaskExtensions
    {
        public static async Task<T> WithTimeout<T>(this Task<T> task, float timeoutSeconds)
        {
            var _timeoutTask = Task.Delay(
                TimeSpan.FromSeconds(timeoutSeconds)
            );

            var _completedTask = await Task.WhenAny(
                task,
                _timeoutTask
            );

            if (_completedTask == _timeoutTask)
                throw new TimeoutException(
                    $"Task timeout after {timeoutSeconds} seconds."
                );

            return await task;
        }

        public static async Task WithTimeout(this Task task, float timeoutSeconds)
        {
            var _timeoutTask = Task.Delay(
                TimeSpan.FromSeconds(timeoutSeconds)
            );

            var _completedTask = await Task.WhenAny(
                task,
                _timeoutTask
            );

            if (_completedTask == _timeoutTask)
                throw new TimeoutException(
                    $"Task timeout after {timeoutSeconds} seconds."
                );

            await task;
        }
        
        public static async Task WithTimeout(this UnityWebRequestAsyncOperation operation, float timeoutSeconds)
        {
            var _timeoutTask = Task.Delay(
                TimeSpan.FromSeconds(timeoutSeconds)
            );

            var _requestTask = Wait(operation);

            var _completed = await Task.WhenAny(
                _requestTask,
                _timeoutTask
            );

            if (_completed == _timeoutTask)
                throw new TimeoutException("Request timeout.");
        }

        private static async Task Wait(UnityWebRequestAsyncOperation operation)
        {
            while (!operation.isDone)
            {
                await Task.Yield();
            }
        }
    }
}