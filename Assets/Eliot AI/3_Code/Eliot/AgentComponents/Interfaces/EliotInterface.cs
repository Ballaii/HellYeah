using System;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// Base class for all the interfaces that bind arbitrary API to the Eliot Agents.
    /// </summary>
    public abstract class EliotInterface
    {
        /// <summary>
        /// Link to the Agent.
        /// </summary>
        protected EliotAgent Agent;
        
        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="agent"></param>
        public EliotInterface(EliotAgent agent)
        {
            Agent = agent;
        }

        /// <summary>
        /// Executed every time before running any function from the utilized interface.
        /// </summary>
        public virtual Action PreUpdateCallback
        {
            get { return null; }
        }

        /// <summary>
        /// Executed every time after running any function from the utilized interface.
        /// </summary>
        public virtual Action PostUpdateCallback
        {
            get { return null; }
        }
    }
}