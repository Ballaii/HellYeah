#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Eliot.AgentComponents;
using Eliot.BehaviourEditor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Eliot.Utility
{
    /// <summary>
    /// Extension class for creating new gui elements.
    /// </summary>
    public static class EliotEditorUtility
    {
        /// <summary>
        /// Cache for copy-pasting agent components values.
        /// </summary>
        public static string LastCopiedAgentComponent;

        #region Draw GUI
        /// <summary>
        /// Draw Inspector GUI for a Parameter.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public static object DisplayParameter(Parameter parameter, string space = "")
        {
            System.Type pType = parameter.Type();
                
            if (pType == typeof(bool))
            {
                if (parameter.parameterValue == null)
                {
                    parameter.parameterValue = default(bool).ToString();
                }

                bool res;
                bool.TryParse(parameter.parameterValue, out res);
                var tmp = EditorGUILayout.Toggle(ObjectNames.NicifyVariableName(space + parameter.parameterName), res);
                parameter.parameterValue = tmp.ToString();

            }
            else if (pType == typeof(int))
            {
                if (parameter.parameterValue == null)
                {
                    parameter.parameterValue = default(int).ToString();
                }
                int res;
                int.TryParse(parameter.parameterValue, out res);
                var tmp = EditorGUILayout.IntField(ObjectNames.NicifyVariableName(space + parameter.parameterName), res);
                parameter.parameterValue = tmp.ToString();
                
            }
            else if (pType == typeof(float))
            {
                if (parameter.parameterValue == null)
                {
                    parameter.parameterValue = default(float).ToString();
                }
                float res;
                float.TryParse(parameter.parameterValue, out res);
                var tmp = EditorGUILayout.FloatField(ObjectNames.NicifyVariableName(space + parameter.parameterName), res);
                parameter.parameterValue = tmp.ToString();
                
            }
            else if (pType == typeof(string))
            {
                if (parameter.parameterValue == null)
                {
                    parameter.parameterValue = default(string);
                }
                string res = parameter.parameterValue;
                var tmp = EditorGUILayout.TextField(ObjectNames.NicifyVariableName(space + parameter.parameterName), res);
                parameter.parameterValue = tmp;
                
            }
            else if (pType == typeof(UnityEngine.Object) || pType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                parameter.parameterObjectValue = EditorGUILayout.ObjectField(
                    ObjectNames.NicifyVariableName(space + parameter.parameterName), parameter.parameterObjectValue, pType, false);
            }

            return parameter.parameterValue;
        }

        /// <summary>
        /// Draw Inspector GUI for a Parameter.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="parameter"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public static object DisplayParameter(Rect rect, Parameter parameter, string space = "")
        {
            System.Type pType;
            pType = Type.GetType(parameter.parameterType, false);

            if(pType == null || pType.ToString().Length == 0)
                pType = Type.GetType(parameter.parameterType + ", UnityEngine", false);
                
            if (pType == typeof(bool))
            {
                if (parameter.parameterValue == null)
                {
                    parameter.parameterValue = default(bool).ToString();
                }

                bool res;
                bool.TryParse(parameter.parameterValue, out res);
                var tmp = EditorGUI.Toggle(rect, ObjectNames.NicifyVariableName(space + parameter.parameterName), res);
                parameter.parameterValue = tmp.ToString();

            }
            else if (pType == typeof(int))
            {
                if (parameter.parameterValue == null)
                {
                    parameter.parameterValue = default(int).ToString();
                }
                int res;
                int.TryParse(parameter.parameterValue, out res);
                var tmp = EditorGUI.IntField(rect, ObjectNames.NicifyVariableName(space + parameter.parameterName), res);
                parameter.parameterValue = tmp.ToString();
                
            }
            else if (pType == typeof(float))
            {
                if (parameter.parameterValue == null)
                {
                    parameter.parameterValue = default(float).ToString();
                }
                float res;
                float.TryParse(parameter.parameterValue, out res);
                var tmp = EditorGUI.FloatField(rect, ObjectNames.NicifyVariableName(space + parameter.parameterName), res);
                parameter.parameterValue = tmp.ToString();
                
            }
            else if (pType == typeof(string))
            {
                if (parameter.parameterValue == null)
                {
                    parameter.parameterValue = default(string);
                }
                string res = parameter.parameterValue;
                var tmp = EditorGUI.TextField(rect, ObjectNames.NicifyVariableName(space + parameter.parameterName), res);
                parameter.parameterValue = tmp;
                
            }
            else if (pType == typeof(UnityEngine.Object) || pType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                parameter.parameterObjectValue = EditorGUI.ObjectField(rect,
                    ObjectNames.NicifyVariableName(space + parameter.parameterName), parameter.parameterObjectValue, pType, false);
            }

            return parameter.parameterValue;
        }
        
        /// <summary>
        /// Create a button that opens IDE and navigates to the provided method.
        /// </summary>
        /// <param name="methodInfo"></param>
        public static void NavigateToCodeButton(MethodInfo methodInfo)
        {
            if (GUILayout.Button("Navigate To Code"))
            {
                string[] fileNames = Directory.GetFiles(Application.dataPath, methodInfo.DeclaringType.Name + ".cs", SearchOption.AllDirectories);
					
                if (fileNames.Length > 0)
                {
                    string finalFileName = Path.GetFullPath(fileNames[0]);
                    int gotoLine = 0;
                    string line;
                    System.IO.StreamReader file = new System.IO.StreamReader(finalFileName);  
                    while((line = file.ReadLine()) != null)  
                    {
                        if (Regex.IsMatch(line, methodInfo.Name + "()"))
                        {
                            gotoLine++;
                            break;
                        }
                        gotoLine++;
                    }  
  
                    file.Close(); 
                    foreach (var lAssetPath in AssetDatabase.GetAllAssetPaths())
                    {
                        if (lAssetPath.EndsWith(methodInfo.DeclaringType.Name + ".cs"))
                        {
                            var lScript = (MonoScript)AssetDatabase.LoadAssetAtPath(lAssetPath, typeof(MonoScript));
                            if (lScript != null)
                            {
                                AssetDatabase.OpenAsset(lScript, gotoLine);
                                break;
                            }
                        }
                    }
                } else {
                    Debug.Log("File Not Found:" + methodInfo.DeclaringType.Name + ".cs");
                }
            }
        }
        
        /// <summary>
        /// Create a button that opens IDE and navigates to the provided method.
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="rect"></param>
        public static void NavigateToCodeButton(MethodInfo methodInfo, Rect rect)
        {
            if (GUI.Button(rect, "Navigate To Code"))
            {
                string[] fileNames = Directory.GetFiles(Application.dataPath, methodInfo.DeclaringType.Name + ".cs", SearchOption.AllDirectories);
					
                if (fileNames.Length > 0)
                {
                    string finalFileName = Path.GetFullPath(fileNames[0]);
                    int gotoLine = 0;
                    string line;
                    System.IO.StreamReader file = new System.IO.StreamReader(finalFileName);  
                    while((line = file.ReadLine()) != null)  
                    {
                        if (Regex.IsMatch(line, methodInfo.Name + "()"))
                        {
                            gotoLine++;
                            break;
                        }
                        gotoLine++;
                    }  
  
                    file.Close(); 
                    Debug.Log(methodInfo.DeclaringType.Name + "\t" + gotoLine);
                    foreach (var lAssetPath in AssetDatabase.GetAllAssetPaths())
                    {
                        if (lAssetPath.EndsWith(methodInfo.DeclaringType.Name + ".cs"))
                        {
                            var lScript = (MonoScript)AssetDatabase.LoadAssetAtPath(lAssetPath, typeof(MonoScript));
                            if (lScript != null)
                            {
                                AssetDatabase.OpenAsset(lScript, gotoLine);
                                break;
                            }
                        }
                    }
                } else {
                    Debug.Log("File Not Found:" + methodInfo.DeclaringType.Name + ".cs");
                }
            }
        }
        
        /// <summary>
        /// Draw the default header with a label and a script reference.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="target"></param>
        /// <param name="style"></param>
        /// <typeparam name="T"></typeparam>
        public static void DrawHeader<T>(string title, UnityEngine.Object target, [CanBeNull] GUIStyle style = null) where T : MonoBehaviour
        {
            if (style == null) style = EliotGUISkin.BlueBackground;
            EditorGUILayout.BeginVertical(style);
            EditorGUILayout.LabelField(title, EditorStyles.whiteMiniLabel);
            EditorGUILayout.EndVertical();
        
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(target as T), typeof(T), false);
            GUI.enabled = true;
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// Draw a white mini label.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="style"></param>
        public static void DrawMiniLabel(string title, [CanBeNull] GUIStyle style = null) 
        {
            if (style == null) style = EliotGUISkin.BlueBackground;
            EditorGUILayout.BeginVertical(style);
            EditorGUILayout.LabelField(title, EditorStyles.whiteMiniLabel);
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// Draw the default foldout header for an agent component.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="style"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool DrawAgentComponentHeader<T>(string title, bool value, UnityEngine.Object target, [CanBeNull] GUIStyle style = null) where T : AgentComponent
        {
            if (style == null) style = EliotGUISkin.BlueBackground;
            EditorGUILayout.BeginVertical(style);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(title + (value ? " -" : " +"), EditorStyles.whiteMiniLabel))
            {
                value = !value;
            }
            
            if (GUILayout.Button("copy", EditorStyles.toolbarButton, GUILayout.MaxWidth(40)))
            {
                LastCopiedAgentComponent = JsonUtility.ToJson(target);
            }
            if (GUILayout.Button("paste", EditorStyles.toolbarButton, GUILayout.MaxWidth(40)))
            {
                try
                {
                    JsonUtility.FromJsonOverwrite(LastCopiedAgentComponent, target);
                    (target as AgentComponent).OnEnable();
                }
                catch (Exception){ }
            }
            if (GUILayout.Button("x", EditorStyles.toolbarButton, GUILayout.MaxWidth(20)))
            {
                (target as AgentComponent).gameObject.GetComponent<EliotAgent>().RemoveComponent<T>();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            if (value)
            {
                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(target as T), typeof(T), false);
                GUI.enabled = true;
                EditorGUILayout.EndVertical();
            }
            return value;
        }
        #endregion
    }
}
#endif