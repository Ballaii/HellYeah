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
	/// Node that represents the Action Component of the behaviour model.
	/// </summary>
	public class ActionNode : Node
	{
		/// <summary>
		/// Keeps the data about hte method to be executed.
		/// </summary>
		public MethodData MethodData;

		/// <summary>
		/// The index of the currently captured group of actions.
		/// </summary>
		public int ActionGroupIndex = 0;
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public ActionNode(){}
		
		/// <summary>
		/// Initialize the ActionNode.
		/// </summary>
		/// <param name="rect"></param>
		public ActionNode(Rect rect) : base(rect,  "Action"){}

#if UNITY_EDITOR
		/// <summary>
		/// Update the node's functionality.
		/// </summary>
		public override void Update()
		{
			if (Transitions == null || Transitions.Count <= 0) return;
			foreach (var transition in Transitions)
			{
				if (!transition) continue;
				transition.Draw();
			}
		}
		
		/// <summary>
		/// Draw the context menu of the node.
		/// </summary>
		public override void DrawMenu()
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Transition"), false, StartTransition, null);
			menu.AddSeparator("");
			menu.AddItem(new GUIContent("Delete"), false, Delete, null);
			menu.ShowAsContext();
		}

		/// <summary>
		/// Initialize the transition starting from this node.
		/// </summary>
		/// <param name="obj"></param>
		private void StartTransition(object obj){
			Window.StartTransition(Rect, this, EliotGUISkin.NeutralColor);
		}

		/// <summary>
		/// Draw the node.
		/// </summary>
		public override void DrawContent()
		{
			GUILayout.Label(Func,EliotGUISkin.GetLabelStyle(Func));
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
			if (TempBindedCoreComponent != null)
				return TempBindedCoreComponent;
			
			var component = gameObject.GetComponent<EliotAgent>().AddInterface(MethodData.FullClassName);
			var metInfo = component.GetType().GetMethod(MethodData.FullMethodName, BindingFlags.Public | BindingFlags.Instance);
			if (metInfo == null) return null;
			var numParams = metInfo.GetParameters().Length;
			Action action = null;
			if(numParams == 0)
			{
				var systemAction = (System.Action) System.Delegate.CreateDelegate(typeof(System.Action), component, metInfo, true);
				action = new Action(systemAction, component.PreUpdateCallback, component.PostUpdateCallback);
			}
			else if(numParams == 1)
			{
				var pType = MethodData.MethodParameters[0].Type();
				
				var param = MethodData.GetParameters()[0];
				
				if(pType == typeof(bool)){
					
					var systemAction = (System.Action<bool>) System.Delegate.CreateDelegate(typeof(System.Action<bool>), component, metInfo, true);
					action = new Action(systemAction, (bool)param, component.PreUpdateCallback, component.PostUpdateCallback);
				}
				else if(pType == typeof(int)){
					var systemAction = (System.Action<int>) System.Delegate.CreateDelegate(typeof(System.Action<int>), component, metInfo, true);
					action = new Action(systemAction, (int)param, component.PreUpdateCallback, component.PostUpdateCallback);
				}
				else if(pType == typeof(float)){
					var systemAction = (System.Action<float>) System.Delegate.CreateDelegate(typeof(System.Action<float>), component, metInfo, true);
					action = new Action(systemAction, (float)param, component.PreUpdateCallback, component.PostUpdateCallback);
				}
				else if(pType == typeof(string)){
					var systemAction = (System.Action<string>) System.Delegate.CreateDelegate(typeof(System.Action<string>), component, metInfo, true);
					action = new Action(systemAction, (string)param, component.PreUpdateCallback, component.PostUpdateCallback);
				}
				else
				{
					
					action = new Action(MethodData, gameObject, component.PreUpdateCallback, component.PostUpdateCallback);
				}
			}
			else
			{
				action = new Action(MethodData, gameObject, component.PreUpdateCallback, component.PostUpdateCallback);
			}

			action.id = NodeId;
			action.CaptureControl = CaptureControl;
			action.Core = core;
			core.Elements.Add(action);
			TempBindedCoreComponent = action;
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
			if (BindedCoreComponent != null) return;
			
			BindedCoreComponent = coreComponent;
			if (Transitions.Count > 0)
			{
				var index = 0;
				for (int i = 0; i < Transitions.Count; i++)
					for (int j = 0; j < Transitions[i].TransitionsData.Count; j++)
						Transitions[i].TransitionsData[j].BindToCoreTransition((coreComponent as Action).Transitions[index++]);
			}
		}
#endif
	}
}