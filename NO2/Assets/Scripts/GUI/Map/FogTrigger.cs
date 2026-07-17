using UnityEngine;

public class FogTrigger : MonoBehaviour
{
    [SerializeField] private GameObject fogImage;
    [SerializeField] private GameObject fogCollider;

    public void OnChildTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);

        }
    }
}
