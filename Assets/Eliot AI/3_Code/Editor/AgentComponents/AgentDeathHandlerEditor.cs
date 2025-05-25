#pragma warning disable CS0414, CS0649, CS0612, CS1692
using System;
using Eliot.Utility;
using UnityEditor;

namespace Eliot.AgentComponents.Editor
{
    /// <summary>
    /// Editor script for the Agent Death Handler component.
    /// </summary>
    public class AgentDeathHandlerEditor : AgentComponentEditor
    {
        /// <summary>
        /// Serialized Properties to draw and other variables.
        /// </summary>
        #region Variables
        
        private SerializedProperty _dropItems;
        private SerializedProperty _onDeathSounds;
        private SerializedProperty _onRespawn;
        private SerializedProperty _onDeath;
        private SerializedProperty _spawnRagdoll;
        private SerializedProperty _newPositionOnRespawn;
        private SerializedProperty _deathType;
        private AgentDeathHandler.DeathType deathType;

        private SerializedProperty _ragdollPref;
        private SerializedProperty _deathDelay;

        private bool _unfoldEvents = false;

        private AgentDeathHandler _agentDeathHandler;
        #endregion

        public AgentDeathHandlerEditor(AgentDeathHandler deathHandler) : base(deathHandler)
        {
            _agentDeathHandler = deathHandler;
        }

        /// <summary>
        /// Executed on selecting the object.
        /// </summary>
        public override void OnEnable()
        {
            _dropItems = serializedObject.FindProperty("_dropItems");
            _onDeathSounds = serializedObject.FindProperty("_onDeathSounds");
            _onRespawn = serializedObject.FindProperty("onRespawn");
            _onDeath = serializedObject.FindProperty("onDeath");
            _ragdollPref = serializedObject.FindProperty("_ragdollPref");
            _deathDelay = serializedObject.FindProperty("deathDelay");
            _spawnRagdoll = serializedObject.FindProperty("spawnRagdoll");
            _deathType = serializedObject.FindProperty("deathType");
            _newPositionOnRespawn = serializedObject.FindProperty("newPositionOnRespawn");
        }

        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public override void DrawInspector(AgentComponent deathHandler)
        {
            base.DrawInspector(deathHandler);
            
            deathHandler.displayEditor = EliotEditorUtility.DrawAgentComponentHeader<AgentDeathHandler>("Eliot Agent Component: " + "Death Handler", deathHandler.displayEditor, deathHandler,
                EliotGUISkin.AgentComponentGreen);
            if (!deathHandler.displayEditor) return;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_deathDelay);
            EditorGUILayout.PropertyField(_dropItems);
            EditorGUILayout.PropertyField(_newPositionOnRespawn);
            EditorGUILayout.PropertyField(_spawnRagdoll);
            if (_spawnRagdoll.boolValue)
            {
                EditorGUILayout.PropertyField(_ragdollPref);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_deathType);
            deathType = (AgentDeathHandler.DeathType) Enum.ToObject(typeof(AgentDeathHandler.DeathType),
                _deathType.intValue);

            switch (deathType)
            {
                case AgentDeathHandler.DeathType.Destroy:
                {
                    EditorGUILayout.HelpBox("The Agent object will be destroyed and a new one will be instantiated.",
                        MessageType.Info);
                    break;
                }

                case AgentDeathHandler.DeathType.Deactivate:
                {
                    EditorGUILayout.HelpBox(
                        "The Agent object will be set inactive and moved to a new spawning position.",
                        MessageType.Info);
                    break;
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_onDeathSounds, true);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUI.indentLevel++;
            _unfoldEvents = EditorGUILayout.Foldout(_unfoldEvents, "Events");
            EditorGUI.indentLevel--;
            if (_unfoldEvents)
            {
                EditorGUILayout.PropertyField(_onRespawn);
                EditorGUILayout.PropertyField(_onDeath);
            }

            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(deathHandler, "Death Handler change");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}