using UnityEngine;

public class DamageArea : MonoBehaviour
{
    [SerializeField] private float damageForSeconds;


    private PlayerController playerController;
    private bool playerOnZone;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController = collision.GetComponent<PlayerController>();

            playerOnZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerController = collision.GetComponent<PlayerController>();

            playerOnZone = false;
        }
    }
    private void Update()
    {
        if (playerOnZone)
        {
            playerController._PlayerData.Health = playerController._PlayerData.Health - damageForSeconds * Time.deltaTime;
        }
    }

}
