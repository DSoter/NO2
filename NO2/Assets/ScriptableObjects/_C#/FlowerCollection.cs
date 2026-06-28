using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FlowerCollection", menuName = "Scriptable Objects/FlowerCollection")]
public class FlowerCollection : ScriptableObject
{
    public List<Flower> allFlowers;
    public Dictionary<Flower, bool> unlockedFlowers;
    
    private string KeySaveKey = "UnlockedFlowers_Keys";
    private string ValueSaveKey = "UnlockedFlowers_Values";

    public void InitializeDictionary()
    {
        unlockedFlowers = new Dictionary<Flower, bool>();
        foreach (Flower f in allFlowers)
        {
            unlockedFlowers.Add(f, false);
        }
        Debug.Log(unlockedFlowers);
        
    }

    public void Save()
    {
        List<string> keys = new List<string>();
        List<string> values = new List<string>();
        foreach (KeyValuePair<Flower, bool> entry in unlockedFlowers)
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

        unlockedFlowers = new Dictionary<Flower, bool>();
        for (int i = 0; i < keys.Length; i++)
        {
            Flower found = Resources.Load<Flower>("ScriptableObjects/Flores/" + keys[i]);
            if (found != null)
            {
                bool unlocked = values[i] == "1";
                unlockedFlowers[found] = unlocked;
            }
        }
    }

    public void Reset()
    {
        PlayerPrefs.SetString(KeySaveKey, "");
        PlayerPrefs.SetString(ValueSaveKey, "");
        PlayerPrefs.Save();
        foreach (KeyValuePair<Flower, bool> entry in unlockedFlowers)
        {
            unlockedFlowers[entry.Key] = false;
        }
    }
}
