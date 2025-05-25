using System;
using System.Reflection;
using UnityEngine;

namespace Eliot.BehaviourEditor
{
    /// <summary>
    /// Hold all the necessary information to store and recover any method parameter information.
    /// </summary>
    [Serializable]
    public class Parameter
    {
        /// <summary>
        /// Name of the parameter.
        /// </summary>
        [SerializeField] public string parameterName;

        /// <summary>
        /// Type of the parameter.
        /// </summary>
        [SerializeField] public string parameterType;

        /// <summary>
        /// Value of the parameter. Will be parsed from string later.
        /// </summary>
        [SerializeField] public string parameterValue;

        /// <summary>
        /// Object value for UnityEngine Objects.
        /// </summary>
        [SerializeField] public UnityEngine.Object parameterObjectValue;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Parameter()
        {
        }

        /// <summary>
        /// Builds a parameter from the given ParameterInfo.
        /// </summary>
        /// <param name="parameterInfo"></param>
        public Parameter(ParameterInfo parameterInfo)
        {
            this.parameterName = parameterInfo.Name;
            this.parameterType = parameterInfo.ParameterType.FullName;

            if (parameterInfo.ParameterType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                this.parameterValue = default(string);
                this.parameterObjectValue = parameterInfo.DefaultValue as UnityEngine.Object;
            }
            else this.parameterValue = parameterInfo.DefaultValue.ToString();
        }

        /// <summary>
        /// Check if matches to other Parameter.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Parameter other)
        {
            return this.parameterName == other.parameterName && this.parameterType == other.parameterType;
        }

        /// <summary>
        /// Check if matches given ParameterInfo.
        /// </summary>
        /// <param name="otherInfo"></param>
        /// <returns></returns>
        public bool Equals(ParameterInfo otherInfo)
        {
            return this.parameterName == otherInfo.Name && this.parameterType == otherInfo.ParameterType.FullName;
        }

        /// <summary>
        /// Return System Type from string.
        /// </summary>
        /// <returns></returns>
        public System.Type Type()
        {
            System.Type pType;
            pType = System.Type.GetType(this.parameterType, false);

            if (pType == null || pType.ToString().Length == 0)
                pType = System.Type.GetType(this.parameterType + ", UnityEngine", false);
            return pType;
        }
    }
}