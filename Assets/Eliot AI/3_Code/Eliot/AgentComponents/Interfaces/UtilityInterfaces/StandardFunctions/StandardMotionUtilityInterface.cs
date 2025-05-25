
namespace Eliot.AgentComponents
{
    /// <summary>
    /// Default utility queries related to motion.
    /// </summary>
    public class StandardMotionUtilityInterface : MotionUtilityInterface
    {
        public StandardMotionUtilityInterface(EliotAgent agent) : base(agent)
        {
        }
        
        /// <summary>
        /// Current agent's speed.
        /// </summary>
        /// <returns></returns>
        [IncludeInBehaviour]
        public float Velocity()
        {
            switch (AgentMotion.Type)
            {
                case MotionEngine.Turret:
                {
                    return 0.0f;
                }

                case MotionEngine.NavMesh:
                {
                    return (AgentMotion.Engine as NavMeshMotionEngine).NavMeshAgent.velocity.magnitude;
                }
#if ASTAR_EXISTS
            case MotionEngine.Astar:
            {
			    return (AgentMotion.Engine as AstarMotionEngine).StarAi.velocity.magnitude;
            }
#endif
            }

            return 0.0f;
        }
    }
}