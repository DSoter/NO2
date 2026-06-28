
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("Objetos en escena")]

    [SerializeField] private Transform currentSpawnPoint;
    private int currentSpawnPointAfterDeath = -1;
    public int currentSpawnPointId;
    [SerializeField] private GameObject prefabPersonaje;

    [Header("Sonidos")]
    [SerializeField] private AudioClip theme, deathSound;

    [Header("Transition")]
    [SerializeField] private TransitionController transition;

    [SerializeField] private bool spawnPlayerEditor;

    //Cosas jugador
    private GameObject player;
    private PlayerController _playerController;

    private CheckpointManager _checkpointManager;

    public List<Transform> checkpoints;
    private void Awake()
    {
        
    }
    private void Update()
    {
        if (spawnPlayerEditor)
        {
            spawnPlayerEditor = false;
            SpawnPlayer();
        }

    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 80, 20), "SpawnPlayer"))
        {
            spawnPlayerEditor=true;
        }

        if (GUI.Button(new Rect(160, 20, 80, 20), "DestroyPlayer"))
        {
            Destroy(player);
        }
    }
    void Start()
    {

        checkpoints = new List<Transform>();
        GameObject[] arrayAux = GameObject.FindGameObjectsWithTag("SpawnPoint");
        List<SpawnId> list = new List<SpawnId>();
        for (int i = 0; i < arrayAux.Length; i++)
        {
            list.Add(arrayAux[i].GetComponent<SpawnId>()); 
        }
        List <SpawnId> listaAux = list
            .OrderBy(c => c.IdSpawn)
            .ToList();
        for (int i = 0; i < listaAux.Count; i++)
        {
            checkpoints.Add(currentSpawnPoint);
            checkpoints[i] = (listaAux[i].transform);
        }
        currentSpawnPointId = checkpoints.IndexOf(currentSpawnPoint);
        if (currentSpawnPointId < 0 || currentSpawnPointId>checkpoints.Count) { currentSpawnPointId = checkpoints.IndexOf(currentSpawnPoint); }
        _checkpointManager = GameManager.Instance.GetComponent<CheckpointManager>();


        if (_checkpointManager.HasToSpawnPlayer)
        {
            _checkpointManager.HasToSpawnPlayer = false;
            SpawnPlayer();
            
        }
        if (_checkpointManager.HasToSpawnPlayerAfterDeath)
        {
            _checkpointManager.HasToSpawnPlayerAfterDeath = false;
            RespawnPlayer();
            
        }

        if (theme != null)
        {
            GameManager.Instance.audioManager.PlayMusic(theme);
        }

    }
    public void SpawnPlayer()
    {
        Debug.Log($"ExitGateDirection: {_checkpointManager.ExitGateDirection}");
        Debug.Log($"IdSpawn: {_checkpointManager.IdSpawn}");
        if (_checkpointManager.IdSpawn > 0)
        {
            currentSpawnPointId = _checkpointManager.IdSpawn; 
        }//este if solo ocurre si el spawn es a un transform asignado por un tp

        currentSpawnPoint = checkpoints[currentSpawnPointId];

        player = Instantiate<GameObject>(prefabPersonaje);
        _checkpointManager.PlayerReference = player.transform;
        _playerController = player.GetComponent<PlayerController>();
        player.transform.position = currentSpawnPoint.transform.position;

        if (_checkpointManager.IdSpawn > 0) //entrar desde un spawn
        {
            Vector3 exitDirection = new Vector3(_checkpointManager.ExitGateDirection.x, _checkpointManager.ExitGateDirection.y, 0); 
            _playerController.ExitScene(exitDirection,0.5f);

            TransitionController tc = FindAnyObjectByType<TransitionController>();
            if (tc != null)
            {
                tc.StartTransition(exitDirection, false);
            }

        }
        _checkpointManager.IdSpawn = -1;
    }

    public void RespawnPlayer()
    {

        _checkpointManager.IdSpawn = -1;
        currentSpawnPoint = checkpoints[currentSpawnPointId];
        _checkpointManager.NextScene= SceneManager.GetActiveScene().name;

        if (_checkpointManager.IdRespawn < 0)
        {

            currentSpawnPointAfterDeath = checkpoints.IndexOf(currentSpawnPoint);
        }
        else
        {
            if (_checkpointManager.SceneWhereRespawn is null) {
                Debug.Log("No hay escena donde respawnear");
            }
            else { 
                if (CheckIsActiveScene(_checkpointManager.SceneWhereRespawn))
                {
                    currentSpawnPointAfterDeath = _checkpointManager.IdRespawn;
                    if (currentSpawnPointAfterDeath >= checkpoints.Count) 
                    {
                        Debug.Log("xd"); 
                    }
                    else { 
                        currentSpawnPoint = checkpoints[currentSpawnPointAfterDeath]; 
                    }
                        
                }
            }
        }
        player = Instantiate<GameObject>(prefabPersonaje);
        _checkpointManager.PlayerReference = player.transform;
        _playerController = player.GetComponent<PlayerController>();
        player.transform.position = currentSpawnPoint.transform.position;
    }

    private bool CheckIsActiveScene(string scene)
    {
        return (SceneManager.GetActiveScene().name.Equals(scene));
    }

    public void UpdateSpawnPoint(Transform newSpawn)
    {
        currentSpawnPoint = newSpawn;
        currentSpawnPointId = checkpoints.IndexOf(newSpawn);
    }
    public void UpdateSpawnPointAfterDeath(Transform newSpawn)
    {
        currentSpawnPointAfterDeath = checkpoints.IndexOf(newSpawn);
    }
    public int obtainIdSpawnPoint(Transform spawn)
    {
        return checkpoints.IndexOf(spawn);
    }
    public string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }


}
