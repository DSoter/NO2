using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Baba : Enemy
{
    [Header("Target")] 
    [SerializeField] private Transform _target;

    [Space(5)]
    [Header("Layer Mask")]
    [SerializeField] private LayerMask _playerLayer;

    [Space(5)]
    [Header("Stats")]
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _knockbackForce = 7f;

    private PlayerController _player;

    private SpriteRenderer _renderer;
    private NavMeshAgent _agent;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _renderer = GetComponent<SpriteRenderer>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;
    }

    void Update()
    {
        if(transform.position.x - _target.position.x > 0)
        {
            _renderer.flipX = true;
        }
        else
        {
            _renderer.flipX = false;
        }

        _agent.SetDestination(_target.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & _playerLayer) != 0)
        {

            if (collision.gameObject.TryGetComponent<PlayerController>(out _player))
            {
                Debug.Log("Baba - Collided with Player"); 
                Vector2 knockbackDirection = (collision.transform.position - transform.position);
                _player.TakeDamage(_damage, knockbackDirection, _knockbackForce);
            }
        }    
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & _playerLayer) != 0)
        {
            if (collision.gameObject == _player.gameObject) 
            {
                Debug.Log("Baba - Collided with Player");
                Vector2 knockbackDirection = (collision.transform.position - transform.position);
                _player.TakeDamage(_damage, knockbackDirection, _knockbackForce);
            }
        }
    }

    public override void Hit(Vector2 direction, float damage, AttackStrength strength)
    {
        Debug.Log("Baba - Hit");
    }
}
