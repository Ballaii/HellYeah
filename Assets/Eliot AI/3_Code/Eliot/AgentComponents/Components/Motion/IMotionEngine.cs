using UnityEngine;

namespace Eliot.AgentComponents
{
	/// <summary>
	/// Common interface for all the Motion Engines in the project.
	/// </summary>
	public interface IMotionEngine
	{
		/// <summary>
		/// Return the position of current Agent's target.
		/// </summary>
		/// <returns></returns>
		Vector3 GetTarget();
		
		/// <summary>
		/// Assign a new target position.
		/// </summary>
		/// <param name="pos"></param>
		void SetTarget(Vector3 pos);
		
		/// <summary>
		/// Initialize an Agent so that it has all necessary parameters for using the engine.
		/// </summary>
		/// <param name="agent"></param>
		void Init(EliotAgent agent);

		/// <summary>
		/// Check if the Agent's motion is currently locked.
		/// </summary>
		/// <returns></returns>
		bool Locked();
		
		/// <summary>
		/// Lock the Agent's Motion component.
		/// </summary>
		void Lock();

		/// <summary>
		/// Unlock the Agent's Motion component.
		/// </summary>
		void Unlock();

		/// <summary>
		/// Rotate Agent towards its target.
		/// </summary>
		/// <param name="targetPosition"></param>
		void LookAtTarget(Vector3 targetPosition);

		/// <summary>
		/// Stand and relax playing default animation.
		/// </summary>
		void Idle();

		/// <summary>
		/// Stand still idling, with different animation from idle one.
		/// </summary>
		void Stand();

		/// <summary>
		/// Move in the given direction.
		/// </summary>
		/// <param name="direction"></param>
		/// <param name="run"></param>
		void Move(Vector3 direction, bool run);

		/// <summary>
		/// Walk to target, playing corresponding animation and audio. Idle if out of energy.
		/// </summary>
		void WalkToTarget();
		
		/// <summary>
		/// Walk away from target, playing corresponding animation and audio. Idle if out of energy.
		/// </summary>
		void WalkFromTarget();
		
		/// <summary>
		/// Run to target, playing corresponding animation and audio. Idle if out of energy.
		/// </summary>
		void RunToTarget();
		
		/// <summary>
		/// Run away from target, playing corresponding animation and audio. Idle if out of energy.
		/// </summary>
		void RunFromTarget();
		
		/// <summary>
		/// Get random point on the NavMesh and walk to it. 
		/// </summary>
		void WanderAround();
		
		/// <summary>
		/// Get random point on the NavMesh and run to it. 
		/// </summary>
		void RunAround();
		
		/// <summary>
		/// Set next waypoint as a target and walk to it. Repeat when Agent got to way point.
		/// </summary>
		void WalkAroundWayPoints();
		
		/// <summary>
		/// Set next waypoint as a target and run to it. Repeat when Agent got to way point.
		/// </summary>
		void RunAroundWayPoints();
		
		/// <summary>
		/// Get random point on the NavMesh and walk to it. Stand for specified amount of time. Walk again.
		/// </summary>
		void Patrol();
		
		/// <summary>
		/// Quick move in certain direction, with a corresponding animation.
		/// </summary>
		/// <param name="direction"></param>
		void Dodge(string direction);
		
		/// <summary>
		/// Let other Agents push this one around.
		/// </summary>
		/// <param name="pusherPos"></param>
		/// <param name="pushPower"></param>
		void Push(Vector3 pusherPos, float pushPower);

		/// <summary>
		/// Check if the path to the current target is valid.
		/// </summary>
		/// <returns></returns>
		bool PathIsValid();

	}
}