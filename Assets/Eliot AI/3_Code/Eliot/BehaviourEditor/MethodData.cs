using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Eliot.AgentComponents;
using Eliot.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Eliot.BehaviourEditor
{
    /// <summary>
    /// Holds all the necessary data to save and reconstruct a method info at runtime.
    /// </summary>
    [Serializable] public class MethodData
    {
        /// <summary>
        /// Full name of the class in which the method has been declared.
        /// </summary>
        public string FullClassName;
        
        /// <summary>
        /// Full name of the method.
        /// </summary>
        public string FullMethodName;
        
        /// <summary>
        /// The list of the method parameters.
        /// </summary>
        [SerializeField] public List<Parameter> MethodParameters = new List<Parameter>();
        
        /// <summary>
        /// Reference to the source method info.
        /// </summary>
        private MethodInfo _methodInfo;
        
        /// <summary>
        /// Reference to the eliot interface that keeps the method.
        /// </summary>
        private EliotInterface _eliotInterface;

        /// <summary>
        /// Return whether the method is null.
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(FullMethodName);
        }

        /// <summary>
        /// Return the Eliot interface to which the method belongs.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public EliotInterface GetInterface(GameObject gameObject)
        {
            if (_eliotInterface == null)
            {
                _eliotInterface = gameObject.GetComponent<EliotAgent>().AddInterface(FullClassName);
            }

            return _eliotInterface;
        }

        /// <summary>
        /// Build the method info from the stored data.
        /// </summary>
        /// <returns></returns>
        public MethodInfo BuildMethodInfo()
        {
            MethodInfo metInfo;
            try
            {
                metInfo = Type.GetType(FullClassName)
                    .GetMethod(FullMethodName, BindingFlags.Public | BindingFlags.Instance);
            }
            catch (System.Exception)
            {
                metInfo = EliotReflectionUtility.FirstMethod<ActionInterface>();
            }

            _methodInfo = metInfo;

            return _methodInfo;
        }

        /// <summary>
        /// Execute the method.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public object Invoke(GameObject gameObject)
        {
            var methodInfo = BuildMethodInfo();
            var parameters = GetParameters();
            return methodInfo.Invoke( GetInterface(gameObject), parameters);
        }

        /// <summary>
        /// Get the method parameters list.
        /// </summary>
        /// <returns></returns>
        public object[] GetParameters()
        {
            List<object> result = new List<object>();
            foreach (var p in MethodParameters)
            {
                var pType = p.Type();
                object value;
                if(pType == typeof(UnityEngine.Object) || pType.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    value = p.parameterObjectValue??(default(object));
                }
                else
                {
                    value = Convert.ChangeType(p.parameterValue, Type.GetTypeCode(pType));
                }
                result.Add(value);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Check whether the parameter has already been added.
        /// </summary>
        /// <param name="parameterInfo"></param>
        /// <returns></returns>
        public bool AlreadyExists(ParameterInfo parameterInfo)
        {
            foreach (var p in MethodParameters)
                if (p.Equals(parameterInfo)) 
                    return true;
            return false;
        }
        
        /// <summary>
        /// Extract data from the given method info.
        /// </summary>
        /// <param name="methodInfo"></param>
        public void GetMethodData(MethodInfo methodInfo)
        {
            FullClassName = methodInfo.DeclaringType.FullName;
            FullMethodName = methodInfo.Name;
            if (methodInfo == null) return;
            
            if(methodInfo.GetParameters().Length == 0)
            {
                MethodParameters = new List<Parameter>();
            }
            else
            {
                foreach (var parameter in methodInfo.GetParameters())
                {
                    if(!AlreadyExists(parameter))
                        MethodParameters.Add(new Parameter(parameter));
                }
                var names = from p in methodInfo.GetParameters() select p.Name;
                for(var i = MethodParameters.Count-1; i >= 0; i--)
                {
                    if(!names.Contains(MethodParameters[i].parameterName))
                    {
                        MethodParameters.RemoveAt(i);
                    }
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the inspector for the parameters.
        /// </summary>
        public void DrawInspector()
        {
            if(MethodParameters.Count > 0)
            {
                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                
                EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
            
                for (int i = 0; i < MethodParameters.Count; i++)
                {
                    EliotEditorUtility.DisplayParameter(MethodParameters[i]);
                }
                if(MethodParameters.Count == 1)
                {
                    var pType = MethodParameters[0].Type();
                    if( !(pType == typeof(bool) || pType == typeof(int) || pType == typeof(float) || pType == typeof(string)) )
                        EditorGUILayout.HelpBox("For performance reasons it is recommended to use "
                        + "'bool', 'int', 'float' or 'string' as the method parameter.", MessageType.Warning);
                }
                if(MethodParameters.Count > 1)
                {
                    EditorGUILayout.HelpBox("This method contains more than one parameter. "
                    + "This will make the Node run significantly slower. If you need performance, " 
                    + "try to restrict the number of parameters per method to 1.", MessageType.Warning);
                }
                EditorGUILayout.EndVertical();
            }
        }
#endif
    }
}