namespace Eliot.AgentComponents
{
    public class StandardPlayerInputActionInterface : PlayerInputActionInterface
    {
        public StandardPlayerInputActionInterface(EliotAgent agent) : base(agent)
        {
        }
        
        [IncludeInBehaviour] public void ExecuteInputCommands() { agentPlayerInput.ExecuteInputCommands(); }
    }
}