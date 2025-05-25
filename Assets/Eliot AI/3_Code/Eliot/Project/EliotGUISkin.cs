#if UNITY_EDITOR
#pragma warning disable CS0414, CS0649, CS0612, CS1692
using UnityEngine;

namespace Eliot
{
    /// <summary>
    /// This singleton contains all the Eliot AI GUI Styles and settings. It should ideally server as the default witht he ability for users to adjust graphics with a scriptable object.
    /// 
    /// benjamin.erwin@subliminumindustries.com 26.11.19 (c) Subliminum Industries
    /// 
    /// Eliot Copyright boilerplate should be added
    /// </summary>
    public static class EliotGUISkin
    {
        /// <summary>
        /// Variables related to controlling the auto-theme application (Restart to change after setting your EliotGUIResources scriptable
        /// </summary>
        
        
        /// <summary>
        /// The font color
        /// </summary>
        private static readonly Color fontColor = Color.white * 0.85f;
        private static readonly Color fontColor_Light = Color.black * 0.85f;
        /// <summary>
        /// The font size of big texts
        /// </summary>
        private const int fontBigSize = 16;
        /// <summary>
        /// The font size of small texts
        /// </summary>
        private const int fontSmallSize = 10;
        /// <summary>
        /// The default margins size
        /// </summary>
        private const int defaultPadding = 8;
        /// <summary>
        /// The default margin rectoffset
        /// </summary>
        private static readonly RectOffset defaultPaddingRectOffset = new RectOffset(defaultPadding, defaultPadding, defaultPadding, defaultPadding);
        /// <summary>
        /// The default margin rectoffset
        /// </summary>
        private static readonly RectOffset defaultNodePadding = new RectOffset(0, 0, 0, 0);
        /// <summary>
        /// Utility Node Padding
        /// </summary>
        public const int defaultUtilityNodePadding = 45;
        /// <summary>
        /// Maximum number of string characters allowed in a node label
        /// </summary>
        private const int nodeMaxChar = 26;

        private const int _defaultInspectorLinePadding = 2;
        
        public static int defaultInspectorLinePadding
        {
            get { return _defaultInspectorLinePadding; }
        }


        public static GUIStyle Background
        {
            get;
            private set;
        }

        public static GUIStyle BackgroundNoBorder
        {
            get;
            private set;
        }

        public static GUIStyle Label
        {
            get;
            private set;
        }
        public static GUIStyle LabelBold
        {
            get;
            private set;
        }
        public static GUIStyle LabelCentered
        {
            get;
            private set;
        }
        public static GUIStyle LabelBoldCentered
        {
            get;
            private set;
        }
        public static GUIStyle LabelBig
        {
            get;
            private set;
        }
        public static GUIStyle LabelBoldBig
        {
            get;
            private set;
        }
        public static GUIStyle LabelCenteredBig
        {
            get;
            private set;
        }
        public static GUIStyle LabelBoldCenteredBig
        {
            get;
            private set;
        }
        public static GUIStyle LabelSmall
        {
            get;
            private set;
        }
        public static GUIStyle LabelBoldSmall
        {
            get;
            private set;
        }
        public static GUIStyle LabelCenteredSmall
        {
            get;
            private set;
        }
        public static GUIStyle LabelBoldCenteredSmall
        {
            get;
            private set;
        }
        public static GUIStyle LabelBackground
        {
            get;
            private set;
        }
        public static GUIStyle LabelBoldBackground
        {
            get;
            private set;
        }
        public static GUIStyle LabelCenteredBackground
        {
            get;
            private set;
        }
        public static GUIStyle LabelBoldCenteredBackground
        {
            get;
            private set;
        }
        public static GUIStyle LabelBigBackground
        {
            get;
            private set;
        }
        public static GUIStyle LabelBoldBigBackground
        {
            get;
            private set;
        }
        public static GUIStyle LabelCenteredBigBackground
        {
            get;
            private set;
        }
        public static GUIStyle LabelBoldCenteredBigBackground
        {
            get;
            private set;
        }
        public static GUIStyle LabelSmallBackground
        {
            get;
            private set;
        }
        public static GUIStyle LabelBoldSmallBackground
        {
            get;
            private set;
        }
        public static GUIStyle LabelCenteredSmallBackground
        {
            get;
            private set;
        }
        public static GUIStyle LabelBoldCenteredSmallBackground
        {
            get;
            private set;
        }

        public static GUIStyle Checker
        {
            get;
            private set;
        }

        public static GUIStyle Button
        {
            get;
            private set;
        }
        public static GUIStyle ButtonBig
        {
            get;
            private set;
        }
        public static GUIStyle ButtonBold
        {
            get;
            private set;
        }
        public static GUIStyle ButtonBigBold
        {
            get;
            private set;
        }
        public static GUIStyle ButtonImageOnly
        {
            get;
            private set;
        }
        public static GUIStyle ButtonPressed
        {
            get;
            private set;
        }
        public static GUIStyle ButtonPressedBig
        {
            get;
            private set;
        }
        public static GUIStyle ButtonPressedBold
        {
            get;
            private set;
        }
        public static GUIStyle ButtonPressedBigBold
        {
            get;
            private set;
        }
        public static GUIStyle ButtonPressedImageOnly
        {
            get;
            private set;
        }
        public static GUIStyle ButtonNoHover
        {
            get;
            private set;
        }
        public static GUIStyle ButtonNoBorder
        {
            get;
            private set;
        }
        public static GUIStyle ButtonBigNoBorder
        {
            get;
            private set;
        }
        public static GUIStyle ButtonBoldNoBorder
        {
            get;
            private set;
        }
        public static GUIStyle ButtonBigBoldNoBorder
        {
            get;
            private set;
        }
        public static GUIStyle ButtonImageOnlyNoBorder
        {
            get;
            private set;
        }
        public static GUIStyle ButtonPressedNoBorder
        {
            get;
            private set;
        }
        public static GUIStyle ButtonPressedBigNoBorder
        {
            get;
            private set;
        }
        public static GUIStyle ButtonPressedBoldNoBorder
        {
            get;
            private set;
        }
        public static GUIStyle ButtonPressedBigBoldNoBorder
        {
            get;
            private set;
        }
        public static GUIStyle ButtonPressedImageOnlyNoBorder
        {
            get;
            private set;
        }
        public static GUIStyle ButtonNoHoverNoBorder
        {
            get;
            private set;
        }


        public static GUIStyle EmptyMiddleAligned
        {
            get;
            private set;
        }
        public static GUIStyle EmptyMiddleAlignedTop
        {
            get;
            private set;
        }
        
        //Node Styles
        public static GUIStyle NodeLabel
        {
            get;
            private set;
        }

        public static GUIStyle EntryNode_Normal
        {
            get;
            private set;
        }
        public static GUIStyle EntryNode_Select
        {
            get;
            private set;
        }
        public static GUIStyle EntryNode_Fade
        {
            get;
            private set;
        }
        public static GUIStyle InvokerNode_Normal
        {
            get;
            private set;
        }
        public static GUIStyle InvokerNode_Select
        {
            get;
            private set;
        }
        public static GUIStyle InvokerNode_Fade
        {
            get;
            private set;
        }
        public static GUIStyle InvokerNode_Break
        {
            get;
            private set;
        }
        public static GUIStyle ObserverNode_Normal
        {
            get;
            private set;
        }
        public static GUIStyle ObserverNode_Select
        {
            get;
            private set;
        }
        public static GUIStyle ObserverNode_Fade
        {
            get;
            private set;
        }
        public static GUIStyle SkillNode_Normal
        {
            get;
            private set;
        }
        public static GUIStyle SkillNode_Select
        {
            get;
            private set;
        }
        public static GUIStyle SkillNode_Fade
        {
            get;
            private set;
        }
        public static GUIStyle TimeNode_Normal
        {
            get;
            private set;
        }
        public static GUIStyle TimeNode_Select
        {
            get;
            private set;
        }
        public static GUIStyle TimeNode_Fade
        {
            get;
            private set;
        }
        public static GUIStyle UtilityNode_Normal
        {
            get;
            private set;
        }
        public static GUIStyle UtilityNode_Select
        {
            get;
            private set;
        }
        public static GUIStyle UtilityNode_Fade
        {
            get;
            private set;
        }
        public static GUIStyle LoopNode_Normal
        {
            get;
            private set;
        }
        public static GUIStyle LoopNode_Select
        {
            get;
            private set;
        }
        public static GUIStyle LoopNode_Fade
        {
            get;
            private set;
        }

        //GUIStyles
        /// <summary>
        /// Orange color to indicate scriptable objects.
        /// </summary>
        public static GUIStyle ScriptableOrange
        {
            get
            {
                return FlatBackgroundBox(new Color(1f, 0.3f, 0.0f, 0.3f));
            }
        }

        /// <summary>
        /// Purple color to indicate Unit component.
        /// </summary>
        public static GUIStyle UnitPurple
        {
            get
            {
                return FlatBackgroundBox(new Color(0.5f, 0.2f, 0.9f, 0.3f));
            }
        }

        /// <summary>
        /// Green color to indicate an agent component.
        /// </summary>
        public static GUIStyle AgentComponentGreen
        {
            get
            {
                return FlatBackgroundBox(new Color(0.3f, 0.8f, 0.9f, 0.3f));
            }
        }

        /// <summary>
        /// Default background color.
        /// </summary>
        public static GUIStyle GrayBackground
        {
            get
            {
                return FlatBackgroundBox(new Color(0.5f, 0.5f, 0.5f, 0.3f));
            }
        }

        /// <summary>
        /// Default blue color for Eliot components headers.
        /// </summary>
        public static GUIStyle BlueBackground
        {
            get
            {
                return FlatBackgroundBox(new Color(0.3f, 0.5f, 1f, 0.3f));
            }
        }

        /// <summary>
        /// Create new background style of a given color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static GUIStyle FlatBackgroundBox(Color color)
        {
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box)
            {
                border = new RectOffset(-1, -1, -1, -1)
            };
            
            var texture = new Texture2D(1, 1);
            texture.SetPixels(new Color[1] {color});
            texture.Apply();
            
            boxStyle.normal.background = texture;
            return boxStyle;
        }

        //Nodes
        public static Color NodeStatusNormal
        {
            get;
            private set;
        }

        /// <summary>
        /// Color of the nodes' outline when they are active and _status is 'warning'. 
        /// </summary>
        public static Color NodeStatusWarning
        {
            get;
            private set;
        }
        /// <summary>
        /// Color of the nodes' outline when they are active and _status is 'error'. 
        /// </summary>
        public static Color NodeStatusError
        {
            get;
            private set;
        }
        //Transitions

        public static Color NeutralColor
        {
            get;
            private set;
        }

        /// Color of 'Yes' transitions.
        public static Color PositiveColor
        {
            get;
            private set;
        }

        /// Color of 'No' transitions.
        public static Color NegativeColor
        {
            get;
            private set;
        }

        /// Color of transitions that are selected.
        public static Color SelectedColor
        {
            get;
            private set;
        }

        /// Color of 'While' transitions.
        public static Color LoopColor
        {
            get;
            private set;
        }

        /// Color of selection box.
        public static Color SelectionBoxColor
        {
            get;
            private set;
        }

        /// Color of selection box's outline.
        public static Color SelectionBoxOutlineColor
        {
            get;
            private set;
        }

        public static string LastCopiedAgentComponent { get; private set; }


        /// <summary>
        /// Constructor
        /// </summary>
        static EliotGUISkin()
        {
            //General GUI Styles for Editor Use
            Background = new GUIStyle();
            Background.alignment = TextAnchor.MiddleCenter;
            Background.border = new RectOffset(2, 2, 2, 2);
            Background.imagePosition = ImagePosition.ImageOnly;
            Background.name = "Background";
            Background.normal.background = EliotProjectSettings.GUIResources.backgroundStylesTexture;
            Background.normal.textColor = fontColor;
            Background.padding = defaultPaddingRectOffset;

            BackgroundNoBorder = new GUIStyle(Background);
            BackgroundNoBorder.padding = new RectOffset(0, 0, 0, 0);
            BackgroundNoBorder.name = "NoBorder";

            Label = new GUIStyle();
            Label.alignment = TextAnchor.MiddleLeft;
            Label.name = "Label";
            Label.normal.textColor = fontColor;
            Label.padding = new RectOffset(3, 3, 3, 3);
            Label.richText = true;
            Label.wordWrap = true;

            LabelBold = new GUIStyle(Label);
            LabelBold.fontStyle = FontStyle.Bold;
            LabelBold.name = Label.name + "Bold";

            LabelCentered = new GUIStyle(Label);
            LabelCentered.alignment = TextAnchor.UpperCenter;
            LabelCentered.name = Label.name + "Centered";

            LabelBoldCentered = new GUIStyle(LabelBold);
            LabelBoldCentered.alignment = LabelCentered.alignment;
            LabelBoldCentered.name = Label.name + "BoldCentered";

            LabelBig = new GUIStyle(Label);
            LabelBig.fontSize = fontBigSize;
            LabelBig.name = Label.name + "Big";

            LabelBoldBig = new GUIStyle(LabelBold);
            LabelBoldBig.fontSize = LabelBig.fontSize;
            LabelBoldBig.name = LabelBold.name + "Big";

            LabelCenteredBig = new GUIStyle(LabelCentered);
            LabelCenteredBig.fontSize = LabelBig.fontSize;
            LabelCenteredBig.name = LabelCentered.name + "Big";

            LabelBoldCenteredBig = new GUIStyle(LabelBoldCentered);
            LabelBoldCenteredBig.fontSize = LabelBig.fontSize;
            LabelBoldCenteredBig.name = LabelBoldCentered.name + "Big";

            LabelSmall = new GUIStyle(Label);
            LabelSmall.fontSize = fontSmallSize;
            LabelSmall.name = Label.name + "Small";

            LabelBoldSmall = new GUIStyle(LabelBold);
            LabelBoldSmall.fontSize = LabelSmall.fontSize;
            LabelBoldSmall.name = LabelBold.name + "Small";

            LabelCenteredSmall = new GUIStyle(LabelCentered);
            LabelCenteredSmall.fontSize = LabelSmall.fontSize;
            LabelCenteredSmall.name = LabelCentered.name + "Small";

            LabelBoldCenteredSmall = new GUIStyle(LabelBoldCentered);
            LabelBoldCenteredSmall.fontSize = LabelSmall.fontSize;
            LabelBoldCenteredSmall.name = LabelBoldCentered.name + "Small";

            LabelBackground = new GUIStyle(Label);
            LabelBackground.border = new RectOffset(1, 1, 1, 1);
            LabelBackground.padding = defaultPaddingRectOffset;
            LabelBackground.name = Label.name + "Background";

            LabelBoldBackground = new GUIStyle(LabelBold);
            LabelBoldBackground.border = LabelBackground.border;
            LabelBoldBackground.normal.background = LabelBackground.normal.background;
            LabelBoldBackground.padding = defaultPaddingRectOffset;
            LabelBoldBackground.name = LabelBold.name + "Background";

            LabelCenteredBackground = new GUIStyle(LabelCentered);
            LabelCenteredBackground.border = LabelBackground.border;
            LabelCenteredBackground.normal.background = LabelBackground.normal.background;
            LabelCenteredBackground.padding = defaultPaddingRectOffset;
            LabelCenteredBackground.name = LabelCentered.name + "Background";

            LabelBoldCenteredBackground = new GUIStyle(LabelBoldCentered);
            LabelBoldCenteredBackground.border = LabelBackground.border;
            LabelBoldCenteredBackground.normal.background = LabelBackground.normal.background;
            LabelBoldCenteredBackground.padding = defaultPaddingRectOffset;
            LabelBoldCenteredBackground.name = LabelBoldCentered.name + "Background";

            LabelBigBackground = new GUIStyle(LabelBig);
            LabelBigBackground.border = LabelBackground.border;
            LabelBigBackground.normal.background = LabelBackground.normal.background;
            LabelBigBackground.padding = defaultPaddingRectOffset;
            LabelBigBackground.name = LabelBig.name + "Background";

            LabelBoldBigBackground = new GUIStyle(LabelBoldBig);
            LabelBoldBigBackground.border = LabelBackground.border;
            LabelBoldBigBackground.normal.background = LabelBackground.normal.background;
            LabelBoldBigBackground.padding = defaultPaddingRectOffset;
            LabelBoldBigBackground.name = LabelBoldBig.name + "Background";

            LabelCenteredBigBackground = new GUIStyle(LabelCenteredBig);
            LabelCenteredBigBackground.border = LabelBackground.border;
            LabelCenteredBigBackground.normal.background = LabelBackground.normal.background;
            LabelCenteredBigBackground.padding = defaultPaddingRectOffset;
            LabelCenteredBigBackground.name = LabelCenteredBig.name + "Background";

            LabelBoldCenteredBigBackground = new GUIStyle(LabelBoldCenteredBig);
            LabelBoldCenteredBigBackground.border = LabelBackground.border;
            LabelBoldCenteredBigBackground.normal.background = LabelBackground.normal.background;
            LabelBoldCenteredBigBackground.padding = defaultPaddingRectOffset;
            LabelBoldCenteredBigBackground.name = LabelBoldCenteredBig.name + "Background";

            LabelSmallBackground = new GUIStyle(LabelSmall);
            LabelSmallBackground.border = LabelBackground.border;
            LabelSmallBackground.normal.background = LabelBackground.normal.background;
            LabelSmallBackground.padding = defaultPaddingRectOffset;
            LabelSmallBackground.name = LabelSmall.name + "Background";

            LabelBoldSmallBackground = new GUIStyle(LabelBoldSmall);
            LabelBoldSmallBackground.border = LabelBackground.border;
            LabelBoldSmallBackground.normal.background = LabelBackground.normal.background;
            LabelBoldSmallBackground.padding = defaultPaddingRectOffset;
            LabelBoldSmallBackground.name = LabelBoldSmall.name + "Background";

            LabelCenteredSmallBackground = new GUIStyle(LabelCenteredSmall);
            LabelCenteredSmallBackground.border = LabelBackground.border;
            LabelCenteredSmallBackground.normal.background = LabelBackground.normal.background;
            LabelCenteredSmallBackground.padding = defaultPaddingRectOffset;
            LabelCenteredSmallBackground.name = LabelCenteredSmall.name + "Background";

            LabelBoldCenteredSmallBackground = new GUIStyle(LabelBoldCenteredSmall);
            LabelBoldCenteredSmallBackground.border = LabelBackground.border;
            LabelBoldCenteredSmallBackground.normal.background = LabelBackground.normal.background;
            LabelBoldCenteredSmallBackground.padding = defaultPaddingRectOffset;
            LabelBoldCenteredSmallBackground.name = LabelBoldCenteredSmall.name + "Background";

            Checker = new GUIStyle();
            Checker.alignment = TextAnchor.MiddleCenter;
            Checker.border = new RectOffset(1, 1, 1, 1);
            Checker.name = "Checker";
            Checker.onNormal.background = Checker.onNormal.background;
            Checker.onActive.background = Checker.active.background;
            Checker.onFocused.background = Checker.focused.background;
            Checker.onHover.background = Checker.hover.background;
            Checker.padding = new RectOffset(0, 0, 0, 0);

            Button = new GUIStyle();
            Button.alignment = TextAnchor.MiddleCenter;
            Button.border = new RectOffset(1, 1, 1, 1);
            Button.name = "Button";
            Button.normal.textColor = fontColor;
            Button.onNormal.background = Button.onNormal.background;
            Button.onNormal.textColor = Button.normal.textColor;
            Button.active.textColor = Button.normal.textColor;
            Button.onActive.background = Button.active.background;
            Button.onActive.textColor = Button.normal.textColor;
            Button.focused.textColor = Button.normal.textColor;
            Button.onFocused.background = Button.focused.background;
            Button.onFocused.textColor = Button.normal.textColor;
            Button.hover.textColor = Button.normal.textColor;
            Button.onHover.background = Button.hover.background;
            Button.onHover.textColor = Button.normal.textColor;
            Button.padding = defaultPaddingRectOffset;

            ButtonBig = new GUIStyle(Button);
            ButtonBig.fontSize = fontBigSize;
            ButtonBig.name = Button.name + "Big";

            ButtonBold = new GUIStyle(Button);
            ButtonBold.fontStyle = FontStyle.Bold;
            ButtonBold.name = Button.name + "Bold";

            ButtonBigBold = new GUIStyle(Button);
            ButtonBigBold.fontSize = ButtonBig.fontSize;
            ButtonBigBold.fontStyle = ButtonBold.fontStyle;
            ButtonBigBold.name = Button.name + "BigBold";

            ButtonImageOnly = new GUIStyle(Button);
            ButtonImageOnly.imagePosition = ImagePosition.ImageOnly;
            ButtonImageOnly.name = Button.name + "ImageOnly";

            ButtonPressed = new GUIStyle(Button);
            ButtonPressed.onNormal.background = ButtonPressed.normal.background;
            ButtonPressed.name = Button.name + "Pressed";

            ButtonPressedBig = new GUIStyle(ButtonPressed);
            ButtonPressedBig.fontSize = fontBigSize;
            ButtonPressedBig.name = ButtonPressed.name + "Big";

            ButtonPressedBold = new GUIStyle(ButtonPressed);
            ButtonPressedBold.fontStyle = FontStyle.Bold;
            ButtonPressedBold.name = ButtonPressed.name + "Bold";

            ButtonPressedBigBold = new GUIStyle(ButtonPressed);
            ButtonPressedBigBold.fontSize = ButtonBig.fontSize;
            ButtonPressedBigBold.fontStyle = ButtonBold.fontStyle;
            ButtonPressedBigBold.name = ButtonPressed.name + "BigBold";

            ButtonPressedImageOnly = new GUIStyle(ButtonPressed);
            ButtonPressedImageOnly.imagePosition = ImagePosition.ImageOnly;
            ButtonPressedImageOnly.name = Button.name + "PressedImageOnly";

            ButtonNoHover = new GUIStyle();
            ButtonNoHover.alignment = Button.alignment;
            ButtonNoHover.border = Button.border;
            ButtonNoHover.imagePosition = Button.imagePosition;
            ButtonNoHover.name = Button.name + "NoHover";
            ButtonNoHover.active.background = Button.active.background;
            ButtonNoHover.active.textColor = Button.active.textColor;
            ButtonNoHover.onActive.background = Button.active.background;
            ButtonNoHover.onActive.textColor = Button.active.textColor;
            ButtonNoHover.normal.background = Button.normal.background;
            ButtonNoHover.normal.textColor = Button.normal.textColor;
            ButtonNoHover.onNormal.background = Button.normal.background;
            ButtonNoHover.onNormal.textColor = Button.normal.textColor;
            ButtonNoHover.padding = Button.padding;

            ButtonNoBorder = new GUIStyle(Button);
            ButtonNoBorder.padding = new RectOffset(0, 0, 0, 0);
            ButtonNoBorder.name = Button.name + "NoBorder";

            ButtonBigNoBorder = new GUIStyle(ButtonBig);
            ButtonBigNoBorder.padding = new RectOffset(0, 0, 0, 0);
            ButtonBigNoBorder.name = ButtonBig.name + "NoBorder";

            ButtonBoldNoBorder = new GUIStyle(ButtonBold);
            ButtonBoldNoBorder.padding = new RectOffset(0, 0, 0, 0);
            ButtonBoldNoBorder.name = ButtonBold.name + "NoBorder";

            ButtonBigBoldNoBorder = new GUIStyle(ButtonBigBold);
            ButtonBigBoldNoBorder.padding = new RectOffset(0, 0, 0, 0);
            ButtonBigBoldNoBorder.name = ButtonBigBold.name + "NoBorder";

            ButtonImageOnlyNoBorder = new GUIStyle(ButtonImageOnly);
            ButtonImageOnlyNoBorder.padding = new RectOffset(0, 0, 0, 0);
            ButtonImageOnlyNoBorder.name = ButtonImageOnly.name + "NoBorder";

            ButtonPressedNoBorder = new GUIStyle(ButtonPressed);
            ButtonPressedNoBorder.padding = new RectOffset(0, 0, 0, 0);
            ButtonPressedNoBorder.name = ButtonPressed.name + "NoBorder";

            ButtonPressedBigNoBorder = new GUIStyle(ButtonPressedBig);
            ButtonPressedBigNoBorder.padding = new RectOffset(0, 0, 0, 0);
            ButtonPressedBigNoBorder.name = ButtonPressedBig.name + "NoBorder";

            ButtonPressedBoldNoBorder = new GUIStyle(ButtonPressedBold);
            ButtonPressedBoldNoBorder.padding = new RectOffset(0, 0, 0, 0);
            ButtonPressedBoldNoBorder.name = ButtonPressedBold.name + "NoBorder";

            ButtonPressedBigBoldNoBorder = new GUIStyle(ButtonPressedBigBold);
            ButtonPressedBigBoldNoBorder.padding = new RectOffset(0, 0, 0, 0);
            ButtonPressedBigBoldNoBorder.name = ButtonPressedBigBold.name + "NoBorder";

            ButtonPressedImageOnlyNoBorder = new GUIStyle(ButtonPressedImageOnly);
            ButtonPressedImageOnlyNoBorder.padding = new RectOffset(0, 0, 0, 0);
            ButtonPressedImageOnlyNoBorder.name = ButtonPressedImageOnly.name + "NoBorder";

            ButtonNoHoverNoBorder = new GUIStyle(ButtonNoHover);
            ButtonNoHoverNoBorder.padding = new RectOffset(0, 0, 0, 0);
            ButtonNoHoverNoBorder.name = ButtonNoHover.name + "NoBorder";

            EmptyMiddleAligned = new GUIStyle();
            EmptyMiddleAligned.alignment = TextAnchor.MiddleCenter;
            EmptyMiddleAligned.imagePosition = ImagePosition.ImageOnly;
            EmptyMiddleAligned.name = "EmptyMiddleAligned";
            EmptyMiddleAligned.padding = new RectOffset(0, 0, 0, 0);

            EmptyMiddleAlignedTop = new GUIStyle(EmptyMiddleAligned);
            EmptyMiddleAlignedTop.alignment = TextAnchor.UpperCenter;
            EmptyMiddleAlignedTop.name = EmptyMiddleAligned.name + "Top";


                NodeLabel = new GUIStyle()
                {
                    name = "NodeFunctionLabel",
                    //padding = defaultPaddingRectOffset,
                    fontStyle = FontStyle.Bold,
                    fontSize = 14,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true,
                    richText = true,
                    clipping = TextClipping.Overflow,
                    contentOffset = new Vector2(0, 0),
                    stretchWidth = true,
                    stretchHeight = true,
                    normal = { textColor = fontColor },
                    hover = { textColor = Color.white },
                    active = { textColor = Color.white },
                    focused = { textColor = Color.white },

                    onNormal = { textColor = Color.white },
                    onHover = { textColor = Color.white },
                    onFocused = { textColor = Color.white },
                    onActive = { textColor = Color.white }
                };

                //Node Styles
                EntryNode_Normal = new GUIStyle()
                {
                    name = "EntryNode_Normal",
                    border = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                    padding = defaultNodePadding,
                    fontStyle = FontStyle.Normal,
                    fontSize = fontSmallSize,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true,
                    richText = true,
                    clipping = TextClipping.Overflow,
                    imagePosition = ImagePosition.TextOnly,
                    contentOffset = new Vector2(0, -30),
                    fixedWidth = EliotNodes.GetNode("Entry").textureWidth,
                    fixedHeight = EliotNodes.GetNode("Entry").textureHeight,
                    stretchWidth = false,
                    stretchHeight = false,
                    normal = { background = EliotNodes.GetNode("Entry").NormalTexture, textColor = fontColor },
                    hover = { background = EliotNodes.GetNode("Entry").NormalTexture, textColor = Color.white },
                    active = { background = EliotNodes.GetNode("Entry").SelectedTexture, textColor = Color.white },
                    focused = { background = EliotNodes.GetNode("Entry").SelectedTexture, textColor = Color.white },

                    onNormal = { background = EliotNodes.GetNode("Entry").SelectedTexture, textColor = Color.white },
                    onHover = { background = EliotNodes.GetNode("Entry").SelectedTexture, textColor = Color.white },
                    onFocused = { background = EliotNodes.GetNode("Entry").SelectedTexture, textColor = Color.white },
                    onActive = { background = EliotNodes.GetNode("Entry").SelectedTexture, textColor = Color.white }
                };
                EntryNode_Select = new GUIStyle(EntryNode_Normal)
                {
                    name = "EntryNode_Select",
                    normal = { background = EliotNodes.GetNode("Entry").SelectedTexture, textColor = Color.white }
                };
                EntryNode_Fade = new GUIStyle(EntryNode_Normal)
                {
                    name = "EntryNode_Fade",
                    normal = { background = EliotNodes.GetNode("Entry").ShadowTexture, textColor = Color.white },
                    hover = { background = EliotNodes.GetNode("Entry").ShadowTexture, textColor = Color.white },
                    focused = { background = EliotNodes.GetNode("Entry").ShadowTexture, textColor = Color.white },
                    active = { background = EliotNodes.GetNode("Entry").ShadowTexture, textColor = Color.white },

                    onNormal = { background = EliotNodes.GetNode("Entry").ShadowTexture, textColor = Color.white },
                    onHover = { background = EliotNodes.GetNode("Entry").ShadowTexture, textColor = Color.white },
                    onFocused = { background = EliotNodes.GetNode("Entry").ShadowTexture, textColor = Color.white },
                    onActive = { background = EliotNodes.GetNode("Entry").ShadowTexture, textColor = Color.white }
                };


                InvokerNode_Normal = new GUIStyle()
                {
                    name = "InvokerNode_Normal",
                    border = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                    padding = defaultNodePadding,
                    fontStyle = FontStyle.Normal,
                    fontSize = fontSmallSize,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true,
                    richText = true,
                    clipping = TextClipping.Overflow,
                    imagePosition = ImagePosition.TextOnly,
                    contentOffset = new Vector2(0, -30),
                    fixedWidth = EliotNodes.GetNode("Action").textureWidth,
                    fixedHeight = EliotNodes.GetNode("Action").textureHeight,
                    stretchWidth = false,
                    stretchHeight = false,
                    normal = { background = EliotNodes.GetNode("Action").NormalTexture, textColor = fontColor },
                    hover = { background = EliotNodes.GetNode("Action").SelectedTexture, textColor = Color.white },
                    active = { background = EliotNodes.GetNode("Action").SelectedTexture, textColor = Color.white },
                    focused = { background = EliotNodes.GetNode("Action").SelectedTexture, textColor = Color.white },

                    onNormal = { background = EliotNodes.GetNode("Action").SelectedTexture, textColor = Color.white },
                    onHover = { background = EliotNodes.GetNode("Action").SelectedTexture, textColor = Color.white },
                    onFocused = { background = EliotNodes.GetNode("Action").SelectedTexture, textColor = Color.white },
                    onActive = { background = EliotNodes.GetNode("Action").SelectedTexture, textColor = Color.white }
                };
                InvokerNode_Select = new GUIStyle(InvokerNode_Normal)
                {
                    name = "InvokerNode_Select",
                    normal = { background = EliotNodes.GetNode("Action").SelectedTexture, textColor = fontColor }
                };
                InvokerNode_Fade = new GUIStyle(InvokerNode_Normal)
                {
                    name = "InvokerNode_Fade",
                    normal = { background = EliotNodes.GetNode("Action").ShadowTexture, textColor = Color.white },
                    hover = { background = EliotNodes.GetNode("Action").ShadowTexture, textColor = Color.white },
                    active = { background = EliotNodes.GetNode("Action").ShadowTexture, textColor = Color.white },
                    focused = { background = EliotNodes.GetNode("Action").ShadowTexture, textColor = Color.white },

                    onNormal = { background = EliotNodes.GetNode("Action").ShadowTexture, textColor = Color.white },
                    onHover = { background = EliotNodes.GetNode("Action").ShadowTexture, textColor = Color.white },
                    onFocused = { background = EliotNodes.GetNode("Action").ShadowTexture, textColor = Color.white },
                    onActive = { background = EliotNodes.GetNode("Action").ShadowTexture, textColor = Color.white }

                };
                InvokerNode_Break = new GUIStyle(InvokerNode_Normal)
                {
                    name = "InvokerNode_Break",
                    normal = { background = EliotProjectSettings.GUIResources.rectBreak, textColor = fontColor },
                    active = { background = EliotProjectSettings.GUIResources.rectBreakGlow, textColor = Color.white },
                    focused = { background = EliotProjectSettings.GUIResources.rectBreakGlow, textColor = Color.white },

                    onNormal = { background = EliotProjectSettings.GUIResources.rectBreakGlow, textColor = Color.white },
                    onHover = { background = EliotProjectSettings.GUIResources.rectBreakGlow, textColor = Color.white },
                    onFocused = { background = EliotProjectSettings.GUIResources.rectBreakGlow, textColor = Color.white },
                    onActive = { background = EliotProjectSettings.GUIResources.rectBreakGlow, textColor = Color.white }

                };

                ObserverNode_Normal = new GUIStyle()
                {
                    name = "ObserverNode_Normal",
                    border = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                    padding = defaultNodePadding,
                    fontStyle = FontStyle.Normal,
                    fontSize = fontSmallSize,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true,
                    richText = true,
                    clipping = TextClipping.Overflow,
                    imagePosition = ImagePosition.TextOnly,
                    contentOffset = new Vector2(0, -30),
                    fixedWidth = EliotNodes.GetNode("Condition").textureWidth,
                    fixedHeight = EliotNodes.GetNode("Condition").textureHeight,
                    stretchWidth = false,
                    stretchHeight = false,

                    normal = { background = EliotNodes.GetNode("Condition").NormalTexture, textColor = fontColor },
                    hover = { background = EliotNodes.GetNode("Condition").SelectedTexture, textColor = Color.white },
                    active = { background = EliotNodes.GetNode("Condition").SelectedTexture, textColor = Color.white },
                    focused = { background = EliotNodes.GetNode("Condition").SelectedTexture, textColor = Color.white },

                    onNormal = { background = EliotNodes.GetNode("Condition").SelectedTexture, textColor = Color.white },
                    onHover = { background = EliotNodes.GetNode("Condition").SelectedTexture, textColor = Color.white },
                    onFocused = { background = EliotNodes.GetNode("Condition").SelectedTexture, textColor = Color.white },
                    onActive = { background = EliotNodes.GetNode("Condition").SelectedTexture, textColor = Color.white }
                };
                ObserverNode_Select = new GUIStyle(ObserverNode_Normal)
                {
                    name = "ObserverNode_Select",
                    normal = { background = EliotNodes.GetNode("Condition").SelectedTexture, textColor = fontColor }
                };
                ObserverNode_Fade = new GUIStyle(ObserverNode_Normal)
                {
                    name = "ObserverNode_Fade",
                    normal = { background = EliotNodes.GetNode("Condition").ShadowTexture, textColor = Color.white },
                    hover = { background = EliotNodes.GetNode("Condition").ShadowTexture, textColor = Color.white },
                    active = { background = EliotNodes.GetNode("Condition").ShadowTexture, textColor = Color.white },
                    focused = { background = EliotNodes.GetNode("Condition").ShadowTexture, textColor = Color.white },

                    onNormal = { background = EliotNodes.GetNode("Condition").ShadowTexture, textColor = Color.white },
                    onHover = { background = EliotNodes.GetNode("Condition").ShadowTexture, textColor = Color.white },
                    onFocused = { background = EliotNodes.GetNode("Condition").ShadowTexture, textColor = Color.white },
                    onActive = { background = EliotNodes.GetNode("Condition").ShadowTexture, textColor = Color.white }
                };


                SkillNode_Normal = new GUIStyle(EntryNode_Normal)
                {
                    name = "SkillNode_Normal",
                    normal = { background = EliotNodes.GetNode("Skill").NormalTexture, textColor = fontColor },
                    hover = { background = EliotNodes.GetNode("Skill").SelectedTexture, textColor = Color.white },
                    active = { background = EliotNodes.GetNode("Skill").SelectedTexture, textColor = Color.white },
                    focused = { background = EliotNodes.GetNode("Skill").SelectedTexture, textColor = Color.white },

                    onNormal = { background = EliotNodes.GetNode("Skill").SelectedTexture, textColor = Color.white },
                    onHover = { background = EliotNodes.GetNode("Skill").SelectedTexture, textColor = Color.white },
                    onFocused = { background = EliotNodes.GetNode("Skill").SelectedTexture, textColor = Color.white },
                    onActive = { background = EliotNodes.GetNode("Skill").SelectedTexture, textColor = Color.white }
                };
                SkillNode_Select = new GUIStyle(SkillNode_Normal)
                {
                    name = "SkillNode_Select",
                    normal = { background = EliotNodes.GetNode("Skill").SelectedTexture, textColor = fontColor }
                };
                SkillNode_Fade = new GUIStyle(SkillNode_Normal)
                {
                    name = "SkillNode_Fade",
                    normal = { background = EliotNodes.GetNode("Skill").ShadowTexture, textColor = Color.white },
                    hover = { background = EliotNodes.GetNode("Skill").ShadowTexture, textColor = Color.white },
                    active = { background = EliotNodes.GetNode("Skill").ShadowTexture, textColor = Color.white },
                    focused = { background = EliotNodes.GetNode("Skill").ShadowTexture, textColor = Color.white },

                    onNormal = { background = EliotNodes.GetNode("Skill").ShadowTexture, textColor = Color.white },
                    onHover = { background = EliotNodes.GetNode("Skill").ShadowTexture, textColor = Color.white },
                    onFocused = { background = EliotNodes.GetNode("Skill").ShadowTexture, textColor = Color.white },
                    onActive = { background = EliotNodes.GetNode("Skill").ShadowTexture, textColor = Color.white }
                };


                TimeNode_Normal = new GUIStyle(ObserverNode_Normal)
                {
                    name = "TimeNode_Normal",
                    normal = { background = EliotNodes.GetNode("Time").NormalTexture, textColor = fontColor },
                    hover = { background = EliotNodes.GetNode("Time").SelectedTexture, textColor = Color.white },
                    active = { background = EliotNodes.GetNode("Time").SelectedTexture, textColor = Color.white },
                    focused = { background = EliotNodes.GetNode("Time").SelectedTexture, textColor = Color.white },

                    onNormal = { background = EliotNodes.GetNode("Time").SelectedTexture, textColor = Color.white },
                    onHover = { background = EliotNodes.GetNode("Time").SelectedTexture, textColor = Color.white },
                    onFocused = { background = EliotNodes.GetNode("Time").SelectedTexture, textColor = Color.white },
                    onActive = { background = EliotNodes.GetNode("Time").SelectedTexture, textColor = Color.white }
                };
                TimeNode_Select = new GUIStyle(TimeNode_Normal)
                {
                    name = "TimeNode_Select",
                    normal = { background = EliotNodes.GetNode("Time").SelectedTexture, textColor = fontColor }
                };
                TimeNode_Fade = new GUIStyle(TimeNode_Normal)
                {
                    name = "TimeNode_Fade",
                    normal = { background = EliotNodes.GetNode("Time").ShadowTexture, textColor = Color.white },
                    hover = { background = EliotNodes.GetNode("Time").ShadowTexture, textColor = Color.white },
                    active = { background = EliotNodes.GetNode("Time").ShadowTexture, textColor = Color.white },
                    focused = { background = EliotNodes.GetNode("Time").ShadowTexture, textColor = Color.white },

                    onNormal = { background = EliotNodes.GetNode("Time").ShadowTexture, textColor = Color.white },
                    onHover = { background = EliotNodes.GetNode("Time").ShadowTexture, textColor = Color.white },
                    onFocused = { background = EliotNodes.GetNode("Time").ShadowTexture, textColor = Color.white },
                    onActive = { background = EliotNodes.GetNode("Time").ShadowTexture, textColor = Color.white }
                };


                LoopNode_Normal = new GUIStyle(InvokerNode_Normal)
                {
                    name = "LoopNode_Normal",
                    normal = { background = EliotNodes.GetNode("Loop").NormalTexture, textColor = fontColor },
                    hover = { background = EliotNodes.GetNode("Loop").SelectedTexture, textColor = Color.white },
                    active = { background = EliotNodes.GetNode("Loop").SelectedTexture, textColor = Color.white },
                    focused = { background = EliotNodes.GetNode("Loop").SelectedTexture, textColor = Color.white },

                    onNormal = { background = EliotNodes.GetNode("Loop").SelectedTexture, textColor = Color.white },
                    onHover = { background = EliotNodes.GetNode("Loop").SelectedTexture, textColor = Color.white },
                    onFocused = { background = EliotNodes.GetNode("Loop").SelectedTexture, textColor = Color.white },
                    onActive = { background = EliotNodes.GetNode("Loop").SelectedTexture, textColor = Color.white }
                };
                LoopNode_Select = new GUIStyle(LoopNode_Normal)
                {
                    name = "LoopNode_Select",
                    normal = { background = EliotNodes.GetNode("Loop").SelectedTexture, textColor = fontColor }
                };
                LoopNode_Fade = new GUIStyle(LoopNode_Normal)
                {
                    name = "LoopNode_Fade",
                    normal = { background = EliotNodes.GetNode("Loop").ShadowTexture, textColor = Color.white },
                    hover = { background = EliotNodes.GetNode("Loop").ShadowTexture, textColor = Color.white },
                    active = { background = EliotNodes.GetNode("Loop").ShadowTexture, textColor = Color.white },
                    focused = { background = EliotNodes.GetNode("Loop").ShadowTexture, textColor = Color.white },

                    onNormal = { background = EliotNodes.GetNode("Loop").ShadowTexture, textColor = Color.white },
                    onHover = { background = EliotNodes.GetNode("Loop").ShadowTexture, textColor = Color.white },
                    onFocused = { background = EliotNodes.GetNode("Loop").ShadowTexture, textColor = Color.white },
                    onActive = { background = EliotNodes.GetNode("Loop").ShadowTexture, textColor = Color.white }
                };


                UtilityNode_Normal = new GUIStyle(InvokerNode_Normal)
                {
                    name = "UtilityNode_Normal",
                    fixedHeight = EliotNodes.GetNode("Utility").textureHeight,
                    fixedWidth = EliotNodes.GetNode("Utility").textureWidth,
                    contentOffset = new Vector2(0, -80),
                    normal = { background = EliotNodes.GetNode("Utility").NormalTexture, textColor = fontColor },
                    hover = { background = EliotNodes.GetNode("Utility").SelectedTexture, textColor = Color.white },
                    active = { background = EliotNodes.GetNode("Utility").SelectedTexture, textColor = Color.white },
                    focused = { background = EliotNodes.GetNode("Utility").SelectedTexture, textColor = Color.white },

                    onNormal = { background = EliotNodes.GetNode("Utility").SelectedTexture, textColor = Color.white },
                    onHover = { background = EliotNodes.GetNode("Utility").SelectedTexture, textColor = Color.white },
                    onFocused = { background = EliotNodes.GetNode("Utility").SelectedTexture, textColor = Color.white },
                    onActive = { background = EliotNodes.GetNode("Utility").SelectedTexture, textColor = Color.white }
                };
                UtilityNode_Select = new GUIStyle(UtilityNode_Normal)
                {
                    name = "UtilityNode_Select",
                    normal = { background = EliotNodes.GetNode("Utility").SelectedTexture, textColor = fontColor }
                };
                UtilityNode_Fade = new GUIStyle(UtilityNode_Normal)
                {
                    name = "UtilityNode_Fade",
                    normal = { background = EliotNodes.GetNode("Utility").ShadowTexture, textColor = Color.white },
                    hover = { background = EliotNodes.GetNode("Utility").ShadowTexture, textColor = Color.white },
                    active = { background = EliotNodes.GetNode("Utility").ShadowTexture, textColor = Color.white },
                    focused = { background = EliotNodes.GetNode("Utility").ShadowTexture, textColor = Color.white },

                    onNormal = { background = EliotNodes.GetNode("Utility").ShadowTexture, textColor = Color.white },
                    onHover = { background = EliotNodes.GetNode("Utility").ShadowTexture, textColor = Color.white },
                    onFocused = { background = EliotNodes.GetNode("Utility").ShadowTexture, textColor = Color.white },
                    onActive = { background = EliotNodes.GetNode("Utility").ShadowTexture, textColor = Color.white }
                };
            

            //Nodes Colors
            NodeStatusNormal = new Color(0f, 0.7f, 1f, .5f);
            NodeStatusWarning = new Color(0.8f, 0.6f, 0.2f, .85f);
            NodeStatusError = new Color(1f, 0.2f, 0.1f, .85f);

            //Transitions Colors
            NeutralColor = fontColor;
            PositiveColor = new Color(0.2f, 0.8f, 0.1f, 1f);
            NegativeColor = new Color(0.8f, 0.3f, 0.2f, 1f);
            SelectedColor = new Color(1f, 1f, 0.2f, 1f);
            LoopColor = new Color(0.7f, 0.1f, 0.9f, 1f);
            SelectionBoxColor = new Color(0, 0.6f, 1, 0.05f);
            SelectionBoxOutlineColor = new Color(0.7f, 0.8f, 1, 0.5f);
        }


        /// <summary>
        /// Adjust the font size of the content text for a node.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static GUIStyle GetLabelStyle(string input)
        {
            int len = input.Length;
            if (len <= nodeMaxChar)
            {
                return new GUIStyle(NodeLabel);
            }
            else if (len > nodeMaxChar)
            {
                return new GUIStyle(NodeLabel) { fontSize = 8};
            }

            return NodeLabel;
        }

        /// <summary>
        /// Returns the corrent GUIStyle to render a node, can we utilize the fade for highlighting here?
        /// </summary>
        /// <param name="value">Name of the Node type</param>
        /// <param name="state"> State of node, 0 = Normal,1 = Selected, 2 = Fade</param>
        /// <returns></returns>
        public static GUIStyle GetNodeStyle(string value, int state)
        {
            switch (value)
            {
                case "Entry":
                    return state == 0 ? EntryNode_Normal : state == 1 ? EntryNode_Select : EntryNode_Fade;
                case "Action":
                    return state == 0 ? InvokerNode_Normal : state == 1 ? InvokerNode_Select : InvokerNode_Fade;
                case "Condition":
                    return state == 0 ? ObserverNode_Normal : state == 1 ? ObserverNode_Select : ObserverNode_Fade;
                case "Skill":
                    return state == 0 ? SkillNode_Normal : state == 1 ? SkillNode_Select : SkillNode_Fade;
                case "Time":
                    return state == 0 ? TimeNode_Normal : state == 1 ? TimeNode_Select : TimeNode_Fade;
                case "Loop":
                    return state == 0 ? LoopNode_Normal : state == 1 ? LoopNode_Select : LoopNode_Fade;
                case "Utility":
                    return state == 0 ? UtilityNode_Normal : state == 1 ? UtilityNode_Select : UtilityNode_Fade;
                default:
                    return state == 0 ? EntryNode_Normal : state == 1 ? EntryNode_Select : EntryNode_Fade;
            }
        }
    }
}
#endif