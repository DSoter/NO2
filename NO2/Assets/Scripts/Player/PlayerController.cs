 using Unity.VisualScripting;
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
    private bool _isRunning = false;
    private PlayerState _state = PlayerState.Move;

    // Movement Configuration
    [SerializeField] private float _walkingSpeed = 50f;
    [SerializeField] private float _runningSpeed = 100f;




    public enum PlayerState
    {
        Move,
        Roll,
        Dead
    }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();

        m_Actions = new InputSystem();
        m_Player = m_Actions.Player;

        m_Player.Move.performed += OnMove;
        m_Player.Move.canceled += OnMove;

        m_Player.Run.performed += OnRun;
        m_Player.Run.canceled += OnRun;

    }

    private void FixedUpdate()
    {
        switch (_state)
        {
            case PlayerState.Move:

                if (_isRunning)
                {
                    _rigidbody.linearVelocity = _runningSpeed * Time.fixedDeltaTime * _moveDirection.normalized;
                }
                else
                {
                    _rigidbody.linearVelocity = _walkingSpeed * Time.fixedDeltaTime * _moveDirection.normalized;
                }

                break;
            case PlayerState.Roll:

                break;

            case PlayerState.Dead:
                _rigidbody.linearVelocity = Vector2.zero;
                return;

        }
       

        HandleAnimatorParams();

        _renderer.flipX = (_moveDirection.x < 0);
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


    private void HandleAnimatorParams()
    {
        _animator.SetFloat("xDir", _moveDirection.x);
        _animator.SetFloat("yDir", _moveDirection.y);
        _animator.SetBool("isIdle", _state == PlayerState.Move && _moveDirection == Vector2.zero);
        _animator.SetBool("isWalking", _state == PlayerState.Move && !_isRunning && _moveDirection != Vector2.zero);
        _animator.SetBool("isRunning", _state == PlayerState.Move && _isRunning && _moveDirection != Vector2.zero);     
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
}
