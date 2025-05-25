using Eliot.AgentComponents;
using Eliot.Utility;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Eliot.BehaviourEditor.Editor
{
    /// <summary>
    /// Editor script for Utility Node.
    /// </summary>
    [CustomEditor(typeof(UtilityNode))]
    public class UtilityNodeEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Used to identify and access the Node via code.
        /// </summary>
        private SerializedProperty _nodeId;
        
        /// <summary>
        /// Reference to the node.
        /// </summary>
        private UtilityNode _utilityNode;
        
        /// <summary>
        /// Reorderable list of curves.
        /// </summary>
        private ReorderableList reorderableListCurves;
        
        /// <summary>
        /// Whether the node captures control.
        /// </summary>
        private SerializedProperty _captureControl;

        /// <summary>
        /// The currently selected curve.
        /// </summary>
        public UtilityCurve selectedCurve;
        
        /// <summary>
        /// Initialize the list.
        /// </summary>
        private void InitReorderableListSkills()
        {
            reorderableListCurves = new ReorderableList(_utilityNode.Curves, 
                typeof(UtilityNode), 
                false, true, true, true);
            
            reorderableListCurves.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Utility Curves");
            };
				
            reorderableListCurves.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (reorderableListCurves.list == null)
                {
                    reorderableListCurves.list = _utilityNode.Curves;
                }

                if (reorderableListCurves.list.Count == 0)
                {
                    reorderableListCurves.list.Add(new UtilityCurve());
                }

                if (reorderableListCurves.list.Count <= index)
                {
                    reorderableListCurves.list.Add(new UtilityCurve());
                }
                var curve = (UtilityCurve)(reorderableListCurves.list[index]);
                var nameRect = new Rect(rect.x + rect.width/10, rect.y, rect.width*0.9f, rect.height);
                curve.Name = EditorGUI.TextField(nameRect, curve.Name);

                serializedObject.ApplyModifiedProperties();
                
            };
            reorderableListCurves.onSelectCallback = (list) => {
                selectedCurve = (UtilityCurve)(reorderableListCurves.list[list.index]);
            };

        }
        
        /// <summary>
        /// This function is called when the object is loaded.
        /// </summary>
        private void OnEnable()
        {
            if(target==null) return;
            _nodeId = serializedObject.FindProperty("NodeId");
            _captureControl = serializedObject.FindProperty("CaptureControl");
            _utilityNode = (UtilityNode) target;
            if (selectedCurve != null)
            {
                if (selectedCurve.methodInfo == null)
                {
                    selectedCurve.methodInfo = selectedCurve.MethodData.BuildMethodInfo();
                }
                var indexCalculationResult =
                    EliotReflectionUtility.GetIndexFromMethodInfo<ActionInterface>(selectedCurve.methodInfo);
                if (indexCalculationResult[0] != -1)
                {
                    selectedCurve.ActionGroupIndex = indexCalculationResult[0];
                    if (indexCalculationResult[1] != -1)
                        selectedCurve.FuncIndex = indexCalculationResult[1];
                }
            }
            
            serializedObject.ApplyModifiedProperties();

            InitReorderableListSkills();
        }

        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EliotEditorUtility.DrawMiniLabel("Node: Utility", EliotGUISkin.GrayBackground);

            serializedObject.Update();
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_nodeId, new GUIContent("Node Id", "Used to identify and access the Node via code"));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_captureControl);
            EditorGUILayout.EndVertical(); 
            
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            reorderableListCurves.DoLayoutList();
            EditorGUILayout.EndVertical();

            if (selectedCurve != null)
            {
                EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
                EditorGUILayout.LabelField(selectedCurve.Name, EditorStyles.whiteMiniLabel);
                EditorGUILayout.EndVertical(); 
                
                selectedCurve.DrawInspector();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}