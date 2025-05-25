using Eliot.Utility;
using UnityEditor;
using EditorGUI = UnityEditor.EditorGUI;

namespace Eliot.AgentComponents.Editor
{
    /// <summary>
    /// Editor script for the Agent Inventory component.
    /// </summary>
    public class AgentInventoryEditor : AgentComponentEditor
    {
        /// <summary>
        /// Serialized Properties to draw and other variables.
        /// </summary>
        #region Variables
        private AgentInventory _agentInventory;

        private SerializedProperty _maxWeight;
        private SerializedProperty _items;

        private SerializedProperty _wieldedItem;

        private SerializedProperty _initFromList;
        private SerializedProperty _initFromChildren;

        private SerializedProperty _dropRadius;
        #endregion

        public AgentInventoryEditor(AgentInventory agentInventory) : base(agentInventory)
        {
            _agentInventory = agentInventory;
        }

        /// <summary>
        /// Executed on selecting the object.
        /// </summary>
        public override void OnEnable()
        {
            _maxWeight = serializedObject.FindProperty("_maxWeight");
            _items = serializedObject.FindProperty("_items");
            _wieldedItem = serializedObject.FindProperty("_wieldedItem");

            _initFromList = serializedObject.FindProperty("_initFromList");
            _initFromChildren = serializedObject.FindProperty("_initFromChildren");

            _dropRadius = serializedObject.FindProperty("_dropRadius");
        }

        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public override void DrawInspector(AgentComponent agentInventory)
        {
            base.DrawInspector(agentInventory);
            
            agentInventory.displayEditor = EliotEditorUtility.DrawAgentComponentHeader<AgentInventory>("Eliot Agent Component: " + "Inventory", agentInventory.displayEditor, agentInventory,
                EliotGUISkin.AgentComponentGreen);
            if (!agentInventory.displayEditor) return;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_maxWeight);
            if (_agentInventory.MaxWeight < 0) _agentInventory.MaxWeight = 0;
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_items, true);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_wieldedItem);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_initFromList);
            EditorGUILayout.PropertyField(_initFromChildren);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_dropRadius);
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(agentInventory, "Inventory change");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}