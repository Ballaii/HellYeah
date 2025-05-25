using Eliot.Utility;
using UnityEditor;

namespace Eliot.Environment.Editor
{
	/// <summary>
	/// Editor script for Eliot Item.
	/// </summary>
	[CustomEditor(typeof(EliotItem))]
	[CanEditMultipleObjects]
	public class EliotItemEditor : UnityEditor.Editor
	{
		/// <summary>
		/// Serialized Properties to draw and other variables.
		/// </summary>
		#region Variables
		private SerializedProperty _value;
		private SerializedProperty _weight;
		private SerializedProperty _amount;
		private SerializedProperty _itemType;
		private SerializedProperty _skill;

		private SerializedProperty _newBehaviour;
		private SerializedProperty _addSkills;
		private SerializedProperty _newGraphics;

		private SerializedProperty _wieldSounds;
		private SerializedProperty _unwieldSounds;
		private SerializedProperty _useSounds;

		private SerializedProperty _onWield;
		private SerializedProperty _onUnwield;
		private SerializedProperty _onPickUp;
		private SerializedProperty _onDrop;
		private SerializedProperty _onUse;

		private bool _unfoldEvents = false;
		#endregion

		/// <summary>
		/// Execute this function when the object is loaded.
		/// </summary>
		private void OnEnable()
		{
			_value = serializedObject.FindProperty("_value");
			_weight = serializedObject.FindProperty("_weight");
			_amount = serializedObject.FindProperty("_amount");
			_itemType = serializedObject.FindProperty("_itemType");
			_skill = serializedObject.FindProperty("_skill");

			_newBehaviour = serializedObject.FindProperty("_newBehaviour");
			_addSkills = serializedObject.FindProperty("_addSkills");
			_newGraphics = serializedObject.FindProperty("_newGraphics");

			_wieldSounds = serializedObject.FindProperty("_wieldSounds");
			_unwieldSounds = serializedObject.FindProperty("_unwieldSounds");
			_useSounds = serializedObject.FindProperty("_useSounds");

			_onWield = serializedObject.FindProperty("onWield");
			_onUnwield = serializedObject.FindProperty("onUnwield");
			_onPickUp = serializedObject.FindProperty("onPickUp");
			_onDrop = serializedObject.FindProperty("onDrop");
			_onUse = serializedObject.FindProperty("onUse");
		}

		/// <summary>
		/// Draw the Inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			EliotEditorUtility.DrawHeader<EliotItem>("Item", target, EliotGUISkin.GrayBackground);

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUILayout.PropertyField(_value);
			EditorGUILayout.PropertyField(_weight);
			EditorGUILayout.PropertyField(_amount);
			EditorGUILayout.PropertyField(_itemType);
			EditorGUILayout.PropertyField(_skill);
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUILayout.PropertyField(_newBehaviour);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_addSkills, true);
			EditorGUI.indentLevel--;
			EditorGUILayout.PropertyField(_newGraphics);
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_wieldSounds, true);
			EditorGUILayout.PropertyField(_unwieldSounds, true);
			EditorGUILayout.PropertyField(_useSounds, true);
			EditorGUI.indentLevel--;
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUI.indentLevel++;
			_unfoldEvents = EditorGUILayout.Foldout(_unfoldEvents, "Events");
			EditorGUI.indentLevel--;
			if (_unfoldEvents)
			{
				EditorGUILayout.PropertyField(_onWield);
				EditorGUILayout.PropertyField(_onUnwield);
				EditorGUILayout.PropertyField(_onPickUp);
				EditorGUILayout.PropertyField(_onDrop);
				EditorGUILayout.PropertyField(_onUse);
			}

			EditorGUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RegisterCompleteObjectUndo(target, "Item change");
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}