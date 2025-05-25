using System;

namespace Eliot.AgentComponents
{
	/// <summary>
	/// Encapsulates Agent's Inventory actions by calling its API
	/// and giving names to these groups of calls. 
	/// </summary>
    public class InventoryActionInterface : ActionInterface
	{
        /// Agent's Inventory component.
        protected readonly AgentInventory AgentInventory;
        

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="agent"></param>
		public InventoryActionInterface(EliotAgent agent) : base(agent)
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