using System.Collections;
using System.Threading.Tasks;

namespace ACore
{
    public class GlobalBehaviour : IBehaviour
    {
        public virtual int Order { get; set; }
        public virtual async Task InitializeAsync() { await Task.CompletedTask; }
        public virtual async Task PostInitializeAsync() { await Task.CompletedTask; }
        public virtual IEnumerator InitializeCoroutine() { yield break; }
        public virtual IEnumerator PostInitializeCoroutine() { yield break; }
        public virtual void Initialize() { }
        public virtual void PostInitialize() { }
    }
}
