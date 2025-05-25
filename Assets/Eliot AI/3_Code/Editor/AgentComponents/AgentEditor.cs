#if UNITY_EDITOR
#pragma warning disable CS0414, CS0649, CS1692
using System;
using System.Collections.Generic;
using Eliot.BehaviourEditor;
using Eliot.Environment;
using Eliot.Utility;
using UnityEditor;
using UnityEngine;

namespace Eliot.AgentComponents.Editor
{
	/// <summary>
	/// Editor extension for Agent component.
	/// </summary>
	[CustomEditor(typeof(EliotAgent))]
	[CanEditMultipleObjects]
	public class AgentEditor : UnityEditor.Editor
	{
		/// <summary>
		/// Serialized Properties to draw and other variables.
		/// </summary>

		#region Variables

		private EliotAgent _agent;

		private SerializedProperty _editorAdvancedMode;
		private SerializedProperty _agentType;
		private SerializedProperty _aiEnabled;
		private SerializedProperty _ping;
		private SerializedProperty _behaviour;
		private SerializedProperty _waypoints;
		private SerializedProperty _applyChangesOnWayPoints;
		private SerializedProperty _skills;

		private bool _displayAgentComponentExtensionsOptions = false;
		private int _displayAgentComponentExtensionsIndex = 0;
		private string _reorderableListSkillsCurrentIndex = "";

		private bool _displayWaypointsOptions = false;

		private bool _displaySettingsOptions = false;
		private bool _adjustOrigin = false;

		private SerializedObject _agentAnimation;
		private SerializedObject _agentPlayerInput;
		private SerializedObject _agentMotion;

		#endregion


		#region Simple

		private Skill _simpleModeSkill;
		private KeyCode _simpleModeSkillKey;
		private AgentResources _agentResources;

		#endregion

		/// <summary>
		/// This function is called when the object is loaded.
		/// </summary>
		private void OnEnable()
		{
			if (target == null) return;
			_agent = (EliotAgent) target;

			_editorAdvancedMode = serializedObject.FindProperty("editorAdvancedMode");
			_agentType = serializedObject.FindProperty("agentType");
			_aiEnabled = serializedObject.FindProperty("_aiEnabled");
			_ping = serializedObject.FindProperty("_ping");
			_behaviour = serializedObject.FindProperty("_behaviour");
			_waypoints = serializedObject.FindProperty("wayPoints");
			_applyChangesOnWayPoints = serializedObject.FindProperty("_applyChangesOnWayPoints");
			_skills = serializedObject.FindProperty("_skills");

			foreach (var component in _agent.GetComponents<AgentComponent>())
			{
				component.AgentOnEnable(_agent);
				component.hideFlags = HideFlags.HideInInspector;
			}


			if (_agent.Skills.Count > 0)
			{
				_simpleModeSkill = _agent.Skills[0];
			}

			var playerInput = _agent.GetComponent<AgentPlayerInput>();
			if (playerInput)
			{
				if (playerInput.keySkillBindings == null)
				{
					playerInput.keySkillBindings = new List<AgentPlayerInput.KeySkillBinding>();
				}
				if (playerInput.keySkillBindings.Count > 0)
				{
					var firstSkillBinding = playerInput.keySkillBindings[0];
					_simpleModeSkillKey = firstSkillBinding.key;
					if (!firstSkillBinding.skill && _agent.Skills[0])
					{
						firstSkillBinding.skill = _agent.Skills[0];
					}
					if (firstSkillBinding.skill)
						_simpleModeSkill = firstSkillBinding.skill;
				}
			}
		}


		private void OnValidate()
		{
			OnEnable();
		}

		/// <summary>
		/// Draw the new inspector properties.
		/// </summary>
		public override void OnInspectorGUI()
		{
			EliotEditorUtility.DrawHeader<EliotAgent>("Eliot Agent", target);
			EditorGUI.BeginChangeCheck();
			serializedObject.Update();

			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			if (GUILayout.Button(_editorAdvancedMode.boolValue
					? new GUIContent("Turn on Simple Mode",
						"Simplifies the agent setup. All the agent components will be added automatically. Inspector GUI will display only the most important properties.")
					: new GUIContent("Turn on Advanced Mode",
						"Unfold all the Agent properties. Advanced level control over each component."),
				EditorStyles.toolbarButton))
			{
				_editorAdvancedMode.boolValue = !_editorAdvancedMode.boolValue;
			}

			EditorGUILayout.EndVertical();
			if (_editorAdvancedMode.boolValue)
			{
				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				_behaviour.objectReferenceValue = EditorGUILayout.ObjectField("Behaviour",
					_behaviour.objectReferenceValue, typeof(EliotBehaviour), false);
				_aiEnabled.boolValue = EditorGUILayout.Toggle("AI Enabled", _aiEnabled.boolValue);
				_ping.floatValue = EditorGUILayout.FloatField("Ping", _ping.floatValue);
				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);

				#region Waypoints

				EditorGUI.indentLevel++;
				_displayWaypointsOptions = EditorGUILayout.Foldout(_displayWaypointsOptions, "Way Points Settings");
				if (_displayWaypointsOptions)
				{
					EditorGUI.indentLevel++;
					_waypoints.objectReferenceValue = EditorGUILayout.ObjectField("Way Points",
						_waypoints.objectReferenceValue, typeof(WayPointsGroup), true);
					_applyChangesOnWayPoints.boolValue =
						EditorGUILayout.Toggle("Apply Changes", _applyChangesOnWayPoints.boolValue);
					EditorGUI.indentLevel--;
				}

				#endregion

				EditorGUILayout.EndVertical();

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);

				_adjustOrigin = GUILayout.Toggle(_adjustOrigin, "Adjust Skill Origin", EditorStyles.toolbarButton);
				if (_adjustOrigin) Tools.current = Tool.None;
				EditorGUILayout.PropertyField(_skills, true);

				EditorGUILayout.EndVertical();
				EditorGUI.indentLevel--;

				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(target, "Agent has been modified");
					serializedObject.ApplyModifiedProperties();
				}

				#region AgentComponents

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.HelpBox("Agent Components", MessageType.None);
				foreach (var agentComponent in _agent.gameObject.GetComponents<AgentComponent>())
				{
					try
					{
						var editor = AgentComponentsEditorMap.GetEditor(agentComponent);
						editor.DrawInspector(agentComponent);
					}
					catch (Exception e)
					{
						agentComponent.AgentReset();
						Debug.LogError(e, agentComponent);
					}

				}

				if (GUILayout.Button("Add Agent Component", EditorStyles.toolbarButton))
				{
					if (_displayAgentComponentExtensionsOptions)
						_displayAgentComponentExtensionsOptions = false;
					else _displayAgentComponentExtensionsOptions = true;
				}

				if (_displayAgentComponentExtensionsOptions)
				{
					var agentComponentExtensions = EliotReflectionUtility.GetDirectExtentions<AgentComponent>();
					var agentComponentExtensionsNames = new List<string>();
					foreach (var agentExtension in agentComponentExtensions)
						if (_agent.GetComponent(agentExtension) == null)
							agentComponentExtensionsNames.Add(agentExtension.Name);

					EditorGUILayout.BeginVertical(EliotGUISkin.BlueBackground);
					var c = GUI.color;
					GUI.color = new Color(0.85f, 0.85f, 0.85f, 1f);
					foreach (var agentExtension in agentComponentExtensions)
					{
						if (_agent.GetComponent(agentExtension) != null)
						{
							var curColor = GUI.color;
							GUI.color = new Color(0.6f, 0.6f, 0.7f, 1f);
							if (GUILayout.Button(agentExtension.Name, EditorStyles.radioButton))
							{
							}

							GUI.color = curColor;
						}
						else
						{
							if (GUILayout.Button(agentExtension.Name, EditorStyles.radioButton))
							{
								foreach (var selected in Selection.gameObjects)
								{
									var agent = selected.GetComponent<EliotAgent>();
									if (agent)
									{
										selected.AddAgentComponent(agentExtension);
										OnEnable();
									}
								}
							}
						}
					}

					GUI.color = c;
					EditorGUILayout.EndVertical();
				}

				EditorGUILayout.EndVertical();

				#endregion
			}
			else
			{
				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				_aiEnabled.boolValue = EditorGUILayout.Toggle("AI Enabled", _aiEnabled.boolValue);
				_behaviour.objectReferenceValue = EditorGUILayout.ObjectField("Behaviour",
					_behaviour.objectReferenceValue, typeof(EliotBehaviour), false);
				EditorGUILayout.PropertyField(_agentType);
				EditorGUILayout.EndVertical();

				var allAgentComponents = EliotReflectionUtility.GetDirectExtentions<AgentComponent>();
				foreach (var component in allAgentComponents)
				{
					if (!_agent.GetComponent(component) && component != typeof(AgentPlayerInput))
					{
						_agent.gameObject.AddAgentComponent(component);
					}

					if (_agentType.enumValueIndex == (int) EliotAgent.AgentType.Player)
					{
						if (!_agent.GetComponent(typeof(AgentPlayerInput)))
						{
							_agent.gameObject.AddAgentComponent(typeof(AgentPlayerInput));
						}
					}
					else if (_agentType.enumValueIndex == (int) EliotAgent.AgentType.NonPlayerCharacter)
					{
						if (_agent.GetComponent(typeof(AgentPlayerInput)))
						{
							DestroyImmediate(_agent.GetComponent(typeof(AgentPlayerInput)));
						}
					}
				}

				#region Waypoints

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				_waypoints.objectReferenceValue = EditorGUILayout.ObjectField("Way Points",
					_waypoints.objectReferenceValue, typeof(WayPointsGroup), true);
				EditorGUILayout.EndVertical();

				#endregion

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);

				_simpleModeSkill = (Skill) EditorGUILayout.ObjectField("Skill",
					_simpleModeSkill, typeof(Skill), false);

				if (_agentType.enumValueIndex == (int) EliotAgent.AgentType.Player)
				{
					_simpleModeSkillKey = (KeyCode) EditorGUILayout.EnumPopup("Fire Key", _simpleModeSkillKey);
					var keySkillBinding = new AgentPlayerInput.KeySkillBinding();
					keySkillBinding.key = _simpleModeSkillKey;
					keySkillBinding.skill = _simpleModeSkill;
					keySkillBinding.keyEvent = AgentPlayerInput.KeyEvent.KeyDown;
					_agent.GetComponent<AgentPlayerInput>().keySkillBindings =
						new List<AgentPlayerInput.KeySkillBinding> {keySkillBinding};
				}

				EditorGUILayout.EndVertical();
				if (_agent.Skills.Count == 0)
					_agent.Skills = new List<Skill> {_simpleModeSkill};
				else _agent.Skills[0] = _simpleModeSkill;

				#region Player Input and Motion

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.LabelField("Motion", EditorStyles.miniBoldLabel);
				if (_agentMotion == null)
					_agentMotion = new SerializedObject(_agent.GetComponent<AgentMotion>());
				EditorGUILayout.PropertyField(_agentMotion.FindProperty("_walkSpeed"));
				EditorGUILayout.PropertyField(_agentMotion.FindProperty("_runSpeed"));
				EditorGUILayout.EndVertical();
				_agentMotion.ApplyModifiedProperties();

				if (_agentType.enumValueIndex == (int) EliotAgent.AgentType.Player)
				{
					EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
					if (_agentPlayerInput == null)
						_agentPlayerInput = new SerializedObject(_agent.GetComponent<AgentPlayerInput>());
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField(_agentPlayerInput.FindProperty("motionInputType"),
						new GUIContent("Motion Input"));
					EditorGUILayout.PropertyField(_agentPlayerInput.FindProperty("motionInputSpace"),
						new GUIContent("Motion Space"));
					EditorGUILayout.PropertyField(_agentPlayerInput.FindProperty("agentCamera"));
					EditorGUILayout.PropertyField(_agentPlayerInput.FindProperty("rotationType"));
					if (EditorGUI.EndChangeCheck())
					{
						_agentPlayerInput.ApplyModifiedProperties();
					}

					EditorGUILayout.EndVertical();
				}

				#endregion

				#region Animation

				if (_agentAnimation == null)
					_agentAnimation = new SerializedObject(_agent.GetComponent<AgentAnimation>());
				switch ((_agentAnimation.targetObject as AgentAnimation).AnimationMode)
				{
					case AnimationMode.Legacy:
					{
						if ((_agentAnimation.targetObject as AgentAnimation).LegacyAnimation)
						{
							EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Animation", EditorStyles.miniBoldLabel);
							if (GUILayout.Button(
								new GUIContent("Align Graphics Position", AgentAnimationEditor.AlignButtonTooltip),
								EditorStyles.toolbarButton))
							{
								AgentAnimationEditor.AdjustGraphicsPosition(
									_agentAnimation.targetObject as AgentAnimation);
							}

							EditorGUILayout.EndHorizontal();
							EditorGUILayout.EndVertical();
						}

						break;
					}

					case AnimationMode.Mecanim:
					{
						if ((_agentAnimation.targetObject as AgentAnimation).Animator)
						{
							EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Animation", EditorStyles.miniBoldLabel);
							if (GUILayout.Button(
								new GUIContent("Align Graphics Position", AgentAnimationEditor.AlignButtonTooltip),
								EditorStyles.toolbarButton))
							{
								AgentAnimationEditor.AdjustGraphicsPosition(
									_agentAnimation.targetObject as AgentAnimation);
							}

							EditorGUILayout.EndHorizontal();
							EditorGUILayout.EndVertical();
						}

						break;
					}
				}

				#endregion

				#region Resources

				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.LabelField("Resources", EditorStyles.miniBoldLabel);
				if (!_agentResources)
					_agentResources = _agent.GetComponent<AgentResources>();
				var editor = AgentComponentsEditorMap.GetEditor(_agentResources);
				(editor as AgentResourcesEditor).DrawReorderableListResources(_agentResources);
				EditorGUILayout.EndVertical();

				#endregion

				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(target, "Agent has been modified");
					EditorUtility.SetDirty(target);
					serializedObject.ApplyModifiedProperties();
				}
			}

			if (Application.isPlaying)
			{
				EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
				EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
				EditorGUILayout.ObjectField("Target", _agent.Target, typeof(Unit), true);
				EditorGUILayout.ObjectField("Current Skill", _agent.CurrentSkill, typeof(Skill), false);
				if (_agent.CurrentSkill)
				{
					EditorGUILayout.LabelField("Skill State: " + _agent.CurrentSkill.state);
				}

				if (_agent.GetComponent<AgentMotion>())
				{
					EditorGUILayout.LabelField("Motion State: " + _agent.GetAgentComponent<AgentMotion>().State);
					EditorGUILayout.LabelField("Motion Locked: " + _agent.GetAgentComponent<AgentMotion>().Locked);
				}

				EditorGUILayout.LabelField("Agent Status: " + _agent.Status);
				EditorGUILayout.EndVertical();
			}
		}


		/// <summary>
		/// Enables the Editor to handle an event in the scene view.
		/// </summary>
		public void OnSceneGUI()
		{
			_agent = target as EliotAgent;
			if (_agent == null) return;

			if (_agent.Target)
			{
				var curColor = Handles.color;
				Handles.color = new Color(0f, 0.75f, 1f, 0.3f);
				var dist = Vector3.Distance(_agent.transform.position, _agent.Target.position);
				var targetDirection = _agent.Target.position - _agent.transform.position;
				if (targetDirection != Vector3.zero)
				{
					var lookRotation = Quaternion.LookRotation(targetDirection);
					Handles.ArrowHandleCap(0,
						_agent.transform.position, lookRotation,
						dist * 0.25f, EventType.Repaint);
					Handles.DrawLine(_agent.transform.position, _agent.Target.position);
				}

				Handles.color = curColor;
			}

			DrawOriginHandles();

			foreach (var agentComponent in _agent.gameObject.GetComponents<AgentComponent>())
			{
				try
				{
					var editor = AgentComponentsEditorMap.GetEditor(agentComponent);
					editor.DrawSceneGUI(agentComponent);
				}
				catch (Exception e)
				{
					Debug.LogError(e, agentComponent);
				}

			}
		}

		/// <summary>
		/// Draw handles fo the skills origin.
		/// </summary>
		private void DrawOriginHandles()
		{
			DrawDoubleSphere(Color.red, _agent.SkillOrigin, 0.1f);
			var c = Handles.color;
			Handles.color = Color.red;
			Handles.ArrowHandleCap(0, _agent.SkillOrigin.position, _agent.SkillOrigin.rotation, 0.25f,
				EventType.Repaint);
			Handles.color = c;

			if (!_adjustOrigin) return;
			EditorGUI.BeginChangeCheck();
			Vector3 newPosition = Handles.PositionHandle(_agent.SkillOrigin.position,
				_agent.SkillOrigin.rotation);

			Quaternion newRotation = Handles.RotationHandle(_agent.SkillOrigin.rotation,
				_agent.SkillOrigin.position);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(_agent.SkillOrigin, "Change Origin Position");
				_agent.SkillOrigin.position = newPosition;
				_agent.SkillOrigin.rotation = newRotation;
			}

		}

		#region UTILITY

		/// <summary>
		/// Use handles to draw a wired sphere.
		/// </summary>
		/// <param name="color"></param>
		/// <param name="origin"></param>
		/// <param name="radius"></param>
		private static void DrawSphere(Color color, Transform origin, float radius)
		{
			var startColor = Handles.color;
			Handles.color = color;
			try
			{
				Handles.DrawWireArc(origin.position, origin.up, -origin.right, 360, radius);
				Handles.DrawWireArc(origin.position, origin.forward, -origin.right, 360, radius);
				Handles.DrawWireArc(origin.position, origin.right, -origin.forward, 360, radius);
			}
			catch (System.Exception)
			{
			}

			Handles.color = startColor;
		}

		/// <summary>
		/// Draw two wired spheres with slightly different radiuses.
		/// </summary>
		/// <param name="color"></param>
		/// <param name="origin"></param>
		/// <param name="radius"></param>
		private static void DrawDoubleSphere(Color color, Transform origin, float radius)
		{
			DrawSphere(color, origin, radius);
			DrawSphere(color, origin, radius + 0.005f);
		}

		#endregion
	}
}
#endif