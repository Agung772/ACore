using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ACore
{
    [AddComponentMenu("Event/Pointer Button")]
    [RequireComponent(typeof(Button))]
    public class PointerButton : EventTrigger
    {
        public bool interactable
        {
            get => button != null && button.interactable;
            set
            {
                if (button != null)
                    button.interactable = value;
            }
        }

        private Button button;

        protected void Awake()
        {
            button = GetComponent<Button>();
        }

        public void AddListener(EventTriggerType id, UnityAction<BaseEventData> action)
        {
            var _entry = GetOrCreateEntry(id);
            _entry.callback.AddListener(action);
        }

        public TriggerEvent GetTriggerEvent(EventTriggerType id)
        {
            return GetOrCreateEntry(id).callback;
        }

        public void RemoveListener(EventTriggerType id, UnityAction<BaseEventData> action)
        {
            var _entry = triggers.Find(e => e.eventID == id);
            _entry?.callback.RemoveListener(action);
        }

        public void RemoveAllListeners(EventTriggerType id)
        {
            var _entry = triggers.Find(e => e.eventID == id);
            _entry?.callback.RemoveAllListeners();
        }

        private Entry GetOrCreateEntry(EventTriggerType id)
        {
            var _entry = triggers.Find(e => e.eventID == id);
            if (_entry != null)
                return _entry;

            _entry = new Entry
            {
                eventID = id,
                callback = new TriggerEvent()
            };

            triggers.Add(_entry);
            return _entry;
        }

        private void ExecuteIfInteractable(EventTriggerType id, BaseEventData data)
        {
            if (!interactable) return;

            var _entry = triggers.Find(e => e.eventID == id);
            _entry?.callback.Invoke(data);
        }

        public override void OnPointerEnter(PointerEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.PointerEnter, eventData);

        public override void OnPointerExit(PointerEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.PointerExit, eventData);

        public override void OnPointerDown(PointerEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.PointerDown, eventData);

        public override void OnPointerUp(PointerEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.PointerUp, eventData);

        public override void OnPointerClick(PointerEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.PointerClick, eventData);

        public override void OnBeginDrag(PointerEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.BeginDrag, eventData);

        public override void OnDrag(PointerEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.Drag, eventData);

        public override void OnEndDrag(PointerEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.EndDrag, eventData);

        public override void OnDrop(PointerEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.Drop, eventData);

        public override void OnScroll(PointerEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.Scroll, eventData);

        public override void OnMove(AxisEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.Move, eventData);

        public override void OnSelect(BaseEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.Select, eventData);

        public override void OnDeselect(BaseEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.Deselect, eventData);

        public override void OnSubmit(BaseEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.Submit, eventData);

        public override void OnCancel(BaseEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.Cancel, eventData);

        public override void OnInitializePotentialDrag(PointerEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.InitializePotentialDrag, eventData);

        public override void OnUpdateSelected(BaseEventData eventData)
            => ExecuteIfInteractable(EventTriggerType.UpdateSelected, eventData);
    }
}