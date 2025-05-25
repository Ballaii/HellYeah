#if UNITY_EDITOR
using Eliot.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Eliot.AgentComponents.Editor
{
    /// <summary>
    /// Editor script for the Attributes Group Scriptable objects.
    /// </summary>
    [CustomEditor(typeof(AttributesGroupScriptable))]
    public class AttributesGroupScriptableEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Attributes reorderable list.
        /// </summary>
        [SerializeField] private ReorderableList attributes;
        
        /// <summary>
        /// The target object.
        /// </summary>
        private AttributesGroupScriptable _attributesGroupScriptable;

        /// <summary>
        /// Whether to foldout the advanced settings.
        /// </summary>
        [SerializeField] private bool foldoutAdvanced = false;

        /// <summary>
        /// Initialize the reorderable list of attributes.
        /// </summary>
        private void InitReorderable()
        {
            attributes = new ReorderableList(_attributesGroupScriptable.attributes,
                typeof(Eliot.AgentComponents.Attribute),
                true, true, true, true);
            attributes.drawElementCallback = (rect, index, active, focused) =>
            {
                var curItem = attributes.list[index] as Eliot.AgentComponents.Attribute;
                if (curItem == null) return;

                curItem.name =
                    EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Name",
                        curItem.name);
                curItem.type = (AttributeType) EditorGUI.EnumPopup(
                    new Rect(rect.x,
                        rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                        rect.width, EditorGUIUtility.singleLineHeight),
                    "Type", curItem.type);
                switch (curItem.type)
                {
                    case AttributeType.Bool:
                    {
                        curItem.boolValue = EditorGUI.Toggle(
                            new Rect(rect.x,
                                rect.y + EditorGUIUtility.singleLineHeight * 2 +
                                EditorGUIUtility.standardVerticalSpacing * 2, rect.width,
                                EditorGUIUtility.singleLineHeight), "Default Value", curItem.boolValue);
                        break;
                    }

                    case AttributeType.Int:
                    {
                        curItem.intValue = EditorGUI.IntField(
                            new Rect(rect.x,
                                rect.y + EditorGUIUtility.singleLineHeight * 2 +
                                EditorGUIUtility.standardVerticalSpacing * 2, rect.width,
                                EditorGUIUtility.singleLineHeight), "Default Value", curItem.intValue);
                        break;
                    }

                    case AttributeType.Float:
                    {
                        curItem.floatValue = EditorGUI.FloatField(
                            new Rect(rect.x,
                                rect.y + EditorGUIUtility.singleLineHeight * 2 +
                                EditorGUIUtility.standardVerticalSpacing * 2, rect.width,
                                EditorGUIUtility.singleLineHeight), "Default Value", curItem.floatValue);
                        break;
                    }

                    case AttributeType.String:
                    {
                        curItem.stringValue = EditorGUI.TextField(
                            new Rect(rect.x,
                                rect.y + EditorGUIUtility.singleLineHeight * 2 +
                                EditorGUIUtility.standardVerticalSpacing * 2, rect.width,
                                EditorGUIUtility.singleLineHeight), "Default Value", curItem.stringValue);
                        break;
                    }

                    case AttributeType.Object:
                    {
                        curItem.objectValue = EditorGUI.ObjectField(
                            new Rect(rect.x,
                                rect.y + EditorGUIUtility.singleLineHeight * 2 +
                                EditorGUIUtility.standardVerticalSpacing * 2, rect.width,
                                EditorGUIUtility.singleLineHeight), "Default Value", curItem.objectValue,
                            typeof(UnityEngine.Object), true);
                        break;
                    }
                }

                attributes.list[index] = curItem;
            };

            attributes.elementHeight =
                EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 3;
            attributes.drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Attributes"); };
        }

        /// <summary>
        /// Execute upon the object being selected.
        /// </summary>
        public void OnEnable()
        {
            _attributesGroupScriptable = target as AttributesGroupScriptable;
            InitReorderable();
        }
        
        /// <summary>
        /// Draw UI in Inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EliotEditorUtility.DrawMiniLabel("Eliot Scriptable: Attributes Group", EliotGUISkin.GrayBackground);

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            attributes.DoLayoutList();
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            foldoutAdvanced = EditorGUILayout.Foldout(foldoutAdvanced, "Advanced");
            if (foldoutAdvanced)
            {
                if (GUILayout.Button("Initialize Default Attributes"))
                {
                    _attributesGroupScriptable.attributes = AttributesGroupScriptable.DefaultAttributes();
                    InitReorderable();
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(_attributesGroupScriptable, "Attributes Group");
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif