#if UNITY_EDITOR
#pragma warning disable CS0414, CS0649, CS1692
using UnityEngine;

namespace Eliot.AgentComponents
{
    public partial class EliotAgent
    {
        /// <summary>
        /// Enumeration of the Agent types to simplify the Agent's properties initialization.
        /// </summary>
        public enum AgentType{ Player, NonPlayerCharacter }
        
        /// <summary>
        /// Whether to display advanced Agent settings in the Inspector or not.
        /// </summary>
        [SerializeField] private bool editorAdvancedMode = false;

        /// <summary>
        /// Type of the Agent. Can be NPC or a Player.
        /// </summary>
        [SerializeField] private AgentType agentType = AgentType.NonPlayerCharacter;
    }
}
#endif