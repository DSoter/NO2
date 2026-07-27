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


    //UIEmergente
    private GameObject _canvas;
    private GameObject _imagenUI;
    private GameObject _textoUI;
    private GameObject _textoLetraImagenUI;
    [SerializeField] private Sprite _keySprite;


    private InputManager inputManager;
    private DiverseMenusManager diverseMenusManager;
    private PlayerController _playerController;

    private string _cachedBindingText;
    private void Awake()
    {

        inputManager = GameManager.Instance.gameObject.GetComponent<InputManager>();
        diverseMenusManager = GameManager.Instance.gameObject.GetComponent<DiverseMenusManager>();

        inputManager.onInteract += OnInteract; 
        
        

        _canvas = transform.GetChild(0).gameObject;
        _imagenUI = _canvas.transform.GetChild(0).gameObject;
        _textoLetraImagenUI = _imagenUI.transform.GetChild(0).gameObject;
        _textoUI = _canvas.transform.GetChild(1).gameObject;
        interactables = new List<Interactable>();

        RefreshInteractBinding();
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

            _textoLetraImagenUI.GetComponent<TextMeshProUGUI>().text = _cachedBindingText;
        }
        else
        {
            _canvas.SetActive(false);
        }
    }

    public void OnInteract()
    {
        
        if (_puedeInteractuar && !diverseMenusManager._IsOpen && (_playerController.GetState() == PlayerController.PlayerState.Move || _playerController.GetState() == PlayerController.PlayerState.Rest)){ 
            interactableObject.Interact();
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
        inputManager.onInteract -= OnInteract;
    }

    public void RefreshInteractBinding()
    {
        _cachedBindingText = InputControlPath.ToHumanReadableString(
         inputManager._InteractRef.action.bindings[0].effectivePath,
         InputControlPath.HumanReadableStringOptions.OmitDevice
     );
    }

    public void ChangeDisplayText( string text)
    {

        _textoUI.GetComponent<TextMeshProUGUI>().text = text;
    }
}

