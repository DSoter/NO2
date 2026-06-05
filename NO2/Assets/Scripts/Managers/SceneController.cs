
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("Objetos en escena")]
    [SerializeField] private Transform currentSpawnPoint;
    [SerializeField] private GameObject prefabPersonaje;

    [Header("Sonidos")]
    [SerializeField] private AudioClip theme, deathSound;

    //Cosas jugador
    private GameObject player;
    private PlayerController _playerController;

    private CheckpointManager _checkpointManager;

    void Start()
    {
        _checkpointManager = GameManager.Instance.GetComponent<CheckpointManager>();
        if (_checkpointManager.GetHasToSpawn())
        {
            SpawnPlayer();
            _checkpointManager.SetHasToSpawn(false);
        }
        if (_checkpointManager.GetHasToSpawnAfterDeath())
        {
            SpawnPlayerAfterDeath();
            _checkpointManager.SetHasToSpawnAfterDeath(false);
        }

        if (theme != null)
        {
            GameManager.Instance.audioManager.PlayMusic(theme);
        }

    }
    public void SpawnPlayer()
    {
        player = Instantiate<GameObject>(prefabPersonaje);
        _playerController = player.GetComponent<PlayerController>();
        player.transform.position = currentSpawnPoint.transform.position;
    }

    public void SpawnPlayerAfterDeath()
    {
        if (_checkpointManager.GetCurrentSpawnPoint() is null)
        {
            _checkpointManager.SetCurrentSpawnPoint(currentSpawnPoint);
        }
        else
        {
            if (SceneManager.GetActiveScene().name.Equals(_checkpointManager.GetCurrentSceneName()))
            {
                currentSpawnPoint = _checkpointManager.GetCurrentSpawnPoint();
            }
        }
        player = Instantiate<GameObject>(prefabPersonaje);
        _playerController = player.GetComponent<PlayerController>();
        player.transform.position = currentSpawnPoint.transform.position;
    }



    public void UpdateSpawnPoint(Transform newSpawn)
    {
        currentSpawnPoint = newSpawn;
    }

    public string GetSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

}
