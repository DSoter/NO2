using System;
using Unity.VisualScripting;
using UnityEngine;

public class PickupPullArea : MonoBehaviour
{
    public event Action<Transform> OnPlayerEnteredPullArea;

    [SerializeField] private CircleCollider2D _collider;
    [SerializeField] private LayerMask _playerLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _playerLayer) != 0)
        {
            Debug.Log("Player Entered Pull Area");
            OnPlayerEnteredPullArea?.Invoke(collision.GetComponent<PlayerController>().Center);
            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _collider.radius);
    }

}
