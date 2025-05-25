#pragma warning disable CS0414, CS0649, CS1692
using System.Collections;
using System.Collections.Generic;
using Eliot.AgentComponents;
using Eliot.BehaviourEditor;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Eliot.Environment
{
	/// <summary>
	/// One of the way points in the group. Can define one of the border points
	/// of the group or a target position for the Agent. Can Interact with Agent
	/// changing his parameters if both Agent and Way point agree on that.
	/// </summary>
	[RequireComponent(typeof(Unit))]
	public class EliotWayPoint : MonoBehaviour
	{
		/// <summary>
		/// Whether the way point will apply its changes to an Agent.
		/// </summary>
		public bool ApplyChangesToAgent
		{
			get { return _applyChangesToAgent; }
		}

		/// <summary>
		/// A new Behaviour model that can be applied to an Agent.
		/// </summary>
		public EliotBehaviour NewBehaviour
		{
			get { return _newBehaviour; }
		}

		/// <summary>
		/// A reference to a new Way Points Group that can be applied to an Agent.
		/// </summary>
		public WayPointsGroup NewWayPointsGroup
		{
			get { return _newWayPointsGroup; }
		}
		
		/// <summary>
		/// Reference to a group if the way point belongs to one.
		/// </summary>
		public WayPointsGroup WayPointsGroup
		{
			get { return _wayPointsGroup; }
			set { _wayPointsGroup = value; }
		}

		/// <summary>
		/// Reference to a group if the way point belongs to one.
		/// </summary>
		[SerializeField] private WayPointsGroup _wayPointsGroup;
		
		[Tooltip("Whether the way point will apply its changes to an Agent.")]
		[SerializeField] private bool _applyChangesToAgent;
		[Space(10)]
		[Tooltip("A new Behaviour model that can be applied to an Agent.")]
		[SerializeField] private EliotBehaviour _newBehaviour;
		[Tooltip("A reference to a new Way Points Group that can be applied to an Agent.")]
		[SerializeField] private WayPointsGroup _newWayPointsGroup;

		/// <summary>
		/// Whether to define a custom action distance or to use the group's one.
		/// </summary>
		public bool overrideActionDistance = false;
		
		/// <summary>
		/// Distance at which the interaction with the agent would start.
		/// </summary>
		[SerializeField] private float actionDistance = 0.5f;
		
		/// <summary>
		/// Whether to define a custom display color or to use the group's one.
		/// </summary>
		public bool overrideColor = false;
		
		/// <summary>
		/// The color of the way point in the editor.
		/// </summary>
		[SerializeField] private Color color = Color.white;

		/// <summary>
		/// Get action distance given the setup.
		/// </summary>
		public float ActionDistance
		{
			get { return overrideActionDistance ? actionDistance : _wayPointsGroup ? _wayPointsGroup.ActionDistance : actionDistance; }
		}

		[Tooltip("This Skill will be applied to the target Agent.")]
		public Skill addEffect;
		[Tooltip("This Skill will be executed by the target Agent.")]
		public Skill executeSkill;

		/// <summary>
		/// Callback that gets executed when Agent enters the way point.
		/// </summary>
		public UnityEvent onAgentEnter;
		
		/// <summary>
		/// How much time in seconds to wait until executing the main part.
		/// </summary>
		public float actionDelay;
		
		/// <summary>
		/// Minimum time to be executing the main part.
		/// </summary>
		public float actionTimeMin;
		
		/// <summary>
		/// Maximum time to be executing the main part.
		/// </summary>
		public float actionTimeMax;
		
		/// <summary>
		/// Callback that gets executed when Agent does the main action.
		/// </summary>
		public UnityEvent onAgentAction;
		
		/// <summary>
		/// Callback that gets executed when Agent exits the way point.
		/// </summary>
		public UnityEvent onAgentExit;

		/// <summary>
		/// Maximum time of doing an action.
		/// </summary>
		public const float MaxActionTime = 120f;

		/// <summary>
		/// Whether to define a custom array of next way points or to use the group's ones.
		/// </summary>
		public bool overrideNextWayPoints = false;
		
		/// <summary>
		/// Next agent's targets.
		/// </summary>
		public List<EliotWayPoint> nextWayPoints = new List<EliotWayPoint>();

		/// <summary>
		/// If true, the way point is marked as not user-created and thus won't disrupt the job of other way points.
		/// </summary>
		public bool automatic = false;
		
		
#if UNITY_EDITOR
		/// <summary>
		/// Draw a circle with Handles with specific color.
		/// </summary>
		/// <param name="color"></param>
		public void DrawCircle(Color color)
		{
			DrawDoubleArc(color, 360, transform, ActionDistance);
		}

		/// <summary>
		/// Draw a label specifying the way point's index in the group.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="color"></param>
		public void DrawIndex(int index, Color color)
		{
			DrawLabel(index.ToString(), color, 0);
		}
		
		#region UTILITY
		
		/// <summary>
		/// Use Handles to draw a label.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="color"></param>
		/// <param name="yOffset"></param>
		private void DrawLabel(string text, Color color, float yOffset) {
			var style = new GUIStyle {normal = {textColor = color}};
			Handles.BeginGUI();
			var pos = transform.position;
			var pos2D = HandleUtility.WorldToGUIPoint(pos);
			GUI.Label(new Rect(pos2D.x, pos2D.y + yOffset, 100, 100), text, style);
			Handles.EndGUI();
		}

		/// <summary>
		/// Use handles to draw to arcs in a row with slightly different radiuses.
		/// </summary>
		/// <param name="color"></param>
		/// <param name="fov"></param>
		/// <param name="origin"></param>
		/// <param name="range"></param>
		private static void DrawDoubleArc(Color color, float fov, Transform origin, float range)
		{
			var startColor = Handles.color;
			Handles.color = color;
			Handles.DrawWireArc(origin.position, origin.up, -origin.right, fov, range);
			Handles.DrawWireArc(origin.position, origin.up, -origin.right, fov, range-0.05f);
			Handles.color = new Color(color.r, color.g, color.b, 0.05f);
			Handles.DrawSolidDisc(origin.position, origin.up, range);
			Handles.color = startColor;
		}
		#endregion
#endif

		/// <summary>
		/// Begin the execution of the way point's action.
		/// </summary>
		/// <param name="agent"></param>
		public void StartAction(EliotAgent agent)
		{
			if (ApplyChangesToAgent && agent.ApplyChangesOnWayPoints)
			{
				if (NewBehaviour)
					agent.SetBehaviour(NewBehaviour);
				if (NewWayPointsGroup)
					agent.WayPoints = NewWayPointsGroup;

				if(onAgentEnter != null) onAgentEnter.Invoke();

				StartCoroutine(WaitAndDoAction(agent));
			}
			else
			{
				if (nextWayPoints.Count > 0)
				{
					agent.Target = nextWayPoints[Random.Range(0, nextWayPoints.Count)].transform;
				}
				agent.isDoingAction = false;
			}
		}

		/// <summary>
		/// Get the next way point.
		/// </summary>
		/// <returns></returns>
		public EliotWayPoint Next()
		{
			if (nextWayPoints == null || nextWayPoints.Count == 0)
			{
				return null;
			}
			else
			{
				return nextWayPoints[Random.Range(0, nextWayPoints.Count)];
			}
		}

		/// <summary>
		/// Continue doing the action.
		/// </summary>
		/// <param name="agent"></param>
		/// <returns></returns>
		private IEnumerator WaitAndDoAction(EliotAgent agent)
		{
			yield return new WaitForSeconds(actionDelay);
			if(onAgentAction != null) onAgentAction.Invoke();
			if(addEffect)
				agent.AddEffect(addEffect, agent);
			if (executeSkill)
			{
				if (!agent.Skills.Contains(executeSkill))
				{
					agent.Skills.Add(executeSkill);
					agent.Skill(executeSkill.name, true)();
				}
			}
			
			yield return new WaitForSeconds(Random.Range(actionTimeMin, actionTimeMax));
			if(onAgentExit != null) onAgentExit.Invoke();
			agent.isDoingAction = false;
			
			if (overrideNextWayPoints && nextWayPoints.Count > 0)
			{
				agent.Target = nextWayPoints[Random.Range(0, nextWayPoints.Count)].transform;
			}
		}
#if UNITY_EDITOR
		/// <summary>
		/// Draw gizmos that are also pickable and always drawn.
		/// </summary>
		private void OnDrawGizmos()
		{
			var c = Gizmos.color;
			var gizmosColor = overrideColor ? color : _wayPointsGroup ? _wayPointsGroup.Colors.WaypointColor : color;
			var gizmosColorTransparent = new Color(gizmosColor.r, gizmosColor.g, gizmosColor.b, 0.3f);
			var gizmosColorSolid = new Color(gizmosColor.r, gizmosColor.g, gizmosColor.b, 1f);
			DrawCircle(gizmosColor);
			
			
			Gizmos.color = gizmosColorTransparent;
			Gizmos.DrawSphere(transform.position, 0.25f);
			Gizmos.color = gizmosColorSolid;
			Gizmos.DrawWireSphere(transform.position, 0.25f);
			Gizmos.color = c;

			if (nextWayPoints != null && nextWayPoints.Count > 0)
			{
				var handlesColor = Handles.color;
				Handles.color = gizmosColor;
				foreach (var nextWayPoint in nextWayPoints)
				{
					Handles.ArrowHandleCap(0, 
						transform.position, 
						Quaternion.LookRotation(nextWayPoint.transform.position - transform.position), 
						ActionDistance, EventType.Repaint);
					Handles.DrawLine(transform.position, nextWayPoint.transform.position);
				}

				Handles.color = handlesColor;
			}
		}
#endif
	}
}