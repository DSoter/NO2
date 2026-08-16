using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PersistentRespawnObject : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private bool respawnAfterRest;
    [SerializeField] private bool respawnAfterDeath;
    [SerializeField] private bool neverRespawn;
    private RespawnObjectsManager respawnObjectsManager;
    [SerializeField] private string guid;

    public string Guid => guid;

    private void Awake()
    {
        if (neverRespawn)
        {
            respawnAfterRest = false;
            respawnAfterDeath = false;
        }
        respawnObjectsManager = GameObject.FindGameObjectWithTag("SceneController").GetComponent<RespawnObjectsManager>();
    }

    private void Start()
    {
        HandleSpawn();
    }

    private void HandleSpawn()
    {
        if (neverRespawn)
        {
            bool shouldSpawn = respawnObjectsManager.GetObjectNeverRespawn(guid);
            if (!shouldSpawn)
                gameObject.SetActive(false);
            return;
        }

        bool shouldSpawnByRest = !respawnAfterRest || respawnObjectsManager.GetObjectRest(guid);
        bool shouldSpawnByDeath = !respawnAfterDeath || respawnObjectsManager.GetObjectRespawnAfterDeath(guid);

        // Si tiene ambos triggers activos, debe cumplir los dos para permanecer visible;
        // si solo tiene uno activo, basta con que ese lo permita.
        if (!shouldSpawnByRest || !shouldSpawnByDeath)
            gameObject.SetActive(false);
    }

    public void RegisterDestroy()
    {
        if (neverRespawn)
        {
            respawnObjectsManager.SetObjectNeverRespawn(guid, false);
        }
        if (respawnAfterRest)
        {
            respawnObjectsManager.SetObjectRest(guid, false);
        }
        if (respawnAfterDeath)
        {
            respawnObjectsManager.SetObjectRespawnAfterDeath(guid, false);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(guid))
        {
            guid = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
        }
    }
#endif
}