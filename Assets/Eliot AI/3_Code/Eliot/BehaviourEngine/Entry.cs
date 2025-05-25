using System.Collections.Generic;

namespace Eliot.BehaviourEngine
{
	/// <summary>
	/// Entry point of the behaviour. By default, the first element in the model to get updated.
	/// </summary>
	public class Entry : CoreComponent
	{
		/// <summary>
		/// List of transitions that link this component to the other ones in the model.
		/// </summary>
		public List<Transition> transitions = new List<Transition>();
		
		/// <summary>
		/// Initialize the component.
		/// </summary>
		/// <param name="id"></param>
		public Entry(string id = "Entry"):base(id){}

		/// <summary>
		/// Make Entry do its job.
		/// </summary>
		public override void Update()
		{
			if(PreUpdateCallback != null) PreUpdateCallback.Invoke();
			
			Active = true;
			
			Status = CoreComponentStatus.Normal;
			if (transitions.Count == 0) return;
			foreach (var transition in transitions)
			{
				var res = transition.Update();
				transition.RelaxSingleIteration();
				if (!res) break;
			}

			if(PostUpdateCallback != null) PostUpdateCallback.Invoke();
		}
		
		/// <summary>
		/// Stop the execution of the current node.
		/// </summary>
		public override void Relax()
		{
			if (!Active) return;
			Active = false;
			if (transitions.Count == 0) return;
			foreach (var transition in transitions)
				transition.Relax();
		}
		
		/// <summary>
		/// Stop the execution of the current node within a single iteration to prevent recursion.
		/// </summary>
		public override void RelaxSingleIteration()
		{
			if (transitions.Count == 0) return;
			foreach (var transition in transitions)
				transition.RelaxSingleIteration();
		}
		

		/// <summary>
		/// Create transition between Entry and other component.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="minProbability"></param>
		/// <param name="maxProbability"></param>
		/// <param name="minCooldown"></param>
		/// <param name="maxCooldown"></param>
		/// <param name="terminate"></param>
		/// <returns></returns>
		public override BehaviourEngine.Transition ConnectWith(CoreComponent start, CoreComponent end, float minProbability, float maxProbability, 
			float minCooldown, float maxCooldown, bool terminate)
		{
			var transition = new Transition(start, end, minProbability, maxProbability, minCooldown, maxCooldown, terminate);
			transitions.Add(transition);
			return transition;
		}
	}
}
