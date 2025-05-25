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
	public class ConditionNode : Node
	{
		/// <summary>
		/// Holds the data necessary to execute the condition.
		/// </summary>
		public MethodData MethodData;
		
		/// <summary>
		/// The index of the currently captured group of conditions.
		/// </summary>
		public int ConditionGroupIndex = 0;

		/// <summary>
		/// Reverses the returned by condition boolean.
		/// </summary>
		public bool Reverse = false;

		/// <summary>
		/// Empty constructor.
		/// </summary>
		public ConditionNode()
		{
		}

		/// <summary>
		/// Initialize the ConditionNode.
		/// </summary>
		/// <param name="rect"></param>
		public ConditionNode(Rect rect) : base(rect, "Condition")
		{
		}
#if UNITY_EDITOR
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
		/// Draw the context menu of the node.
		/// </summary>
		public override void DrawMenu()
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Transition/Yes"), false, StartTransition, "yes");
			menu.AddItem(new GUIContent("Transition/No"), false, StartTransition, "no");
			menu.AddSeparator("");
			menu.AddItem(new GUIContent("Delete"), false, Delete, null);
			menu.ShowAsContext();
		}

		/// <summary>
		/// Initialize the transition starting from this node.
		/// </summary>
		/// <param name="obj"></param>
		private void StartTransition(object obj)
		{
			var str = (string) obj;
			switch (str)
			{
				case "yes":
					Window.StartTransition(Rect, this, EliotGUISkin.PositiveColor);
					break;
				case "no":
					Window.StartTransition(Rect, this, EliotGUISkin.NegativeColor, true);
					break;
			}
		}

		/// <summary>
		/// Draw the node.
		/// </summary>
		public override void DrawContent()
		{
			GUILayout.Label((Reverse ? "Not " : "") + Func + "?", new GUIStyle(EliotGUISkin.GetLabelStyle(Func)) { normal = { textColor = EliotGUISkin.PositiveColor } });
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
			{
				return TempBindedCoreComponent;
			}

			var component = gameObject.GetComponent<EliotAgent>().AddInterface(MethodData.FullClassName);
			if (component == null)
			{
				return null;
			}

			var metInfo = component.GetType()
				.GetMethod(MethodData.FullMethodName, BindingFlags.Public | BindingFlags.Instance);
			if (metInfo == null) return null;
			var numParams = metInfo.GetParameters().Length;
			Condition condition = null;
			if (numParams == 0)
			{
				var systemCondition =
					(EliotCondition) Delegate.CreateDelegate(typeof(EliotCondition), component, metInfo, true);
				condition = new Condition(systemCondition, component.PreUpdateCallback, component.PostUpdateCallback);
			}
			else if (numParams == 1)
			{
				var pType = MethodData.MethodParameters[0].Type();

				var param = MethodData.GetParameters()[0];

				if (pType == typeof(bool))
				{
					var systemCondition =
						(EliotCondition<bool>) Delegate.CreateDelegate(typeof(EliotCondition<bool>), component, metInfo,
							true);
					condition = new Condition(systemCondition, (bool) param, component.PreUpdateCallback,
						component.PostUpdateCallback);
				}
				else if (pType == typeof(int))
				{
					var systemCondition =
						(EliotCondition<int>) Delegate.CreateDelegate(typeof(EliotCondition<int>), component, metInfo,
							true);
					condition = new Condition(systemCondition, (int) param, component.PreUpdateCallback,
						component.PostUpdateCallback);
				}
				else if (pType == typeof(float))
				{
					var systemCondition = (EliotCondition<float>) Delegate.CreateDelegate(typeof(EliotCondition<float>),
						component, metInfo, true);
					condition = new Condition(systemCondition, (float) param, component.PreUpdateCallback,
						component.PostUpdateCallback);
				}
				else if (pType == typeof(string))
				{
					var systemCondition = (EliotCondition<string>) Delegate.CreateDelegate(typeof(EliotCondition<string>),
						component, metInfo, true);
					condition = new Condition(systemCondition, (string) param, component.PreUpdateCallback,
						component.PostUpdateCallback);
				}
				else
				{
					condition = new Condition(MethodData, gameObject, component.PreUpdateCallback,
						component.PostUpdateCallback);
				}
			}
			else
			{
				condition = new Condition(MethodData, gameObject, component.PreUpdateCallback,
					component.PostUpdateCallback);
			}

			condition.id = NodeId;
			condition.CaptureControl = CaptureControl;
			condition.Core = core;
			condition.reverse = Reverse;
			core.Elements.Add(condition);
			TempBindedCoreComponent = condition;
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
							Transitions[i].TransitionsData[j]
								.BindToCoreTransition((coreComponent as Condition).TransitionsElse[else_i++]);
						else
							Transitions[i].TransitionsData[j]
								.BindToCoreTransition((coreComponent as Condition).TransitionsIf[if_i++]);
					}
				}
			}
		}
#endif
	}
}