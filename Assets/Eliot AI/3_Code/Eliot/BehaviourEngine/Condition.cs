using System.Collections.Generic;
using Eliot.BehaviourEditor;
using UnityEngine;

namespace Eliot.BehaviourEngine
{
	/// <summary>
	/// Keeps information about whatever condition needs to be checked.
	/// </summary>
	public delegate bool EliotCondition();
	
	/// <summary>
	/// Keeps information about whatever condition needs to be checked. Takes in a single parameter.
	/// </summary>
	public delegate bool EliotCondition<in T>(T obj);
	
	/// <summary>
	/// Condition is responsible for checking the specific condition
	/// and take appropriate action as far as the result goes.
	/// </summary>
	public class Condition : CoreComponent
	{
		/// <summary>
		/// List of transitions that link this component to the other ones in the model if the condition is true.
		/// </summary>
		public List<Transition> TransitionsIf
		{
			get { return transitionsIf; }
		}
		
		/// <summary>
		/// List of transitions that link this component to the other ones in the model if the condition is false.
		/// </summary>
		public List<Transition> TransitionsElse
		{
			get { return transitionsElse; }
		}
		
		/// <summary>
		/// Keep track of which parameters to use on execution.
		/// </summary>
		public ActivationModes activationMode;
		
		/// Hold the boolean method and target instance to check the condition whenever needed.
		public EliotCondition condition;
		public EliotCondition<bool> conditionBool;
		public EliotCondition<int> conditionInt;
		public EliotCondition<float> conditionFloat;
		public EliotCondition<string> conditionString;
		public object parameterValue;
		/// Links to other components that get activated when the result of checking condition is true.
		public List<Transition> transitionsIf = new List<Transition>();
		/// Links to other components that get activated when the result of checking condition is false.
		public List<Transition> transitionsElse = new List<Transition>();
		
		/// <summary>
		/// The game object to which the execution should be bind.
		/// </summary>
		public GameObject gameObject;

		/// <summary>
		/// Holds the data necessary to build an executable function at runtime. 
		/// </summary>
		public MethodData methodData;
		
		/// If true, action taken upon checking the condition will be reversed.
		public bool reverse;

		/// <summary>
		/// Empty constructor.
		/// </summary>
		public Condition(){}

		/// <summary>
		/// Initialize the component.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="preUpdateCallback"></param>
		/// <param name="postUpdateCallback"></param>
		public Condition(EliotCondition condition, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Condition")
		{
			if(condition != null) this.condition += condition;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.NoParameters;
		}
		
		/// <summary>
		/// Build an condition that takes a single boolean parameter.
		/// </summary>
		/// <param name="conditionBool"></param>
		/// <param name="parameterValue"></param>
		/// <param name="preUpdateCallback"></param>
		/// <param name="postUpdateCallback"></param>
		public Condition(EliotCondition<bool> conditionBool, bool parameterValue, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Condition")
		{
			if(conditionBool != null) this.conditionBool += conditionBool;
			this.parameterValue = parameterValue;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.OneParameterBool;
		}
		
		/// <summary>
		/// Build an condition that takes a single integer parameter.
		/// </summary>
		/// <param name="conditionInt"></param>
		/// <param name="parameterValue"></param>
		/// <param name="preUpdateCallback"></param>
		/// <param name="postUpdateCallback"></param>
		public Condition(EliotCondition<int> conditionInt, int parameterValue, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Condition")
		{
			if(conditionInt != null) this.conditionInt += conditionInt;
			this.parameterValue = parameterValue;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.OneParameterInt;
		}
		
		/// <summary>
		/// Build an condition that takes a single float parameter.
		/// </summary>
		/// <param name="conditionFloat"></param>
		/// <param name="parameterValue"></param>
		/// <param name="preUpdateCallback"></param>
		/// <param name="postUpdateCallback"></param>
		public Condition(EliotCondition<float> conditionFloat, float parameterValue, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Condition")
		{
			if(conditionFloat != null) this.conditionFloat += conditionFloat;
			this.parameterValue = parameterValue;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.OneParameterFloat;
		}
		
		/// <summary>
		/// Build an condition that takes a single string parameter.
		/// </summary>
		/// <param name="conditionString"></param>
		/// <param name="parameterValue"></param>
		/// <param name="preUpdateCallback"></param>
		/// <param name="postUpdateCallback"></param>
		public Condition(EliotCondition<string> conditionString, string parameterValue, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Condition")
		{
			if(conditionString != null) this.conditionString += conditionString;
			this.parameterValue = parameterValue;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.OneParameterString;
		}
		
		/// <summary>
		/// Build an condition that takes any number of parameters.
		/// </summary>
		/// <param name="methodData"></param>
		/// <param name="gameObject"></param>
		/// <param name="preUpdateCallback"></param>
		/// <param name="postUpdateCallback"></param>
		public Condition(MethodData methodData, GameObject gameObject, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Action")
		{
			this.methodData = methodData;
			this.gameObject = gameObject;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.Other;
		}

		/// <summary>
		/// Make Condition do its job.
		/// </summary>
		public override void Update()
		{
			Active = true;
			
			if (Core.ActiveComponent != this && SingleIterationActive) return;
			SingleIterationActive = true;
			
			if(PreUpdateCallback != null) PreUpdateCallback.Invoke();
			
			Status = CoreComponentStatus.Normal;

			bool conditionMet = false;
			try
			{
				conditionMet = reverse? !Activate() : Activate();
			}
			catch (EliotAgentComponentNotFoundException e)
			{
				Status = CoreComponentStatus.Warning;
				Debug.LogWarning(e, gameObject);
			}
			catch (System.Exception e)
			{
				Status = CoreComponentStatus.Error;
				Debug.LogError(e, gameObject);
			}
			
			switch (conditionMet)
			{
				case true:
				{
					if (transitionsIf.Count == 0) return;
					foreach(var transition in transitionsIf)
						if (!transition.Update()) break;
					RelaxElse();
					break;
				}
				case false:
				{
					if (transitionsElse.Count == 0) return;
					foreach(var transition in transitionsElse)
						if (!transition.Update()) break;
					RelaxIf();
					break;
				}
			}
			
			if(Core.ActiveComponent == this) RelaxSingleIteration();
			
			if(PostUpdateCallback != null) PostUpdateCallback.Invoke();
		}

		/// <summary>
		/// Execute the condition function.
		/// </summary>
		/// <returns></returns>
		private bool Activate()
		{
			if (CaptureControl) Core.SetActiveComponent(this);
			
			switch(this.activationMode)
			{
				case ActivationModes.NoParameters:
				{
					if (condition != null)
						return condition();
					break;
				}
				case ActivationModes.OneParameterBool:
				{
					if (conditionBool != null)
						return conditionBool((bool)parameterValue);
					break;
				}
				case ActivationModes.OneParameterInt:
				{
					if (conditionInt != null)
						return conditionInt((int)parameterValue);
					break;
				}
				case ActivationModes.OneParameterFloat:
				{
					if (conditionFloat != null)
						return conditionFloat((float)parameterValue);
					break;
				}
				case ActivationModes.OneParameterString:
				{
					if (conditionString != null)
						return conditionString((string)parameterValue);
					break;
				}
				case ActivationModes.Other:
				{
					if (methodData != null)
						return (bool)methodData.Invoke(gameObject);
					break;
				}
			}

			return false;
		}
		
		/// <summary>
		/// Stop the execution of the current node.
		/// </summary>
		public override void Relax()
		{
			if (!Active) return;
			Active = false;

			RelaxIf();

			RelaxElse();
		}

		/// <summary>
		/// Stop the execution of the 'if' branch.
		/// </summary>
		private void RelaxIf()
		{
			if (transitionsIf.Count > 0)
			{
				foreach (var transition in transitionsIf)
					transition.Relax();
			}
		}
        
		/// <summary>
		/// Stop the execution of the 'else' branch.
		/// </summary>
		private void RelaxElse()
		{
			if (transitionsElse.Count > 0)
			{
				foreach (var transition in transitionsElse)
					transition.Relax();
			}
		}
		
		/// <summary>
		/// Stop the execution of the current node within a single iteration to prevent recursion.
		/// </summary>
		public override void RelaxSingleIteration()
		{
			if (!SingleIterationActive) return;
			SingleIterationActive = false;

			RelaxSingleIterationIf();

			RelaxSingleIterationElse();
		}
        
		/// <summary>
		/// Stop the execution of the 'if' branch a single iteration to prevent recursion.
		/// </summary>
		private void RelaxSingleIterationIf()
		{
			if (transitionsIf.Count > 0)
			{
				foreach (var transition in transitionsIf)
					transition.RelaxSingleIteration();
			}
		}
        
		/// <summary>
		/// Stop the execution of the 'else' branch a single iteration to prevent recursion.
		/// </summary>
		private void RelaxSingleIterationElse()
		{
			if (transitionsElse.Count > 0)
			{
				foreach (var transition in transitionsElse)
					transition.RelaxSingleIteration();
			}
		}

		/// <summary>
		/// Create transition in 'True' group between the Condition and other component.
		/// </summary>
		/// <param name="end"></param>
		/// <param name="minProbability"></param>
		/// <param name="maxProbability"></param>
		/// <param name="minCooldown"></param>
		/// <param name="maxCooldown"></param>
		/// <param name="terminate"></param>
		/// <param name="start"></param>
		public override Transition ConnectWith(CoreComponent start, CoreComponent end, float minProbability, float maxProbability,
			float minCooldown, float maxCooldown, bool terminate)
		{
			var transition = new Transition(start, end, minProbability, maxProbability, minCooldown, maxCooldown, terminate);
			transitionsIf.Add(transition);
			return transition;
		}

		/// <summary>
		/// Create transition in 'False' group between the Condition and other component.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="minProbability"></param>
		/// <param name="maxProbability"></param>
		/// <param name="minCooldown"></param>
		/// <param name="maxCooldown"></param>
		/// <param name="terminate"></param>
		public Transition ConnectWith_Else(CoreComponent start, CoreComponent end, float minProbability, float maxProbability,
			float minCooldown, float maxCooldown, bool terminate)
		{
			var transition = new Transition(start, end, minProbability, maxProbability, minCooldown, maxCooldown, terminate);
			transitionsElse.Add(transition);
			return transition;
		}
	}
}
