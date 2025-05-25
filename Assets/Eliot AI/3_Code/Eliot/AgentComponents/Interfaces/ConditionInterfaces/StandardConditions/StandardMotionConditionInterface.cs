namespace Eliot.AgentComponents
{
    /// <summary>
    /// The Standard Library of motion related conditions.
    /// </summary>
    public class StandardMotionConditionInterface : MotionConditionInterface
    {
        public StandardMotionConditionInterface(EliotAgent agent) : base(agent)
        {
        }

        /// <summary>
        /// Check whether the current Agent's motion state is Idling.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool Idling()
        {
            if(AgentMotion==null) throw new EliotAgentComponentNotFoundException("Motion");
            return AgentMotion.State == MotionState.Idling;
        }

        /// <summary>
        /// Check whether the current Agent's motion state is Standing.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool Standing()
        {
            return AgentMotion.State == MotionState.Standing;
        }

        /// <summary>
        /// Check whether the current Agent's motion state is Walking.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool Walking()
        {
            return AgentMotion.State == MotionState.Walking;
        }

        /// <summary>
        /// Check whether the current Agent's motion state is WalkingAway.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool WalkingAway()
        {
            return AgentMotion.State == MotionState.WalkingAway;
        }

        /// <summary>
        /// Check whether the current Agent's motion state is Running.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool Running()
        {
            return AgentMotion.State == MotionState.Running;
        }

        /// <summary>
        /// Check whether the current Agent's motion state is RunningAway.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool RunningAway()
        {
            return AgentMotion.State == MotionState.RunningAway;
        }

        /// <summary>
        /// Check whether the current Agent's motion state is Dodging.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool Dodging()
        {
            return AgentMotion.State == MotionState.Dodging;
        }

    }
}
