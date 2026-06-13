using UnityEngine;

public class SpawnId : MonoBehaviour
{
    [SerializeField] private int spawnid;


    public int IdSpawn {
        get
        { 
            return spawnid; 
        }
    }
#if UNITY_EDITOR
    private void Reset()
    {
        SpawnId[] allSpawnIds = FindObjectsByType<SpawnId>();
        spawnid = allSpawnIds.Length - 1; // El actual ya está incluido
    }
#endif
}
