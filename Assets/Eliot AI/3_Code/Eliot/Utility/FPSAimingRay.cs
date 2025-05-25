#pragma warning disable CS0414, CS1692, CS0649
using Eliot.AgentComponents;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Eliot.Utility
{
	/// <summary>
	/// Casts a ray to let Agents know if they are being aimed at by someone who is not an Eliot Agent.
	/// </summary>
	public class FPSAimingRay : MonoBehaviour
	{
		[Tooltip("How far does this thing work.")]
		[SerializeField] private float _range = 5f;
		[Tooltip("How long will the agent think that he is being aimed at.")]
		[SerializeField] private float _duration = 0.1f;
		[Space]
		[Tooltip("Show the ray in the Editor?")]
		[SerializeField] private bool _debug = true;
		[Tooltip("Color of the ray.")]
		[SerializeField] private Color _handlesColor;

		/// <summary>
		/// Update is called once per frame.
		/// </summary>
		private void FixedUpdate()
		{
			RaycastHit hit;
			if (!Physics.Raycast(transform.position, transform.forward, out hit, _range)) return;
			var agent = hit.transform.gameObject.GetComponent<EliotAgent>();
			if(agent) agent.SetStatus(AgentStatus.BeingAimedAt, _duration);
		}
#if UNITY_EDITOR
		/// <summary>
		/// Draw helper objects in the Editor.
		/// </summary>
		private void OnDrawGizmos()
		{
			if (!_debug) return;
			var startColor = Handles.color;
			Handles.color = _handlesColor;
			Handles.DrawLine(transform.position, transform.position + transform.forward * _range);
#if UNITY_5_6_OR_NEWER
			Handles.CubeHandleCap(0, transform.position, Quaternion.identity, 0.1f, EventType.Ignore);
#endif
			Handles.color = startColor;
		}
#endif
	}
}