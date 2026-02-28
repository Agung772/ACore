using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ACore
{
    [AddComponentMenu("Event/Pointer Button")]
    public class PointerButton : EventTrigger
    {
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
    }
}