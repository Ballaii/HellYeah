using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// A scriptable template for a group of resources.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "New Resources Profile", menuName = "Eliot AI/Resources Profile")]
    public class ResourcesProfileScriptable : ScriptableObject
    {
        /// <summary>
        /// Get the list of resources.
        /// </summary>
        public List<ResourceScriptable> resources
        {
            get
            {
#if UNITY_EDITOR
                if (_resources == null || _resources.Count == 0)
                {
                    LoadResources();
                }
#endif

                return _resources;
            }
            set { _resources = value; }
        }

        /// <summary>
        /// The list of resources.
        /// </summary>
        private List<ResourceScriptable> _resources = new List<ResourceScriptable>();

        /// <summary>
        /// Find a resource by name.
        /// </summary>
        /// <param name="resourceName"></param>
        public ResourceScriptable this[string resourceName]
        {
            get
            {
                if (resources.Count == 0) return null;
                foreach (var resource in resources)
                {
                    if (resource.name == resourceName)
                        return resource;
                }

                return null;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Add a new resource to the list.
        /// </summary>
        public void AddNewResource()
        {
            var newResource = CreateInstance<ResourceScriptable>();
            resources.Add(newResource);
            try
            {
                AssetDatabase.AddObjectToAsset(newResource, this);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(newResource));
            }
            catch (System.Exception)
            {
            }
        }

        /// <summary>
        /// Remove the resource from the list and destroy the scriptable object associated with it.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveResource(int index)
        {
            Undo.DestroyObjectImmediate(resources[index]);
            resources.RemoveAt(index);
        }

        /// <summary>
        /// Execute on selecting the object.
        /// </summary>
        public void OnEnable()
        {
            hideFlags = HideFlags.None;
            LoadResources();
        }

        /// <summary>
        /// Load the resources list from children scriptables.
        /// </summary>
        public void LoadResources()
        {
            UnityEngine.Object[] data = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
            foreach (var d in data)
            {
                if (d is ResourceScriptable)
                    if (!_resources.Contains(d as ResourceScriptable))
                    {
                        _resources.Add(d as ResourceScriptable);
                    }
            }
        }
#endif
    }
}