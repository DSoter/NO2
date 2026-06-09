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

    // InputSystem 
    private InputSystem m_Actions;
    private InputSystem.PlayerActions m_Player;

    // Local Variables
    private Vector2 _moveDirection = new Vector2(0,0);
    private Vector2 _lookDirection = new Vector2(1, 0);
    private float _staminaRegenTimer = 0;
    private bool _isRunning = false;
    private bool canRoll => _state != PlayerState.Roll && HasStamina();
    private PlayerState _state = PlayerState.Move;

    //Secene controler
    private SceneController _sceneController;

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

        _sceneController = FindAnyObjectByType<SceneController>();

        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();

        m_Actions = new InputSystem();
        m_Player = m_Actions.Player;

        m_Player.Move.performed += OnMove;
        m_Player.Move.canceled += OnMove;

        m_Player.Run.performed += OnRun;
        m_Player.Run.canceled += OnRun;

        m_Player.Roll.started += OnRoll;

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
        if (isDead) { _state = PlayerState.Dead; }
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

        m_Player.Disable();
        m_Player.Move.performed -= OnMove;
        m_Player.Move.canceled -= OnMove;
        m_Player.Run.performed -= OnRun;
        m_Player.Run.canceled -= OnRun;
        m_Player.Roll.started -= OnRoll;
        m_Actions.Dispose();

        CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();
        Destroy(gameObject); // Destruir primero
        cm.SpawnPlayerAfterDeath(); // Llamar despu�s, desde un objeto que sobrevive

    }

    

    void OnDestroy()
    {
        if (m_Actions != null)
        {
            m_Actions.Dispose();
            m_Actions = null;
        }
    }
    void OnEnable()
    {
        m_Player.Enable();
    }
    void OnDisable()
    {
        m_Player.Disable();
    }

}
