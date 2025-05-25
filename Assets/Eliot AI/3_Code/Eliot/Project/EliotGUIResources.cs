using UnityEngine;

namespace Eliot
{
    /// <summary>
    /// Eliot AI GUI Resources used by the Editor
    /// 
    /// benjamin.erwin@subliminumindustries.com 26.11.19 (c) Subliminum Industries
    /// 
    /// Eliot Copyright boilerplate should be added
    /// </summary>
    [CreateAssetMenu(fileName = "Eliot GUI Resources", menuName = "Eliot AI/GUIResources")]
    [System.Serializable]
    public class EliotGUIResources : ScriptableObject
    {
        /// <summary>
        /// Icon Graphics
        /// </summary>
        [Header("Icons")]
        public Texture2D logoTexture; //Eliot Logo 256x
        public Texture2D facebookIconTexture;
        public Texture2D youtubeIconTexture;
        public Texture2D eliotWebIconTexture;

        /// <summary>
        /// General GUI Style Graphics
        /// </summary>
        [Header("Styles Textures")]
        public Texture2D backgroundStylesTexture;
        public Texture2D electrifiedBackgroundTexture;
        
        /// <summary>
        /// Icon of the Behaviour Editor Window.
        /// </summary>
        [Header("Behaviour Editor Textures")]
        public Texture2D EditorIcon;
        
        /// <summary>
		/// Background texture of the Behaviour Editor Window.
		/// </summary>
		public Texture2D BE_BackgroundTexture;

        /// <summary>
        /// Textures used for rendering the nodes
        /// </summary>
        public Texture2D boxNorm;
        public Texture2D boxSelect;
        public Texture2D boxGlow;

        public Texture2D entryRectNorm;
        public Texture2D entryRectSelect;
        public Texture2D entryRectGlow;

        public Texture2D rectBreak;
        public Texture2D rectBreakGlow;

        public Texture2D rectNorm;
        public Texture2D rectSelect;
        public Texture2D rectGlow;

        public Texture2D rombNorm;
        public Texture2D rombSelect;
        public Texture2D rombGlow;

        public Texture2D utilityRect;
    }
}
