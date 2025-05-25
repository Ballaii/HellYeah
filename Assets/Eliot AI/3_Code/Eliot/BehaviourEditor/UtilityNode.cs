using System;
using System.Collections.Generic;
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
	/// Holds the necessary information to evaluate functions' utilities and execute the most valuable action.
	/// </summary>
	public class UtilityNode : Node
	{
		/// <summary>
		/// List of the Utility Curves evaluated against each other.
		/// </summary>
		[SerializeField] public List<UtilityCurve> Curves = new List<UtilityCurve>();

		/// <summary>
		/// Editor only. Keeps the track of which curve to associate the new transition with.
		/// </summary>
		public UtilityCurve CurveThatStartedTransition = null;

		/// <summary>
		/// Empty constructor.
		/// </summary>
		public UtilityNode()
		{
		}

		/// <summary>
		/// Initialize the ActionNode.
		/// </summary>
		/// <param name="rect"></param>
		public UtilityNode(Rect rect) : base(rect, "Utility")
		{
		}
#if UNITY_EDITOR
		/// <summary>
		/// Draw the capturing control indicator.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="offset"></param>
		public void DrawArrow(Vector2 start, Vector2 end, float offset)
		{
			var arrowHead = new Vector3[3];

			var forward = (Vector3) (end - start).normalized;
			var right = Vector3.Cross(Vector3.forward, forward).normalized;
			var size = HandleUtility.GetHandleSize(end);
			var width = size * 0.3f;
			var height = size * 0.5f;

			var len = (end - start).magnitude;
			Vector3 cen = start + (len * offset + height / 2f) * (Vector2) forward;
			arrowHead[0] = cen;
			arrowHead[1] = cen - forward * height + right * width;
			arrowHead[2] = cen - forward * height - right * width;

			Handles.DrawAAConvexPolygon(arrowHead);
		}

		/// <summary>
		/// Draw the axis of the node.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="e"></param>
		/// <param name="c"></param>
		private void DrawArrowAxis(Vector3 s, Vector3 e, Color c)
		{
			var col = Handles.color;
			Handles.color = c;

			Handles.DrawAAPolyLine(2f, s, e);
			DrawArrow((Vector2) s, (Vector2) e, 1f);

			Handles.color = col;
		}

		/// <summary>
		/// Do this after all the nodes been updated.
		/// </summary>
		public override void LateUpdate()
		{
			base.LateUpdate();
			RenderCurves();
			Vector3 xS = new Vector3(Rect.x + 16f, Rect.y + Rect.height - 16f, 0);
			Vector3 xE = new Vector3(Rect.x + Rect.width - 16f, Rect.y + Rect.height - 16f, 0);
			DrawArrowAxis(xS, xE, Color.red);

			Vector3 yS = new Vector3(Rect.x + 16f, Rect.y + Rect.height - 16f, 0);
			Vector3 yE = new Vector3(Rect.x + 16f, Rect.y + 16f, 0);
			DrawArrowAxis(yS, yE, Color.green);

			Handles.SphereHandleCap(666, xS, new Quaternion(0, 0, 1, 0), 2f, EventType.ContextClick);
		}

		/// <summary>
		/// Draw the curves in the Node's rect.
		/// </summary>
		private void RenderCurves()
		{
			Handles.color = Color.white;

			Rect backgroundRect = Rect;
            backgroundRect.width -= EliotGUISkin.defaultUtilityNodePadding;
			backgroundRect.height -= EliotGUISkin.defaultUtilityNodePadding;
			backgroundRect.center = Rect.center;

			var outlineRect = Rect;
			outlineRect.width = (outlineRect.width - EliotGUISkin.defaultUtilityNodePadding) + 1f;
			outlineRect.height = (outlineRect.height - EliotGUISkin.defaultUtilityNodePadding) + 1f;
			outlineRect.center = Rect.center;
			
			GUI.DrawTextureWithTexCoords(backgroundRect, EliotProjectSettings.GUIResources.utilityRect, new Rect(0, 0, backgroundRect.width/EliotProjectSettings.GUIResources.utilityRect.width, backgroundRect.height / EliotProjectSettings.GUIResources.utilityRect.height),false);

			float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
			foreach (var curve in Curves)
			{
				foreach (var key in curve.Curve.keys)
				{
					var x = key.time;
					var y = key.value;
					if (x < minX) minX = x;
					if (x > maxX) maxX = x;
					if (y < minY) minY = y;
					if (y > maxY) maxY = y;
				}
			}

			foreach (UtilityCurve curve in Curves)
			{
				curve.Render(Rect, EliotGUISkin.defaultUtilityNodePadding, minX, maxX, minY, maxY);
			}
		}

		/// <summary>
		/// Update the node's functionality.
		/// </summary>
		public override void Update()
		{
			foreach (UtilityCurve curve in Curves)
			{
				curve.UpdateTransitions();
			}
		}

		/// <summary>
		/// Remove the given transition.
		/// </summary>
		/// <param name="transition"></param>
		public override void RemoveTransition(NodesTransition transition)
		{
			foreach (UtilityCurve curve in Curves)
			{
				if (curve.Transitions.Contains(transition))
				{
					curve.Transitions.Remove(transition);
					break;
				}

			}

			if (Transitions.Contains(transition))
				Transitions.Remove(transition);
		}

		/// <summary>
		/// Draw the context menu of the node.
		/// </summary>
		public override void DrawMenu()
		{
			var menu = new GenericMenu();
			var namesList = new List<string>();
			foreach (var curve in Curves)
			{
				var newOption = curve.Name;
				int numOccur = 0;
				foreach (var name in namesList)
				{
					if (name == newOption)
						numOccur++;
				}

				menu.AddItem(new GUIContent("Transition/" + (newOption + (numOccur == 0 ? "" : " " + numOccur))), false,
					StartTransition, curve);
				namesList.Add(newOption);
			}

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
			if (obj != null)
			{
				CurveThatStartedTransition = (obj as UtilityCurve);
			}

			var window = EditorWindow.GetWindow<BehaviourEditorWindow>("Behaviour");
			window.StartTransition(Rect, this, (obj as UtilityCurve).Color, false,
				(start, end, transition) =>
				{
					bool transitionExists = false;
					NodesTransition resultTransition = null;
					var curve = (start as UtilityNode).CurveThatStartedTransition;
					foreach (var t in curve.Transitions)
					{
						if (t.End == end)
						{
							transitionExists = true;
							resultTransition = t;
							break;
						}
					}

					if (curve != null)
					{
						if (!transitionExists)
						{
							curve.Transitions.Add(transition);
						}
						else
						{
							resultTransition.TransitionsData.Add(new NodesTransitionData(resultTransition));
						}
					}
				}
			);
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

			var utility = new BehaviourEngine.Utility();
			utility.id = NodeId;
			utility.Core = core;
			utility.CaptureControl = CaptureControl;
			TempBindedCoreComponent = utility;
			foreach (var curve in Curves)
			{
				var component = gameObject.GetComponent<EliotAgent>().AddInterface(curve.MethodData.FullClassName);
				TempBindedCoreComponent.PreUpdateCallback += component.PreUpdateCallback;
				TempBindedCoreComponent.PostUpdateCallback += component.PostUpdateCallback;
				var metInfo = component.GetType().GetMethod(curve.MethodData.FullMethodName,
					BindingFlags.Public | BindingFlags.Instance);
				if (metInfo == null) return null;
				var numParams = metInfo.GetParameters().Length;
				UtilityCurveCore utilityCurveCore = null;
				if (numParams == 0)
				{
					var action =
						(EliotUtilityValue) Delegate.CreateDelegate(typeof(EliotUtilityValue), component, metInfo,
							true);
					utilityCurveCore = new UtilityCurveCore(curve.Curve, action);
				}
				else if (numParams == 1)
				{
					var pType = curve.MethodData.MethodParameters[0].Type();
					var param = curve.MethodData.GetParameters()[0];

					if (pType == typeof(bool))
					{

						var action = (EliotUtilityValue<bool>) Delegate.CreateDelegate(typeof(EliotUtilityValue<bool>),
							component, metInfo, true);
						utilityCurveCore = new UtilityCurveCore(curve.Curve, action, (bool) param);
					}
					else if (pType == typeof(int))
					{
						var action = (EliotUtilityValue<int>) Delegate.CreateDelegate(typeof(EliotUtilityValue<int>),
							component, metInfo, true);
						utilityCurveCore = new UtilityCurveCore(curve.Curve, action, (int) param);
					}
					else if (pType == typeof(float))
					{
						var action =
							(EliotUtilityValue<float>) Delegate.CreateDelegate(typeof(EliotUtilityValue<float>),
								component, metInfo, true);
						utilityCurveCore = new UtilityCurveCore(curve.Curve, action, (float) param);
					}
					else if (pType == typeof(string))
					{
						var action =
							(EliotUtilityValue<string>) Delegate.CreateDelegate(typeof(EliotUtilityValue<string>),
								component, metInfo, true);
						utilityCurveCore = new UtilityCurveCore(curve.Curve, action, (string) param);
					}
					else
					{
						utilityCurveCore = new UtilityCurveCore(curve.Curve, curve.MethodData, gameObject);
					}
				}
				else
				{
					utilityCurveCore = new UtilityCurveCore(curve.Curve, curve.MethodData, gameObject);
				}

				foreach (var transition in curve.Transitions)
				{
					if (!transition) continue;
					var coreTransition = transition.BuildBehaviourTransition(TempBindedCoreComponent, gameObject, core);
					utilityCurveCore.Transitions.AddRange(coreTransition);
				}

				utility.UtilityCurves.Add(utilityCurveCore);
			}

			core.Elements.Add(utility);

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

			for (int i = 0; i < Curves.Count; i++)
			{
				if (Curves[i] == null) continue;
				for (int j = 0; j < Curves[i].Transitions.Count; j++)
				{
					if (!Curves[i].Transitions[j]) continue;
					int index = 0;
					for (int k = 0; k < Curves[i].Transitions[j].TransitionsData.Count; k++)
						Curves[i].Transitions[j].TransitionsData[k].BindToCoreTransition(
							(coreComponent as BehaviourEngine.Utility).UtilityCurves[i].Transitions[index++]
						);
				}
			}
		}
#endif
	}
}