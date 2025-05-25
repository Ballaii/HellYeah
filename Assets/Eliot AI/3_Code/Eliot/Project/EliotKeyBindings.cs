using System.Collections.Generic;
using UnityEngine;

namespace Eliot
{
    [System.Serializable]
    public class EliotKeyBinding
    {
        /// <summary>
        /// Possible actions.
        /// </summary>
        public enum KeyBindingOptions
        {
            CreateInvoker,
            CreateObserver,
            CreateTime,
            CreateSkill,
            CreateUtility,
            StartDefaultTransition,
            StartAlternativeTransition,
            Frame,
        }

        /// <summary>
        /// Name to display.
        /// </summary>
        public string Name;

        /// <summary>
        /// Action.
        /// </summary>
        public KeyBindingOptions keyBindingOptions;

        /// <summary>
        /// Input.
        /// </summary>
        public List<KeyCode> Keys = new List<KeyCode>();

        /// <summary>
        /// Create new binding.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="keyBindingOptions"></param>
        /// <param name="keys"></param>
        public EliotKeyBinding(string name, KeyBindingOptions keyBindingOptions, List<KeyCode> keys)
        {
            this.Name = name;
            this.keyBindingOptions = keyBindingOptions;
            this.Keys = keys;
        }

        /// <summary>
        /// Default binding option for creation of an Action Node.
        /// </summary>
        public static EliotKeyBinding CreateInvoker
        {
            get
            {
                return new EliotKeyBinding(
                    "CreateInvoker",
                    KeyBindingOptions.CreateInvoker,
                    new List<KeyCode>
                    {
                                KeyCode.I
                    }
                );
            }
        }

        /// <summary>
        /// Default binding option for creation of an Condition Node.
        /// </summary>
        public static EliotKeyBinding CreateObserver
        {
            get
            {
                return new EliotKeyBinding(
                    "CreateObserver",
                    KeyBindingOptions.CreateObserver,
                    new List<KeyCode>
                    {
                                KeyCode.O
                    }
                );
            }
        }

        /// <summary>
        /// Default binding option for creation of a Time Node.
        /// </summary>
        public static EliotKeyBinding CreateTime
        {
            get
            {
                return new EliotKeyBinding(
                    "CreateTime",
                    KeyBindingOptions.CreateTime,
                    new List<KeyCode>
                    {
                                KeyCode.T
                    }
                );
            }
        }

        /// <summary>
        /// Default binding option for creation of a Skill Node.
        /// </summary>
        public static EliotKeyBinding CreateSkill
        {
            get
            {
                return new EliotKeyBinding(
                    "CreateSkill",
                    KeyBindingOptions.CreateSkill,
                    new List<KeyCode>
                    {
                                KeyCode.P
                    }
                );
            }
        }

        /// <summary>
        /// Default binding option for creation of a Utility Node.
        /// </summary>
        public static EliotKeyBinding CreateUtility
        {
            get
            {
                return new EliotKeyBinding(
                    "CreateUtility",
                    KeyBindingOptions.CreateUtility,
                    new List<KeyCode>
                    {
                                KeyCode.U
                    }
                );
            }
        }

        /// <summary>
        /// Default binding option for starting a default transition.
        /// </summary>
        public static EliotKeyBinding StartDefaultTransition
        {
            get
            {
                return new EliotKeyBinding(
                    "StartDefaultTransition",
                    KeyBindingOptions.StartDefaultTransition,
                    new List<KeyCode>
                    {
                                KeyCode.Y
                    }
                );
            }
        }

        /// <summary>
        /// Default binding option for starting an alternative transition.
        /// </summary>
        public static EliotKeyBinding StartAlternativeTransition
        {
            get
            {
                return new EliotKeyBinding(
                    "StartAlternativeTransition",
                    KeyBindingOptions.StartAlternativeTransition,
                    new List<KeyCode>
                    {
                                KeyCode.N
                    }
                );
            }
        }

        /// <summary>
        /// Default binding option for centering the view rect on the nodes.
        /// </summary>
        public static EliotKeyBinding Frame
        {
            get
            {
                return new EliotKeyBinding(
                    "Frame",
                    KeyBindingOptions.Frame,
                    new List<KeyCode>
                    {
                                KeyCode.F
                    }
                );
            }
        }
    }
}