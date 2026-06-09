
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

    //Cosas jugador
    private GameObject player;
    private PlayerController _playerController;

    private CheckpointManager _checkpointManager;

    public List<Transform> checkpoints;
    private void Awake()
    {
        
    }
    void Start()
    {

        checkpoints = new List<Transform>();
        List<SpawnId> listaAux = FindObjectsByType<SpawnId>()
            .OrderBy(c => c.spawnId)
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
            _checkpointManager.HasToSpawnPlayer =false;
            SpawnPlayer();
            
        }
        if (_checkpointManager.HasToSpawnPlayerAfterDeath)
        {
            _checkpointManager.HasToSpawnPlayerAfterDeath = false;
            SpawnPlayerAfterDeath();
            
        }

        if (theme != null)
        {
            GameManager.Instance.audioManager.PlayMusic(theme);
        }

    }
    public void SpawnPlayer()
    {
        if (_checkpointManager.IdSpawn >0) { currentSpawnPointId = _checkpointManager.IdSpawn; }//este if solo ocurre si el spawn es a un transform asignado por un tp
        currentSpawnPoint = checkpoints[currentSpawnPointId];
        player = Instantiate<GameObject>(prefabPersonaje);
        _playerController = player.GetComponent<PlayerController>();
        player.transform.position = currentSpawnPoint.transform.position;
        if (_checkpointManager.IdSpawn > 0)
        {
            player.transform.position -= new Vector3(1,0,0);
        }
        _checkpointManager.IdSpawn = -1;
    }

    public void SpawnPlayerAfterDeath()
    {

        _checkpointManager.IdSpawn = -1;
        currentSpawnPoint = checkpoints[currentSpawnPointId];
        _checkpointManager.CurrentSceneName= SceneManager.GetActiveScene().name;

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
                if (SceneManager.GetActiveScene().name.Equals(_checkpointManager.SceneWhereRespawn))
                {
                    currentSpawnPointAfterDeath = _checkpointManager.IdRespawn;
                    if (currentSpawnPointAfterDeath >= checkpoints.Count) { Debug.Log("xd"); }
                    else { currentSpawnPoint = checkpoints[currentSpawnPointAfterDeath]; }
                        
                }
            }
        }
        player = Instantiate<GameObject>(prefabPersonaje);
        _playerController = player.GetComponent<PlayerController>();
        player.transform.position = currentSpawnPoint.transform.position;
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
