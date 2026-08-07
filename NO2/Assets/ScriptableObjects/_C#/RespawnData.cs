using UnityEngine;

[CreateAssetMenu(fileName = "RespawnData", menuName = "Scriptable Objects/RespawnData")]
public class RespawnData : ScriptableObject
{
    private const string KeyScene = "RespawnData_scene";
    private const string KeyId = "RespawnData_id";

    [Header("Valores por defecto (partida nueva)")]
    [SerializeField] private string defaultSceneName = "PruebaTileMap";
    [SerializeField] private int defaultCheckpointId = 0;

    private string sceneWhereRespawn;
    private int checkpointId;

    public string SceneWhereRespawn
    {
        get { return sceneWhereRespawn; }
        set { sceneWhereRespawn = value; }
    }
    public int CheckpointId
    {
        get { return checkpointId; }
        set { checkpointId = value; }
    }

    public void Save()
    {
        PlayerPrefs.SetString(KeyScene, sceneWhereRespawn);
        PlayerPrefs.SetInt(KeyId, checkpointId);
        PlayerPrefs.Save();
        Debug.Log($"RespawnData guardado: escena={sceneWhereRespawn} id={checkpointId}");
    }

    public bool Load()
    {
        if (!PlayerPrefs.HasKey(KeyScene))
        {
            sceneWhereRespawn = defaultSceneName;
            checkpointId = defaultCheckpointId;
            return false;
        }

        sceneWhereRespawn = PlayerPrefs.GetString(KeyScene, defaultSceneName);
        checkpointId = PlayerPrefs.GetInt(KeyId, defaultCheckpointId);
        return true;
    }

    [ContextMenu("Reset")]
    public void Reset()
    {
        sceneWhereRespawn = defaultSceneName;
        checkpointId = defaultCheckpointId;
        PlayerPrefs.SetString(KeyScene, defaultSceneName);
        PlayerPrefs.SetInt(KeyId, defaultCheckpointId);
        PlayerPrefs.Save();
        Debug.Log("RespawnData reseteado a valores por defecto");
    }
}