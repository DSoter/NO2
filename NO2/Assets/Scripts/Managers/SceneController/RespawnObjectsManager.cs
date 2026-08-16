using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnObjectsManager : MonoBehaviour
{
    [SerializeField] private ObjectsOnScene objectsOnScene;
    private bool hasToUpdateRest;
    private Dictionary<string, PersistentRespawnObject> respawnObjects = new Dictionary<string, PersistentRespawnObject>();

    private void Awake()
    {
        objectsOnScene.Load();
        PersistentRespawnObject[] objects = FindObjectsByType<PersistentRespawnObject>();

        foreach (PersistentRespawnObject obj in objects)
        {
            respawnObjects.Add(obj.Guid, obj);
        }

        CheckpointManager checkpointManager = GameManager.Instance.GetComponent<CheckpointManager>();
        string sceneName = SceneManager.GetActiveScene().name;

        if (!checkpointManager.ScenesHasToRespawn.ContainsKey(sceneName))
        {
            checkpointManager.ScenesHasToRespawn.Add(sceneName, true);
        }

        hasToUpdateRest = checkpointManager.ScenesHasToRespawn[sceneName];
        checkpointManager.ScenesHasToRespawn[sceneName] = false;
        if (hasToUpdateRest)
        {
            UpdateRest();
        }

        if (!checkpointManager.ScenesHasToRespawnAfterDeath.ContainsKey(sceneName))
        {
            checkpointManager.ScenesHasToRespawnAfterDeath.Add(sceneName, true);
        }

        bool hasToUpdateDeath = checkpointManager.ScenesHasToRespawnAfterDeath[sceneName];
        checkpointManager.ScenesHasToRespawnAfterDeath[sceneName] = false;
        if (hasToUpdateDeath)
        {
            UpdateDeath();
        }
    }

    public void UpdateRest()
    {
        Debug.Log("Respawn all rest things");
        if (objectsOnScene.ObjectsRespawnAfterRest == null)
        {
            objectsOnScene.ObjectsRespawnAfterRest = new Dictionary<string, bool>();
            objectsOnScene.ObjectsRespawnAfterDeath = new Dictionary<string, bool>();
            objectsOnScene.ObjectsNeverRespawn = new Dictionary<string, bool>();
        }

        List<string> keys = new List<string>(objectsOnScene.ObjectsRespawnAfterRest.Keys);
        foreach (string objeto in keys)
        {
            objectsOnScene.ObjectsRespawnAfterRest[objeto] = true;
            if (respawnObjects.ContainsKey(objeto))
                respawnObjects[objeto].gameObject.SetActive(true);
        }

        objectsOnScene.Save();
        hasToUpdateRest = false;
    }

    public void UpdateDeath()
    {
        Debug.Log("Respawn all death things");
        if (objectsOnScene.ObjectsRespawnAfterDeath == null)
        {
            objectsOnScene.ObjectsRespawnAfterRest = new Dictionary<string, bool>();
            objectsOnScene.ObjectsRespawnAfterDeath = new Dictionary<string, bool>();
            objectsOnScene.ObjectsNeverRespawn = new Dictionary<string, bool>();
        }

        List<string> keys = new List<string>(objectsOnScene.ObjectsRespawnAfterDeath.Keys);
        foreach (string objeto in keys)
        {
            objectsOnScene.ObjectsRespawnAfterDeath[objeto] = true;
            if (respawnObjects.ContainsKey(objeto))
                respawnObjects[objeto].gameObject.SetActive(true);
        }

        objectsOnScene.Save();
    }

    public void SetObjectRest(string objeto, bool state)
    {
        if (!objectsOnScene.ObjectsRespawnAfterRest.ContainsKey(objeto))
        {
            objectsOnScene.ObjectsRespawnAfterRest.Add(objeto, true);
        }
        objectsOnScene.ObjectsRespawnAfterRest[objeto] = state;
        objectsOnScene.Save();
    }

    public bool GetObjectRest(string objeto)
    {
        if (!objectsOnScene.ObjectsRespawnAfterRest.ContainsKey(objeto))
        {
            objectsOnScene.ObjectsRespawnAfterRest.Add(objeto, true);
            return true;
        }
        else
        {
            return objectsOnScene.ObjectsRespawnAfterRest[objeto];
        }
    }

    public void SetObjectRespawnAfterDeath(string objeto, bool state)
    {
        if (!objectsOnScene.ObjectsRespawnAfterDeath.ContainsKey(objeto))
        {
            objectsOnScene.ObjectsRespawnAfterDeath.Add(objeto, true);
        }
        objectsOnScene.ObjectsRespawnAfterDeath[objeto] = state;
        objectsOnScene.Save();
    }

    public bool GetObjectRespawnAfterDeath(string objeto)
    {
        if (!objectsOnScene.ObjectsRespawnAfterDeath.ContainsKey(objeto))
        {
            objectsOnScene.ObjectsRespawnAfterDeath.Add(objeto, true);
            return true;
        }
        else
        {
            return objectsOnScene.ObjectsRespawnAfterDeath[objeto];
        }
    }

    public void SetObjectNeverRespawn(string objeto, bool state)
    {
        if (!objectsOnScene.ObjectsNeverRespawn.ContainsKey(objeto))
        {
            objectsOnScene.ObjectsNeverRespawn.Add(objeto, true);
        }
        objectsOnScene.ObjectsNeverRespawn[objeto] = state;
        objectsOnScene.Save();
    }

    public bool GetObjectNeverRespawn(string objeto)
    {
        if (!objectsOnScene.ObjectsNeverRespawn.ContainsKey(objeto))
        {
            objectsOnScene.ObjectsNeverRespawn.Add(objeto, true);
            return true;
        }
        else
        {
            return objectsOnScene.ObjectsNeverRespawn[objeto];
        }
    }
}