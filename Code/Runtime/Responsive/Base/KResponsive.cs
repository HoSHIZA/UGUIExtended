using System;
using ShizoGames.ShizoUtility;
using UnityEngine;

namespace ShizoGames.UGUIExtended.Responsive.Base
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public abstract class KResponsive : MonoBehaviour
    {
        [Tooltip("Sets the refresh mode of the responsive interface element.")]
        [SerializeField] protected RefreshMode _refreshMode;
        
        private bool _dirty;
        
        private RectTransform _rectTransform;
        protected RectTransform RectTransform => _rectTransform
            ? _rectTransform
            : _rectTransform = (RectTransform)transform;
        
        private void OnEnable()
        {
            _dirty = true;

            ScreenUtility.OrientationChanged += Refresh_UseScreenOrientation;
        }
        
        private void OnDisable()
        {
            ScreenUtility.OrientationChanged -= Refresh_UseScreenOrientation;
        }
        
        private void OnRectTransformDimensionsChange()
        {
            if (_refreshMode == RefreshMode.Default)
            {
                _dirty = true;
            }
        }
        
        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                RefreshUseRefreshMode(_refreshMode);
                
                return;
            }
#endif
            
            if (!_dirty) return;
            
            RefreshUseRefreshMode(_refreshMode);
            
            _dirty = false;
        }
        
        public void SetRefreshMode(RefreshMode mode)
        {
            _refreshMode = _refreshMode == mode
                ? _refreshMode
                : mode;
            
            RefreshUseRefreshMode(_refreshMode);
        }
        
        public void SetDirty()
        {
            _dirty = true;
        }
        
        private void RefreshUseRefreshMode(RefreshMode refreshMode)
        {
            switch (refreshMode)
            {
                case RefreshMode.Default:
                    Refresh_Default();
                    break;
                case RefreshMode.UseScreenOrientation:
                    Refresh_UseScreenOrientation(ScreenUtility.CurrentOrientation);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(refreshMode), refreshMode, null);
            }
        }
        
        protected abstract void Refresh_Default();
        protected abstract void Refresh_UseScreenOrientation(ScreenUtility.ScreenOrientation orientation);
        
        public enum RefreshMode
        {
            Default,
            UseScreenOrientation,
        }
    }
}