using System.Collections.Generic;

namespace Eliot.AgentComponents.Editor
{
    /// <summary>
    /// maps an agent component to an editor without having link them inside the classes.
    /// </summary>
    public static class AgentComponentsEditorMap
    {
        /// <summary>
        /// Cache for the editors.
        /// </summary>
        public static Dictionary<AgentComponent, AgentComponentEditor> editorMap =
            new Dictionary<AgentComponent, AgentComponentEditor>();

        /// <summary>
        /// Return the proper editor/drawer.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public static AgentComponentEditor GetEditor(AgentComponent component)
        {
            if (editorMap.ContainsKey(component)) return editorMap[component];
            if (component.GetType() == typeof(AgentAnimation))
            {
                var editor = new AgentAnimationEditor((AgentAnimation) component);
                editorMap.Add(component, editor);
                return editor;
            }

            if (component.GetType() == typeof(AgentInventory))
            {
                var editor = new AgentInventoryEditor((AgentInventory) component);
                editorMap.Add(component, editor);
                return editor;
            }

            if (component.GetType() == typeof(AgentResources))
            {
                var editor = new AgentResourcesEditor((AgentResources) component);
                editorMap.Add(component, editor);
                return editor;
            }

            if (component.GetType() == typeof(AgentMotion))
            {
                var editor = new AgentMotionEditor((AgentMotion) component);
                editorMap.Add(component, editor);
                return editor;
            }

            if (component.GetType() == typeof(AgentPerception))
            {
                var editor = new AgentPerceptionEditor((AgentPerception) component);
                editorMap.Add(component, editor);
                return editor;
            }

            if (component.GetType() == typeof(AgentDeathHandler))
            {
                var editor = new AgentDeathHandlerEditor((AgentDeathHandler) component);
                editorMap.Add(component, editor);
                return editor;
            }
            
            if (component.GetType() == typeof(AgentPlayerInput))
            {
                var editor = new AgentPlayerInputEditor((AgentPlayerInput) component);
                editorMap.Add(component, editor);
                return editor;
            }

            return null;
        }
    }
}