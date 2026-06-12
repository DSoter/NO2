using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class InteractManager : MonoBehaviour
{
    //Interactuar con objetos
    [HideInInspector] public Interactable interactableObject;
    [HideInInspector] public List<Interactable> interactables;
    private bool _puedeInteractuar;

    // InputSystem 
    private InputSystem m_Actions;
    private InputSystem.PlayerActions m_Player;

    //UIEmergente
    private GameObject _canvas;
    private GameObject _imagenUI;
    private GameObject _textoUI;
    [SerializeField] private Sprite _keySpriteE;


    //player controller
    private PlayerController _playerController;
    private void Awake()
    {
        m_Actions = new InputSystem();

        PrefsToKeybinds();


        PrepareActions();

        


        _canvas = transform.GetChild(0).gameObject;
        _imagenUI = _canvas.transform.GetChild(0).gameObject;
        _textoUI = _canvas.transform.GetChild(1).gameObject;
        interactables = new List<Interactable>();
    }
    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_puedeInteractuar)
        {
            _canvas.SetActive(true);
            _imagenUI.GetComponent<UnityEngine.UI.Image>().sprite = _keySpriteE;
        }
        else
        {
            _canvas.SetActive(false);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_puedeInteractuar && _playerController.GetState() == PlayerController.PlayerState.Move) { 
                interactableObject.Interact();
            }
        }
        if (context.canceled)
        {

        }
    }
    public void SetPuedeInteractuar(bool interact)
    {
        _puedeInteractuar = interact;
    }
    public bool GetPuedeInteractuar()
    {
        return _puedeInteractuar;
    }

    void OnDestroy()
    {
        //if (m_Actions != null)
        //{
        //    m_Actions.Dispose();
        //    m_Actions = null;
        //}
    }
    void OnEnable()
    {
        m_Player.Enable();
    }
    void OnDisable()
    {
        m_Player.Disable();
    }

    public void UpdateRebindingInteract()
    {
        StartCoroutine(WaitAndChangeBindings());
        
        
    }
    IEnumerator WaitAndChangeBindings()
    {
        DisposeActions();
        
        yield return null;
        PrefsToKeybinds();
        PrepareActions();
    }

 

    public void PrefsToKeybinds()
    {
        m_Actions = new InputSystem();

        string json = PlayerPrefs.GetString("rebinds", "");
        if (!string.IsNullOrEmpty(json))
        {
            m_Actions.asset.LoadBindingOverridesFromJson(json);
        }

        m_Player = m_Actions.Player;
    }
    private void DisposeActions()
    {
        m_Player.Disable();
        m_Player.Interact.performed -= OnInteract;
        m_Player.Interact.canceled -= OnInteract;
        m_Actions.Dispose();
    }

    private void PrepareActions()
    {
        m_Player = m_Actions.Player;

        m_Player.Interact.performed += OnInteract; 
        m_Player.Interact.canceled += OnInteract; 
    }

}

