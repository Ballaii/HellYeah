#pragma warning disable CS0414, CS0649, CS0612, CS1692
using UnityEditor;
using UnityEngine;

namespace Eliot.Utility.Editor
{
    /// <summary>
    /// Editor script for EliotCameraController.
    /// </summary>
    [CustomEditor(typeof(EliotCameraController))]
    [CanEditMultipleObjects]
    public class EliotCameraControllerEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Serialized Properties to draw and other variables.
        /// </summary>

        #region Variables

        private EliotCameraController _cameraController;

        private SerializedProperty _target;
        
        private SerializedProperty _pivot;
        private SerializedProperty _autoPivot;
        
        private SerializedProperty _pivotOffset;
        private SerializedProperty _cameraOffset;
        
        private SerializedProperty _initializeFromCamera;

        private SerializedProperty _smoothFollow;
        private SerializedProperty _followSpeed;
        
        private SerializedProperty _distance;
        private SerializedProperty _canZoom;
        private SerializedProperty _minDistance;
        private SerializedProperty _maxDistance;
        private SerializedProperty _zoomSpeed;

        private SerializedProperty _horizontalSpeed;
        private SerializedProperty _verticalSpeed;
        #endregion

        /// <summary>
        /// Executed on selecting the object.
        /// </summary>
        private void OnEnable()
        {
            _cameraController = target as EliotCameraController;

            _target = serializedObject.FindProperty("target");
            
            _pivot = serializedObject.FindProperty("pivot");
            _autoPivot = serializedObject.FindProperty("autoPivot");
            
            _pivotOffset = serializedObject.FindProperty("pivotOffset");
            _cameraOffset = serializedObject.FindProperty("cameraOffset");
            
            _initializeFromCamera = serializedObject.FindProperty("initializeFromCamera");

            _smoothFollow = serializedObject.FindProperty("smoothFollow");
            _followSpeed = serializedObject.FindProperty("followSpeed");
            
            _distance = serializedObject.FindProperty("distance");
            _canZoom = serializedObject.FindProperty("canZoom");
            _minDistance = serializedObject.FindProperty("minDistance");
            _maxDistance = serializedObject.FindProperty("maxDistance");
            _zoomSpeed = serializedObject.FindProperty("zoomSpeed");

            _horizontalSpeed = serializedObject.FindProperty("horizontalSpeed");
            _verticalSpeed = serializedObject.FindProperty("verticalSpeed");
        }
        
        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EliotEditorUtility.DrawHeader<EliotCameraController>("Camera Controller", target, EliotGUISkin.GrayBackground);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_target);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_autoPivot);
            if(!_autoPivot.boolValue)
                EditorGUILayout.PropertyField(_pivot);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_pivotOffset);
            EditorGUILayout.PropertyField(_cameraOffset);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_initializeFromCamera, new GUIContent("Init From Camera", "Read distance and rotation from the camera position as it is in Editor."));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_smoothFollow);
            if(_smoothFollow.boolValue)
                EditorGUILayout.PropertyField(_followSpeed);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_distance);
            EditorGUILayout.PropertyField(_canZoom);
            if (_canZoom.boolValue)
            {
                EditorGUILayout.PropertyField(_minDistance);
                EditorGUILayout.PropertyField(_maxDistance);
                EditorGUILayout.PropertyField(_zoomSpeed);
            }
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_horizontalSpeed);
            EditorGUILayout.PropertyField(_verticalSpeed);
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(target, "Camera Controller change");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}

