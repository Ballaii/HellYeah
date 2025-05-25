using System;

namespace Eliot.AgentComponents
{
	/// <summary>
	/// Encapsulates Agent's Motion actions by calling its API
	/// and giving names to these groups of calls. 
	/// </summary>
    public class MotionActionInterface : ActionInterface
	{
        // Agent's Motion component.
		protected readonly AgentMotion AgentMotionComponent;
        protected readonly IMotionEngine _motion;

        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="agent"></param>
		public MotionActionInterface(EliotAgent agent) : base(agent)
		{
			AgentMotionComponent = agent.GetAgentComponent<Eliot.AgentComponents.AgentMotion>();
			if(AgentMotionComponent)
				_motion = AgentMotionComponent.Engine;
		}
		
		public override Action PreUpdateCallback
		{
			get
			{
				return () =>
				{
					if (_motion == null)
						throw new EliotAgentComponentNotFoundException("Motion");
				};
			}
		}
	}
}