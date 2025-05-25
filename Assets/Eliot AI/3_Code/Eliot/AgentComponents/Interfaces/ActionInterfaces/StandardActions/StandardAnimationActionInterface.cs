using UnityEngine;

namespace Eliot.AgentComponents
{
    public class StandardAnimationActionInterface : AnimationActionInterface
    {
        public StandardAnimationActionInterface(EliotAgent agent) : base(agent)
        {
        }

        [IncludeInBehaviour]
        public void SetAnimatorTrigger(string animatorTriggerName)
        {
            if (AgentAnimationComponent.Animator)
            {
                AgentAnimationComponent.Animator.SetTrigger(animatorTriggerName);
            }
            else
            {
                Debug.Log("There's no animator assigned to this agent", AgentAnimationComponent);
            }
        }
        
        [IncludeInBehaviour]
        public void ResetAnimatorTrigger(string animatorTriggerName)
        {
            if (AgentAnimationComponent.Animator)
            {
                AgentAnimationComponent.Animator.ResetTrigger(animatorTriggerName);
            }
        }
        
        [IncludeInBehaviour]
        public void SetAnimatorBool(string animatorBoolName, bool value)
        {
            if (AgentAnimationComponent.Animator)
            {
                AgentAnimationComponent.Animator.SetBool(animatorBoolName, value);
            }
        }
        
        [IncludeInBehaviour]
        public void SetAnimatorFloat(string animatorFloatName, float value)
        {
            if (AgentAnimationComponent.Animator)
            {
                AgentAnimationComponent.Animator.SetFloat(animatorFloatName, value);
            }
        }
        
        [IncludeInBehaviour]
        public void SetAnimatorInt(string animatorIntName, int value)
        {
            if (AgentAnimationComponent.Animator)
            {
                AgentAnimationComponent.Animator.SetInteger(animatorIntName, value);
            }
        }
    }
}