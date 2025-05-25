using System;

#pragma warning disable CS0414

namespace Eliot.AgentComponents
{
	/// <summary>
	/// Encapsulates the calls to Agent's Inventory API in order
	/// to check any user-defined conditions related to it.
	/// </summary>
	public class InventoryConditionInterface : ConditionInterface
	{
		/// Agent's Inventory component.
		protected readonly AgentInventory AgentInventory;

		/// <summary>
		/// Initialize the interface.
		/// </summary>
		/// <param name="agent"></param>
		public InventoryConditionInterface(EliotAgent agent) : base(agent)
		{
			Agent = agent;
			AgentInventory = agent.GetAgentComponent<Eliot.AgentComponents.AgentInventory>();
		}
		
		public override Action PreUpdateCallback
		{
			get
			{
				return () =>
				{
					if (AgentInventory == null)
						throw new EliotAgentComponentNotFoundException("Inventory");
				};
			}
		}
	}
}