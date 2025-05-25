namespace Eliot.AgentComponents
{
    /// <summary>
    /// Any distinct states of Agent that are invoked in different
    /// user-defined circumstances.
    /// </summary>
    public enum AgentStatus
    {
        Normal,
        Alert,
        Danger,
        HeardSomething,
        BeingAimedAt
    }
}