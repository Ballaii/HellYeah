using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Eliot.AgentComponents;
using Eliot.Utility;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityEngine;

namespace Eliot.BehaviourEditor
{
    /// <summary>
    /// Object that holds the information of a utility function.
    /// </summary>
    [Serializable] public class UtilityCurve
    {
        /// <summary>
        /// The animation curve that defines a function.
        /// </summary>
        [SerializeField] public AnimationCurve Curve = AnimationCurve.Linear(0,0,1,1);
        
        /// <summary>
        /// Displayed color of the curve.
        /// </summary>
        [SerializeField] public Color Color = Color.green;
        
        /// <summary>
        /// Name of the function defined by the curve.
        /// </summary>
        public string Name = "Utility Curve";
        
        /// <summary>
        /// Method that the evaluates the input avis value.
        /// </summary>
        public MethodData MethodData = new MethodData();
        
        /// <summary>
        /// MethodInfo that the evaluates the input avis value.
        /// </summary>
        public MethodInfo methodInfo;

        /// <summary>
        /// List of transitions associated with the curve.
        /// </summary>
        public List<NodesTransition> Transitions = new List<NodesTransition>();

        /// <summary>
        /// Index of the group of actions to which the executed method belongs.
        /// </summary>
        public int ActionGroupIndex = 0;

        /// <summary>
        /// Index of the captured function in the group.
        /// </summary>
        public int FuncIndex = 0;

        /// <summary>
        /// Captured method name.
        /// </summary>
        public string FunctionName = "";

        /// <summary>
        /// Editor only. Whether to display the points to adjust them manually instead of doing so with the curve editor.
        /// </summary>
        private bool foldoutPoints = false;

#if UNITY_EDITOR
        /// <summary>
        /// The list of keys.
        /// </summary>
        private ReorderableList reorderableListKeys;
#endif
        
        /// <summary>
        /// Temporary list.
        /// </summary>
        private List<Keyframe> tempKeyframes = new List<Keyframe>();

        /// <summary>
        /// Build a new Utility Curve object.
        /// </summary>
        public UtilityCurve()
        {
            Curve = AnimationCurve.Linear(0,0,1,1);
            Curve.preWrapMode = WrapMode.ClampForever;
            Curve.postWrapMode = WrapMode.ClampForever;
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Build the reorderable list of keyframes.
        /// </summary>
        /// <returns></returns>
        private ReorderableList GetReorderableListKeys()
        {
            if(reorderableListKeys == null){
                reorderableListKeys = new ReorderableList(tempKeyframes, 
                typeof(Keyframe), 
                true, true, true, true);
                
                reorderableListKeys.drawHeaderCallback = (rect)=>{EditorGUI.LabelField(rect, "Keyframes");};
                reorderableListKeys.onRemoveCallback = (list)=>
                {
                    var newKeyframes = new List<Keyframe>();
                    for(int i = 0; i < tempKeyframes.Count; i++){
                        if(i != list.index)
                        {
                            newKeyframes.Add(tempKeyframes[i]);
                        }                   
                    }
                    tempKeyframes = newKeyframes;
                    list.list = tempKeyframes;
                };
                reorderableListKeys.onAddCallback = (list)=>{
                    var newKeyframes = new List<Keyframe>();
                    for(int i = 0; i < Curve.keys.Length; i++)
                            newKeyframes.Add(Curve.keys[i]);     
                    var newKeyframe = new Keyframe(tempKeyframes.Last().time, tempKeyframes.Last().value);
                    newKeyframe.inTangent = tempKeyframes.Last().inTangent;
                    newKeyframe.outTangent = tempKeyframes.Last().outTangent;
#if UNITY_2019
                    newKeyframe.inWeight = tempKeyframes.Last().inWeight;
                    newKeyframe.outWeight = tempKeyframes.Last().outWeight;
#endif
                    newKeyframes.Add(newKeyframe);
                    tempKeyframes = newKeyframes;
                    list.list = tempKeyframes;
                };
                reorderableListKeys.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width / 6, rect.height), "Value");
                   var time = EditorGUI.FloatField(new Rect(rect.x + rect.width / 6,
                        rect.y, rect.width / 3, rect.height), tempKeyframes[index].time);
                    EditorGUI.LabelField(
                        new Rect(rect.x + rect.width / 6 + rect.width / 3, rect.y,
                            rect.width / 6, rect.height), "Utility");
                    var value = EditorGUI.FloatField(new Rect(rect.x + rect.width / 3 + rect.width / 3,
                        rect.y, rect.width / 3, rect.height), tempKeyframes[index].value);
                    var newKeyframe = new Keyframe(time, value);
                    newKeyframe.inTangent = tempKeyframes.Last().inTangent;
                    newKeyframe.outTangent = tempKeyframes.Last().outTangent;
#if UNITY_2019
                    newKeyframe.inWeight = tempKeyframes.Last().inWeight;
                    newKeyframe.outWeight = tempKeyframes.Last().outWeight;
#endif
                    tempKeyframes[index] = newKeyframe;
                };
            }
            return reorderableListKeys;
        }

        /// <summary>
        /// Render the curve inside the node.
        /// </summary>
        /// <param name="nodeRect"></param>
        /// <param name="padding"></param>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        public void Render(Rect nodeRect, int padding, float minX, float maxX, float minY, float maxY)
        {
            try
            {
                var c = Handles.color;
                List<Vector3> points = new List<Vector3>();
                var scaleX = (nodeRect.width - padding) / (maxX-minX);
                var scaleY = (nodeRect.height - padding) / (maxY-minY);
                foreach (var key in Curve.keys)
                {
                    float x = nodeRect.x + padding * 0.5f + (key.time - minX) * scaleX;
                    float y = nodeRect.y + nodeRect.height - padding * 0.5f - (key.value-minY) * scaleY + 2f;
                    var v = new Vector3(x, y, 0);
                    points.Add(v);
                }

                Handles.color = Color;

                #region LEFT_HANDLE
                {
                    var delta = (Curve.keys[0].time - minX) * scaleX;
                    if (delta > 0.01f)
                    {
                        var v = points[0];
                        v.x -= delta;
                        var bPoints = Handles.MakeBezierPoints(v, points[0], points[0], points[0], 20);
                        Handles.DrawAAPolyLine(2f, bPoints);
                    }
                }
                #endregion
                for (int i = 1; i < points.Count; i++)
                {
                    var normalizer = 0.0f;
                    var inT = points[i - 1] + new Vector3(scaleX * normalizer * -1, scaleY * normalizer * -1, 0) * Curve.keys[i - 1].inTangent;
                    var outT = points[i] + new Vector3(scaleX * normalizer, scaleY * normalizer, 0) * Curve.keys[i].outTangent;
                    var bPoints = Handles.MakeBezierPoints(points[i - 1], points[i], inT, outT, 20);
                    Handles.DrawAAPolyLine(2f, bPoints);
                }
                #region RIGHT_HANDLE

                {
                    var lastIndex = Curve.keys.Length - 1;
                    var delta = (maxX - Curve.keys[lastIndex].time) * scaleX;
                    if (delta > 0.01f)
                    {
                        var v = points[lastIndex];
                        v.x += delta;
                        var bPoints = Handles.MakeBezierPoints(points[lastIndex], v, points[lastIndex], points[lastIndex], 20);
                        Handles.DrawAAPolyLine(2f, bPoints);
                    }
                }
                #endregion

                Handles.color = c;
            }
            catch (System.Exception)
            {
                
            }
        }

        /// <summary>
        /// Draw the associated transitions.
        /// </summary>
        public void UpdateTransitions()
        {
            if (Transitions.Count <= 0) return;
            foreach (var transition in Transitions)
            {
                if (!transition) continue;
                transition.Color = Color;
                transition.Draw();
            }
        }

        /// <summary>
        /// Draw the curve's inspector.
        /// </summary>
        public void DrawInspector()
        {
            var actionGroupsT = EliotReflectionUtility.GetDirectExtentions<UtilityInterface>();
            var actionGroups = new List<string>();
            foreach (var interf in actionGroupsT)
            {
                var interfaceName = interf.Name;
                if (Regex.IsMatch(interfaceName, "UtilityInterface"))
                    actionGroups.Add(interfaceName.Replace("UtilityInterface", ""));
                else
                    actionGroups.Add(interfaceName);
            }
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            ActionGroupIndex = EditorGUILayout.Popup("Utility Group", ActionGroupIndex, actionGroups.ToArray());
            var selectedType = actionGroupsT[ActionGroupIndex];
            var options = EliotReflectionUtility.GetFunctions(selectedType);
            
            FuncIndex = EditorGUILayout.Popup("Function", FuncIndex, options);
            EditorGUILayout.EndVertical(); 
            
            FunctionName = options[FuncIndex];
            methodInfo = EliotReflectionUtility.GetMethodInfoFromMethodName(selectedType, FunctionName);
            MethodData.GetMethodData(methodInfo);
            MethodData.DrawInspector();
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EliotEditorUtility.NavigateToCodeButton(methodInfo);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            Color = EditorGUILayout.ColorField("Curve Color", Color);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            Curve = EditorGUILayout.CurveField("Utility Curve", Curve);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUI.indentLevel++;
            foldoutPoints = EditorGUILayout.Foldout(foldoutPoints, "Keyframes");
            if (foldoutPoints)
            {
                tempKeyframes = Curve.keys.ToList();
                GetReorderableListKeys().DoLayoutList();
                Curve.keys = tempKeyframes.ToArray();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
#endif
    }
}