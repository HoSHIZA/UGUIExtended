using System;
using UnityEngine;
using UnityEngine.UI;

namespace KDebugger.Plugins.ShizoGames.UGUIExtended.Layout
{
    [AddComponentMenu(PackageConfig.ADD_COMPONENT_UI_ROOT + "Layout/Advanced Layout Group")]
    public class AdvancedLayoutGroup : HorizontalOrVerticalLayoutGroup
    {
        [SerializeField] private LayoutGroupType _layoutType;
        
        private bool IsVertical => _layoutType == LayoutGroupType.Vertical;
        
        protected AdvancedLayoutGroup()
        {}
        
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            
            CalcAlongAxis(0, IsVertical);
        }
        
        public override void CalculateLayoutInputVertical()
        {
            CalcAlongAxis(1, IsVertical);
        }
        
        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis(0, IsVertical);
        }
        
        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis(1, IsVertical);
        }
        
        public void ChangeLayoutType(LayoutGroupType type)
        {
            if (_layoutType == type) return;
            
            _layoutType = type;
            
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
        
        [Serializable]
        public enum LayoutGroupType
        {
            Vertical,
            Horizontal,
        }
    }
}

#if UNITY_EDITOR
// ReSharper disable once CheckNamespace
namespace KDebugger.Plugins.ShizoGames.UGUIExtended.Editor.Layout
{
    using UnityEngine;
    using UnityEditor;
    using KDebugger.Plugins.ShizoGames.UGUIExtended.Layout;

    [CustomEditor(typeof(AdvancedLayoutGroup), true)]
    [CanEditMultipleObjects]
    public class AdvancedLayoutGroupEditor : Editor
    {
        private SerializedProperty _layoutType;
        private SerializedProperty m_Padding;
        private SerializedProperty m_Spacing;
        private SerializedProperty m_ChildAlignment;
        private SerializedProperty m_ReverseArrangement;
        private SerializedProperty m_ChildControlWidth;
        private SerializedProperty m_ChildControlHeight;
        private SerializedProperty m_ChildScaleWidth;
        private SerializedProperty m_ChildScaleHeight;
        private SerializedProperty m_ChildForceExpandWidth;
        private SerializedProperty m_ChildForceExpandHeight;

        protected virtual void OnEnable()
        {
            _layoutType = serializedObject.FindProperty("_layoutType");
            m_Padding = serializedObject.FindProperty("m_Padding");
            m_Spacing = serializedObject.FindProperty("m_Spacing");
            m_ChildAlignment = serializedObject.FindProperty("m_ChildAlignment");
            m_ReverseArrangement = serializedObject.FindProperty("m_ReverseArrangement");
            m_ChildControlWidth = serializedObject.FindProperty("m_ChildControlWidth");
            m_ChildControlHeight = serializedObject.FindProperty("m_ChildControlHeight");
            m_ChildScaleWidth = serializedObject.FindProperty("m_ChildScaleWidth");
            m_ChildScaleHeight = serializedObject.FindProperty("m_ChildScaleHeight");
            m_ChildForceExpandWidth = serializedObject.FindProperty("m_ChildForceExpandWidth");
            m_ChildForceExpandHeight = serializedObject.FindProperty("m_ChildForceExpandHeight");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_layoutType, true);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_Padding, true);
            EditorGUILayout.PropertyField(m_Spacing, true);
            EditorGUILayout.PropertyField(m_ChildAlignment, true);
            EditorGUILayout.PropertyField(m_ReverseArrangement, true);

            var rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, -1, EditorGUIUtility.TrTextContent("Control Child Size"));
            rect.width = Mathf.Max(50, (rect.width - 4) / 3);
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, m_ChildControlWidth, EditorGUIUtility.TrTextContent("Width"));
            rect.x += rect.width + 2;
            ToggleLeft(rect, m_ChildControlHeight, EditorGUIUtility.TrTextContent("Height"));
            EditorGUIUtility.labelWidth = 0;

            rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, -1, EditorGUIUtility.TrTextContent("Use Child Scale"));
            rect.width = Mathf.Max(50, (rect.width - 4) / 3);
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, m_ChildScaleWidth, EditorGUIUtility.TrTextContent("Width"));
            rect.x += rect.width + 2;
            ToggleLeft(rect, m_ChildScaleHeight, EditorGUIUtility.TrTextContent("Height"));
            EditorGUIUtility.labelWidth = 0;

            rect = EditorGUILayout.GetControlRect();
            rect = EditorGUI.PrefixLabel(rect, -1, EditorGUIUtility.TrTextContent("Child Force Expand"));
            rect.width = Mathf.Max(50, (rect.width - 4) / 3);
            EditorGUIUtility.labelWidth = 50;
            ToggleLeft(rect, m_ChildForceExpandWidth, EditorGUIUtility.TrTextContent("Width"));
            rect.x += rect.width + 2;
            ToggleLeft(rect, m_ChildForceExpandHeight, EditorGUIUtility.TrTextContent("Height"));
            EditorGUIUtility.labelWidth = 0;

            serializedObject.ApplyModifiedProperties();
        }

        private void ToggleLeft(Rect position, SerializedProperty property, GUIContent label)
        {
            var toggle = property.boolValue;
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            toggle = EditorGUI.ToggleLeft(position, label, toggle);
            EditorGUI.indentLevel = oldIndent;
            if (EditorGUI.EndChangeCheck())
            {
                property.boolValue = property.hasMultipleDifferentValues || !property.boolValue;
            }
            EditorGUI.showMixedValue = false;
        }
    }
}
#endif
