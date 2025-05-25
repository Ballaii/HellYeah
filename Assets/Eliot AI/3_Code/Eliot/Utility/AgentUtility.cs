using System;
using Eliot.AgentComponents;
using UnityEngine;

namespace Eliot.Utility
{
    public static class AgentUtility
    {
        /// <summary>
        /// Look for specific child of Agent's transform by name.
        /// Create such if not found.
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="objName"></param>
        /// <returns></returns>
        public static Transform FindTransformByName(this EliotAgent agent, string objName)
        {
            if (!agent) return null;
            var child = agent.transform.FindDeepChild(objName);
            if (child == null)
            {
                var newChild = new GameObject(objName);
                newChild.transform.parent = agent.transform;
                newChild.transform.localPosition = new Vector3(0, 1, 0);
                child = newChild.transform;
            }

            return child;
        }

        /// <summary>
        /// Get the Transform Component of the Agent's graphics holder.
        /// </summary>
        public static Transform GetGraphicsContainer(this EliotAgent agent)
        {
            return agent.FindTransformByName("__graphics__");
        }

        /// <summary>
        /// Get the Transform Component of the Agent's perception origin.
        /// </summary>
        public static Transform GetPerceptionOrigin(this EliotAgent agent)
        {
            return agent.FindTransformByName("__look__");
        }

        /// <summary>
        /// Get the Transform Component of the Agent's skill cast origin.
        /// </summary>
        public static Transform GetSkillOrigin(this EliotAgent agent)
        {
            return agent.FindTransformByName("__shoot__");
        }

        /// <summary>
        /// Get the Agent's AudioSource Component or add one if none is present.
        /// </summary>
        /// <returns></returns>
        public static AudioSource GetAudioSource(this EliotAgent agent)
        {
            return agent.GetComponent<AudioSource>() ? agent.GetComponent<AudioSource>() : agent.gameObject.AddComponent<AudioSource>();
        }

        /// <summary>
        /// Add an AgentComponent to anAgent and initialize it.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddAgentComponent<T>(this GameObject gameObject) where T : AgentComponent
        {
            var agentComponent = gameObject.AddComponent<T>();
            agentComponent.OnAddComponent();
            agentComponent.Init(gameObject.GetComponent<EliotAgent>());
        }
        
        /// <summary>
        /// Add an AgentComponent to anAgent and initialize it.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="agentComponentType"></param>
        public static void AddAgentComponent(this GameObject gameObject, Type agentComponentType) 
        {
            var agentComponent = gameObject.AddComponent(agentComponentType);
            (agentComponent as AgentComponent).OnAddComponent();
            (agentComponent as AgentComponent).Init(gameObject.GetComponent<EliotAgent>());
        }
    }
}
