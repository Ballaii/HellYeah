using System.Collections;
using System.Collections.Generic;
using Eliot.Environment.Editor;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace Eliot.Environment
{
    /// <summary>
    /// Holds information about a single wave of agents to be pooled.
    /// </summary>
    [System.Serializable]
    public class WayPointsSpawningWave
    {
        /// <summary>
        /// Possible wave states.
        /// </summary>
        public enum WaveState
        {
            Loading,
            Spawning,
            CoolDown
        }

        /// <summary>
        /// Current state of the wave.
        /// </summary>
        public WaveState state;
        
        /// <summary>
        /// Used to identify the wave easier in the Inspector.
        /// </summary>
        public string DescriptiveName = "";
        
        /// <summary>
        /// Whether to use the way points group's prefabs or to override them.
        /// </summary>
        public bool UsePoolPrefabs = true;
        
        /// <summary>
        /// Prefabs to override with.
        /// </summary>
        public List<GameObject> SpawnedPrefabs = new List<GameObject>();
        
        /// <summary>
        /// How many agents to spawn.
        /// </summary>
        public int NumberSpawned = 1;
        
        /// <summary>
        /// How much time to wait before starting to spawn agents.
        /// </summary>
        public float WaveDelay = 0f;
        
        /// <summary>
        /// How much time to be spawning agents.
        /// </summary>
        public float SpawningDuration = 0f;
        
        /// <summary>
        /// How much time to wait before switching to the next wave.
        /// </summary>
        public float WaveCooldown = 10f;
        
        /// <summary>
        /// How many times to run the wave before going to the next one.
        /// </summary>
        public int NumbersRepeat = 1;

        /// <summary>
        /// Whether the wave is doing its job at the moment.
        /// </summary>
        private bool _isSpawning = false;

        /// <summary>
        /// Editor only. Whether to display the spawned prefabs list.
        /// </summary>
        private bool _showPrefs = false;

        /// <summary>
        /// Editor only. Calculated height of inspector GUI.
        /// </summary>
        public float EditorHeight = 0;

        /// <summary>
        /// Temp variable used to display the spawned prefabs list.
        /// </summary>
        private int _spawnedPrefabsCapacity = 0;

        /// <summary>
        /// Action to do on wave start or on transition to this wave.
        /// </summary>
        public UnityEvent onWaveStart;
        
        /// <summary>
        /// Action to do after the delay has been passed.
        /// </summary>
        public UnityEvent onWaveDelayPassed;
        
        /// <summary>
        /// Action to do every time an agent gets spawned.
        /// </summary>
        public UnityEvent onAgentSpawned;
        
        /// <summary>
        /// Action to do when all the agents have been spawned.
        /// </summary>
        public UnityEvent onWaveEnd;
        
        /// <summary>
        /// Action to do right before transitioning to the next wave.
        /// </summary>
        public UnityEvent onWaveCooldownPassed;

        /// <summary>
        /// A temporary utility object that allows drawing UnityEvents for the wave in the Inspector.
        /// </summary>
        private WaveEventsDrawHelper _eventsDrawHelper;
        
        /// <summary>
        /// Use handler to spawn a new agent.
        /// </summary>
        /// <param name="wavesHandler"></param>
        public void Spawn(WayPointsSpawningWavesHandler wavesHandler)
        {
            if (!_isSpawning)
            {
                _isSpawning = true;
                wavesHandler.WayPointsGroup.StartCoroutine(SpawnEnum(wavesHandler));
            }
        }

        /// <summary>
        /// Handle the continuous process of spawning agents.
        /// </summary>
        /// <param name="wavesHandler"></param>
        /// <returns></returns>
        public IEnumerator SpawnEnum(WayPointsSpawningWavesHandler wavesHandler)
        {
            if (NumbersRepeat <= 0) NumbersRepeat = 1;
            for (var i = 0; i < NumbersRepeat; i++)
            {
                state = WaveState.Loading;
                
                if(onWaveStart != null) onWaveStart.Invoke();
                yield return new WaitForSeconds(WaveDelay);
                if(onWaveDelayPassed != null) onWaveDelayPassed.Invoke();

                state = WaveState.Spawning;
                    
                int numSpawned = 0;
                float interval = SpawningDuration / (float) NumberSpawned;
                while (numSpawned < NumberSpawned)
                {
                    if (UsePoolPrefabs)
                    {
                        wavesHandler.WayPointsGroup.SpawnAgent();
                    }
                    else
                    {
                        wavesHandler.WayPointsGroup.SpawnAgent(SpawnedPrefabs);
                    }
                    if(onAgentSpawned != null) onAgentSpawned.Invoke();

                    numSpawned++;
                    yield return new WaitForSeconds(interval);
                }
                
                if(onWaveEnd != null) onWaveEnd.Invoke();
                
                state = WaveState.CoolDown;
                
                yield return new WaitForSeconds(WaveCooldown);
                if(onWaveCooldownPassed != null) onWaveCooldownPassed.Invoke();
            }

            _isSpawning = false;
            wavesHandler.NextWave();
        }
        
        public void OnEnable()
        {
            if (SpawnedPrefabs != null && SpawnedPrefabs.Capacity > _spawnedPrefabsCapacity)
                _spawnedPrefabsCapacity = SpawnedPrefabs.Capacity;
        }

#if UNITY_EDITOR

        private WaveEventsDrawHelper GetEventDrawer()
        {

            if (_eventsDrawHelper == null)
            {
                _eventsDrawHelper = ScriptableObject.CreateInstance<WaveEventsDrawHelper>();
                _eventsDrawHelper.BindWave(this);
            }
            
            return _eventsDrawHelper;
        }

        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public void DrawEditor(SerializedObject serializedObject, int index)
        {
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.LabelField("Wave " + index
#if UNITY_2019
                , EditorStyles.foldoutHeader
#else
                , EditorStyles.boldLabel
#endif
            );

            DescriptiveName = EditorGUILayout.TextField("Descriptive Name", DescriptiveName);

            UsePoolPrefabs = EditorGUILayout.Toggle("Use Pool Prefabs", UsePoolPrefabs);

            if (!UsePoolPrefabs)
            {
                _showPrefs = EditorGUILayout.Foldout(_showPrefs, "Spawned Prefabs");
                if (_showPrefs)
                {
                    EditorGUI.indentLevel++;
                    _spawnedPrefabsCapacity = EditorGUILayout.IntField("Size", _spawnedPrefabsCapacity);
                    if (SpawnedPrefabs.Count <= _spawnedPrefabsCapacity)
                        SpawnedPrefabs.Capacity = _spawnedPrefabsCapacity;
                    else
                    {
                        for (int i = SpawnedPrefabs.Count - 1; i >= _spawnedPrefabsCapacity; i--)
                        {
                            SpawnedPrefabs.RemoveAt(i);
                        }

                        SpawnedPrefabs.Capacity = _spawnedPrefabsCapacity;
                    }

                    for (var i = 0; i < SpawnedPrefabs.Capacity; i++)
                    {
                        if (SpawnedPrefabs.Count <= i)
                        {
                            SpawnedPrefabs.Add(null);
                        }

                        SpawnedPrefabs[i] = (GameObject) EditorGUILayout.ObjectField("Element " + i,
                            SpawnedPrefabs[i], typeof(GameObject), true);
                    }

                    EditorGUI.indentLevel--;
                }
            }

            NumberSpawned = EditorGUILayout.IntField("Number Spawned", NumberSpawned);
            WaveDelay = EditorGUILayout.FloatField("Wave Delay", WaveDelay);
            SpawningDuration =
                EditorGUILayout.FloatField("Spawning Duration", SpawningDuration);
            WaveCooldown = EditorGUILayout.FloatField("Wave Cooldown", WaveCooldown);
            NumbersRepeat = EditorGUILayout.IntField("Numbers Repeat", NumbersRepeat);
            if (NumbersRepeat < 0) NumbersRepeat = 0;
            
            
            var serializableWaveEvents = new SerializedObject(GetEventDrawer());
            EditorGUILayout.PropertyField(serializableWaveEvents.FindProperty("onWaveStart"), true);
            EditorGUILayout.PropertyField(serializableWaveEvents.FindProperty("onWaveDelayPassed"), true);
            EditorGUILayout.PropertyField(serializableWaveEvents.FindProperty("onAgentSpawned"), true);
            EditorGUILayout.PropertyField(serializableWaveEvents.FindProperty("onWaveEnd"), true);
            EditorGUILayout.PropertyField(serializableWaveEvents.FindProperty("onWaveCooldownPassed"), true);
            serializableWaveEvents.ApplyModifiedProperties();
            this.onWaveStart = GetEventDrawer().onWaveStart;
            this.onWaveDelayPassed = GetEventDrawer().onWaveDelayPassed;
            this.onAgentSpawned = GetEventDrawer().onAgentSpawned;
            this.onWaveEnd = GetEventDrawer().onWaveEnd;
            this.onWaveCooldownPassed = GetEventDrawer().onWaveCooldownPassed;
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndVertical();
        }
#endif
    }
}