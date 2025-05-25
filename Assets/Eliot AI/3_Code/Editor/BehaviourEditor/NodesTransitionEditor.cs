using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Eliot.AgentComponents;
using Eliot.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Eliot.BehaviourEditor.Editor
{
    /// <summary>
    /// Editor script for nodes transitions.
    /// </summary>
    [CustomEditor(typeof(NodesTransition))]
    public class NodesTransitionEditor : UnityEditor.Editor
    {
        /// A list of names of functions in the current function group of the node.
        private string[] _options;

        /// <summary>
        /// List of names of possible condition groups to pick from. 
        /// </summary>
        private List<string> _conditionGroups = new List<string>();

        /// <summary>
        ///  List of possible action groups to pick from.
        /// </summary>
        private List<Type> _conditionGroupsT = new List<Type>();

        /// <summary>
        /// Method Info that captures the method to be executed.
        /// </summary>
        private MethodInfo _methodInfo = EliotReflectionUtility.FirstMethod<ConditionInterface>();

        /// <summary>
        /// Holds the information about the action to be executed.
        /// </summary>
        private MethodData _methodData;

        /// <summary>
        /// List of transitions.
        /// </summary>
        private ReorderableList _reorderableListTransitions;

        /// <summary>
        /// Currently selected transition do draw.
        /// </summary>
        NodesTransitionData _curTransition;

        /// <summary>
        /// Currently selected transition's index.
        /// </summary>
        private int _curTransitionIndex = 0;

        /// <summary>
        /// Whether to display advances settings related to probability.
        /// </summary>
        private bool _displayAdvancedProbabilitySettings;

        /// <summary>
        /// Capture a function.
        /// </summary>
        /// <param name="transition"></param>
        void RefreshSerializedProperties(NodesTransitionData transition)
        {
            _curTransition = transition;
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
                if (transition.MethodData == null)
                {
                    _methodData.GetMethodData(_methodInfo);
                }
                else
                    _methodData = transition.MethodData;
            }
        }

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        void OnEnable()
        {
            if ((target as NodesTransition).TransitionsData.Count == 0)
            {
                (target as NodesTransition).TransitionsData.Add(new NodesTransitionData((target as NodesTransition)));
                _curTransition = (target as NodesTransition).TransitionsData[0];
                RefreshSerializedProperties(_curTransition);
            }
            else
            {
                _curTransition = (target as NodesTransition).TransitionsData[0];
                RefreshSerializedProperties(_curTransition);
            }

            _reorderableListTransitions =
                new ReorderableList((target as NodesTransition).TransitionsData, typeof(NodesTransitionData));
            _reorderableListTransitions.onSelectCallback = (list) =>
            {
                _curTransitionIndex = list.index;
                var obj = (NodesTransitionData) list.list[_curTransitionIndex];
                RefreshSerializedProperties(obj);
            };
            _reorderableListTransitions.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                EditorGUI.LabelField(rect, "Transition " + index);
            };
            _reorderableListTransitions.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "Transitions"); };

            _reorderableListTransitions.onAddCallback = list =>
            {
                var newTransitionData = new NodesTransitionData(target as NodesTransition);
                list.list.Add(newTransitionData);
            };
        }

        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EliotEditorUtility.DrawMiniLabel("Transition", EliotGUISkin.GrayBackground);

            EditorGUI.BeginChangeCheck();
            serializedObject.Update();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            _reorderableListTransitions.DoLayoutList();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.LabelField("Transition " + _curTransitionIndex, EditorStyles.whiteMiniLabel);
            _curTransition.TransitionId = EditorGUILayout.TextField(new GUIContent("Transition Id", "Used to identify and access the Transition via code"), _curTransition.TransitionId);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            _curTransition.UseCondition = EditorGUILayout.Toggle("UseCondition", _curTransition.UseCondition);
            EditorGUILayout.EndVertical();
            if (_curTransition.UseCondition)
            {
                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                _curTransition.Reverse = EditorGUILayout.Toggle("Reverse", _curTransition.Reverse);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                _curTransition.ConditionGroupIndex = EditorGUILayout.Popup("Condition Group",
                    _curTransition.ConditionGroupIndex,
                    _conditionGroups.ToArray());
                var selectedType = _conditionGroupsT[_curTransition.ConditionGroupIndex];
                _options = EliotReflectionUtility.GetFunctions(selectedType);

                EditorGUIUtility.fieldWidth = 150;
                _curTransition.FuncIndex = EditorGUILayout.Popup("Function", _curTransition.FuncIndex, _options);
                _curTransition.FunctionName = _options[_curTransition.FuncIndex];
                EditorGUILayout.EndVertical();

                _methodInfo =
                    EliotReflectionUtility.GetMethodInfoFromMethodName(selectedType, _curTransition.FunctionName);
                _curTransition.MethodData.GetMethodData(_methodInfo);
                _curTransition.MethodData.DrawInspector();

                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                EliotEditorUtility.NavigateToCodeButton(_methodInfo);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            _curTransition.CaptureControl = EditorGUILayout.Toggle(new GUIContent("Capture Control", "If true, the origin node of the transition will be executed every frame instead of Entry until BREAK is called."), _curTransition.CaptureControl);
            if (!_curTransition.CaptureControl && _curTransition.UseCondition)
            {
                _curTransition.CaptureControlOnTrue =
                    EditorGUILayout.Toggle("Capture Control On True", _curTransition.CaptureControlOnTrue);
                _curTransition.CaptureControlOnFalse = EditorGUILayout.Toggle("Capture Control On False",
                    _curTransition.CaptureControlOnFalse);

                if (_curTransition.CaptureControlOnTrue && _curTransition.CaptureControlOnFalse)
                {
                    _curTransition.CaptureControlOnTrue = false;
                    _curTransition.CaptureControlOnFalse = false;
                    _curTransition.CaptureControl = true;
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.LabelField("Probability", EditorStyles.boldLabel);
            EditorGUILayout.MinMaxSlider(ref _curTransition.MinProbability, ref _curTransition.MaxProbability, 0f,
                100f);
            if (_curTransition.MinProbability == 100f && _curTransition.MaxProbability == 100f)
                EditorGUILayout.LabelField("Probability of activating is 100%");
            else
                EditorGUILayout.LabelField("Probability of activating is between " +
                                           (int) _curTransition.MinProbability + "% and " +
                                           (int) _curTransition.MaxProbability + "%");
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUI.indentLevel++;
            _displayAdvancedProbabilitySettings =
                EditorGUILayout.Foldout(_displayAdvancedProbabilitySettings, "Advanced");
            if (_displayAdvancedProbabilitySettings)
            {
                var prevMinTime = _curTransition.MinProbability;
                var prevMaxTime = _curTransition.MaxProbability;

                _curTransition.MinProbability =
                    EditorGUILayout.FloatField("Min Probability", _curTransition.MinProbability);
                _curTransition.MaxProbability =
                    EditorGUILayout.FloatField("Max Probability", _curTransition.MaxProbability);

                if (_curTransition.MinProbability > prevMaxTime)
                    _curTransition.MaxProbability = _curTransition.MinProbability;
                if (_curTransition.MaxProbability < prevMinTime)
                    _curTransition.MinProbability = _curTransition.MaxProbability;
                if (_curTransition.MinProbability < 0) _curTransition.MinProbability = 0;
                if (_curTransition.MaxProbability < 0) _curTransition.MaxProbability = 0;
                if (_curTransition.MinProbability > 100) _curTransition.MinProbability = 100;
                if (_curTransition.MaxProbability > 100) _curTransition.MaxProbability = 100;
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.LabelField("Cooldown", EditorStyles.boldLabel);
            _curTransition.MinCooldown = EditorGUILayout.FloatField("Min Cooldown", _curTransition.MinCooldown);
            _curTransition.MaxCooldown = EditorGUILayout.FloatField("Max Cooldown", _curTransition.MaxCooldown);
            EditorGUILayout.EndVertical();

            var prevMinCooldown = _curTransition.MinCooldown;
            var prevMaxCooldown = _curTransition.MaxCooldown;

            if (_curTransition.MinCooldown > prevMaxCooldown)
                _curTransition.MaxCooldown = _curTransition.MinCooldown;
            if (_curTransition.MaxCooldown < prevMinCooldown)
                _curTransition.MinCooldown = _curTransition.MaxCooldown;
            if (_curTransition.MinCooldown < 0) _curTransition.MinCooldown = 0;
            if (_curTransition.MaxCooldown < 0) _curTransition.MaxCooldown = 0;

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            _curTransition.Terminate = EditorGUILayout.Toggle("Terminate", _curTransition.Terminate);
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Transition has been modified");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}