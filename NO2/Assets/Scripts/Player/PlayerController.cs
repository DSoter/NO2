using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    // Gameobject Componentes
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _renderer;
    private Collider2D _collider;

    // Local Variables
    private Vector2 _moveDirection = new Vector2(0, 0);
    private Vector2 _lookDirection = new Vector2(1, 0);
    private float _staminaRegenTimer = 0;
    private int _attackCounter = 0;
    private bool _areInputsEnabled = true;
    private bool _dialogIsOpen = false;
    private bool _isOnOxigenZone = false;
    private float _chargeTime = 0f;
    private bool _chargeReleased = false;

    private bool _isPause => Time.timeScale == 0;
    private bool _isRunning = false;
    private bool canRoll => _state == PlayerState.Move && HasStamina();
    private bool canAttack => _state == PlayerState.Move && HasStamina();
    private bool canCharge => _state == PlayerState.Move && HasStamina();

    private InputManager _inputManager;

    private PlayerState _state = PlayerState.Move;

    // References
    [Header("Sonidos")]
    [SerializeField] private AudioClip deathSound;

    [Space(5)]
    [Header("Scriptable Objects")]
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private OxygenData _oxygenData;
    [SerializeField] private HealData _healData;

    [Space(5)]
    [Header("Attacks")]
    [SerializeField] private WeakAttackController _weakAttack;
    [SerializeField] private StrongAttackController _strongAttack;

    [Space(5)]
    [Header("Oxygen Alerts")]
    [SerializeField] private AudioClip _oxygenAlert50;
    [SerializeField] private AudioClip _oxygenAlert10;

    [Space(5)]
    [Header("Center")]
    [SerializeField] private Transform _center;

    // MOVE TO PLAYER DATA (COMPLETAR)
    [Space(5)]
    [Header("Animation durations")]
    [SerializeField] private float deathAnimationSeconds = 2f;
    [SerializeField] private float _acceleration = 25f;
    [SerializeField] private float _attackImpulse = 1f;

    [Space(5)]
    [Header("Charge Settings")]
    [SerializeField] private float _minCharge = 0.5f;
    [SerializeField] private float _maxCharge = 1.5f;

    public Transform Center
    {
        get { return _center; }
    }

    public PlayerData _PlayerData
    {
        get { return _playerData; }
    }

    public enum PlayerState
    {
        Move,
        Roll,
        Dead,
        WeakAttack,
        Charging,
        StrongAttack,
        Rest
    }

    public PlayerState GetState()
    {
        return _state;
    }

    public void SetState(PlayerState state)
    {
        _state = state;
    }


    // -------------------------------------------------------------------------
    // Unity Lifecycle
    // -------------------------------------------------------------------------

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();

        _inputManager = GameManager.Instance.gameObject.GetComponent<InputManager>();

        _playerData.Stamina = _playerData.MaxStamina;
        _oxygenData.LastOxygenSeconds = _playerData.LastOxygenSeconds;
    }

    private void Start()
    {
        _playerData.OnOxygenIncreased += HandleOnOxygenIncreased;
        _oxygenData.LastOxygenTimer = 0;
    }

    private void Update()
    {
        if (_isPause)
        {
            return;
        }

        HandleAnimatorParams();
        HandleOxigen();
        HandleCooldownPotion();

        switch (_state)
        {
            case PlayerState.Move:

                UpdateLookDirection();
                HandleStaminaRegeneration();

                HandleHealthStatus();

                break;
            case PlayerState.Roll:

                break;
            case PlayerState.WeakAttack:

                HandleHealthStatus();
                break;
            case PlayerState.Charging:

                UpdateLookDirectionWithMouse();

                _chargeTime = Mathf.Min(_chargeTime + Time.deltaTime, _maxCharge);

                if (_chargeTime >= _minCharge && _chargeReleased)
                {
                    _animator.SetTrigger("StrongAttack");

                    StartCoroutine(StrongAttackCoroutine());

                    _state = PlayerState.StrongAttack;
                    _chargeReleased = false;
                    _chargeTime = 0f;
                }

                break;
            case PlayerState.StrongAttack:

                break;
            case PlayerState.Dead:

                break;
            case PlayerState.Rest:
                HandleHealthRegeneration();
                HandleStaminaRegeneration();

                HandleHealthStatus();
                _isRunning = false;
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (_state)
        {
            case PlayerState.Move:

                if (_isRunning && HasStamina())
                {
                    Run();
                }
                else
                {
                    Walk();
                }

                break;
            case PlayerState.Roll:

                break;
            case PlayerState.WeakAttack:

                break;
            case PlayerState.Charging:

                break;
            case PlayerState.Dead:

                _rigidbody.linearVelocity = Vector2.zero;
                break;
            case PlayerState.Rest:
                _rigidbody.linearVelocity = Vector2.zero;
                break;
        }
    }

    void OnDestroy()
    {
        DisposeActions();
    }

    void OnEnable()
    {
        if (_inputManager == null) return;
        EnableActions();
    }

    void OnDisable()
    {
        DisposeActions();
    }


    // -------------------------------------------------------------------------
    // Movement
    // -------------------------------------------------------------------------

    private void Run()
    {
        if (_moveDirection.magnitude > 0)
        {
            ConsumeStamina(_playerData.RunningStaminaCost * Time.deltaTime);
        }

        ApplyMovementForce(_playerData.RunningSpeed);
    }

    private void Walk()
    {
        ApplyMovementForce(_playerData.WalkingSpeed);
    }

    private void ApplyMovementForce(float targetSpeed)
    {
        Vector2 targetVelocity = targetSpeed * _moveDirection;
        Vector2 velocityChange = targetVelocity - _rigidbody.linearVelocity;

        _rigidbody.AddForce(velocityChange * _acceleration, ForceMode2D.Force);
    }

    private void SetVelocityInstant(Vector2 targetVelocity)
    {
        Vector2 velocityChange = targetVelocity - _rigidbody.linearVelocity;
        _rigidbody.AddForce(velocityChange, ForceMode2D.Impulse);
    }

    private void UpdateLookDirection()
    {
        if (_moveDirection.magnitude > 0)
        {
            _lookDirection = _moveDirection.normalized;
        }

        _renderer.flipX = (_lookDirection.x < 0);
    }

    private void UpdateLookDirectionWithMouse()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        _lookDirection = (mousePos - transform.position).normalized;

        _renderer.flipX = (_lookDirection.x < 0);
    }

    private Vector2 SnapToEightDirections(Vector2 direction)
    {
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float snapped = Mathf.Round(angle / 45f) * 45f;
        float rad = snapped * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }


    // -------------------------------------------------------------------------
    // Combat
    // -------------------------------------------------------------------------

    private void WeakAttack()
    {
        if (canAttack && _areInputsEnabled && !_isPause && !_dialogIsOpen)
        {
            StartCoroutine(WeakAttackCoroutine());
        }
    }

    private IEnumerator WeakAttackCoroutine()
    {
        _state = PlayerState.WeakAttack;

        // Calculate the attack direction based on the mouse position
        UpdateLookDirectionWithMouse();

        // Apply small force with the attack direction
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.AddForce(_lookDirection * _attackImpulse, ForceMode2D.Impulse);

        // Set the attack direction in the animator
        _animator.SetFloat("xDir", _lookDirection.x);
        _animator.SetFloat("yDir", _lookDirection.y);

        // Play attack animation
        bool isAttackFlipped = (_attackCounter % 2) == 1;
        _weakAttack.Play(isAttackFlipped);
        _attackCounter++;

        // Consume stamina
        ConsumeStamina(_playerData.WeakAttackStaminaCost);

        yield return new WaitForSeconds(_playerData.WeakAttackSeconds);

        _state = PlayerState.Move;
    }

    private void OnStrongAttackPerformed()
    {
        if (canCharge && !_isPause)
        {
            _state = PlayerState.Charging;
        }
    }

    private void OnStrongAttackCancelled()
    {
        if (_state == PlayerState.Charging)
        {
            _chargeReleased = true;
        }
    }

    private IEnumerator StrongAttackCoroutine()
    {
        float aux = Mathf.InverseLerp(_minCharge, _maxCharge, _chargeTime);
        _strongAttack.Damage = Mathf.Lerp(_playerData.MinStrongAttackDamage, _playerData.MaxStrongAttackDamage, aux);

        _strongAttack.TargetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        yield return new WaitForSeconds(0.2f);

        ConsumeStamina(_playerData.StrongAttackStaminaCost);
        _strongAttack.Play();

        _state = PlayerState.Move;
    }


    // -------------------------------------------------------------------------
    // Roll
    // -------------------------------------------------------------------------

    public void Roll()
    {
        if (canRoll && _areInputsEnabled && !_isPause && !_dialogIsOpen)
        {
            StartCoroutine(RollCoroutine());
        }
    }

    private IEnumerator RollCoroutine()
    {
        _state = PlayerState.Roll;

        UpdateLookDirection();
        _lookDirection = SnapToEightDirections(_lookDirection);

        ConsumeStamina(_playerData.RollingStaminaCost);

        SetVelocityInstant(_playerData.IniRollingSpeed * _lookDirection);
        yield return new WaitForSeconds(_playerData.IniRollingSeconds);

        SetVelocityInstant(_playerData.EndRollingSpeed * _lookDirection);
        yield return new WaitForSeconds(_playerData.EndRollingSeconds);

        _state = PlayerState.Move;
    }


    // -------------------------------------------------------------------------
    // Stamina
    // -------------------------------------------------------------------------

    private bool HasStamina()
    {
        return _playerData.Stamina > 0;
    }

    private void ConsumeStamina(float amount)
    {
        _staminaRegenTimer = 0;
        _playerData.Stamina = _playerData.Stamina - amount;
    }

    private void HandleStaminaRegeneration()
    {
        bool shouldRegenerate = _staminaRegenTimer >= _playerData.SecondsUntilStaminaRegeneration && (!_isRunning || _moveDirection.magnitude == 0);

        if (shouldRegenerate)
        {
            _playerData.Stamina = Mathf.Min(_playerData.MaxStamina, _playerData.Stamina + _playerData.StaminaRegenerationSpeed * Time.deltaTime);
        }
        else
        {
            _staminaRegenTimer += Time.deltaTime;
        }
    }


    // -------------------------------------------------------------------------
    // Health
    // -------------------------------------------------------------------------

    private void HandleHealthStatus() //gestionar posibles fectos de estado y daño por segundo
    {
        if (_playerData.Health <= 0)
        {
            Death();
        }
    }

    private void HandleHealthRegeneration()
    {
        bool shouldRegenerate = _state == PlayerState.Rest; //ahora mismo no hace falta porque el handle health está dentro del estado rest

        if (shouldRegenerate)
        {
            _playerData.Health = Mathf.Min(_playerData.MaxHealth, _playerData.Health + _playerData.HealthRegenerationSpeed * Time.deltaTime);
        }
    }

    public void TryToHeal()
    {
        if (_state != PlayerState.Move) { return; }
        if (_healData.ActualCooldownSeconds < _healData.CooldownSeconds) { return; }
        if (_playerData.Health == _playerData.MaxHealth)
        {
            Debug.Log("Full vida, no se puede curar");
            return;
        }
        if (_healData.RemainingUses > 0)
        {
            _healData.RemainingUses--;

            _playerData.Health += _healData.HealAmount;
            _healData.ActualCooldownSeconds = 0;
        }
        else
        {
            Debug.Log("No quedan usos de pociones");
        }
    }

    private void HandleCooldownPotion()
    {
        _healData.ActualCooldownSeconds += Time.deltaTime;
        _healData.ActualCooldownSeconds = Mathf.Min(_healData.ActualCooldownSeconds, _healData.CooldownSeconds);
    }


    // -------------------------------------------------------------------------
    // Oxygen
    // -------------------------------------------------------------------------

    private void HandleOxigen()
    {
        if (_isOnOxigenZone)
        {
            _playerData.Oxygen = _playerData.Oxygen + _playerData.OxygenRegenerationSpeed * Time.deltaTime;
        }
        else
        {
            _playerData.Oxygen = _playerData.Oxygen - _playerData.OxygenDropingSpeed * Time.deltaTime;

            float percentage = _playerData.Oxygen / _playerData.MaxOxygen * 100;

            if (percentage < 50 && !_oxygenData.HalfOxygenAlertPlayed)
            {
                GameManager.Instance.GetComponent<AudioManager>().PlaySound(_oxygenAlert50);
                _oxygenData.HalfOxygenAlertPlayed = true;
            }
            else if (percentage < 10 && !_oxygenData.LowOxygenAlertPlayed)
            {
                GameManager.Instance.GetComponent<AudioManager>().PlaySound(_oxygenAlert10);
                _oxygenData.LowOxygenAlertPlayed = true;
            }
            else if (_playerData.Oxygen <= 0)
            {
                HandleLastSeconds();
            }
        }
    }

    private void HandleLastSeconds()
    {
        if (_oxygenData.LastOxygenTimer < _oxygenData.LastOxygenSeconds)
        {
            _oxygenData.LastOxygenTimer += Time.deltaTime;
        }
        else
        {
            //Death();
            DeathWithoutOxygen(); //Ahora mismo no va
        }
    }

    private void HandleOnOxygenIncreased()
    {
        // Reset the last seconds timer when oxygen is increased
        _oxygenData.LastOxygenTimer = 0;

        // Reset the alerts only when the oxygen percentage goes above the thresholds
        float percentage = _playerData.Oxygen / _playerData.MaxOxygen * 100;

        if (percentage > 50)
        {
            _oxygenData.HalfOxygenAlertPlayed = false;
            _oxygenData.LowOxygenAlertPlayed = false;
        }
        else if (percentage > 10)
        {
            _oxygenData.LowOxygenAlertPlayed = false;
        }
    }

    public void ResetOxygen()
    {
        _playerData.Oxygen = _playerData.MaxOxygen;
    }

    [ContextMenu("EnterOxigenZone")]
    public void EnterOxigenZone()
    {
        // Play Oxigen Refilling COMPLETAR
        _isOnOxigenZone = true;

        _oxygenData.LastOxygenTimer = 0;
    }

    [ContextMenu("ExitOxigenZone")]
    public void ExitOxigenZone()
    {
        _isOnOxigenZone = false;
    }


    // -------------------------------------------------------------------------
    // Death
    // -------------------------------------------------------------------------

    public void SetDead(bool isDead)
    {
        if (isDead)
        {
            _state = PlayerState.Dead;
        }
        else
        {
            _state = PlayerState.Move;
        }
    }

    public void Death() //and respawn other player or the logic
    {
        if (_state != PlayerState.Dead)
        {
            if (deathSound != null)
                GameManager.Instance.audioManager.PlaySound(deathSound);
            StartCoroutine(WaitAndKill(deathAnimationSeconds));
        }
    }

    private IEnumerator WaitAndKill(float segundos)
    {
        SetDead(true);
        _renderer.color = Color.red;
        _renderer.sortingLayerName = "Foreforeground";

        CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();

        cm.CameraLockedPlayer = true;

        yield return new WaitForSeconds(segundos);

        _renderer.sortingLayerName = "Default";
        SetDead(false);

        DisposeActions();

        cm.PlayerReference = transform;
        Destroy(gameObject); // Destruir primero
        cm.CameraLockedPlayer = false;
        cm.RespawnPlayer(); // Llamar despues, desde un objeto que sobrevive
    }

    public void DeathWithoutOxygen() //and respawn other player or the logic
    {
        if (_state != PlayerState.Dead)
        {
            if (deathSound != null)
                GameManager.Instance.audioManager.PlaySound(deathSound);
            StartCoroutine(WaitAndKillWithoutOxygen(deathAnimationSeconds));
        }
    }

    private IEnumerator WaitAndKillWithoutOxygen(float segundos)
    {
        SetDead(true);

        _renderer.color = Color.red;
        _renderer.sortingLayerName = "Foreforeground";
        CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();

        cm.CameraLockedPlayer = true;

        yield return new WaitForSeconds(segundos);

        SetDead(false);
        _renderer.sortingLayerName = "Default";
        DisposeActions();

        cm.PlayerReference = transform;
        Destroy(gameObject); // Destruir primero
        cm.CameraLockedPlayer = false;
        cm.RespawnPlayerAfterNoOxygen(); // Llamar despues, desde un objeto que sobrevive
    }


    // -------------------------------------------------------------------------
    // Scene Transitions
    // -------------------------------------------------------------------------

    public void ExitScene(Vector2 exitDirection, float animationDurationSeconds)
    {
        StartCoroutine(ExitSceneCoroutine(exitDirection, animationDurationSeconds));
    }

    private IEnumerator ExitSceneCoroutine(Vector2 exitDirection, float animationDurationSeconds)
    {
        Debug.Log(exitDirection);

        _state = PlayerState.Move;
        _isRunning = false;

        if (_collider != null)
        {
            _collider.enabled = false;
        }

        _areInputsEnabled = false;

        _moveDirection = exitDirection.normalized;
        yield return new WaitForSeconds(animationDurationSeconds);
        _moveDirection = Vector2.zero;

        if (_collider != null)
        {
            _collider.enabled = true;
        }

        _areInputsEnabled = true;
    }


    // -------------------------------------------------------------------------
    // Animator
    // -------------------------------------------------------------------------

    private void HandleAnimatorParams()
    {
        _animator.SetFloat("xDir", _lookDirection.x);
        _animator.SetFloat("yDir", _lookDirection.y);
        _animator.SetBool("isIdle", _state == PlayerState.Move && _moveDirection == Vector2.zero);
        _animator.SetBool("isWalking", _state == PlayerState.Move && _moveDirection != Vector2.zero && (!_isRunning || _isRunning && !HasStamina()));
        _animator.SetBool("isRunning", _state == PlayerState.Move && _moveDirection != Vector2.zero && _isRunning && HasStamina());
        _animator.SetBool("isRolling", _state == PlayerState.Roll);
        _animator.SetBool("isWeakAttacking", _state == PlayerState.WeakAttack);
        _animator.SetFloat("ChargeTime", _chargeTime);
    }


    // -------------------------------------------------------------------------
    // Dialog
    // -------------------------------------------------------------------------

    private void UpdateDialogIsOpen()
    {
        _dialogIsOpen = GameManager.Instance.GetComponent<DiverseMenusManager>().DialogIsOpen;
        if (_dialogIsOpen)
        {
            _moveDirection = Vector2.zero;
        }
    }


    // -------------------------------------------------------------------------
    // Input
    // -------------------------------------------------------------------------

    public void MovePerformed()
    {
        if (_areInputsEnabled && !_dialogIsOpen)
        {
            _moveDirection = _inputManager._MoveDirection;
        }
    }

    public void MoveCancelled()
    {
        if (_areInputsEnabled)
        {
            _moveDirection = Vector2.zero;
        }
    }

    public void RunPerformed()
    {
        if (_areInputsEnabled && !_dialogIsOpen)
        {
            _isRunning = true;
        }
    }

    public void RunCancelled()
    {
        if (_areInputsEnabled)
        {
            _isRunning = false;
        }
    }

    private void DisposeActions()
    {
        if (_inputManager == null) return;
        _inputManager.onMovePerformed -= MovePerformed;
        _inputManager.onMoveCancelled -= MoveCancelled;
        _inputManager.onRunPerformed -= RunPerformed;
        _inputManager.onRunCancelled -= RunCancelled;
        _inputManager.onRoll -= Roll;
        _inputManager.onWeakAttack -= WeakAttack;
        _inputManager.onStrongAttackPerformed -= OnStrongAttackPerformed;
        _inputManager.onStrongAttackCancelled -= OnStrongAttackCancelled;
        _inputManager.onHeal -= TryToHeal;
        GameManager.Instance.GetComponent<DiverseMenusManager>().dialogOpened -= UpdateDialogIsOpen;
    }

    private void EnableActions()
    {
        _inputManager.onMovePerformed += MovePerformed;
        _inputManager.onMoveCancelled += MoveCancelled;
        _inputManager.onRunPerformed += RunPerformed;
        _inputManager.onRunCancelled += RunCancelled;
        _inputManager.onRoll += Roll;
        _inputManager.onWeakAttack += WeakAttack;
        _inputManager.onStrongAttackPerformed += OnStrongAttackPerformed;
        _inputManager.onStrongAttackCancelled += OnStrongAttackCancelled;
        _inputManager.onHeal += TryToHeal;
        GameManager.Instance.GetComponent<DiverseMenusManager>().dialogOpened += UpdateDialogIsOpen;
    }
}
