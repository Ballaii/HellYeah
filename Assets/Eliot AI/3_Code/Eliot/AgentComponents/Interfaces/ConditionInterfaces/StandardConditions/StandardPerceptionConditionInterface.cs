using System;
using Eliot.Environment;
using UnityEngine;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// The Standard Library of perception related conditions.
    /// </summary>
    public class StandardPerceptionConditionInterface : PerceptionConditionInterface
    {
        public StandardPerceptionConditionInterface(EliotAgent agent) : base(agent)
        {
        }

        /// <para>
        /// Define here names of queries which agent will run when requested by behaviour engine.
        /// Queries can check any public members of an object that is being checked.
        /// Agent's detection recognises all Units with colliders in the defined _range.
        /// </para>

        /// <summary>
        /// Return true if Agent can see an enemy or has seen one recently.
        /// </summary>
        /// /// <returns></returns>
        [IncludeInBehaviour]
        public bool SeeEnemy()
        {
            if(AgentPerception==null) throw new EliotAgentComponentNotFoundException("Perception");
            return AgentPerception.SeeUnit(unit => unit.Type == Environment.UnitType.Agent && !Agent.Unit.IsFriend(unit));
        }

        /// <summary>
        /// Return true if Agent can see a friend or has seen one recently.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool SeeFriend()
        { return AgentPerception.SeeUnit(unit => unit.Type == Environment.UnitType.Agent && Agent.Unit.IsFriend(unit)); }

        /// <summary>
        /// Return true if Agent can see a block or has seen one recently.
        /// </summary>
        [IncludeInBehaviour]
        public bool SeeBlock()
        { return AgentPerception.SeeUnit(unit => unit.Type == Environment.UnitType.Block); }

        /// <summary>
        /// Return true if Agent can see a corpse or has seen one recently.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool SeeCorpse()
        { return AgentPerception.SeeUnit(unit => unit.Type == Environment.UnitType.Corpse); }
        
        /// <summary>
        /// Return true if Agent can see a shelter or has seen one recently.
        /// </summary>
        [IncludeInBehaviour]
        public bool SeeShelter()
        { return AgentPerception.SeeUnit(unit => unit.Type == Environment.UnitType.Shelter); }

        /// <summary>
        /// Return true if Agent can see an enemy that is about to attack him.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool EnemyIsLoadingAttack()
        {
            return Agent.Status == AgentStatus.BeingAimedAt
                   || AgentPerception.SeeUnit(unit => unit && unit.Type == Environment.UnitType.Agent && unit.Team != Agent.Unit.Team
                                           && unit.GetComponent<EliotAgent>() && unit.GetComponent<EliotAgent>().CurrentSkill
                                             && unit.GetComponent<EliotAgent>().CurrentSkill.state == SkillState.Loading);
        }

        /// <summary>
        /// Return true if Agent's current target is in front of him with an error
        /// in measurement defined by Agent's general settings.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool TargetIsInFrontOfMe()
        {
            return Vector3.Angle(
                       Agent.Target ? Agent.Target.position - Agent.transform.position : Agent.transform.forward,
                       Agent.transform.forward) <= Agent["AimFieldOfView", 10f];
        }

        /// <summary>
        /// Return true if Agent's target is in front of him and is turned with its back to the Agent.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool SeeTargetsBack()
        {
            if (!Agent.Target) return false;
            return TargetIsInFrontOfMe() &&
                   Vector3.Angle(Agent.Target.forward, Agent.transform.forward) <= Agent["BackFieldOfView", 10f];
        }
        
        [IncludeInBehaviour]
        public bool TargetInRange(float range)
        {
            var targetCollider = Agent.Target.GetComponent<Collider>();
            float dist;
            if (targetCollider)
            {
                dist = Vector3.Distance(Agent.transform.position, targetCollider.ClosestPoint(Agent.transform.position));
            }
            else dist = Vector3.Distance(Agent.transform.position, Agent.Target.position)
                        - (Agent.Target.GetComponent<EliotAgent>() ? AgentPerception.TargetRadius() : 0);
            return dist <= range;
        }
        
        [IncludeInBehaviour]
        public bool InWayPointRange()
        {
            var dist = Vector3.Distance(Agent.transform.position, Agent.Target.position)
                       - (Agent.Target.GetComponent<EliotAgent>() ? AgentPerception.TargetRadius() : 0);
            var wayPoint = Agent.Target.GetComponent<EliotWayPoint>();
            if (wayPoint)
            {
                return dist <= wayPoint.ActionDistance;
            }

            return false;
        }

        /// <summary>
        /// Return true if Agent's current target is at the distance that is defined
        /// by user as Close Distance or closer.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool TargetInCloseRange()
        {
            return TargetInRange(Agent["CloseRange", 3f]);
        }

        /// <summary>
        /// Return true if Agent's current target is at the distance that is defined
        /// by user as Mid Distance or closer.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool TargetInMidRange()
        {
            return TargetInRange(Agent["MidRange", 5f]);
        }

        /// <summary>
        /// Return true if Agent's current target is at the distance that is defined by user as Far Distance or closer.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool TargetInFarRange()
        {
            return TargetInRange(Agent["FarRange", 5f]);
        }

        /// <summary>
        /// Return true if Agent's current target is at the distance that is defined by user as Far Distance or closer.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool TargetInSkillRange()
        {
            var dist = Vector3.Distance(Agent.transform.position, Agent.Target.position) - AgentPerception.TargetRadius();
            return dist <= (Agent.CurrentSkill ? Agent.CurrentSkill._range : Agent["FarRange", 10f]);
        }

        /// <summary>
        /// Return true if Agent can see a Unit with low health.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool TargetHasLowHealth(string healthResourceName = "Health")
        {
            return AgentPerception.SeeUnit(unit =>
            {
                var agent = unit.GetComponent<EliotAgent>();
                var agentResources = agent.GetAgentComponent<Eliot.AgentComponents.AgentResources>();
                if (!agentResources) return false;
                var health = agentResources[healthResourceName];
                if (health==null) return false;
                
                return health.currentValue <= agent["LowHealth", 0];
            });
        }

        /// <summary>
        /// Return true if Agent can see a Unit with maximum health.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool TargetHasFullHealth(string healthResourceName = "Health")
        {
            return AgentPerception.SeeUnit(unit =>
            {
                var agent = unit.GetComponent<EliotAgent>();
                var agentResources = agent.GetAgentComponent<Eliot.AgentComponents.AgentResources>();
                if (!agentResources) return false;
                var health = agentResources[healthResourceName];
                if (health==null) return false;
                
                return health.currentValue == health.maxValue;
            });
        }

        /// <summary>
        /// Return true if Agent can see a Unit with health lower than maximum.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool TargetNotFullyHealthy(string healthResourceName = "Health")
        {
            return AgentPerception.SeeUnit(unit =>
            {
                var agent = unit.GetComponent<EliotAgent>();
                var agentResources = agent.GetAgentComponent<Eliot.AgentComponents.AgentResources>();
                if (!agentResources) return false;
                var health = agentResources[healthResourceName];
                if (health==null) return false;
                
                return health.currentValue < health.maxValue;
            });
        }

        /// <summary>
        /// Return true if Agent can see a friend Agent with health lower than maximum.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool FriendNotFullyHealthy(string healthResourceName = "Health")
        {
            return AgentPerception.SeeUnit(unit =>
            {
                var agent = unit.GetComponent<EliotAgent>();
                if (unit.Team != Agent.Unit.Team) return false;
                var agentResources = agent.GetAgentComponent<Eliot.AgentComponents.AgentResources>();
                if (!agentResources) return false;
                var health = agentResources[healthResourceName];
                if (health==null) return false;
                
                return health.currentValue < health.maxValue;
            });
        }

        /// <summary>
        /// Return true if Agent can see enemy Agent who has the first one as a target.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool EnemyIsAimingAtMe()
        {
            return AgentPerception.SeeUnit(unit => unit && unit.Type == Environment.UnitType.Agent && unit.Team != Agent.Unit.Team
                                         && unit.GetComponent<EliotAgent>() && unit.GetComponent<EliotAgent>().Target == Agent.transform);
        }

        /// <summary>
        /// Return true if there are any objects between Agent and its target.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool ObstaclesBeforeTarget()
        { return AgentPerception.ObstaclesBetweenMeAndTarget(Agent.GetAgentComponent<Eliot.AgentComponents.AgentPerception>().Origin, Agent.Target, AgentPerception.PerceptionDimensions, 0.5f, AgentPerception.PerceptionMask); }

        /// <summary>
        /// Return true if Agent's current target is an enemy and is trying to run.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool EnemyIsFleeing()
        {
            return AgentPerception.SeeUnit(unit => unit.Type == Environment.UnitType.Agent && unit.Team != Agent.Unit.Team
                                         && unit.GetComponent<EliotAgent>()
                                         && (unit.GetComponent<EliotAgent>().GetAgentComponent<Eliot.AgentComponents.AgentMotion>().State == MotionState.Running
                                             || unit.GetComponent<EliotAgent>().GetAgentComponent<Eliot.AgentComponents.AgentMotion>().State == MotionState.Walking));
        }

        /// <summary>
        /// Return true if the Agent can see an enemy. If there are multiple enemies
        /// in the field of view, set the one with lowest health as a target.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool TargetWeakestEnemy(string healthResourceName = "Health")
        {
            try
            {
                var flag = AgentPerception.SelectUnitByCriteria
                (
                    query: unit => unit && unit.GetComponent<EliotAgent>() && unit.Type == Environment.UnitType.Agent &&
                                   !unit.IsFriend(Agent.Unit),
                    criterion: unit =>
                    {
                        var agent = unit.GetComponent<EliotAgent>();
                        if (!agent) return 0;
                        var resources = agent.GetAgentComponent<AgentResources>();
                        if (!resources) return 0;
                        var health = resources[healthResourceName];
                        if (health==null) return 0;
                        return health.currentValue;
                    },
                    chooseMax: false
                );
                return !flag ? AgentPerception.SeeUnit(unit => unit.Type == Environment.UnitType.Agent && unit.Team != Agent.Unit.Team) : flag;
            }
            catch (Exception)
            {
                return AgentPerception.SeeUnit(unit => unit.Type == Environment.UnitType.Agent && unit.Team != Agent.Unit.Team);
            }
        }
        
        /// <summary>
        /// Return true if the Agent can see an enemy. If there are multiple enemies
        /// in the field of view, set the one with lowest health as a target.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool TargetWeakestFriend(string healthResourceName = "Health")
        {
            try
            {
                var flag = AgentPerception.SelectUnitByCriteria
                (
                    query: unit => unit && unit.GetComponent<EliotAgent>() && unit.Type == Environment.UnitType.Agent &&
                                   unit.IsFriend(Agent.Unit),
                    criterion: unit =>
                    {
                        var agent = unit.GetComponent<EliotAgent>();
                        if (!agent) return 0;
                        var resources = agent.GetAgentComponent<AgentResources>();
                        if (!resources) return 0;
                        var health = resources[healthResourceName];
                        if (health==null) return 0;
                        return health.currentValue;
                    },
                    chooseMax: false
                );
                return !flag ? AgentPerception.SeeUnit(unit => unit.Type == Environment.UnitType.Agent && unit.IsFriend(Agent.Unit)) : flag;
            }
            catch (Exception)
            {
                return AgentPerception.SeeUnit(unit => unit.Type == Environment.UnitType.Agent && unit.IsFriend(Agent.Unit));
            }
        }

        /// <summary>
        /// Return true if the Agent can see an enemy. If there are multiple enemies
        /// in the field of view, set the one that is closest to the Agent.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool TargetClosestEnemy()
        {
            try
            {
                var flag = AgentPerception.SelectUnitByCriteria
                (
                    query: unit => unit && unit.GetComponent<EliotAgent>() && unit.Type == Environment.UnitType.Agent &&
                                   unit.Team != Agent.Unit.Team,
                    criterion: unit => Vector3.Distance(unit.transform.position, Agent.transform.position),
                    chooseMax: false
                );
                return !flag ? AgentPerception.SeeUnit(unit => unit.Type == Environment.UnitType.Agent && unit.Team != Agent.Unit.Team) : flag;
            }
            catch (Exception)
            {
                return AgentPerception.SeeUnit(unit => unit.Type == Environment.UnitType.Agent && unit.Team != Agent.Unit.Team);
            }
        }

        /// <summary>
        /// Return true if the Agent can see a weapon that is better than the currently wielded one.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool SeeBetterWeapon()
        {
            return AgentPerception.SeeUnit(unit => unit && unit.Type == Environment.UnitType.Item
                                              && unit.GetComponent<EliotItem>().Type == ItemType.Weapon
                                              && Agent.GetAgentComponent<Eliot.AgentComponents.AgentInventory>().ItemIsBetterThanCurrent(unit.GetComponent<EliotItem>()));
        }

        /// <summary>
        /// Return true if the Agent heared something unusual and set the source of the sounds as a target.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool HeardSomething()
        {
            if (Agent.Status == AgentStatus.HeardSomething)
            {
                Agent.Target = Agent.GetDefaultTarget(Agent.GetAgentComponent<Eliot.AgentComponents.AgentPerception>().SuspiciousPosition);
                if (Vector3.Distance(Agent.transform.position, Agent.Target.position) <= Agent["CloseRange", 3f])
                {
                    Agent.Status = AgentStatus.Normal;
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check whether the Agent's target is just a dummy helper object or Agent itself.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public bool TargetIsFake()
        {
            return (Agent.Target.GetComponent<EliotWayPoint>() && Agent.Target.GetComponent<EliotWayPoint>().automatic) || (Agent.Target.gameObject == Agent.gameObject);
        }

        /// <summary>
        /// Return true if Agent can see any Item that is a potion and has healing properties.
        /// </summary>
        /// /// <returns></returns>
        [IncludeInBehaviour]
        public bool SeeHealingPotion()
        {
            return AgentPerception.SeeUnit(unit => unit && unit.Type == Environment.UnitType.Item
                                                    && unit.GetComponent<EliotItem>().Type == ItemType.HealingPotion
                                                    && unit.GetComponent<EliotItem>().Skill);
        }

        /// <summary>
        /// Return true if Agent can see any Item that is a potion and has energy replenishing properties.
        /// </summary>
        /// /// <returns></returns>
        [IncludeInBehaviour]
        public bool SeeEnergyPotion()
        {
            return AgentPerception.SeeUnit(unit => unit && unit.Type == Environment.UnitType.Item
                                                    && unit.GetComponent<EliotItem>().Type == ItemType.HealingPotion
                                                    && unit.GetComponent<EliotItem>().Skill);
        }
        
        /// <summary>
        /// Return true if Agent can see a Unit of a given type.
        /// </summary>
        /// /// <returns></returns>
        [IncludeInBehaviour]
        public bool SeeUnitOfType(string type)
        {
            return AgentPerception.SeeUnit(
                unit =>
                {
                    return unit.Type.ToString() == type || unit["Type"].stringValue == type;
                });
        }
        
        [IncludeInBehaviour]
        public bool TargetIsFriendly()
        {
            var target = Agent.Target;
            if (!target) return false;
            var unit = target.GetComponent<Unit>();
            if (!unit) return false;
            return unit.IsFriend(Agent.Unit);
        }
        
        [IncludeInBehaviour]
        public bool IsBeingTargeted()
        {
            var result = false;
            foreach (var unit in AgentPerception.SeenUnits)
            {
                if(!unit) continue;
                if (unit.Type == UnitType.Agent && !unit.IsFriend(Agent.Unit) &&
                    unit.GetComponent<EliotAgent>().Target == Agent.transform)
                {
                    result = true;
                }
            }

            return result;
        }
    }
}
