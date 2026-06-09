using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private string sceneWhereRespawn;
    public int idRespawn;
    public int idSpawn;
    private string currentSceneName;
    private bool hasToSpawnPlayer;
    private bool hasToSpawnPlayerAfterDeath;
    private static int debugCounter;


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
    public string CurrentSceneName
    {
        get { return currentSceneName; }
        set { currentSceneName = value; }
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


    public void FinishScene(string sceneName, int newSpawnPoint)
    {
        currentSceneName= sceneName;
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
        if (currentSceneName is null) {
            currentSceneName = SceneManager.GetActiveScene().name;
        }
        if (!(SceneManager.GetActiveScene().name.Equals(currentSceneName))) {
            hasToSpawnPlayer = true;
            SceneManager.LoadScene(currentSceneName);
        }
        else { 
            SceneController sc = FindAnyObjectByType<SceneController>();
            if (sc is null)
            {
                Debug.LogError("SceneController no encontrado");
                return;
            }
            sc.SpawnPlayer();
        }

    }

    public void SpawnPlayerAfterDeath()
    {
        StartCoroutine(SpawnAfterDeathCoroutine());
    }

    private IEnumerator SpawnAfterDeathCoroutine()
    {
        yield return null; 

        if (sceneWhereRespawn is not null)
        { if (sceneWhereRespawn.Equals("")) { sceneWhereRespawn = null; } }

        if (sceneWhereRespawn is not null)
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
                sc.SpawnPlayerAfterDeath();
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
            sc.SpawnPlayerAfterDeath();
        }
    }
    //public void SpawnPlayerAfterDeath()
    //{
    //    if (sceneWhereRespawn is not null)
    //    { if (sceneWhereRespawn.Equals("")) { sceneWhereRespawn = null; } }

    //    if (sceneWhereRespawn is not null) {
    //        if (!SceneManager.GetActiveScene().name.Equals(sceneWhereRespawn))
    //        {
    //            hasToSpawnPlayerAfterDeath = true;
    //            File.AppendAllText("C:/Temp/debug.txt",
    //$"LoadScene llamado: {sceneWhereRespawn} | StackTrace: {System.Environment.StackTrace}\n");
    //            SceneManager.LoadScene(sceneWhereRespawn);
    //        }
    //        else
    //        {
    //            SceneController sc = FindAnyObjectByType<SceneController>();
    //            if (sc is null)
    //            {
    //                Debug.LogError("SceneController no encontrado");
    //                return;
    //            }
    //            sc.SpawnPlayerAfterDeath();
    //        }
    //    }
    //    else
    //    {
    //        SceneController sc = FindAnyObjectByType<SceneController>();
    //        if (sc is null)
    //        {
    //            Debug.LogError("SceneController no encontrado");
    //            return;
    //        }
    //        sc.SpawnPlayerAfterDeath();
    //    }

    //}
    public void UpdateSpawnPointAfterDeath(Transform transform)
    {
        if (currentSceneName is null) { return; }
        else
        {
            SceneController sc = FindAnyObjectByType<SceneController>();
            if (sc is null)
            {
                Debug.LogError("SceneController no encontrado");

                return;
            }
            idRespawn = sc.obtainIdSpawnPoint(transform);
        }
    }


}
