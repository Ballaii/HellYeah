using Eliot.BehaviourEngine;
using Eliot.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Eliot.BehaviourEditor
{
    /// <summary>
    /// Node that represents the Loop Component of the behaviour model.
    /// </summary>
    public class TimeNode : Node
    {

        /// <summary>
        /// If true, action taken upon checking the condition will be reversed.
        /// </summary>
        public bool Reverse
        {
            get { return _reverse; }
            set { _reverse = value; }
        }

        /// <summary>
        /// Minimum time to be executing the 'while' transitions.
        /// </summary>
        public float MinTime;

        /// <summary>
        /// Maximum time to be executing the 'while' transitions.
        /// </summary>
        public float MaxTime;

        /// <summary>
        /// Get random value between min and max time.
        /// </summary>
        public float RandomTime
        {
            get { return UnityEngine.Random.Range(MinTime, MaxTime); }
        }

        [Tooltip("If true, action taken upon checking the condition will be reversed.")] [SerializeField]
        private bool _reverse;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public TimeNode()
        {
        }

        /// <summary>
        /// Initialize the LoopNode.
        /// </summary>
        /// <param name="rect"></param>
        public TimeNode(Rect rect) : base(rect, "Time")
        {
        }

#if UNITY_EDITOR
        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        public new void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// Update the node's functionality.
        /// </summary>
        public override void Update()
        {
            if (Transitions.Count <= 0) return;
            foreach (var transition in Transitions)
            {
                transition.Draw();
            }
        }

        /// <summary>
        /// Draw the node.
        /// </summary>
        public override void DrawContent()
        {
            GUILayout.Label("For " + ((MinTime != MaxTime) ? (MinTime + " - " + MaxTime) : MinTime.ToString()) + " sec", new GUIStyle(EliotGUISkin.NodeLabel) { normal = { textColor = EliotGUISkin.LoopColor } });
        }

        /// <summary>
        /// Draw the context menu of the node.
        /// </summary>
        public override void DrawMenu()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Transition/While"), false, StartTransition, true);
            menu.AddItem(new GUIContent("Transition/End of loop"), false, StartTransition, false);
            menu.AddItem(new GUIContent("Delete"), false, Delete, null);
            menu.ShowAsContext();
        }

        /// <summary>
        /// Initialize the transition starting from this node.
        /// </summary>
        /// <param name="obj"></param>
        private void StartTransition(object obj)
        {
            if ((bool) obj)
                Window.StartTransition(Rect, this, EliotGUISkin.LoopColor);
            else Window.StartTransition(Rect, this, EliotGUISkin.NeutralColor, true);
        }
#endif
        
        /// <summary>
        /// Build the behaviour engine component from the input data.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="core"></param>
        /// <returns></returns>
        public override CoreComponent GetBehaviourEngineComponent(GameObject gameObject, BehaviourCore core)
        {
            if (TempBindedCoreComponent != null) return TempBindedCoreComponent;

            var condition = EliotTime.IsTime(RandomTime, gameObject.GetHashCode(), EditorId);
            var loop = new Loop(condition);
            loop.Core = core;
            loop.id = NodeId;
            core.Elements.Add(loop);
            TempBindedCoreComponent = loop;
            foreach (var transition in Transitions)
            {
                transition.BuildBehaviourTransition(TempBindedCoreComponent, gameObject, core);
            }

            if (TempBindedCoreComponent == null)
            {
                Debug.LogError("Time node is null");
            }

            return TempBindedCoreComponent;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Associate the node with a runtime behaviour core component for the debugging purposes. 
        /// </summary>
        /// <param name="coreComponent"></param>
        public override void BindToCoreComponent(CoreComponent coreComponent)
        {
            BindedCoreComponent = coreComponent;
            if (Transitions.Count > 0)
            {
                int else_i = 0;
                int if_i = 0;
                for (int i = 0; i < Transitions.Count; i++)
                {
                    for (int j = 0; j < Transitions[i].TransitionsData.Count; j++)
                    {
                        if (Transitions[i].IsNegative)
                            Transitions[i].TransitionsData[j]
                                .BindToCoreTransition((coreComponent as Loop).transitionsEnd[else_i++]);
                        else
                            Transitions[i].TransitionsData[j]
                                .BindToCoreTransition((coreComponent as Loop).transitionsWhile[if_i++]);
                    }
                }
            }
        }
#endif
    }
}




