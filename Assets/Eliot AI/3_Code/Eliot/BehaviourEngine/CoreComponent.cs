#pragma warning disable CS0414, CS0649, CS1692
using System;

namespace Eliot.BehaviourEngine
{
	/// <summary>
	/// Indicates howe well the execution is going.
	/// </summary>
	public enum CoreComponentStatus
	{
		Normal,
		Warning,
		Error
	}
	
	/// <summary>
	/// Component is a base class for all objects that
	/// interact with each other in the process
	/// of the behaviour lifetime.
	/// </summary>
	public abstract class CoreComponent
	{
		/// <summary>
		/// Defines a unique name of the component in the model.
		/// </summary>
		public string id;
		
		/// Returns if the object already exists.
		private readonly bool _iExist;

		/// <summary>
		/// Is true when the component is being executed.
		/// </summary>
		public bool Active = false;
		
		/// <summary>
		/// Is true when the component is being executed. Might be updated within a single iteration recursively.
		/// </summary>
		public bool SingleIterationActive = false;
		
		/// <summary>
		/// Indicates howe well the execution is going.
		/// </summary>
		public CoreComponentStatus Status;

		/// <summary>
		/// Execute before running the node.
		/// </summary>
		public System.Action PreUpdateCallback { get; set; }
		
		/// <summary>
		/// Execute after running the node.
		/// </summary>
		public System.Action PostUpdateCallback { get; set; }

		/// <summary>
		/// If true, set this component as the active component of the behaviour core.
		/// </summary>
		public bool CaptureControl;

		/// <summary>
		/// Reference to the behaviour core.
		/// </summary>
		public BehaviourCore Core;
		
		/// <summary>
		/// Initialize component.
		/// </summary>
		/// <param name="id"></param>
		protected CoreComponent(string id = "P")
		{
			_iExist = true;
			this.id = id;
		}

		/// <summary>
		/// Make component do its job.
		/// </summary>
		public abstract void Update();
		
		/// <summary>
		/// Stop the execution of the current node.
		/// </summary>
		public abstract void Relax();
		
		/// <summary>
		/// Stop the execution of the current node within a single iteration to prevent recursion.
		/// </summary>
		public abstract void RelaxSingleIteration();
        		
		/// <summary>
		/// Add a transition between this component and the other one.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="minProbability"></param>
		/// <param name="maxProbability"></param>
		/// <param name="minCooldown"></param>
		/// <param name="maxCooldown"></param>
		/// <param name="terminate"></param>
		public abstract BehaviourEngine.Transition ConnectWith(CoreComponent start, CoreComponent end, float minProbability, float maxProbability, 
			float minCooldown, float maxCooldown, bool terminate);
		
	}
}
