#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Eliot.Project
{
    /// <summary>
    /// Window that contains useful links to get started.
    /// </summary>
    [InitializeOnLoad] public class EliotWelcomeWindow : EditorWindow
    {
        /// <summary>
        /// Initial size of the window.
        /// </summary>
        public static Vector2 WindowSize = new Vector2(550, 700);

        /// <summary>
        /// Size of the Eliot AI logo.
        /// </summary>
        public static float LogoSize = 300f;
        
        /// <summary>
        /// Size of the SM icons. 
        /// </summary>
        public static float IconsSize = 50f;

        /// <summary>
        /// Static Editor Variables Controlling the Welcome Screen
        /// </summary>
        private const string _displayWelcomeScreenID = "ELIOT_WS";
        
        private const string _displayWelcomeScreenID_SingleSession = "ELIOT_WS_SINGLE_SESSION";
        

        /// <summary>
        /// Stores the information about whether to display the welcome screen on loading unity.
        /// </summary>
        public static bool DisplayWelcomeScreen
        {
            get
            {
                if (!EditorPrefs.HasKey(_displayWelcomeScreenID))
                {
                    EditorPrefs.SetBool(_displayWelcomeScreenID, true);
                }
                return EditorPrefs.GetBool(_displayWelcomeScreenID, true);
            }
            set
            {
                EditorPrefs.SetBool(_displayWelcomeScreenID, value);
            }
        }
        
        public static bool DisplayWelcomeScreenSingleSession
        {
            get
            {
                if (!EditorPrefs.HasKey(_displayWelcomeScreenID_SingleSession))
                {
                    EditorPrefs.SetBool(_displayWelcomeScreenID_SingleSession, true);
                }
                return EditorPrefs.GetBool(_displayWelcomeScreenID_SingleSession, true);
            }
            set
            {
                EditorPrefs.SetBool(_displayWelcomeScreenID_SingleSession, value);
            }
        }

        /// <summary>
        /// Executes on Unity loading.
        /// </summary>
        static EliotWelcomeWindow()
        {
            EditorApplication.delayCall += () =>
            {
                if (DisplayWelcomeScreenSingleSession && !Application.isPlaying)
                {
                    InitWindow();
                    DisplayWelcomeScreenSingleSession = false;
                }
            };
#if UNITY_2019
            EditorApplication.quitting += () => { DisplayWelcomeScreenSingleSession = DisplayWelcomeScreen; };
#endif
        }

        /// <summary>
        /// Initialize the window.
        /// </summary>
        [MenuItem("Tools/Eliot AI/Show Welcome Screen", priority = 50)]
        private static void InitWindow() 
        {
            var screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            GetWindowWithRect<EliotWelcomeWindow>(new Rect(screenCenter, WindowSize), true, "Welcome to Eliot AI Pro", true);
        }

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        private void OnEnable()
        {
            var screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            try{position = new Rect(screenCenter, WindowSize);}catch(Exception){/**/}
        }

        /// <summary>
        /// Draw the window GUI.
        /// </summary>
        private void OnGUI()
        {
            var backgroundRect = new Rect(0, 0, position.width, position.height);
            var backgroundTexture = EliotProjectSettings.GUIResources.backgroundStylesTexture;
            GUI.DrawTexture(
                backgroundRect, backgroundTexture
                //,new Rect(0, 0, position.width, position.height)
                );
            
            //var logoRect = new Rect(position.width*0.5f - LogoSize*0.5f, 0, LogoSize, LogoSize);
            //var logoRect = new Rect(0, 0, 550, 290);
            //GUI.DrawTexture(logoRect, EliotProjectSettings.GUIResources.electrifiedBackgroundTexture);
            
            var welcomeTextRect = new Rect(position.width*0.5f - 200, 20f, 400, 50);
            GUI.Label(welcomeTextRect, "Welcome to Eliot AI Pro!", EliotGUISkin.LabelBoldCenteredBig);
            
            var thanksTextRect = new Rect(position.width*0.5f - 200, 50f, 400, 50);
            GUI.Label(thanksTextRect, "Thank you for choosing Eliot AI!", EliotGUISkin.LabelCenteredBig);
            
            var getStartedTextRect = new Rect(position.width*0.5f - 200, 125f, 400, 50);
            GUI.Label(getStartedTextRect, "Get Started", EliotGUISkin.LabelBoldCentered);
            
            var quickStartRect = new Rect(position.width*0.05f, 150f, position.width * 0.9f, 25);
            if (GUI.Button(quickStartRect, new GUIContent("Quick Start Guide")))
            {
                Help.BrowseURL("https://docs.google.com/document/d/1_5svPcnkMDTVhkybuDIbixjy3pUUEYBdthTVyiIDmzc/edit?usp=sharing");
            }
            
            var eliotAiWebsiteIconRect = new Rect(position.width*0.1f, 200f, IconsSize, IconsSize);
            if (GUI.Button(eliotAiWebsiteIconRect, EliotProjectSettings.GUIResources.eliotWebIconTexture,
                GUIStyle.none))
            {
                Help.BrowseURL("http://www.eliot-ai.com/");
            }
            var eliotAiWebsiteTextRect = new Rect(position.width*0.1f - 75, 250f, 200, 50);
            GUI.Label(eliotAiWebsiteTextRect, "Eliot AI Website", EliotGUISkin.LabelBoldCentered);
            
            var youtubeIconRect = new Rect(position.width*0.5f - 25, 200f, IconsSize, IconsSize);
            if (GUI.Button(youtubeIconRect, EliotProjectSettings.GUIResources.youtubeIconTexture, GUIStyle.none))
            {
                Help.BrowseURL("http://www.youtube.com/channel/UCOZeavQpk9NZuL6NzECMmGw/");
            }
            var youtubeTextRect = new Rect(position.width*0.5f - 100,  250f, 200, 50);
            GUI.Label(youtubeTextRect, "Tutorials", EliotGUISkin.LabelBoldCentered);
            
            var facebookIconRect = new Rect(position.width*0.9f - 25,  200f, IconsSize, IconsSize);
            if (GUI.Button(facebookIconRect, EliotProjectSettings.GUIResources.facebookIconTexture, GUIStyle.none))
            {
                Help.BrowseURL("https://www.facebook.com/eliotaiproject");
            }
            var facebookTextRect = new Rect(position.width*0.9f - 100,  250f, 200, 50);
            GUI.Label(facebookTextRect, "Facebook Page", EliotGUISkin.LabelBoldCentered);
            
            
            var infoBoxRect = new Rect(position.width*0.05f, 300f, position.width * 0.9f, 260);
            var style = EditorStyles.helpBox;
            style.richText = true;
            style.fontSize = 12;
            var content = "Version 1.0.4\n\n" +
                          "- You can now instantiate the Basic Agent Prefab from the context menu\n" +
                          "- Invoker is now renamed to Action to better represent its function\n" +
                          "- Observer is now renamed to Condition\n" +
                          "- You can now access Behaviour elements at runtime by their ID and change their parameters with code\n" +
                          "- Minor bugfixes and improvements" +
                          "\n\n\n\nFocus on what's important and continue inspiring people! We are excited to see your game succeed! :D";
            GUI.Box(infoBoxRect, content, style);
            
            
            var rateUsRect = new Rect(position.width*0.05f, position.height*0.85f, position.width * 0.9f, 30);
            var c = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0f, 0.5f, 0.9f);
            if (GUI.Button(rateUsRect, new GUIContent("Please consider leaving a review to support Eliot AI. Thanks!")))
            {
                Help.BrowseURL("https://assetstore.unity.com/packages/slug/158558");
            }
            GUI.backgroundColor = c;
            
            var showAtStartupButtonRect = new Rect(position.width*0.675f, position.height*0.92f, 150, 30);
            DisplayWelcomeScreen = GUI.Toggle(showAtStartupButtonRect, DisplayWelcomeScreen, new GUIContent("Show at startup (" + (DisplayWelcomeScreen?"on)":"off)") ),
                EditorStyles.toolbarButton);
        }
    }
}
#endif