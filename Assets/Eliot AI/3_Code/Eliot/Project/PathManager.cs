#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;

namespace Eliot.Repository
{
    /// <summary>
    /// Helps with finding files, folders etc. in the filesystem.
    /// </summary>
    public static class PathManager
    {
        /// Cache for Eliot GUI Resources path
        private static string _GUIResources;
        /// Cache for Eliot images path
        private static string _textures;        
        
        /// <summary>
        /// Find Eliot root folder in the project
        /// </summary>
        /// <returns></returns>
        public static string EliotRoot()
        {
            return "Assets/Eliot AI/3_Code/Eliot";
        }
        
        /// <summary>
        /// Directory that contains GUI resources.
        /// </summary>
        /// <returns></returns>
        public static string EliotGUIResources()
        {
            if(_GUIResources == null)
            {
                _GUIResources = EliotRoot() + "/Project/GUI/";
            }
            return _GUIResources;
        }

        /// <summary>
        /// Find Eliot Images folder in the project
        /// </summary>
        /// <returns></returns>
        public static string EliotTextures()
        {
            if (_textures != null)
                return _textures;
            _textures = EliotGUIResources() + "Textures/";
            return _textures;
        }
    }
}
#endif