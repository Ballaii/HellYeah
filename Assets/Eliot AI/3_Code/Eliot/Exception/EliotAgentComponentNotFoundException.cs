namespace Eliot
{
    /// <summary>
    /// Thrown when Eliot Agent Component is not found.
    /// </summary>
    public class EliotAgentComponentNotFoundException : EliotException
    {
        /// <summary>
        /// Base constructor. Log the message.
        /// </summary>
        /// <param name="message"></param>
        public EliotAgentComponentNotFoundException(string message) : base("EliotAgentComponentNotFoundException. Type " + message + " has not been added to the Agent.")
        {
            
        }
    }
}