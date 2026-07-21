using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    private string sceneWhereRespawn;
    private int idRespawn;
    private int idSpawn;
    private string nextScene;
    private bool hasToSpawnPlayer;
    private bool hasToSpawnPlayerAfterDeath;
    private TilemapToScriptable tilemapToScriptable;

    private Transform playerReference;

    private Vector2 enterGateDirection;
    private Vector2 exitGateDirection;
    [SerializeField] private float gateTransitionSeconds = 0.5f;


    private SceneController sc;
    private DiverseMenusManager _menusManager;
    private bool managerPaused;

    private bool cameraLockedPlayer;

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
    public Transform PlayerReference
    {
        get { return playerReference; }
        set { playerReference = value; }
    }
    

    public TilemapToScriptable TilemapToScriptable
    {
        get { return tilemapToScriptable; }
        set { tilemapToScriptable = value; }
    }
    public Vector2 ExitGateDirection
    {
        get { return exitGateDirection; }
        set { exitGateDirection = value; }
    }
    public Vector2 EnterGateDirection
    {
        get { return enterGateDirection; }
        set { enterGateDirection = value; }
    }
    public bool ManagerPaused
    {
        get { return managerPaused; }
        set { managerPaused = value; }
    }

    public bool CameraLockedPlayer
    {
        get { return  cameraLockedPlayer; }
        set { cameraLockedPlayer = value; }
    }


    public void GoNextScene(string sceneName, int newSpawnPoint)
    {
        nextScene = sceneName;
        idSpawn = newSpawnPoint;
        StartCoroutine(NextSceneCoroutine(gateTransitionSeconds));
        
    }
    private IEnumerator NextSceneCoroutine(float waitDurationSeconds)
    {
        TransitionController tc = FindAnyObjectByType<TransitionController>();
        if (tc != null)
        {
            tc.StartTransition(enterGateDirection, true);
        }


        PlayerController playerScript= playerReference.gameObject.GetComponent<PlayerController>();
        playerScript.ExitScene(enterGateDirection * (-1), waitDurationSeconds);


        yield return new WaitForSeconds(waitDurationSeconds);

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
        if (CheckIsActiveScene(nextScene))
        {
            if (ExistsSceneController())
            {
                sc.SpawnPlayer();
            }
        }
        else
        {            
            hasToSpawnPlayer = true;
            SceneManager.LoadScene(nextScene);
        }

    }

    public void RespawnPlayer()
    {
        Debug.Log(idRespawn);
        StartCoroutine(RespawnCoroutine());
    }
    public void RespawnPlayerAfterNoOxygen()
    {
        Debug.Log(idRespawn);
        StartCoroutine(RespawnAfterNoOxygenCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return null;

        OpenGameOver();

        

        
    }
    private IEnumerator RespawnAfterNoOxygenCoroutine()
    {
        yield return null;

        OpenGameOverWithoutFadeIn();




    }




    public void RespawnAfterGameOver()
    {

        _menusManager.OpenMenus();//Para cerrar el menú

        if (sceneWhereRespawn != null)
        {
            if (sceneWhereRespawn.Equals(""))
            {
                if (ExistsSceneController())
                {
                    sc.RespawnPlayer();
                }
            }
            else
            {
                if (!CheckIsActiveScene(sceneWhereRespawn))
                {
                    hasToSpawnPlayerAfterDeath = true;
                    SceneManager.LoadScene(sceneWhereRespawn);
                }
                else
                {
                    if (ExistsSceneController())
                    {
                        sc.RespawnPlayer();
                    }
                }
            }
        }
        else
        {
            if (ExistsSceneController())
            {
                sc.RespawnPlayer();
            }
        }
    }
    private void OpenGameOver()
    {
        managerPaused = true;
        _menusManager = GameManager.Instance.GetComponent<DiverseMenusManager>();
        _menusManager.SetMenuType(DiverseMenusManager.MenuType.Gameover);
        _menusManager.OpenMenus();
    }

    private void OpenGameOverWithoutFadeIn()
    {
        managerPaused = true;
        _menusManager = GameManager.Instance.GetComponent<DiverseMenusManager>();
        _menusManager.SetMenuType(DiverseMenusManager.MenuType.Gameover);
        _menusManager.GameOverWithoutFadeIn = true;
        Debug.Log("Game over without fade in");
        _menusManager.OpenMenus();
    }

    private bool CheckIsActiveScene(string scene)
    {
        return (SceneManager.GetActiveScene().name.Equals(scene));
    }
    private bool ExistsSceneController()
    {
        sc = FindAnyObjectByType<SceneController>();
        if (sc == null)
        {
            Debug.LogError("SceneController no encontrado");
            return false;
        }
        return true;
    }

}
