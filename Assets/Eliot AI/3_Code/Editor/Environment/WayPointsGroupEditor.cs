using Eliot.Utility;
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Eliot.AgentComponents;
using UnityEditor;
using UnityEngine;

namespace Eliot.Environment.Editor
{
	/// <summary>
	/// Editor extension for WayPointsGroup objects.
	/// </summary>
	[CustomEditor(typeof(WayPointsGroup))]
	[CanEditMultipleObjects]
	[Serializable]
	public class WayPointsGroupEditor : UnityEditor.Editor
	{
		/// <summary>
		/// Serialized Properties to draw and other variables.
		/// </summary>
		#region Variables
		private WayPointsGroup _wayPointsGroup;
		/// Specify the radius at which to set way points.
		[SerializeField] private float _radius = 1;
		/// Specify the number of way points to create.
		[SerializeField] private int _waypointsNum = 5;
		/// Specify the number of Agents to create.
		[SerializeField] private int _agentsNumber;
		/// Whether or not to set this way points group at a created Agents' one.
		[SerializeField] private bool _setThisAsAgentsWayPoints = true;
		/// Whether or not to put created Agents inside.
		[SerializeField] private bool _putAgentsInside = true;
		/// Specify the Agent prefab to create. If none is specified,
		/// random one from the array of Agents will be picked.
		[SerializeField] private GameObject _agentPrefab;
		/// Keep the record of created Agents. 
		[SerializeField] private List<GameObject> _spawnedAgents = new List<GameObject>();
		/// The object that can be a parent to newly created Agents.
		private GameObject _agentsParent;
		/// Whether or not the user is currently placing way points.
		private bool _wayPointsPlacementMode;

		private SerializedProperty _customizationColors;
		private SerializedProperty _actionDistance;

		private SerializedProperty _poolAgents, _poolingMode, _maxAgentsNumber, _agentsPoolCoolDown, _pooledAgentsPrefabs;

		private EliotWayPoint _lastCreatedWayPoint;
		#endregion
		
		/// <summary>
		/// This function is called when the object is loaded.
		/// </summary>
		private void OnEnable()
		{
			hideFlags = HideFlags.DontSave;
			_wayPointsGroup = target as WayPointsGroup;
			_customizationColors = serializedObject.FindProperty("colors");
			_actionDistance = serializedObject.FindProperty("actionDistance");
			
			// Pooling related serialized properties
			_poolAgents = serializedObject.FindProperty("poolAgents");
			_poolingMode = serializedObject.FindProperty("poolingMode");
			_maxAgentsNumber = serializedObject.FindProperty("maxAgentsNumber");
			_agentsPoolCoolDown = serializedObject.FindProperty("agentsPoolCoolDown");
			_pooledAgentsPrefabs = serializedObject.FindProperty("pooledAgentsPrefabs");
			
			_wayPointsGroup.PoolHandler().Init(_wayPointsGroup);
			_wayPointsGroup.wavesHandler.OnEnable();
		}
		
		/// <summary>
		/// Draw the new inspector properties.
		/// </summary>
		public override void OnInspectorGUI()
		{
			EliotEditorUtility.DrawHeader<WayPointsGroup>("WayPoints Group", target, EliotGUISkin.GrayBackground);
        
			EditorGUI.BeginChangeCheck();
			
			serializedObject.Update();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_customizationColors, true);
			EditorGUI.indentLevel--;
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUILayout.PropertyField(_actionDistance);
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			if (GUILayout.Button("Project On Plane"))
			{
				RaycastHit hit;
				var transform = Selection.activeTransform;
				var ray = new Ray(transform.position, -transform.up);
				if (Physics.Raycast(ray, out hit))
					Selection.activeTransform.position = hit.point;
				else
				{
					ray = new Ray(transform.position, transform.up);
					if (Physics.Raycast(ray, out hit))
						Selection.activeTransform.position = hit.point;
				}
			}
			
			_wayPointsPlacementMode = GUILayout.Toggle(_wayPointsPlacementMode, "Place Way Points", GUI.skin.button);
			EditorGUILayout.EndVertical();
			
			serializedObject.ApplyModifiedProperties();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Set Way Points Number");
			_waypointsNum = EditorGUILayout.IntField(_waypointsNum);
			if (GUILayout.Button("Set Number"))
				Selection.activeGameObject.GetComponent<WayPointsGroup>().SetWayPointsNumber(_waypointsNum, _radius);
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Set On Radius");
			_radius = EditorGUILayout.FloatField(_radius);
			if (GUILayout.Button("Set"))
				Selection.activeGameObject.GetComponent<WayPointsGroup>().SetWayPointsAtRadius(_radius);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			if (GUILayout.Button("Clear Way Points"))
				Selection.activeGameObject.GetComponent<WayPointsGroup>().Clear();
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			var labelSkin = GUI.skin.label;
			labelSkin.richText = true;
			EditorGUILayout.TextArea("<size=10>Specify the Agent prefab to create. If none is specified,\n" +
			                         " random one from the array of Agents will be picked.</size>", labelSkin);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Agent Prefab");
			_agentPrefab = (GameObject)EditorGUILayout.ObjectField(_agentPrefab, typeof(GameObject), false);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Agents Number");
			_agentsNumber = EditorGUILayout.IntField(_agentsNumber);
			EditorGUILayout.EndHorizontal();
			_setThisAsAgentsWayPoints = GUILayout.Toggle(_setThisAsAgentsWayPoints, "Set This As Way Points");
			_putAgentsInside = GUILayout.Toggle(_putAgentsInside, "Put Agents Inside");
			if (GUILayout.Button("Mass Place Agents"))
			{
				if (_putAgentsInside)
					_agentsParent = Selection.activeGameObject.GetComponent<WayPointsGroup>().AgentsParent();
				if(_agentsNumber > 0)
					for (var i = 0; i < _agentsNumber; i++)
					{
						var prefab = _agentPrefab == null ? 
							Selection.activeGameObject.GetComponent<WayPointsGroup>().RandomAgentFromPrefabsList() : _agentPrefab;
						var position = Selection.activeGameObject.GetComponent<WayPointsGroup>().RandomPoint();
						var rotation = new Quaternion(0, UnityEngine.Random.Range(0, 2*Mathf.PI), 0, 1);
						var newAgent = Instantiate(prefab, position, rotation) as GameObject;
						var agent = newAgent.GetComponent<EliotAgent>();
						if (agent && _setThisAsAgentsWayPoints)
						{
							agent.WayPoints = Selection.activeGameObject.GetComponent<WayPointsGroup>();
						}
						_spawnedAgents.Add(newAgent);
						
						if (_putAgentsInside)
							newAgent.transform.parent = _agentsParent.transform;
					}
			}
			if (GUILayout.Button("Clear Agents"))
			{
				var agentsParent = Selection.activeTransform.Find("__agents__");
				if (agentsParent)
				{
					for (var i = agentsParent.childCount - 1; i >= 0; i--)
						DestroyImmediate(agentsParent.GetChild(i).gameObject);
				}
				else
				{
					if (_spawnedAgents.Count > 0)
						for (var i = _spawnedAgents.Count - 1; i >= 0; i--)
							DestroyImmediate(_spawnedAgents[i]);
				}
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUILayout.PropertyField(_poolAgents);

			if (_poolAgents.boolValue)
			{
				EditorGUILayout.PropertyField(_poolingMode);
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(_pooledAgentsPrefabs, true);
				EditorGUI.indentLevel--;
				if (_poolingMode.enumValueIndex == 0 || _poolingMode.enumValueIndex == 2)
				{
					EditorGUILayout.PropertyField(_maxAgentsNumber);
					EditorGUILayout.PropertyField(_agentsPoolCoolDown);
				}
				if (_poolingMode.enumValueIndex == 1 || _poolingMode.enumValueIndex == 2)
				{
					if ((target as WayPointsGroup).agentsPoolHandler.WavesHandler == null)
					{
						(target as WayPointsGroup).agentsPoolHandler.WavesHandler =
							new WayPointsSpawningWavesHandler((target as WayPointsGroup));
					}
					EditorGUI.indentLevel++;
					(target as WayPointsGroup).agentsPoolHandler.WavesHandler.DrawEditor(serializedObject);
					EditorGUI.indentLevel--;
				}
			}
			EditorGUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RegisterCompleteObjectUndo(target, "Way Points Group change");
				serializedObject.ApplyModifiedProperties();
			}
		}
		
		/// <summary>
		/// Enables the Editor to handle an event in the scene view.
		/// </summary>
		private void OnSceneGUI()
		{
			if (_wayPointsPlacementMode)
			{
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
				if (Event.current.type == EventType.MouseDown && Event.current.button == 0
				    && !Event.current.command && !Event.current.control && !Event.current.alt)
				{
					var obj = Selection.activeObject;
					RaycastHit hit;
					var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
					if (Physics.Raycast(ray, out hit))
					{
						var position = hit.point;
						var wayPointsCount = Selection.activeGameObject
							.GetComponent<WayPointsGroup>().WayPointsNumber();

						var newWayPoint = new GameObject("WayPoint[" 
						                                 + wayPointsCount + "]");
						newWayPoint.transform.position = position;
						newWayPoint.transform.parent = Selection.activeTransform;
						var wayPoint = newWayPoint.AddComponent<EliotWayPoint>();
						wayPoint.WayPointsGroup = target as WayPointsGroup;
						
						if(_lastCreatedWayPoint)
						{
							_lastCreatedWayPoint.nextWayPoints = new List<EliotWayPoint>{wayPoint};

							if (wayPointsCount > 1)
							{
								wayPoint.nextWayPoints = new List<EliotWayPoint>{_wayPointsGroup[0]};
							}
						}
						_lastCreatedWayPoint = wayPoint;
					}

					Selection.activeObject = obj;
					Event.current.Use();
				}
			}
		}
	}
}
#endif