using Eliot.Environment;
using UnityEngine;

public class PathAssigner : MonoBehaviour
{
    public WayPointsGroup assignedPath;
    public string team;
    
    private AgentsPoolHandler poolHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        var wpg = GetComponent<WayPointsGroup>();
        if (!wpg) return;
        poolHandler = wpg.agentsPoolHandler;

        poolHandler.onSpawnAgent = agent =>
        {
            agent.WayPoints = assignedPath;
            agent.Unit.Team = team;
        };
    }
}
