#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace Eliot
{
    /// <summary>
    /// This singleton contains all the Eliot AI project settings. It does not include information about agents or how they will behave or what data they use. That is the AgentSettings.
    /// 
    /// benjamin.erwin@subliminumindustries.com 26.11.19 (c) Subliminum Industries
    /// 
    /// Eliot Copyright boilerplate should be added
    /// </summary>
    public static class EliotProjectSettings
    {
        //Resources Scriptable containing all the required GUI Assets
        private static EliotGUIResources _GUIResources;

        /// <summary>
        /// Eliot Key Bindings
        /// </summary>
        private static readonly List<EliotKeyBinding> _keyBindings = new List<EliotKeyBinding>()
        {
            EliotKeyBinding.CreateInvoker,
            EliotKeyBinding.CreateObserver,
            EliotKeyBinding.CreateSkill,
            EliotKeyBinding.CreateTime,
            EliotKeyBinding.CreateUtility,
            EliotKeyBinding.StartDefaultTransition,
            EliotKeyBinding.StartAlternativeTransition,
            EliotKeyBinding.Frame
        };

        public static List<EliotKeyBinding> KeyBindings
        {
            get
            {
                return _keyBindings;
            }
        }

        /// <summary>
        /// Encapsulates some of the editor settings. Can be modified by User. User can create multiple settings files.
        /// </summary>
        public static EliotGUIResources GUIResources
        {
            get
            {
                if (_GUIResources == null)
                {
                    _GUIResources = (EliotGUIResources)AssetDatabase.LoadAssetAtPath(Eliot.Repository.PathManager.EliotGUIResources() + "DefaultEliotGUIResources.asset", typeof(EliotGUIResources));
                }

                return _GUIResources;
            }
        }

        public static string BaseNPCAgentPrefabName
        {
            get { return "EliotAIBaseAgentPrefab"; }
        }
        
        public static string BaseNPCAgentSkeletonPrefabName
        {
            get { return "EliotAIBaseAgentSkeletonPrefab"; }
        }
        
        public static string BaseTPSPlayerPrefab
        {
            get { return "EliotAIBaseTPSPlayerPrefab"; }
        }
        
        public static string BaseTPSPlayerSoldierPrefab
        {
            get { return "EliotAIBaseTPSPlayerSoldierPrefab"; }
        }
    }
}
#endif