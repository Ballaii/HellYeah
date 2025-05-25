#pragma warning disable CS0414, CS0649, CS0612, CS1692
using System.Collections.Generic;
using Eliot.BehaviourEditor;
using Eliot.Repository;

namespace Eliot
{
    /// <summary>
    /// The following class is there to help convert Behaviours between different versions.
    /// </summary>
    public static class BehaviourVersionManager
    {
        /// <summary>
        /// Convert the first version of Behaviour to Behaviour 1.2
        /// </summary>
        /// <param name="oldJson"></param>
        /// <returns></returns>
        private static string convert__v_1_0__to__1_2(string oldJson)
        {
            if (oldJson == null) return null;

            #region VERSION_1_FUNCTIONS_DATABASE

            string[] action_motion_functions =
            {
                "Idle",
                "Stand",
                "WalkToTarget",
                "WalkAway",
                "RunToTarget",
                "RunAway",
                "DodgeBack",
                "DodgeRight",
                "DodgeLeft",
                "RushForward",
                "DodgeRandom",
                "DodgeLeftOrRight",
                "LookAtTarget",
                "WanderAround",
                "WalkAroundWaypoints",
                "RunAround",
                "RunAroundWaypoints",
                "Patrol"
            };
            string[] action_inventory_functions =
            {
                "PickItems",
                "WieldBestWeapon",
                "DropWorstItem",
                "UseBestHealingPotion",
                "UseBestEnergyPotion"
            };

            string[] condition_resources_functions =
            {
                "HealthFull",
                "HealthLow",
                "HealthTwoThirds",
                "HealthHalf",
                "HealthOneThird",
                "EnergyFull",
                "EnergyLow",
                "EnergyTwoThirds",
                "EnergyHalf",
                "EnergyOneThird",
                "EnoughEnergyForSkill"
            };
            string[] condition_perception_functions =
            {
                "SeeEnemy",
                "SeeFriend",
                "SeeBlock",
                "SeeCorpse",
                "EnemyIsLoadingAttack",
                "TargetIsInFrontOfMe",
                "SeeTargetsBack",
                "TargetInCloseRange",
                "TargetInMidRange",
                "TargetInFarRange",
                "TargetInSkillRange",
                "TargetHasLowHealth",
                "TargetHasFullHealth",
                "TargetNotFullyHealthy",
                "FriendNotFullyHealthy",
                "EnemyIsAimingAtMe",
                "ObstaclesBeforeTarget",
                "EnemyIsFleeing",
                "TargetWeakestEnemy",
                "TargetClosestEnemy",
                "SeeBetterWeapon",
                "HearedSomething",
                "TargetIsFake",
                "SeeHealingPotion",
                "SeeEnergyPotion"
            };
            string[] condition_motion_functions =
            {
                "Idling",
                "Standing",
                "Walking",
                "WalkingAway",
                "Running",
                "RunningAway",
                "Dodging"
            };
            string[] condition_inventory_functions =
            {
                "HaveBetterWeapon",
                "HaveHealingPotion",
                "HaveEnergyPotion"
            };
            string[] condition_general_functions =
            {
                "StatusNormal",
                "StatusAlert",
                "StatusDanger",
                "StatusBeingAimedAt",
                "FarFromHome",
                "AtHome",
                "SkillReadyToUse"
            };

            #endregion

            var conditionMap = new Dictionary<ConditionGroup, string[]>
            {
                {ConditionGroup.General, condition_general_functions},
                {ConditionGroup.Inventory, condition_inventory_functions},
                {ConditionGroup.Motion, condition_motion_functions},
                {ConditionGroup.Perception, condition_perception_functions},
                {ConditionGroup.Resources, condition_resources_functions}
            };

            var oldBehaviour = oldJson.Json();
            oldBehaviour["version"] = "{version:1.2}".Json();
            try
            {
                var nodes = oldBehaviour["nodes"].Objects;

                foreach (JsonObject node in nodes)
                {
                    var type = node["type"].String;
                    if (type == "Entry") continue;

                    if (type == "Action")
                    {
                        var actionGroup = node["actionGroup"].ActionGroup;
                        string str = null;
                        switch (actionGroup)
                        {
                            case ActionGroup.Motion:
                            {
                                var index = node["functionID"].Int;
                                str = action_motion_functions[index];
                                break;
                            }
                            case ActionGroup.Inventory:
                            {
                                var index = node["functionID"].Int;
                                str = action_inventory_functions[index];
                                break;
                            }
                            case ActionGroup.Skill:
                                str = node["functionID"].String;
                                break;
                        }

                        node["functionName"] = ("{functionName:" + str + "}").Json();
                    }
                    else if (type == "Condition" || type == "Loop")
                    {
                        var conditionGroup = node["conditionGroup"].ConditionGroup;
                        var index = node["functionID"].Int;
                        var str = conditionMap[conditionGroup][index];

                        #region Spelling

                        if (str == "HearedSomething") str = "HeardSomething";

                        #endregion

                        node["functionName"] = ("{functionName:" + str + "}").Json();
                    }
                }

                return oldBehaviour.Json();
            }
            catch (System.Exception)
            {
                return null;

            }
        }

        /// <summary>
        /// Read the version of the Behaviour by its string and return an updated version of that string.
        /// </summary>
        /// <param name="oldJson"></param>
        /// <returns></returns>
        public static string UpdateJson(string oldJson)
        {
            var oldBehaviour = oldJson.Json();
            float version;
            try
            {
                version = oldBehaviour["version"].Float;
            }
            catch (System.Exception)
            {
                return convert__v_1_0__to__1_2(oldJson);
            }

            return oldJson;
        }
    }
}