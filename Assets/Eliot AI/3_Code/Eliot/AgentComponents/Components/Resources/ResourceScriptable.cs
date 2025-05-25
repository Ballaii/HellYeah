namespace Eliot.AgentComponents
{
    /// <summary>
    /// Encapsulates a spendable resource. Can be spent automatically or on actions.
    /// </summary>
    [System.Serializable] public class ResourceScriptable : UnityEngine.ScriptableObject
    {
        /// <summary>
        /// Maximum possible value.
        /// </summary>
        public int maxValue = 0;
        
        /// <summary>
        /// Minimum possible value.
        /// </summary>
        public int minValue = 0;

        /// <summary>
        /// The value of the resource at the initialization.
        /// </summary>
        public int initialValue = 0;

        /// <summary>
        /// The value added once every 'replenishCooldown' seconds.
        /// </summary>
        public int replenishAmount = 0;
        
        /// <summary>
        /// How much time (in seconds) to wait before replenishing again.
        /// </summary>
        public float replenishCooldown = 0;

        /// <summary>
        /// If true, the DeathHandler component will be forced to call Die() method when the resource is empty.
        /// </summary>
        public bool dieOnEmpty = false;
        
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public ResourceScriptable() { }

        /// <summary>
        /// Build a resource.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="initialValue"></param>
        /// <param name="replenishAmount"></param>
        /// <param name="replenishCooldown"></param>
        public ResourceScriptable(string name, int min, int max, int initialValue, int replenishAmount, float replenishCooldown)
        {
            this.name = name;
            dieOnEmpty = this.name == AgentResources.DefaultHealthResourceName;
            minValue = min;
            maxValue = max;
            this.initialValue = initialValue;
            this.replenishAmount = replenishAmount;
            this.replenishCooldown = replenishCooldown;
        }
        
        /// <summary>
        /// Get a copy of the resource.
        /// </summary>
        /// <returns></returns>
        public Resource Clone()
        {
            var clone = new Resource
            {
                name = this.name,
                maxValue = this.maxValue,
                minValue = this.minValue,
                initialValue = this.initialValue,
                replenishAmount = this.replenishAmount,
                replenishCooldown = this.replenishCooldown,
                dieOnEmpty = this.dieOnEmpty
            };
            return clone;
        }
    }
}