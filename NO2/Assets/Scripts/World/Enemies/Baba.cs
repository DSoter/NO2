using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Baba : Enemy
{
    [Header("Patroll")] 
    [SerializeField] private Transform[] _patrollPoints;
    private int _currentPatrollPoint = 0;

    [Space(5)]
    [Header("Detection Area")]
    [SerializeField] private DetectionArea _detectionArea;

    [Space(5)]
    [Header("Charge")]
    [SerializeField] [Range(0,1)] private float _targetingProportion = 0.8f;

    [Space(5)]
    [Header("Attack")]
    [SerializeField] private float _attackImpulse = 10f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _knockbackForce = 7f;
    [SerializeField] private float _attackSeconds = 1f;
    [SerializeField] private float _cooldownSeconds = 2f;
    private float _cooldownTimer;
    private Vector2 _attackDirection;

    [Space(5)]
    [Header("Ranges")]
    [SerializeField] private float _backToPatrollRange = 20f;
    [SerializeField] private float _attackRange = 2f;

    [Space(5)]
    [Header("Layer Mask")]
    [SerializeField] private LayerMask _playerLayer;

    private PlayerController _player;

    private NavMeshAgent _agent;
    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private BabaState _state = BabaState.Patroll;
    private enum BabaState
    {
        Patroll,
        Chase,
        Charge,
        Attack,
        Hurt
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _cooldownTimer = _cooldownSeconds;
    }

    void Start()
    {
        _detectionArea.OnPlayerDetected += StartChasing;
    }

    void Update()
    {
        switch (_state)
        {
            case BabaState.Patroll:

                _cooldownTimer -= Time.deltaTime;

                if (_patrollPoints.Length == 0) break;

                Vector3 targetPosition = _patrollPoints[_currentPatrollPoint].position;

                CheckIfShouldFlip(targetPosition);
                _agent.SetDestination(targetPosition);

                // If close enought to the current patroll point, move to the next one
                if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
                {
                    _currentPatrollPoint = (_currentPatrollPoint + 1) % _patrollPoints.Length;
                }

                break;

            case BabaState.Chase:

                _cooldownTimer -= Time.deltaTime;

                CheckIfShouldFlip(_player.transform.position);
                _agent.SetDestination(_player.transform.position);

                if (Vector3.Distance(transform.position, _player.transform.position) > _backToPatrollRange)
                {
                    Debug.Log("Baba - Player is too far away, going back to patrolling");
                    _state = BabaState.Patroll;
                }
                else if (Vector3.Distance(transform.position, _player.transform.position) < _attackRange && _cooldownTimer <= 0)
                {
                    Debug.Log("Baba - Player is in attack range");
                    _state = BabaState.Charge;
                    _agent.enabled = false;
                    _animator.SetTrigger("Attack");
                }

                break;

            case BabaState.Charge:
                if(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < _targetingProportion)
                {
                    CheckIfShouldFlip(_player.transform.position);
                    _attackDirection = (_player.transform.position - transform.position).normalized;
                }
                break;
        }
    }


    public void OnAttackStartedAnimationEvent()
    {
        _cooldownTimer = _cooldownSeconds;

        _state = BabaState.Attack;
        _rigidbody.AddForce(_attackDirection * _attackImpulse, ForceMode2D.Impulse);  
        StartCoroutine(AttackTimerCoroutine());
    }

    private IEnumerator AttackTimerCoroutine()
    {
        yield return new WaitForSeconds(_attackSeconds);

        _animator.SetTrigger("EndAttack");
        _state = BabaState.Chase;
        _agent.enabled = true;
    }

    private void CheckIfShouldFlip(Vector3 target)
    {
        if (transform.position.x - target.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
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

        // Vulnetability Check: If the Baba is in Charge state while receiving a strong attack, it takes double damage
        if (_state == BabaState.Charge && strength == AttackStrength.Strong)
        {
            Debug.Log("Baba - Vulnerable Damage Taken: " + damage * 2);
            _healthPoints -= damage * 2;
        }
        else
        {
            Debug.Log("Baba - Damage Taken: " + damage);
            _healthPoints -= damage;
        }


        // Apply Hit Effects and Check for Death
        if (_healthPoints <= 0)
        {
            SpawnContent(direction);
            Destroy(gameObject);
        }
        else
        {
            StopAllCoroutines();

            _state = BabaState.Hurt;
            _animator.SetTrigger("Hurt");

            _agent.enabled = false;

            _rigidbody.linearVelocity = Vector2.zero;
            _rigidbody.AddForce(direction * 2, ForceMode2D.Impulse);
        }
    }

    public void OnHurtEndedAnimationEvent()
    {
        _agent.enabled = true;

        if(_player != null)
        {
            _state = BabaState.Chase;
        }
        else
        {
            _state = BabaState.Patroll;
        }
    }

    public void StartChasing(PlayerController player)
    {
        if (_state == BabaState.Patroll)
        {
            Debug.Log("Baba - Player detected, starting to chase");

            _state = BabaState.Chase;
            _player = player;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _backToPatrollRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
