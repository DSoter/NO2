using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private string sceneWhereRespawn;
    [SerializeField] public int idRespawn;
    public int idSpawn;
    [SerializeField] private string nextScene;
    private bool hasToSpawnPlayer;
    private bool hasToSpawnPlayerAfterDeath;
    [SerializeField] private float gateTransitionSeconds;


    public string SceneWhereRespawn
    {
        get { return sceneWhereRespawn; }
        set { sceneWhereRespawn = value; }
    }
    public int IdRespawn
    {
        get { return idRespawn; }
        set {  idRespawn = value; }
    }
    public int IdSpawn
    {
        get { return idSpawn; }
        set { idSpawn = value; }
    }
    public string NextScene
    {
        get { return nextScene; }
        set { nextScene = value; }
    }
    public bool HasToSpawnPlayer
    {
        get { return hasToSpawnPlayer; }
        set { hasToSpawnPlayer = value; }
    }
    public bool HasToSpawnPlayerAfterDeath
    {
        get {return hasToSpawnPlayerAfterDeath; }
        set { hasToSpawnPlayerAfterDeath = value; }
    }
    public float GateTransitionSeconds
    {
        get { return gateTransitionSeconds; }
    }


    public void FinishScene(string sceneName, int newSpawnPoint)
    {
        nextScene = sceneName;
        idSpawn = newSpawnPoint;
        SpawnPlayer();
    }
    public void StartScene(string sceneName)
    {
        hasToSpawnPlayer = true;
        SceneManager.LoadScene(sceneName);
        
        
    }
    public void SpawnPlayer()
    {
        if (nextScene is null)
        {
            nextScene = SceneManager.GetActiveScene().name;
        }   
        if (SceneManager.GetActiveScene().name.Equals(nextScene))
        {
            SceneController sc = FindAnyObjectByType<SceneController>();
            if (sc is null)
            {
                Debug.LogError("SceneController no encontrado");
                return;
            }
            sc.SpawnPlayer();
        }
        else
        {            
            hasToSpawnPlayer = true;
            SceneManager.LoadScene(nextScene);
        }

    }

    public void RespawnPlayer()//poner nombre a respawn alpargata
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return null; 

        if (sceneWhereRespawn is not null)
        { 
            if (sceneWhereRespawn.Equals(""))
            { 
                SceneController sc = FindAnyObjectByType<SceneController>();
                if (sc is null)
                {
                    Debug.LogError("SceneController no encontrado");
                    //return;
                }
                sc.RespawnPlayer();
            }
            else
            {
                if (!SceneManager.GetActiveScene().name.Equals(sceneWhereRespawn))
                {
                    hasToSpawnPlayerAfterDeath = true;
                    SceneManager.LoadScene(sceneWhereRespawn);
                }
                else
                {
                    SceneController sc = FindAnyObjectByType<SceneController>();
                    if (sc is null)
                    {
                        Debug.LogError("SceneController no encontrado");
                        //return;
                    }
                    sc.RespawnPlayer();
                }
            }
        }
        else
        {
            SceneController sc = FindAnyObjectByType<SceneController>();
            if (sc is null)
            {
                Debug.LogError("SceneController no encontrado");
                //return;
            }
            sc.RespawnPlayer();
        }
    }

   


}
