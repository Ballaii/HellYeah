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
	public class SkillNode : Node
	{
		/// <summary>
		/// The reference to the skill the node holds.
		/// </summary>
		public Skill Skill;

		[Tooltip("Should a Skill be executed or just set as current for retrieving information about it?")]
		public bool ExecuteSkill;

		/// <summary>
		/// Empty constructor.
		/// </summary>
		public SkillNode()
		{
		}

		/// <summary>
		/// Initialize the ActionNode.
		/// </summary>
		/// <param name="rect"></param>
		public SkillNode(Rect rect) : base(rect, "Skill")
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
		private void StartTransition(object obj)
		{
			Window.StartTransition(Rect, this, EliotGUISkin.NeutralColor);
		}

		/// <summary>
		/// Draw the node.
		/// </summary>
		public override void DrawContent()
		{
            string skillName = Skill && Skill.name.Length > 0 ? Skill.name : "SKILL";
            if (ExecuteSkill)
            {
                GUILayout.Label(skillName, new GUIStyle(EliotGUISkin.GetLabelStyle(skillName)) { normal = { textColor = Color.white } });
            }else
            {
                GUILayout.Label(skillName, new GUIStyle(EliotGUISkin.GetLabelStyle(skillName)) { normal = { textColor = Color.grey } });
            }
            
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

			var action = Skill ? gameObject.GetComponent<EliotAgent>().Skill(Skill.name, ExecuteSkill) : () => { };
			var invoker = new Action(action) {CaptureControl = CaptureControl, Core = core};
			invoker.id = NodeId;
			core.Elements.Add(invoker);
			TempBindedCoreComponent = invoker;
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
				var index = 0;
				for (int i = 0; i < Transitions.Count; i++)
				for (int j = 0; j < Transitions[i].TransitionsData.Count; j++)
					Transitions[i].TransitionsData[j]
						.BindToCoreTransition((coreComponent as Action).Transitions[index++]);
			}
		}
#endif
	}
}