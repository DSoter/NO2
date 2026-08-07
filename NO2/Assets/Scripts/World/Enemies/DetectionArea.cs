using System;
using UnityEngine;

public class DetectionArea : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private CircleCollider2D _collider;

    public event Action<PlayerController> OnPlayerDetected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _playerLayer) != 0)
        {
            if (collision.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                OnPlayerDetected?.Invoke(player);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _collider.radius);
    }
}
