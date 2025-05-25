using UnityEngine;
using UnityEngine.Events;

namespace Eliot.AgentComponents.Editor
{
    /// <summary>
    /// Helper class to access arbitrary resource's events and draw them using built-in drawer.
    /// </summary>
    public class ResourceEventsDrawHelper : ScriptableObject
    {
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
        /// Copy the references to the events.
        /// </summary>
        /// <param name="resource"></param>
        public void BindResource(Resource resource)
        {
            onReplenish = resource.onReplenish;
            onReduce = resource.onReduce;
            onEmpty = resource.onEmpty;
            onFull = resource.onFull;
        }
    }
}