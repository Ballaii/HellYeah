#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace Eliot
{
    /// <summary>
    /// Eliot Nodes and Eliot Node Settings class houses the static node settings and variables used throughout the behaviour system.
    /// 
    /// benjamin.erwin@subliminumindustries.com 26.11.19 (c) Subliminum Industries
    /// 
    /// Eliot Copyright boilerplate should be added
    /// </summary>
    public static class EliotNodes
    {
        private static float utilitySquareWidth = 146;

        /// <summary>
        /// The list of settings for all the nodes.
        /// </summary>
        public static List<EliotNodeSettings> NodesSettings
        {
            get;

            private set;
        }

        public static EliotNodeSettings Entry
        {
            get;
            private set;
        }

        /// <summary>
        /// Default settings for the Action node.
        /// </summary>
        public static EliotNodeSettings Action
        {
            get;
            private set;
        }

        /// <summary>
        /// Default settings for the Condition node.
        /// </summary>
        public static EliotNodeSettings Condition
        {
            get;
            private set;
        }

        /// <summary>
        /// Default settings for the Skill node.
        /// </summary>
        public static EliotNodeSettings Skill
        {
            get;
            private set;
        }

        /// <summary>
        /// Default settings for the Time node.
        /// </summary>
        public static EliotNodeSettings Time
        {
            get;
            private set;
        }

        /// <summary>
        /// Default settings for the Utility node.
        /// </summary>
        public static EliotNodeSettings Utility
        {
            get;
            private set;
        }

        public static EliotNodeSettings Loop
        {
            get;
            private set;
        }

        /// <summary>
        /// Construct the Eliot Node Types and Settings for usage with Eliot AI Framework
        /// </summary>
        static EliotNodes()
        {
            Entry = new EliotNodeSettings("Entry", EliotProjectSettings.GUIResources.entryRectNorm, EliotProjectSettings.GUIResources.entryRectSelect, EliotProjectSettings.GUIResources.entryRectGlow);
            Action = new EliotNodeSettings("Action", EliotProjectSettings.GUIResources.rectNorm, EliotProjectSettings.GUIResources.rectSelect, EliotProjectSettings.GUIResources.rectGlow);
            Condition = new EliotNodeSettings("Condition", EliotProjectSettings.GUIResources.rombNorm, EliotProjectSettings.GUIResources.rombSelect, EliotProjectSettings.GUIResources.rombGlow);
            Skill = new EliotNodeSettings("Skill", EliotProjectSettings.GUIResources.rectNorm, EliotProjectSettings.GUIResources.rectSelect, EliotProjectSettings.GUIResources.rectGlow);
            Time = new EliotNodeSettings("Time", EliotProjectSettings.GUIResources.rombNorm, EliotProjectSettings.GUIResources.rombSelect, EliotProjectSettings.GUIResources.rombGlow);
            Utility = new EliotNodeSettings("Utility", EliotProjectSettings.GUIResources.boxNorm, EliotProjectSettings.GUIResources.boxSelect, EliotProjectSettings.GUIResources.boxGlow, utilitySquareWidth, utilitySquareWidth);
            Loop = new EliotNodeSettings("Loop", EliotProjectSettings.GUIResources.rombNorm, EliotProjectSettings.GUIResources.rombSelect, EliotProjectSettings.GUIResources.rombGlow);

            NodesSettings = new List<EliotNodeSettings>();
            NodesSettings.Add(Entry);
            NodesSettings.Add(Action);
            NodesSettings.Add(Condition);
            NodesSettings.Add(Skill);
            NodesSettings.Add(Time);
            NodesSettings.Add(Utility);
            NodesSettings.Add(Loop);
        }

        /// <summary>
        /// Returns a nodes settings based on the NodeName
        /// </summary>
        /// <param name="value">Name of the input node</param>
        /// <returns></returns>
        public static EliotNodeSettings GetNode(string value)
        {
            foreach (EliotNodeSettings node in NodesSettings)
            {
                if (node.Name == value)
                    return node;
            }

            return null;
        }
    }
    
    /// <summary>
    /// Actual Node Settings Class
    /// </summary>
    public class EliotNodeSettings
    {
        /// <summary>
        /// Name of a node.
        /// </summary>
        public string Name;

        // <summary>
        /// Color of the nodes' outline when they are active and _status is 'normal'. 
        /// </summary>
        public Color NodeStatusNormal
        {
            get
            {
#if UNITY_2019
                if(NodeStatusNormal==null)
                {
                    NodeStatusNormal = EliotGUISkin.NodeStatusNormal;
                }
#endif
                return NodeStatusNormal;
            }
            set
            {
                NodeStatusNormal = value;
            }
        }

        /// <summary>
        /// Color of the nodes' outline when they are active and _status is 'warning'. 
        /// </summary>
        public Color NodeStatusWarning
        {
            get
            {
#if UNITY_2019
                if (NodeStatusWarning == null)
                {
                    NodeStatusWarning = EliotGUISkin.NodeStatusWarning;
                }
#endif
                return NodeStatusWarning;
            }
            set
            {
                NodeStatusWarning = value;
            }
        }

        /// <summary>
        /// Color of the nodes' outline when they are active and _status is 'error'. 
        /// </summary>
        public Color NodeStatusError
        {
            get
            {
#if UNITY_2019
                if (NodeStatusError == null)
                {
                    NodeStatusError = EliotGUISkin.NodeStatusError;
                }
#endif
                return NodeStatusError;
            }
            set
            {
                NodeStatusError = value;
            }
        }

        /// <summary>
        /// Texture displayed by default.
        /// </summary>
        public Texture2D NormalTexture;

        /// <summary>
        /// Texture displayed when the node is selected.
        /// </summary>
        public Texture2D SelectedTexture;

        /// <summary>
        /// Texture displayed when the node is highlighted.
        /// </summary>
        public Texture2D ShadowTexture;

        /// <summary>
        /// Size of the texture render
        /// </summary>
        public float textureWidth;
        public float textureHeight;

        /// <summary>
        /// Create new node settings object.
        /// </summary>
        /// <param name="name">Node Name</param>
        /// <param name="normalTexture"></param>
        /// <param name="selectedTexture"></param>
        /// <param name="shadowTexture"></param>
        public EliotNodeSettings(string name, Texture2D normalTexture, Texture2D selectedTexture, Texture2D shadowTexture, float width = 216, float height = 56)
        {
            Name = name;
            NormalTexture = normalTexture;
            SelectedTexture = selectedTexture;
            ShadowTexture = shadowTexture;
            textureWidth = width;
            textureHeight = height;
        }
    }
}
#endif