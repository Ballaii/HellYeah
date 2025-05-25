namespace Eliot.AgentComponents
{
    /// <summary>
    /// Default utility queries related to resources.
    /// </summary>
    public class StandardResourcesUtilityInterface : ResourcesUtilityInterface
    {
        public StandardResourcesUtilityInterface(EliotAgent agent) : base(agent)
        {
        }

        /// <summary>
        /// Returns the current value of the agent's resource with a given name.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        [IncludeInBehaviour]
        public float CurrentResourceValue(string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName)) return 0;
            var resource = agentResources[resourceName];
            if (resource == null) return 0;
            return resource.currentValue;
        }

        /// <summary>
        /// Returns the current value of the agent's resource named 'Health'.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public float CurrentHealth()
        {
            return CurrentResourceValue(AgentResources.DefaultHealthResourceName);
        }

        /// <summary>
        /// Returns the current value of the agent's resource named 'Energy'.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public float CurrentEnergy()
        {
            return CurrentResourceValue(AgentResources.DefaultEnergyResourceName);
        }
    }
}