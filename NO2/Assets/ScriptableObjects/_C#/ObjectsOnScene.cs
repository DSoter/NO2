using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectsOnScene", menuName = "Scriptable Objects/ObjectsOnScene")]
public class ObjectsOnScene : ScriptableObject
{
    private Dictionary<string, bool> objectsRespawnAfterRest;
    private Dictionary<string, bool> objectsNeverRespawn;

    private string SaveKeyRest => name + "_rest";
    private string SaveKeyNever => name + "_never";

    public Dictionary<string, bool> ObjectsRespawnAfterRest
    {
        get { return objectsRespawnAfterRest; }
        set { objectsRespawnAfterRest = value; }
    }
    public Dictionary<string, bool> ObjectsNeverRespawn
    {
        get { return objectsNeverRespawn; }
        set { objectsNeverRespawn = value; }
    }

    public void Save()
    {
        SaveDictionary(objectsRespawnAfterRest, SaveKeyRest);
        SaveDictionary(objectsNeverRespawn, SaveKeyNever);
    }

    public void Load()
    {
        objectsRespawnAfterRest = LoadDictionary(SaveKeyRest);
        objectsNeverRespawn = LoadDictionary(SaveKeyNever);
    }

    public void Reset()
    {
        PlayerPrefs.DeleteKey(SaveKeyRest + "_keys");
        PlayerPrefs.DeleteKey(SaveKeyRest + "_values");
        PlayerPrefs.DeleteKey(SaveKeyNever + "_keys");
        PlayerPrefs.DeleteKey(SaveKeyNever + "_values");
        PlayerPrefs.Save();
        objectsRespawnAfterRest = new Dictionary<string, bool>();
        objectsNeverRespawn = new Dictionary<string, bool>();
    }

    private void SaveDictionary(Dictionary<string, bool> dict, string key)
    {
        if (dict == null) return;
        List<string> keys = new List<string>();
        List<string> values = new List<string>();
        foreach (KeyValuePair<string, bool> pair in dict)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value ? "1" : "0");
        }
        PlayerPrefs.SetString(key + "_keys", string.Join("|", keys));
        PlayerPrefs.SetString(key + "_values", string.Join("|", values));
        PlayerPrefs.Save();
    }

    private Dictionary<string, bool> LoadDictionary(string key)
    {
        Dictionary<string, bool> dict = new Dictionary<string, bool>();
        string savedKeys = PlayerPrefs.GetString(key + "_keys", "");
        string savedValues = PlayerPrefs.GetString(key + "_values", "");
        if (string.IsNullOrEmpty(savedKeys)) return dict;

        string[] keys = savedKeys.Split('|');
        string[] values = savedValues.Split('|');
        for (int i = 0; i < keys.Length; i++)
            dict[keys[i]] = values[i] == "1";
        return dict;
    }
}