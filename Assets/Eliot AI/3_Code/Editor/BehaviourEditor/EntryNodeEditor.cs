using Eliot.Utility;
using UnityEditor;

namespace Eliot.BehaviourEditor.Editor
{
    /// <summary>
    /// Editor script for Entry nodes.
    /// </summary>
    [CustomEditor(typeof(EntryNode))]
    public class EntryNodeEditor : UnityEditor.Editor
    {
        /// <summary>
        /// Draw the Inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EliotEditorUtility.DrawMiniLabel("Node: Entry", EliotGUISkin.GrayBackground);
            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.HelpBox("The Behaviour starts here", MessageType.Info);
            EditorGUILayout.EndVertical();
        }
    }
}