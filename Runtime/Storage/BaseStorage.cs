namespace ACore
{
    public class BaseStorage
    {
        public virtual void OnDefault() { }
        public virtual void OnCreate() { }
        public virtual void OnLoad() { }
        public virtual void OnSave() { }
    }
}

