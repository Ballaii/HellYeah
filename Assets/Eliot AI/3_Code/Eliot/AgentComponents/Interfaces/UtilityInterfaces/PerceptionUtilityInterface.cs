namespace Eliot.AgentComponents
{
    /// <summary>
    /// Base category for all the utility queries related to perception.
    /// </summary>
    public class PerceptionUtilityInterface : UtilityInterface
    {
        /// <summary>
        /// Link to the Agent's perception component.
        /// </summary>
        protected AgentPerception AgentPerception;
        public PerceptionUtilityInterface(EliotAgent agent) : base(agent)
        {
            AgentPerception = agent.GetAgentComponent<AgentPerception>();
        }
    }
}
