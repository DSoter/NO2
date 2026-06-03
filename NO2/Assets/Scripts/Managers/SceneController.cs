
using UnityEngine;
using System.Collections;

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
    private void SpawnPlayer()
    {
        player = Instantiate<GameObject>(prefabPersonaje);
        _playerController = player.GetComponent<PlayerController>();
        player.transform.position = currentSpawnPoint.transform.position;
    }


    public void KillPlayer()//and respawn it
    {
        if (deathSound != null)
            GameManager.Instance.audioManager.PlaySound(deathSound);
        StartCoroutine(WaitAndKill(0.5f));
    }
    IEnumerator WaitAndKill(float segundos)
    {
        _playerController.SetDead(true);
        player.GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(segundos);

        _playerController.SetDead(false);
        Destroy(player);
        SpawnPlayer();
    }



    public void UpdateSpawnPoint(Transform newSpawn)
    {
        currentSpawnPoint = newSpawn;
    }

}
