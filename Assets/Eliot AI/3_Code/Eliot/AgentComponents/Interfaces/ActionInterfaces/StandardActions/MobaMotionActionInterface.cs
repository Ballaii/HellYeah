using Eliot.AgentComponents;
using UnityEngine;

public class MobaMotionActionInterface : MotionActionInterface
{
    public MobaMotionActionInterface(EliotAgent agent) : base(agent)
    {
    }

    [IncludeInBehaviour]
    public void RunToHomeBase()
    {
        var homeBase = Agent["HomeBase"].objectValue as GameObject;
        if (homeBase)
        {
            Agent.Target = homeBase.transform;
            AgentMotionComponent.Engine.RunToTarget();
        }
        else
        {
            AgentMotionComponent.Engine.RunAround();
        }
    }
    
    [IncludeInBehaviour]
    public void RunToEnemyBase()
    {
        var homeBase = Agent["EnemyBase"].objectValue as GameObject;
        if (homeBase)
        {
            Agent.Target = homeBase.transform;
            AgentMotionComponent.Engine.RunToTarget();
        }
        else
        {
            AgentMotionComponent.Engine.RunAround();
        }
    }
}
