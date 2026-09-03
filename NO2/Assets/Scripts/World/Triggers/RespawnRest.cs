using UnityEngine;

public class RespawnRest : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GetComponent<CheckpointManager>().ResetAllRestRespawns();
        }
    }
}
