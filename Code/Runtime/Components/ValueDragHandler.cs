using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KDebugger.Plugins.ShizoGames.UGUIExtended.Components
{
    [DisallowMultipleComponent]
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Drag/Value Drag Handler")]
    public sealed class ValueDragHandler : MonoBehaviour
    {
        [SerializeField] private Selectable _slider;

        public event Action OnDragBegin;
        public event Action OnDragEnd;
        public event Action OnDragCanceled;
        public event Action<float> OnDragValueChanged;

        private bool _dragBegin;
        private bool _dragging;

        private void Awake()
        {
            var eventTrigger = _slider.gameObject.AddComponent<EventTrigger>();
            
            var pointerDown = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            pointerDown.callback.AddListener(OnPointerDown);
            
            var pointerDrag = new EventTrigger.Entry
            {
                eventID = EventTriggerType.Drag
            };
            pointerDrag.callback.AddListener(OnPointerDrag);

            var pointerUp = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            pointerUp.callback.AddListener(OnPointerUp);
            
            eventTrigger.triggers.Add(pointerDown);
            eventTrigger.triggers.Add(pointerDrag);
            eventTrigger.triggers.Add(pointerUp);
        }
        
        private void Update()
        {
            if (!_dragBegin || !_dragging) return;
            
#if ENABLE_INPUT_SYSTEM
            var escapeIsPressed = UnityEngine.InputSystem.Keyboard.current.escapeKey.isPressed;
#elif ENABLE_LEGACY_INPUT_MANAGER
            var escapePressed = Input.GetKeyDown(KeyCode.Escape);
#else
            var escapePressed = false;
#endif
            
            if (escapePressed)
            {
                OnDragCanceled?.Invoke();

                _dragBegin = false;
                _dragging = false;
            }
        }

        private void OnPointerDown(BaseEventData arg0)
        {
            _dragBegin = true;
        }

        private void OnPointerDrag(BaseEventData arg0)
        {
            _dragging = true;
            
            if (_dragBegin)
            {
                OnDragBegin?.Invoke();
            }
            
            var data = (PointerEventData) arg0;

            OnDragValueChanged?.Invoke(data.position.x - data.pressPosition.x);
        }

        private void OnPointerUp(BaseEventData arg0)
        {
            if (_dragBegin && _dragging)
            {
                OnDragEnd?.Invoke();
            }

            _dragBegin = false;
            _dragging = false;
        }
    }
}