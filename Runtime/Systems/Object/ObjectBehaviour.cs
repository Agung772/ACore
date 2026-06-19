using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACore
{
    public class ObjectBehaviour : MonoBehaviour
    {
        [FoldoutGroup("Base")] [BoxGroup("Base/Settings")]
        public bool isGlobal;

        [FoldoutGroup("Base")] [BoxGroup("Base/Settings")]
        public bool canMulti;
        
        [FoldoutGroup("Base")] [BoxGroup("Base/Auto Remove")] [SerializeField, LabelText("Active")]
        public bool autoRemove;

        [FoldoutGroup("Base")] [ShowIf(nameof(autoRemove))] [BoxGroup("Base/Auto Remove")] [SerializeField]
        private float removeAfter = 1f;

        [FoldoutGroup("Base")] [ShowIf(nameof(autoRemove))] [BoxGroup("Base/Auto Remove")] [SerializeField]
        private bool useUnScaledTime;
        
        public Action onRemove;
        
        public virtual void Awake()
        {
            if (autoRemove)
            {
                gameObject.LeanDelayedCall(removeAfter, Remove).setIgnoreTimeScale(useUnScaledTime);
            }
        }
        
        public virtual void Start()
        {

        }
        
        public virtual T HideAndShow<T>() where T : UIBehaviour
        {
            gameObject.SetActive(false);
            var _newPopup = OBJECT.Show<T>();
            _newPopup.onRemove += () => gameObject.SetActive(true);
            return _newPopup;
        }
        
        public virtual T RemoveAndShow<T>() where T : UIBehaviour
        {
            Remove();
            return OBJECT.Show<T>();
        }
        
        public virtual void Remove()
        {
            onRemove?.Invoke();
            OBJECT.RemoveInternal(this);
        }
    }
}