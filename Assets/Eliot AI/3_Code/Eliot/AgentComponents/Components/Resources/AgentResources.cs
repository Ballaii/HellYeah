using System.Collections.Generic;
using UnityEngine;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// Available options of the way the Skill affects target's health. 
    /// </summary>
    public enum ResourceAffectionWay { Increase, Reduce }
    
    /// <summary>
    /// Agent component that handles arbitrary resources associated with the agent (in-game economy).
    /// </summary>
    [System.Serializable]
    public class AgentResources : AgentComponent
    {
        public const string DefaultHealthResourceName = "Health";
        public const string DefaultEnergyResourceName = "Energy";
        
        /// <summary>
        /// The list of the resources profiles associated with the agent.
        /// </summary>
        [SerializeField] public List<ResourcesProfile> resourcesProfiles = new List<ResourcesProfile>();

        /// <summary>
        /// Dictionary cache that enables faster access to the resources if requested second time and on.
        /// </summary>
        private Dictionary<string, Resource> cache = new Dictionary<string, Resource>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="agent"></param>
        public AgentResources(EliotAgent agent) : base(agent)
        {
        }

        /// <summary>
        /// This function is called when the component is added to an Agent.
        /// </summary>
        public override void OnAddComponent()
        {
            resourcesProfiles = new List<ResourcesProfile>();
            cache = new Dictionary<string, Resource>();
        }

        /// <summary>
        /// This function is called when Agent needs to be reset.
        /// </summary>
        public override void AgentReset()
        {
            foreach (var resourcesProfile in resourcesProfiles)
            {
                foreach (var resource in resourcesProfile.resources)
                {
                    if (resource == null)
                    {
                        OnAddComponent();
                        return;
                    }
                    resource.currentValue = resource.initialValue;
                }
            }
        }

        /// <summary>
        /// Find resource by name.
        /// </summary>
        /// <param name="resourceName"></param>
        public Resource this[string resourceName]
        {
            get
            {
                if (cache!=null && cache.ContainsKey(resourceName))
                    return cache[resourceName];
                foreach (var resourcesProfile in resourcesProfiles)
                {
                    var result = resourcesProfile[resourceName];
                    if (result != null)
                    {
                        if(cache==null)cache = new Dictionary<string, Resource>();
                        cache.Add(resourceName, result);
                        return result;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Replenish the given resource by the given amount.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="amount"></param>
        public void ReplenishResource(string resourceName, int amount)
        {
            var resource = this[resourceName];
            if(resource != null)
                resource.Replenish(amount);
        }
        
        /// <summary>
        /// Reduce the given resource by the given amount.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="amount"></param>
        public void ReduceResource(string resourceName, int amount)
        {
            var resource = this[resourceName];
            if(resource != null)
                resource.Reduce(amount);
        }

        /// <summary>
        /// Handle a resource action. (Replenish or Reduce)
        /// </summary>
        /// <param name="resourceAction"></param>
        public void Action(ResourceAction resourceAction)
        {
            switch (resourceAction.affectionWay)
            {
                case ResourceAffectionWay.Increase:
                {
                    ReplenishResource(resourceAction.resourceName, resourceAction.power);
                    break;
                }

                case ResourceAffectionWay.Reduce:
                {
                    ReduceResource(resourceAction.resourceName, resourceAction.power);
                    if (resourceAction.resourceName == AgentResources.DefaultHealthResourceName)
                    {
                        var animation = Agent.GetAgentComponent<AgentAnimation>();
                        if (animation)
                        {
                            animation.Animate(AnimationState.TakingDamage);
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Handle multiple resource actions at once.
        /// </summary>
        /// <param name="resourcesActions"></param>
        public void Action(IEnumerable<ResourceAction> resourcesActions)
        {
            foreach (var action in resourcesActions)
            {
                Action(action);
            }
        }

        /// <summary>
        /// Check whether agent has enough resources to perform an action.
        /// </summary>
        /// <param name="resourceAction"></param>
        /// <returns></returns>
        public bool CanHandle(ResourceAction resourceAction)
        {
            if (resourceAction.affectionWay == ResourceAffectionWay.Increase)
            {
                return true;
            }
            else
            {
                var resource = this[resourceAction.resourceName];
                return resource.currentValue - resourceAction.power > 0;
            }
        }

        /// <summary>
        /// Check whether agent has enough resources to perform an action.
        /// </summary>
        /// <param name="resourcesActions"></param>
        /// <returns></returns>
        public bool CanHandle(IEnumerable<ResourceAction> resourcesActions)
        {
            foreach (var action in resourcesActions)
            {
                if (!CanHandle(action)) return false;
            }

            return true;
        }

        /// <summary>
        /// Action performed once per agent update.
        /// </summary>
        public override void AgentUpdate()
        {
            foreach (var resourcesProfile in resourcesProfiles)
            {
                if(resourcesProfile == null) continue;
                foreach (var resource in resourcesProfile.resources)
                {
                    if(resource != null)
                        resource.Update();
                }
            }
        }

        /// <summary>
        /// Initialize the component.
        /// </summary>
        /// <param name="agent"></param>
        public override void Init(EliotAgent agent)
        {
            base.Init(agent);
            if (resourcesProfiles == null || resourcesProfiles.Count == 0) return;
            foreach (var resourcesProfile in resourcesProfiles)
            {
                if(resourcesProfile == null) continue;
                foreach (var resource in resourcesProfile.resources)
                {
                    if(resource != null)
                        resource.Init(agent);
                }
            }
        }
    }
}