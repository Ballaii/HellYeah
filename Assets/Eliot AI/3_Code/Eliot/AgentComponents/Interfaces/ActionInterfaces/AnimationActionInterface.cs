using System;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// Encapsulates Agent's Motion actions by calling its API
    /// and giving names to these groups of calls. 
    /// </summary>
    public class AnimationActionInterface : ActionInterface
    {
        // Agent's Motion component.
        protected readonly AgentAnimation AgentAnimationComponent;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="agent"></param>
        public AnimationActionInterface(EliotAgent agent) : base(agent)
        {
            AgentAnimationComponent = agent.GetAgentComponent<Eliot.AgentComponents.AgentAnimation>();
        }
		
        public override Action PreUpdateCallback
        {
            get
            {
                return () =>
                {
                    if (AgentAnimationComponent == null)
                        throw new EliotAgentComponentNotFoundException("Animation");
                };
            }
        }
    }
}