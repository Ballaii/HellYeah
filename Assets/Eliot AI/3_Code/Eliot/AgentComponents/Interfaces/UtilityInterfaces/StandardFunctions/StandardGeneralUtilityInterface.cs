using UnityEngine;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// General/uncategorized utility queries.
    /// </summary>
    public class StandardGeneralUtilityInterface : GeneralUtilityInterface
    {
        public StandardGeneralUtilityInterface(EliotAgent agent) : base(agent)
        {
        }
        
        /// <summary>
        /// Return the current distance to the agent's target.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public float DistanceToTarget()
        {
            float distance;
            if(Agent.Target)
                distance = Vector3.Distance(Agent.transform.position, Agent.Target.position);
            else
                distance = Vector3.Distance(Agent.transform.position, Vector3.zero);
            
            return distance;
        }
        
        /// <summary>
        /// Return the current distance to the agent's initial position.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public float DistanceToHome()
        {
            float distance;
            if(Agent.WayPoints)
                distance = Vector3.Distance(Agent.transform.position, Agent.WayPoints.transform.position);
            else
                distance = Vector3.Distance(Agent.transform.position, Vector3.zero);
            
            return distance;
        }
    }
}