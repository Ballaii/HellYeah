#if UNITY_EDITOR
using Eliot.BehaviourEditor;
using Eliot.Repository;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Eliot
{
    /// <summary>
    /// Open behaviour in the editor when it is doubleclicked.
    /// </summary>
    public static class BehaviourOpenHandler
    {
        /// <summary>
        /// Open the editor window and set it's behaviour to the doubleclicked one.
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        [OnOpenAssetAttribute]
        public static bool OpenBehaviorEditor(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (!obj) return false;
            if (obj.GetType() == typeof(EliotBehaviour))
            {
                var behaviour = obj as EliotBehaviour;
                if (!string.IsNullOrEmpty(behaviour.Json))
                {
                    behaviour.Json = BehaviourVersionManager.UpdateJson(behaviour.Json);
                    behaviour.Serialize();
                    behaviour.Json = "";
                }

                var editor = EditorWindow.GetWindow<BehaviourEditorWindow>("Behaviour");

                editor.SetBehaviour(behaviour);
                editor.Reverse(null);
            }

            return false; // could not handle the open
        }
    }
}
#endif