using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

namespace Eliot.Environment
{
	/// <summary>
	/// A controller object for a set of Way Points that can be used by Agents as an area or a path.
	/// </summary>
	public class WayPointsGroup : MonoBehaviour
	{
		/// <summary>
		/// The possible ways of pooling agents.
		/// </summary>
		public enum PoolingMode
		{
			Continuous,
			Waves,
			ContinuousAndWaves
		}

		/// <summary>
		/// A list of all the way points in the group.
		/// </summary>
		public List<EliotWayPoint> Points
		{
			get { return _points; }
		}

		/// <summary>
		/// Distance on which an Agent is considered to be in the _range of a way point of this group.
		/// </summary>
		public float ActionDistance
		{
			get { return actionDistance; }
		}

		/// <summary>
		/// The colors used to indicate different parts of the group.
		/// </summary>
		public WayPointsColors Colors
		{
			get
			{
				if (colors == null)
					colors = new WayPointsColors();
				return colors;
			}
		}

		[Header("Customization")]
		[Tooltip("Use colors to recognize the utility of this waypoints group with a single glance.")]
		[SerializeField]
		public WayPointsColors colors;

		[Tooltip("Distance on which an Agent is concidered to be in the _range of a waypoint of this group.")]
		[SerializeField]
		private float actionDistance = 1f;

		[Header("Pool")]
		[Tooltip("Whether this waypoints group should replenish the number of its population of Agents.")]
		public bool poolAgents;

		public PoolingMode poolingMode;

		[Tooltip("Number of Agents at which no new Agents will be instantiated.")]
		public int maxAgentsNumber;

		[Tooltip("Defines how often new Agents will be instantiated.")]
		public float agentsPoolCoolDown = 30f;

		[Tooltip("Random object from this list will be instantiated as a group's Agent.")]
		public List<GameObject> pooledAgentsPrefabs = new List<GameObject>();

		/// The last time the way points group instantiated an Agent.
		private float _lastTimePooledAgent;

		/// A list of all the way points in the group.
		private List<EliotWayPoint> _points = new List<EliotWayPoint>();

		/// Link to the GameObject that holds Agents as its children.
		private GameObject _agentsParent;

		/// <summary>
		/// Handles pooling agents at runtime.
		/// </summary>
		[SerializeField] public AgentsPoolHandler agentsPoolHandler;

		/// <summary>
		/// Handles pooling agents in waves.
		/// </summary>
		[SerializeField] public WayPointsSpawningWavesHandler wavesHandler;

		/// <summary>
		/// Get the pool handler or create one.
		/// </summary>
		/// <returns></returns>
		public AgentsPoolHandler PoolHandler()
		{
			if (agentsPoolHandler == null)
			{
				agentsPoolHandler = new AgentsPoolHandler(GetComponent<WayPointsGroup>());
			}

			return agentsPoolHandler;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Instantiate a new Way Points Group from the Editor.
		/// </summary>
		[MenuItem("Tools/Eliot AI/Create/Way-Points Group")]
		[MenuItem("GameObject/Eliot AI/Way-Points Group", false, 0)]
		private static void NewWayPointsGroup()
		{
			var waypointsObj = new GameObject("New Way-Points Group");
			Vector3 spawnPosition = Vector3.zero;
			try
			{
				spawnPosition = SceneView.lastActiveSceneView.pivot;
			}
			catch (System.Exception){ }
			waypointsObj.transform.position = spawnPosition;
			var waypointsGroup = waypointsObj.AddComponent<WayPointsGroup>();
			waypointsGroup.agentsPoolHandler = new AgentsPoolHandler(waypointsGroup);
			waypointsGroup.wavesHandler = new WayPointsSpawningWavesHandler(waypointsGroup);
			Selection.activeGameObject = waypointsObj;
		}
#endif

		/// <summary>
		/// Initialize the components on the loading of the scene.
		/// </summary>
		private void Start()
		{
			_points = new List<EliotWayPoint>();
			var wpts = GetComponentsInChildren<EliotWayPoint>();
			foreach (var child in wpts)
			{
				_points.Add(child);
			}

			agentsPoolHandler.Init(GetComponent<WayPointsGroup>());
		}

#if UNITY_EDITOR
		/// <summary>
		/// Draw helper objects for the group object as well as for each way point in the group.
		/// </summary>
		private void OnDrawGizmos()
		{
			// Draw label.
			var style = new GUIStyle();
			try
			{
				style.normal.textColor = Colors.OriginColor;
			}
			catch (System.Exception)
			{
				/*Dont worry*/
			}

			Handles.BeginGUI();
			var pos = transform.position + transform.up * 0.5f;
			var pos2D = HandleUtility.WorldToGUIPoint(pos);
			GUI.Label(new Rect(pos2D.x + 10, pos2D.y - 10, 100, 100), name, style);
			Handles.EndGUI();

			// Draw gizmos.
			try
			{
				Gizmos.color = Colors.OriginColor;
			}
			catch (System.Exception)
			{
				/*OK*/
			}

			Gizmos.DrawWireSphere(transform.position, 0.3f);
			var points = new List<Transform>();
			var index = 0;
			foreach (Transform point in transform)
			{
				if (point.GetComponent<EliotWayPoint>())
				{
					Handles.color = Colors.LineColor;
					Handles.DrawDottedLine(transform.position, point.position, 1f);
					points.Add(point);
					point.GetComponent<EliotWayPoint>().DrawIndex(index++, Colors.WaypointColor);
				}
			}
		}
#endif

		/// <summary>
		/// Get a collection of all triangles of the mesh that consists of Way Points.
		/// </summary>
		/// <returns></returns>
		private Vector3[][] AllTriangles()
		{
			if (!transform) return null;
			var triangles = new List<Vector3[]>();
			var waypoints = new List<Vector3>();
			var number = 0;
			foreach (Transform child in transform)
				if (child.GetComponent<EliotWayPoint>())
				{
					waypoints.Add(child.position);
					number++;
				}

			if (number == 0) return null;

			for (var i = 0; i < number; i++)
			{
				var a = i;
				var b = a + 1 >= number ? 0 : a + 1;
				triangles.Add(new List<Vector3>
				{
					waypoints[a],
					waypoints[b]
				}.ToArray());
			}

			return triangles.ToArray();
		}

		/// <summary>
		/// Pick a legit random triple of points, one of which is the group object itself.
		/// </summary>
		/// <returns></returns>
		private Vector3[] RandomTriangle()
		{
			if (!transform) return null;
			var waypoints = new List<Vector3>();
			var number = 0;
			foreach (Transform child in transform)
				if (child.GetComponent<EliotWayPoint>())
				{
					waypoints.Add(child.position);
					number++;
				}

			if (number == 0) return null;

			var randomA = Random.Range(0, number);
			var randomB = randomA + 1 >= number ? 0 : randomA + 1;
			return new List<Vector3>
			{
				waypoints[randomA],
				waypoints[randomB]
			}.ToArray();
		}

		/// <summary>
		/// Calculate a random point inside a randomly picked triangle.
		/// </summary>
		/// <returns></returns>
		public Vector3 RandomPoint()
		{
			if (WayPointsNumber() == 0)
			{
				return transform.position;
			}
			var triangle = RandomTriangle();
			var r1 = Random.Range(0f, 1f);
			var r2 = Random.Range(0f, 1f);
			var a = transform.position;
			var b = triangle[0];
			var c = triangle[1];
			return (1f - Mathf.Sqrt(r1)) * a + (Mathf.Sqrt(r1) * (1f - r2)) * b + (r2 * Mathf.Sqrt(r1)) * c;
		}

		/// <summary>
		/// Set all the waypoints uniformly around the group object and set them at specific radius.
		/// </summary>
		/// <param name="radius"></param>
		public void SetWayPointsAtRadius(float radius)
		{
			var number = 0;
			foreach (Transform child in transform)
				if (child.GetComponent<EliotWayPoint>())
					number++;
			if (number == 0) return;
			var delta = 2 * Mathf.PI / number;
			var fi = 0f;
			foreach (Transform point in transform)
			{
				if (!point.GetComponent<EliotWayPoint>()) continue;
				fi += delta;
				var x = radius * Mathf.Cos(fi);
				var z = radius * Mathf.Sin(fi);
				point.position = transform.position + new Vector3(x, 0, z);
			}
		}

		/// <summary>
		/// Remove all way points from the group.
		/// </summary>
		public void Clear()
		{
			var number = transform.childCount;
			if (number == 0) return;
			for (var i = transform.childCount - 1; i >= 0; i--)
			{
				var child = transform.GetChild(i);
				if (child.GetComponent<EliotWayPoint>())
					DestroyImmediate(child.gameObject);
			}
		}

		/// <summary>
		/// Clear the group, create specific number of way points and set them at specific radius.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="radius"></param>
		public void SetWayPointsNumber(int number, float radius)
		{
			Clear();
			if (number == 0) return;
			for (var i = 0; i < number; i++)
			{
				var newWayPoint = new GameObject("WayPoint[" + i + "]");
				newWayPoint.transform.position = transform.position;
				newWayPoint.transform.parent = transform;
				newWayPoint.AddComponent<EliotWayPoint>();
			}

			SetWayPointsAtRadius(radius);
		}

		/// <summary>
		/// Get the child that holds all the group's Agents or create one.
		/// </summary>
		/// <returns></returns>
		public GameObject AgentsParent()
		{
			if (!_agentsParent)
			{
				var agentsParent = transform.Find("__agents__");
				if (agentsParent)
				{
					_agentsParent = agentsParent.gameObject;
				}
#if UNITY_EDITOR
				else
				{
					_agentsParent = new GameObject("__agents__");
					_agentsParent.transform.parent = transform;
					_agentsParent.transform.localPosition = Vector3.zero;
				}
#endif
			}

			return _agentsParent;
		}

		/// <summary>
		/// Return the current number of Agents in the group.
		/// </summary>
		/// <returns></returns>
		public int AgentsNumber()
		{
			int totalActive = 0;
			foreach (Transform child in AgentsParent().transform)
			{
				if (child.gameObject.activeSelf)
				{
					totalActive++;
				}
			}

			return totalActive;
		}

		/// <summary>
		/// Return the current number of way points in the group.
		/// </summary>
		/// <returns></returns>
		public int WayPointsNumber()
		{
			return transform.GetComponentsInChildren<EliotWayPoint>().Length;
		}

		/// <summary>
		/// Pick a random Agent from the array of possible Agents.
		/// </summary>
		/// <returns></returns>
		public GameObject RandomAgentFromPrefabsList()
		{
			if (pooledAgentsPrefabs == null || pooledAgentsPrefabs.Count == 0) return null;
			return pooledAgentsPrefabs[Random.Range(0, pooledAgentsPrefabs.Count)];
		}

		/// <summary>
		/// Instantiate a new Agent at a random position inside the group's area.
		/// </summary>
		public void SpawnAgent()
		{
			if (!poolAgents) return;
			if (pooledAgentsPrefabs.Count == 0) return;
			this.agentsPoolHandler.SpawnAgent();
		}

		/// <summary>
		/// Spawn an agent using pool handler.
		/// </summary>
		/// <param name="pooledObjectsList"></param>
		public void SpawnAgent(List<GameObject> pooledObjectsList)
		{
			if (!poolAgents) return;
			if (pooledObjectsList.Count == 0) return;
			this.agentsPoolHandler.SpawnAgent(pooledObjectsList);
		}

		#region CheckIfPointIsInside

		private static float Sign(Vector3 p1, Vector3 p2, Vector3 p3)
		{
			return (p1.x - p3.x) * (p2.z - p3.z) - (p2.x - p3.x) * (p1.z - p3.z);
		}

		private static bool PointInTriangle(Vector3 pt, Vector3 v1, Vector3 v2, Vector3 v3)
		{
			bool b1, b2, b3;

			b1 = Sign(pt, v1, v2) < 0.0f;
			b2 = Sign(pt, v2, v3) < 0.0f;
			b3 = Sign(pt, v3, v1) < 0.0f;

			return ((b1 == b2) && (b2 == b3));
		}

		/// <summary>
		/// Check whether a point is inside the group's polygon.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public bool IsInsidePolygon(Vector3 point)
		{
			foreach (var triangle in AllTriangles())
				if (PointInTriangle(point, transform.position, triangle[0], triangle[1]))
					return true;

			return false;
		}

		#endregion

		public void OnEnable()
		{
			wavesHandler.OnEnable();
		}

		/// <summary>
		/// Update the object every frame.
		/// </summary>
		private void Update()
		{
			var poolHandler = PoolHandler();
			if(poolHandler != null)
				poolHandler.Update();
		}

		/// <summary>
		/// Overload the subscription of the object.
		/// </summary>
		/// <param name="index"></param>
		public EliotWayPoint this[int index]
		{
			get
			{
				if (index + 1 > Points.Count) index = 0;
				if (index < 0) index = Points.Count + index;
				return transform.GetComponentsInChildren<EliotWayPoint>()[index];
			}
		}
	}
}