using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ShizoGames.UGUIExtended.Components.Drag
{
    [DisallowMultipleComponent]
    public sealed class DraggableRectHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private PointerUnityEvent _onBeginDragEvent = new PointerUnityEvent();
        [SerializeField] private PointerUnityEvent _onDragEvent = new PointerUnityEvent();
        [SerializeField] private PointerUnityEvent _onEndDragEvent = new PointerUnityEvent();

        public PointerUnityEvent OnBeginDragEvent => _onBeginDragEvent;
        public PointerUnityEvent OnDragEvent => _onDragEvent;
        public PointerUnityEvent OnEndDragEvent => _onEndDragEvent;

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDragEvent?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragEvent?.Invoke(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnEndDragEvent?.Invoke(eventData);
        }
        
        [Serializable]
        public class PointerUnityEvent : UnityEvent<PointerEventData>
        {
        }
    }
}
