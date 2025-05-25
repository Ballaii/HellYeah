#pragma warning disable CS0414, CS0649, CS0612, CS1692
using Eliot.Utility;
using UnityEditor;

namespace Eliot.AgentComponents.Editor
{
    /// <summary>
    /// Editor script for Projectiles.
    /// </summary>
    [CustomEditor(typeof(EliotProjectile))]
    [CanEditMultipleObjects]
    public class ProjectileEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Serialized Properties to draw and other variables.
        /// </summary>
        #region Variables
        private EliotProjectile _projectile;

        private SerializedProperty _canAffectEnemies;
        private SerializedProperty _canAffectFriends;

        private SerializedProperty _minDamage;
        private SerializedProperty _maxDamage;

        private SerializedProperty _speed;
        private SerializedProperty _rotationSpeed;

        private SerializedProperty _lifeTime;
        private SerializedProperty _chaseTarget;
        private SerializedProperty _detachChildren;
        private SerializedProperty _destroyOnAnyCollision;

        private SerializedProperty _sendDamageMessage;
        private SerializedProperty _damageMethodName;
        private SerializedProperty _messages;

        private SerializedProperty _skill;
        #endregion

        /// <summary>
        /// Executed on selecting the object.
        /// </summary>
        private void OnEnable()
        {
            _projectile = target as EliotProjectile;

            _canAffectEnemies = serializedObject.FindProperty("_canAffectEnemies");
            _canAffectFriends = serializedObject.FindProperty("_canAffectFriends");

            _minDamage = serializedObject.FindProperty("_minDamage");
            _maxDamage = serializedObject.FindProperty("_maxDamage");

            _speed = serializedObject.FindProperty("_speed");
            _rotationSpeed = serializedObject.FindProperty("_rotationSpeed");

            _lifeTime = serializedObject.FindProperty("_lifeTime");
            _chaseTarget = serializedObject.FindProperty("_chaseTarget");
            _detachChildren = serializedObject.FindProperty("_detachChildren");
            _destroyOnAnyCollision = serializedObject.FindProperty("_destroyOnAnyCollision");

            _sendDamageMessage = serializedObject.FindProperty("_sendDamageMessage");
            _damageMethodName = serializedObject.FindProperty("_damageMethodName");
            _messages = serializedObject.FindProperty("_messages");

            _skill = serializedObject.FindProperty("_skill");
        }
        
        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EliotEditorUtility.DrawHeader<EliotProjectile>("Projectile", target, EliotGUISkin.GrayBackground);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_skill);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_canAffectEnemies);
            EditorGUILayout.PropertyField(_canAffectFriends);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_minDamage);
            EditorGUILayout.PropertyField(_maxDamage);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_speed);
            EditorGUILayout.PropertyField(_rotationSpeed);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_lifeTime);
            EditorGUILayout.PropertyField(_chaseTarget);
            EditorGUILayout.PropertyField(_detachChildren);
            EditorGUILayout.PropertyField(_destroyOnAnyCollision);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_sendDamageMessage);
            EditorGUILayout.PropertyField(_damageMethodName);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_messages, true);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(target, "Projectile change");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}