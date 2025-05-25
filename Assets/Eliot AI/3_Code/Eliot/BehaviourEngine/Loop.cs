using System;
using System.Collections.Generic;
using Eliot.BehaviourEditor;
using UnityEngine;

namespace Eliot.BehaviourEngine
{
    /// <summary>
    /// Loop is responsible for checking the specific condition
    /// and take appropriate action as far as the result goes.
    /// While the result of checking its condition is true, Loop
    /// substitutes Entry in the behaviour model.
    /// </summary>
    public class Loop : CoreComponent
    {
        /// <summary>
        /// Keep track of which parameters to use on execution.
        /// </summary>
        public ActivationModes activationMode;
		
        /// Hold the method and target instance to check the condition whenever needed.
        public EliotCondition condition;
        public EliotCondition<bool> conditionBool;
        public EliotCondition<int> conditionInt;
        public EliotCondition<float> conditionFloat;
        public EliotCondition<string> conditionString;
        public object parameterValue;

        /// <summary>
        /// The game object to which the execution should be bind.
        /// </summary>
        public GameObject gameObject;

        /// <summary>
        /// Holds the data necessary to build an executable function at runtime. 
        /// </summary>
        public MethodData methodData;
        
        /// Links to other components that get activated when the result of checking condition is true.
        public List<Transition> transitionsWhile = new List<Transition>();
        /// Links to other components that get activated when the result of checking condition is false.
        public List<Transition> transitionsEnd = new List<Transition>();
        /// Link to the core, whose ActiveComponent can be substituted by this Loop.
        public BehaviourCore core;
        /// If true, action taken upon checking the condition will be reversed.
        public bool reverse;

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Loop(){}
        
        /// <summary>
        /// Initialize the component.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="preUpdateCallback"></param>
        /// <param name="postUpdateCallback"></param>
        public Loop(EliotCondition condition, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Condition")
		{
			if(condition != null) this.condition += condition;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.NoParameters;
		}
		
        /// <summary>
        /// Build loop that takes a single boolean parameter.
        /// </summary>
        /// <param name="conditionBool"></param>
        /// <param name="parameterValue"></param>
        /// <param name="preUpdateCallback"></param>
        /// <param name="postUpdateCallback"></param>
		public Loop(EliotCondition<bool> conditionBool, bool parameterValue, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Condition")
		{
			if(conditionBool != null) this.conditionBool += conditionBool;
			this.parameterValue = parameterValue;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.OneParameterBool;
		}
		
        /// <summary>
        /// Build loop that takes a single integer parameter.
        /// </summary>
        /// <param name="conditionInt"></param>
        /// <param name="parameterValue"></param>
        /// <param name="preUpdateCallback"></param>
        /// <param name="postUpdateCallback"></param>
		public Loop(EliotCondition<int> conditionInt, int parameterValue, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Condition")
		{
			if(conditionInt != null) this.conditionInt += conditionInt;
			this.parameterValue = parameterValue;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.OneParameterInt;
		}
		
        /// <summary>
        /// Build loop that takes a single float parameter.
        /// </summary>
        /// <param name="conditionFloat"></param>
        /// <param name="parameterValue"></param>
        /// <param name="preUpdateCallback"></param>
        /// <param name="postUpdateCallback"></param>
		public Loop(EliotCondition<float> conditionFloat, float parameterValue, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Condition")
		{
			if(conditionFloat != null) this.conditionFloat += conditionFloat;
			this.parameterValue = parameterValue;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.OneParameterFloat;
		}
		
        /// <summary>
        /// Build loop that takes a single string parameter.
        /// </summary>
        /// <param name="conditionString"></param>
        /// <param name="parameterValue"></param>
        /// <param name="preUpdateCallback"></param>
        /// <param name="postUpdateCallback"></param>
		public Loop(EliotCondition<string> conditionString, string parameterValue, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Condition")
		{
			if(conditionString != null) this.conditionString += conditionString;
			this.parameterValue = parameterValue;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.OneParameterString;
		}
		
        /// <summary>
        /// Build loop that takes any number of parameters.
        /// </summary>
        /// <param name="methodData"></param>
        /// <param name="gameObject"></param>
        /// <param name="preUpdateCallback"></param>
        /// <param name="postUpdateCallback"></param>
		public Loop(MethodData methodData, GameObject gameObject, System.Action preUpdateCallback=null, System.Action postUpdateCallback=null) : base("Action")
		{
			this.methodData = methodData;
			this.gameObject = gameObject;
			if(preUpdateCallback != null) PreUpdateCallback = preUpdateCallback;
			if(postUpdateCallback != null) PostUpdateCallback = postUpdateCallback;
			this.activationMode = ActivationModes.Other;
		}

        /// <summary>
        /// Make Loop do its job.
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
            catch (Exception e)
            {
                Status = CoreComponentStatus.Error;
                Debug.LogError(e, gameObject);
            }

            switch (conditionMet)
            {
                case true:
                {
                    Core.SetActiveComponent(this);
                    if (transitionsWhile.Count == 0) return;
                    foreach(var transition in transitionsWhile)
                        if (!transition.Update()) break;
                    RelaxEnd();
                    break;
                }
                case false:
                {
                    Core.Reset();
                    if (transitionsEnd.Count == 0) return;
                    foreach(var transition in transitionsEnd)
                        if (!transition.Update()) break;
                    RelaxWhile();
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

            RelaxWhile();

            RelaxEnd();
        }

        /// <summary>
        /// Stop the execution of the 'while' branch.
        /// </summary>
        private void RelaxWhile()
        {
            if (transitionsWhile.Count > 0)
            {
                foreach (var transition in transitionsWhile)
                    transition.Relax();
            }
        }
        
        /// <summary>
        /// Stop the execution of the 'end' branch.
        /// </summary>
        private void RelaxEnd()
        {
            if (transitionsEnd.Count > 0)
            {
                foreach (var transition in transitionsEnd)
                    transition.Relax();
            }
        }
        
        /// <summary>
        /// Stop the execution of the current node within a single iteration to prevent recursion.
        /// </summary>
        public override void RelaxSingleIteration()
        {
            SingleIterationActive = false;

            RelaxSingleIterationWhile();

            RelaxSingleIterationEnd();
        }
        
        /// <summary>
        /// Stop the execution of the 'while' branch a single iteration to prevent recursion.
        /// </summary>
        private void RelaxSingleIterationWhile()
        {
            if (transitionsWhile.Count > 0)
            {
                foreach (var transition in transitionsWhile)
                    transition.RelaxSingleIteration();
            }
        }
        
        /// <summary>
        /// Stop the execution of the 'end' branch a single iteration to prevent recursion.
        /// </summary>
        private void RelaxSingleIterationEnd()
        {
            if (transitionsEnd.Count > 0)
            {
                foreach (var transition in transitionsEnd)
                    transition.RelaxSingleIteration();
            }
        }

        /// <summary>
        /// Create transition in 'While' group between the Loop and other component.
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
            transitionsWhile.Add(transition);
            return transition;
        }
        
        /// <summary>
        /// Create transition in 'End' group between the Loop and other component.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="minProbability"></param>
        /// <param name="maxProbability"></param>
        /// <param name="minCooldown"></param>
        /// <param name="maxCooldown"></param>
        /// <param name="terminate"></param>
        /// <returns></returns>
        public BehaviourEngine.Transition ConnectWith_End(CoreComponent start, CoreComponent end, float minProbability, float maxProbability,
            float minCooldown, float maxCooldown, bool terminate)
        {
            var transition = new Transition(start, end, minProbability, maxProbability, minCooldown, maxCooldown, terminate);
            transitionsEnd.Add(transition);
            return transition;
        }
    }
}
