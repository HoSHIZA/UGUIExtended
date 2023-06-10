using System;
using ShizoGames.ShizoUtility;
using ShizoGames.UGUIExtended.Responsive.Base;
using UnityEngine;
using UnityEngine.UI;

namespace ShizoGames.UGUIExtended.Responsive
{
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Responsive/Responsive Resize")]
    public class ResponsiveResize : KResponsive
    {
        [Header("Config")]
        
        [Tooltip("Elements for resizing.")]
        [SerializeField] private Element[] _elements = Array.Empty<Element>();
        
        [Tooltip("Elements for resizing, when the screen orientation is landscape.")]
        [SerializeField, Space] private ElementOrientationBased[] _landscape = Array.Empty<ElementOrientationBased>();
        
        [Tooltip("Elements for resizing, when the screen orientation is portrait.")]
        [SerializeField] private ElementOrientationBased[] _portrait = Array.Empty<ElementOrientationBased>();
        
        protected override void Refresh_Default()
        {
            foreach (var element in _elements)
            {
                if (!element.Target) continue;
                
                var maxWidth = float.MinValue;
                var selectedWidth = -1f;
                
                foreach (var size in element.SizeDefinitions)
                {
                    if (size.Threshold <= RectTransform.rect.width && size.Threshold > maxWidth)
                    {
                        maxWidth = size.Threshold;
                        selectedWidth = size.Width;
                    }
                }
                
                if (selectedWidth > 0)
                {
                    element.Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, selectedWidth);
                    
                    if (TryGetComponent<LayoutElement>(out var layoutElement))
                    {
                        layoutElement.preferredWidth = selectedWidth;
                    }
                }
            }
        }

        protected override void Refresh_UseScreenOrientation(ScreenUtility.ScreenOrientation orientation)
        {
            var elements = orientation == ScreenUtility.ScreenOrientation.Landscape
                ? _landscape
                : _portrait;
            
            foreach (var element in elements)
            {
                if (!element.Target) continue;
                
                if (element.Target.TryGetComponent<LayoutElement>(out var layoutElement))
                {
                    if (element.Width > 0)
                    {
                        layoutElement.minWidth = element.Width;
                        layoutElement.preferredWidth = element.Width;
                    }

                    if (element.Height > 0)
                    {
                        layoutElement.minHeight = element.Height;
                        layoutElement.preferredHeight = element.Height;
                    }
                }
                else
                {
                    if (element.Width > 0)
                    {
                        element.Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, element.Width);
                    }

                    if (element.Height > 0)
                    {
                        element.Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, element.Height);
                    }
                }
            }
        }
        
        [Serializable]
        public class Element
        {
            [SerializeField] private SizeDefinition[] _sizeDefinitions = Array.Empty<SizeDefinition>();
            [SerializeField] private RectTransform _target;
            
            public SizeDefinition[] SizeDefinitions => _sizeDefinitions;
            public RectTransform Target => _target;

            [Serializable]
            public struct SizeDefinition
            {
                [SerializeField] private float _width;
                [SerializeField] private float _threshold;
                
                public float Width => _width;
                public float Threshold => _threshold;
            }
        }
        
        [Serializable]
        public class ElementOrientationBased
        {
            [SerializeField] private RectTransform _target;
            [SerializeField] private float _width;
            [SerializeField] private float _height;

            public RectTransform Target => _target;
            public float Width => _width;
            public float Height => _height;
        }
    }
}