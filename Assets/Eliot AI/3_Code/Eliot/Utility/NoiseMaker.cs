#pragma warning disable CS0414, CS0649
using Eliot.AgentComponents;
using UnityEngine;

namespace Eliot.Utility
{
	/// <summary>
	/// Let all the Eliot Agents in certain radius know that there
	/// is something going on at the position of the gameObject.
	/// </summary>
	public class NoiseMaker : MonoBehaviour
	{
		[Tooltip("Radius at which the Agents will know about the event.")]
		[SerializeField] private float _range;
		[Tooltip("How long will Agents be aware of the noise.")]
		[SerializeField] private float _duration;
		[Space]
		[Tooltip("Should the gameObject be destroyed after making the noise?")]
		[SerializeField] private bool _destroyAfterJobIsDone;
		
		/// <summary>
		/// Use this for initialization.
		/// </summary>
		private void Start()
		{
			Skill.MakeNoise(_range, transform, _duration);
			if(_destroyAfterJobIsDone) Destroy(gameObject);
		}
	}
}