#if UNITY_EDITOR
using Eliot.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Eliot.AgentComponents.Editor
{
    /// <summary>
    /// Editor script for the scriptable resources groups templates.
    /// </summary>
    [CustomEditor(typeof(ResourcesProfileScriptable))]
    public class ResourcesProfileScriptableEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Beautiful list. 
        /// </summary>
        [SerializeField] private ReorderableList resources;
        
        /// <summary>
        /// Link to the scriptable target.
        /// </summary>
        private ResourcesProfileScriptable _resourcesProfileScriptable;

        /// <summary>
        /// Calculate the vertical offset as the GUI is drawn.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        private float Offset(ref float offset)
        {
            offset += EditorGUIUtility.singleLineHeight;
            offset += EditorGUIUtility.standardVerticalSpacing;
            return offset;
        }

        /// <summary>
        /// Initialize the beautiful list.
        /// </summary>
        private void InitReorderableListResources()
        {
            resources = new ReorderableList(_resourcesProfileScriptable.resources, typeof(ResourceScriptable), true, true, true,
                true);

            resources.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Resources"); };

            resources.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                float offset = 0;
                _resourcesProfileScriptable.resources[index].name = EditorGUI.TextField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Name",
                    _resourcesProfileScriptable.resources[index].name);
                _resourcesProfileScriptable.resources[index].minValue = EditorGUI.IntField(
                    new Rect(rect.x, rect.y + Offset(ref offset), rect.width, EditorGUIUtility.singleLineHeight), "Min",
                    _resourcesProfileScriptable.resources[index].minValue);
                _resourcesProfileScriptable.resources[index].maxValue = EditorGUI.IntField(
                    new Rect(rect.x, rect.y + Offset(ref offset), rect.width, EditorGUIUtility.singleLineHeight), "Max",
                    _resourcesProfileScriptable.resources[index].maxValue);
                _resourcesProfileScriptable.resources[index].initialValue =
                    EditorGUI.IntSlider(
                        new Rect(rect.x, rect.y + Offset(ref offset), rect.width, EditorGUIUtility.singleLineHeight),
                        "Initial Value",
                        _resourcesProfileScriptable.resources[index].initialValue,
                        _resourcesProfileScriptable.resources[index].minValue,
                        _resourcesProfileScriptable.resources[index].maxValue);
                _resourcesProfileScriptable.resources[index].replenishAmount = EditorGUI.IntField(
                    new Rect(rect.x, rect.y + Offset(ref offset), rect.width, EditorGUIUtility.singleLineHeight),
                    "Replenish Amount", _resourcesProfileScriptable.resources[index].replenishAmount
                );
                _resourcesProfileScriptable.resources[index].replenishCooldown = EditorGUI.FloatField(
                    new Rect(rect.x, rect.y + Offset(ref offset), rect.width, EditorGUIUtility.singleLineHeight),
                    "Replenish Cooldown", _resourcesProfileScriptable.resources[index].replenishCooldown
                );
            };

            resources.elementHeight = 120f;

            resources.onAddCallback = list => _resourcesProfileScriptable.AddNewResource();
            resources.onRemoveCallback = list => { _resourcesProfileScriptable.RemoveResource(list.index); };

        }

        /// <summary>
        /// Execute on selecting hte object.
        /// </summary>
        public void OnEnable()
        {
            _resourcesProfileScriptable = target as ResourcesProfileScriptable;

            InitReorderableListResources();
        }

        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EliotEditorUtility.DrawMiniLabel("Eliot Scriptable: Resources Profile", EliotGUISkin.GrayBackground);

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            resources.DoLayoutList();
            EditorGUILayout.EndVertical();

            if (Event.current.control && Event.current.keyCode == KeyCode.Z)
                _resourcesProfileScriptable.LoadResources();
        }
    }
}
#endif