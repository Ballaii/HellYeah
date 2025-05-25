namespace Eliot.AgentComponents
{
    /// <summary>
    /// Enumeration representing possible states of Animation component of the Agent.
    /// </summary>
    public enum AnimationState
    {
        Idling,
        Walking,
        Running,
        Dodging,
        TakingDamage,
        LoadingSkill,
        UsingSkill,
        Dying
    }
}