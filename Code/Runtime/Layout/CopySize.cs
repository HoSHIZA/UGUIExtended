using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShizoGames.UGUIExtended.Layout
{
    [ExecuteAlways]
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Layout/Copy Size")]
    public sealed class CopySize : UIBehaviour
    {
        [SerializeField] private CopySizeMode _copyMode;
        [SerializeField] private CopySizeTarget _copyTarget;
        [SerializeField] private RectTransform _target;
        
        [Space]
        [SerializeField] private bool _width;
        [SerializeField] private bool _height;
        
        [Space]
        [SerializeField] private float _widthIncrement;
        [SerializeField] private float _heightIncrement;
        
        private LayoutElement _toLayoutElement;
        private RectTransform _from;
        private RectTransform _to;

        protected override void Awake()
        {
            if (!Application.isPlaying) return;
            
            Refresh();
        }

        private void Update()
        {
            if (_copyMode == CopySizeMode.CopyForTarget) return;
            
            Refresh();
        }

        protected override void OnTransformParentChanged()
        {
            if (_copyTarget != CopySizeTarget.Parent) return;
            
            if (_copyMode == CopySizeMode.CopyFromTarget)
            {
                Setup(true);
            }
            else
            {
                Refresh();
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            if (_copyMode == CopySizeMode.CopyFromTarget) return;
            
            Refresh();
        }

        private void OnTransformChildrenChanged()
        {
            if (_copyTarget != CopySizeTarget.Children) return;
            
            if (_copyMode == CopySizeMode.CopyFromTarget)
            {
                Setup(true);
            }
            else
            {
                Refresh();
            }
        }

        private void Refresh()
        {
#if UNITY_EDITOR
            if (_copyTarget == CopySizeTarget.Parent || _copyTarget == CopySizeTarget.Children)
            {
                Setup(true);
            }
            else
            {
                Setup();
            }
#endif
            
            if (!_target || !_from || !_to) return;
            
            var fromSizeDelta = _from.sizeDelta;
            
            if (_width)
            {
                if (_toLayoutElement && !_toLayoutElement.ignoreLayout)
                {
                    _toLayoutElement.minWidth = fromSizeDelta.x + _widthIncrement;
                    _toLayoutElement.preferredWidth = fromSizeDelta.x + _widthIncrement;
                }
                else
                {
                    _to.sizeDelta = new Vector2(fromSizeDelta.x + _widthIncrement, _to.sizeDelta.y);
                }
            }

            if (_height)
            {
                if (_toLayoutElement && !_toLayoutElement.ignoreLayout)
                {
                    _toLayoutElement.minHeight = fromSizeDelta.y + _heightIncrement;
                    _toLayoutElement.preferredHeight = fromSizeDelta.y + _heightIncrement;
                }
                else
                {
                    _to.sizeDelta = new Vector2(_to.sizeDelta.x, fromSizeDelta.y + _heightIncrement);
                }
            }
        }

        private void Setup(bool resetTarget = false)
        {
            if (resetTarget)
            {
                _target = null;
            }
            
            Reset();
            
            switch (_copyTarget)
            {
                case CopySizeTarget.Target:
                    break;
                case CopySizeTarget.Parent:
                    _target = transform.parent as RectTransform;
                    break;
                case CopySizeTarget.Children:
                    for (var i = 0; i < transform.childCount; i++)
                    {
                        var child = transform.GetChild(i);

                        if (!child.gameObject.activeSelf) continue;
                        
                        _target = child as RectTransform;
                            
                        break;
                    }
                    break;
            }
            
            if (!_target) return;
            
            switch (_copyMode)
            {
                case CopySizeMode.CopyForTarget:
                    _from = (RectTransform)transform;
                    _to = _target;
                    break;
                case CopySizeMode.CopyFromTarget:
                    _from = _target;
                    _to = (RectTransform)transform;
                    break;
            }
            
            _toLayoutElement = _to.GetComponent<LayoutElement>();
        }

        private void Reset()
        {
            _toLayoutElement = null;
            _from = null;
            _to = null;
        }
        
        public enum CopySizeMode
        {
            CopyFromTarget,
            CopyForTarget,
        }
        
        public enum CopySizeTarget
        {
            Target,
            Parent,
            Children,
        }
    }
}