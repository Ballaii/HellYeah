using System.Collections.Generic;
using Eliot.BehaviourEditor;
using UnityEngine;

namespace Eliot.BehaviourEngine
{
    /// <summary>
    /// Holds the information necessary to evaluate a utility function.
    /// </summary>
    public class UtilityCurveCore
    {
        /// <summary>
        /// Keep track of which parameters to use on execution.
        /// </summary>
        public ActivationModes ActivationMode;

        public EliotUtilityValue UtilityValue;
        public EliotUtilityValue<bool> UtilityValueBool;
        public EliotUtilityValue<int> UtilityValueInt;
        public EliotUtilityValue<float> UtilityValueFloat;
        public EliotUtilityValue<string> UtilityValueString;
        public object ParameterValue;
        public MethodData MethodData;
        public GameObject GameObject;

        /// <summary>
        /// The curve that represents a function.
        /// </summary>
        public AnimationCurve Curve;

        /// <summary>
        /// List of transitions that link this component to the other ones in the model.
        /// </summary>
        public List<Transition> Transitions = new List<Transition>();

        /// <summary>
        /// Build a utility curve.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="utilityValue"></param>
        public UtilityCurveCore(AnimationCurve curve, EliotUtilityValue utilityValue)
        {
            this.Curve = curve;
            this.Transitions = new List<Transition>();
            this.UtilityValue = utilityValue;
            ActivationMode = ActivationModes.NoParameters;
        }

        /// <summary>
        /// Build a utility curve that takes in a single boolean parameter.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="utilityValueBool"></param>
        /// <param name="parameterValue"></param>
        public UtilityCurveCore(AnimationCurve curve, EliotUtilityValue<bool> utilityValueBool, bool parameterValue)
        {
            this.Curve = curve;
            this.Transitions = new List<Transition>();
            this.UtilityValueBool = utilityValueBool;
            this.ParameterValue = parameterValue;
            ActivationMode = ActivationModes.OneParameterBool;
        }

        /// <summary>
        /// Build a utility curve that takes in a single integer parameter.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="utilityValueInt"></param>
        /// <param name="parameterValue"></param>
        public UtilityCurveCore(AnimationCurve curve, EliotUtilityValue<int> utilityValueInt, int parameterValue)
        {
            this.Curve = curve;
            this.Transitions = new List<Transition>();
            this.UtilityValueInt = utilityValueInt;
            this.ParameterValue = parameterValue;
            ActivationMode = ActivationModes.OneParameterInt;
        }

        /// <summary>
        /// Build a utility curve that takes in a single float parameter.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="utilityValueFloat"></param>
        /// <param name="parameterValue"></param>
        public UtilityCurveCore(AnimationCurve curve, EliotUtilityValue<float> utilityValueFloat, float parameterValue)
        {
            this.Curve = curve;
            this.Transitions = new List<Transition>();
            this.UtilityValueFloat = utilityValueFloat;
            this.ParameterValue = parameterValue;
            ActivationMode = ActivationModes.OneParameterFloat;
        }

        /// <summary>
        /// Build a utility curve that takes in a single string parameter.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="utilityValueString"></param>
        /// <param name="parameterValue"></param>
        public UtilityCurveCore(AnimationCurve curve, EliotUtilityValue<string> utilityValueString,
            string parameterValue)
        {
            this.Curve = curve;
            this.Transitions = new List<Transition>();
            this.UtilityValueString = utilityValueString;
            this.ParameterValue = parameterValue;
            ActivationMode = ActivationModes.OneParameterString;
        }

        /// <summary>
        /// Build a utility curve that takes in any number of parameters.
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="methodData"></param>
        /// <param name="gameObject"></param>
        public UtilityCurveCore(AnimationCurve curve, MethodData methodData, GameObject gameObject)
        {
            this.Curve = curve;
            this.Transitions = new List<Transition>();
            this.MethodData = methodData;
            this.GameObject = gameObject;
            ActivationMode = ActivationModes.Other;
        }

        /// <summary>
        /// Evaluate the curve utility at the given value.
        /// </summary>
        /// <returns></returns>
        public float GetUtility()
        {
            float value = 0;
            switch (this.ActivationMode)
            {
                case ActivationModes.NoParameters:
                {
                    value = UtilityValue();
                    break;
                }

                case ActivationModes.OneParameterBool:
                {
                    value = UtilityValueBool((bool) ParameterValue);
                    break;
                }

                case ActivationModes.OneParameterInt:
                {
                    value = UtilityValueInt((int) ParameterValue);
                    break;
                }

                case ActivationModes.OneParameterFloat:
                {
                    value = UtilityValueFloat((float) ParameterValue);
                    break;
                }

                case ActivationModes.OneParameterString:
                {
                    value = UtilityValueString((string) ParameterValue);
                    break;
                }

                case ActivationModes.Other:
                {
                    value = (float) MethodData.Invoke(GameObject);
                    break;
                }
            }

            return Curve.Evaluate(value);
        }
    }
}