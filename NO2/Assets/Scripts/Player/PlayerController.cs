 using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
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
    private bool _isRunning = false;
    private PlayerState _state = PlayerState.Move;

    //Secene controler
    private SceneController _sceneController;

    // Movement Configuration
    [SerializeField] private float _walkingSpeed = 50f;
    [SerializeField] private float _runningSpeed = 100f;

    [Header("Sonidos")]
    [SerializeField] private AudioClip  deathSound;


    [Header("Player data")]
    [SerializeField] private PlayerData _playerData;



    //Interactuar con objetos
    [HideInInspector] public GameObject objetoInteractuable;
    private bool _puedeInteractuar;



    //UIEmergente
    private GameObject _canvas;
    private GameObject _imagenUI;
    private GameObject _textoUI;
    [SerializeField] private Sprite _teclaE;


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

        m_Player.Interact.performed += OnInteract;
        m_Player.Interact.canceled += OnInteract;

        _sceneController = FindAnyObjectByType<SceneController>();

        _canvas = transform.GetChild(0).gameObject;
        _imagenUI = _canvas.transform.GetChild(0).gameObject;
        _textoUI = _canvas.transform.GetChild(1).gameObject;

    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0f) return;
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


        if (_puedeInteractuar)
        {
            _canvas.SetActive(true);
            _imagenUI.GetComponent<Image>().sprite = _teclaE;
        }
        else
        {
            _canvas.SetActive(false);
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
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_puedeInteractuar) { objetoInteractuable.GetComponent<InteractuablePrueba>().interact(); }
        }
        if (context.canceled)
        {
            
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
    public void SetPuedeInteractuar(bool interact)
    {
        _puedeInteractuar = interact;
    }
    public bool GetPuedeInteractuar()
    {
        return _puedeInteractuar;
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
        _sceneController.SpawnPlayer();
        Destroy(transform.gameObject);

    }
}
