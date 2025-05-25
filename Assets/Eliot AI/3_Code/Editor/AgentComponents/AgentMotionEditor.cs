using Eliot.Utility;
using UnityEditor;

namespace Eliot.AgentComponents.Editor
{
    /// <summary>
    /// Editor script for the Agent Animation component.
    /// </summary>
    public class AgentMotionEditor : AgentComponentEditor
    {
        /// <summary>
        /// Serialized Properties to draw and other variables.
        /// </summary>
        #region Variables
        private AgentMotion _agentMotion;

        private SerializedProperty _type;

        #region Turret

        private SerializedProperty _head;
        private SerializedProperty _idleRotationSpeed;
        private SerializedProperty _clampIdleRotation;
        private SerializedProperty _clampedIdleRotStart;
        private SerializedProperty _clampedIdleRotEnd;

        #endregion

        #region NavMesh

        private SerializedProperty _weight;
        private SerializedProperty _walkSpeed;
        private SerializedProperty _runSpeed;
        private SerializedProperty _walkCost;
        private SerializedProperty _canRun;
        private SerializedProperty _runCost;
        private SerializedProperty _rotationSpeed;
        private SerializedProperty _canDodge;
        private SerializedProperty _dodgeSpeed;
        private SerializedProperty _dodgeDelay;
        private SerializedProperty _dodgeCoolDown;
        private SerializedProperty _dodgeDuration;
        private SerializedProperty _patrolTime;
        private SerializedProperty _walkingStepSounds;
        private SerializedProperty _walkingStepPing;
        private SerializedProperty _runningStepSounds;
        private SerializedProperty _runningStepPing;
        private SerializedProperty _dodgeSounds;

        #endregion
        #endregion

        public AgentMotionEditor(AgentMotion agentMotion) : base(agentMotion)
        {
            _agentMotion = agentMotion;
        }
        /// <summary>
        /// Executed on selecting the object.
        /// </summary>
        public override void OnEnable()
        {
            _type = serializedObject.FindProperty("_type");

            #region Turret

            _head = serializedObject.FindProperty("_head");
            _idleRotationSpeed = serializedObject.FindProperty("_idleRotationSpeed");
            _clampIdleRotation = serializedObject.FindProperty("_clampIdleRotation");
            _clampedIdleRotStart = serializedObject.FindProperty("_clampedIdleRotStart");
            _clampedIdleRotEnd = serializedObject.FindProperty("_clampedIdleRotEnd");

            #endregion

            #region NavMesh

            _weight = serializedObject.FindProperty("_weight");
            _walkSpeed = serializedObject.FindProperty("_walkSpeed");
            _runSpeed = serializedObject.FindProperty("_runSpeed");
            _walkCost = serializedObject.FindProperty("_walkCost");
            _canRun = serializedObject.FindProperty("_canRun");
            _runCost = serializedObject.FindProperty("_runCost");
            _rotationSpeed = serializedObject.FindProperty("_rotationSpeed");
            _canDodge = serializedObject.FindProperty("_canDodge");
            _dodgeSpeed = serializedObject.FindProperty("_dodgeSpeed");
            _dodgeDelay = serializedObject.FindProperty("_dodgeDelay");
            _dodgeCoolDown = serializedObject.FindProperty("_dodgeCoolDown");
            _dodgeDuration = serializedObject.FindProperty("_dodgeDuration");
            _patrolTime = serializedObject.FindProperty("_patrolTime");
            _walkingStepSounds = serializedObject.FindProperty("_walkingStepSounds");
            _walkingStepPing = serializedObject.FindProperty("_walkingStepPing");
            _runningStepSounds = serializedObject.FindProperty("_runningStepSounds");
            _runningStepPing = serializedObject.FindProperty("_runningStepPing");
            _dodgeSounds = serializedObject.FindProperty("_dodgeSounds");

            #endregion
        }

        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public override void DrawInspector(AgentComponent agentMotion)
        {
            base.DrawInspector(agentMotion);
            
            agentMotion.displayEditor = EliotEditorUtility.DrawAgentComponentHeader<AgentMotion>("Eliot Agent Component: " + "Motion", agentMotion.displayEditor, agentMotion,
                EliotGUISkin.AgentComponentGreen);
            if (!agentMotion.displayEditor) return;
            
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_type);
            EditorGUILayout.EndVertical();

            if (_agentMotion.Type == MotionEngine.Turret)
            {
                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                EditorGUILayout.PropertyField(_head);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                EditorGUILayout.PropertyField(_idleRotationSpeed);
                EditorGUILayout.PropertyField(_clampIdleRotation);
                EditorGUILayout.PropertyField(_clampedIdleRotStart);
                EditorGUILayout.PropertyField(_clampedIdleRotEnd);
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                EditorGUILayout.PropertyField(_weight);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                EditorGUILayout.PropertyField(_walkSpeed);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_walkCost, true);
                EditorGUILayout.PropertyField(_walkingStepSounds, true);
                EditorGUI.indentLevel--;
                EditorGUILayout.PropertyField(_walkingStepPing, true);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                EditorGUILayout.PropertyField(_canRun);
                if (_agentMotion.CanRun)
                {
                    EditorGUILayout.PropertyField(_runSpeed);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_runCost, true);
                    EditorGUILayout.PropertyField(_runningStepSounds, true);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.PropertyField(_runningStepPing, true);
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                EditorGUILayout.PropertyField(_canDodge);
                if (_agentMotion.CanDodge)
                {
                    EditorGUILayout.PropertyField(_dodgeSpeed);
                    EditorGUILayout.PropertyField(_dodgeDelay);
                    EditorGUILayout.PropertyField(_dodgeCoolDown);
                    EditorGUILayout.PropertyField(_dodgeDuration);
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_dodgeSounds, true);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                EditorGUILayout.PropertyField(_rotationSpeed);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                EditorGUILayout.PropertyField(_patrolTime);
                EditorGUILayout.EndVertical();
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(agentMotion, "Motion change");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}