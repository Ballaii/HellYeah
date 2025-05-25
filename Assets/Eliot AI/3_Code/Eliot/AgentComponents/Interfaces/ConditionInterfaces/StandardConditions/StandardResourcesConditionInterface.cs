using UnityEngine;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// The Standard Library of resources related conditions.
    /// </summary>
    public class StandardResourcesConditionInterface : ResourcesConditionInterface
    {

        public StandardResourcesConditionInterface(EliotAgent agent) : base(agent) 
        {
        }

        private float MultipliedValue(Resource resource, float percent)
        {
            return resource.maxValue * (Mathf.Clamp(percent, 0f, 100f)/100f);
        }

        /// <summary>
        /// Return true if the given resource is at it's maximum value.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool ResourceFull(string resourceName)
        {
            var resource = agentResources[resourceName];
            if (resource == null) return false;
            return resource.currentValue == resource.maxValue;
        }
        
        /// <summary>
        /// Return true if the given resource is at it's minimum value.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool ResourceEmpty(string resourceName)
        {
            var resource = agentResources[resourceName];
            if (resource == null) return false;
            return resource.currentValue == resource.minValue;
        }

        /// <summary>
        /// Return true if the given resource's value is less than or equals to the given percentage.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool ResourceLessEquals(string resourceName, float percent)
        {
            var resource = agentResources[resourceName];
            if (resource == null)
            {
                Debug.Log("Resource " + resourceName + " is not attached to the Agent.", Agent);
                return false;
            }
            return resource.currentValue <= MultipliedValue(resource, percent);
        }
        
        /// <summary>
        /// Return true if the given resource's value is more than or equals to the given percentage.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool ResourceMoreEquals(string resourceName, float percent)
        {
            var resource = agentResources[resourceName];
            if (resource == null) return false;
            return resource.currentValue >= MultipliedValue(resource, percent);
        }
        
        /// <summary>
        /// Return true if the given resource's value is less than the given percentage.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool ResourceLessThan(string resourceName, float percent)
        {
            var resource = agentResources[resourceName];
            if (resource == null) return false;
            return resource.currentValue < MultipliedValue(resource, percent);
        }
        
        /// <summary>
        /// Return true if the given resource's value is more than the given percentage.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool ResourceMoreThan(string resourceName, float percent)
        {
            var resource = agentResources[resourceName];
            if (resource == null) return false;
            return resource.currentValue > MultipliedValue(resource, percent);
        }
        
        /// <summary>
        /// Return true if the given resource's value is equals to the given percentage.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool ResourceEquals(string resourceName, float percent)
        {
            var resource = agentResources[resourceName];
            if (resource == null) return false;
            return resource.currentValue == (int)MultipliedValue(resource, percent);
        }
        
        /// <summary>
        /// Return true if Health is at it's maximum value.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour] public bool HealthFull()
        {
            return ResourceFull(AgentResources.DefaultHealthResourceName);
        }
        
        /// <summary>
        /// Return true if Health is at it's minimum value.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool HealthEmpty()
        {
            return ResourceEmpty(AgentResources.DefaultHealthResourceName);
        }
        
        /// <summary>
        /// Return true if Health is equals to or lower than agent's 'LowHealth' attribute value.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool HealthLow()
        {
            var resource = agentResources[AgentResources.DefaultHealthResourceName];
            if (resource == null) return false;
            var lowHealthAttribute = Agent["LowHealth"];
            if (lowHealthAttribute == null) return false;
            return resource.currentValue <= lowHealthAttribute.intValue;
        }

        /// <summary>
        /// Return true if Health is equals to or lower than given percentage.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool HealthLessEquals(float percent)
        {
            return ResourceLessEquals(AgentResources.DefaultHealthResourceName, percent);
        }
        
        /// <summary>
        /// Return true if Health is equals to or higher than given percentage.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool HealthMoreEquals(float percent)
        {
            return ResourceMoreEquals(AgentResources.DefaultHealthResourceName, percent);
        }
        
        /// <summary>
        /// Return true if Health is lower than given percentage.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool HealthLessThan(float percent)
        {
            return ResourceLessThan(AgentResources.DefaultHealthResourceName, percent);
        }
        
        /// <summary>
        /// Return true if Health is higher than given percentage.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool HealthMoreThan(float percent)
        {
            return ResourceMoreThan(AgentResources.DefaultHealthResourceName, percent);
        }
        
        /// <summary>
        /// Return true if Health is equals to the given percentage.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool HealthEquals(float percent)
        {
            return ResourceEquals(AgentResources.DefaultHealthResourceName, percent);
        }

        /// <summary>
        /// Return true if Energy is at it's maximum value.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour] public bool EnergyFull()
        {
            return ResourceFull(AgentResources.DefaultEnergyResourceName);
        }
        
        /// <summary>
        /// Return true if Energy is at it's minimum value.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool EnergyEmpty()
        {
            return ResourceEmpty(AgentResources.DefaultEnergyResourceName);
        }
        
        /// <summary>
        /// Return true if Energy is equals to or lower than agent's 'LowEnergy' attribute value.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool EnergyLow()
        {
            var resource = agentResources[AgentResources.DefaultEnergyResourceName];
            if (resource == null) return false;
            var lowEnergyAttribute = Agent["LowEnergy"];
            if (lowEnergyAttribute == null) return false;
            return resource.currentValue <= lowEnergyAttribute.intValue;
        }

        /// <summary>
        /// Return true if Energy is equals to or lower than given percentage.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool EnergyLessEquals(float percent)
        {
            return ResourceLessEquals(AgentResources.DefaultEnergyResourceName, percent);
        }
        
        /// <summary>
        /// Return true if Energy is equals to or higher than given percentage.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool EnergyMoreEquals(float percent)
        {
            return ResourceMoreEquals(AgentResources.DefaultEnergyResourceName, percent);
        }
        
        /// <summary>
        /// Return true if Energy is lower than given percentage.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool EnergyLessThan(float percent)
        {
            return ResourceLessThan(AgentResources.DefaultEnergyResourceName, percent);
        }
        
        /// <summary>
        /// Return true if Energy is higher than given percentage.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool EnergyMoreThan(float percent)
        {
            return ResourceMoreThan(AgentResources.DefaultEnergyResourceName, percent);
        }
        
        /// <summary>
        /// Return true if Energy is equals to the given percentage.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        [IncludeInBehaviour] public bool EnergyEquals(float percent)
        {
            return ResourceEquals(AgentResources.DefaultEnergyResourceName, percent);
        }

        /// <summary>
        /// Return true if Agent has enough resources to use the current skill.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool CanUseCurrentSkill()
        {
            var agentResources = Agent.GetAgentComponent<AgentResources>();
            if (!agentResources) return true;
            return agentResources.CanHandle(Agent.CurrentSkill.resourcesCost);
        }
        
        /// <summary>
        /// Return true if Agent has enough resources to use the given skill.
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool CanUseSkill(Skill skill)
        {
            var agentResources = Agent.GetAgentComponent<AgentResources>();
            if (!agentResources) return true;
            return agentResources.CanHandle(skill.resourcesCost);
        }
    }
}
