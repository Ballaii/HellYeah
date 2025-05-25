using System;
using System.Collections.Generic;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// Holder object for an attributes list.
    /// </summary>
    [Serializable] public class AttributesGroup
    {
        /// <summary>
        /// Scriptable object source.
        /// </summary>
        public AttributesGroupScriptable source;
        
        /// <summary>
        /// The attributes list.
        /// </summary>
        public List<Eliot.AgentComponents.Attribute> attributes = new List<Attribute>();

        /// <summary>
        /// Cache dictionary to enable faster access to the attributes that have been accessed previously.
        /// </summary>
        public Dictionary<string, Attribute> cache = new Dictionary<string, Attribute>();

        /// <summary>
        /// Only for Editor purposes.
        /// </summary>
        public bool displayAttributes = true;

        /// <summary>
        /// Return an attribute by name.
        /// </summary>
        /// <param name="attributeName"></param>
        public Eliot.AgentComponents.Attribute this[string attributeName]
        {
            get
            {
                if (attributes.Count == 0) return null;
                if (cache.ContainsKey(attributeName)) return cache[attributeName];
                foreach (var attribute in attributes)
                {
                    if (attribute.name == attributeName)
                    {
                        cache.Add(attributeName, attribute);
                        return attribute;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Check whether the attributes group matches an arbitrary scriptable source.
        /// </summary>
        /// <param name="scriptable"></param>
        /// <returns></returns>
        public bool Matches(AttributesGroupScriptable scriptable)
        {
            if (attributes == null) return false;
            if (attributes.Count != scriptable.attributes.Count) return false;
            for (int i = 0; i < attributes.Count; i++)
            {
                if (!attributes[i].Matches(scriptable.attributes[i])) return false;
            }

            return true;
        }

        /// <summary>
        /// Check whether the attributes group matches the scriptable source.
        /// </summary>
        /// <returns></returns>
        public bool MatchesSource()
        {
            if (attributes == null) return false;
            if (attributes.Count != source.attributes.Count) return false;
            for (int i = 0; i < attributes.Count; i++)
            {
                if (!attributes[i].Matches(source.attributes[i])) return false;
            }

            return true;
        }

        /// <summary>
        /// Use the source's values for all the attributes in the Attributes Group.
        /// </summary>
        public void MatchSource()
        {
            if (MatchesSource()) return;
            foreach (var sourceAttribute in source.attributes)
            {
                if (this[sourceAttribute.name] == null)
                {
                    attributes.Add(sourceAttribute.Clone());
                }
            }

            foreach (var attribute in attributes)
            {
                if (source[attribute.name] == null)
                {
                    attributes.Remove(attribute);
                }
            }
        }
        
        /// <summary>
        /// Reinitialize the list of attributes from the source.
        /// </summary>
        public void MatchSourceCompletely()
        {
            if (MatchesSource()) return;
            attributes = new List<Attribute>();
            for (int i = 0; i < source.attributes.Count; i++)
            {
                attributes.Add(source.attributes[i].Clone());
            }
        }
    }
}