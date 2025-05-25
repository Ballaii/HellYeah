using Eliot.Utility;
using UnityEditor;
using UnityEngine;

namespace Eliot.BehaviourEditor.Editor
{
    /// <summary>
    /// Editor script for Skill Node.
    /// </summary>
    [CustomEditor(typeof(SkillNode))]
    public class SkillNodeEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Used to identify and access the Node via code.
        /// </summary>
        private SerializedProperty _nodeId;
        
        /// <summary>
        /// Whether the node captures control.
        /// </summary>
        public SerializedProperty CaptureControl;
        
        /// <summary>
        /// Should the Skill be executed or just set as the current one.
        /// </summary>
        public SerializedProperty ExecuteSkill;
        
        /// <summary>
        /// The Skill reference to use.
        /// </summary>
        public SerializedProperty Skill;

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        private void OnEnable()
        {
            _nodeId = serializedObject.FindProperty("NodeId");
            CaptureControl = serializedObject.FindProperty("CaptureControl");
            ExecuteSkill = serializedObject.FindProperty("ExecuteSkill");
            Skill = serializedObject.FindProperty("Skill");
        }

        /// <summary>
        /// Draw the inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EliotEditorUtility.DrawMiniLabel("Node: Skill", EliotGUISkin.GrayBackground);
            
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_nodeId, new GUIContent("Node Id", "Used to identify and access the Node via code"));
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(CaptureControl);
            EditorGUILayout.EndVertical(); 
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(ExecuteSkill);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(Skill);
            EditorGUILayout.EndVertical();
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Skill Node has been modified");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}