using UnityEngine;

public static class TransformExtension
{
    public static void DetroyChildren(this Transform transform)
    {
        foreach (var child in transform)
        {
            GameObject.Destroy(((Transform)child).gameObject);
        }
    }

    public static Transform Reset(this Transform transform, Transform parent = null)
    {
        if (parent != null)
        {
            transform.SetParent(parent);
        }

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        return transform;
    }

    public static RectTransform Reset(this RectTransform rectTransform, Transform parent, Vector2 deltaSize = default(Vector2))
    {
        if (parent != null)
        {
            rectTransform.SetParent(parent);
        }

        rectTransform.anchoredPosition = Vector2.zero;

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, deltaSize.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, deltaSize.y);

        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
        return rectTransform;
    }
}
