using System;
using System.Collections;
using System.Collections.Generic;
using Eliot.AgentComponents;
using Eliot.BehaviourEngine;
using UnityEngine;
using Action = System.Action;

namespace Eliot.Utility
{
    /// <summary>
    /// Helps schedule tasks, aka actions.
    /// </summary>
    public static class EliotTime
    {
        /// <summary>
        /// Actions that are already scheduled.
        /// </summary>
        private static List<Action> _actionsList = new List<Action>();
        
        /// <summary>
        /// Hash of an assigned timer.
        /// </summary>
        private static List<string> _timerHashList = new List<string>();
        /// <summary>
        /// Associated finish time of a task.
        /// </summary>
        private static List<float> _targetTimesList = new List<float>();

        /// <summary>
        /// Schedule an action to be executed in a given number of seconds.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="seconds"></param>
        /// <param name="action"></param>
        /// <param name="canStack"></param>
        public static void ExecuteIn(EliotAgent driver, float seconds, Action action, bool canStack)
        {
            if (canStack)
            {
                driver.StartCoroutine(ExecuteIn_enumerator(seconds, action));
            }
            else
            {
                if (_actionsList.Contains(action)) return;
                _actionsList.Add(action);
                driver.StartCoroutine(ExecuteIn_enumerator(seconds, action));
            }
        }
        
        /// <summary>
        /// Return false if current time is less than initial, true otherwise.
        /// </summary>
        /// <returns></returns>
        public static bool IsTime_Internal(float seconds, int agentId, int nodeId)
        {
            var hash = agentId.ToString() + nodeId.ToString();
            if (_timerHashList.Contains(hash))
            {
                var index = _timerHashList.IndexOf(hash);
                var isTime = Time.time >= _targetTimesList[index];
                //Debug.Log("Current time: " + Time.time + "; Target time: " + _targetTimesList[index]);
                if (isTime)
                {
                    _timerHashList.RemoveAt(index);
                    _targetTimesList.RemoveAt(index);
                }
                return !isTime;
            }
            _timerHashList.Add(hash);
            _targetTimesList.Add(Time.time + seconds);
            return true;
        }

        /// <summary>
        /// Check if it is already time to do the action based on its hash.
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="agentId"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static EliotCondition IsTime(float seconds, int agentId, int nodeId){
            return () =>
            {
                return IsTime_Internal(seconds, agentId, nodeId);
            };
        }

        /// <summary>
        /// Check if it is already time to do the action based on its hash. Random time between min and max.
        /// </summary>
        /// <param name="minSeconds"></param>
        /// <param name="maxSeconds"></param>
        /// <param name="agentId"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public static EliotCondition IsTime(float minSeconds, float maxSeconds, int agentId, int nodeId)
        {
            return () =>
            {
                return IsTime_Internal(UnityEngine.Random.Range(minSeconds, maxSeconds), agentId, nodeId);
            };
        }

        #region UTILITY

        /// <summary>
        /// Wait for a given number of seconds and execute the action.
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private static IEnumerator ExecuteIn_enumerator(float seconds, Action action)
        {
            if(seconds > 0)
                yield return new WaitForSeconds(seconds);
            action();
            if (_actionsList.Contains(action))
                _actionsList.Remove(action);
        }

        #endregion
    }
}