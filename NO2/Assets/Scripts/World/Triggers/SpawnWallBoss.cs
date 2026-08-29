using UnityEngine;

public class SpawnWallBoss : MonoBehaviour
{
    [SerializeField] private GameObject wall;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            wall.SetActive(true);
        }
    }
}
