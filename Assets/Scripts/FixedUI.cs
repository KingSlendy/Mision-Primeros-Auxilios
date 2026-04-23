using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class FixedUI : MonoBehaviour {
    public float TargetAspect = 16f / 9f;

    private RectTransform rectTransform;
    private RectTransform parentRectTransform;

    void OnEnable() {
        UpdateSize();
    }

    void OnRectTransformDimensionsChange() {
        UpdateSize();
    }

    void UpdateSize() {
        if (rectTransform == null) {
            rectTransform = GetComponent<RectTransform>();
        }

        if (parentRectTransform == null) {
            parentRectTransform = (RectTransform)transform.parent;
        }

        if (parentRectTransform == null) {
            return;
        }

        float parentWidth = parentRectTransform.rect.width;
        float parentHeight = parentRectTransform.rect.height;

        if (parentWidth <= 0f || parentHeight <= 0f) {
            return;
        }

        float parentAspect = parentWidth / parentHeight;

        float width;
        float height;

        if (parentAspect > TargetAspect) {
            height = parentHeight;
            width = height * TargetAspect;
        } else {
            width = parentWidth;
            height = width / TargetAspect;
        }

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.anchoredPosition = Vector2.zero;
    }
}