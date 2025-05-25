using System;
using System.Reflection;
using Eliot.AgentComponents;
using Eliot.BehaviourEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Eliot.BehaviourEditor
{
    /// <summary>
    /// Node that represents the Loop Component of the behaviour model.
    /// </summary>
    public class LoopNode : Node
    {
        [Tooltip("If true, action taken upon checking the condition will be reversed.")]
        public bool Reverse = false;

        /// <summary>
        /// The index of the currently captured group of conditions.
        /// </summary>
        public int ConditionGroupIndex = 0;
        
        /// <summary>
        /// Keeps the data about the method to be executed.
        /// </summary>
        public MethodData MethodData;
        
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public LoopNode(){}
        
        /// <summary>
        /// Initialize the LoopNode.
        /// </summary>
        /// <param name="rect"></param>
        public LoopNode(Rect rect) : base(rect, "Loop"){}

#if UNITY_EDITOR
        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        public new void OnEnable()
        {
            base.OnEnable();
            if(this.MethodData == null)  MethodData = new MethodData();
        }
        
        /// <summary>
        /// Update the node's functionality.
        /// </summary>
        public override void Update()
        {
            if (Transitions.Count <= 0) return;
            foreach (var transition in Transitions)
            {
                transition.Draw();
            }
        }

        /// <summary>
        /// Draw the node.
        /// </summary>
        public override void DrawContent()
        {
            //var col = ColorUtility.ToHtmlStringRGBA(EliotGUISkin.LoopColor);
            //GUILayout.Label( "<size=12><b><color=#" + col + ">while " + (Reverse ? "!" : "") + "</color></b></size>" + NormalSized(Func));
            GUILayout.Label("While " + (Reverse ? "! " : " ") + Func, new GUIStyle(EliotGUISkin.GetLabelStyle(Func)) { normal = { textColor = EliotGUISkin.LoopColor } });
        }

        /// <summary>
        /// Draw the context menu of the node.
        /// </summary>
        public override void DrawMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Transition/While"), false, StartTransition, true);
            menu.AddItem(new GUIContent("Transition/End of loop"), false, StartTransition, false);
            menu.AddItem(new GUIContent("Delete"), false, Delete, null);
            menu.ShowAsContext();
        }
        
        /// <summary>
        /// Initialize the transition starting from this node.
        /// </summary>
        /// <param name="obj"></param>
        private void StartTransition(object obj){
            if ((bool)obj)
                Window.StartTransition(Rect, this, EliotGUISkin.LoopColor);
            else Window.StartTransition(Rect, this, EliotGUISkin.NeutralColor, true);
        }
#endif
        
        /// <summary>
        /// Build the behaviour engine component from the input data.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="core"></param>
        /// <returns></returns>
        public override CoreComponent GetBehaviourEngineComponent(GameObject gameObject, BehaviourCore core)
        {
            if (TempBindedCoreComponent != null) return TempBindedCoreComponent;
            var component = gameObject.GetComponent<EliotAgent>().AddInterface(MethodData.FullClassName);
            if (component == null)
            {
                return null;
            }
            var metInfo = component.GetType().GetMethod(MethodData.FullMethodName, BindingFlags.Public | BindingFlags.Instance);
            if (metInfo == null) return null;
            var numParams = metInfo.GetParameters().Length;
            Loop loop = null;
            if (numParams == 0)
            {
                var condition =
                    (EliotCondition) Delegate.CreateDelegate(typeof(EliotCondition), component, metInfo, true);
                loop = new Loop(condition, component.PreUpdateCallback, component.PostUpdateCallback);
            }
            else if(numParams == 1)
            {
                var pType = MethodData.MethodParameters[0].Type();
				
                var param = MethodData.GetParameters()[0];
				
                if(pType == typeof(bool)){
                    var condition = (EliotCondition<bool>) Delegate.CreateDelegate(typeof(EliotCondition<bool>), component, metInfo, true);
                    loop = new Loop(condition, (bool)param, component.PreUpdateCallback, component.PostUpdateCallback);
                }
                else if(pType == typeof(int)){
                    var condition = (EliotCondition<int>) Delegate.CreateDelegate(typeof(EliotCondition<int>), component, metInfo, true);
                    loop = new Loop(condition, (int)param, component.PreUpdateCallback, component.PostUpdateCallback);
                }
                else if(pType == typeof(float)){
                    var condition = (EliotCondition<float>) Delegate.CreateDelegate(typeof(EliotCondition<float>), component, metInfo, true);
                    loop = new Loop(condition, (float)param, component.PreUpdateCallback, component.PostUpdateCallback);
                }
                else if(pType == typeof(string)){
                    var condition = (EliotCondition<string>) Delegate.CreateDelegate(typeof(EliotCondition<string>), component, metInfo, true);
                    loop = new Loop(condition, (string)param, component.PreUpdateCallback, component.PostUpdateCallback);
                }
                else
                {
                    loop = new Loop(MethodData, gameObject, component.PreUpdateCallback, component.PostUpdateCallback);
                }
            }
            else
            {
                loop = new Loop(MethodData, gameObject, component.PreUpdateCallback, component.PostUpdateCallback);
            }

            loop.id = NodeId;
            loop.CaptureControl = CaptureControl;
            loop.Core = core;
            loop.reverse = Reverse;
            core.Elements.Add(loop);
            TempBindedCoreComponent = loop;
            foreach (var transition in Transitions)
            {
                transition.BuildBehaviourTransition(TempBindedCoreComponent, gameObject, core);
            }

            return TempBindedCoreComponent;
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Associate the node with a runtime behaviour core component for the debugging purposes. 
        /// </summary>
        /// <param name="coreComponent"></param>
        public override void BindToCoreComponent(CoreComponent coreComponent)
        {
            BindedCoreComponent = coreComponent;
            
            if (Transitions.Count > 0)
            {
                int else_i = 0;
                int if_i = 0;
                for (int i = 0; i < Transitions.Count; i++)
                {
                    for (int j = 0; j < Transitions[i].TransitionsData.Count; j++)
                    {
                        if (Transitions[i].IsNegative)
                            Transitions[i].TransitionsData[j].BindToCoreTransition((coreComponent as Loop).transitionsEnd[else_i++]);
                        else
                            Transitions[i].TransitionsData[j].BindToCoreTransition((coreComponent as Loop).transitionsWhile[if_i++]);
                    }
                }
            }
        }
#endif
    }
}




