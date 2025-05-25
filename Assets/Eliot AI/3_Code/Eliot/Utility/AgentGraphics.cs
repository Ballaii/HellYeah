using UnityEngine;
using Animation = UnityEngine.Animation;

namespace Eliot.Utility
{
    /// <summary>
    /// This class keeps the necessary references for Agent to ret up new graphics properly.
    /// </summary>
    public class AgentGraphics : MonoBehaviour
    {
        [Tooltip("Object with an Animation component on a new graphics.")]
        public Animation Animation;
        [Tooltip("Object with an Animator component on a new graphics.")]
        public Animator Animator;
        [Space]
        [Tooltip("Agent's __shoot__ will copy position and rotation from this one.")]
        public Transform NewShooterPosition;
        [Tooltip("Agent's __look__ will copy position and rotation from this one.")]
        public Transform NewPerceptionOriginPosition;
        [Space]
        [Tooltip("Whether or not to invoke any method on Agent when changing graphics.")]
        public bool SendMessageOnChange;
        [Tooltip("Name of the method to invoke.")]
        public string MethodName;
        [Tooltip("String parameter of the method. Can be left empty. " +
                 "Method will be invoked with no parameters in this case.")]
        public string MethodParams;
    }
}

