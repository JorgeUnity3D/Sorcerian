using UnityEngine;

namespace Kapibara.ConnectSlots
{
    public static class PositionConverter
    {
        // ----------------------------
        // WORLD → SCREEN (Overlay)
        // ----------------------------
        public static Vector2 ConvertToScreenSpaceOverlayCanvasPosition(this Transform transform)
        {
            return Camera.main.WorldToScreenPoint(transform.position);
        }

        // ----------------------------
        // WORLD → SCREEN (Camera Space Canvas → local UI coords)
        // ----------------------------
        public static Vector2 ConvertToScreenSpaceCameraCanvasPosition(
            this Transform worldTransform,
            Canvas targetCanvas)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldTransform.position);
            RectTransform canvasRect = targetCanvas.GetComponent<RectTransform>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPos,
                targetCanvas.worldCamera,
                out Vector2 localPos);

            return localPos;
        }

        // ----------------------------
        // UI (RectTransform) → WORLD (Overlay)
        // ----------------------------
        public static Vector3 ConvertToWorldspacePositionOverlay(this RectTransform rectTransform)
        {
            Vector3 screenPos = rectTransform.position;
            screenPos.z = -Camera.main.transform.position.z;
            return Camera.main.ScreenToWorldPoint(screenPos);
        }

        // ----------------------------
        // UI (RectTransform) → WORLD (Camera Space Canvas)
        // ----------------------------
        public static Vector3 ConvertToWorldspacePositionCamera(
            this RectTransform rectTransform,
            Canvas canvas)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
                canvas.worldCamera,
                rectTransform.position);

            // camera planeDistance determines depth in world
            float z = canvas.planeDistance;

            Vector3 worldPos = canvas.worldCamera.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, z));
            return worldPos;
        }

        // ----------------------------
        // SCREEN → Canvas localPoint helper
        // ----------------------------
        public static Vector2 ScreenToCanvasLocalPoint(
            Vector2 screenPos,
            Canvas canvas)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPos,
                canvas.worldCamera,
                out Vector2 localPoint);

            return localPoint;
        }
    }
}
