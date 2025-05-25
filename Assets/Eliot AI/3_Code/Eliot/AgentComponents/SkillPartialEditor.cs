#if UNITY_EDITOR
using UnityEngine;

namespace Eliot.AgentComponents
{
    public partial class Skill
    {
        [SerializeField] private bool editorAdvancedMode = false;
    }
}
#endif