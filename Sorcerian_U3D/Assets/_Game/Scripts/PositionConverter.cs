using UnityEngine;

namespace Kapibara.ConnectSlots
{
    public static class PositionConverter
    {
        /// <summary>
        /// Converts a world Transform position into a screen-space position compatible
        /// with a Canvas in Screen Space - Overlay.
        /// </summary>
        /// <param name="transform">The world Transform.</param>
        /// <returns>Screen-space position (Vector2) usable by UI RectTransforms.</returns>
        public static Vector2 ConvertToScreenSpaceOverlayCanvasPosition(this Transform transform)
        {
            return Camera.main.WorldToScreenPoint(transform.position);
        }

        /// <summary>
        /// Converts a RectTransform screen-space position (from a Screen Space - Overlay canvas)
        /// to world-space coordinates using the main camera.
        /// </summary>
        /// <param name="rectTransform">The RectTransform in the UI.</param>
        /// <param name="z">Z depth to use for world placement (usually target world object's Z).</param>
        /// <returns>World-space position (Vector3).</returns>
        public static Vector3 ConvertToWorldspacePosition(this RectTransform rectTransform, float z = 0f)
        {
            Vector3 screenPos = rectTransform.position;
            screenPos.z = z; // Set desired depth in world
            return Camera.main.ScreenToWorldPoint(screenPos);
        }
    }
}