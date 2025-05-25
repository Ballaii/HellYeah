using System;
using UnityEngine;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// Keep references to Animation clips that correspond to certain Agent's states.
    /// </summary>
    [Serializable] public class LegacyAnimationClips
    {
        public AnimationClip Idle;
        public AnimationClip Walk;
        public AnimationClip Run;
        public AnimationClip Dodge;
        public AnimationClip TakeDamage;
        public AnimationClip LoadSkill;
        public AnimationClip UseSkill;
        public AnimationClip Death;
    }
}