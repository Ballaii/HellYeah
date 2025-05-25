using System;
using System.Collections.Generic;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// A runtime handler object for an arbitrary resources list.
    /// </summary>
    [Serializable]
    public class ResourcesProfile
    {
        /// <summary>
        /// The scriptable source template.
        /// </summary>
        public ResourcesProfileScriptable source;

        /// <summary>
        /// The list of resources.
        /// </summary>
        public List<Resource> resources = new List<Resource>();

        /// <summary>
        /// Editor only. Whether to display resources.
        /// </summary>
        public bool displayResources = true;

        /// <summary>
        /// Enables faster access to resources by their names.
        /// </summary>
        private Dictionary<string, Resource> cache = new Dictionary<string, Resource>();

        /// <summary>
        /// Find a resource by name.
        /// </summary>
        /// <param name="resourceName"></param>
        public Resource this[string resourceName]
        {
            get
            {
                if (resources.Count == 0) return null;
                if (cache.ContainsKey(resourceName)) return cache[resourceName];
                foreach (var resource in resources)
                {
                    if (resource.name == resourceName)
                    {
                        cache.Add(resourceName, resource);
                        return resource;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Check whether the profile matches an arbitrary scriptable template.
        /// </summary>
        /// <param name="scriptable"></param>
        /// <returns></returns>
        public bool Matches(ResourcesProfileScriptable scriptable)
        {
            if (resources == null) return false;
            if (resources.Count != scriptable.resources.Count) return false;
            for (int i = 0; i < resources.Count; i++)
            {
                if (!resources[i].Matches(scriptable.resources[i])) return false;
            }

            return true;
        }

        /// <summary>
        /// Check whether the profile matches the source template.
        /// </summary>
        /// <returns></returns>
        public bool MatchesSource()
        {
            if (resources == null) return false;
            if (!source) return false;
            if (resources.Count != source.resources.Count) return false;
            for (int i = 0; i < resources.Count; i++)
            {
                if (resources[i] == null || !source.resources[i]) continue;
                if (!resources[i].Matches(source.resources[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}