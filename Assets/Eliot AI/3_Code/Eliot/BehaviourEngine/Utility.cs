using System.Collections.Generic;

namespace Eliot.BehaviourEngine
{
    /// <summary>
    /// A delegate that executes any no-parameters function and returns a float value.
    /// </summary>
    public delegate float EliotUtilityValue();
    
    /// <summary>
    /// A delegate that executes any single-parameter function and returns a float value.
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    public delegate float EliotUtilityValue<in T>(T obj);

    /// <summary>
    /// Evaluates multiple utility functions against each other and runs the most valuable branch.
    /// </summary>
    public class Utility : CoreComponent
    {
        /// <summary>
        /// The list of all the utility functions that need to be evaluated against each other.
        /// </summary>
        public List<UtilityCurveCore> UtilityCurves = new List<UtilityCurveCore>();
        
        /// <summary>
        /// Keeps all the references of curves' transitions.
        /// </summary>
        public List<Transition> Transitions = new List<Transition>();
        
        /// <summary>
        /// Make component do its job.
        /// </summary>
        public override void Update()
        {
            Active = true;
            
            if (Core.ActiveComponent != this && SingleIterationActive) return;
            SingleIterationActive = true;

            if(PreUpdateCallback != null) PreUpdateCallback.Invoke();
            
            if (CaptureControl) Core.SetActiveComponent(this);
            
            UtilityCurveCore curveWithMaxUtility = UtilityCurves[0];
            float maxUtility = curveWithMaxUtility.GetUtility();
            for (int i = 1; i < UtilityCurves.Count; i++)
            {
                var curUtility = UtilityCurves[i].GetUtility();
                if (curUtility > maxUtility)
                {
                    maxUtility = curUtility;
                    curveWithMaxUtility = UtilityCurves[i];
                }
            }

            foreach (var transition in curveWithMaxUtility.Transitions)
            {
                transition.Update();
            }
            for(int i = 0; i < UtilityCurves.Count; i++)
                if(UtilityCurves[i] != curveWithMaxUtility)
                    foreach (var transition in UtilityCurves[i].Transitions)
                        transition.Relax();
            
            if(Core.ActiveComponent == this) RelaxSingleIteration();

            if(PostUpdateCallback != null) PostUpdateCallback.Invoke();
        }

        /// <summary>
        /// Stop the execution of the current node.
        /// </summary>
        public override void Relax()
        {
            if (!Active) return;
            Active = false;
            foreach (var transition in Transitions)
                transition.Relax();
        }
		
        /// <summary>
        /// Stop the execution of the current node within a single iteration to prevent recursion.
        /// </summary>
        public override void RelaxSingleIteration()
        {
            if (!SingleIterationActive) return;
            SingleIterationActive = false;
            foreach (var transition in Transitions)
                transition.RelaxSingleIteration();
        }

        /// <summary>
        /// Create transition between the Utility and other component.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="minProbability"></param>
        /// <param name="maxProbability"></param>
        /// <param name="minCooldown"></param>
        /// <param name="maxCooldown"></param>
        /// <param name="terminate"></param>
        /// <returns></returns>
        public override BehaviourEngine.Transition ConnectWith(CoreComponent start, CoreComponent end, float minProbability, float maxProbability, float minCooldown, float maxCooldown, bool terminate)
        {
            var transition = new Transition(start, end, minProbability, maxProbability, minCooldown, maxCooldown, terminate);
            Transitions.Add(transition);
            return transition;
        }
    }
}