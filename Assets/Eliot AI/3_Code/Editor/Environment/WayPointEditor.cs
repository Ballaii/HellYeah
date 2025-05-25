using Eliot.Utility;
using UnityEditor;

namespace Eliot.Environment.Editor
{
    /// <summary>
    /// Editor script for Eliot Way Point.
    /// </summary>
    [CustomEditor(typeof(EliotWayPoint))]
    [CanEditMultipleObjects]
    public class WayPointEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Serialized Properties to draw and other variables.
        /// </summary>
        #region Variables
        private EliotWayPoint _wayPoint;

        private SerializedProperty _wayPointsGroup;

        private SerializedProperty _applyChangesToAgent;
        private SerializedProperty _newBehaviour;
        private SerializedProperty _newWayPointsGroup;

        private SerializedProperty _overrideActionDistance;
        private SerializedProperty _actionDistance;

        private SerializedProperty _overrideColor;
        private SerializedProperty _color;

        private SerializedProperty _addEffect;
        private SerializedProperty _executeSkill;

        private SerializedProperty _onAgentEnter;
        private SerializedProperty _actionDelay;

        private SerializedProperty _actionTimeMin;
        private SerializedProperty _actionTimeMax;

        private SerializedProperty _onAgentAction;
        private SerializedProperty _onAgentExit;

        private SerializedProperty _overrideNextWayPoints;
        private SerializedProperty _nextWayPoints;

        private bool _unfoldEvents = false;
        #endregion

        /// <summary>
        /// Execute this function when the object is loaded.
        /// </summary>
        private void OnEnable()
        {
            _wayPoint = target as EliotWayPoint;

            _wayPointsGroup = serializedObject.FindProperty("_wayPointsGroup");

            _applyChangesToAgent = serializedObject.FindProperty("_applyChangesToAgent");
            _overrideActionDistance = serializedObject.FindProperty("overrideActionDistance");
            _actionDistance = serializedObject.FindProperty("actionDistance");

            _newBehaviour = serializedObject.FindProperty("_newBehaviour");
            _newWayPointsGroup = serializedObject.FindProperty("_newWayPointsGroup");

            _overrideColor = serializedObject.FindProperty("overrideColor");
            _color = serializedObject.FindProperty("color");

            _addEffect = serializedObject.FindProperty("addEffect");
            _executeSkill = serializedObject.FindProperty("executeSkill");

            _onAgentEnter = serializedObject.FindProperty("onAgentEnter");
            _actionDelay = serializedObject.FindProperty("actionDelay");
            _actionTimeMin = serializedObject.FindProperty("actionTimeMin");
            _actionTimeMax = serializedObject.FindProperty("actionTimeMax");
            _onAgentAction = serializedObject.FindProperty("onAgentAction");
            _onAgentExit = serializedObject.FindProperty("onAgentExit");

            _overrideNextWayPoints = serializedObject.FindProperty("overrideNextWayPoints");
            _nextWayPoints = serializedObject.FindProperty("nextWayPoints");
        }

        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EliotEditorUtility.DrawHeader<EliotWayPoint>("Way Point", target, EliotGUISkin.GrayBackground);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_wayPointsGroup);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_overrideActionDistance);
            if (_overrideActionDistance.boolValue)
                EditorGUILayout.PropertyField(_actionDistance);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_overrideColor);
            if (_overrideColor.boolValue)
                EditorGUILayout.PropertyField(_color);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_applyChangesToAgent);
            if (_applyChangesToAgent.boolValue)
            {
                EditorGUILayout.PropertyField(_newBehaviour);
                EditorGUILayout.PropertyField(_newWayPointsGroup);
            }

            EditorGUILayout.EndVertical();

            if (_applyChangesToAgent.boolValue)
            {
                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                EditorGUILayout.PropertyField(_addEffect);
                EditorGUILayout.PropertyField(_executeSkill);
                EditorGUILayout.EndVertical();
            }

            if (_applyChangesToAgent.boolValue)
            {
                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                EditorGUI.indentLevel++;
                _unfoldEvents = EditorGUILayout.Foldout(_unfoldEvents, "Events");
                EditorGUI.indentLevel--;
                if (_unfoldEvents)
                {
                    EditorGUILayout.PropertyField(_onAgentEnter);
                    EditorGUILayout.PropertyField(_actionDelay);
                    
                    EditorGUILayout.MinMaxSlider("Action Time", ref _wayPoint.actionTimeMin,
                        ref _wayPoint.actionTimeMax,
                        0f,
                        EliotWayPoint.MaxActionTime);
                    
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(_actionTimeMin);
                    EditorGUILayout.PropertyField(_actionTimeMax);
                    EditorGUI.indentLevel--;

                    EditorGUILayout.PropertyField(_onAgentAction);
                    EditorGUILayout.PropertyField(_onAgentExit);
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_overrideNextWayPoints);
            if (_overrideNextWayPoints.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_nextWayPoints, true);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(target, "Way Point change");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}