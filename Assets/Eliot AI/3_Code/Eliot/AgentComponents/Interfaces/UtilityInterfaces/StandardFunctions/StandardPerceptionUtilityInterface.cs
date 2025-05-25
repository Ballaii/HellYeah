using Eliot.Environment;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// Default utility queries related to perception.
    /// </summary>
    public class StandardPerceptionUtilityInterface : PerceptionUtilityInterface
    {
        public StandardPerceptionUtilityInterface(EliotAgent agent) : base(agent)
        {
        }

        /// <summary>
        /// Return the current value of a target's resource called 'Health'.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public float TargetsHealth()
        {
            var target = AgentPerception.TargetUnit;
            if (target)
            {
                var targetsResources = target.GetComponent<EliotAgent>().GetAgentComponent<AgentResources>();
                if (!targetsResources) return 0;
                var targetsHealth = targetsResources["Health"];
                if (targetsHealth == null) return 0;
                return targetsHealth.currentValue;
            }

            return 0.0f;
        }

        /// <summary>
        /// Return the current value of a target's resource called 'Energy'.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public float TargetsEnergy()
        {
            var target = AgentPerception.TargetUnit;
            if (target)
            {
                var targetsResources = target.GetComponent<EliotAgent>().GetAgentComponent<AgentResources>();
                if (!targetsResources) return 0;
                var targetsHealth = targetsResources["Energy"];
                if (targetsHealth == null) return 0;
                return targetsHealth.currentValue;
            }

            return 0.0f;
        }

        /// <summary>
        /// Return the current value of a target's resource with a given name.
        /// </summary>
        /// <param name="resourceName"></param>
        /// <returns></returns>
        [IncludeInBehaviour]
        public float TargetsResourceValue(string resourceName)
        {
            var target = AgentPerception.TargetUnit;
            if (target)
            {
                var targetsResources = target.GetComponent<EliotAgent>().GetAgentComponent<AgentResources>();
                if (!targetsResources) return 0;
                var targetsHealth = targetsResources[resourceName];
                if (targetsHealth == null) return 0;
                return targetsHealth.currentValue;
            }

            return 0.0f;
        }
        
        [IncludeInBehaviour]
        public float NumberOfEnemies()
        {
            var number = 0f;
            foreach (var unit in AgentPerception.SeenUnits)
            {
                if (unit.Type == UnitType.Agent && !unit.IsFriend(Agent.Unit))
                    number += 1f;
            }
            return number;
        }
        
        [IncludeInBehaviour]
        public float NumberOfFriends()
        {
            var number = 0f;
            foreach (var unit in AgentPerception.SeenUnits)
            {
                if (unit.Type == UnitType.Agent && unit.IsFriend(Agent.Unit))
                    number += 1f;
            }
            return number;
        }
        
        [IncludeInBehaviour]
        public float EnemiesToFriendsRatio()
        {
            var numberOfEnemies = 0f;
            var numberOfFriends = 0f;
            foreach (var unit in AgentPerception.SeenUnits)
            {
                if (unit.Type == UnitType.Agent)
                {
                    if (!unit.IsFriend(Agent.Unit))
                        numberOfEnemies += 1f;
                    else
                        numberOfFriends += 1f;
                }
            }
            return numberOfEnemies / numberOfFriends;
        }
    }
}