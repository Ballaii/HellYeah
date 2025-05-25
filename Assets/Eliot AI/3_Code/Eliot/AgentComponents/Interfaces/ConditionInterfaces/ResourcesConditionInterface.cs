using System;

namespace Eliot.AgentComponents
{
	/// <summary>
	/// Encapsulates the calls to Agent's Resources API in order
	/// to check any user-defined conditions related to it.
	/// </summary>
	public class ResourcesConditionInterface : ConditionInterface
	{
		/// Agent's Resources component.
		protected readonly AgentResources agentResources;
		

		/// <summary>
		/// Initialize the interface.
		/// </summary>
		/// <param name="agent"></param>
		public ResourcesConditionInterface(EliotAgent agent) : base(agent)
		{
			Agent = agent;
			agentResources = agent.GetAgentComponent<Eliot.AgentComponents.AgentResources>();
		}
		
		public override Action PreUpdateCallback
		{
			get
			{
				return () =>
				{
					if (agentResources == null)
						throw new EliotAgentComponentNotFoundException("Agent Component: Resources");
				};
			}
		}
	}
}