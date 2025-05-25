using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// A scriptable template for an Attributes Group. Can be shared between multiple Units.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "New Attributes Group", menuName = "Eliot AI/Attributes Group")]
    public class AttributesGroupScriptable : ScriptableObject
    {
        /// <summary>
        /// The list of attributes.
        /// </summary>
        public List<Attribute> attributes = new List<Attribute>();


        /// <summary>
        /// Execute upon selecting the object.
        /// </summary>
        public void OnEnable()
        {
#if UNITY_EDITOR
            hideFlags = HideFlags.None;
            EditorUtility.SetDirty(this);
#endif
        }


        /// <summary>
        /// Find an Attribute by name.
        /// </summary>
        /// <param name="attributeName"></param>
        public Attribute this[string attributeName]
        {
            get
            {
                if (attributes.Count == 0) return null;
                foreach (var attribute in attributes)
                {
                    if (attribute.name == attributeName)
                        return attribute;
                }

                return null;
            }
        }

        /// <summary>
        /// Return a list of default attributes that are used internally if added to an Agent.
        /// </summary>
        /// <returns></returns>
        public static List<Attribute> DefaultAttributes()
        {
            List<Attribute> result = new List<Attribute>();
            result.Add(new Attribute("LowHealth", 0));
            result.Add(new Attribute("LowEnergy", 0));
            result.Add(new Attribute("CloseRange", 0.5f));
            result.Add(new Attribute("MidRange", 5f));
            result.Add(new Attribute("FarRange", 10f));
            result.Add(new Attribute("FarFromHome", 50f));
            result.Add(new Attribute("AtHomeRange", 10f));
            result.Add(new Attribute("AimFieldOfView", 50f));
            result.Add(new Attribute("BackFieldOfView", 50f));
            return result;
        }
    }
}