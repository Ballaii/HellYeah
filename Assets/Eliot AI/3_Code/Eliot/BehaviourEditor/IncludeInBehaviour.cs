namespace Eliot.AgentComponents
{
	/// <summary>
	/// Mark methods of specific classes by this attribute
	/// to include them in Behaviour model editor.
	/// </summary>
	[System.AttributeUsage(System.AttributeTargets.Method)]
	public class IncludeInBehaviour : System.Attribute{}
}