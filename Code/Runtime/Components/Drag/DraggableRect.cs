﻿using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ShizoGames.ImprovedUI.Components.Drag
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Draggable Rect")]
    public sealed class DraggableRect : UIBehaviour
    {
        [SerializeField] private bool _interactable = true;

        [Header("Config")]
        [SerializeField] private DraggableRestriction _restriction = DraggableRestriction.Strict;

        [Header("References")]
        [SerializeField] private RectTransform _target;
        [SerializeField] private RectTransform _handle;

        private bool _isDrag;
        private Vector3 _startPosition;

        private DraggableRectHandler _dragHandle;
        
        private RectTransform _rectTransform;
        private RectTransform RectTransform => _rectTransform
            ? _rectTransform
            : _rectTransform = transform as RectTransform;
        
        [SerializeField] public DraggableEvent OnBeginDrag = new DraggableEvent(); 
        [SerializeField] public DraggableEvent OnDrag = new DraggableEvent();
        [SerializeField] public DraggableEvent OnEndDrag = new DraggableEvent();
        
        public Vector3 StartPosition { get; private set; }

        public bool Interactable
        {
            get => _interactable;
            set => InteractableChange(value);
        }

        public DraggableRestriction Restriction
        {
            get => _restriction;
            set => _restriction = value;
        }

        public RectTransform Target
        {
            get => _target;
            set => SetTarget(value);
        }

        public RectTransform Handle
        {
            get => _handle;
            set => SetHandle(value);
        }

        protected override void Start()
        {
            SetTarget(_target ? _target : RectTransform);
            SetHandle(_handle ? _handle : RectTransform);
        }

        protected override void OnDestroy()
        {
            RemoveListeners();
        }

        public override bool IsActive()
        {
            return base.IsActive() && Interactable;
        }

        public void BeginDrag(PointerEventData eventData)
        {
#if UNITY_STANDALONE
            if (eventData.currentInputModule.input.mousePresent)
            {
                if (eventData.button != PointerEventData.InputButton.Left) return;
            }
#endif
            
            if (!IsActive()) return;

            _isDrag = true;

            StartPosition = Target.localPosition;

            OnBeginDrag.Invoke(this);
        }
        
        public void Drag(PointerEventData eventData)
        {
#if UNITY_STANDALONE
            if (eventData.currentInputModule.input.mousePresent)
            {
                if (eventData.button != PointerEventData.InputButton.Left) return;
            }
#endif
            
            if (!_isDrag) return;
            if (eventData.used) return;

            eventData.Use();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Target, eventData.position, eventData.pressEventCamera, out var currentPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Target, eventData.pressPosition, eventData.pressEventCamera, out var originalPoint);

            Drag(currentPoint - originalPoint);
            
            OnDrag.Invoke(this);
        }

        public void Drag(Vector2 delta)
        {
            delta.y *= -1;

            var angle = Target.localRotation.eulerAngles.z * Mathf.Deg2Rad;
            var dragDelta = new Vector2(
                delta.x * Mathf.Cos(angle) + delta.y * Mathf.Sin(angle),
                delta.x * Mathf.Sin(angle) - delta.y * Mathf.Cos(angle));

            var position = new Vector3(StartPosition.x + dragDelta.x, StartPosition.y + dragDelta.y, StartPosition.z);

            position = _restriction == DraggableRestriction.Strict ? RestrictPosition(position) : position;

            Target.localPosition = position;

            CopyRectTransformValues();
        }

        public void EndDrag(PointerEventData eventData)
        {
#if UNITY_STANDALONE
            if (eventData.currentInputModule.input.mousePresent)
            {
                if (eventData.button != PointerEventData.InputButton.Left) return;
            }
#endif
            
            if (!_isDrag) return;
            
            Drag(eventData);

            _isDrag = false;
            
            OnEndDrag.Invoke(this);
        }

        private void SetTarget(RectTransform target)
        {
            if (target == null) return;

            _target = target;

            RectTransform.SetParent(_target.parent, false);

            CopyRectTransformValues();
        }

        private void SetHandle(RectTransform handle)
        {
            if (_handle != null)
            {
                RemoveListeners();
                
                Destroy(_dragHandle);
            }

            _handle = handle;
            _dragHandle = _handle.TryGetComponent<DraggableRectHandler>(out var dragHandle) 
                ? dragHandle 
                : _handle.gameObject.AddComponent<DraggableRectHandler>();

            AddListeners();
        }

        private void InteractableChange(bool interactable)
        {
            if (_interactable == interactable) return;
            
            _interactable = interactable;
            
            if (!IsActive()) return;
            
            OnInteractableChanged(Interactable);
        }

        private void OnInteractableChanged(bool interactable)
        {
            if (interactable) return;

            EndDrag(null);
        }

        private void AddListeners()
        {
            if (_dragHandle == null) return;
            
            _dragHandle.OnBeginDragEvent.AddListener(BeginDrag);
            _dragHandle.OnDragEvent.AddListener(Drag);
            _dragHandle.OnEndDragEvent.AddListener(EndDrag);
        }

        private void RemoveListeners()
        {
            if (_dragHandle == null) return;
            
            _dragHandle.OnBeginDragEvent.RemoveListener(BeginDrag);
            _dragHandle.OnDragEvent.RemoveListener(Drag);
            _dragHandle.OnEndDragEvent.RemoveListener(EndDrag);
        }

        private Vector3 RestrictPosition(Vector3 position)
        {
            var parent = Target.parent as RectTransform;

            if (parent == null) return position;

            var parentSize = parent.rect.size;
            var parentPivot = parent.pivot;
            
            var targetSize = Target.rect.size;
            var targetPivot = Target.pivot;
            
            var minX = -(parentSize.x * parentPivot.x) + (targetSize.x * targetPivot.x);
            var maxX = (parentSize.x - (1f - parentPivot.x)) - (targetSize.x * (1f - targetPivot.x));
            
            var minY = -(parentSize.y * parentPivot.y) + (targetSize.y * targetPivot.y);
            var maxY = (parentSize.y - (1f - parentPivot.y)) - (targetSize.y * (1f - targetPivot.y));

            return new Vector3(
                Mathf.Clamp(position.x, minX, maxX), 
                Mathf.Clamp(position.y, minY, maxY), 
                position.z);
        }

        private void CopyRectTransformValues()
        {
            RectTransform.localPosition = Target.localPosition;
            RectTransform.localRotation = Target.localRotation;
            RectTransform.localScale = Target.localScale;
            RectTransform.sizeDelta = Target.sizeDelta;
            RectTransform.anchorMin = Target.anchorMin;
            RectTransform.anchorMax = Target.anchorMax;
            RectTransform.pivot = Target.pivot;
        }

        public enum DraggableRestriction
        {
            None = 0,
            Strict = 1,
        }
        
        [Serializable]
        public class DraggableEvent : UnityEvent<DraggableRect>
        {
        }
    }
}
