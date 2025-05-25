using Eliot.Utility;
using UnityEditor;
using UnityEngine;

namespace Eliot.AgentComponents.Editor
{
	/// <summary>
	/// Editor script for the Skill scriptable objects.
	/// </summary>
	[CustomEditor(typeof(Skill))]
	public class SkillEditor : UnityEditor.Editor
	{
		/// <summary>
		/// Serialized Properties to draw and other variables.
		/// </summary>
		#region Variables

		private Skill _skill;
		[SerializeField] private SerializedProperty editorAdvancedMode;
		
		[SerializeField] private SerializedProperty _interruptible;
		[SerializeField] private SerializedProperty _canStack;
		[SerializeField] private SerializedProperty _canAffectEnemies;
		[SerializeField] private SerializedProperty _canAffectFriends;
		[SerializeField] private SerializedProperty _adjustOriginRotation;
		[SerializeField] private SerializedProperty _initSkillBy;

		[SerializeField] private SerializedProperty _projectilePrefab;

		[SerializeField] private SerializedProperty _range;
		[SerializeField] private SerializedProperty _fieldOfView;

		[SerializeField] private SerializedProperty _loadTime;
		[SerializeField] private SerializedProperty _invocationDuration;
		[SerializeField] private SerializedProperty _invocationPing;
		[SerializeField] private SerializedProperty _effectDuration;
		[SerializeField] private SerializedProperty _effectPing;
		[SerializeField] private SerializedProperty _coolDown;

		[SerializeField] private SerializedProperty _minPower;
		[SerializeField] private SerializedProperty _maxPower;
		[SerializeField] private SerializedProperty _pushPower;
		[SerializeField] private SerializedProperty _interruptTarget;
		[SerializeField] private SerializedProperty _freezeMotion;
		[SerializeField] private SerializedProperty _freezeMotionOnCooldown;
		[SerializeField] private SerializedProperty _lookAtTarget;
		[SerializeField] private SerializedProperty _resourcesCost;

		[SerializeField] private SerializedProperty _resourcesActions;

		[SerializeField] private SerializedProperty _setStatus;

		[SerializeField] private SerializedProperty _status;
		[SerializeField] private SerializedProperty _statusDuration;

		[SerializeField] private SerializedProperty _setPositionAsTarget;

		[Space] [SerializeField] private SerializedProperty _makeNoise;
		[SerializeField] private SerializedProperty _noiseDuration;

		[SerializeField] private SerializedProperty _additionalEffects;

		[SerializeField] private SerializedProperty _loadingAnimation;

		[SerializeField] private SerializedProperty _executingAnimation;
		[SerializeField] private SerializedProperty _loadingMessage;
		[SerializeField] private SerializedProperty _executingMessage;

		[SerializeField] private SerializedProperty _onApplyFX;

		[SerializeField] private SerializedProperty _onApplyFXOnTarget;

		[SerializeField] private SerializedProperty _makeChildOfTarget;

		[SerializeField] private SerializedProperty _onApplyMessageToTarget;

		[SerializeField] private SerializedProperty _passPowerToTarget;
		[SerializeField] private SerializedProperty _tryPassDamageToNonAgent;

		[SerializeField] private SerializedProperty _onApplyMessageToCaster;


		[SerializeField] private SerializedProperty _loadingSkillSounds;
		[SerializeField] private SerializedProperty _executingSkillSounds;
		#endregion

		/// <summary>
		/// This function is called when the object is loaded.
		/// </summary>
		public void OnEnable()
		{
			_skill = target as Skill;
			
			editorAdvancedMode = serializedObject.FindProperty("editorAdvancedMode");

			_interruptible = serializedObject.FindProperty("_interruptible");
			_canStack = serializedObject.FindProperty("_canStack");
			_canAffectEnemies = serializedObject.FindProperty("_canAffectEnemies");
			_canAffectFriends = serializedObject.FindProperty("_canAffectFriends");
			_adjustOriginRotation = serializedObject.FindProperty("_adjustOriginRotation");
			_initSkillBy = serializedObject.FindProperty("_initSkillBy");

			_projectilePrefab = serializedObject.FindProperty("_projectilePrefab");

			_range = serializedObject.FindProperty("_range");
			_fieldOfView = serializedObject.FindProperty("_fieldOfView");

			_loadTime = serializedObject.FindProperty("_loadTime");
			_invocationDuration = serializedObject.FindProperty("_invocationDuration");
			_invocationPing = serializedObject.FindProperty("_invocationPing");
			_effectDuration = serializedObject.FindProperty("_effectDuration");
			_effectPing = serializedObject.FindProperty("_effectPing");
			_coolDown = serializedObject.FindProperty("_coolDown");

			_minPower = serializedObject.FindProperty("_minPower");
			_maxPower = serializedObject.FindProperty("_maxPower");
			_pushPower = serializedObject.FindProperty("_pushPower");
			_interruptTarget = serializedObject.FindProperty("_interruptTarget");
			_freezeMotion = serializedObject.FindProperty("_freezeMotion");
			_freezeMotionOnCooldown = serializedObject.FindProperty("_freezeMotionOnCooldown");
			_lookAtTarget = serializedObject.FindProperty("_lookAtTarget");
			_resourcesCost = serializedObject.FindProperty("resourcesCost");

			_resourcesActions = serializedObject.FindProperty("resourcesActions");
			_setStatus = serializedObject.FindProperty("_setStatus");
			_status = serializedObject.FindProperty("_status");
			_statusDuration = serializedObject.FindProperty("_statusDuration");
			_setPositionAsTarget = serializedObject.FindProperty("_setPositionAsTarget");
			_makeNoise = serializedObject.FindProperty("_makeNoise");
			_noiseDuration = serializedObject.FindProperty("_noiseDuration");
			_additionalEffects = serializedObject.FindProperty("_additionalEffects");

			_loadingAnimation = serializedObject.FindProperty("_loadingAnimation");
			_executingAnimation = serializedObject.FindProperty("_executingAnimation");

			_loadingMessage = serializedObject.FindProperty("_loadingMessage");
			_executingMessage = serializedObject.FindProperty("_executingMessage");

			_onApplyFX = serializedObject.FindProperty("_onApplyFx");
			_onApplyFXOnTarget = serializedObject.FindProperty("_onApplyFxOnTarget");
			_makeChildOfTarget = serializedObject.FindProperty("_makeChildOfTarget");
			_onApplyMessageToTarget = serializedObject.FindProperty("_onApplyMessageToTarget");
			_passPowerToTarget = serializedObject.FindProperty("_passPowerToTarget");
			_tryPassDamageToNonAgent = serializedObject.FindProperty("_tryPassDamageToNonAgent");
			_onApplyMessageToCaster = serializedObject.FindProperty("_onApplyMessageToCaster");

			_loadingSkillSounds = serializedObject.FindProperty("_loadingSkillSounds");
			_executingSkillSounds = serializedObject.FindProperty("_executingSkillSounds");
		}

		/// <summary>
		/// Draw the Inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			EliotEditorUtility.DrawMiniLabel("Eliot Scriptable: Skill", EliotGUISkin.ScriptableOrange);
			EditorGUI.BeginChangeCheck();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			if (GUILayout.Button(editorAdvancedMode.boolValue ? "Turn off Advanced Mode" : "Toggle on Advanced Mode", EditorStyles.toolbarButton))
			{
				editorAdvancedMode.boolValue = !editorAdvancedMode.boolValue;
			}
			
			EditorGUILayout.EndVertical();
			
			if (editorAdvancedMode.boolValue)
			{
				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.PropertyField(_interruptible);
				EditorGUILayout.PropertyField(_canStack);
				EditorGUILayout.PropertyField(_canAffectEnemies);
				EditorGUILayout.PropertyField(_canAffectFriends);
				EditorGUILayout.PropertyField(_adjustOriginRotation);
				EditorGUILayout.PropertyField(_initSkillBy);
				if (_skill._initSkillBy == InitSkillBy.Projectile)
				{
					EditorGUILayout.PropertyField(_projectilePrefab);
				}

				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.PropertyField(_range);
				EditorGUILayout.PropertyField(_fieldOfView);
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.PropertyField(_loadTime);
				EditorGUILayout.PropertyField(_invocationDuration);
				EditorGUILayout.PropertyField(_invocationPing);
				EditorGUILayout.PropertyField(_effectDuration);
				EditorGUILayout.PropertyField(_effectPing);
				EditorGUILayout.PropertyField(_coolDown);
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.LabelField("Economy", EditorStyles.boldLabel);
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(_resourcesActions, true);
				EditorGUILayout.PropertyField(_resourcesCost, true);
				EditorGUI.indentLevel--;
				EditorGUILayout.PropertyField(_pushPower);
				EditorGUILayout.PropertyField(_interruptTarget);
				EditorGUILayout.PropertyField(_freezeMotion);
				EditorGUILayout.PropertyField(_freezeMotionOnCooldown);
				EditorGUILayout.PropertyField(_lookAtTarget);
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.PropertyField(_setStatus);
				EditorGUILayout.PropertyField(_status);
				EditorGUILayout.PropertyField(_statusDuration);
				EditorGUILayout.PropertyField(_setPositionAsTarget);
				EditorGUILayout.PropertyField(_makeNoise);
				EditorGUILayout.PropertyField(_noiseDuration);
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(_additionalEffects, true);
				EditorGUI.indentLevel--;
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.PropertyField(_loadingAnimation);
				EditorGUILayout.PropertyField(_executingAnimation);
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.PropertyField(_loadingMessage);
				EditorGUILayout.PropertyField(_executingMessage);
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.PropertyField(_onApplyFX);
				EditorGUILayout.PropertyField(_onApplyFXOnTarget);
				EditorGUILayout.PropertyField(_makeChildOfTarget);
				EditorGUILayout.PropertyField(_onApplyMessageToTarget);
				EditorGUILayout.PropertyField(_passPowerToTarget);
				if (_passPowerToTarget.boolValue)
				{
					EditorGUILayout.PropertyField(_minPower);
					EditorGUILayout.PropertyField(_maxPower);
				}
				EditorGUILayout.PropertyField(_tryPassDamageToNonAgent);
				EditorGUILayout.PropertyField(_onApplyMessageToCaster);
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(_loadingSkillSounds, true);
				EditorGUILayout.PropertyField(_executingSkillSounds, true);
				EditorGUI.indentLevel--;
				EditorGUILayout.EndVertical();
			}
			else
			{
				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.PropertyField(_initSkillBy);
				if (_skill._initSkillBy == InitSkillBy.Projectile)
				{
					EditorGUILayout.PropertyField(_projectilePrefab);
				}

				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.PropertyField(_range);
				EditorGUILayout.EndVertical();
				
				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.PropertyField(_loadTime);
				EditorGUILayout.PropertyField(_invocationDuration);
				EditorGUILayout.PropertyField(_invocationPing);
				EditorGUILayout.PropertyField(_effectDuration);
				EditorGUILayout.PropertyField(_effectPing);
				EditorGUILayout.PropertyField(_coolDown);
				EditorGUILayout.EndVertical();
				
				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.LabelField("Economy", EditorStyles.boldLabel);
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(_resourcesActions, true);
				EditorGUILayout.PropertyField(_resourcesCost, true);
				EditorGUI.indentLevel--;
				EditorGUILayout.EndVertical();
				
				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.PropertyField(_loadingMessage);
				EditorGUILayout.PropertyField(_executingMessage);
				EditorGUILayout.EndVertical();
				
				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.PropertyField(_onApplyFX);
				EditorGUILayout.PropertyField(_onApplyFXOnTarget);
				EditorGUILayout.EndVertical();
			}

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RegisterCompleteObjectUndo(target, "Skill change");
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}