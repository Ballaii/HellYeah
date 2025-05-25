using System;
using System.Collections.Generic;
using Eliot.AgentComponents;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Eliot.Environment
{
    /// <summary>
    /// Handles the pool of agents at runtime.
    /// </summary>
    [System.Serializable]
    public class AgentsPoolHandler
    {
        /// <summary>
        /// Define the area where to spawn the agents.
        /// </summary>
        public WayPointsGroup wayPointsGroup;

        /// <summary>
        /// The way of pooling.
        /// </summary>
        public WayPointsGroup.PoolingMode poolingMode
        {
            get { return wayPointsGroup.poolingMode; }
        }

        /// <summary>
        /// Whether to pool at all.
        /// </summary>
        public bool poolAgents
        {
            get { return wayPointsGroup.poolAgents; }
        }

        /// <summary>
        /// Maximum number of agents that can be present in the scene at the same time that have been created by this pool handler.
        /// </summary>
        public int maxAgentsNumber
        {
            get { return wayPointsGroup.maxAgentsNumber; }
        }

        /// <summary>
        /// How much time to wait until pooling the next agent.
        /// </summary>
        public float agentsPoolCoolDown
        {
            get { return wayPointsGroup.agentsPoolCoolDown; }
        }

        /// <summary>
        /// The prefabs to instantiate.
        /// </summary>
        public List<GameObject> AgentsPrefabs
        {
            get { return wayPointsGroup.pooledAgentsPrefabs; }
        }

        /// <summary>
        /// Cache.
        /// </summary>
        public List<GameObject> AgentsPool = new List<GameObject>();

        /// <summary>
        /// Did the waves start spawning agents?
        /// </summary>
        public bool WavesInitiated = false;

        /// <summary>
        /// Handles pooling agents in waves.
        /// </summary>
        public WayPointsSpawningWavesHandler WavesHandler
        {
            get
            {
                if (wayPointsGroup.wavesHandler == null)
                {
                    wayPointsGroup.wavesHandler = new WayPointsSpawningWavesHandler(wayPointsGroup);
                }
                return wayPointsGroup.wavesHandler;
            }
            set { wayPointsGroup.wavesHandler = value; }
        }

        /// <summary>
        /// Temp variable. The last time an agent has been pooled.
        /// </summary>
        private float _lastTimePooledAgent;

        public Action<EliotAgent> onSpawnAgent;

        /// <summary>
        /// Create a new Pool Handler.
        /// </summary>
        /// <param name="wayPointsGroup"></param>
        public AgentsPoolHandler(WayPointsGroup wayPointsGroup)
        {
            this.wayPointsGroup = wayPointsGroup;
        }

        /// <summary>
        /// Initialize the pool.
        /// </summary>
        /// <param name="wayPointsGroup"></param>
        public void Init(WayPointsGroup wayPointsGroup)
        {
            this.wayPointsGroup = wayPointsGroup;
            this.WavesHandler.Init(wayPointsGroup);
            WavesInitiated = false;
        }

        /// <summary>
        /// Get a random prefab from the available ones.
        /// </summary>
        /// <returns></returns>
        public GameObject RandomAgentFromPrefabsList()
        {
            return wayPointsGroup.RandomAgentFromPrefabsList();
        }

        /// <summary>
        /// Get a random prefab from the given ones.
        /// </summary>
        /// <param name="pooledObjectsList"></param>
        /// <returns></returns>
        public GameObject RandomAgentFromPrefabsList(List<GameObject> pooledObjectsList)
        {
            return pooledObjectsList[Random.Range(0, pooledObjectsList.Count)];
        }

        /// <summary>
        /// Random agent from cache.
        /// </summary>
        /// <returns></returns>
        public GameObject RandomAgentFromPool()
        {
            return AgentsPool[Random.Range(0, AgentsPool.Count)];
        }

        /// <summary>
        /// Create a new agent from prefab.
        /// </summary>
        /// <param name="agentGameObject"></param>
        public void InstantiateNewAgent(GameObject agentGameObject)
        {
            if (!agentGameObject) return;
            var position = this.wayPointsGroup.RandomPoint();
            var rotation = new Quaternion(0, Random.Range(0, 2 * Mathf.PI), 0, 1);
            var newAgent = GameObject.Instantiate(agentGameObject, position, rotation) as GameObject;
            var agent = newAgent.GetComponent<EliotAgent>();
            if (agent)
            {
                agent.WayPoints = wayPointsGroup;
            }

            newAgent.transform.parent = wayPointsGroup.AgentsParent().transform;
            
            if(onSpawnAgent != null) onSpawnAgent.Invoke(agent);
        }

        /// <summary>
        /// Reuse an agent from cache instead of instantiating a new one.
        /// </summary>
        private void SpawnAgentFromPool()
        {
            var agent = RandomAgentFromPool();
            if (!agent) return;
            var eliotAgent = agent.GetComponent<EliotAgent>();
            eliotAgent.enabled = true;
            eliotAgent.AgentReset();
            if (agent.GetComponent<AgentDeathHandler>().newPositionOnRespawn)
            {
                var position = this.wayPointsGroup.RandomPoint();
                var rotation = new Quaternion(0, Random.Range(0, 2 * Mathf.PI), 0, 1);
                agent.transform.position = position;
                agent.transform.rotation = rotation;
            }

            agent.gameObject.SetActive(true);
            
            if(onSpawnAgent != null) onSpawnAgent.Invoke(eliotAgent);
            
            AgentsPool.Remove(agent);
        }

        /// <summary>
        /// Reuse an agent from cache instead of instantiating a new one.
        /// </summary>
        /// <param name="pooledObjectsList"></param>
        private void SpawnAgentFromPool(List<GameObject> pooledObjectsList)
        {
            var agent = RandomAgentFromPrefabsList(pooledObjectsList);
            if (!agent) return;
            agent.GetComponent<EliotAgent>().AgentReset();
            if (agent.GetComponent<AgentDeathHandler>().newPositionOnRespawn)
            {
                var position = this.wayPointsGroup.RandomPoint();
                var rotation = new Quaternion(0, Random.Range(0, 2 * Mathf.PI), 0, 1);
                agent.transform.position = position;
                agent.transform.rotation = rotation;
            }

            agent.gameObject.SetActive(true);
            AgentsPool.Remove(agent);
        }

        /// <summary>
        /// Spawn an agent considering he pool handler setup.
        /// </summary>
        public void SpawnAgent()
        {
            if (AgentsPool.Count > 0)
            {
                SpawnAgentFromPool();
            }
            else
            {
                var agentPrefab = RandomAgentFromPrefabsList();
                if(agentPrefab) InstantiateNewAgent(agentPrefab);
            }
        }

        /// <summary>
        /// Spawn an agent considering he pool handler setup.
        /// </summary>
        /// <param name="pooledObjectsList"></param>
        public void SpawnAgent(List<GameObject> pooledObjectsList)
        {
            if (AgentsPool.Count > 0)
            {
                SpawnAgentFromPool(pooledObjectsList);
            }
            else
            {
                InstantiateNewAgent(RandomAgentFromPrefabsList(pooledObjectsList));
            }
        }

        /// <summary>
        /// Add an agent to cache to reuse it later.
        /// </summary>
        /// <param name="agent"></param>
        public void AddAgentToPool(EliotAgent agent)
        {
            if (!AgentsPool.Contains(agent.gameObject))
            {
                AgentsPool.Add(agent.gameObject);
            }
            agent.gameObject.SetActive(false);
        }

        /// <summary>
        /// Update the pool handler.
        /// </summary>
        public void Update()
        {
            if (poolingMode == WayPointsGroup.PoolingMode.Continuous ||
                poolingMode == WayPointsGroup.PoolingMode.ContinuousAndWaves)
            {
                if (poolAgents && wayPointsGroup.AgentsNumber() == maxAgentsNumber)
                {
                    _lastTimePooledAgent = Time.time;
                }

                if (poolAgents && wayPointsGroup.AgentsNumber() < maxAgentsNumber
                               && Time.time > _lastTimePooledAgent + agentsPoolCoolDown)
                {
                    _lastTimePooledAgent = Time.time;
                    SpawnAgent();
                }
            }

            if (!WavesInitiated)
            {
                if (poolingMode == WayPointsGroup.PoolingMode.Waves ||
                    poolingMode == WayPointsGroup.PoolingMode.ContinuousAndWaves)
                {
                    WavesHandler.InitWaves();
                    WavesInitiated = true;
                }
            }
        }
    }
}