using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace Eliot.BehaviourEditor
{
	/// <summary>
	/// Keeps the information about elements of the behaviour, and their interconnections.
	/// </summary>
	[Serializable][CreateAssetMenu(fileName = "New Behaviour", menuName = "Eliot AI/Behaviour")]
	public class EliotBehaviour : ScriptableObject
	{
		/// <summary>
		/// String that contains all needed information about the structure of the Behaviour model.
		/// </summary>
		public string Json
		{
			get { return _json; }
			set { _json = value; }
		}

		/// <summary>
		/// Collection of nodes of the behaviour model.
		/// </summary>
		[HideInInspector] public List<Node> Nodes = new List<Node>();

		/// <summary>
		/// Collection of transitions of the behaviour model.
		/// </summary>
		[HideInInspector] public List<NodesTransition> Transitions = new List<NodesTransition>();

		/// String that contains all needed information about the structure of the Behaviour model.
		[HideInInspector][SerializeField] private string _json; 
#if UNITY_EDITOR

		/// <summary>
		/// Initialize components.
		/// </summary>
		/// <param name="nodes"></param>
		public void InitTransitions(IEnumerable<Node> nodes)
		{
			var trans =  from node in nodes from transition in node.Transitions select transition;
			Transitions = trans.ToList();
		}

		/// <summary>
		/// Dump the nodes to disc.
		/// </summary>
		public void Save()
		{
			Object[] data = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
			foreach (var node in Nodes)
			{
				if (data.Contains(node)) continue;
				if (AssetDatabase.Contains(node)) continue;
				try
				{
					AssetDatabase.AddObjectToAsset(node, this);
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(node));
				}
				catch (System.Exception){/**/}
			}
			foreach (var node in Transitions)
			{
				try
				{
					if (data.Contains(node)) continue;
					if (AssetDatabase.Contains(node)) continue;
					AssetDatabase.AddObjectToAsset(node, this);
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(node));
				}catch(System.Exception){}
			}
		}

		/// <summary>
		/// Read the nodes from disc.
		/// </summary>
		public void Load()
		{
			AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
		}
		
		/// <summary>
		/// Returns the relative path to the asset.
		/// </summary>
		/// <returns></returns>
		public string GetFullPath()
		{
			return AssetDatabase.GetAssetPath(this);
		}
#endif
	}
}