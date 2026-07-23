using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PersistentRespawnObject : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private bool respawnAfterRest;
    [SerializeField] private bool neverRespawn;
    private RespawnObjectsManager respawnObjectsManager;
    [SerializeField] private string guid;

    public string Guid => guid;

    private void Awake()
    {
        if (neverRespawn) { respawnAfterRest = false; }
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
        }
        else if (respawnAfterRest)
        {
            bool shouldSpawn = respawnObjectsManager.GetObjectRest(guid);
            if (!shouldSpawn)
                gameObject.SetActive(false);
        }   
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