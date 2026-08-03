using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapDataRegister : MonoBehaviour
{
    [SerializeField] private WorldMapData worldMapData;
    [SerializeField] private ScenesVisited scenesVisited;
    public WorldMapData WorldMapData => worldMapData;
    public ScenesVisited Scenes => scenesVisited;

    private string SaveKeyVisited => worldMapData.name + "_scenesVisited";

    [Serializable]
    public class ScenesVisited
    {
        public List<string> references = new List<string>();
    }

    private void Awake()
    {
        LoadVisited();
    }

    public void LoadVisited()
    {
        if (scenesVisited == null) scenesVisited = new ScenesVisited();

        string saved = PlayerPrefs.GetString(SaveKeyVisited, "");
        scenesVisited.references = string.IsNullOrEmpty(saved)
            ? new List<string>()
            : new List<string>(saved.Split(','));

        Debug.Log($"ScenesVisited cargadas: {scenesVisited.references.Count}");
    }

    public void SaveVisited()
    {
        PlayerPrefs.SetString(SaveKeyVisited, string.Join(",", scenesVisited.references));
        PlayerPrefs.Save();
    }

    [ContextMenu("Reset Scenes Visited")]
    public void ResetVisited()
    {
        PlayerPrefs.DeleteKey(SaveKeyVisited);
        PlayerPrefs.Save();
        scenesVisited.references = new List<string>();
    }

    public bool HasSavedGame()
    {
        string saved = PlayerPrefs.GetString(SaveKeyVisited, "");
        return !string.IsNullOrEmpty(saved);
    }
}