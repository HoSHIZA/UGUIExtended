using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KDebugger.Plugins.ShizoGames.UGUIExtended.Layout
{
    [ExecuteAlways]
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Layout/Size Equalizer")]
    public sealed class SizeEqualizer : UIBehaviour
    {
        [SerializeField] private FitType _fitType;

        private LayoutElement _layoutElement;

        private RectTransform _rectTransform;
        private RectTransform RectTransform => _rectTransform = _rectTransform
            ? _rectTransform
            : transform as RectTransform;

        protected override void Awake()
        {
            if (!Application.isPlaying) return;

            TryGetComponent(out _layoutElement);
        }

        private void Update()
        {
            var rectTransformSizeDelta = RectTransform.sizeDelta;

            switch (_fitType)
            {
                case FitType.WidthFromHeight:
                {
                    if (_layoutElement)
                    {
                        _layoutElement.minWidth = rectTransformSizeDelta.y;
                        _layoutElement.preferredWidth = rectTransformSizeDelta.y;
                    }
                    else
                    {
                        RectTransform.sizeDelta = new Vector2(rectTransformSizeDelta.y, rectTransformSizeDelta.y);
                    }
                    
                    break;
                }
                case FitType.HeightFromWidth:
                {
                    if (_layoutElement)
                    {
                        _layoutElement.minHeight = rectTransformSizeDelta.x;
                        _layoutElement.preferredHeight = rectTransformSizeDelta.x;
                    }
                    else
                    {
                        RectTransform.sizeDelta = new Vector2(rectTransformSizeDelta.x, rectTransformSizeDelta.x);
                    }
                    
                    break;
                }
            }
        }
        
        #pragma warning disable CS0114
        private void Reset()
        {
            _layoutElement = null;
        }
        #pragma warning restore CS0114

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (_layoutElement) return;

            _layoutElement = GetComponent<LayoutElement>();
        }
#endif
        
        private enum FitType
        {
            WidthFromHeight,
            HeightFromWidth,
        }
    }
}