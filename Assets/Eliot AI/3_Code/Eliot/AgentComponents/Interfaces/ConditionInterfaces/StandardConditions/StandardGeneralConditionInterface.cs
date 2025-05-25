using UnityEngine;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// The Standard Library of general conditions.
    /// </summary>
    public class StandardGeneralConditionInterface : GeneralConditionInterface
    {
        public StandardGeneralConditionInterface(EliotAgent agent) : base(agent)
        {
        }

        /// <summary>
        /// Check if Agent's _status is Normal.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool StatusNormal()
        {
            return Agent.Status == AgentStatus.Normal;
        }

        /// <summary>
        /// Check if Agent's _status is Alert.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool StatusAlert()
        {
            return Agent.Status == AgentStatus.Alert;
        }

        /// <summary>
        /// Check if Agent's _status is Danger.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool StatusDanger()
        {
            return Agent.Status == AgentStatus.Danger;
        }

        /// <summary>
        /// Check if Agent's _status is BeingAimedAt.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool StatusBeingAimedAt()
        {
            return Agent.Status == AgentStatus.BeingAimedAt;
        }

        /// <summary>
        /// Check if distance between Agent and his WaypointsGroup origin is bigger than the specified value.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool FarFromHome()
        {
            return Vector3.Distance(Agent.transform.position,
                       Agent.WayPoints ?
                           Agent.WayPoints.transform.position : Agent.GetAgentComponent<Eliot.AgentComponents.AgentMotion>().initialPosition)
                   >= Agent["FarFromHome", 50f];
        }

        /// <summary>
        /// Check if distance between Agent and his WaypointsGroup origin is smaller than the specified value.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool AtHome()
        {
            return Agent.WayPoints ? Agent.WayPoints.IsInsidePolygon(Agent.transform.position) :
                Vector3.Distance(Agent.transform.position, Agent.GetAgentComponent<Eliot.AgentComponents.AgentMotion>().initialPosition) <= Agent["AtHomeRange", 10f];
        }

        /// <summary>
        /// Check if Agent is casting a Skill at the moment.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour] public bool SkillReadyToUse() { return Agent.CurrentSkill; }
        
        /// <summary>
        /// Return true if current Skill _status is Idling.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour] public bool SkillStateIdling() { return Agent.CurrentSkill && Agent.CurrentSkill.state == SkillState.Idling; }
        
        /// <summary>
        /// Return true if current Skill _status is Loading.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour] public bool SkillStateLoading() { return Agent.CurrentSkill && Agent.CurrentSkill.state == SkillState.Loading; }
        
        /// <summary>
        /// Return true if current Skill _status is Invoking.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour] public bool SkillStateInvoking() { return Agent.CurrentSkill && Agent.CurrentSkill.state == SkillState.Invoking; }
        
        /// <summary>
        /// Return true if current Skill _status is Cool Down.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour] public bool SkillStateCoolDown() { return Agent.CurrentSkill && Agent.CurrentSkill.state == SkillState.CoolDown; }

    }
}
