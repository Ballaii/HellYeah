using UnityEngine;

namespace Eliot.Utility
{
    /// <summary>
    /// Utility class that lets handle zoomable parts in editor windows.
    /// </summary>
    public static class EditorWindowZoomArea
    {
        private const float editorWindowTabHeight = 21.0f;
        private static Matrix4x4 _prevGuiMatrix;
        private static Matrix4x4 _pauseGuiMatrix;

        /// <summary>
        /// Begin the zoomable editor window rect.
        /// </summary>
        /// <param name="zoomScale"></param>
        /// <param name="screenCoordsArea"></param>
        /// <returns></returns>
        public static Rect Begin(float zoomScale, Rect screenCoordsArea)
        {
            GUI.EndGroup();

            Rect clippedArea = screenCoordsArea.ScaleSizeBy(1.0f / zoomScale, screenCoordsArea.TopLeft());
            clippedArea.y += editorWindowTabHeight;
            GUI.BeginGroup(clippedArea);

            _prevGuiMatrix = GUI.matrix;
            Matrix4x4 translation = Matrix4x4.TRS(clippedArea.TopLeft(), Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f));
            GUI.matrix = translation * scale * translation.inverse * GUI.matrix;

            return clippedArea;
        }

        /// <summary>
        /// End the zoomable editor window rect.
        /// </summary>
        public static void End()
        {
            GUI.matrix = _prevGuiMatrix;
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(0.0f, editorWindowTabHeight, Screen.width, Screen.height));
        }

        /// <summary>
        /// Pause the zoomable editor window part to insert some gui elements that aren't supposed to be zoomed.
        /// </summary>
        /// <param name="zoomScale"></param>
        /// <param name="screenCoordsArea"></param>
        public static void Pause(float zoomScale, Rect screenCoordsArea)
        {
            Rect clippedArea = screenCoordsArea.ScaleSizeBy(1.0f / zoomScale, screenCoordsArea.TopLeft());
            GUI.BeginGroup(clippedArea);
            _pauseGuiMatrix = GUI.matrix;
            Matrix4x4 translation = Matrix4x4.TRS(clippedArea.TopLeft(), Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(new Vector3(1f / zoomScale, 1f / zoomScale, 1.0f));
            GUI.matrix = translation * scale * translation.inverse * GUI.matrix;
        }

        /// <summary>
        /// Resume the zoomable editor window part.
        /// </summary>
        public static void Resume()
        {
            GUI.matrix = _pauseGuiMatrix;
            GUI.EndGroup();
        }
    }
}