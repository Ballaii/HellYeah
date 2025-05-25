using UnityEngine;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// Base class for all the Agent Components.
    /// </summary>
    [System.Serializable]
    public partial class AgentComponent : MonoBehaviour
    {
        /// <summary>
        /// Link to the Agent.
        /// </summary>
        public EliotAgent Agent
        {
            get { return _agent; }
            set { _agent = value; }
        }

        /// Link to the Agent.
        protected EliotAgent _agent;
        
        public AgentComponent(EliotAgent agent)
        {
            _agent = agent;
        }

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        public void OnEnable()
        {
            this.hideFlags = HideFlags.HideInInspector;
        }
        
        /// <summary>
        /// This function gets called when the Agent is selected in the Editor.
        /// </summary>
        /// <param name="agent"></param>
        public virtual void AgentOnEnable(EliotAgent agent)
        {
        }

        /// <summary>
        /// Initialize the component on Start.
        /// </summary>
        /// <param name="agent"></param>
        public virtual void Init(EliotAgent agent)
        {
            _agent = agent;
        }
        
        /// <summary>
        /// Reset the component to the initial state.
        /// </summary>
        public virtual void AgentReset(){}

        /// <summary>
        /// Called by Agent on Update.
        /// </summary>
        public virtual void AgentUpdate(){}

        /// <summary>
        /// Called by Agent on FixedUpdate.
        /// </summary>
        public virtual void AgentFixedUpdate(){}

        /// <summary>
        /// This function is called when the component is added to a GameObject.
        /// </summary>
        public virtual void OnAddComponent(){}
    }
}