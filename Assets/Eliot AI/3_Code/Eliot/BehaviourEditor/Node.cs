#pragma warning disable CS0414, CS0649, CS0612, CS1692
using System;
using System.Collections.Generic;
using Eliot.BehaviourEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Eliot.BehaviourEditor
{
    /// <summary>
    /// The base class for all the type of nodes in the Behaviour model editor.
    /// </summary>
    [Serializable]
    public class Node : ScriptableObject
    {
        /// <summary>
        /// Check whether the node already exists.
        /// </summary>
        public bool Exist;

        /// <summary>
        /// Unique window ID for the editor.
        /// </summary>
        public int EditorId;

        /// <summary>
        /// Name of the node. Reflects the node type.
        /// </summary>
        public string NodeName;

        /// <summary>
        /// Used to identify and access the Node via code.
        /// </summary>
        public string NodeId;
        
        /// <summary>
        /// The Rect component of the node.
        /// </summary>
        [SerializeField] public Rect Rect;

        /// <summary>
        /// List of node's transitions.
        /// </summary>
        public List<NodesTransition> Transitions = new List<NodesTransition>();
        
        /// <summary>
        /// List of transitions that go into the node.
        /// </summary>
        public List<NodesTransition> TransitionsIn = new List<NodesTransition>();
        
        /// <summary>
        /// Whether the node is currently grouped with another nodes.
        /// </summary>
        public bool Grouped { get; set; }
        
        /// <summary>
        /// Index of the function from the function group that the node holds information about.
        /// </summary>
        public int FuncIndex;
        
        /// <summary>
        /// Name of the method currently stored by the node.
        /// </summary>
        public string FunctionName
        {
            get { return _functionName; }
            set { _functionName = value; }
        }
        
        /// <summary>
        /// Name of the function from the function group that the node holds information about.
        /// </summary>
        public string Func{
            get{ return FuncNames.Length > FuncIndex ? FuncNames[FuncIndex] : "NaN"; }
        }
        
        /// A list of names of functions in the current function group of the node.
        [HideInInspector][SerializeField] public string[] FuncNames = {"loading..."};
        /// Name of the method currently stored by the node.
        [HideInInspector][SerializeField] private string _functionName = null;

        /// <summary>
        /// Associated core component. For visualizing purposes.
        /// </summary>
        public CoreComponent BindedCoreComponent;
        
        /// <summary>
        /// Associated core component. For visualizing purposes. Utility reference.
        /// </summary>
        public CoreComponent TempBindedCoreComponent;

        /// <summary>
        /// The behaviour to which the node belongs.
        /// </summary>
        public EliotBehaviour Behaviour;

        /// <summary>
        /// Whether to set the active core object to the current one.
        /// </summary>
        [Tooltip("If true, the node will be executed every frame instead of Entry until BREAK is called.")]
        public bool CaptureControl = false;

#if UNITY_EDITOR
        /// <summary>
        /// Reference to the editor window instance.
        /// </summary>
        protected BehaviourEditorWindow _window;

        /// <summary>
        /// Reference to the editor window instance.
        /// </summary>
        public BehaviourEditorWindow Window
        {
            get
            {
                if (_window == null)
                {
                    _window = EditorWindow.GetWindow<BehaviourEditorWindow>("Behaviour");
                }

                return _window;
            }
            set { _window = value; }
        }
#endif
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Node(){}
        
        /// <summary>
        /// Initialize the node.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="nodeName"></param>
        public Node(Rect rect, string nodeName = "Node")
        {
            NodeName = nodeName;
            name = nodeName;
            Rect = rect;
            Exist = true;
#if UNITY_EDITOR
            if (!Window.Nodes.Contains(this))
            {
                Window.Nodes.Add(this);
            }
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        public virtual void OnEnable()
        {
            hideFlags = HideFlags.HideInHierarchy;
            Exist = true;
            name = NodeName;
            if (string.IsNullOrEmpty(NodeId)) NodeId = NodeName;
            if(Transitions == null) Transitions  = new List<NodesTransition>();
            if(TransitionsIn == null) TransitionsIn  = new List<NodesTransition>();
            if (Behaviour && !Behaviour.Nodes.Contains(this))
            {
                Behaviour.Nodes.Add(this);
            }

            InitTransitions();
        }

        /// <summary>
        /// Initialize the transitions.
        /// </summary>
        public void InitTransitions()
        {
            foreach (var transitionIn in TransitionsIn)
            {
                if(!(transitionIn is NodesTransition)) continue;
                (transitionIn as NodesTransition).End = this;
            }
            foreach (var transition in Transitions)
            {
                if(!(transition is NodesTransition)) continue;
                (transition as NodesTransition).Start = this;
            }
        }

        /// <summary>
        /// Add a new transition ot the node.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        /// <param name="isNegative"></param>
        /// <returns></returns>
        public virtual NodesTransition AddTransition(Node start, Node end, Color? color = null, bool isNegative = false)
        {
            bool transitionExists = false;
            NodesTransition resultTransition = null;
            if(start is UtilityNode){
                foreach(var curve in (start as UtilityNode).Curves){
                    foreach(var transition in curve.Transitions){
                        if(transition.End == end){
                            transitionExists = true;
                            resultTransition = transition;
                            break;
                        }
                    }
                } 
            }
            else{ 
                foreach(var transition in start.Transitions){
                    if(transition.End == end){
                        transitionExists = true;
                        resultTransition = transition;
                        break;
                    }
                }
            }
            if(!transitionExists){
                var transition = CreateInstance<NodesTransition>();
				EditorUtility.SetDirty(transition);
				transition.Start = start;
				transition.End = end;
				transition.End.TransitionsIn.Add(transition);
				transition.Color = color.Value;
				transition.IsNegative = isNegative;
                start.Transitions.Add(transition);
                resultTransition = transition;
            }
            else{
                if(!(start is UtilityNode)){
                    var newTransitionData = new NodesTransitionData(resultTransition);
                    resultTransition.TransitionsData.Add(newTransitionData);
                }
            }
            return resultTransition;
        }
        
        /// <summary>
        /// Update the node's functionality.
        /// </summary>
        public virtual void Update(){}

        /// <summary>
        /// Execute after all the nodes have been updated.
        /// </summary>
        public virtual void LateUpdate()
        {
            if(CaptureControl)DisplayCaptureControl();
        }
        
        /// <summary>
        /// Draw the context menu of the node.
        /// </summary>
        public virtual void DrawMenu(){}
#endif
        /// <summary>
        /// Build a Behavior Component from the node.
        /// </summary>
        public virtual Eliot.BehaviourEngine.CoreComponent GetBehaviourEngineComponent(GameObject gameObject, BehaviourCore core)
        {
            return null;
        }
#if UNITY_EDITOR
        public virtual void ResetBehaviourEngineComponent()
        {
            if (TempBindedCoreComponent == null) return;
            TempBindedCoreComponent = null;
            foreach (var transition in Transitions)
            {
                transition.End.ResetBehaviourEngineComponent();
            }
        }

        /// <summary>
        /// Node's window component functionality.
        /// </summary>
        /// <param name="id"></param>
        public virtual void NodeFunction(int id)
        {
            EditorId = id;
            if(Exist)
                Undo.RecordObject(this, "Node has been dragged");
            if (!Grouped)
            {
                GUI.DragWindow();
            }

            DrawContent();
        }

        /// <summary>
        /// Draw GUI elements inside the node window.
        /// </summary>
        public virtual void DrawContent(){}

        /// <summary>
        /// Remove the node from the editor.
        /// </summary>
        /// <param name="obj"></param>
        public void Delete(object obj)
        {
            Exist = false;
            Window.RemoveNode(this);
        }

        /// <summary>
        /// Remove the transition from the list.
        /// </summary>
        /// <param name="transition"></param>
        public virtual void RemoveTransition(NodesTransition transition)
        {
            if(Transitions.Contains(transition))
                Transitions.Remove(transition);
        }
        
        /// <summary>
        /// Associate the node with a runtime behaviour core component for the debugging purposes. 
        /// </summary>
        /// <param name="coreComponent"></param>
        public virtual void BindToCoreComponent(CoreComponent coreComponent)
        {
            
        }
        
        /// <summary>
        /// Remove the node's association with a runtime behaviour core component for the debugging purposes. 
        /// </summary>
        public virtual void UnbindCoreComponent()
        {
            if (BindedCoreComponent == null) return;
            BindedCoreComponent = null;
            if(Transitions.Count > 0)
                for (int i = 0; i < Transitions.Count; i++)
                {
                    Transitions[i].UnbindCoreTransition();
                }
        }
        
        /// <summary>
        /// Draw the arrow that indicated the control capture.
        /// </summary>
        public void DisplayCaptureControl()
        {
            #region Arrow

            Vector3 cen = new Vector3(Rect.x + (Rect.width/2), Rect.y + (Rect.height + 20));
            Vector3[] arrowHead = new Vector3[3];
            Vector3 arrowCen = cen;
            Vector3 forward = (Vector3)(Vector3.up).normalized;
            Vector3 right = Vector3.Cross(Vector3.forward, forward).normalized;
            float size = 20f;
            float width = size * 0.3f;
            float height = size * 0.6f;

            arrowHead[0] = arrowCen - forward*25f;
            arrowHead[1] = arrowCen + forward * height + right*width - forward*25f;
            arrowHead[2] = arrowCen + forward * height - right*width - forward*25f;

            Color cD = Handles.color;
            float greyscale= 1f;
			Handles.color = new Color(greyscale, greyscale, greyscale, 0.5f);
            Handles.DrawAAConvexPolygon(arrowHead);
            Handles.color = cD;

            #endregion
        }
#endif
    }
}