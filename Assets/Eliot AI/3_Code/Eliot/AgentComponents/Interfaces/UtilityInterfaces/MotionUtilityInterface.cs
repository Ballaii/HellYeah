namespace Eliot.AgentComponents
{
    /// <summary>
    /// Base category for all the utility queries related to motion.
    /// </summary>
    public class MotionUtilityInterface : UtilityInterface
    {
        /// <summary>
        /// Link to the Agent's motion component.
        /// </summary>
        protected AgentMotion AgentMotion;
        public MotionUtilityInterface(EliotAgent agent) : base(agent)
        {
            AgentMotion = agent.GetAgentComponent<AgentMotion>();
        }
    }
}