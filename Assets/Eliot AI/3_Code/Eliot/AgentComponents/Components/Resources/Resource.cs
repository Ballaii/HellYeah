using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Eliot.AgentComponents
{
    [System.Serializable]
    public class Resource
    {
        /// <summary>
        /// the way of referring to the resource. 
        /// </summary>
        public string name;
        
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
        /// Current value of the resource.
        /// </summary>
        public int currentValue = 0;

        /// <summary>
        /// If true, the DeathHandler component will be forced to call Die() method when the resource is empty.
        /// </summary>
        public bool dieOnEmpty = false;

        /// <summary>
        /// Action to do on replenishing. This one can only be set through API.
        /// </summary>
        public Action<int> onReplenishCallback;
        
        /// <summary>
        /// Action to do on reducing. This one can only be set through API.
        /// </summary>
        public Action<int> onReduceCallback;

        /// <summary>
        /// Action to do on replenishing.
        /// </summary>
        public UnityEvent onReplenish;
        
        /// <summary>
        /// Action to do on reducing.
        /// </summary>
        public UnityEvent onReduce;
        
        /// <summary>
        /// Action to do when the resource reaches its lowest value.
        /// </summary>
        public UnityEvent onEmpty;
        
        /// <summary>
        /// Action to do when the resource reaches its highest value.
        /// </summary>
        public UnityEvent onFull;
        
        /// <summary>
        /// Actions to do when resource's current value reaches arbitrary values. Can only be set manually through API.
        /// </summary>
        public Dictionary<int, Action> onValueCallbacks = new Dictionary<int, Action>();

        #region Temp

        /// <summary>
        /// For proper updating.
        /// </summary>
        private float _lastTimeUpdated = 0f;

        #endregion

        public EliotAgent agent;
        
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Resource() { }

        /// <summary>
        /// Build a resource.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="initialValue"></param>
        /// <param name="replenishAmount"></param>
        /// <param name="replenishCooldown"></param>
        public Resource(string name, int min, int max, int initialValue, int replenishAmount, float replenishCooldown)
        {
            this.name = name;
            if (this.name == AgentResources.DefaultHealthResourceName)
                dieOnEmpty = true;
            else dieOnEmpty = false;
            minValue = min;
            maxValue = max;
            this.initialValue = initialValue;
            currentValue = initialValue;
            this.replenishAmount = replenishAmount;
            this.replenishCooldown = replenishCooldown;
        }

        /// <summary>
        /// Replenish the resource by given amount.
        /// </summary>
        /// <param name="amount"></param>
        public void Replenish(int amount)
        {
            currentValue = Mathf.Clamp(currentValue + amount, minValue, maxValue);
            if(onReplenishCallback != null) onReplenishCallback.Invoke(amount);
            if(onReplenish != null) onReplenish.Invoke();
            if (onValueCallbacks.ContainsKey(currentValue)) onValueCallbacks[currentValue]();
            if (currentValue >= maxValue && onFull != null) onFull.Invoke();
        }
        
        /// <summary>
        /// Reduce the resource by given amount.
        /// </summary>
        /// <param name="amount"></param>
        public void Reduce(int amount)
        {
            currentValue = Mathf.Clamp(currentValue - amount, minValue, maxValue);
            if(onReduceCallback != null) onReduceCallback.Invoke(amount);
            if(onReduce != null) onReduce.Invoke();
            if (onValueCallbacks.ContainsKey(currentValue)) onValueCallbacks[currentValue]();
            if (currentValue <= minValue && onEmpty != null) onEmpty.Invoke();
        }

        /// <summary>
        /// Initialize the resource.
        /// </summary>
        public void Init(EliotAgent agent)
        {
            currentValue = initialValue;
            this.agent = agent;
        }

        /// <summary>
        /// Update the current value.
        /// </summary>
        public void Update()
        {
            if (Time.time >= _lastTimeUpdated + replenishCooldown)
            {
                _lastTimeUpdated = Time.time;
                if (replenishAmount != 0)
                {
                    if (replenishAmount > 0)
                        Replenish(replenishAmount);
                    else Reduce(replenishAmount);
                }
            }
            
            if (dieOnEmpty)
            {
                if (currentValue <= minValue)
                {
                    var deathHandler = agent.GetAgentComponent<AgentDeathHandler>();
                    if (deathHandler)
                    {
                        deathHandler.Die();
                    }
                }
            }
        }

        /// <summary>
        /// Check whether the resource matches another one by name.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Matches(ResourceScriptable other)
        {
            return 
                this.name == other.name;
        }
    }
}