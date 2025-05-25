using System;

namespace Eliot.AgentComponents
{
    public class PlayerInputConditionInterface : ConditionInterface
    {
        /// Agent's Resources component.
        protected readonly AgentPlayerInput agentPlayerInput;
        
        /// <summary>
        /// Initialize the interface.
        /// </summary>
        /// <param name="agent"></param>
        public PlayerInputConditionInterface(EliotAgent agent) : base(agent)
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
                        throw new EliotAgentComponentNotFoundException("Agent Component: Player Input");
                };
            }
        }
    }
}