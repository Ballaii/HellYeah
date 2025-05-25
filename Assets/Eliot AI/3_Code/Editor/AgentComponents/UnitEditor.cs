using System;
using System.Collections.Generic;
using Eliot.Environment;
using Eliot.Utility;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eliot.AgentComponents.Editor
{
    /// <summary>
    /// Custom editor for Unit objects.
    /// </summary>
    [Serializable]
    [CustomEditor(typeof(Unit))]
    [CanEditMultipleObjects]
    public class UnitEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Serialized Properties to draw and other variables.
        /// </summary>
        #region Variables

        private Unit _unit;

        private ReorderableList _attributesGroups;

        #region SerializableProperties

        private SerializedProperty _type;
        private SerializedProperty _team;
        private SerializedProperty _friendlyTeams;
#if PIXELCRUSHERS_LOVEHATE
        private SerializedProperty _useLoveHate;
#endif
        private SerializedProperty _visualizationType;
        private SerializedProperty _gizmosColor;
        private SerializedProperty _visualizationBoxSize;
        private SerializedProperty _visualizationSphereRadius;

        #endregion

        [SerializeField] private bool unfoldAttributes = false;

        private List<AttributesGroupScriptable> containedListSources = new List<AttributesGroupScriptable>();

        #endregion

        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        private void OnEnable()
        {
            _unit = target as Unit;

            _type = serializedObject.FindProperty("_type");
            _team = serializedObject.FindProperty("_team");
            _friendlyTeams = serializedObject.FindProperty("_friendlyTeams");
#if PIXELCRUSHERS_LOVEHATE
            _useLoveHate = serializedObject.FindProperty("_useLoveHate");
#endif
            
            _visualizationType = serializedObject.FindProperty("visualizationType");
            _gizmosColor = serializedObject.FindProperty("gizmosColor");
            _visualizationBoxSize = serializedObject.FindProperty("visualizationBoxSize");
            _visualizationSphereRadius = serializedObject.FindProperty("visualizationSphereRadius");

            _attributesGroups = new ReorderableList(_unit.attributesGroups, typeof(AttributesGroup),
                true, true, true, true);
            _attributesGroups.drawHeaderCallback = rect => { EditorGUI.LabelField(rect, "Attributes Groups"); };
            _attributesGroups.drawElementCallback = (rect, index, active, focused) =>
            {
                var curItem = _attributesGroups.list[index] as AttributesGroup;
                if (curItem == null) return;
                curItem.cache = new Dictionary<string, Attribute>();
                curItem.source = (AttributesGroupScriptable) EditorGUI.ObjectField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "Source " + (index + 1).ToString(), curItem.source, typeof(AttributesGroupScriptable), false);
                containedListSources.Add(curItem.source);

                if (curItem.source && curItem.source.attributes != null && curItem.source.attributes.Count > 0)
                {
                    curItem.displayAttributes = EditorGUI.Foldout(new Rect(rect.x,
                        rect.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing),
                        rect.width,
                        EditorGUIUtility.singleLineHeight), curItem.displayAttributes, "Attributes");
                    curItem.MatchSource();
                    if (curItem.displayAttributes)
                    {
                        var sourceAttributes = curItem.source.attributes;
                        if (sourceAttributes != null && sourceAttributes.Count > 0)
                        {
                            for (int i = 0; i < sourceAttributes.Count; i++)
                            {
                                EditorGUI.LabelField(
                                    new Rect(rect.x + 15,
                                        rect.y +
                                        (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) *
                                        (i + 2), rect.width / 2f, EditorGUIUtility.singleLineHeight),
                                    sourceAttributes[i].name);
                                var curAttribute = curItem.attributes[i];
                                switch (curAttribute.type)
                                {

                                    case AttributeType.Bool:
                                    {
                                        curAttribute.boolValue = EditorGUI.Toggle(new Rect(rect.x + rect.width / 2,
                                                rect.y + (EditorGUIUtility.singleLineHeight +
                                                          EditorGUIUtility.standardVerticalSpacing) * (i + 2),
                                                rect.width / 3, EditorGUIUtility.singleLineHeight),
                                            curItem.attributes[i].boolValue);
                                        break;
                                    }

                                    case AttributeType.Int:
                                    {
                                        curAttribute.intValue = EditorGUI.IntField(new Rect(rect.x + rect.width / 2,
                                                rect.y + (EditorGUIUtility.singleLineHeight +
                                                          EditorGUIUtility.standardVerticalSpacing) * (i + 2),
                                                rect.width / 3, EditorGUIUtility.singleLineHeight),
                                            curItem.attributes[i].intValue);
                                        break;
                                    }

                                    case AttributeType.Float:
                                    {
                                        curAttribute.floatValue = EditorGUI.FloatField(new Rect(rect.x + rect.width / 2,
                                                rect.y + (EditorGUIUtility.singleLineHeight +
                                                          EditorGUIUtility.standardVerticalSpacing) * (i + 2),
                                                rect.width / 3, EditorGUIUtility.singleLineHeight),
                                            curItem.attributes[i].floatValue);
                                        break;
                                    }

                                    case AttributeType.String:
                                    {
                                        curAttribute.stringValue = EditorGUI.TextField(new Rect(rect.x + rect.width / 2,
                                                rect.y + (EditorGUIUtility.singleLineHeight +
                                                          EditorGUIUtility.standardVerticalSpacing) * (i + 2),
                                                rect.width / 3, EditorGUIUtility.singleLineHeight),
                                            curItem.attributes[i].stringValue);
                                        break;
                                    }

                                    case AttributeType.Object:
                                    {
                                        curAttribute.objectValue = EditorGUI.ObjectField(new Rect(
                                                rect.x + rect.width / 2,
                                                rect.y + (EditorGUIUtility.singleLineHeight +
                                                          EditorGUIUtility.standardVerticalSpacing) * (i + 2),
                                                rect.width / 3, EditorGUIUtility.singleLineHeight),
                                            curItem.attributes[i].objectValue, typeof(UnityEngine.Object), true);
                                        break;
                                    }
                                }

                                if (GUI.Button(new Rect(rect.x + rect.width - rect.width / 8,
                                    rect.y +
                                    (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) *
                                    (i + 2),
                                    rect.width / 8, EditorGUIUtility.singleLineHeight), "reset"))
                                {
                                    curAttribute = sourceAttributes[i];
                                }

                                curItem.attributes[i] = curAttribute;
                            }

                            if (GUI.Button(new Rect(rect.x + rect.width - rect.width / 6,
                                rect.y +
                                (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) *
                                (curItem.source.attributes.Count + 2),
                                rect.width / 6, EditorGUIUtility.singleLineHeight), "reset all"))
                            {
                                curItem.attributes = new List<Attribute>();
                                for (int i = 0; i < curItem.source.attributes.Count; i++)
                                {
                                    curItem.attributes.Add(curItem.source.attributes[i].Clone());
                                }
                            }
                        }
                    }
                }
            };
            _attributesGroups.elementHeightCallback = index =>
            {
                var curItem = _attributesGroups.list[index] as AttributesGroup;
                if (curItem == null || curItem.source == null || curItem.source.attributes == null ||
                    curItem.source.attributes.Count == 0)
                    return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                if (!curItem.displayAttributes)
                    return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
                var count = curItem.source.attributes.Count + 3;
                return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * count;
            };
        }

        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EliotEditorUtility.DrawHeader<Unit>("Unit", target, EliotGUISkin.UnitPurple);

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_type);
            EditorGUILayout.PropertyField(_team);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_friendlyTeams, true);
#if PIXELCRUSHERS_LOVEHATE
        EditorGUILayout.PropertyField(_useLoveHate);
#endif
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            unfoldAttributes = EditorGUILayout.Foldout(unfoldAttributes, "Attributes");
            if (unfoldAttributes)
            {

                _attributesGroups.DoLayoutList();

                containedListSources = new List<AttributesGroupScriptable>();
                for (int i = _unit.attributesGroups.Count - 1; i >= 0; i--)
                {
                    if (containedListSources.Contains(_unit.attributesGroups[i].source))
                    {
                        Debug.LogWarning("Unit " + _unit.gameObject.name + " already contains the '" +
                                         _unit.attributesGroups[i].source.name + "' Attributes Group.");
                        _unit.attributesGroups.RemoveAt(i);
                    }
                    else
                    {
                        containedListSources.Add(_unit.attributesGroups[i].source);
                    }
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.LabelField("Visualization", EditorStyles.miniBoldLabel);
            EditorGUILayout.PropertyField(_visualizationType);
            if (_unit.visualizationType == Unit.UnitVisualizationType.Box)
            {
                EditorGUILayout.PropertyField(_gizmosColor);
                EditorGUILayout.PropertyField(_visualizationBoxSize, new GUIContent("Box Size"));
            }
            else if (_unit.visualizationType == Unit.UnitVisualizationType.Sphere)
            {
                EditorGUILayout.PropertyField(_gizmosColor);
                EditorGUILayout.PropertyField(_visualizationSphereRadius, new GUIContent("Sphere Radius"));
            }
            else if (_unit.visualizationType == Unit.UnitVisualizationType.FromCollider)
            {
                EditorGUILayout.PropertyField(_gizmosColor);
            }
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_unit);
                if (!Application.isPlaying)
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

        }
    }
}