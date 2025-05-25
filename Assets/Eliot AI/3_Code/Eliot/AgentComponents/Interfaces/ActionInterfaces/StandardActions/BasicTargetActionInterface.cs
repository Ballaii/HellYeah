using Eliot.AgentComponents;

public class BasicTargetActionInterface : TargetActionInterface {

	public BasicTargetActionInterface(EliotAgent agent) : base(agent)
	{
	}

	/// <summary>
	/// Reset the target to the default one.
	/// </summary>
	[IncludeInBehaviour] public void ResetTarget()
	{
		Agent.Target = Agent.GetDefaultTarget();
	}
}
