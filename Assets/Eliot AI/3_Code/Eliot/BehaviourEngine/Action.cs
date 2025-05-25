using System.Collections.Generic;
using Eliot.BehaviourEditor;
using UnityEngine;

namespace Eliot.BehaviourEngine
{
	/// <summary>
	/// Action is responsible for running the method it holds on the proper object instance.
	/// </summary>
	public class Action : CoreComponent
	{
		/// <summary>
		/// List of transitions that link this component to the other ones in the model.
		/// </summary>
		public List<Transition> Transitions
		{
			get { return transitions; }
		}

		/// <summary>
		/// Keep track of which parameters to use on execution.
		/// </summary>
		public ActivationModes activationMode;
		
		#region Delegates
		/// Hold the method to be invoked.
		public System.Action action;
		public System.Action<bool> actionBool;
		public System.Action<int> actionInt;
		public System.Action<float> actionFloat;
		public System.Action<string> actionString;
		public object parameterValue;
		#endregion

		/// List of transitions that link this component to the other ones in the model.
		public List<Transition> transitions = new List<Transition>();
		
		/// <summary>
		/// The game object to which the execution should be bind.
		/// </summary>
		public GameObject gameObject;

		/// <summary>
		/// Holds the data necessary to build an executable function at runtime. 
		/// </summary>
		public MethodData methodData;
		
		
		/// <summary>
		/// Empty constructor.
		/// </summary>
		public Action(){}

		/// <summary>
		/// Initialize the component.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="preUpdateCallback"></param>
		/// <param name="postUpdateCallback"></param>
		public Action(System.Action action, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Action")
		{
			if (action != null) this.action += action;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.NoParameters;
		}

		/// <summary>
		/// Build action that takes a single boolean parameter.
		/// </summary>
		/// <param name="actionBool"></param>
		/// <param name="parameterValue"></param>
		/// <param name="preUpdateCallback"></param>
		/// <param name="postUpdateCallback"></param>
		public Action(System.Action<bool> actionBool, bool parameterValue, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Action")
		{
			if (actionBool != null) this.actionBool += actionBool;
			this.parameterValue = parameterValue;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.OneParameterBool;
		}

		/// <summary>
		/// Build action that takes a single integer parameter.
		/// </summary>
		/// <param name="actionInt"></param>
		/// <param name="parameterValue"></param>
		/// <param name="preUpdateCallback"></param>
		/// <param name="postUpdateCallback"></param>
		public Action(System.Action<int> actionInt, int parameterValue, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Action")
		{
			if (actionInt != null) this.actionInt += actionInt;
			this.parameterValue = parameterValue;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.OneParameterInt;
		}

		/// <summary>
		/// Build action that takes a single float parameter.
		/// </summary>
		/// <param name="actionFloat"></param>
		/// <param name="parameterValue"></param>
		/// <param name="preUpdateCallback"></param>
		/// <param name="postUpdateCallback"></param>
		public Action(System.Action<float> actionFloat, float parameterValue, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Action")
		{
			if (actionFloat != null) this.actionFloat += actionFloat;
			this.parameterValue = parameterValue;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.OneParameterFloat;
		}

		/// <summary>
		/// Build action that takes a single string parameter.
		/// </summary>
		/// <param name="actionString"></param>
		/// <param name="parameterValue"></param>
		/// <param name="preUpdateCallback"></param>
		/// <param name="postUpdateCallback"></param>
		public Action(System.Action<string> actionString, string parameterValue, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Action")
		{
			if (actionString != null) this.actionString += actionString;
			this.parameterValue = parameterValue;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.OneParameterString;
		}
		
		/// <summary>
		/// Build action that takes any number of parameters.
		/// </summary>
		/// <param name="methodData"></param>
		/// <param name="gameObject"></param>
		/// <param name="preUpdateCallback"></param>
		/// <param name="postUpdateCallback"></param>
		public Action(MethodData methodData, GameObject gameObject, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Action")
		{
			this.methodData = methodData;
			this.gameObject = gameObject;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.Other;
		}

		/// <summary>
		/// Make Action do its job.
		/// </summary>
		public override void Update()
		{
			Active = true;
			
			if (Core.ActiveComponent != this && SingleIterationActive) return;
			SingleIterationActive = true;
			
			if(PreUpdateCallback != null) PreUpdateCallback.Invoke();

			Status = CoreComponentStatus.Normal;
			try
			{
				Activate();
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

			if (transitions.Count == 0) return;
			foreach(var transition in transitions)
				if (!transition.Update()) break;
			
			if(Core.ActiveComponent == this)
				RelaxSingleIteration();
			
			if(PostUpdateCallback != null) PostUpdateCallback.Invoke();
		}

		/// <summary>
		/// Run the captured function.
		/// </summary>
		private void Activate() 
		{
			if (CaptureControl) Core.SetActiveComponent(this);
			
			switch(this.activationMode)
			{
				case ActivationModes.NoParameters:
				{
					if (action != null)
						action();
					break;
				}
				case ActivationModes.OneParameterBool:
				{
					if (actionBool != null)
						actionBool((bool)parameterValue);
					break;
				}
				case ActivationModes.OneParameterInt:
				{
					if (actionInt != null)
						actionInt((int)parameterValue);
					break;
				}
				case ActivationModes.OneParameterFloat:
				{
					if (actionFloat != null)
						actionFloat((float)parameterValue);
					break;
				}
				case ActivationModes.OneParameterString:
				{
					if (actionString != null)
						actionString((string)parameterValue);
					break;
				}
				case ActivationModes.Other:
				{
					if (methodData != null)
						methodData.Invoke(gameObject);
					break;
				}
			}
		}

		/// <summary>
		/// Stop the execution of the current node.
		/// </summary>
		public override void Relax()
		{
			if (!Active) return;
			Active = false;
			if (transitions.Count == 0) return;
			foreach (var transition in transitions)
				transition.Relax();
		}
		
		/// <summary>
		/// Stop the execution of the current node within a single iteration to prevent recursion.
		/// </summary>
		public override void RelaxSingleIteration()
		{
			if (!SingleIterationActive) return;
			SingleIterationActive = false;
			if (transitions.Count == 0) return;
			foreach (var transition in transitions)
				transition.RelaxSingleIteration();
		}

		/// <summary>
		/// Create transition between the Action and other component.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="minProbability"></param>
		/// <param name="maxProbability"></param>
		/// <param name="minCooldown"></param>
		/// <param name="maxCooldown"></param>
		/// <param name="terminate"></param>
		/// <returns></returns>
		public override BehaviourEngine.Transition ConnectWith(CoreComponent start, CoreComponent end, float minProbability, float maxProbability, 
			float minCooldown, float maxCooldown, bool terminate)
		{
			var transition = new Transition(start, end, minProbability, maxProbability, minCooldown, maxCooldown, terminate);
			transitions.Add(transition);
			return transition;
		}
	}
}
