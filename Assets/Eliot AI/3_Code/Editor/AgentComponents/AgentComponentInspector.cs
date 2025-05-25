using System;
using UnityEditor;
using UnityEngine;

namespace Eliot.AgentComponents.Editor
{
    [CustomEditor(typeof(AgentComponent), true)]
    [CanEditMultipleObjects]
    public class AgentComponentInspector : UnityEditor.Editor
    {
        public void OnEnable()
        {
            hideFlags = HideFlags.HideInInspector;
        }

        public override void OnInspectorGUI()
        {
        }
    }
}