using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockedFlowers", menuName = "Scriptable Objects/UnlockedFlowers")]
public class UnlockedFlowers : ScriptableObject
{
    public List<Flower> unlockedFlowers;
    private string SaveKey = "UnlockedFlowers";

    public void Save()
    {
        List<string> names = new List<string>();
        foreach (Flower f in unlockedFlowers)
            names.Add(f.name);
        PlayerPrefs.SetString(SaveKey, string.Join(",", names));
        PlayerPrefs.Save();
    }

    public void Load()
    {
        string saved = PlayerPrefs.GetString(SaveKey, "");
        if (string.IsNullOrEmpty(saved)) return;

        unlockedFlowers = new List<Flower>();
        foreach (string n in saved.Split(','))
        {
            Flower found = Resources.Load<Flower>("ScriptableObjects/Flores/" + n);
            if (found != null) unlockedFlowers.Add(found);
        }
    }
    public void Reset()
    {
        PlayerPrefs.SetString(SaveKey, "");
        PlayerPrefs.Save();
    }
}
