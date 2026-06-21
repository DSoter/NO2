 using NUnit.Framework;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

// AYUDA PORFAVOR

public class PlayerController : MonoBehaviour
{


    // Gameobject Componentes
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _renderer;
    private Collider2D _collider;

    // Local Variables
    private Vector2 _moveDirection = new Vector2(0,0);
    private Vector2 _lookDirection = new Vector2(1, 0);
    private float _staminaRegenTimer = 0;
    private int _attackCounter = 0;
    private bool _areInputsEnabled = true;
    private bool _isRunning = false;
    private bool canRoll => _state == PlayerState.Move && HasStamina();
    private bool canAttack => _state == PlayerState.Move && HasStamina();
    private PlayerState _state = PlayerState.Move;

    // InputSystem 
    private InputActionReference _moveRef, _runRef, _rollRef, _weakAttackRef, _strongAttackRef;
    [Header("Input System")]
    [SerializeField] private InputActionAsset _inputSystemReference;



    // References
    [Header("Sonidos")]
    [SerializeField] private AudioClip deathSound;

    [Space(5)]
    [Header("Player data")]
    [SerializeField] private PlayerData _playerData;

    [Space(5)]
    [Header("Weak Attack")]
    [SerializeField] private WeakAttackController _weakAttack;

    // MOVE TO PLAYER DATA (COMPLETAR)
    [Space(5)]
    [Header("Animation durations")]
    [SerializeField] private float deathAnimationSeconds = 2f;
    [SerializeField] private float _acceleration = 25f;

    [SerializeField] private float _attackImpulse = 1f;

    public enum PlayerState
    {
        Move,
        Roll,
        Dead,
        WeakAttack,
    }

    public PlayerState GetState()
    {
        return _state;
    }


    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
         

        InitializePrefsActions();
        //m_Actions = new InputSystem();

        //PrefsToKeybinds();

        //PrepareActions();

        _playerData.Stamina = _playerData.MaxStamina;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        HandleAnimatorParams();

        switch (_state)
        {
            case PlayerState.Move:

                UpdateLookDirection();
                HandleStaminaRegeneration();

                break;
            case PlayerState.Roll:


                break;
            case PlayerState.WeakAttack:

                break;
            case PlayerState.Dead:

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
            case PlayerState.Dead:

                _rigidbody.linearVelocity = Vector2.zero;
                break;
        }
    }

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
        _rigidbody.AddForce(velocityChange, ForceMode2D.Force);
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

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed && _areInputsEnabled)
        {
            _moveDirection = context.ReadValue<Vector2>();
        }
        if (context.canceled && _areInputsEnabled)
        {
            _moveDirection = Vector2.zero;
        }

    }

    public void OnRun(InputAction.CallbackContext context)
    {

        
        if (context.performed && _areInputsEnabled)
        {
            _isRunning = true;
        }

        if (context.canceled && _areInputsEnabled)
        {
            _isRunning = false;
        }
    }
   

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.started && canRoll && _areInputsEnabled)
        {
            StartCoroutine(RollCoroutine());
        }
    }

    private IEnumerator RollCoroutine()
    {
        _state = PlayerState.Roll;

        UpdateLookDirection();
        ConsumeStamina(_playerData.RollingStaminaCost);

        SetVelocityInstant(_playerData.IniRollingSpeed * _lookDirection);
        yield return new WaitForSeconds(_playerData.IniRollingSeconds);


        SetVelocityInstant(_playerData.EndRollingSpeed * _lookDirection);
        yield return new WaitForSeconds(_playerData.EndRollingSeconds);

         

        _state = PlayerState.Move;
    }

    private void OnWeakAttack(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack && _areInputsEnabled)
        {
            Debug.Log("Weak Attack");
            _state = PlayerState.WeakAttack;

            // Calculate the attack direction based on the mouse position
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            _lookDirection = (mousePos - transform.position).normalized;

            // Apply small force with the attack direction
            //_rigidbody.linearVelocity = _rigidbody.linearVelocity/2;
            _rigidbody.AddForce(_lookDirection * _attackImpulse, ForceMode2D.Impulse);

            // Set the attack direction in the animator
            _animator.SetFloat("xDir", _lookDirection.x);
            _animator.SetFloat("yDir", _lookDirection.y);

            _renderer.flipX = (_lookDirection.x < 0);

            // Play attack animation
            bool isAttackFlipped = (_attackCounter % 2) == 1; 
            _weakAttack.Play(isAttackFlipped);

            _attackCounter++;
        }
    }

    private void EndWeakAttack()
    {
        _state = PlayerState.Move;
    }

    private void UpdateLookDirection()
    {
        if(_moveDirection.magnitude > 0)
        {
            _lookDirection = _moveDirection.normalized;
        }

        _renderer.flipX = (_lookDirection.x < 0);
    }

    private void HandleAnimatorParams()
    {
        _animator.SetFloat("xDir", _lookDirection.x);
        _animator.SetFloat("yDir", _lookDirection.y);
        _animator.SetBool("isIdle", _state == PlayerState.Move && _moveDirection == Vector2.zero);
        _animator.SetBool("isWalking", _state == PlayerState.Move && _moveDirection != Vector2.zero && (!_isRunning || _isRunning && !HasStamina()));
        _animator.SetBool("isRunning", _state == PlayerState.Move && _moveDirection != Vector2.zero && _isRunning && HasStamina());
        _animator.SetBool("isRolling", _state == PlayerState.Roll);
        _animator.SetBool("isWeakAttacking", _state == PlayerState.WeakAttack);
    }

    private bool HasStamina()
    {
        return _playerData.Stamina > 0;
    }

    private void ConsumeStamina(float amount)
    {
        _staminaRegenTimer = 0;
        _playerData.Stamina = Mathf.Max(0, _playerData.Stamina - amount);
    }

    public void SetDead(bool isDead)
    {
        if (isDead) {
            _state = PlayerState.Dead;
        }
    }

    public void Death()//and respawn other player or the logic
    {
        if (deathSound != null)
            GameManager.Instance.audioManager.PlaySound(deathSound);
        StartCoroutine(WaitAndKill(deathAnimationSeconds));
    }
    IEnumerator WaitAndKill(float segundos)
    {
        SetDead(true);
        _renderer.color = Color.red;
        CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();

        cm.CameraLockedPlayer = true;

        yield return new WaitForSeconds(segundos);

        SetDead(false);

        DisposeActions();

        
        cm.PlayerReference = transform;
        Destroy(gameObject); // Destruir primero
        cm.CameraLockedPlayer = false;
        cm.RespawnPlayer(); // Llamar despues, desde un objeto que sobrevive

    }

    

    void OnDestroy()
    {
        DisposeActions();
    }
    void OnEnable()
    {
        EnableActions();
    }
    void OnDisable()
    {
        DisposeActions();
    }

 

    private void InitializePrefsActions()
    {


        string json = PlayerPrefs.GetString("rebinds", "");
        if (!string.IsNullOrEmpty(json))
        {
            _inputSystemReference.LoadBindingOverridesFromJson(json);
        }

        InputActionMap playerMap = _inputSystemReference.FindActionMap("Player");

        _moveRef = InputActionReference.Create(playerMap.FindAction("Move"));
        _runRef = InputActionReference.Create(playerMap.FindAction("Run"));
        _rollRef = InputActionReference.Create(playerMap.FindAction("Roll"));
        
        _weakAttackRef = InputActionReference.Create(playerMap.FindAction("WeakAttack"));
        _strongAttackRef = InputActionReference.Create(playerMap.FindAction("StrongAttack"));

        EnableActions();
        playerMap.Enable();
    }

    private void DisposeActions()
    {
        if (_moveRef != null)
        {
            _moveRef.action.performed -= OnMove;
            _moveRef.action.canceled -= OnMove;
            _runRef.action.performed -= OnRun;
            _runRef.action.canceled -= OnRun;
            _rollRef.action.started -= OnRoll;
            _weakAttackRef.action.performed -= OnWeakAttack;
            //_strongAttackRef.action.performed -= OnStrongAttack;
            _inputSystemReference.FindActionMap("Player").Disable();
        }
    }

    private void EnableActions()
    {
        _moveRef.action.performed += OnMove;
        _moveRef.action.canceled += OnMove;
        _runRef.action.performed += OnRun;
        _runRef.action.canceled += OnRun;
        _rollRef.action.started += OnRoll;
        _weakAttackRef.action.performed += OnWeakAttack;
        //strongAttackRef.action.performed += OnStrongAttack;
    }

    public void ExitScene(Vector2 exitDirection, float animationDurationSeconds)
    {
        StartCoroutine(ExitSceneCoroutine(exitDirection, animationDurationSeconds));
    }

    private IEnumerator ExitSceneCoroutine(Vector2 exitDirection, float animationDurationSeconds)
    {
        _state = PlayerState.Move;
        _isRunning = false;

        _collider.enabled = false;
        _areInputsEnabled = false;

        _moveDirection = exitDirection.normalized;
        yield return new WaitForSeconds(animationDurationSeconds);
        _moveDirection = Vector2.zero;

        _collider.enabled = true;
        _areInputsEnabled = true;

    }
}
