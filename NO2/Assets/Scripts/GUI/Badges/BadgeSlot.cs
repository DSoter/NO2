using UnityEngine;
using UnityEngine.UI;

public class BadgeSlot : MonoBehaviour
{
    public int slotIndex;
    private Image _highlight;
    private Color _originalColor;

    private void Awake()
    {
        _highlight = GetComponent<Image>();
        _originalColor = _highlight.color;
    }

    public void SetHighlight(bool active)
    {
        if (_highlight != null)
            _highlight.color = active ? new Color(1f, 1f, 0f, 0.3f) : _originalColor;
    }
}