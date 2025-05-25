using System;

namespace Eliot.AgentComponents
{
    public class PlayerInputActionInterface : ActionInterface
    {
        protected readonly AgentPlayerInput agentPlayerInput;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="agent"></param>
        public PlayerInputActionInterface(EliotAgent agent) : base(agent)
        {
            Agent = agent;
            agentPlayerInput = agent.GetAgentComponent<Eliot.AgentComponents.AgentPlayerInput>();
        }
		
        public override Action PreUpdateCallback
        {
            get
            {
                return () =>
                {
                    if (agentPlayerInput == null)
                        throw new EliotAgentComponentNotFoundException("AgentPlayerInput");
                };
            }
        }
    }
}