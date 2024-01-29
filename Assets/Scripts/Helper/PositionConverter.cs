using UnityEngine;
using UnityEngine.UI;

public static class PositionConverter
{
    public static Vector2 ScreenToUIElementLocalPosition(Vector2 screenPosition, RectTransform uiElement)
    {
        Canvas canvas = uiElement.GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("UI element is not under a Canvas!");
            return Vector2.zero;
        }

        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(uiElement, screenPosition, canvas.worldCamera, out localPosition);

        return localPosition;
    }
}
