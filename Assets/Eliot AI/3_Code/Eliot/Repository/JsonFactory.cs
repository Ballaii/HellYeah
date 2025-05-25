using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eliot.AgentComponents;
using Eliot.BehaviourEditor;
using UnityEditor;
using UnityEngine;

namespace Eliot.Repository
{
    /// <summary>
    /// Constructs JsonObject and valid Json strings.
    /// </summary>
    [System.Obsolete] public static class JsonFactory
    {
        private static MethodInfo GetMethodInfoFromJson<T>(int functionId)
        {
            List<Type> types = EliotReflectionUtility.GetExtentions<T>();
            List<MethodInfo> methods = new List<MethodInfo>();
            foreach(var obj in types)
            {
                var localMethodsList = obj.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach(var method in localMethodsList)
                    methods.Add(method);
            }
            var filtered = from method in methods
                where method.GetCustomAttributes(typeof(IncludeInBehaviour), false).Any()
                select method;
            var sortableArray = filtered.ToArray();
            Array.Sort(sortableArray, delegate(MethodInfo mi1, MethodInfo mi2) {
                return String.Compare(mi1.Name, mi2.Name, StringComparison.InvariantCulture);
            });

            return sortableArray.ToList()[functionId];
        }
   
        #region CONVERT JSON TO ELIOT OBJECTS
#if UNITY_EDITOR
        /// <summary>
        /// Create EntryNode from json string.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="jObj"></param>
        /// <returns></returns>
        private static Node Serialize(this EntryNode node, JsonObject jObj)
        {
            node.NodeName = jObj["type"].String;
            node.Window = EditorWindow.GetWindow<BehaviourEditorWindow>("Behaviour");
            var x = jObj["rect"]["posX"].Float;
            var y = jObj["rect"]["posY"].Float;
            var w = jObj["rect"]["width"].Float;
            var h = jObj["rect"]["height"].Float;
            node.Rect = new Rect(x, y, w, h);
			
            return node;
        }
        
        /// <summary>
        /// Create ActionNode from json string.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="jObj"></param>
        /// <returns></returns>
        private static Node Serialize(this ActionNode node, JsonObject jObj)
        {
            node.NodeName = jObj["type"].String;
            node.Window = EditorWindow.GetWindow<BehaviourEditorWindow>("Behaviour");
            
            var actionGroup = jObj["actionGroup"].String;
            var functionId = jObj["functionID"].Int;
            node.MethodData = new MethodData();
            
            if (actionGroup == "Inventory")
            {
                node.MethodData.GetMethodData(GetMethodInfoFromJson<InventoryActionInterface>(functionId)); 
            }
            else
            {
                node.MethodData.GetMethodData(GetMethodInfoFromJson<MotionActionInterface>(functionId)); 
            }
            
            node.FuncNames = new [] {node.MethodData.FullMethodName};
            
            var x = jObj["rect"]["posX"].Float;
            var y = jObj["rect"]["posY"].Float;
            var w = jObj["rect"]["width"].Float;
            var h = jObj["rect"]["height"].Float;
            node.Rect = new Rect(x, y, w, h);
			
            return node;
        }
        
        /// <summary>
        /// Create ConditionNode from json string.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="jObj"></param>
        /// <returns></returns>
        private static Node Serialize(this ConditionNode node, JsonObject jObj)
        {
            node.NodeName = jObj["type"].String;
            node.Window = EditorWindow.GetWindow<BehaviourEditorWindow>("Behaviour");
            var conditionGroup = jObj["conditionGroup"].String;
            var functionId = jObj["functionID"].Int;
            node.MethodData = new MethodData();
            
            if (conditionGroup == "Perception")
            {
                node.MethodData.GetMethodData(GetMethodInfoFromJson<PerceptionConditionInterface>(functionId));
            }
            else if (conditionGroup == "Motion")
            {
                node.MethodData.GetMethodData(GetMethodInfoFromJson<MotionConditionInterface>(functionId));
            }
            else if (conditionGroup == "Inventory")
            {
                node.MethodData.GetMethodData(GetMethodInfoFromJson<InventoryConditionInterface>(functionId));
            }
            else if (conditionGroup == "General")
            {
                node.MethodData.GetMethodData(GetMethodInfoFromJson<GeneralConditionInterface>(functionId));
            }
            else if (conditionGroup == "Resources")
            {
                node.MethodData.GetMethodData(GetMethodInfoFromJson<ResourcesConditionInterface>(functionId));
            }

            node.FuncNames = new [] {node.MethodData.FullMethodName};
            
            node.Reverse = jObj["reverse"]!=null && jObj["reverse"].Bool;
            var x = jObj["rect"]["posX"].Float;
            var y = jObj["rect"]["posY"].Float;
            var w = jObj["rect"]["width"].Float;
            var h = jObj["rect"]["height"].Float;
            node.Rect = new Rect(x, y, w, h);
			
            return node;
        }
        
        /// <summary>
        /// Create LoopNode from json string.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="jObj"></param>
        /// <returns></returns>
        private static Node Serialize(this LoopNode node, JsonObject jObj)
        {
            node.NodeName = jObj["type"].String;
            node.Window = EditorWindow.GetWindow<BehaviourEditorWindow>("Behaviour");
            var conditionGroup = jObj["conditionGroup"].String;
            var functionId = jObj["functionID"].Int;
            node.MethodData = new MethodData();
            
            try
            { node.FunctionName = jObj["functionName"].String; }
            catch (Exception){}
            
            if (conditionGroup == "Perception")
            {
                node.MethodData.GetMethodData(GetMethodInfoFromJson<PerceptionConditionInterface>(functionId));
            }
            else if (conditionGroup == "Motion")
            {
                node.MethodData.GetMethodData(GetMethodInfoFromJson<MotionConditionInterface>(functionId));
            }
            else if (conditionGroup == "Inventory")
            {
                node.MethodData.GetMethodData(GetMethodInfoFromJson<InventoryConditionInterface>(functionId));
            }
            else if (conditionGroup == "General")
            {
                node.MethodData.GetMethodData(GetMethodInfoFromJson<GeneralConditionInterface>(functionId));
            }
            else if (conditionGroup == "Resources")
            {
                node.MethodData.GetMethodData(GetMethodInfoFromJson<ResourcesConditionInterface>(functionId));
                
            }
            node.FuncNames = new [] {node.MethodData.FullMethodName};

            node.Reverse = jObj["reverse"]!=null && jObj["reverse"].Bool;
            var x = jObj["rect"]["posX"].Float;
            var y = jObj["rect"]["posY"].Float;
            var w = jObj["rect"]["width"].Float;
            var h = jObj["rect"]["height"].Float;
            node.Rect = new Rect(x, y, w, h);
			
            return node;
        }
        
        /// <summary>
        /// Create Node from json string.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="jObj"></param>
        /// <returns></returns>
        public static Node Serialize(this Node node, JsonObject jObj)
        {
            if (jObj["type"].String == "Entry") return ((EntryNode) node).Serialize(jObj);
            if (jObj["type"].String == "Action")
            {
                var actionGroup = jObj["actionGroup"].String;
                if (actionGroup == "Skill")
                {
                    node = ScriptableObject.CreateInstance<SkillNode>();
                    node.NodeName = "Skill";
                    node.FuncIndex = 0;
                    node.FuncNames = new[] { jObj["functionID"].String };
                    try
                    { node.FunctionName = jObj["functionName"].String; }
                    catch (Exception){}
                
                    if(jObj["executeSkill"] != null)
                        (node as SkillNode).ExecuteSkill = jObj["executeSkill"].Bool;

                    var guid = AssetDatabase.FindAssets(node.FunctionName + " t:Skill");

                    if (guid.Length > 0)
                    {
                        var skillPath = AssetDatabase.GUIDToAssetPath(guid[0]);
                        var skillFile = AssetDatabase.LoadAssetAtPath<Skill>(skillPath);
                        (node as SkillNode).Skill = skillFile;
                    }

                    var x = jObj["rect"]["posX"].Float;
                    var y = jObj["rect"]["posY"].Float;
                    var w = jObj["rect"]["width"].Float;
                    var h = jObj["rect"]["height"].Float;
                    node.Rect = new Rect(x, y, w, h);
                    return node;
                }
                return ((ActionNode) node).Serialize(jObj);
            }
            if (jObj["type"].String == "Condition") return ((ConditionNode) node).Serialize(jObj);
            if (jObj["type"].String == "Loop")
            {
                var conditionGroup = jObj["conditionGroup"].String;
                if (conditionGroup == "Time")
                {
                    node = ScriptableObject.CreateInstance<TimeNode>();
                    node.NodeName = "Time";
                    (node as TimeNode).MinTime = jObj["minTime"].Float;
                    (node as TimeNode).MaxTime = jObj["maxTime"].Float;
                    return node;
                }
                return ((LoopNode) node).Serialize(jObj);
            }
            return null;
        }

        /// <summary>
        /// Create all the Behaviour objects from its json string.
        /// </summary>
        /// <param name="behaviour"></param>
        public static void Serialize(this EliotBehaviour behaviour)
        {
            behaviour.Nodes = new List<Node>();
            var jObj = new JsonObject(behaviour.Json);
            var jNodes = jObj["nodes"].Objects;
            
            if (jNodes == null)
            {
                behaviour.Nodes = new List<Node>();
                behaviour.Transitions = new List<NodesTransition>();
            }
            else
            {
                if (jNodes.Count > 0)
                    foreach (var node in jNodes)
                        behaviour.Nodes.Add(BehaviourEditorWindow.TemplateNode(((JsonObject)node)["type"].String).Serialize((JsonObject)node));

                behaviour.Transitions = new List<NodesTransition>();
                var jTransitions = jObj["transitions"].Objects;
                foreach (var trans in jTransitions)
                {
                    var transition = (JsonObject) trans;
                    if (transition["startID"] == null) break;
                    var startId = transition["startID"].Int;
                    var endId = transition["endID"].Int;
                    var isNeg = transition["type"].String.Equals("negative");
                    int minRate, maxRate;
                    float minCooldown, maxCooldown;
                    bool terminate;
                    try
                    {
                        minRate = transition["minRate"].Int;
                        maxRate = transition["maxRate"].Int;
                        minCooldown = transition["minCooldown"].Float;
                        maxCooldown = transition["maxCooldown"].Float;
                        terminate = transition["terminate"].Bool;
                    }
                    catch (Exception)
                    {
                        minRate = 1;
                        maxRate = 1;
                        minCooldown = 0f;
                        maxCooldown = 0f;
                        terminate = false;
                    }

                    var color = EliotGUISkin.NeutralColor;
                    var tColor = transition["color"];
                    if (tColor != null)
                    {
                        var r = transition["color"]["r"].Float;
                        var g = transition["color"]["g"].Float;
                        var b = transition["color"]["b"].Float;
                        var a = Mathf.Clamp(1f / (maxRate * 0.2f)+0.2f, 0.5f, 2f);
                        color = new Color(r, g, b, a);
                    }

                    var t = BehaviourEditorWindow.TemplateTransition();
                    t.Start = behaviour.Nodes[startId];
                    t.End = behaviour.Nodes[endId];
                    t.IsNegative = isNeg;
                    var transitionData = new NodesTransitionData(t);
                    transitionData.MinProbability = 100f;
                    transitionData.MaxProbability = 100f;
                    transitionData.MinCooldown = minCooldown;
                    transitionData.MaxCooldown = maxCooldown;
                    transitionData.Terminate = terminate;
                    t.TransitionsData = new List<NodesTransitionData>();
                    t.TransitionsData.Add(transitionData);
                    t.Color = color;
                    behaviour.Nodes[startId].Transitions.Add(t);
                    behaviour.Transitions.Add(t);
                }
                
            }
        }
#endif
        #endregion
    }
}