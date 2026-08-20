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

        if (!respawnAfterRest && !respawnAfterDeath)
            return; // objeto persistente normal, ningún trigger lo controla

        bool allowedByRest = respawnAfterRest && respawnObjectsManager.GetObjectRest(guid);
        bool allowedByDeath = respawnAfterDeath && respawnObjectsManager.GetObjectRespawnAfterDeath(guid);

        bool shouldBeVisible = allowedByRest || allowedByDeath;

        if (!shouldBeVisible)
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