namespace Eliot.AgentComponents
{
    /// <summary>
    /// Options for types of Agent's motion.
    /// </summary>
    public enum MotionEngine
    {
        NavMesh, 
        Turret,
#if ASTAR_EXISTS
        Astar,
#endif
    }
}