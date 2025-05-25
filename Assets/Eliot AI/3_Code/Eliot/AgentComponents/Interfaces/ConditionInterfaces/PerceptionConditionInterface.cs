using System;
using Eliot.Environment;

namespace Eliot.AgentComponents
{
	/// <summary>
	/// Query units checking for wanted characteristics.
	/// </summary>
	/// <param name="unit"></param>
	public delegate bool UnitQuery(Unit unit);
	
	/// <summary>
	/// Query units looking for the quantity of wanted criterion.
	/// </summary>
	/// <param name="unit"></param>
	public delegate float UnitCriterion(Unit unit);
	
	
	/// <summary>
	/// Encapsulates the calls to Agent's Perception API in order
	/// to check any user-defined conditions related to it.
	/// </summary>
	public class PerceptionConditionInterface : ConditionInterface
	{
		/// Agent's Perception component.
		protected readonly AgentPerception AgentPerception;

		/// <summary>
		/// Initialize the interface.
		/// </summary>
		/// <param name="agent"></param>
		public PerceptionConditionInterface(EliotAgent agent) : base(agent)
		{
			Agent = agent;
			AgentPerception = agent.GetAgentComponent<Eliot.AgentComponents.AgentPerception>();
		}
		
		public override Action PreUpdateCallback
		{
			get
			{
				return () =>
				{
					if (AgentPerception == null)
						throw new EliotAgentComponentNotFoundException("Perception");
				};
			}
		}
		
	}
}