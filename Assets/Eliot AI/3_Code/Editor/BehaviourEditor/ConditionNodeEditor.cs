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
	/// Editor extension for ConditionNode objects.
	/// </summary>
	[CustomEditor(typeof(ConditionNode))]
	public class ConditionNodeEditor : UnityEditor.Editor
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
		/// List of names of possible condition groups to pick from. 
		/// </summary>
		private List<string> _conditionGroups = new List<string>();
		
		/// <summary>
		/// List of possible action groups to pick from. 
		/// </summary>
		private List<Type> _conditionGroupsT = new List<Type>();
		
		/// <summary>
		/// Index of currently captured action group.
		/// </summary>
		private SerializedProperty _conditionGroupIndex;

		/// <summary>
		/// Method Info that captures the method to be executed.
		/// </summary>
		private MethodInfo _methodInfo = EliotReflectionUtility.FirstMethod<ConditionInterface>();
		
		/// <summary>
		/// Holds the information about the action to be executed.
		/// </summary>
		private MethodData _methodData;
		
		/// <summary>
		/// Whether the node captures control.
		/// </summary>
		private SerializedProperty _captureControl;
		
		/// <summary>
		/// Reverse the condition return value.
		/// </summary>
		private SerializedProperty _reverse;
		
		/// <summary>
		/// This function is called when the object is loaded.
		/// </summary>
		private void OnEnable()
		{
			_nodeId = serializedObject.FindProperty("NodeId");
			_reverse = serializedObject.FindProperty("Reverse");
			_captureControl = serializedObject.FindProperty("CaptureControl");
			
			_conditionGroupsT = EliotReflectionUtility.GetDirectExtentions<ConditionInterface>();
			foreach (var interf in _conditionGroupsT)
			{
				var interfaceName = interf.Name;
				if (Regex.IsMatch(interfaceName, "ConditionInterface"))
					_conditionGroups.Add(interfaceName.Replace("ConditionInterface", ""));
				else
					_conditionGroups.Add(interfaceName);
			}
			
			if (_methodData == null)
			{
				if ( (target as ConditionNode).MethodData.IsEmpty() )
				{
					(target as ConditionNode).MethodData.GetMethodData(_methodInfo);
					_methodData = (target as ConditionNode).MethodData;
				}
				else
				{
					_methodData = (target as ConditionNode).MethodData;
					if(_methodData != null)
						_methodInfo = _methodData.BuildMethodInfo();
				}
			}
			else
			{
				_methodInfo = _methodData.BuildMethodInfo();
			}

			hideFlags = HideFlags.DontSave;
			_funcIndex = serializedObject.FindProperty("FuncIndex");
			_conditionGroupIndex = serializedObject.FindProperty("ConditionGroupIndex");
			
			_functionName = serializedObject.FindProperty("_functionName");
			
			var indexCalculationResult = EliotReflectionUtility.GetIndexFromMethodInfo<ConditionInterface>(_methodInfo);
			if (indexCalculationResult[0] != -1)
			{
				_conditionGroupIndex.intValue = indexCalculationResult[0];
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
			EliotEditorUtility.DrawMiniLabel("Node: Condition", EliotGUISkin.GrayBackground);
			
			EditorGUI.BeginChangeCheck();
			
			serializedObject.Update();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUILayout.PropertyField(_nodeId, new GUIContent("Node Id", "Used to identify and access the Node via code"));
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUILayout.PropertyField(_reverse);
			EditorGUILayout.EndVertical(); 
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUILayout.PropertyField(_captureControl);
			EditorGUILayout.EndVertical(); 
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			_conditionGroupIndex.intValue = EditorGUILayout.Popup("Condition Group",
				_conditionGroupIndex.intValue,
				_conditionGroups.ToArray());
			var selectedType = _conditionGroupsT[_conditionGroupIndex.intValue];
			_options = EliotReflectionUtility.GetFunctions(selectedType);
			(serializedObject.targetObject as ConditionNode).FuncNames = _options;
			
			EditorGUIUtility.fieldWidth = 150;
			if(_funcIndex.intValue < _options.Length)
				_funcIndex.intValue = EditorGUILayout.Popup("Function", _funcIndex.intValue, _options);
			else
				_funcIndex.intValue = 0;
			EditorGUILayout.EndVertical(); 
			
			_functionName.stringValue = _options[_funcIndex.intValue];
			_methodInfo = EliotReflectionUtility.GetMethodInfoFromMethodName(selectedType, _functionName.stringValue);
			(serializedObject.targetObject as ConditionNode).MethodData.GetMethodData(_methodInfo);
			
			(serializedObject.targetObject as ConditionNode).MethodData.DrawInspector();

			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EliotEditorUtility.NavigateToCodeButton(_methodInfo);
			EditorGUILayout.EndVertical();
			
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Condition Node has been modified");
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}
#endif