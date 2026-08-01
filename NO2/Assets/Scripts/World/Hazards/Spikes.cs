using UnityEngine;

public class Spikes : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _knockbackForce = 5f;

    [Space(5)]
    [Header("Layer Mask")]
    [SerializeField] private LayerMask _playerLayer;

    private PlayerController _player;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _playerLayer) != 0)
        {
            if (collision.TryGetComponent<PlayerController>(out _player))
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position);
                _player.TakeDamage(_damage, knockbackDirection, _knockbackForce);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _playerLayer) != 0)
        {
            Vector2 knockbackDirection = (collision.transform.position - transform.position);
            _player.TakeDamage(_damage, knockbackDirection, _knockbackForce);
        }
    }
}
