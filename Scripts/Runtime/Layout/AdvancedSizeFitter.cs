using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShizoGames.ImprovedUI.Layout
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Layout/Advanced Size Fitter")]
    public class AdvancedSizeFitter : UIBehaviour, ILayoutSelfController
    {
        [SerializeField] private FitMode _horizontalFit = FitMode.Unconstrained;
        [SerializeField] private FitMode _verticalFit = FitMode.Unconstrained;

        private DrivenRectTransformTracker _tracker;

        public FitMode HorizontalFit
        { 
            get => _horizontalFit;
            set
            {
                _horizontalFit = value;
                
                SetDirty();
            }
        }

        public FitMode VerticalFit
        { 
            get => _verticalFit;
            set
            {
                _verticalFit = value;
                
                SetDirty();
            }
        }

        private RectTransform _rectTransform;
        private RectTransform RectTransform => _rectTransform = _rectTransform 
            ? _rectTransform 
            : transform as RectTransform;
        
        private RectTransform _parentRectTransform;
        private RectTransform ParentRectTransform => _parentRectTransform = _parentRectTransform 
            ? _parentRectTransform 
            : transform.parent as RectTransform;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            SetDirty();
        }

        protected override void OnDisable()
        {
            _tracker.Clear();
            
            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
            
            base.OnDisable();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }

        public virtual void SetLayoutHorizontal()
        {
            _tracker.Clear();
            
            HandleSelfFittingAlongAxis(0);
        }

        public virtual void SetLayoutVertical()
        {
            _tracker.Clear();
            
            HandleSelfFittingAlongAxis(1);
        }

        private void HandleSelfFittingAlongAxis(int axis)
        {
            var fitting = axis == 0 ? HorizontalFit : VerticalFit;
            
            if (fitting == FitMode.Unconstrained)
            {
                _tracker.Add(this, _rectTransform, DrivenTransformProperties.None);

                return;
            }
            
            _tracker.Add(this, RectTransform, axis == 0
                ? DrivenTransformProperties.SizeDeltaX
                : DrivenTransformProperties.SizeDeltaY);
            
            var rectSize = axis == 0 ? ParentRectTransform.rect.width : ParentRectTransform.rect.height;
            var minSize = LayoutUtility.GetMinSize(RectTransform, axis);
            var preferredSize = LayoutUtility.GetPreferredSize(RectTransform, axis);
            
            switch (fitting)
            {
                case FitMode.MinSize:
                {
                    RectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, minSize);
                    
                    break;
                }
                case FitMode.PreferredSize:
                {
                    RectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, preferredSize);
                    
                    break;
                }
                case FitMode.PreferredSizeUpperThreshold:
                {
                    RectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis) axis, Mathf.Min(preferredSize, rectSize));

                    break;
                }
                case FitMode.PreferredSizeLowerThreshold:
                {
                    RectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, preferredSize >= rectSize ? preferredSize : rectSize);
                    
                    break;
                }
            }
        }

        private void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            _rectTransform = transform as RectTransform;
            
            SetDirty();
        }
#endif

        public enum FitMode
        {
            Unconstrained,
            MinSize,
            PreferredSize,
            PreferredSizeUpperThreshold,
            PreferredSizeLowerThreshold,
        }
    }
}