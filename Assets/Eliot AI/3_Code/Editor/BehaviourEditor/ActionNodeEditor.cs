#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Eliot.AgentComponents;
using Eliot.Utility;
using UnityEditor;
using UnityEngine;

namespace Eliot.BehaviourEditor.Editor
{
	/// <summary>
	/// Editor extension for ActionNode objects.
	/// </summary>
	[CustomEditor(typeof(ActionNode))]
	public class ActionNodeEditor : UnityEditor.Editor
	{
		/// <summary>
		/// Used to identify and access the Node via code.
		/// </summary>
		private SerializedProperty _nodeId;
		
		/// <summary>
		/// Index of the function from the function group that the node holds information about.
		/// </summary>
		private SerializedProperty _funcIndex;

		/// <summary>
		/// The name of the method represented by the Node.
		/// </summary>
		private SerializedProperty _functionName;

		/// <summary>
		/// A list of names of functions in the current function group of the node.
		/// </summary>
		private string[] _options;

		/// <summary>
		/// Holds the information about the action to be executed.
		/// </summary>
		private MethodData _methodData;

		/// <summary>
		/// List of names of possible action groups to pick from. 
		/// </summary>
		private List<string> _actionGroups = new List<string>();

		/// <summary>
		/// List of possible action groups to pick from. 
		/// </summary>
		private List<Type> _actionGroupsT = new List<Type>();

		/// <summary>
		/// Index of currently captured action group.
		/// </summary>
		private SerializedProperty _actionGroupIndex;

		/// <summary>
		/// Method Info that captures the method to be executed.
		/// </summary>
		private MethodInfo _methodInfo = EliotReflectionUtility.FirstMethod<ActionInterface>();

		/// <summary>
		/// Whether the node captures control.
		/// </summary>
		private SerializedProperty _captureControl;

		/// <summary>
		/// This function is called when the object is loaded.
		/// </summary>
		private void OnEnable()
		{
			_nodeId = serializedObject.FindProperty("NodeId");
			
			_captureControl = serializedObject.FindProperty("CaptureControl");

			_actionGroupsT = EliotReflectionUtility.GetDirectExtentions<ActionInterface>();
			foreach (var interf in _actionGroupsT)
			{
				var interfaceName = interf.Name;
				if (Regex.IsMatch(interfaceName, "ActionInterface"))
					_actionGroups.Add(interfaceName.Replace("ActionInterface", ""));
				else
					_actionGroups.Add(interfaceName);
			}

			if (_methodData == null)
			{
				if ((target as ActionNode).MethodData.IsEmpty())
				{
					(target as ActionNode).MethodData.GetMethodData(_methodInfo);
					_methodData = (target as ActionNode).MethodData;
				}
				else
				{
					_methodData = (target as ActionNode).MethodData;
					if (_methodData != null)
					{
						_methodInfo = _methodData.BuildMethodInfo();
					}
				}
			}
			else
			{
				_methodInfo = _methodData.BuildMethodInfo();
			}

			hideFlags = HideFlags.DontSave;
			_funcIndex = serializedObject.FindProperty("FuncIndex");
			_actionGroupIndex = serializedObject.FindProperty("ActionGroupIndex");

			_functionName = serializedObject.FindProperty("_functionName");


			var indexCalculationResult = EliotReflectionUtility.GetIndexFromMethodInfo<ActionInterface>(_methodInfo);
			if (indexCalculationResult[0] != -1)
			{
				_actionGroupIndex.intValue = indexCalculationResult[0];
				if (indexCalculationResult[1] != -1)
					_funcIndex.intValue = indexCalculationResult[1];
			}

			serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// Draw the new inspector properties.
		/// </summary>
		public override void OnInspectorGUI()
		{
			EliotEditorUtility.DrawMiniLabel("Node: Action", EliotGUISkin.GrayBackground);

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUILayout.PropertyField(_nodeId, new GUIContent("Node Id", "Used to identify and access the Node via code"));
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUILayout.PropertyField(_captureControl);
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			_actionGroupIndex.intValue =
				EditorGUILayout.Popup("Action Group", _actionGroupIndex.intValue, _actionGroups.ToArray());
			var selectedType = _actionGroupsT[_actionGroupIndex.intValue];
			_options = EliotReflectionUtility.GetFunctions(selectedType);
			(serializedObject.targetObject as ActionNode).FuncNames = _options;

			EditorGUIUtility.fieldWidth = 150;
			if (_funcIndex.intValue < _options.Length)
				_funcIndex.intValue = EditorGUILayout.Popup("Function", _funcIndex.intValue, _options);
			else
				_funcIndex.intValue = 0;
			EditorGUILayout.EndVertical();

			_functionName.stringValue = _options[_funcIndex.intValue];
			_methodInfo = EliotReflectionUtility.GetMethodInfoFromMethodName(selectedType, _functionName.stringValue);
			(serializedObject.targetObject as ActionNode).MethodData.GetMethodData(_methodInfo);
			(serializedObject.targetObject as ActionNode).MethodData.DrawInspector();

			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EliotEditorUtility.NavigateToCodeButton(_methodInfo);
			EditorGUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Action Node has been modified");
				serializedObject.ApplyModifiedProperties();
			}

			(target as ActionNode).MethodData = _methodData;
		}

		/// <summary>
		/// This function is called when the scriptable object goes out of scope.
		/// </summary>
		private void OnDisable()
		{
			(target as ActionNode).Window.Save(null);
		}
	}
}
#endif