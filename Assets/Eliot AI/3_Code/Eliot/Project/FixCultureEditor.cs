using System.Globalization;
using UnityEditor;
using System.Threading;
using UnityEngine;

#if UNITY_EDITOR
/// <summary>
/// Prevent the 'black transitions' bug in the editor.
/// psych77 figured this out.
/// </summary>
[InitializeOnLoad]
public static class FixCultureEditor
{
    static FixCultureEditor()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }
}
#endif

/// <summary>
/// Prevent the 'black transitions' bug at runtime.
/// psych77 figured this out.
/// </summary>
public static class FixCultureRuntime
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void FixCulture()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }
}