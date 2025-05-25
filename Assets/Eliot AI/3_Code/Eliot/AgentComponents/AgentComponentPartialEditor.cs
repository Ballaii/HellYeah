#if UNITY_EDITOR
namespace Eliot.AgentComponents
{
    /// <summary>
    /// Holds the editor related variables for an Agent component.
    /// </summary>
    public partial class AgentComponent
    {
        /// <summary>
        /// Whether to display the component's properties in the Inspector.
        /// </summary>
        public bool displayEditor
        {
            get
            { return _displayEditor;
            }
            set
            {
                _displayEditor = value;
            }
        }

        private bool _displayEditor;
    }
}
#endif