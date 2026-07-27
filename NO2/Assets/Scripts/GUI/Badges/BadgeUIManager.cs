using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static PlayerController;
using static UnityEngine.Rendering.DebugUI;

public class BadgeUIManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BadgeCollection badgeCollection;
    [SerializeField] private EquippedBadges equippedBadges;
    [SerializeField] private Canvas canvas;

    [Header("UI")]
    [SerializeField] private RectTransform equipPanel;
    [SerializeField] private Transform collectionPanel;
    [SerializeField] private BadgeSlot[] slots; // 4 slots asignados en inspector
    [SerializeField] private GameObject badgeIconPrefab;
    [SerializeField] private float slotWidth = 100f;
    [SerializeField] private float maxDropDistance = 200f;

    [Header("Panel Info")]
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private Image infoIcon;
    [SerializeField] private TMPro.TMP_Text infoName;
    [SerializeField] private TMPro.TMP_Text infoDescription;

    

    private List<BadgeIcon> _collectionIcons = new List<BadgeIcon>();
    public PlayerController.PlayerState playerState;

    private void OnEnable()
    {
        badgeCollection.Load();
        equippedBadges.Load(badgeCollection);

        CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();

        if (cm != null)
        {
            PlayerController player = cm.PlayerReference.gameObject.GetComponent<PlayerController>();
            playerState = player.GetState();
        }
        StartCoroutine(RefreshNextFrame());
    }
    private void OnDisable()
    {
        descriptionPanel.SetActive(false);
    }

    public float GetSlotWidth() => slotWidth;


    private IEnumerator RefreshNextFrame()
    {
        yield return null;
        RefreshUI();
    }
    public void RefreshUI()
    {
        foreach (Transform child in equipPanel)
        {
            BadgeIcon icon = child.GetComponent<BadgeIcon>();
            if (icon != null && icon.IsEquipped)
                Destroy(child.gameObject);
        }

        foreach (BadgeIcon icon in _collectionIcons)
            if (icon != null) Destroy(icon.gameObject);
        _collectionIcons.Clear();

        Badge[] equipped = equippedBadges.Slots;
        HashSet<Badge> alreadyPlaced = new HashSet<Badge>();
        for (int i = 0; i < EquippedBadges.TotalSlots; i++)
        {
            if (equipped[i] == null || alreadyPlaced.Contains(equipped[i])) continue;
            alreadyPlaced.Add(equipped[i]);

            GameObject iconGO = Instantiate(badgeIconPrefab, slots[i].transform);
            BadgeIcon icon = iconGO.GetComponent<BadgeIcon>();
            icon.Initialize(equipped[i], this, canvas, true);
            iconGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            iconGO.transform.SetParent(equipPanel);
            iconGO.transform.SetAsLastSibling();

            
        }


        foreach (KeyValuePair<Badge, bool> entry in badgeCollection.UnlockedBadges)
        {
            Badge b = entry.Key;
            if (!badgeCollection.UnlockedBadges[b]) continue;
            if (alreadyPlaced.Contains(b)) continue;

            
            GameObject iconGO = Instantiate(badgeIconPrefab, collectionPanel);
            BadgeIcon icon = iconGO.GetComponent<BadgeIcon>();
            icon.Initialize(b, this, canvas, false);
            _collectionIcons.Add(icon);
        }

        
    }

    public void OnBeginDrag(BadgeIcon icon)
    {

        ShowBadgeInfo(icon.badge);
        if (playerState != PlayerState.Rest)
        {
           return;
        }
        if (icon.IsEquipped)
            equippedBadges.UnequipBadge(icon.badge);
    }

    public void OnDrag(BadgeIcon icon, PointerEventData eventData)
    {
        
        if (playerState != PlayerState.Rest)
        {
            return;
        }
        int closest = GetClosestSlotIndex(eventData.position, icon.badge.slotsSize);

        if (closest >= 0 && IsCloseEnough(eventData.position, closest)
            && equippedBadges.CanEquip(closest, icon.badge.slotsSize))
            UpdateHighlights(closest, icon.badge.slotsSize);
        else
            ClearHighlights();
    }

    public void OnEndDrag(BadgeIcon icon, PointerEventData eventData)
    {
        if (playerState != PlayerState.Rest)
        {
            return;
        }

        ClearHighlights();

        if (IsOverCollectionPanel(eventData.position))
        {
            if (icon.IsEquipped)
            {
                equippedBadges.UnequipBadge(icon.badge);
                equippedBadges.Save();
            }
            Destroy(icon.gameObject);
            RefreshUI();
            return;
        }

        int closest = GetClosestSlotIndex(eventData.position, icon.badge.slotsSize);

        if (closest >= 0 && IsCloseEnough(eventData.position, closest)
            && equippedBadges.CanEquip(closest, icon.badge.slotsSize))
        {
            List<Badge> displaced = equippedBadges.Equip(icon.badge, closest);
            foreach (Badge b in displaced)
                SpawnInCollection(b);

            Destroy(icon.gameObject);
            RefreshUI();
        }
        else
        {
            if (icon.IsEquipped)
                equippedBadges.Equip(icon.badge, GetCurrentSlot(icon.badge));
            icon.ReturnToOriginalPosition();
        }
    }


    private bool IsCloseEnough(Vector2 screenPosition, int slotIndex)
    {
        Vector2 slotScreen = RectTransformUtility.WorldToScreenPoint(
            null, slots[slotIndex].transform.position);
        return Vector2.Distance(screenPosition, slotScreen) <= maxDropDistance;
    }
    public void OnHoverSlot(int size, bool active)
    {
        // Se puede expandir para highlight al hacer hover
    }


    private bool IsOverCollectionPanel(Vector2 screenPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            collectionPanel.GetComponent<RectTransform>(),
            screenPosition,
            null
        );
    }

    private int GetClosestSlotIndex(Vector2 screenPosition, int size)
    {
        float minDist = float.MaxValue;
        int closest = -1;

        for (int i = 0; i < slots.Length; i++)
        {
            Vector2 slotScreen = RectTransformUtility.WorldToScreenPoint(
                null, slots[i].transform.position);
            float dist = Vector2.Distance(screenPosition, slotScreen);
            if (dist < minDist)
            {
                minDist = dist;
                closest = i;
            }
        }

        return closest;
    }

    private int GetCurrentSlot(Badge badge)
    {
        Badge[] equipped = equippedBadges.Slots;
        for (int i = 0; i < equipped.Length; i++)
            if (equipped[i] == badge) return i;
        return 0;
    }

    private void UpdateHighlights(int centerSlot, int size)
    {
        ClearHighlights();
        if (centerSlot < 0) return;

        (int start, int end) = equippedBadges.GetSlotRange(centerSlot, size);
        for (int i = start; i <= end; i++)
            slots[i].SetHighlight(true);
    }

    private void ClearHighlights()
    {
        foreach (BadgeSlot slot in slots)
            slot.SetHighlight(false);
    }

    private void SpawnInCollection(Badge badge)
    {
        GameObject iconGO = Instantiate(badgeIconPrefab, collectionPanel);
        BadgeIcon icon = iconGO.GetComponent<BadgeIcon>();
        icon.Initialize(badge, this, canvas, false);
        _collectionIcons.Add(icon);
    }

    public void ShowBadgeInfo(Badge badge)
    {
        if (badge == null) return;
        descriptionPanel.SetActive(true);
        infoIcon.sprite = badge.icon;
        infoName.text = badge.badgeName;
        infoDescription.text = badge.description;
    }

    public RectTransform GetSlotRect()
    {
        return slots[0].GetComponent<RectTransform>();
    }
}