using System;

#pragma warning disable CS0414

namespace Eliot.AgentComponents
{
	/// <summary>
	/// Encapsulates the calls to Agent's Motion API in order
	/// to check any user-defined conditions related to it.
	/// </summary>
	public class MotionConditionInterface : ConditionInterface
	{

		/// Agent's Motion component.
		protected readonly AgentMotion AgentMotion;

		/// <summary>
		/// Initialize the interface.
		/// </summary>
		/// <param name="agent"></param>
		public MotionConditionInterface(EliotAgent agent) : base(agent)
		{
			Agent = agent;
		    AgentMotion = agent.GetAgentComponent<Eliot.AgentComponents.AgentMotion>();
		}
		
		public override Action PreUpdateCallback
		{
			get
			{
				return () =>
				{
					if (AgentMotion == null)
						throw new EliotAgentComponentNotFoundException("Motion");
				};
			}
		}
	}
}