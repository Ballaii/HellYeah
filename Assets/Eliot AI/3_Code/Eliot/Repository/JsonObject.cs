using System;
using System.Collections.Generic;
using Eliot.BehaviourEditor;
using UnityEngine;

namespace Eliot.Repository
{
    /// <summary>
    /// Object that is created from a json string. Can hold all the needed
    /// information for building any types of objects.
    /// </summary>
    [System.Obsolete] public class JsonObject
    {
        #region PROPERTIES
        /// <summary>
        /// Name of the object. Analogues to a name of a variable.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        /// <summary>
        /// List of objects parsed from json.
        /// </summary>
        public List<object> Objects
        {
            get { return _objects; }
            set { _objects = value; }
        }
        
        /// <summary>
        /// Wheather the object is an array. Formatting json depends on this value.
        /// </summary>
        public bool IsArray
        {
            get { return _isArray; }
            set {_isArray = value; }
        }

        /// <summary>
        /// If there is one object in a list, treat object's value as the first one.
        /// Otherwise the object's value is the list.
        /// </summary>
        public object Value
        {
            get { return _objects[0]; }
            set { _objects = new List<object> {value}; }
        }

        #endregion

        #region FIELDS
        /// Name of the object. Analogues to a name of a variable.
        private string _name;
        /// Wheather the object is an array. Formatting json depends on this value.
        private bool _isArray;
        /// List of objects parsed from json.
        private List<object> _objects = new List<object>();
        
        #endregion

        #region INTERNAL
        /// <summary>
        /// Return true if it is possible to reduce the amount of objects withoul losing any meaningfull data.
        /// </summary>
        /// <returns></returns>
        private bool Reducible()
        {
            return string.IsNullOrEmpty(Name) && Objects.Count == 1;
        }
        
        /// <summary>
        /// Copy another object's properies.
        /// </summary>
        /// <param name="obj"></param>
        private void CopyFrom(object obj)
        {
            if (!(obj is JsonObject)) return;
            var jobj = obj as JsonObject;
            Name = jobj.Name;
            Objects = jobj.Objects;
            IsArray = jobj.IsArray;
        }
        
        /// <summary>
        /// Reduce the amount of objects by removing unnecessary ones and rebuilding the structure.
        /// </summary>
        private void CollapseIntoChild()
        {
            if (Objects.Count == 1 && !IsArray)
            {
                if (!(Value is JsonObject)) return;
                var jobj = Value as JsonObject;
                jobj.CollapseIntoChild();
                IsArray = jobj.IsArray;
                Objects = jobj.Objects;
            }
            else
                foreach (var obj in Objects)
                    if (obj is JsonObject)
                        (obj as JsonObject).CollapseIntoChild();
        }
        #endregion
        
        #region INTERFACE
        
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public JsonObject(){}
        
        /// <summary>
        /// Initialize the object from json string.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="reduceNesting"></param>
        public JsonObject(string json, bool reduceNesting = true)
        {
            var newObj = JsonParser.Read(json);
            Name = newObj.Name;
            Objects = newObj.Objects;
            IsArray = newObj.IsArray;
            
            if (reduceNesting)
            {
                ReduceNesting();
                if (Name != null)
                    GetInBox();
            }
        }
        
        /// <summary>
        /// Return a coppy of this object.
        /// </summary>
        /// <returns></returns>
        public JsonObject Copy()
        {
            var res = new JsonObject();
            res.Name = Name;
            res.Objects = Objects;
            res.IsArray = IsArray;
            return res;
        }
        
        /// <summary>
        /// Create a new empty json object and become its value.
        /// </summary>
        public void GetInBox()
        {
            var copy = Copy();
            Name = null;
            IsArray = false;
            Objects = new List<object>();
            Value = copy;
        }
        
        /// <summary>
        /// Remove reducable objects throughout the hiherarchy.
        /// </summary>
        public void ReduceNesting()
        {
            
            if(Reducible())
                CopyFrom(Value);
            
            foreach (var obj in Objects)
                if(obj is JsonObject)
                    (obj as JsonObject).ReduceNesting();

            CollapseIntoChild();
        }
        
        #region Subscription
        
        /// <summary>
        /// Get a child by its index.
        /// </summary>
        /// <param name="index"></param>
        public JsonObject this[int index]
        {
            get
            {
                return Objects.Count > index ? (JsonObject)Objects[index] : null;
            }
            set
            {
                if(Objects.Count > index)
                    Objects[index] = value;
            }
        }
        
        /// <summary>
        /// Get a child by its variable name.
        /// </summary>
        /// <param name="index"></param>
        public JsonObject this[string index]
        {
            get
            {
                foreach (var obj in Objects)
                    if (obj is JsonObject && (obj as JsonObject).Name == index)
                        return obj as JsonObject;
                return null;
            }
            set
            {
                var found = false;
                for(var i = 0; i < Objects.Count; i++)
                    if (Objects[i] is JsonObject && (Objects[i] as JsonObject).Name == index)
                    {
                        Objects[i] = value.Objects[0];
                        found = true;
                    }

                if (!found)
                {
                    Objects.Add(new JsonObject{Name=index});
                    this[index] = value;
                }
            }
        }
        #endregion

        /// <summary>
        /// Turn the object to a json string.
        /// </summary>
        /// <param name="quotes"></param>
        /// <returns></returns>
        public string Json(bool quotes = true)
        {
            var str = string.IsNullOrEmpty(Name) ? "" : (quotes?"\"":"") + Name + (quotes?"\"":"") + ":";
            str += IsArray ? "[" : (Objects.Count > 1 ? "{" : (quotes?"\"":""));
            
            for(var i = 0; i < Objects.Count; i++)
            {
                str += Objects[i] is JsonObject ? ((JsonObject) Objects[i]).Json() : Objects[i].ToString();
                if(i < Objects.Count - 1)
                    str += ",";
            }

            return str + (IsArray ? "]" : (Objects.Count > 1 ? "}" : (quotes?"\"":"")));
        }
        
        /// <summary>
        /// Return a string that would help user understand the object's structure.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var str = string.IsNullOrEmpty(Name) ? "" : "'" + Name + "' : ";
            str += IsArray ? "[" : (Objects.Count > 1 ? "{\n----" : "");
            
            for(var i = 0; i < Objects.Count; i++)
            {
                str += Objects[i];
                if(i < Objects.Count - 1)
                    str += "," + (IsArray ? " " : "\n----");
            }

            return str + (IsArray ? "]" : (Objects.Count > 1 ? "\n}" : ""));
        }
        
        /// <summary>
        /// Overload adding two json objects.
        /// </summary>
        /// <param name="var1"></param>
        /// <param name="var2"></param>
        /// <returns></returns>
        public static JsonObject operator +(JsonObject var1, JsonObject var2)
        {
            return new JsonObject(var1.Json() + "," + var2.Json());
        }
        #endregion

        #region CONVERT

        /// <summary>
        /// Convert object's value to an arbitrary type and return it.
        /// </summary>
        public T Astype<T>()
        {
            return (T)Value;
        }
        
        /// <summary>
        /// Return a boolean from object's value.
        /// </summary>
        public bool Bool
        {
            get { return bool.Parse(Value.ToString()); }
        }
        
        /// <summary>
        /// Return a string from object's value.
        /// </summary>
        public string String
        {
            get
            {
                try
                {
                    return Value.ToString();
                }
                catch (System.Exception)
                {
                    Debug.Log("Value not found");
                    return null;
                }
            }
        }
        
        /// <summary>
        /// Return an integer from object's value.
        /// </summary>
        public int Int
        {
            get
            {
                try
                {
                    return int.Parse(Value.ToString());
                }
                catch (Exception)
                {
                    Debug.Log("Value not found");
                    return int.MinValue;
                }
            }
        }

        /// <summary>
        /// Return a float from object's value.
        /// </summary>
        public float Float
        {
            get
            {
                try
                {
                    return float.Parse(Value.ToString());
                }
                catch (Exception)
                {
                    Debug.Log("Value not found");
                    return float.MinValue;
                }
            }
        }
        
        /// <summary>
        /// Return an ActionGroup from object's value.
        /// </summary>
        public ActionGroup ActionGroup
        {
            get{return (ActionGroup) Enum.Parse(typeof(ActionGroup), Value.ToString());}
        }
        
        /// <summary>
        /// Return a ConditionGroup from object's value.
        /// </summary>
        public ConditionGroup ConditionGroup
        {
            get{return (ConditionGroup) Enum.Parse(typeof(ConditionGroup), Value.ToString());}
        }

        #endregion
    }
}