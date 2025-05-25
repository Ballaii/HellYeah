using UnityEngine;
using UnityEngine.Events;

namespace Eliot.Environment.Editor
{
	/// <summary>
	/// Helper class to access arbitrary wave's events and draw them using built-in drawer.
	/// </summary>
	public class WaveEventsDrawHelper : ScriptableObject
	{
		/// <summary>
		/// Action to do on wave start or on transition to this wave.
		/// </summary>
		public UnityEvent onWaveStart;
		
		/// <summary>
		/// Action to do after the delay has been passed.
		/// </summary>
		public UnityEvent onWaveDelayPassed;
		
		/// <summary>
		/// Action to do every time an agent gets spawned.
		/// </summary>
		public UnityEvent onAgentSpawned;
		
		/// <summary>
		/// Action to do when all the agents have been spawned.
		/// </summary>
		public UnityEvent onWaveEnd;
		
		/// <summary>
		/// Action to do right before transitioning to the next wave.
		/// </summary>
		public UnityEvent onWaveCooldownPassed;
		
		/// <summary>
		/// Copy the references to the events.
		/// </summary>
		/// <param name="wave"></param>
		public void BindWave(WayPointsSpawningWave wave)
		{
			onWaveStart = wave.onWaveStart;
			onWaveDelayPassed = wave.onWaveDelayPassed;
			onAgentSpawned = wave.onAgentSpawned;
			onWaveEnd = wave.onWaveEnd;
			onWaveCooldownPassed = wave.onWaveCooldownPassed;
		}
	}
}