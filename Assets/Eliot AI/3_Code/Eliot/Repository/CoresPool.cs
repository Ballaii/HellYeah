#pragma warning disable CS0219, CS1692
using UnityEngine;
using Eliot.BehaviourEngine;
using Eliot.BehaviourEditor;

namespace Eliot.Repository
{
	/// <summary>
	/// Responsible for turning json string into behaviour algorithm.
	/// </summary>
    public static class CoresPool
    {
	    /// <summary>
	    /// Turn the Behaviour data holder into an executable behaviour binded to a particular agent.
	    /// </summary>
	    /// <param name="behaviour"></param>
	    /// <param name="gameObject"></param>
	    /// <returns></returns>
	    public static BehaviourCore GetCore(EliotBehaviour behaviour, GameObject gameObject)
	    {
		    BehaviourCore resultCore = new BehaviourCore();
#if UNITY_EDITOR
		    resultCore.FullPath = behaviour.GetFullPath();
#endif
		    EntryNode entryNode = null;
		    foreach (var node in behaviour.Nodes)
		    {
			    if (node is EntryNode)
			    {
				    entryNode = node as EntryNode;
				    break;
			    }
		    }
		    if (entryNode == null) return null;
#if UNITY_EDITOR
		    entryNode.ResetBehaviourEngineComponent();
#endif
		    entryNode.GetBehaviourEngineComponent(gameObject, resultCore);
		    return resultCore;
	    }
    }
}