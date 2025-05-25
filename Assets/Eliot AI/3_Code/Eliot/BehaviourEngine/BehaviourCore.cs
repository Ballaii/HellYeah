using System.Collections.Generic;

namespace Eliot.BehaviourEngine
{
	/// <summary>
	/// Driver of the Behavior model. Activates and controls active model component.
	/// </summary>
	public class BehaviourCore
	{
		/// <summary>
		/// Entry point of the Behaviour. By default
		/// activates connected elements every update.
		/// </summary>
		public Entry Entry
		{
			get { return _entry;}
			set
			{
				_entry = value;
				ActiveComponent = value;
			}
		}
		
		/// <summary>
		/// Current active element of the model. Can be of type Entry or Loop.
		/// </summary>
		public CoreComponent ActiveComponent { get; set; }

		/// <summary>
		/// List of all the Core Components
		/// </summary>
		public List<CoreComponent> Elements = new List<CoreComponent>();
		
		/// <summary>
		/// List of all the Transitions
		/// </summary>
		public List<Transition> Transitions = new List<Transition>();
		
		/// Entry point of the Behaviour. 
		private Entry _entry;

		public string FullPath = "";

		public System.Action OnReset;

		/// <summary>
		/// Update current active component of the model.
		/// </summary>
		public void Update(){
			if (ActiveComponent != null)
			{
				ActiveComponent.Update();
			}
		}

		/// <summary>
		/// Set the active component to entry.
		/// </summary>
		public void Reset()
		{
			SetActiveComponent(_entry);
			if(OnReset != null) OnReset.Invoke();
		}

		/// <summary>
		/// Set active component to an arbitrary core component.
		/// </summary>
		/// <param name="component"></param>
		public void SetActiveComponent(CoreComponent component)
		{
			if (ActiveComponent == component) return;
			ActiveComponent.RelaxSingleIteration();
			ActiveComponent.Relax();
			ActiveComponent = component;
		}

		/// <summary>
		/// Get Node (Core Component) by it's unique Id.
		/// </summary>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		public CoreComponent GetNode(string nodeId)
		{
			foreach (var node in Elements)
			{
				if (node.id == nodeId)
					return node;
			}

			return null;
		}
		
		/// <summary>
		/// Get Node (Core Component) by it's unique Id.
		/// </summary>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		public T GetNode<T>(string nodeId) where T : CoreComponent
		{
			foreach (var node in Elements)
			{
				if (node.id == nodeId)
					return node as T;
			}

			return null;
		}
		
		/// <summary>
		/// Get Nodes (Core Component) that match the given Id.
		/// </summary>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		public List<CoreComponent> GetNodes(string nodeId)
		{
			var result = new List<CoreComponent>();
			foreach (var node in Elements)
			{
				if (node.id == nodeId)
					result.Add(node);
			}

			return result;
		}
		
		/// <summary>
		/// Get Nodes (Core Component) that match the given Id.
		/// </summary>
		/// <param name="nodeId"></param>
		/// <returns></returns>
		public List<T> GetNodes<T>(string nodeId) where T : CoreComponent
		{
			var result = new List<T>();
			foreach (var node in Elements)
			{
				if (node.id == nodeId)
					result.Add(node as T);
			}

			return result;
		}
		
		/// <summary>
		/// Get Transition by it's unique Id.
		/// </summary>
		/// <param name="transitionId"></param>
		/// <returns></returns>
		public Transition GetTransition(string transitionId)
		{
			foreach (var transition in Transitions)
			{
				if (transition.id == transitionId)
					return transition;
			}

			return null;
		}

		/// <summary>
		/// Get all Transition that match the given Id.
		/// </summary>
		/// <param name="transitionId"></param>
		/// <returns></returns>
		public List<Transition> GetTransitions(string transitionId)
		{
			var result = new List<Transition>();
			foreach (var transition in Transitions)
			{
				if (transition.id == transitionId)
					 result.Add(transition);
			}

			return result;
		}
	}
}
