using ShizoGames.UGUIExtended.Components;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace ShizoGames.ImprovedUI.Editor.Components
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NonDrawingGraphic), false)]
    public class NonDrawingGraphicEditor : GraphicEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            GUI.enabled = false;
            EditorGUILayout.PropertyField(m_Script);
            GUI.enabled = true;
            
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}