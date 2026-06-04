
using UnityEngine;


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

    void Start()
    {
        if (theme != null)
        {
            GameManager.Instance.audioManager.PlayMusic(theme);
        }
        SpawnPlayer();

    }
    public void SpawnPlayer()
    {
        player = Instantiate<GameObject>(prefabPersonaje);
        _playerController = player.GetComponent<PlayerController>();
        player.transform.position = currentSpawnPoint.transform.position;
    }


    



    public void UpdateSpawnPoint(Transform newSpawn)
    {
        currentSpawnPoint = newSpawn;
    }

}
