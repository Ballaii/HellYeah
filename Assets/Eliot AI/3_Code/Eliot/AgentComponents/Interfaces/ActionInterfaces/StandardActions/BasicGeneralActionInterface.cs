using Eliot.BehaviourEditor;
using UnityEngine;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// General utility actions.
    /// </summary>
    public class BasicGeneralActionInterface : GeneralActionInterface
    {
        public BasicGeneralActionInterface(EliotAgent agent) : base(agent)
        {
        }

        /// <summary>
        /// Sets the active behavior core component to an entry.
        /// </summary>
        [IncludeInBehaviour]
        public void BREAK()
        {
            Agent.BehaviourCore.Reset();
        }

        /// <summary>
        /// Output a log message to the console.
        /// </summary>
        /// <param name="message"></param>
        [IncludeInBehaviour]
        public void DebugLog(string message)
        {
            Debug.Log(message);
        }

        /// <summary>
        /// Output a warning message to the console.
        /// </summary>
        /// <param name="message"></param>
        [IncludeInBehaviour]
        public void DebugWarning(string message)
        {
            Debug.LogWarning(message);
        }

        /// <summary>
        /// Output an error message to the console.
        /// </summary>
        /// <param name="message"></param>
        [IncludeInBehaviour]
        public void DebugError(string message)
        {
            Debug.LogError(message);
        }

        /// <summary>
        /// Set the new behaviour from within the current one.
        /// </summary>
        /// <param name="behaviour"></param>
        [IncludeInBehaviour]
        public void SetBehaviour(EliotBehaviour behaviour)
        {
            Agent.SetBehaviour(behaviour);
        }

    }
}