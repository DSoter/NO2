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
    [SerializeField] private KeyIconDatabase keyIconDatabase;


    private InputManager inputManager;
    private DiverseMenusManager diverseMenusManager;
    private PlayerController _playerController;

    private string _cachedBindingText;
    private Sprite _cachedKeyIcon;
    private void Awake()
    {
        _canvas = transform.GetChild(0).gameObject;
        _imagenUI = _canvas.transform.GetChild(0).gameObject;
        _textoLetraImagenUI = _imagenUI.transform.GetChild(0).gameObject;
        _textoUI = _canvas.transform.GetChild(1).gameObject;
        interactables = new List<Interactable>();

    }
    private void Start()
    {
        inputManager = GameManager.Instance.gameObject.GetComponent<InputManager>();
        diverseMenusManager = GameManager.Instance.gameObject.GetComponent<DiverseMenusManager>();

        inputManager.onInteract += OnInteract;
        RefreshInteractBinding();

        _playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_puedeInteractuar)
        {
            _canvas.SetActive(true);

            if (_cachedKeyIcon != null)
            {
                _imagenUI.GetComponent<UnityEngine.UI.Image>().sprite = _cachedKeyIcon;
                _textoLetraImagenUI.SetActive(false);
            }
            else
            {
                _imagenUI.GetComponent<UnityEngine.UI.Image>().sprite = _keySprite;
                _textoLetraImagenUI.SetActive(true);
                _textoLetraImagenUI.GetComponent<TextMeshProUGUI>().text = _cachedBindingText;
            }
        }
        else
        {
            _canvas.SetActive(false);
        }
    }

    public void OnInteract()
    {

        if (_puedeInteractuar && !diverseMenusManager._IsOpen && (_playerController.GetState() == PlayerController.PlayerState.Move || _playerController.GetState() == PlayerController.PlayerState.Rest))
        {
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
        string effectivePath = inputManager._InteractRef.action.bindings[0].effectivePath;

        _cachedKeyIcon = keyIconDatabase.GetIcon(effectivePath);

        _cachedBindingText = InputControlPath.ToHumanReadableString(
         effectivePath,
         InputControlPath.HumanReadableStringOptions.OmitDevice
     );
    }

    public void ChangeDisplayText(string text)
    {

        _textoUI.GetComponent<TextMeshProUGUI>().text = text;
    }
}