#pragma warning disable CS0414, CS0649, CS0612, CS1692, CS0219
using UnityEngine.AI;
#if UNITY_EDITOR
#pragma warning disable CS0414, CS1692
using System.Collections.Generic;
using Eliot.Environment;
using UnityEditor;
using UnityEngine;
using Eliot.BehaviourEditor;

namespace Eliot.AgentComponents.Editor
{
    /// <summary>
    /// Editor window that helps constructing new Agents.
    /// </summary>
    public class CreateAgentWindow : EditorWindow
    {
        public bool addPerception = false;
        public bool addMotion = false;
        public bool addResources = false;
        public bool addDeathHandler = false;
        public bool addAnimation = false;
        public bool addInventory = false;

        /// Name of an Agent.
        private string _name = "";
        /// Team of an Agent.
        private string _team = "";
        /// Team of an Agent.
        private float _weight = 1;
        /// How far can Agent see.
        private float _seeDistance = 10;
        /// Field of view of Agent's perception.
        private float _seeFOV = 180;
        /// Resolution of Agent's perception.
        private UnitFactoryOptions _seeResolution = UnitFactoryOptions.Medium;
        /// Wheather the Agent can move.
        private bool _canMove = true;

        private MotionEngine _motionEngine;
        /// Speed with which the Agent can move.
        private UnitFactoryOptions _moveSpeed = UnitFactoryOptions.Medium;
        /// Whether the Agent can run.
        private bool _canRun = true;
        /// Whether the Agent can dodge.
        private bool _canDodge;
        /// For how long can Agent remember his targets.
        private int _memoryDuration = 10;
        /// Whether the Agent uses health as a resource.
        private bool _isMortal = true;
        /// Maximum health capacity.
        private int _maxHealth = 10;
        /// Whether the Agent uses energy as a resource.
        private bool _useEnergy;
        /// Maximum energy capacity.
        private int _maxEnergy = 10;
        /// Agent's graphics.
        private GameObject _graphics;
        /// Agent's ragdoll.
        private GameObject _ragdoll;
        /// Agent's Behaviour model.
        private EliotBehaviour _behaviour;
        /// Agent's waypoints group.
        private WayPointsGroup _wayPoints;
        /// Agent's initial skills.
        private List<Skill> _skills = new List<Skill>();
        
        private List<AttributesGroupScriptable> _attributesGroups = new List<AttributesGroupScriptable>();
        /// Position of the scroll rect.
        private static Vector2 _scrollPosition = Vector2.zero;

        private AgentPerception.Dimensions _perceptionDimensions;
        private AgentPerception.Modes _perceptionMode;
        
        private List<ResourcesProfileScriptable> _resourcesProfiles = new List<ResourcesProfileScriptable>();
        
        #region INTERNAL ENUMS
        /// <summary>
        /// Enumerates options for some factory parameters.
        /// </summary>
        private enum UnitFactoryOptions {Low, Medium, High}
        #endregion
        
        /// <summary>
        /// Initialize new factory window.
        /// </summary>
        [MenuItem("Tools/Eliot AI/Create/Agent")]
        private static void InitWindow()
        {
            GetWindowWithRect<CreateAgentWindow>(new Rect(750, 250, 350, 550), true, "Create Agent");
        }

        private static void InstantiateAgent(string prefabName)
        {
            var baseAgentPrefabId = AssetDatabase.FindAssets(prefabName);
            if (baseAgentPrefabId.Length == 0)
            {
                Debug.LogWarning("There is no '" + prefabName + "' prefab in the project. Please consider reloading the package.");
                return;
            }
            var baseAgentPrefabPath = AssetDatabase.GUIDToAssetPath(baseAgentPrefabId[0]);
            var baseAgentPrefab = AssetDatabase.LoadAssetAtPath (baseAgentPrefabPath, typeof(GameObject)) as GameObject;
            Vector3 spawnPosition = Vector3.zero;
            try
            {
                spawnPosition = SceneView.lastActiveSceneView.pivot;
            }
            catch (System.Exception){ }
            var baseAgent = GameObject.Instantiate(baseAgentPrefab, spawnPosition, Quaternion.identity);
            baseAgent.name = prefabName;
            Selection.activeObject = baseAgent;
        }

        [MenuItem("GameObject/Eliot AI/Base NPC", false, 0)]
        private static void InstantiateBasicAgent()
        {
            InstantiateAgent(EliotProjectSettings.BaseNPCAgentPrefabName);
        }
        
        [MenuItem("GameObject/Eliot AI/Base NPC - Skeleton", false, 0)]
        private static void InstantiateBasicSkeletonAgent()
        {
            InstantiateAgent(EliotProjectSettings.BaseNPCAgentSkeletonPrefabName);
        }
        
        [MenuItem("GameObject/Eliot AI/Base TPS Player", false, 0)]
        private static void InstantiateBasicPlayerAgent()
        {
            InstantiateAgent(EliotProjectSettings.BaseTPSPlayerPrefab);
        }
        
        [MenuItem("GameObject/Eliot AI/Base TPS Player - Soldier", false, 0)]
        private static void InstantiateBasicPlayerSoldierAgent()
        {
            InstantiateAgent(EliotProjectSettings.BaseTPSPlayerSoldierPrefab);
        }

        /// <summary>
        /// Draw all the window graphics.
        /// </summary>
        private void OnGUI()
        {
            Tip("Create new Agent here giving some general idea of his \nparameters." +
                "You can always adjust them in the inspector \nof already created Agent.");
            Space();
            
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            Header("General");
            _name = EditorGUILayout.TextField("Agent's name: ", _name);
            _team = EditorGUILayout.TextField("Team: ", _team);
            _weight = EditorGUILayout.FloatField("Weight: ", _weight);
            Space(2);

            #region Attributes

            Header("Attributes");
            if(_attributesGroups.Count > 0)
                for (var i = _attributesGroups.Count-1; i >= 0; i--)
                {
                    EditorGUILayout.BeginHorizontal();
                    _attributesGroups[i] = (AttributesGroupScriptable) EditorGUILayout.ObjectField(_attributesGroups[i], typeof(AttributesGroupScriptable), false);
                    if(GUILayout.Button("Remove"))
                        _attributesGroups.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                }
            if (GUILayout.Button("Add attributes group"))
                _attributesGroups.Add(null);
            
            Space(2);
            
            #endregion
            
            addPerception = EditorGUILayout.ToggleLeft("Add Perception", addPerception);
            if (addPerception)
            {
                Header("Perception");
                _perceptionDimensions = (AgentPerception.Dimensions) EditorGUILayout.EnumPopup("Dimensions", _perceptionDimensions);
                _perceptionMode = (AgentPerception.Modes) EditorGUILayout.EnumPopup("Mode", _perceptionMode);
                _seeDistance = EditorGUILayout.FloatField("Perception _range:", _seeDistance);
                _seeFOV = EditorGUILayout.FloatField("Field of view: ", _seeFOV);
                _seeResolution =
                    (UnitFactoryOptions) EditorGUILayout.EnumPopup("Accuracy of perception: ", _seeResolution);
                _memoryDuration = EditorGUILayout.IntField("Memory duration: ", _memoryDuration);
            }

            Space(2);
            
            addMotion = EditorGUILayout.ToggleLeft("Add Motion", addMotion);
            if (addMotion)
            {
                Header("Motion");
                _motionEngine = (MotionEngine) EditorGUILayout.EnumPopup("Motion engine: ", _motionEngine);
                _moveSpeed = (UnitFactoryOptions) EditorGUILayout.EnumPopup("Move speed: ", _moveSpeed);
                _canRun = EditorGUILayout.Toggle("Can it run?", _canRun);
                _canDodge = EditorGUILayout.Toggle("Can it dodge?", _canDodge);
            }

            Space(2);
            
            addResources = EditorGUILayout.ToggleLeft("Add Resources", addResources);
            if (addResources)
            {
                Header("Resources");
                if(_resourcesProfiles.Count > 0)
                    for (var i = _resourcesProfiles.Count-1; i >= 0; i--)
                    {
                        EditorGUILayout.BeginHorizontal();
                        _resourcesProfiles[i] = (ResourcesProfileScriptable) EditorGUILayout.ObjectField(_resourcesProfiles[i], typeof(ResourcesProfileScriptable), false);
                        if(GUILayout.Button("Remove"))
                            _resourcesProfiles.RemoveAt(i);
                        EditorGUILayout.EndHorizontal();
                    }
                if (GUILayout.Button("Add resource profile"))
                    _resourcesProfiles.Add(null);
            
                EditorGUILayout.Space();
            }
            
            Space(2);
            
            addDeathHandler = EditorGUILayout.ToggleLeft("Add Death Handler", addDeathHandler);
            addInventory = EditorGUILayout.ToggleLeft("Add Inventory", addInventory);
            addAnimation = EditorGUILayout.ToggleLeft("Add Animation", addAnimation);

            Space(2);
            Header("Other");
            _graphics = (GameObject)EditorGUILayout.ObjectField("Graphics: ", _graphics, typeof(GameObject), false);
            _behaviour = (EliotBehaviour)EditorGUILayout.ObjectField("Behaviour: ", _behaviour, typeof(EliotBehaviour), false);
            _wayPoints = (WayPointsGroup)EditorGUILayout.ObjectField("Way Points: ", _wayPoints, typeof(WayPointsGroup), true);
            Space(2);
            Header("Skills");
            if(_skills.Count > 0)
                for (var i = _skills.Count-1; i >= 0; i--)
                {
                    EditorGUILayout.BeginHorizontal();
                    _skills[i] = (Skill) EditorGUILayout.ObjectField(_skills[i], typeof(Skill), false);
                    if(GUILayout.Button("Remove"))
                        _skills.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                }
            if (GUILayout.Button("Add skill"))
                _skills.Add(null);
            
            EditorGUILayout.Space();

            if (GUILayout.Button("Create"))
            {
                var newAgentGameObject = new GameObject(_name);
                var newAgent = newAgentGameObject.AddComponent<EliotAgent>();
                newAgent.Behaviour = _behaviour;
                newAgent.GetComponent<Unit>().Team = _team;
                newAgent.WayPoints = _wayPoints;
                newAgent.transform.position = SceneView.lastActiveSceneView.pivot;
                newAgent.Skills = _skills;

                var unit = newAgent.Unit;
                unit.attributesGroups = new List<AttributesGroup>();
                foreach (var sourceAttributesGroup in _attributesGroups)
                {
                    var newAttributesGroup = new AttributesGroup();
                    newAttributesGroup.source = sourceAttributesGroup;
                    unit.attributesGroups.Add(newAttributesGroup);
                }

                var newAgentTransform = newAgent.transform;
                var graphicsPos = new GameObject("__graphics__");
                graphicsPos.transform.parent = newAgentTransform;
                graphicsPos.transform.localPosition = new Vector3(0, 1, 0);

                var collider = newAgentGameObject.AddComponent<CapsuleCollider>();
                collider.center = new Vector3(0, 1, 0);

                // Instantiate graphics
                if (_graphics)
                {
                    var gr =
                        Instantiate(_graphics, graphicsPos.transform.position,
                            graphicsPos.transform.rotation) as GameObject;
                    gr.transform.parent = graphicsPos.transform;
                }

                #region Perception

                {
                    if (addPerception)
                    {
                        var agentPerception = newAgent.AddAgentComponent<AgentPerception>();
                        var memory = AgentMemory.CreateMemory(_memoryDuration);
                        agentPerception.AgentMemory = memory;
                        var resolution = 0;
                        switch (_seeResolution)
                        {
                            case UnitFactoryOptions.Low:
                                resolution = 7;
                                break;
                            case UnitFactoryOptions.Medium:
                                resolution = 15;
                                break;
                            case UnitFactoryOptions.High:
                                resolution = 35;
                                break;
                        }

                        agentPerception.Resolution = resolution;
                        agentPerception.Range = _seeDistance;
                        agentPerception.FieldOfView = _seeFOV;
                        agentPerception.PerceptionDimensions = _perceptionDimensions;
                        agentPerception.PerceptionMode = _perceptionMode;
                    }
                }

                #endregion

                #region Motion

                {
                    if (addMotion)
                    {
                        var agentMotion = newAgent.AddAgentComponent<AgentMotion>();
                        var speed = 0f;
                        switch (_moveSpeed)
                        {
                            case UnitFactoryOptions.Low:
                                speed = 1.5f;
                                break;
                            case UnitFactoryOptions.Medium:
                                speed = 3f;
                                break;
                            case UnitFactoryOptions.High:
                                speed = 5f;
                                break;
                        }

                        agentMotion.WalkSpeed = speed;
                        agentMotion.RunSpeed = _canRun ? speed * 2 : speed;
                        agentMotion.DodgeSpeed = _canDodge ? 10f : 0f;
                        agentMotion.DodgeDuration = _canDodge ? 0.1f : 0f;
                        agentMotion.Weight = _weight;

                        if (agentMotion.Type == MotionEngine.NavMesh) // No need to add NavMeshAgent if Agent is not a creature
                        {
                            var navMesh = newAgentGameObject.GetComponent<NavMeshAgent>();
                            if(!navMesh) navMesh = newAgentGameObject.AddComponent<NavMeshAgent>();
                            navMesh.acceleration = 100;
                            navMesh.angularSpeed = 1000;
                        }
                    }
                }

                #endregion

                #region Resources

                {
                    if (addResources)
                    {
                        var agentResources = newAgent.AddAgentComponent<AgentResources>();
                        agentResources.resourcesProfiles = new List<ResourcesProfile>();
                        foreach (var sourceProfile in _resourcesProfiles)
                        {
                            var newProfile = new ResourcesProfile();
                            newProfile.source = sourceProfile;
                            agentResources.resourcesProfiles.Add(newProfile);
                        }
                    }
                }

                #endregion

                #region DeathHandler

                {
                    if (addDeathHandler)
                    {
                        var agentDeathHandler = newAgent.AddAgentComponent<AgentDeathHandler>();
                    }
                }

                #endregion

                #region Inventory

                {
                    if (addInventory)
                    {
                        var agentInventory = newAgent.AddAgentComponent<AgentInventory>();
                    }
                }

                #endregion

                #region Animation

                {
                    if (addAnimation)
                    {
                        var agentAnimation = newAgent.AddAgentComponent<AgentAnimation>();
                    }
                }

                #endregion

                Selection.activeObject = newAgentGameObject;

            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Make some space in the editor.
        /// </summary>
        /// <param name="number"></param>
        private static void Space(int number = 1)
        {
            for(var i = 0; i < number; i++) 
                EditorGUILayout.Space();
        }

        /// <summary>
        /// Add text area to the editor with small font size to insert some tips for user.
        /// </summary>
        /// <param name="text"></param>
        private static void Tip(string text)
        {
            var labelSkin = GUI.skin.label;
            labelSkin.richText = true;
            EditorGUILayout.TextArea("<size=10>" + text + "</size>", labelSkin);
        }
        
        /// <summary>
        /// Insert some bold text in the editor.
        /// </summary>
        /// <param name="text"></param>
        private static void Header(string text)
        {
            var labelSkin = GUI.skin.label;
            labelSkin.richText = true;
            EditorGUILayout.TextArea("<b>" + text + "</b>", labelSkin);
        }
    }
}
#endif