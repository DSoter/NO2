using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    private bool _isRunning = false;
    private PlayerState _state = PlayerState.Move;

    //Secene controler
    private SceneController _sceneController;

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
    }

    private void Update()
    {
        switch (_state)
        {
            case PlayerState.Move:

                UpdateLookDirection();

                if (_isRunning)
                {
                    _rigidbody.linearVelocity = _playerData.WalkingSpeed * _moveDirection;
                }
                else
                {
                    _rigidbody.linearVelocity = _playerData.RunningSpeed * _moveDirection;
                }

                break;
            case PlayerState.Roll:


                break;
            case PlayerState.Dead:

                _rigidbody.linearVelocity = Vector2.zero;

                return;

        }

        HandleAnimatorParams();
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
        if (context.started && _state != PlayerState.Roll)
        {
            StartCoroutine(RollCoroutine());
        }
    }

    private IEnumerator RollCoroutine()
    {
        _state = PlayerState.Roll;

        UpdateLookDirection();

        _rigidbody.linearVelocity = 6 * _lookDirection;
        yield return new WaitForSeconds(0.3f);
        _rigidbody.linearVelocity = 2 * _lookDirection;
        yield return new WaitForSeconds(0.2f);

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
        _animator.SetBool("isWalking", _state == PlayerState.Move && !_isRunning && _moveDirection != Vector2.zero);
        _animator.SetBool("isRunning", _state == PlayerState.Move && _isRunning && _moveDirection != Vector2.zero);
        _animator.SetBool("isRolling", _state == PlayerState.Roll);
    }

    void OnDestroy()
    {
        m_Actions.Dispose();
    }
    void OnEnable()
    {
        m_Player.Enable();
    }
    void OnDisable()
    {
        m_Player.Disable();
    }
    public void SetDead(bool isDead)
    {
        if (isDead){ _state = PlayerState.Dead; }   
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
        GameManager.Instance.GetComponent<CheckpointManager>().SpawnPlayerAfterDeath();
        Destroy(transform.gameObject);

    }
}
