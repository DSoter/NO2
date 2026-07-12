using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    private InputActionReference _moveRef, _interactRef, _runRef, _rollRef, _weakAttackRef, _strongAttackRef;
    private InputActionReference _mapRef, _flowerRef, _badgesRef, _scapeRef, _navigateLeft, _navigateRight, _confirm;
    private InputActionReference _goUp, _goDown, _goLeft, _goRight;

    public InputActionReference _InteractRef
    {
        get { return _interactRef; }
    }

    [Header("Input System")]
    [SerializeField] private InputActionAsset _inputSystemReference;


    public event Action onMovePerformed, onMoveCancelled, onInteract, onRunPerformed, onRunCancelled, onRoll, onWeakAttack, onStrongAttack;
    public event Action onMap, onFlower, onBadge, onEscape, onNavigateLeft, onNavigateRight, onConfirm;
    public event Action onGoUp, onGoDown, onGoLeft, onGoRight;

    private Vector2 _moveDirection;

    public Vector2 _MoveDirection
    {
        get {  return _moveDirection; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        InitializePrefsActions();
    }
    private void Update()
    {
        if (!_moveRef.action.enabled)
            Debug.LogError("¡Move action se ha desactivado! " + Time.frameCount);
        if (_scapeRef != null && !_scapeRef.action.enabled)
            Debug.LogError($"¡Escape action desactivada! Frame {Time.frameCount}");
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("OnMove fired"); // antes de cualquier if
        if (context.performed)
        {
            _moveDirection = context.ReadValue<Vector2>();
            onMovePerformed?.Invoke();
            
        }
        if (context.canceled){
            _moveDirection = Vector2.zero;
            onMoveCancelled?.Invoke();
            
        }
    }
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("funciona llamada");
            onInteract?.Invoke();
        }
    }
    private void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onRunPerformed?.Invoke();
        }
        if (context.canceled)
        {
            onRunCancelled?.Invoke();
        }
    }
    private void OnRoll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onRoll?.Invoke();
        }
    }

    private void OnWeakAttack(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {
            Debug.Log("funciona llamada");
            onWeakAttack?.Invoke();
        }
    }
    private void OnMap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onMap?.Invoke();
        }
    }
    private void OnFlower(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onFlower?.Invoke();
        }
    }
    private void OnBadge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onBadge?.Invoke();
        }
    }
    private void OnEscape(InputAction.CallbackContext context)
    {
        Debug.Log("OnEscape fired en InputManager");
        if (context.performed)
        {
            Debug.Log($"onEscape suscriptores: {onEscape?.GetInvocationList().Length ?? 0}");
            onEscape?.Invoke();
        }
    }
    private void OnNavigateLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onNavigateLeft?.Invoke();
        }
    }
    private void OnNavigateRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onNavigateRight?.Invoke();
        }
    }
    private void OnConfirm(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onConfirm?.Invoke();
        }
    }

    private void OnGoUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onGoUp?.Invoke();
        }
    }
    private void OnGoDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onGoDown?.Invoke();
        }
    }
    private void OnGoLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onGoLeft?.Invoke();
        }
    }
    private void OnGoRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onGoRight?.Invoke();
        }
    }

    void OnDestroy()
    {
        DisposeActions();
    }
    void OnEnable()
    {
        if (_moveRef == null) return;
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
        _interactRef = InputActionReference.Create(playerMap.FindAction("Interact"));

        _weakAttackRef = InputActionReference.Create(playerMap.FindAction("WeakAttack"));
        _strongAttackRef = InputActionReference.Create(playerMap.FindAction("StrongAttack"));

        

        InputActionMap UIMap = _inputSystemReference.FindActionMap("UI");

        _mapRef = InputActionReference.Create(UIMap.FindAction("OpenMap"));
        _flowerRef = InputActionReference.Create(UIMap.FindAction("OpenFlowers"));
        _badgesRef = InputActionReference.Create(UIMap.FindAction("OpenBadges"));
        _scapeRef = InputActionReference.Create(UIMap.FindAction("Escape"));
        _navigateLeft = InputActionReference.Create(UIMap.FindAction("NavigateLeft"));
        _navigateRight = InputActionReference.Create(UIMap.FindAction("NavigateRight"));
        _confirm = InputActionReference.Create(UIMap.FindAction("Confirm"));
        _goUp = InputActionReference.Create(UIMap.FindAction("GoUp"));
        _goDown = InputActionReference.Create(UIMap.FindAction("GoDown"));
        _goRight = InputActionReference.Create(UIMap.FindAction("GoRight"));
        _goLeft = InputActionReference.Create(UIMap.FindAction("GoLeft"));
        UIMap.Enable();
        playerMap.Enable();
        Debug.Log($"Player map enabled: {playerMap.enabled}");
        Debug.Log($"UI map enabled: {UIMap.enabled}");

        EnableActions();

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
            _interactRef.action.performed -= OnInteract;
            _mapRef.action.performed -= OnMap;
            _flowerRef.action.performed -= OnFlower;
            _badgesRef.action.performed -= OnBadge;
            _navigateLeft.action.performed -= OnNavigateLeft;
            _navigateRight.action.performed -= OnNavigateRight;
            _confirm.action.performed -= OnConfirm;
            _goUp.action.performed -= OnGoUp;
            _goDown.action.performed -= OnGoDown;
            _goRight.action.performed -= OnGoRight;
            _goLeft.action.performed -= OnGoLeft;
            _scapeRef.action.performed -= OnEscape;
            //_strongAttackRef.action.performed -= OnStrongAttack;

            //_inputSystemReference.FindActionMap("Player").Disable();

            //_inputSystemReference.FindActionMap("UI").Disable();
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
        _interactRef.action.performed += OnInteract;
        //strongAttackRef.action.performed += OnStrongAttack;
        _mapRef.action.performed += OnMap;
        _flowerRef.action.performed += OnFlower;
        _badgesRef.action.performed += OnBadge;
        _navigateLeft.action.performed += OnNavigateLeft;
        _navigateRight.action.performed += OnNavigateRight;
        _confirm.action.performed += OnConfirm;
        _goUp.action.performed += OnGoUp;
        _goDown.action.performed += OnGoDown;
        _goRight.action.performed += OnGoRight;
        _goLeft.action.performed += OnGoLeft;
        _scapeRef.action.performed += OnEscape;

        Debug.Log($"Acciones suscritas. Move enabled: {_moveRef.action.enabled}");
    }



}
