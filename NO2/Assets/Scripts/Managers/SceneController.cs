
using UnityEngine;
using System.Collections;

public class SceneController : MonoBehaviour
{
    [Header("Objetos en escena")]
    [SerializeField] private Transform currentSpawnPoint;
    [SerializeField] private GameObject prefabPersonaje;

    [Header("Sonidos")]
    [SerializeField] private AudioClip theme, deathSound;


    private GameObject player;

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
        //playerController.SetCanMove(false);
        player.GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(segundos);

        //playerController.SetCanMove(true);
        Destroy(player);
        SpawnPlayer();
    }



    public void UpdateSpawnPoint(Transform newSpawn)
    {
        currentSpawnPoint = newSpawn;
    }

}
