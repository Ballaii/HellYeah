#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Eliot.BehaviourEditor
{
    /// <summary>
    /// Handles copying the nodes.
    /// </summary>
    public static class CopyBuffer
    {
        /// <summary>
        /// Buffer list.
        /// </summary>
        public static List<Object> Objects = new List<Object>();
        
        /// <summary>
        /// Buffer helper dictionary. Used to restore transitions.
        /// </summary>
        public static Dictionary<Node, Node> NodesDictionary = new Dictionary<Node, Node>();
        
        /// <summary>
        /// Copy the selected objects references into the buffer.
        /// </summary>
        /// <param name="selection"></param>
        public static void Copy(Object[] selection)
        {
            Objects = new List<Object>();
            NodesDictionary = new Dictionary<Node, Node>();
            
            foreach (var entry in selection)
            {
                if(!(entry is EntryNode))
                    Objects.Add(entry);
            }
        }

        /// <summary>
        /// Instantiate a copy of the copied objects.
        /// </summary>
        public static void Paste()
        {
            NodesDictionary = new Dictionary<Node, Node>();
            if (Objects.Count > 0)
            {
                var newSelection = new List<Object>();
                
                foreach (var bufferNode in Objects)
                {
                    if (!(bufferNode is Node))
                    {
                        continue;
                    }
                    var copyNode = bufferNode as Node;
                    var behaviourEditorWindow = EditorWindow.GetWindow<BehaviourEditorWindow>("Behaviour");
                    copyNode.Behaviour = behaviourEditorWindow.Behaviour;
                    var newNodeFromBuffer = Object.Instantiate(copyNode);
                    Undo.RegisterCreatedObjectUndo(newNodeFromBuffer, "Create new Node");
                    newNodeFromBuffer.Rect.position += new Vector2(20, 20);
                    if(!NodesDictionary.ContainsKey(copyNode))
                        NodesDictionary.Add(copyNode, newNodeFromBuffer);
                    if(!behaviourEditorWindow.Nodes.Contains(newNodeFromBuffer))
                        behaviourEditorWindow.Nodes.Add(newNodeFromBuffer);
                    
                    copyNode.Grouped = false;
                    newNodeFromBuffer.Grouped = true;
                    newSelection.Add(newNodeFromBuffer as Object);
                }
                
                // restore the refs
                foreach (KeyValuePair<Node, Node> entry in NodesDictionary)
                {
                    // restore the IN refs
                    foreach (var transitionIn in entry.Value.TransitionsIn)
                    {
                        //var transitionIn = transitionIn as NodesTransition;
                        transitionIn.End = entry.Key;
                    }
                    entry.Value.TransitionsIn = new List<NodesTransition>();
                    
                    // restore the OUT refs
                    foreach (var transition in entry.Value.Transitions)
                    {
                        transition.Start = entry.Key;
                    }
                }
                
                // build new transitions
                foreach (KeyValuePair<Node, Node> entry in NodesDictionary)
                {
                    var i = 0;
                    try
                    {
                        if(entry.Value is UtilityNode)
                        {
                            (entry.Value as UtilityNode).Transitions = new List<NodesTransition>();
                            for(var j = 0; j < (entry.Key as UtilityNode).Curves.Count; j++)
                            {
                                var valueCurve = (entry.Value as UtilityNode).Curves[j];
                                var keyCurve = (entry.Key as UtilityNode).Curves[j];
                                valueCurve.Transitions = new List<NodesTransition>();

                                for (i = 0; i < keyCurve.Transitions.Count; i++)
                                {
                                    var transition = keyCurve.Transitions[i];

                                    if (!(NodesDictionary.ContainsKey(transition.End))) continue;

                                    var transitionCopy = Object.Instantiate(transition) as NodesTransition;
                                    Undo.RegisterCreatedObjectUndo(transitionCopy, "Create new Transition");

                                    transitionCopy.Start = NodesDictionary[transition.Start];
                                    transitionCopy.End = NodesDictionary[transition.End];

                                    if (!valueCurve.Transitions.Contains(transitionCopy))
                                        valueCurve.Transitions.Add(transitionCopy);

                                    if (!(transitionCopy.End.TransitionsIn.Contains(transitionCopy)))
                                        transitionCopy.End.TransitionsIn.Add(transitionCopy);

                                    if (keyCurve.Transitions.Contains(transitionCopy))
                                        keyCurve.Transitions.Remove(transitionCopy);

                                    if (!(entry.Value as UtilityNode).Transitions.Contains(transitionCopy))
                                        (entry.Value as UtilityNode).Transitions.Add(transitionCopy);
                                    
                                    if ((entry.Key as UtilityNode).Transitions.Contains(transitionCopy))
                                        (entry.Key as UtilityNode).Transitions.Remove(transitionCopy);
                                }
                            }
                        }
                        else
                        {
                            entry.Value.Transitions = new List<NodesTransition>();
                        
                            for (i = 0; i < entry.Key.Transitions.Count; i++)
                            {
                                var transition = entry.Key.Transitions[i];

                                if (!(NodesDictionary.ContainsKey(transition.End))) continue;

                                var transitionCopy = Object.Instantiate(transition) as NodesTransition;
                                Undo.RegisterCreatedObjectUndo(transitionCopy, "Create new Transition");

                                transitionCopy.Start = NodesDictionary[transition.Start];
                                transitionCopy.End = NodesDictionary[transition.End];

                                if (!entry.Value.Transitions.Contains(transitionCopy))
                                    entry.Value.Transitions.Add(transitionCopy);

                                if (!(transitionCopy.End.TransitionsIn.Contains(transitionCopy)))
                                    transitionCopy.End.TransitionsIn.Add(transitionCopy);

                                if (entry.Key.Transitions.Contains(transitionCopy))
                                    entry.Key.Transitions.Remove(transitionCopy);
                            }
                        }
                        
                    }
                    catch (System.Exception) {}
                }
                
                Selection.objects = newSelection.ToArray();
            }
            var window = EditorWindow.GetWindow<BehaviourEditorWindow>("Behaviour");
            window.Save(null);
        }
    }
}
#endif