using System;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// The Standard Library of motion related actions.
    /// </summary>
    public class StandardMotionActionInterface : MotionActionInterface
    {
        public StandardMotionActionInterface(EliotAgent agent) : base(agent)
        {
        }

        /// <summary>
        /// Stand doing nothing if MotionType is Creature,
        /// rotate from side to side if MotionType is Cannon.
        /// </summary>
        [IncludeInBehaviour] public void Idle() { _motion.Idle(); }

        /// <summary>
        /// Stop and stand.
        /// </summary>
        [IncludeInBehaviour] public void Stand() { _motion.Stand(); }

        /// <summary>
        /// Walk to the Agent's current target.
        /// </summary>
        [IncludeInBehaviour]
        public void WalkToTarget()
        {
            _motion.WalkToTarget();
        }

        /// <summary>
        /// Walk away from the Agent's current target.
        /// </summary>
        [IncludeInBehaviour] public void WalkAway() { _motion.WalkFromTarget(); }

        /// <summary>
        /// Run to the Agent's current target.
        /// </summary>
        [IncludeInBehaviour] public void RunToTarget() { _motion.RunToTarget(); }

        /// <summary>
        /// Run away from the Agent's current target.
        /// </summary>
        [IncludeInBehaviour] public void RunAway() { _motion.RunFromTarget(); }

        /// <summary>
        /// Dodge backwards.
        /// </summary>
        [IncludeInBehaviour] public void DodgeBack() { _motion.Dodge("back"); }

        /// <summary>
        /// Dodge to the right.
        /// </summary>
        [IncludeInBehaviour] public void DodgeRight() { _motion.Dodge("r"); }

        /// <summary>
        /// Dodge to the left.
        /// </summary>
        [IncludeInBehaviour] public void DodgeLeft() { _motion.Dodge("l"); }

        /// <summary>
        /// Execute dodging algorithm towards the Agent's target.
        /// </summary>
        [IncludeInBehaviour] public void RushForward() { _motion.Dodge("f"); }

        /// <summary>
        /// Dodge randomly left, right or backwards.
        /// </summary>
        [IncludeInBehaviour] public void DodgeRandom() { _motion.Dodge("random"); }

        /// <summary>
        /// Dodge randomly left or right.
        /// </summary>
        [IncludeInBehaviour] public void DodgeLeftOrRight() { _motion.Dodge("left_or_right"); }

        /// <summary>
        /// Rotate Agent's transform so that its forward direction points to the target.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [IncludeInBehaviour]
        public void LookAtTarget()
        {
            _motion.LookAtTarget(AgentMotionComponent.Type == MotionEngine.Turret ? AgentMotionComponent.Target : Agent.Target.position);
        }

        /// <summary>
        /// Pick a random spot in available area, set it as a target and move towards it.
        /// </summary>
        [IncludeInBehaviour]
        public void WanderAround()
        {
            _motion.WanderAround();
        }

        /// <summary>
        /// Walk through all waypoints in Waypoints Group in a row starting with the first one.
        /// </summary>
        [IncludeInBehaviour] public void WalkAroundWaypoints() { _motion.WalkAroundWayPoints(); }

        /// <summary>
        /// Pick a random spot in available area, set it as a target and run towards it.
        /// </summary>
        [IncludeInBehaviour] public void RunAround() { _motion.RunAround(); }

        /// <summary>
        /// Run through all waypoints in Waypoints Group in a row starting with the first one.
        /// </summary>
        [IncludeInBehaviour] public void RunAroundWaypoints() { _motion.RunAroundWayPoints(); }

        /// <summary>
        /// 1. Pick a random spot in available area, set it as a target and move towards it.
        /// 2. Wait for time specified in Agent's general settings.
        /// 3. Repeat.
        /// </summary>
        [IncludeInBehaviour] public void Patrol() { _motion.Patrol(); }
    }
}
