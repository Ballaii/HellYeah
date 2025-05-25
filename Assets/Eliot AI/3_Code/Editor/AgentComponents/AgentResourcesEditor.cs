using System;
using System.Collections.Generic;
using Eliot.Utility;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eliot.AgentComponents.Editor
{
    /// <summary>
    /// Editor script for the Agent Resources component.
    /// </summary>
    [System.Serializable]
    public class AgentResourcesEditor : AgentComponentEditor
    {
        /// <summary>
        /// Serialized Properties to draw and other variables.
        /// </summary>

        #region Variables

        public ReorderableList reorderableListResources;
        
        public Eliot.AgentComponents.AgentResources _agentResources;

        [SerializeField] public ResourcesProfile currentResourcesProfile;

        [SerializeField] private Dictionary<Resource, bool> unfoldEvents = new Dictionary<Resource, bool>();

        [SerializeField] private Dictionary<Resource, ResourceEventsDrawHelper> eventsDrawers =
            new Dictionary<Resource, ResourceEventsDrawHelper>();

        private List<ResourcesProfileScriptable>
            containedListResourcesProfiles = new List<ResourcesProfileScriptable>();

        #endregion

        public AgentResourcesEditor(AgentResources agentResources) : base(agentResources)
        {
            _agentResources = agentResources;
            serializedObject = new SerializedObject(_agentResources);
        }

        /// <summary>
        /// Get or construct the event drawer object for resource's events.
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        private ResourceEventsDrawHelper GetEventDrawer(Resource resource)
        {
            if (resource == null)
            {
                return null;
            }

            if (eventsDrawers == null)
            {
                eventsDrawers = new Dictionary<Resource, ResourceEventsDrawHelper>();
            }

            if (!eventsDrawers.ContainsKey(resource))
            {
                var newDrawHelper = ScriptableObject.CreateInstance<ResourceEventsDrawHelper>();
                newDrawHelper.BindResource(resource);
                eventsDrawers.Add(resource, newDrawHelper);

                return newDrawHelper;
            }
            
            if (!eventsDrawers[resource])
            {
                var newDrawHelper = ScriptableObject.CreateInstance<ResourceEventsDrawHelper>();
                newDrawHelper.BindResource(resource);
                eventsDrawers[resource] = newDrawHelper;
            }

            return eventsDrawers[resource];
        }

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        public override void OnEnable()
        {
            if (unfoldEvents == null) unfoldEvents = new Dictionary<Resource, bool>();
            
            if (_agentResources.resourcesProfiles == null) return;
            for (int i = 0; i < _agentResources.resourcesProfiles.Count; i++)
            {
                if (!_agentResources.resourcesProfiles[i].source) continue;
                for (int j = 0; j < _agentResources.resourcesProfiles[i].source.resources.Count; j++)
                {
                    if (_agentResources.resourcesProfiles[i].resources.Count <= j) continue;
                    if (_agentResources.resourcesProfiles[i].resources[j] != null)
                    {
                        GetEventDrawer(_agentResources.resourcesProfiles[i].resources[j]).onReplenish =
                            _agentResources.resourcesProfiles[i].resources[j].onReplenish;
                        GetEventDrawer(_agentResources.resourcesProfiles[i].resources[j]).onReduce =
                            _agentResources.resourcesProfiles[i].resources[j].onReduce;
                        GetEventDrawer(_agentResources.resourcesProfiles[i].resources[j]).onFull =
                            _agentResources.resourcesProfiles[i].resources[j].onFull;
                        GetEventDrawer(_agentResources.resourcesProfiles[i].resources[j]).onEmpty =
                            _agentResources.resourcesProfiles[i].resources[j].onEmpty;
                    }
                }
            }

            if (reorderableListResources == null)
                InitReorderableListResources();
        }

        /// <summary>
        /// Initialize the list of resources.
        /// </summary>
        private void InitReorderableListResources()
        {
            reorderableListResources =
                new ReorderableList( _agentResources.resourcesProfiles, typeof(ResourcesProfile), true, true, true, true);

            reorderableListResources.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Resources Profiles");
            };

            reorderableListResources.drawElementCallback = (rect, index, active, focused) =>
            {
                var curItem = reorderableListResources.list[index] as ResourcesProfile;
                if (curItem == null) return;
                curItem.source = (ResourcesProfileScriptable) EditorGUI.ObjectField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Source " + (index + 1).ToString(), curItem.source, typeof(ResourcesProfileScriptable), false);
            };

            reorderableListResources.elementHeight =
                EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            reorderableListResources.onAddCallback = list => list.list.Add(new ResourcesProfile());

            reorderableListResources.onSelectCallback = list =>
            {
                currentResourcesProfile = (ResourcesProfile) list.list[list.index];
            };
        }

        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public override void DrawInspector(AgentComponent agentResources)
        {
            _agentResources = (AgentResources) agentResources;

            base.DrawInspector(agentResources);

            agentResources.displayEditor = EliotEditorUtility.DrawAgentComponentHeader<AgentResources>(
                "Eliot Agent Component: " + "Resources", agentResources.displayEditor, agentResources,
                EliotGUISkin.AgentComponentGreen);
            if (!agentResources.displayEditor) return;
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            DrawReorderableListResources(_agentResources);
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Initialize the reorderable list for agent resources profiles.
        /// </summary>
        /// <param name="agentResources"></param>
        public void DrawReorderableListResources(AgentComponent agentResources)
        {
            if(serializedObject==null)
                serializedObject = new SerializedObject(agentResources);
            
            EditorGUI.BeginChangeCheck();
            
            if (_agentResources.resourcesProfiles == null) _agentResources.resourcesProfiles = new List<ResourcesProfile>();

            if (reorderableListResources == null)
            {
                InitReorderableListResources();
            }

            try
            {
                reorderableListResources.DoLayoutList();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
            containedListResourcesProfiles = new List<ResourcesProfileScriptable>();

            for (int i = _agentResources.resourcesProfiles.Count - 1; i >= 0; i--)
            {
                if (containedListResourcesProfiles.Contains(_agentResources.resourcesProfiles[i].source))
                {
                    Debug.LogWarning("Agent " + agentResources.gameObject.name + " already contains the '" +
                                     _agentResources.resourcesProfiles[i].source.name + "' Resources Profile.");
                    _agentResources.resourcesProfiles.RemoveAt(i);
                }
                else
                {
                    containedListResourcesProfiles.Add(_agentResources.resourcesProfiles[i].source);
                }
            }

            if (reorderableListResources.count > 0 &&
                currentResourcesProfile != null && currentResourcesProfile.source &&
                currentResourcesProfile.source.resources != null &&
                currentResourcesProfile.source.resources.Count > 0)
            {

                EditorGUI.indentLevel++;


                currentResourcesProfile.displayResources =
                    EditorGUILayout.Foldout(currentResourcesProfile.displayResources, "Resources");
                if (currentResourcesProfile.displayResources)
                {
                    if (currentResourcesProfile.resources == null || !currentResourcesProfile.MatchesSource())
                    {
                        currentResourcesProfile.resources = new List<Resource>();
                        for (int i = 0; i < currentResourcesProfile.source.resources.Count; i++)
                        {
                            currentResourcesProfile.resources.Add(currentResourcesProfile.source.resources[i].Clone());
                            unfoldEvents.Add(currentResourcesProfile.resources[i], false);
                        }
                    }

                    var sourceResources = currentResourcesProfile.source.resources;
                    if (sourceResources != null && sourceResources.Count > 0)
                    {
                        for (int i = 0; i < sourceResources.Count; i++)
                        {
                            #region Name

                            EditorGUILayout.BeginHorizontal();
                            var fieldWidth = EditorGUIUtility.fieldWidth;
                            EditorGUIUtility.fieldWidth = EditorGUIUtility.currentViewWidth * 0.4f;
                            EditorGUILayout.LabelField(sourceResources[i].name, EditorStyles.boldLabel);
                            var curResource = currentResourcesProfile.resources[i];
                            if (curResource == null) continue;
                            EditorGUIUtility.fieldWidth = fieldWidth;

                            if (GUILayout.Button("reset"))
                            {
                                curResource = sourceResources[i].Clone();
                            }

                            EditorGUILayout.EndHorizontal();

                            #endregion

                            #region MinValue

                            EditorGUI.indentLevel++;
                            curResource.minValue = EditorGUILayout.IntField("Min Value", curResource.minValue);

                            #endregion

                            #region MaxValue

                            curResource.maxValue = EditorGUILayout.IntField("Max Value", curResource.maxValue);

                            #endregion

                            #region InitialValue

                            curResource.initialValue = EditorGUILayout.IntSlider("Initial Value",
                                curResource.initialValue, curResource.minValue, curResource.maxValue);

                            #endregion

                            #region CurrentValue

                            if (Application.isPlaying)
                            {
                                curResource.currentValue = EditorGUILayout.IntSlider("Current Value",
                                    curResource.currentValue, curResource.minValue, curResource.maxValue);
                            }

                            #endregion

                            #region ReplenishAmount

                            curResource.replenishAmount =
                                EditorGUILayout.IntField("Replenish Amount", curResource.replenishAmount);

                            #endregion

                            #region ReplenishCooldown

                            curResource.replenishCooldown =
                                EditorGUILayout.FloatField("Replenish Cooldown", curResource.replenishCooldown);

                            #endregion
                            
                            #region DieOnEmpty

                            AgentDeathHandler deathHandler = null;
                            if(_agentResources) deathHandler = _agentResources.GetComponent<AgentDeathHandler>();
                            if (deathHandler)
                            {
                                curResource.dieOnEmpty =
                                    EditorGUILayout.Toggle("Die On Empty", curResource.dieOnEmpty);
                            }

                            #endregion

                            #region Events

                            if (!unfoldEvents.ContainsKey(curResource))
                                unfoldEvents.Add(curResource, false);
                            unfoldEvents[curResource] =
                                EditorGUILayout.Foldout(unfoldEvents[curResource], "Events");
                            if (unfoldEvents[curResource])
                            {
                                var eventDrawer = GetEventDrawer(curResource);
                                if (eventDrawer)
                                {
                                    var serializableResource = new SerializedObject(GetEventDrawer(curResource));

                                    #region OnReplenish

                                    var __tmp_onReplenish = serializableResource.FindProperty("onReplenish");
                                    EditorGUILayout.PropertyField(__tmp_onReplenish, true);

                                    #endregion

                                    #region OnReduce

                                    var __tmp_onReduce = serializableResource.FindProperty("onReduce");
                                    EditorGUILayout.PropertyField(__tmp_onReduce);

                                    #endregion

                                    #region OnFull

                                    var __tmp_onFull = serializableResource.FindProperty("onFull");
                                    EditorGUILayout.PropertyField(__tmp_onFull);

                                    #endregion

                                    #region OnEmpty

                                    var __tmp_onEmpty = serializableResource.FindProperty("onEmpty");
                                    EditorGUILayout.PropertyField(__tmp_onEmpty);

                                    #endregion

                                    serializableResource.ApplyModifiedProperties();
                                }
                                else
                                {
                                    Debug.Log("EventDrawer could not be created properly");
                                }
                            }

                            #endregion

                            EditorGUI.indentLevel--;

                            currentResourcesProfile.resources[i] = curResource;
                        }

                        if (GUILayout.Button("reset all"))
                        {
                            currentResourcesProfile.resources = new List<Resource>();
                            for (int i = 0; i < currentResourcesProfile.source.resources.Count; i++)
                            {
                                currentResourcesProfile.resources.Add(currentResourcesProfile.source.resources[i]
                                    .Clone());
                            }
                        }
                    }

                    if (reorderableListResources.list != null &&
                        reorderableListResources.list.Count > reorderableListResources.index &&
                        currentResourcesProfile != null && reorderableListResources.index >= 0)
                    {
                        reorderableListResources.list[reorderableListResources.index] = currentResourcesProfile;
                        _agentResources.resourcesProfiles[reorderableListResources.index] = currentResourcesProfile;
                    }
                }
                else
                {
                    if (currentResourcesProfile.resources == null || !currentResourcesProfile.MatchesSource())
                    {
                        currentResourcesProfile.resources = new List<Resource>();
                        for (int i = 0; i < currentResourcesProfile.source.resources.Count; i++)
                        {
                            currentResourcesProfile.resources.Add(currentResourcesProfile.source.resources[i].Clone());
                        }
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                for (int i = 0; i < _agentResources.resourcesProfiles.Count; i++)
                {
                    try
                    {
                        if (!_agentResources.resourcesProfiles[i].source) continue;
                        for (int j = 0; j < _agentResources.resourcesProfiles[i].source.resources.Count; j++)
                        {
                            if (_agentResources.resourcesProfiles[i].resources[j] != null)
                            {
                                _agentResources.resourcesProfiles[i].resources[j].onReplenish =
                                    GetEventDrawer(_agentResources.resourcesProfiles[i].resources[j]).onReplenish;
                                _agentResources.resourcesProfiles[i].resources[j].onReduce =
                                    GetEventDrawer(_agentResources.resourcesProfiles[i].resources[j]).onReduce;
                                _agentResources.resourcesProfiles[i].resources[j].onFull =
                                    GetEventDrawer(_agentResources.resourcesProfiles[i].resources[j]).onFull;
                                _agentResources.resourcesProfiles[i].resources[j].onEmpty =
                                    GetEventDrawer(_agentResources.resourcesProfiles[i].resources[j]).onEmpty;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }
                
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(agentResources);

                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
    }
}
