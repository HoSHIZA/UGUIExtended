using TMPro;
using UnityEngine;

namespace ShizoGames.UGUIExtended
{
    [DisallowMultipleComponent]
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Drag/Value Drag Field")]
    public sealed class ValueDragField : MonoBehaviour
    {
        [SerializeField] private ValueDragHandler _valueDragHandler;
        [SerializeField] private TMP_InputField _inputField;
        
        internal ValueDragHandler ValueDragHandler => _valueDragHandler;
        internal TMP_InputField InputField => _inputField;
        
        public string Value
        {
            get => _inputField.text;
            set => _inputField.text = value;
        }
        
        public void Setup(string defaultValue)
        {
            Value = defaultValue;
        }
    }
}