using NUnit.Framework;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{


    // Gameobject Componentes
    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _renderer;

    


    // Local Variables
    private Vector2 _moveDirection = new Vector2(0,0);
    private Vector2 _lookDirection = new Vector2(1, 0);
    private float _staminaRegenTimer = 0;
    private bool _isRunning = false;
    private bool canRoll => _state != PlayerState.Roll && HasStamina();
    private PlayerState _state = PlayerState.Move;

    // InputSystem 
    private InputActionReference _moveRef, _runRef, _rollRef, _weakAttackRef, _strongAttackRef;
    [Header("Input System")]
    [SerializeField] private InputActionAsset _inputSystemReference;



   //References
   [Header("Sonidos")]
    [SerializeField] private AudioClip deathSound;

    [Space(5)]
    [Header("Player data")]
    [SerializeField] private PlayerData _playerData;

    public enum PlayerState
    {
        Move,
        Roll,
        Dead
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

        Debug.Log("Antes de inicializar actions");
        InitializePrefsActions();
        Debug.Log("Despues de inicializar actions");
        //m_Actions = new InputSystem();

        //PrefsToKeybinds();

        //PrepareActions();

        _playerData.Stamina = _playerData.MaxStamina;
    }

    private void Update()
    {
        HandleAnimatorParams();

        switch (_state)
        {
            case PlayerState.Move:

                UpdateLookDirection();

                if (_isRunning && HasStamina())
                {
                    Run();
                }
                else
                {
                    Walk();   
                }

                HandleStaminaRegeneration();

                break;
            case PlayerState.Roll:


                break;
            case PlayerState.Dead:

                _rigidbody.linearVelocity = Vector2.zero;

                return;

        }

    }

    private void Run() 
    {
        if (_moveDirection.magnitude > 0)
        {
            ConsumeStamina(_playerData.RunningStaminaCost * Time.deltaTime);

        }

        _rigidbody.linearVelocity = _playerData.RunningSpeed * _moveDirection;

    }

    private void Walk()
    {
        _rigidbody.linearVelocity = _playerData.WalkingSpeed * _moveDirection;
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
        if (context.performed)
        {
            _moveDirection = context.ReadValue<Vector2>();
        }
        if (context.canceled)
        {
            _moveDirection = Vector2.zero;
        }

    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed){
            _isRunning = true;
        }

        if (context.canceled)
        {
            _isRunning = false;
        }
    }
   

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.started && canRoll)
        {
            StartCoroutine(RollCoroutine());
        }
    }

    private IEnumerator RollCoroutine()
    {
        _state = PlayerState.Roll;

        UpdateLookDirection();
        ConsumeStamina(_playerData.RollingStaminaCost);

        _rigidbody.linearVelocity = _playerData.IniRollingSpeed * _lookDirection;
        yield return new WaitForSeconds(0.3f); // Change to PlayerData


        _rigidbody.linearVelocity = _playerData.EndRollingSpeed * _lookDirection;
        yield return new WaitForSeconds(0.2f); // Change to PlayerData

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
        StartCoroutine(WaitAndKill(0.5f));
    }
    IEnumerator WaitAndKill(float segundos)
    {
        SetDead(true);
        _renderer.color = Color.red;

        yield return new WaitForSeconds(segundos);

        SetDead(false);

        DisposeActions();

        CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();
        Destroy(gameObject); // Destruir primero
        cm.RespawnPlayer(); // Llamar despu�s, desde un objeto que sobrevive

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
            //_weakAttackRef.action.performed -= OnWeakAttack;
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
        //weakAttackRef.action.performed += OnWeakAttack;
        //strongAttackRef.action.performed += OnStrongAttack;
    }

}
