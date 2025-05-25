using Eliot.Utility;
using UnityEditor;
using UnityEngine;

namespace Eliot.BehaviourEditor.Editor
{
    /// <summary>
    /// Editor script for Time Node.
    /// </summary>
    [CustomEditor(typeof(TimeNode))]
    public class TimeNodeEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Used to identify and access the Node via code.
        /// </summary>
        private SerializedProperty _nodeId;
        
        /// <summary>
        /// Minimum time to be executing the 'while' transitions.
        /// </summary>
        public SerializedProperty MinTime;
        
        /// <summary>
        /// Maximum time to be executing the 'while' transitions.
        /// </summary>
        public SerializedProperty MaxTime;

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        private void OnEnable()
        {
            _nodeId = serializedObject.FindProperty("NodeId");
            MinTime = serializedObject.FindProperty("MinTime");
            MaxTime = serializedObject.FindProperty("MaxTime");
        }

        /// <summary>
        /// Draw the inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EliotEditorUtility.DrawMiniLabel("Node: Time", EliotGUISkin.GrayBackground);
            
            serializedObject.Update();
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_nodeId, new GUIContent("Node Id", "Used to identify and access the Node via code"));
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.HelpBox("Connect the Node that needs to be run for the specified " +
                                    "period with the purple Transition. Connect the Node that " +
                                    "is to be activated after the time has passed with the white Transition.",
                MessageType.Info);
            EditorGUILayout.EndVertical(); 
            
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(MinTime);
            EditorGUILayout.PropertyField(MaxTime);
            EditorGUILayout.EndVertical(); 
            
            var prevMinTime = MinTime.floatValue;
            var prevMaxTime = MaxTime.floatValue;
				
            if (MinTime.floatValue > prevMaxTime)
                MaxTime.floatValue = MinTime.floatValue;
            if (MaxTime.floatValue < prevMinTime)
                MinTime.floatValue = MaxTime.floatValue;
            if (MinTime.floatValue < 0) MinTime.floatValue = 0;
            if (MaxTime.floatValue < 0) MaxTime.floatValue = 0;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Time Node has been modified");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}