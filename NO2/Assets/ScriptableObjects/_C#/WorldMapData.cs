using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldMapData", menuName = "Scriptable Objects/WorldMapData")]
public class WorldMapData : ScriptableObject
{
    [Serializable]
    public class SceneMapEntry
    {
        public string sceneName;
        public SceneMapData mapData;
        public Vector2Int offsetInCells; // posición relativa en celdas respecto a la escena origen
    }

    [Serializable]
    public class GatePosition
    {
        public string sceneName;
        public int gateId;
        public Vector2 worldPosition; // posición del trigger en el mundo
    }

    public List<SceneMapEntry> scenes = new List<SceneMapEntry>();
    public List<GatePosition> gatePositions = new List<GatePosition>();

    private string SaveKeyGates => name + "_gates";
    private string SaveKeyOffsets => name + "_offsets";

    // Registrar la posición de un gate al cargar la escena
    public void RegisterGatePosition(string sceneName, int gateId, Vector2 worldPosition)
    {
        GatePosition existing = gatePositions.Find(g => g.sceneName == sceneName && g.gateId == gateId);
        if (existing != null)
        {
            existing.worldPosition = worldPosition;
        }
        else
        {
            gatePositions.Add(new GatePosition
            {
                sceneName = sceneName,
                gateId = gateId,
                worldPosition = worldPosition
            });
        }
    }

    public Vector2 GetGatePosition(string sceneName, int gateId)
    {
        GatePosition gate = gatePositions.Find(g => g.sceneName == sceneName && g.gateId == gateId);
        return gate != null ? gate.worldPosition : Vector2.zero;
    }

    // Calcular offsets entre escenas usando los gates conectados
    public void CalculateOffsets(string originScene)
    {
        // Resetear offsets
        foreach (SceneMapEntry entry in scenes)
            entry.offsetInCells = Vector2Int.zero;

        SceneMapEntry origin = scenes.Find(s => s.sceneName == originScene);
        if (origin == null) return;

        origin.offsetInCells = Vector2Int.zero;

        // BFS para propagar offsets desde la escena origen
        HashSet<string> visited = new HashSet<string>();
        Queue<string> queue = new Queue<string>();
        queue.Enqueue(originScene);
        visited.Add(originScene);

        while (queue.Count > 0)
        {
            string currentScene = queue.Dequeue();
            SceneMapEntry currentEntry = scenes.Find(s => s.sceneName == currentScene);
            if (currentEntry == null) continue;

            // Buscar todos los gates de esta escena
            List<GatePosition> currentGates = gatePositions.FindAll(g => g.sceneName == currentScene);

            foreach (GatePosition gate in currentGates)
            {
                // Buscar el GateData que conecta este gate con otra escena
                GateData gateData = FindGateData(currentScene, gate.gateId);
                if (gateData == null) continue;

                string neighborScene = gateData.sceneName1 == currentScene
                    ? gateData.sceneName2
                    : gateData.sceneName1;
                int neighborGateId = gateData.sceneName1 == currentScene
                    ? gateData.gateId2
                    : gateData.gateId1;

                if (visited.Contains(neighborScene)) continue;

                GatePosition neighborGate = gatePositions.Find(
                    g => g.sceneName == neighborScene && g.gateId == neighborGateId);
                if (neighborGate == null) continue;

                // Calcular offset en celdas
                SceneMapEntry neighborEntry = scenes.Find(s => s.sceneName == neighborScene);
                if (neighborEntry == null) continue;

                float cellSize = 1f; // ajusta si tus celdas no son 1 unidad
                Vector2 diff = gate.worldPosition - neighborGate.worldPosition;
                Vector2Int diffInCells = new Vector2Int(
                    Mathf.RoundToInt(diff.x / cellSize),
                    Mathf.RoundToInt(diff.y / cellSize)
                );

                neighborEntry.offsetInCells = currentEntry.offsetInCells + diffInCells;

                visited.Add(neighborScene);
                queue.Enqueue(neighborScene);
            }
        }

        SaveOffsets();
    }

    public bool WorldMapNeedsUpdate
    {
        get { return PlayerPrefs.GetInt(name + "_worldDirty", 1) == 1; }
        set { PlayerPrefs.SetInt(name + "_worldDirty", value ? 1 : 0); PlayerPrefs.Save(); }
    }

    private GateData FindGateData(string sceneName, int gateId)
    {
        // Los GateData están en Resources para poder acceder sin cargar la escena
        GateData[] allGates = Resources.LoadAll<GateData>("GateData");
        foreach (GateData gd in allGates)
        {
            if ((gd.sceneName1 == sceneName && gd.gateId1 == gateId) ||
                (gd.sceneName2 == sceneName && gd.gateId2 == gateId))
                return gd;
        }
        return null;
    }

    public void SaveOffsets()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (SceneMapEntry entry in scenes)
            sb.Append($"{entry.sceneName}:{entry.offsetInCells.x},{entry.offsetInCells.y}|");
        PlayerPrefs.SetString(SaveKeyOffsets, sb.ToString());
        PlayerPrefs.Save();
    }

    public void LoadOffsets()
    {
        string saved = PlayerPrefs.GetString(SaveKeyOffsets, "");
        if (string.IsNullOrEmpty(saved)) return;

        foreach (string part in saved.Split('|'))
        {
            if (string.IsNullOrEmpty(part)) continue;
            string[] split = part.Split(':');
            if (split.Length < 2) continue;

            string sceneName = split[0];
            string[] coords = split[1].Split(',');
            if (coords.Length < 2) continue;

            SceneMapEntry entry = scenes.Find(s => s.sceneName == sceneName);
            if (entry != null)
                entry.offsetInCells = new Vector2Int(
                    int.Parse(coords[0]),
                    int.Parse(coords[1])
                );
        }
    }
}