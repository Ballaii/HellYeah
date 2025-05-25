using System;
using UnityEngine;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// Set the names of parameters that are used to control this Agent's Animator Controller.
    /// </summary>
    [Serializable] public class AnimatorParameters
    {
        [Tooltip("The name of an Animator parameter on which the motion animations depend.")]
        public string Vertical = "Vertical";
        [Tooltip("The name of an Animator parameter on which the turning animations depend.")]
        public string Horizontal = "Horizontal";
        [Tooltip("The name of an Animator parameter which triggers the animation of dodging.")]
        public string DodgeBool = "Dodge";
        [Tooltip("The name of an Animator parameter which triggers the animation of taking damage.")]
        public string GetHitBool = "GetHit";
        [Tooltip("The name of an Animator parameter which triggers the animation of loading a skill.")]
        public string LoadSkillBool = "LoadSkill";
        [Tooltip("The name of an Animator parameter which triggers the animation of using a skill.")]
        public string ExecuteSkillBool = "Skill";
        [Tooltip("The name of an Animator parameter which triggers the animation of dying.")]
        public string DeathBool = "Death";
    }
}