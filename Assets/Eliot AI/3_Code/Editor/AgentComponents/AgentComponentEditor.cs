using System;
using UnityEditor;

namespace Eliot.AgentComponents.Editor
{
    /// <summary>
    /// Base Editor class for all the Agent Components.
    /// </summary>
    [Serializable] public class AgentComponentEditor 
    {
        /// <summary>
        /// Public link to the agent.
        /// </summary>
        public EliotAgent Agent
        {
            get { return _agent; }
            set { _agent = value; }
        }

        /// <summary>
        /// The target object whose properties are drawn and edited.
        /// </summary>
        public SerializedObject serializedObject;

        /// <summary>
        /// Link to the agent.
        /// </summary>
        protected EliotAgent _agent;

        /// <summary>
        /// Constructor that initializes the serializable object.
        /// </summary>
        /// <param name="agentComponent"></param>
        public AgentComponentEditor(AgentComponent agentComponent)
        {
            serializedObject = new SerializedObject(agentComponent);
        }
        
        /// <summary>
        /// Initialize the variables and components.
        /// </summary>
        public virtual void OnEnable(){}
        
        /// <summary>
        /// Draw the default inspector with the blue label.
        /// </summary>
        public virtual void DrawInspector(AgentComponent agentComponent)
        {
            if(serializedObject==null)
                serializedObject = new SerializedObject(agentComponent);
            serializedObject.Update();
            OnEnable();
        }

        /// <summary>
        /// Handles drawing helper gizmos and handles in the scene.
        /// </summary>
        /// <param name="agentComponent"></param>
        public virtual void DrawSceneGUI(AgentComponent agentComponent)
        {
            if(serializedObject==null)
                serializedObject = new SerializedObject(agentComponent);
        }
    }
}