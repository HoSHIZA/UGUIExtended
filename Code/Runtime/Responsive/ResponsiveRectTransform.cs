using System;
using System.Linq;
using KDebugger.Plugins.ShizoGames.ShizoUtility;
using KDebugger.Plugins.ShizoGames.UGUIExtended.Responsive.Base;
using UnityEngine;

namespace KDebugger.Plugins.ShizoGames.UGUIExtended.Responsive
{
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Responsive/Responsive RectTransform")]
    public class ResponsiveRectTransform : KResponsive
    {
        [Header("Config")]
        
        [Tooltip("Default installation profile.")]
        [SerializeField] private string _initialProfile = "Default";
        
        [Tooltip("Profiles for changing RectTransform settings.")]
        [SerializeField] private Profile[] _profiles = Array.Empty<Profile>();
        
        [Tooltip("Change RectTransform settings, when the screen orientation is landscape.")]
        [SerializeField, Space] private Entry[] _landscape = Array.Empty<Entry>();
        
        [Tooltip("Change RectTransform settings, when the screen orientation is portrait.")]
        [SerializeField] private Entry[] _portrait = Array.Empty<Entry>();
        
        private static readonly Vector2 _negativeVector2 = new Vector2(-1, -1);
        
        private void Start()
        {
            SetInitialProfile();
        }
        
        protected override void Refresh_Default()
        {
            
        }
        
        protected override void Refresh_UseScreenOrientation(ScreenUtility.ScreenOrientation orientation)
        {
            var entries = orientation == ScreenUtility.ScreenOrientation.Landscape
                ? _landscape
                : _portrait;
            
            foreach (var entry in entries)
            {
                foreach (var target in entry.Targets)
                {
                    if (!target) continue;

                    SetTargetValuesFromEntry(target, entry);
                }
            }
        }
        
        public void SetInitialProfile()
        {
            if (string.IsNullOrEmpty(_initialProfile)) return;
            
            SetProfile(_initialProfile);
        }
        
        public void SetProfile(string id)
        {
            if (_refreshMode == RefreshMode.UseScreenOrientation) return;
            
            id = id.ToLower();

            var profile = _profiles.FirstOrDefault(t => t.Id.ToLower() == id);

            if (profile == null) return;

            foreach (var entry in profile.Entries)
            {
                foreach (var target in entry.Targets)
                {
                    SetTargetValuesFromEntry(target, entry);
                }
            }
        }
        
        private void SetTargetValuesFromEntry(RectTransform target, Entry entry)
        {
            target.anchorMin = GetFixedVectorValue(target.anchorMin, entry.AnchorMin);
            target.anchorMax = GetFixedVectorValue(target.anchorMax, entry.AnchorMax);
            target.anchoredPosition = GetFixedVectorValue(target.anchoredPosition, entry.AnchoredPosition);
            target.sizeDelta = GetFixedVectorValue(target.sizeDelta, entry.SizeDelta);
        }
        
        private Vector2 GetFixedVectorValue(Vector2 target, Vector2 value)
        {
            var result = new Vector2(
                Math.Abs(value.x - (-1)) < 0.01f ? target.x : value.x,
                Math.Abs(value.y - (-1)) < 0.01f ? target.y : value.y);

            return result;
        }
        
        [Serializable]
        public sealed class Profile
        {
            [SerializeField] private string _id;
            [Space]
            [SerializeField] private Entry[] _entries = Array.Empty<Entry>();

            public string Id => _id;
            public Entry[] Entries => _entries;
        }
        
        [Serializable]
        public sealed class Entry
        {
            [SerializeField] private RectTransform[] _targets = Array.Empty<RectTransform>();
            [Space]
            [SerializeField] private Vector2 _anchorMin = _negativeVector2;
            [SerializeField] private Vector2 _anchorMax = _negativeVector2;
            [SerializeField] private Vector2 _anchoredPosition = _negativeVector2;
            [SerializeField] private Vector2 _sizeDelta = _negativeVector2;
            
            public RectTransform[] Targets => _targets;
            public Vector2 AnchorMin => _anchorMin;
            public Vector2 AnchorMax => _anchorMax;
            public Vector2 AnchoredPosition => _anchoredPosition;
            public Vector2 SizeDelta => _sizeDelta;
        }
    }
}