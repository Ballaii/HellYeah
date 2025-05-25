using Eliot.BehaviourEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Eliot.BehaviourEditor
{
	/// <summary>
	/// Node that represents the Entry Component of the behaviour model.
	/// </summary>
	public class EntryNode : Node
	{
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public EntryNode(){}
		
		/// <summary>
		/// Initialize the EntryNode.
		/// </summary>
		/// <param name="rect"></param>
		public EntryNode(Rect rect) : base(rect, "Entry"){}

#if UNITY_EDITOR
		/// <summary>
		/// Update the node's functionality.
		/// </summary>
		public override void Update()
		{
			try
			{
				if (Transitions.Count <= 0) return;
				foreach (var transition in Transitions)
				{
					transition.Draw();
				}
			}
			catch(System.Exception){/**/}
		}
		
		/// <summary>
		/// Draw the context menu of the node.
		/// </summary>
		public override void DrawMenu()
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Transition"), false, StartTransition, null);
			menu.ShowAsContext();
		}
#endif
		
		/// <summary>
		/// Build the behaviour engine component from the input data.
		/// </summary>
		/// <param name="gameObject"></param>
		/// <param name="core"></param>
		/// <returns></returns>
		public override Eliot.BehaviourEngine.CoreComponent GetBehaviourEngineComponent(GameObject gameObject, BehaviourCore core)
		{
			if (TempBindedCoreComponent != null) return TempBindedCoreComponent;
			
			var entry = new Eliot.BehaviourEngine.Entry();
			entry.Core = core;
			core.Entry = entry;
			core.ActiveComponent = entry;
			core.Elements.Add(entry);
			TempBindedCoreComponent = entry;
			foreach (var transition in Transitions)
			{
				transition.BuildBehaviourTransition(TempBindedCoreComponent, gameObject, core);
			}

			return TempBindedCoreComponent;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Initialize the transition starting from this node.
		/// </summary>
		/// <param name="obj"></param>
		public void StartTransition(object obj)
        {
	        Window.StartTransition(Rect, this, EliotGUISkin.NeutralColor);
		}

		/// <summary>
		/// Draw the node.
		/// </summary>
		public override void DrawContent()
		{
			GUILayout.Label("Entry",EliotGUISkin.GetLabelStyle("Entry"));
		}

		/// <summary>
		/// Associate the node with a runtime behaviour core component for the debugging purposes. 
		/// </summary>
		/// <param name="coreComponent"></param>
		public override void BindToCoreComponent(CoreComponent coreComponent)
		{
			BindedCoreComponent = coreComponent;
			if (Transitions.Count > 0)
			{
				var index = 0;
				for (int i = 0; i < Transitions.Count; i++)
					for (int j = 0; j < Transitions[i].TransitionsData.Count; j++)
						Transitions[i].TransitionsData[j].BindToCoreTransition((coreComponent as Entry).transitions[index++]);
			}
		}
		#endif
	}
}