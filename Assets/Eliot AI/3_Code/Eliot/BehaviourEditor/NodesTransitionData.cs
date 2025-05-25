using System;
using System.Reflection;
using Eliot.AgentComponents;
using UnityEngine;
using Eliot.BehaviourEngine;

namespace Eliot.BehaviourEditor
{
	/// <summary>
	/// Encapsulates all the necessary data for a single core transition.
	/// </summary>
	[System.Serializable]
	public class NodesTransitionData
	{
		/// <summary>
		/// Used to identify and access the Transition via code.
		/// </summary>
		public string TransitionId = "Transition";
		
		[Header("Probability")]
		[Tooltip("Minimum times the Transition will skip the Update" +
		         " before activating its target Component.")]
		[SerializeField]
		public float MinProbability = 100f;

		[Tooltip("Maximum times the Transition will skip the Update " +
		         "before activating its target Component.")]
		[SerializeField]
		public float MaxProbability = 100f;

		//[Header("Cooldown")]
		[Tooltip("Minimum duration of time the Transition should skip the Update.")] [SerializeField]
		public float MinCooldown;

		[Tooltip("Maximum duration of time the Transition should skip the Update.")] [SerializeField]
		public float MaxCooldown;

		[Space]
		[Tooltip("Whether or not the successful Update should prevent all the" +
		         " other Transitions in the same group from being Updated.")]
		public bool Terminate;

		/// <summary>
		/// Associated runtime core component.
		/// </summary>
		public Transition BindedCoreTransition;

		/// <summary>
		/// Holds the data necessary for the method execution.
		/// </summary>
		public MethodData MethodData;

		/// <summary>
		/// The name of the current captured function.
		/// </summary>
		public string FunctionName = null;

		/// <summary>
		/// Holds the currently captured condition group index.
		/// </summary>
		public int ConditionGroupIndex;

		/// <summary>
		/// Holds the currently captured function index.
		/// </summary>
		public int FuncIndex;

		/// <summary>
		/// Whether to give control to the Start node upon activation.
		/// </summary>
		[Tooltip("If true, the origin node of the transition will be executed every frame instead of Entry until BREAK is called.")]
		public bool CaptureControl = false;

		/// <summary>
		/// Whether to give control to the Start node upon activation if the executed condition returns true.
		/// </summary>
		public bool CaptureControlOnTrue = false;

		/// <summary>
		/// Whether to give control to the Start node upon activation if the executed condition returns false.
		/// </summary>
		public bool CaptureControlOnFalse = false;

		/// <summary>
		/// Whether to check an arbitrary condition upon activation. Filters the execution of the branch if the condition returns false.
		/// </summary>
		public bool UseCondition = false;

		/// <summary>
		/// Reverses the returned by condition boolean.
		/// </summary>
		public bool Reverse = false;

		/// <summary>
		/// The handler reference.
		/// </summary>
		public NodesTransition Transition;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="transition"></param>
		public NodesTransitionData(NodesTransition transition)
		{
			this.Transition = transition;
		}

		/// <summary>
		/// Build and return the code element.
		/// </summary>
		/// <param name="startComponent"></param>
		/// <param name="gameObject"></param>
		/// <param name="core"></param>
		/// <returns></returns>
		public BehaviourEngine.Transition BuildBehaviourTransition(CoreComponent startComponent, GameObject gameObject,
			BehaviourCore core)
		{
			Transition resultTransition = null;
			if (Transition.Start is ConditionNode)
			{
				if (Transition.IsNegative)
				{
					var coreEnd = Transition.End.GetBehaviourEngineComponent(gameObject, core);
					resultTransition = (startComponent as Condition).ConnectWith_Else(
						(startComponent as Condition), coreEnd, MinProbability, MaxProbability, MinCooldown, MaxCooldown,
						Terminate);
				}
				else
				{
					var coreEnd = Transition.End.GetBehaviourEngineComponent(gameObject, core);
					resultTransition = (startComponent as Condition).ConnectWith(
						(startComponent as Condition), coreEnd, MinProbability, MaxProbability, MinCooldown, MaxCooldown,
						Terminate);
				}
			}
			else if (Transition.Start is LoopNode)
			{
				if (Transition.IsNegative)
					resultTransition = (startComponent as Loop)
						.ConnectWith_End((startComponent as Loop),
							Transition.End.GetBehaviourEngineComponent(gameObject, core),
							MinProbability, MaxProbability, MinCooldown, MaxCooldown, Terminate
						);
				else
					resultTransition = (startComponent as Loop)
						.ConnectWith((startComponent as Loop),
							Transition.End.GetBehaviourEngineComponent(gameObject, core),
							MinProbability, MaxProbability, MinCooldown, MaxCooldown, Terminate
						);
			}
			else if (Transition.Start is TimeNode)
			{
				if (Transition.IsNegative)
					resultTransition = (startComponent as Loop)
						.ConnectWith_End((startComponent as Loop),
							Transition.End.GetBehaviourEngineComponent(gameObject, core),
							MinProbability, MaxProbability, MinCooldown, MaxCooldown, Terminate
						);
				else
					resultTransition = (startComponent as Loop)
						.ConnectWith((startComponent as Loop),
							Transition.End.GetBehaviourEngineComponent(gameObject, core),
							MinProbability, MaxProbability, MinCooldown, MaxCooldown, Terminate
						);
			}
			else
			{
				var coreEnd = Transition.End.GetBehaviourEngineComponent(gameObject, core);
				resultTransition = startComponent.ConnectWith(startComponent, coreEnd, MinProbability, MaxProbability,
					MinCooldown, MaxCooldown, Terminate);
			}

			if (UseCondition)
			{
				var component = gameObject.GetComponent<EliotAgent>().AddInterface(MethodData.FullClassName);
				var metInfo = component.GetType()
					.GetMethod(MethodData.FullMethodName, BindingFlags.Public | BindingFlags.Instance);
				if (metInfo == null) return null;
				var numParams = metInfo.GetParameters().Length;
				if (numParams == 0)
				{
					var condition =
						(EliotCondition) Delegate.CreateDelegate(typeof(EliotCondition), component, metInfo, true);
					resultTransition.SetCondition(condition, Reverse);
				}
				else if (numParams == 1)
				{
					var pType = MethodData.MethodParameters[0].Type();

					var param = MethodData.GetParameters()[0];

					if (pType == typeof(bool))
					{
						var condition = (EliotCondition<bool>) Delegate.CreateDelegate(typeof(EliotCondition<bool>),
							component, metInfo, true);
						resultTransition.SetCondition(condition, (bool) param, Reverse);
					}
					else if (pType == typeof(int))
					{
						var condition = (EliotCondition<int>) Delegate.CreateDelegate(typeof(EliotCondition<int>),
							component, metInfo, true);
						resultTransition.SetCondition(condition, (int) param, Reverse);
					}
					else if (pType == typeof(float))
					{
						var condition = (EliotCondition<float>) Delegate.CreateDelegate(typeof(EliotCondition<float>),
							component, metInfo, true);
						resultTransition.SetCondition(condition, (float) param, Reverse);
					}
					else if (pType == typeof(string))
					{
						var condition = (EliotCondition<string>) Delegate.CreateDelegate(typeof(EliotCondition<string>),
							component, metInfo, true);
						resultTransition.SetCondition(condition, (string) param, Reverse);
					}
					else
					{
						resultTransition.SetCondition(MethodData, gameObject, Reverse);
					}
				}
				else
				{
					resultTransition.SetCondition(MethodData, gameObject, Reverse);
				}
			}

			resultTransition.id = TransitionId;
			resultTransition.captureControl = CaptureControl;

			return resultTransition;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Associate the transition with a runtime behaviour core component for the debugging purposes. 
		/// </summary>
		/// <param name="coreTransition"></param>
		public virtual void BindToCoreTransition(Transition coreTransition)
		{
			BindedCoreTransition = coreTransition;
			Transition.End.BindToCoreComponent(coreTransition.end);
		}
#endif
	}
}