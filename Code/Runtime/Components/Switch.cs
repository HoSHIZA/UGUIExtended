using System;
using ShizoGames.UGUIExtended.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ShizoGames.UGUIExtended.Components
{
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Switch")]
    public sealed class Switch : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private bool _isOn;
        [SerializeField] private bool _enableAnimations;
        [SerializeField] private float _animationDuration = 0.15f;
        [SerializeField] private AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
        
        [Header("Components")]
        [SerializeField] private Button _button;
        [SerializeField] private Image _background;
        [SerializeField] private Image _knob;
        
        [Header("Config - Off")]
        [SerializeField] private Color _offBackgroundColor;
        [SerializeField] private Color _offKnobColor;
        
        [Header("Config - On")]
        [SerializeField] private Color _onBackgroundColor;
        [SerializeField] private Color _onKnobColor;
        
        [Space]
        [SerializeField] private UnityEvent<bool> _onValueChanged;

        private Slider _slider;
        private Slider Slider => _slider
            ? _slider
            : _slider = GetComponentInChildren<Slider>();

        public event Action<bool> OnValueChanged;

        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (_isOn == value) return;
                
                SetValue(value);
            }
        }

        private void Awake()
        {
            _button.onClick.AddListener(() => SetValue(!_isOn));
        }

        public void SetValue(bool isOn, bool silent = false, bool skipAnimation = false)
        {
            var sliderValue = isOn ? 1f : 0f;
            
            if (_enableAnimations && !skipAnimation)
            {
                InterpolationUtility.Interpolate(this, value => Slider.value = value, 
                    Slider.value, sliderValue, _animationDuration, _curve);
            }
            else
            {
                Slider.value = sliderValue;
            }

            if (_offBackgroundColor != _onBackgroundColor)
            {
                var color = isOn ? _onBackgroundColor : _offBackgroundColor;
                
                if (_enableAnimations && !skipAnimation)
                {
                    InterpolationUtility.Interpolate(this, value => _background.color = value, 
                        _background.color, color, _animationDuration);
                }
                else
                {
                    _background.color = color;
                }
            }
            
            if (_offKnobColor != _onKnobColor)
            {
                var color = isOn ? _onKnobColor : _offKnobColor;
                
                if (_enableAnimations && !skipAnimation)
                {
                    InterpolationUtility.Interpolate(this, value => _knob.color = value, 
                        _knob.color, color, _animationDuration);
                }
                else
                {
                    _knob.color = color;
                }
            }
            
            _isOn = isOn;
            
            if (!silent)
            {
                _onValueChanged?.Invoke(isOn);
                OnValueChanged?.Invoke(isOn);
            }
        }

#if UNITY_EDITOR
        private bool _editorPrevValue;
        
        private void OnValidate()
        {
            if (_isOn == _editorPrevValue) return;
            
            SetValue(_isOn, true, true);
            
            _onValueChanged?.Invoke(_isOn);
            
            _editorPrevValue = _isOn;
        }
#endif
    }
}