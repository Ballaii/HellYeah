using Eliot.Utility;
using UnityEditor;
using UnityEngine;

namespace Eliot.AgentComponents.Editor
{
    /// <summary>
    /// Editor script for the Agent Player Input component.
    /// </summary>
    public class AgentPlayerInputEditor : AgentComponentEditor
    {
        /// <summary>
        /// Serialized Properties to draw and other variables.
        /// </summary>
        #region Variables
        
        private SerializedProperty _forceExecution;
        
        private SerializedProperty _agentCamera;
        private SerializedProperty _motionInputType;
        private SerializedProperty _motionAccelerationKey;
        private SerializedProperty _motionInputSpace;
        private SerializedProperty _cursorProjectionMotionKey;
        
        private SerializedProperty _rotationType;

        private SerializedProperty _keySkillBindings;
        private SerializedProperty _keyActionBindings;

        private SerializedProperty _targetIndicatorPrefab;
        private SerializedProperty _useDefaultTargetIndicator;
        private SerializedProperty _spawnIndicatorOnAgents;
        
        private AgentPlayerInput _agentPlayerInput;
        
        #endregion

        public AgentPlayerInputEditor(AgentPlayerInput agentPlayerInput) : base(agentPlayerInput)
        {
            _agentPlayerInput = agentPlayerInput;
        }
        
        /// <summary>
        /// Executed on selecting the object.
        /// </summary>
        public override void OnEnable()
        {
            _forceExecution = serializedObject.FindProperty("forceExecution");
            
            _agentCamera = serializedObject.FindProperty("agentCamera");
            _motionInputType = serializedObject.FindProperty("motionInputType");
            _motionAccelerationKey = serializedObject.FindProperty("motionAccelerationKey");
            _motionInputSpace = serializedObject.FindProperty("motionInputSpace");
            _cursorProjectionMotionKey = serializedObject.FindProperty("cursorProjectionMotionKey");
            
            _rotationType = serializedObject.FindProperty("rotationType");

            _keySkillBindings = serializedObject.FindProperty("keySkillBindings");
            _keyActionBindings = serializedObject.FindProperty("keyActionBindings");
            
            _targetIndicatorPrefab = serializedObject.FindProperty("targetIndicatorPrefab");
            _useDefaultTargetIndicator = serializedObject.FindProperty("useDefaultTargetIndicator");
            _spawnIndicatorOnAgents = serializedObject.FindProperty("spawnIndicatorOnAgents");
        }
        
        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public override void DrawInspector(AgentComponent playerInput)
        {
            base.DrawInspector(playerInput);
            
            playerInput.displayEditor = EliotEditorUtility.DrawAgentComponentHeader<AgentPlayerInput>("Eliot Agent Component: " + "Player Input", playerInput.displayEditor, playerInput,
                EliotGUISkin.AgentComponentGreen);
            if (!playerInput.displayEditor) return;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_forceExecution, new GUIContent("Force Execution", "If true, Agent will execute commands as soon as gets input. Otherwise input will be collected but the execution will be initialized from outside."));
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_motionInputType);
            EditorGUILayout.PropertyField(_motionAccelerationKey);
            if(_agentPlayerInput.motionInputType == AgentPlayerInput.MotionInputType.WASD)
                EditorGUILayout.PropertyField(_motionInputSpace);
            if(_agentPlayerInput.motionInputType == AgentPlayerInput.MotionInputType.CursorProjection)
                EditorGUILayout.PropertyField(_cursorProjectionMotionKey);
            EditorGUILayout.PropertyField(_rotationType);
            EditorGUILayout.EndVertical();
            
            if (_agentPlayerInput.motionInputType == AgentPlayerInput.MotionInputType.CursorProjection)
            {
                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                EditorGUILayout.PropertyField(_targetIndicatorPrefab);
                EditorGUILayout.PropertyField(_useDefaultTargetIndicator);
                EditorGUILayout.PropertyField(_spawnIndicatorOnAgents);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_agentCamera);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUI.indentLevel++;
            try
            {
                EditorGUILayout.PropertyField(_keySkillBindings, true);
            }catch(ExitGUIException){}

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_keyActionBindings, true);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(playerInput, "Player Input change");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}