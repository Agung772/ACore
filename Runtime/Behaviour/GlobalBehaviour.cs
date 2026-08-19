using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace ACore
{
    public class GlobalBehaviour : IBehaviour
    {
        public virtual async Task InitializeAsync() { await Task.CompletedTask; }
        public virtual async Task PostInitializeAsync() { await Task.CompletedTask; }
        public virtual IEnumerator InitializeCoroutine() { yield break; }
        public virtual IEnumerator PostInitializeCoroutine() { yield break; }
        public virtual void Initialize() { }
        public virtual void PostInitialize() { }
        internal IEnumerator RunInitialize()
        {
            Initialize();
            yield return InitializeCoroutine();
            var _task = InitializeAsync();
            yield return new WaitUntil(() => _task.IsCompleted);
        }
        internal IEnumerator RunPostInitialize()
        {
            PostInitialize();
            yield return PostInitializeCoroutine();
            var _task = PostInitializeAsync();
            yield return new WaitUntil(() => _task.IsCompleted);
        }
    }
}
