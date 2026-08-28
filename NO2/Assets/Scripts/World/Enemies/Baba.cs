using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class Baba : Enemy
{
    [Header("Patroll")] 
    [SerializeField] private Transform[] _patrollPoints;
    private int _currentPatrollPoint = 0;

    [Space(5)]
    [Header("ScriptReferences")]
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
    [Header("On Hit")]
    [SerializeField] private float _disabledSecondsOnHit = 0.5f;
    [SerializeField] private float _knockbackAmountOnHit = 5f;
    [SerializeField] private Color _burnColor;
    [SerializeField] private Color _severeBurnColor;

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
    private SpriteRenderer _renderer;
    private CinemachineImpulseSource _impulseSource;
    private SpriteFlash _spriteFlash;

    private Coroutine _burnCoroutine;
    private Coroutine _severeBurnCoroutine;

    public override bool IsVulnerable => _state == BabaState.Charge;

    private BabaState _state = BabaState.Patroll;
    private enum BabaState
    {
        Patroll,
        Chase,
        Charge,
        Attack
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _spriteFlash = GetComponent<SpriteFlash>();

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

    private void LateUpdate()
    {
        if (transform.position.z != 0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
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
        base.Hit(direction, damage, strength);

        // Apply Hit Effects and Check for Death

        Debug.Log("Baba HP: " + _healthPoints);

        _impulseSource.GenerateImpulse();

        if (_healthPoints <= 0)
        {
            SpawnContent(direction);
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            _spriteFlash.ApplyEffect();
            _rigidbody.AddForce(direction * _knockbackAmountOnHit, ForceMode2D.Impulse);
        }

        //cosas insignia carga grande
        if (_player.HasToUnlockBadgeBigCharge)
        {
            Debug.Log("se ha conseguido el logro");
            GameManager.Instance.GetComponent<AchievementManager>().NotifyEvent("big_charge");
            _player.HasToUnlockBadgeBigCharge=false;
        }
        
    }

    public override void ApplyBurn(float damagePerSecond, int duration)
    {
        if (_severeBurnCoroutine != null) { return; } // Severe burn has priority over basic burn

        if(_burnCoroutine != null)
        {
            StopCoroutine( _burnCoroutine );
        }

        _burnCoroutine = StartCoroutine(BurnCoroutine(damagePerSecond, duration));
    }

    public override void ApplySevereBurn(float damagePerSecond, int duration)
    {
        if(_burnCoroutine != null)
        {
            StopCoroutine( _burnCoroutine );
        }

        if(_severeBurnCoroutine != null)
        {
            StopCoroutine(_severeBurnCoroutine);
        }

        _severeBurnCoroutine = StartCoroutine(SevereBurnCoroutine(damagePerSecond, duration));

    }

    private IEnumerator BurnCoroutine(float damagePerSecond, int duration)
    {
        int burnCount = 0;

        _renderer.color = _burnColor;


        while (burnCount < duration)
        {
            yield return new WaitForSeconds(1);
            Hit(Vector2.zero, damagePerSecond, AttackStrength.Weak);
            burnCount++;
        }

        _renderer.color = Color.white;
        _burnCoroutine = null;
    }

    private IEnumerator SevereBurnCoroutine(float damagePerSecond, int duration)
    {
        int burnCount = 0;

        _renderer.color = _severeBurnColor;


        while (burnCount < duration)
        {
            yield return new WaitForSeconds(1);
            Hit(Vector2.zero, damagePerSecond, AttackStrength.Weak);
            burnCount++;
        }

        _renderer.color = Color.white;
        _severeBurnCoroutine = null;
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
