using System;
using System.Collections.Generic;
using ShizoGames.ImprovedUI.Responsive.Base;
using ShizoGames.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace ShizoGames.ImprovedUI.Responsive
{
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Responsive/Responsive Swap Container")]
    public class ResponsiveSwapContainer : KResponsive
    {
        [Header("Config")]
        
        [Tooltip("Setting the container at objects, when the screen orientation is landscape.")]
        [SerializeField] private Entry[] _landscape = Array.Empty<Entry>();
        
        [Tooltip("Setting the container at objects, when the screen orientation is portrait.")]
        [SerializeField] private Entry[] _portrait = Array.Empty<Entry>();
        
        protected override void Refresh_Default()
        {
            
        }
        
        protected override void Refresh_UseScreenOrientation(ScreenUtility.ScreenOrientation orientation)
        {
            var isLandscape = orientation == ScreenUtility.ScreenOrientation.Landscape;
            
            Swap(_landscape, isLandscape);
            Swap(_portrait, !isLandscape);
        }

        private void Awake()
        {
            _refreshMode = RefreshMode.UseScreenOrientation;
        }

        private void Swap(IEnumerable<Entry> entries, bool condition)
        {
            if (!condition) return;

            foreach (var entry in entries)
            {
                if (!entry.From || !entry.To) continue;
                if (entry.From.childCount == 0) continue;

                var childCount = entry.From.childCount;
                for (var i = 0; i < childCount; i++)
                {
                    entry.From.GetChild(0).SetParent(entry.To);
                }

                if (entry.EnableAndDisableContainers)
                {
                    entry.From.gameObject.SetActive(false);
                    entry.To.gameObject.SetActive(true);
                }
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(entry.From);
                LayoutRebuilder.ForceRebuildLayoutImmediate(entry.To);
            }
        }
        
        [Serializable]
        public struct Entry
        {
            [SerializeField] private RectTransform _from;
            [SerializeField] private RectTransform _to;
            [SerializeField] private bool _enableAndDisableContainers;
            
            public RectTransform From => _from;
            public RectTransform To => _to;
            public bool EnableAndDisableContainers => _enableAndDisableContainers;
        }
    }
}