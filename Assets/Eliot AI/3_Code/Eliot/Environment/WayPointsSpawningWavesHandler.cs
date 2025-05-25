using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using EditorGUILayout = UnityEditor.EditorGUILayout;
using UnityEditorInternal;
#endif

namespace Eliot.Environment
{
    /// <summary>
    /// Handles spawning agents in waves.
    /// </summary>
    [System.Serializable]
    public class WayPointsSpawningWavesHandler
    {
        /// <summary>
        /// The area to spawn agents within.
        /// </summary>
        [SerializeField] private WayPointsGroup _wayPointsGroup;

        /// <summary>
        /// Public getter for the WP group.
        /// </summary>
        public WayPointsGroup WayPointsGroup
        {
            get { return _wayPointsGroup; }
        }

        /// <summary>
        /// The list of waves.
        /// </summary>
        [SerializeField] public List<WayPointsSpawningWave> waves = new List<WayPointsSpawningWave>();
#if UNITY_EDITOR
        /// <summary>
        /// Pretty list of waves to draw.
        /// </summary>
        [SerializeField] private ReorderableList _wavesList = null;
#endif

        /// <summary>
        /// Editor only. Whether to display the gui.
        /// </summary>
        [SerializeField] private bool _displayWaves = false;

        /// <summary>
        /// Should the handler run the first wave after completing the last one?
        /// </summary>
        public bool Loop = false;

        /// <summary>
        /// The reference to the currently running wave.
        /// </summary>
        [SerializeField] private WayPointsSpawningWave _currentWave;
        
        /// <summary>
        /// The index of the currently running wave.
        /// </summary>
        private int _currentWaveIndex = 0;

        /// <summary>
        /// The reference to the currently running wave.
        /// </summary>
        public WayPointsSpawningWave CurrentWave
        {
            get { return _currentWave; }
            set { _currentWave = value; }
        }

        private WayPointsSpawningWave _currentlySelectedWave;
        private int _currentlySelectedWaveIndex = 0;

#if UNITY_EDITOR
        /// <summary>
        /// Initialize the list of waves.
        /// </summary>
        public ReorderableList WavesList
        {
            get
            {
                if (_wavesList == null)
                {
                    _wavesList = new ReorderableList(waves, typeof(WayPointsSpawningWave),
                        true, true, true, true);
                    _wavesList.drawHeaderCallback = rect => { UnityEditor.EditorGUI.LabelField(rect, "Waves"); };
                    _wavesList.drawElementCallback = (rect, index, active, focused) =>
                    {
                        //waves[index].DrawEditor(rect, index);
                        string descriptiveName = waves[index].DescriptiveName;
                        EditorGUI.LabelField(rect, 
                            "Wave " + (index+1).ToString() 
                                    + (string.IsNullOrEmpty(descriptiveName) ? "" : (": " + descriptiveName)) );
                    };
                    //_wavesList.elementHeightCallback = (index) => { return waves[index].EditorHeight; };
                    _wavesList.onSelectCallback = list =>
                    {
                        _currentlySelectedWaveIndex = list.index;
                        _currentlySelectedWave = waves[_currentlySelectedWaveIndex];
                    };
                }

                return _wavesList;
            }
        }
#endif
        
        /// <summary>
        /// Create the new waves handler.
        /// </summary>
        /// <param name="wayPointsGroup"></param>
        public WayPointsSpawningWavesHandler(WayPointsGroup wayPointsGroup)
        {
            _wayPointsGroup = wayPointsGroup;
            _currentWaveIndex = 0;
            if (waves.Count > 0)
                _currentWave = waves[_currentWaveIndex];
        }

        /// <summary>
        /// get the next wave.
        /// </summary>
        public void NextWave()
        {
            if (Loop)
            {
                if (_currentWaveIndex < waves.Count - 1)
                {
                    _currentWave = waves[++_currentWaveIndex];
                }
                else
                {
                    _currentWaveIndex = 0;
                    _currentWave = waves[_currentWaveIndex];
                }

                _currentWave.Spawn(_wayPointsGroup.agentsPoolHandler.WavesHandler);
            }
            else
            {
                if (_currentWaveIndex < waves.Count - 1)
                {
                    _currentWave = waves[++_currentWaveIndex];
                    _currentWave.Spawn(_wayPointsGroup.agentsPoolHandler.WavesHandler);
                }
            }
        }
        
        /// <summary>
        /// Initialize the waves handler with a way points group.
        /// </summary>
        /// <param name="wayPointsGroup"></param>
        public void Init(WayPointsGroup wayPointsGroup)
        {
            this._wayPointsGroup = wayPointsGroup;
        }

        /// <summary>
        /// Start running the waves.
        /// </summary>
        public void InitWaves()
        {
            if (waves.Count == 0) return;
            CurrentWave = waves[0];
            CurrentWave.Spawn(_wayPointsGroup.agentsPoolHandler.WavesHandler);
        }

        public void OnEnable()
        {
            foreach (var wave in waves)
            {
                wave.OnEnable();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public void DrawEditor(SerializedObject serializedObject)
        {
            _displayWaves = EditorGUILayout.Foldout(_displayWaves, "Configure Waves");
            if (_displayWaves)
            {
                Loop = EditorGUILayout.Toggle("Loop", Loop);
                WavesList.DoLayoutList();
            }

            if (_currentlySelectedWave != null)
            {
                _currentlySelectedWave.DrawEditor(serializedObject, _currentlySelectedWaveIndex + 1);
            }

            if (_currentWave == null) return;
            
            if (Application.isPlaying)
            {
                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Current Wave: Wave " + (_currentWaveIndex + 1) + 
                                           (string.IsNullOrEmpty(_currentWave.DescriptiveName) ? "" : (" - " + _currentWave.DescriptiveName)) );
                EditorGUILayout.LabelField("Wave " + (_currentWaveIndex + 1) + " State: " + _currentWave.state.ToString());
                EditorGUILayout.EndVertical();
            }
        }
#endif
    }
}