#pragma warning disable CS0414, CS0649, CS1692
using Eliot.BehaviourEditor;
using UnityEngine;

namespace Eliot.BehaviourEngine
{
	/// <summary>
	/// Transition binds other components together,
	/// transmitting information between them.
	/// </summary>
	public class Transition
	{
		/// <summary>
		/// Defines a unique name of the component in the model.
		/// </summary>
		public string id;

		/// <summary>
		/// The component from which the transition originates.
		/// </summary>
		public CoreComponent start;

		/// <summary>
		/// The component towards which the transition is directed.
		/// </summary>
		public CoreComponent end;

		/// <summary>
		/// Is true when the component is being executed.
		/// </summary>
		public bool active = false;

		/// <summary>
		/// Is true when the component is being executed. Might be updated within a single iteration recursively.
		/// </summary>
		public bool singleIterationActive = false;

		/// If true, action taken upon checking the condition will be reversed.
		public bool reverse;

		/// <summary>
		/// Keep track of which parameters to use on execution.
		/// </summary>
		public ActivationModes activationMode;

		public EliotCondition condition;
		public EliotCondition<bool> conditionBool;
		public EliotCondition<int> conditionInt;
		public EliotCondition<float> conditionFloat;
		public EliotCondition<string> conditionString;
		public MethodData methodData;
		public object parameterValue;
		public GameObject gameObject;

		/// Minimum times the Transition will skip the Update before activating
		/// its target Component.
		public float activationProbabilityMin;

		/// Maximum times the Transition will skip the Update before activating
		/// its target Component.
		public float activationProbabilityMax;

		/// Whether or not the Transition should skip any Updates based on probability.
		public bool useProbability = true;

		/// Minimum duration of time the Transition should skip the Update.
		public float minCooldown;

		/// Maximum duration of time the Transition should skip the Update.
		public float maxCooldown;

		/// Last time the Transition Updated based on cooldown.
		public float lastTimeUpdated;

		/// Actual duration of time the Transition should skip the Update.
		public float cooldown;

		/// Whether or not the Transition should skip any Updates based on cooldown.
		public bool useCooldown = true;

		/// Whether or not the successful Update should prevent all the
		/// other Transitions in the same group from being Updated.
		public bool terminateOriginUpdate;

		/// <summary>
		/// If true, set the strat component as the active component of the behaviour core.
		/// </summary>
		public bool captureControl;

		/// <summary>
		/// Initialize the component.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="activationProbabilityMin"></param>
		/// <param name="activationProbabilityMax"></param>
		/// <param name="minCooldown"></param>
		/// <param name="maxCooldown"></param>
		/// <param name="terminateOriginUpdate"></param>
		public Transition(CoreComponent start, CoreComponent end, float activationProbabilityMin,
			float activationProbabilityMax,
			float minCooldown, float maxCooldown, bool terminateOriginUpdate)
		{
			this.start = start;
			this.end = end;
			this.activationProbabilityMin = activationProbabilityMin;
			this.activationProbabilityMax = activationProbabilityMax;

			if (this.activationProbabilityMin == 100f && this.activationProbabilityMax == 100f)
				useProbability = false;

			if (minCooldown < 0) minCooldown = 0;
			if (maxCooldown < 0) maxCooldown = 0;
			if (minCooldown == 0 && maxCooldown == 0) useCooldown = false;
			if (useCooldown)
			{
				if (maxCooldown <= minCooldown)
					maxCooldown = minCooldown + 1;
			}

			this.minCooldown = minCooldown;
			this.maxCooldown = maxCooldown;

			this.terminateOriginUpdate = terminateOriginUpdate;

			this.activationMode = ActivationModes.None;
		}

		/// <summary>
		/// Set a condition with no parameters.
		/// </summary>
		/// <param name="condition"></param>
		/// <param name="reverse"></param>
		public void SetCondition(EliotCondition condition, bool reverse)
		{
			this.condition = condition;
			this.reverse = reverse;
			this.activationMode = ActivationModes.NoParameters;
		}

		/// <summary>
		/// Set a condition with a single boolean parameter.
		/// </summary>
		/// <param name="conditionBool"></param>
		/// <param name="parameterValue"></param>
		/// <param name="reverse"></param>
		public void SetCondition(EliotCondition<bool> conditionBool, bool parameterValue, bool reverse)
		{
			this.conditionBool = conditionBool;
			this.parameterValue = parameterValue;
			this.reverse = reverse;
			this.activationMode = ActivationModes.OneParameterBool;
		}

		/// <summary>
		/// Set a condition with a single integer parameter.
		/// </summary>
		/// <param name="conditionInt"></param>
		/// <param name="parameterValue"></param>
		/// <param name="reverse"></param>
		public void SetCondition(EliotCondition<int> conditionInt, int parameterValue, bool reverse)
		{
			this.conditionInt = conditionInt;
			this.parameterValue = parameterValue;
			this.reverse = reverse;
			this.activationMode = ActivationModes.OneParameterInt;
		}

		/// <summary>
		/// Set a condition with a single float parameter.
		/// </summary>
		/// <param name="conditionFloat"></param>
		/// <param name="parameterValue"></param>
		/// <param name="reverse"></param>
		public void SetCondition(EliotCondition<float> conditionFloat, float parameterValue, bool reverse)
		{
			this.conditionFloat = conditionFloat;
			this.parameterValue = parameterValue;
			this.reverse = reverse;
			this.activationMode = ActivationModes.OneParameterFloat;
		}

		/// <summary>
		/// Set a condition with a single string parameter.
		/// </summary>
		/// <param name="conditionString"></param>
		/// <param name="parameterValue"></param>
		/// <param name="reverse"></param>
		public void SetCondition(EliotCondition<string> conditionString, string parameterValue, bool reverse)
		{
			this.conditionString = conditionString;
			this.parameterValue = parameterValue;
			this.reverse = reverse;
			this.activationMode = ActivationModes.OneParameterString;
		}

		/// <summary>
		/// Set a condition with any number of parameters.
		/// </summary>
		/// <param name="methodData"></param>
		/// <param name="gameObject"></param>
		/// <param name="reverse"></param>
		public void SetCondition(MethodData methodData, GameObject gameObject, bool reverse)
		{
			this.methodData = methodData;
			this.gameObject = gameObject;
			this.reverse = reverse;
			this.activationMode = ActivationModes.Other;
		}

		/// <summary>
		/// Is it ready to be updated?
		/// </summary>
		/// <returns></returns>
		private bool CanUpdateTransition()
		{
			bool canUpdateBasedOnProbability = useProbability || Eliot.Utility.EliotRandom.TrueWithProbability(activationProbabilityMin, activationProbabilityMax);
			bool canUpdateBasedOnCooldown = true;

			if (useCooldown)
			{
				if (Time.time >= cooldown + lastTimeUpdated)
				{
					lastTimeUpdated = Time.time;
					cooldown = Random.Range(minCooldown, maxCooldown);
				}
				else
				{
					canUpdateBasedOnCooldown = false;
				}
			}

			return canUpdateBasedOnProbability && canUpdateBasedOnCooldown;
		}

		/// <summary>
		/// Run the transition.
		/// </summary>
		/// <returns></returns>
		public bool Update()
		{
			if (captureControl)
				start.Core.ActiveComponent = start;

			if (CanUpdateTransition())
			{
				singleIterationActive = true;
				active = true;

				if (activationMode != ActivationModes.None)
				{
					var activationResult = Activate();
					if (reverse) activationResult = !activationResult;
					if (activationResult)
						end.Update();
				}
				else
				{
					end.Update();
				}

				return !terminateOriginUpdate;
			}

			return true;
		}

		/// <summary>
		/// Execute the condition function.
		/// </summary>
		/// <returns></returns>
		private bool Activate()
		{
			switch (this.activationMode)
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
						return conditionBool((bool) parameterValue);
					break;
				}

				case ActivationModes.OneParameterInt:
				{
					if (conditionInt != null)
						return conditionInt((int) parameterValue);
					break;
				}

				case ActivationModes.OneParameterFloat:
				{
					if (conditionFloat != null)
						return conditionFloat((float) parameterValue);
					break;
				}

				case ActivationModes.OneParameterString:
				{
					if (conditionString != null)
						return conditionString((string) parameterValue);
					break;
				}

				case ActivationModes.Other:
				{
					if (methodData != null)
						return (bool) methodData.Invoke(gameObject);
					break;
				}
			}

			return false;
		}

		/// <summary>
		/// Stop the execution of the current node.
		/// </summary>
		public void Relax()
		{
			if (!active) return;
			active = false;
			end.Relax();
		}

		/// <summary>
		/// Stop the execution of the current node within a single iteration to prevent recursion.
		/// </summary>
		public void RelaxSingleIteration()
		{
			if (!singleIterationActive) return;
			singleIterationActive = false;
			end.RelaxSingleIteration();
		}
	}
}

