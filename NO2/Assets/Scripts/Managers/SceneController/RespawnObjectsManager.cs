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

        string sceneName = SceneManager.GetActiveScene().name;
        if (!GameManager.Instance.GetComponent<CheckpointManager>().ScenesHasToRespawn.ContainsKey(sceneName))
        {
            GameManager.Instance.GetComponent<CheckpointManager>().ScenesHasToRespawn.Add(sceneName, true);
            //si el checkpoint manager no tiene la referencia de la escena la genera nueva y la pone a true
        }

        hasToUpdateRest = GameManager.Instance.GetComponent<CheckpointManager>().ScenesHasToRespawn[sceneName];
        GameManager.Instance.GetComponent<CheckpointManager>().ScenesHasToRespawn[sceneName] = false;
        if (hasToUpdateRest)
        {
            UpdateRest();
        }
    }
    public void UpdateRest()
    {
        Debug.Log("Respawn all rest things");
        if (objectsOnScene.ObjectsRespawnAfterRest == null)
        {
            objectsOnScene.ObjectsRespawnAfterRest = new Dictionary<string, bool>();
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
            objectsOnScene.ObjectsRespawnAfterRest.Add(objeto,true);
            return true;
        }
        else
        {
            return objectsOnScene.ObjectsRespawnAfterRest[objeto];
        }
    }
    public void SetObjectNeverRespawn(string objeto, bool state)
    {
        if(!objectsOnScene.ObjectsNeverRespawn.ContainsKey(objeto)) {
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
