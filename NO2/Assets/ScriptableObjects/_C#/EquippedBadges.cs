using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquippedBadges", menuName = "Scriptable Objects/EquippedBadges")]
public class EquippedBadges : ScriptableObject
{
    public const int TotalSlots = 4;
    private Badge[] slots = new Badge[TotalSlots];
    public Badge[] Slots => slots;

    public event Action OnEquippedChanged;

    private string SaveKey => name + "_equipped";

    public (int start, int end) GetSlotRange(int slotIndex, int size)
    {
        int half = size / 2;
        int start = slotIndex - half;
        int end = start + size - 1;

        if (start < 0)
        {
            end -= start;
            start = 0;
        }
        if (end >= TotalSlots)
        {
            start -= (end - TotalSlots + 1);
            end = TotalSlots - 1;
        }

        return (start, end);
    }

    public bool CanEquip(int slotIndex, int size)
    {
        (int start, int end) = GetSlotRange(slotIndex, size);
        return start >= 0 && end < TotalSlots;
    }

    // Devuelve las insignias desplazadas al equipar
    public List<Badge> Equip(Badge badge, int slotIndex)
    {
        (int start, int end) = GetSlotRange(slotIndex, badge.slotsSize);
        List<Badge> displaced = new List<Badge>();

        for (int i = start; i <= end; i++)
        {
            if (slots[i] != null && !displaced.Contains(slots[i]))
                displaced.Add(slots[i]);
        }

        foreach (Badge b in displaced)
            UnequipBadge(b);

        for (int i = start; i <= end; i++)
            slots[i] = badge;

        Save();
        OnEquippedChanged?.Invoke();
        return displaced;
    }

    public void UnequipBadge(Badge badge)
    {
        for (int i = 0; i < TotalSlots; i++)
            if (slots[i] == badge)
                slots[i] = null;

        Save();
        OnEquippedChanged?.Invoke();
    }

    public void Save()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < TotalSlots; i++)
        {
            sb.Append(slots[i] != null ? slots[i].name : "null");
            if (i < TotalSlots - 1) sb.Append(",");
        }
        PlayerPrefs.SetString(SaveKey, sb.ToString());
        PlayerPrefs.Save();
    }

    public void Load(BadgeCollection collection)
    {
        slots = new Badge[TotalSlots];
        string saved = PlayerPrefs.GetString(SaveKey, "");
        if (string.IsNullOrEmpty(saved)) return;

        string[] names = saved.Split(',');
        for (int i = 0; i < TotalSlots && i < names.Length; i++)
        {
            if (names[i] == "null") continue;
            slots[i] = collection.allBadges.Find(b => b.name == names[i]);
        }
    }

    public void Reset()
    {
        slots = new Badge[TotalSlots];
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        OnEquippedChanged?.Invoke();
    }
}
