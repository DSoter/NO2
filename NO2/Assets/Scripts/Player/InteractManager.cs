using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    private GameObject _textoLetraImagenUI;
    [SerializeField] private Sprite _keySprite;


    [SerializeField] private InputActionReference actionReference;
    //player controller
    private PlayerController _playerController;
    private void Awake()
    {
        actionReference.action.performed += OnInteract;
        actionReference.action.canceled += OnInteract;       
        
        

        _canvas = transform.GetChild(0).gameObject;
        _imagenUI = _canvas.transform.GetChild(0).gameObject;
        _textoLetraImagenUI = _imagenUI.transform.GetChild(0).gameObject;
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
            _imagenUI.GetComponent<UnityEngine.UI.Image>().sprite = _keySprite;

            _textoLetraImagenUI.GetComponent<TextMeshProUGUI>().text = ObtainStringInteractBinding();
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


    void OnEnable()
    {
        actionReference.action.Enable();
    }
    void OnDisable()
    {
        actionReference.action.Disable();
    }
    void OnDestroy()
    {
        actionReference.action.performed -= OnInteract;
        actionReference.action.canceled -= OnInteract;
    }

    private string ObtainStringInteractBinding()
    {
        string json = PlayerPrefs.GetString("rebinds", "");
        if (!string.IsNullOrEmpty(json))
        {
            actionReference.action.actionMap.asset.LoadBindingOverridesFromJson(json);
        }

        string text =
            InputControlPath.ToHumanReadableString(
                actionReference.action.bindings[0].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );
        return text;
    }


 

    

}

