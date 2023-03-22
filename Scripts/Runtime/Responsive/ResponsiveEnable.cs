using System;
using ShizoGames.ImprovedUI.Responsive.Base;
using ShizoGames.Utilities;
using UnityEngine;

namespace ShizoGames.ImprovedUI.Responsive
{
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Responsive/Responsive Enable")]
    public class ResponsiveEnable : KResponsive
    {
        [Header("Config")]
        
        [Tooltip("Entries for enabling/disabling.")]
        [SerializeField] private Entry[] _entries = Array.Empty<Entry>();
        
        [Tooltip("Enable/disable, when the screen orientation is landscape.")]
        [SerializeField, Space] private EntryOrientationBased _landscape;
        
        [Tooltip("Enable/disable, when the screen orientation is portrait.")]
        [SerializeField] private EntryOrientationBased _portrait;
        
        protected override void Refresh_Default()
        {
            foreach (var entry in _entries)
            {
                var enable = true;

                switch (entry.EnableMode)
                {
                    case EnableModes.Above:
                    {
                        if (entry.ThresholdHeight > 0)
                        {
                            enable = RectTransform.rect.height >= entry.ThresholdHeight;
                        }
                        else if (entry.ThresholdWidth > 0)
                        {
                            enable = RectTransform.rect.width >= entry.ThresholdWidth;
                        }

                        break;
                    }
                    case EnableModes.Below:
                    {
                        if (entry.ThresholdHeight > 0)
                        {
                            enable = RectTransform.rect.height <= entry.ThresholdHeight;
                        }
                        else if (entry.ThresholdWidth > 0)
                        {
                            enable = RectTransform.rect.width <= entry.ThresholdWidth;
                        }

                        break;
                    }
                }

                foreach (var obj in entry.GameObjects)
                {
                    if (obj) obj.SetActive(enable);
                }

                foreach (var component in entry.Components)
                {
                    if (component) component.enabled = enable;
                }
            }
        }
        
        protected override void Refresh_UseScreenOrientation(ScreenUtility.ScreenOrientation orientation)
        {
            var isLandscape = orientation == ScreenUtility.ScreenOrientation.Landscape;
            var isPortrait = orientation == ScreenUtility.ScreenOrientation.Portrait;

            foreach (var obj in _landscape.GameObjects)
            {
                if (obj) obj.SetActive(isLandscape);
            }

            foreach (var component in _landscape.Components)
            {
                if (component) component.enabled = isLandscape;
            }

            foreach (var obj in _portrait.GameObjects)
            {
                if (obj) obj.SetActive(isPortrait);
            }

            foreach (var component in _portrait.Components)
            {
                if (component) component.enabled = isPortrait;
            }
        }

        /// <summary>
        /// A class to represent entry, which should change size depending on the threshold value.
        /// </summary>
        [Serializable]
        public class Entry
        {
            [SerializeField] private Behaviour[] _components = Array.Empty<Behaviour>();
            [SerializeField] private GameObject[] _gameObjects = Array.Empty<GameObject>();
            [Space]
            [SerializeField] private EnableModes _enableMode;
            [SerializeField] private float _thresholdHeight;
            [SerializeField] private float _thresholdWidth;
            
            public Behaviour[] Components => _components;
            public GameObject[] GameObjects => _gameObjects;
            
            public EnableModes EnableMode => _enableMode;
            public float ThresholdHeight => _thresholdHeight;
            public float ThresholdWidth => _thresholdWidth;
        }
        
        /// <summary>
        /// A class to represent entry, which should change size depending when screen orientation changed.
        /// </summary>
        [Serializable]
        public class EntryOrientationBased
        {
            [SerializeField] private Behaviour[] _components = Array.Empty<Behaviour>();
            [SerializeField] private GameObject[] _gameObjects = Array.Empty<GameObject>();
            
            public Behaviour[] Components => _components;
            public GameObject[] GameObjects => _gameObjects;
        }
        
        /// <summary>
        /// Object or component enable mode.
        /// </summary>
        public enum EnableModes
        {
            Above,
            Below,
        }
    }
}