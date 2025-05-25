#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;

namespace Eliot
{
    /// <summary>
    /// Responsible for configuring the project so that it is ready to seamlessly work
    /// together with integrated assets if they are installed. Or to work as if no
    /// additional code is there.
    /// </summary>
    [InitializeOnLoad] public class IntegrationManager
    {
        /// <summary>
        /// Do this on project recompilation
        /// </summary>
        static IntegrationManager()
        {
            SetupPackage("Eliot", null, "ELIOT");
            SetupPackage("Pathfinding", null, "ASTAR_EXISTS");
            SetupPackage("PixelCrushers.LoveHate", null, "PIXELCRUSHERS_LOVEHATE");
        }

        /// <summary>
        /// Get project ready for a third-party package
        /// </summary>
       private static void SetupPackage(string @namespace, string[] @classes, string directiveName)
        {
            // Search for Pathfinding namespace in the project
            var matchDetected = false;
            
            if (!string.IsNullOrEmpty(@namespace))
            {
                var namespaceFound = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.Namespace == @namespace
                    select type).Any();
                matchDetected = namespaceFound;
            }

            if (@classes != null && @classes.Length > 0)
            {
                var classesLen = @classes.Length;
                var found = 0;
                for (var i = 0; i < @classes.Length; i++)
                {
                    var classFound = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        from type in assembly.GetTypes()
                        where type.Name == @classes[i]
                        select type).Any();
                    if (classFound)
                        found++;
                }

                matchDetected = found == classesLen;
            }

            // If found, get build target platform
            var buildTargetGroup =  BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            
            // Check if we already added needed #define preprocessor directive
            var existingSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            if (existingSymbols.Contains(directiveName))
            {
                if (!matchDetected)
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,
                        existingSymbols.Replace(directiveName, ""));
                    return;
                }
                return;
            }

            if (matchDetected)
            {
                // If not, add it now
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup,
                    existingSymbols + (";" + directiveName));
            }
        }
    }
}
#endif