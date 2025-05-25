using UnityEngine;

namespace Eliot.Utility
{
    /// <summary>
    /// Help deal with hierarchies of the transforms.
    /// </summary>
    public static class TransformUtility
    {
        /// <summary>
        /// Find child recursively. BFS.
        /// </summary>
        /// <param name="aParent"></param>
        /// <param name="aName"></param>
        /// <returns></returns>
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            var result = aParent.Find(aName);
            if (result != null)
                return result;
            foreach(Transform child in aParent)
            {
                result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}