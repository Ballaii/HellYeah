using UnityEngine;

namespace Eliot.Utility
{
    /// <summary>
    /// Extension methods for the UnityEngine Rect struct.
    /// </summary>
    public static class RectUtility
    {
        /// <summary>
        /// Return the top left position of the given rect.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Vector2 TopLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMin);
        }

        /// <summary>
        /// Scale the given rect given new scale and a pivot position.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="scale"></param>
        /// <param name="pivotPoint"></param>
        /// <returns></returns>
        public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
        {
            Rect result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }
    }
}