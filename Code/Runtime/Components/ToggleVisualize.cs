using UnityEngine;
using UnityEngine.UI;

namespace KDebugger.Plugins.ShizoGames.UGUIExtended.Components
{
    [RequireComponent(typeof(Toggle))]
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Toggle Visualize")]
    public class ToggleVisualize : MonoBehaviour
    {
        [SerializeField] private bool _subscribe;
        [SerializeField] private bool _changeTargetGraphic;
        [Space]
        [SerializeField] private GameObject _onDisplay;
        [SerializeField] private GameObject _offDisplay;

        public GameObject CurrentDisplay => _toggle.isOn ? _onDisplay : _offDisplay;
        public GameObject OnDisplay => _onDisplay;
        public GameObject OffDisplay => _offDisplay;

        public Image CurrentDisplayImage => _toggle.isOn ? OnDisplayImage : OffDisplayImage;
        
        private Image _onDisplayImage;
        public Image OnDisplayImage => _onDisplayImage
            ? _onDisplayImage
            : _onDisplayImage = OnDisplay.GetComponent<Image>();

        private Image _offDisplayImage;
        public Image OffDisplayImage => _offDisplayImage
            ? _offDisplayImage
            : _offDisplayImage = OffDisplay.GetComponent<Image>();

        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();

            UpdateDisplay(_toggle.isOn);
            
            if (_subscribe)
            {
                _toggle.onValueChanged.AddListener(UpdateDisplay);
            }
        }

        public void UpdateDisplay(bool isOn)
        {
            if (!_onDisplay || !_offDisplay) return;
            
            _onDisplay.SetActive(isOn);
            _offDisplay.SetActive(!isOn);
            
            if (_changeTargetGraphic && CurrentDisplayImage != null)
            {
                _toggle.targetGraphic = CurrentDisplayImage;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_subscribe) return; 
            if (_toggle) return;
            
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(UpdateDisplay);
        }
#endif
    }
}