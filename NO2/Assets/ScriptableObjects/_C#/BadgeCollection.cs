using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BadgeCollection", menuName = "Scriptable Objects/BadgeCollection")]
public class BadgeCollection : ScriptableObject
{
    public List<Badge> allBadges;
    private Dictionary<Badge,bool> unlockedBadges;
    

    public Dictionary<Badge,bool> UnlockedBadges
    {
        get { return unlockedBadges; }
        set { unlockedBadges = value; }
    }

    private string KeySaveKey = "UnlockedFlowers_Keys";
    private string ValueSaveKey = "UnlockedFlowers_Values";

    public void InitializeDictionary()
    {
        unlockedBadges = new Dictionary<Badge, bool>();
        foreach (Badge b in allBadges)
        {
            unlockedBadges.Add(b, false);
        }


    }

    public void Unlock(Badge badge)
    {
        if (unlockedBadges == null)
        {
            unlockedBadges = new Dictionary<Badge, bool>();
            foreach(Badge b in allBadges)
            {
                unlockedBadges.Add(b, false);
            }
        }
        Debug.Log("Se ha desbloqueado la insignia: ");
        Debug.Log(badge.name);
        unlockedBadges[badge] = true;
        Save();
    }

    public void Save()
    {
        if (unlockedBadges == null) return;
        List<string> keys = new List<string>();
        List<string> values = new List<string>();
        foreach (KeyValuePair<Badge, bool> entry in unlockedBadges)
        {
            keys.Add(entry.Key.name);
            values.Add(entry.Value ? "1" : "0");
        }
        PlayerPrefs.SetString(KeySaveKey, string.Join(",", keys));
        PlayerPrefs.SetString(ValueSaveKey, string.Join(",", values));
        PlayerPrefs.Save();
        
    }

    public void Load()
    {
        InitializeDictionary();
        string savedKeys = PlayerPrefs.GetString(KeySaveKey, "");
        string savedValues = PlayerPrefs.GetString(ValueSaveKey, "");

        if (string.IsNullOrEmpty(savedKeys)) return;

        string[] keys = savedKeys.Split(',');
        string[] values = savedValues.Split(',');

        for (int i = 0; i < keys.Length; i++)
        {
            Badge found = Resources.Load<Badge>("ScriptableObjects/Insignias/" + keys[i]);
            if (found != null)
            {
                bool unlocked = values[i] == "1";
                unlockedBadges[found] = unlocked;
            }
        }

        
    }

    public void Reset()
    {
        PlayerPrefs.SetString(KeySaveKey, "");
        PlayerPrefs.SetString(ValueSaveKey, "");
        PlayerPrefs.Save();
        foreach (KeyValuePair<Badge, bool> entry in unlockedBadges)
        {
            unlockedBadges[entry.Key] = false;
        }
    }
}