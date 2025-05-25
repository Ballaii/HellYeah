using System;
using Random = UnityEngine.Random;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// Encapsulates a way to affect a resource.
    /// </summary>
    [Serializable]
    public struct ResourceAction
    {
        /// <summary>
        /// Resource name to affect.
        /// </summary>
        public string resourceName;
        
        /// <summary>
        /// Affection way/type (replenish/reduce).
        /// </summary>
        public Eliot.AgentComponents.ResourceAffectionWay affectionWay;
        
        /// <summary>
        /// Minimum amount of resource units to add/reduce.
        /// </summary>
        public int minPower;
        
        /// <summary>
        /// Maximum amount of resource units to add/reduce.
        /// </summary>
        public int maxPower;

        /// <summary>
        /// Get random between min and max.
        /// </summary>
        public int power
        {
            get { return minPower == maxPower ? minPower : Random.Range(minPower, maxPower); }
        }

        /// <summary>
        /// Build a resource action with a static power.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="affectionWay"></param>
        /// <param name="power"></param>
        public ResourceAction(string resourceName, Eliot.AgentComponents.ResourceAffectionWay affectionWay, int power)
        {
            this.resourceName = resourceName;
            this.affectionWay = affectionWay;
            this.minPower = power;
            this.maxPower = power;
        }

        /// <summary>
        /// Build a resource action.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="affectionWay"></param>
        /// <param name="minPower"></param>
        /// <param name="maxPower"></param>
        public ResourceAction(string resourceName, Eliot.AgentComponents.ResourceAffectionWay affectionWay,
            int minPower, int maxPower)
        {
            this.resourceName = resourceName;
            this.affectionWay = affectionWay;
            this.minPower = minPower;
            this.maxPower = maxPower;
        }
    }
}