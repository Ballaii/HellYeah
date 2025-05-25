using System;
using Object = UnityEngine.Object;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable] public class Attribute
    {
        /// <summary>
        /// The type of the attribute. Can be int, bool, float, string and UnityEngine.Object.
        /// </summary>
        public AttributeType type;
        
        /// <summary>
        /// How to refer to the attribute.
        /// </summary>
        public string name;
        
        /// <summary>
        /// Holder for the boolean value.
        /// </summary>
        public bool boolValue;
        
        /// <summary>
        /// Holder for the integer value.
        /// </summary>
        public int intValue;
        
        /// <summary>
        /// Holder for the float value.
        /// </summary>
        public float floatValue;
        
        /// <summary>
        /// Holder for the string value.
        /// </summary>
        public string stringValue;
        
        /// <summary>
        /// Holder for the UnityEngine.Object value.
        /// </summary>
        public Object objectValue;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Attribute(){}
        
        /// <summary>
        /// Create an Attribute of type Bool.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Attribute(string name, bool value)
        {
            this.name = name;
            this.boolValue = value;
            this.type = AttributeType.Bool;
        }
        
        /// <summary>
        /// Create an Attribute of type Int.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Attribute(string name, int value)
        {
            this.name = name;
            this.intValue = value;
            this.type = AttributeType.Int;
        }
        
        /// <summary>
        /// Create an Attribute of type Float.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Attribute(string name, float value)
        {
            this.name = name;
            this.floatValue = value;
            this.type = AttributeType.Float;
        }
        
        /// <summary>
        /// Create an Attribute of type String.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Attribute(string name, string value)
        {
            this.name = name;
            this.stringValue = value;
            this.type = AttributeType.String;
        }
        
        /// <summary>
        /// Create an Attribute of type UnityEngine.Object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Attribute(string name, Object value)
        {
            this.name = name;
            this.objectValue = value;
            this.type = AttributeType.Object;
        }
        
        /// <summary>
        /// Create a copy of the Attribute.
        /// </summary>
        /// <returns></returns>
        public Attribute Clone()
        {
            var clone = new Attribute();
            clone.type = this.type;
            clone.name = this.name;
            clone.boolValue = this.boolValue;
            clone.intValue = this.intValue;
            clone.floatValue = this.floatValue;
            clone.stringValue = this.stringValue;
            clone.objectValue = this.objectValue;
            return clone;
        }

        /// <summary>
        /// Check whether the attribute matches another one (to avoid reinitialization of the same attributes multiple times).
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Matches(Attribute other)
        {
            return this.type == other.type && this.name == other.name;
        }
    }
}