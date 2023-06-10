using UnityEngine;
using UnityEngine.UI;

namespace ShizoGames.UGUIExtended.Components
{
    [RequireComponent(typeof(Toggle))]
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Toggle Splitter")]
    public class ToggleSplitter : MonoBehaviour
    {
        [SerializeField] private bool _subscribe;
        [Space]
        [SerializeField] private Toggle.ToggleEvent _toggleOn = new Toggle.ToggleEvent();
        [SerializeField] private Toggle.ToggleEvent _toggleOff = new Toggle.ToggleEvent();

        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();

            if (_subscribe)
            {
                _toggle.onValueChanged.AddListener(UpdateValue);
            }
        }

        public void UpdateValue(bool isOn)
        {
            if (isOn)
            {
                _toggleOn?.Invoke(true);
            }
            else
            {
                _toggleOff?.Invoke(false);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_subscribe) return; 
            if (_toggle) return;
            
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(UpdateValue);
        }
#endif
    }
}