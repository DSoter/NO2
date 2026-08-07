using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FastScroll : MonoBehaviour, IScrollHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollSpeed = 3f; // Ajusta este valor

    public void OnScroll(PointerEventData eventData)
    {
        if (scrollRect == null) return;

        Vector2 delta = eventData.scrollDelta;
        delta.y *= scrollSpeed;
        scrollRect.velocity += new Vector2(0, -delta.y * 1000f * scrollSpeed);
    }
}