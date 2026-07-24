using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PlayerController;

public class BadgeIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
                                        IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Badge badge;
    private BadgeUIManager _manager;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    private Vector2 _originalPosition;
    private Transform _originalParent;
    private bool _isEquipped;
    private int _originalSiblingIndex;

    public bool IsEquipped
    {
        get => _isEquipped;
        set => _isEquipped = value;
    }

    public void Initialize(Badge b, BadgeUIManager manager, Canvas canvas, bool isEquipped)
    {
        badge = b;
        _manager = manager;
        _canvas = canvas;
        _isEquipped = isEquipped;

        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        GetComponent<Image>().sprite = b.icon;

        // Escalar visualmente según slotsSize
        float slotWidth = _manager.GetSlotWidth();
        //_rectTransform.sizeDelta = new Vector2(slotWidth * b.slotsSize, _rectTransform.sizeDelta.y);
        RectTransform slotRect = _manager.GetSlotRect();
        float slotHeight = slotRect.rect.height;
        _rectTransform.sizeDelta = new Vector2(slotWidth * b.slotsSize, slotHeight);

        _rectTransform.pivot = new Vector2(0f, 0.5f);
        _rectTransform.anchorMin = new Vector2(0f, 0.5f);
        _rectTransform.anchorMax = new Vector2(0f, 0.5f);
        _rectTransform.anchoredPosition = new Vector2(0f, 0f);
    }

    

    public void OnPointerClick(PointerEventData eventData)
    {
        _manager.ShowBadgeInfo(badge);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_manager.playerState != PlayerState.Rest)
        {
            return;
        }
        _originalPosition = _rectTransform.anchoredPosition;
        _originalParent = transform.parent;
        _originalSiblingIndex = transform.GetSiblingIndex();

        
        transform.SetParent(_canvas.transform);
        _canvasGroup.blocksRaycasts = false;

        _manager.OnBeginDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_manager.playerState != PlayerState.Rest)
        {
            return;
        }
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        _manager.OnDrag(this, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_manager.playerState != PlayerState.Rest)
        {
            return;
        }
        _canvasGroup.blocksRaycasts = true;
        _manager.OnEndDrag(this, eventData);
    }

    public void OnPointerEnter(PointerEventData eventData) => _manager.OnHoverSlot(badge.slotsSize, true);
    public void OnPointerExit(PointerEventData eventData) => _manager.OnHoverSlot(badge.slotsSize, false);

    public void ReturnToOriginalPosition()
    {
        transform.SetParent(_originalParent);
        transform.SetSiblingIndex(_originalSiblingIndex);
        _rectTransform.anchoredPosition = _originalPosition;
    }
}