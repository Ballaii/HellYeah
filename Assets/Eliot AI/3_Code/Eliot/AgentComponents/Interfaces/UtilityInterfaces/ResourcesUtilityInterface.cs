namespace Eliot.AgentComponents
{
    /// <summary>
    /// Base category for all the utility queries related to resources.
    /// </summary>
    public class ResourcesUtilityInterface : UtilityInterface
    {
        /// <summary>
        /// Link to the Agent's resources component.
        /// </summary>
        protected AgentResources agentResources;
        public ResourcesUtilityInterface(EliotAgent agent) : base(agent)
        {
            agentResources = agent.GetAgentComponent<AgentResources>();
        }
    }
}
