using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ACore
{
    public class PointerButton : MonoBehaviour,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public UnityEvent onPointerDown;
        public UnityEvent onPointerUp;
        public UnityEvent onPointerClick;
        public UnityEvent onBeginDrag;
        public UnityEvent onDrag;
        public UnityEvent onEndDrag;
        public UnityEvent onPointerEnter;
        public UnityEvent onPointerExit;
        public UnityEvent onPointerHold;

        private bool isHolding;

        public void OnPointerDown(PointerEventData eventData)
        {
            isHolding = true;
            onPointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isHolding = false;
            onPointerUp?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onPointerClick?.Invoke();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            onDrag?.Invoke();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onPointerExit?.Invoke();
        }

        private void Update()
        {
            if (isHolding)
                onPointerHold?.Invoke();
        }
    }
}