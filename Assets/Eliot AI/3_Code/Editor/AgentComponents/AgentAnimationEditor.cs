using Eliot.Utility;
using UnityEditor;
using UnityEngine;

namespace Eliot.AgentComponents.Editor
{
    /// <summary>
    /// Editor script for the Agent Animation component.
    /// </summary>
    public class AgentAnimationEditor : AgentComponentEditor
    {
        /// <summary>
        /// Serialized Properties to draw and other variables.
        /// </summary>
        #region Variables
        private AgentAnimation _agentAnimation;

        private SerializedProperty _animationMode;

        private SerializedProperty _animation;
        private SerializedProperty _clips;

        private SerializedProperty _animator;
        private SerializedProperty _rootMotionRotation;
        private SerializedProperty _changeGraphicsName;
        private SerializedProperty _parameters;

        public const string AlignButtonTooltip = "Please note that this does not always work perfectly. If your mesh has weird geometry or if your graphics consists of multiple meshes, you might still need to adjust it further manually.";
        
        #endregion

        public AgentAnimationEditor(AgentAnimation agentAnimation) : base(agentAnimation)
        {
            _agentAnimation = agentAnimation;
        }
        
        /// <summary>
        /// Executed on selecting the object.
        /// </summary>
        public override void OnEnable()
        {
            _animationMode = serializedObject.FindProperty("_animationMode");

            _animation = serializedObject.FindProperty("_animation");
            _clips = serializedObject.FindProperty("_clips");

            _animator = serializedObject.FindProperty("_animator");
            _rootMotionRotation = serializedObject.FindProperty("_rootMotionRotation");
            _changeGraphicsName = serializedObject.FindProperty("_changeGraphicsName");
            _parameters = serializedObject.FindProperty("_parameters");
        }

        /// <summary>
        /// Draw the Inspector GUI.
        /// </summary>
        public override void DrawInspector(AgentComponent agentAnimation)
        {
            base.DrawInspector(agentAnimation);
            
            agentAnimation.displayEditor = EliotEditorUtility.DrawAgentComponentHeader<AgentAnimation>("Eliot Agent Component: " + "Animation", agentAnimation.displayEditor, agentAnimation,
                EliotGUISkin.AgentComponentGreen);
            if (!agentAnimation.displayEditor) return;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            if (GUILayout.Button(new GUIContent("Align Graphics Position", AlignButtonTooltip), EditorStyles.toolbarButton))
            {
                AdjustGraphicsPosition(agentAnimation as AgentAnimation);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            EditorGUILayout.PropertyField(_animationMode);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
            if (_agentAnimation.AnimationMode == AnimationMode.Legacy)
            {
                EditorGUILayout.PropertyField(_animation);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_clips, true);
                EditorGUI.indentLevel--;
            }
            else if (_agentAnimation.AnimationMode == AnimationMode.Mecanim)
            {
                EditorGUILayout.PropertyField(_animator);
                EditorGUILayout.PropertyField(_changeGraphicsName);
                if (_agentAnimation && _agentAnimation.Animator && _agentAnimation.Animator.applyRootMotion)
                {
                    EditorGUILayout.PropertyField(_rootMotionRotation);
                }
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_parameters, true);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(agentAnimation, "Animation change");
                serializedObject.ApplyModifiedProperties();
            }
        }
        
        /// <summary>
        /// Aligns the position of an object in the hierarchy that has Animation or Animator component attached to it.
        /// </summary>
        /// <param name="agentAnimation"></param>
        public static void AdjustGraphicsPosition(AgentAnimation agentAnimation)
        {
            var transform = agentAnimation.transform;
            var renderer = transform.GetComponentInChildren<Renderer>();
            Bounds bounds = renderer.bounds;

            Transform animatedT = null;
            if (agentAnimation.Animator) animatedT = agentAnimation.Animator.transform;
            else if (agentAnimation.LegacyAnimation) animatedT = agentAnimation.LegacyAnimation.transform;
            if (!animatedT) return;

            if (renderer)
            {
                var agentY = transform.position.y;
                var lowestPoint = (bounds.min.y <= bounds.max.y) ? bounds.min.y : bounds.max.y;
				
                float lowestColliderPoint = agentY;
                var capsule = agentAnimation.GetComponent<CapsuleCollider>();
                if (capsule)
                {
                    lowestColliderPoint = agentY + capsule.center.y - capsule.height * 0.5f;
                }
                else
                {
                    var controller = agentAnimation.GetComponent<CharacterController>();
                    if (controller)
                    {
                        lowestColliderPoint = agentY + controller.center.y - controller.height * 0.5f;
                    }
                }
                var distance = lowestColliderPoint - lowestPoint;
                animatedT.position += new Vector3(0, distance, 0);
                animatedT.position = new Vector3(transform.position.x, animatedT.position.y, transform.position.z);
            }
        }
    }
}