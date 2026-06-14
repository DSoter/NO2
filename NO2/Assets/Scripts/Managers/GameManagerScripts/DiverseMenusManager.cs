using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DiverseMenusManager : MonoBehaviour
{
    private int menuIndex;

    //GameOver 0
    //Map 1
    //Bandges 2
    //Flowers 3

    public int MenuIndex
    {
        get { return menuIndex; }
        set { menuIndex = value; }
    }


    [SerializeField] private string menusSceneName = "MenusAndGameOver";

    // InputSystem 
    private InputActionReference _mapRef, _flowerRef, _badgesRef, _scapeRef;
    [Header("Input System")]
    [SerializeField] private InputActionAsset _inputSystemReference;

    private bool _wantsToOpen;
    private bool _isOpen;

    private void Awake()
    {
        InitializePrefsActions();
    }

    private void Update()
    {
        if (_wantsToOpen)
        {
            _wantsToOpen= false;
            OpenMenus();
        }
    }

    public void OpenMenus()
    {
        _isOpen = !_isOpen;

        if (_isOpen)
        {
            //if (enterPauseSound != null)
            //    GameManager.Instance.audioManager.PlaySound(enterPauseSound);

            Time.timeScale = 0f;
            SceneManager.LoadScene(menusSceneName, LoadSceneMode.Additive);
        }
        else
        {
            //if (exitPauseSound != null)
            //    GameManager.Instance.audioManager.PlaySound(exitPauseSound);
            Time.timeScale = 1f;
            SceneManager.UnloadSceneAsync(menusSceneName);


        }
    }

    public void QuitToMainMenu()
    {
        _isOpen = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
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






    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //_wantsToPause = true;
            //OpenMenus();
        }
    }



    //Alpargata hacer que si está en el mapa y se pulsa flores 
    public void OnMap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            menuIndex = 1;
            OpenMenus();
        }
    }
    
    public void OnFlower(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            menuIndex = 2;
            OpenMenus();
        }
    }
    public void OnBadge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            menuIndex = 3;
            OpenMenus();
        }
    }

    private void InitializePrefsActions()
    {


        string json = PlayerPrefs.GetString("rebinds", "");
        if (!string.IsNullOrEmpty(json))
        {
            _inputSystemReference.LoadBindingOverridesFromJson(json);
        }

        InputActionMap UIMap = _inputSystemReference.FindActionMap("UI");

        _mapRef = InputActionReference.Create(UIMap.FindAction("OpenMap"));
        _flowerRef = InputActionReference.Create(UIMap.FindAction("OpenFlowers"));
        _badgesRef = InputActionReference.Create(UIMap.FindAction("OpenBadges"));
        _scapeRef = InputActionReference.Create(UIMap.FindAction("Escape"));

        EnableActions();
        UIMap.Enable();
    }
    private void DisposeActions()
    {
        if (_mapRef != null)
        {
            _mapRef.action.performed -= OnMap;
            _flowerRef.action.performed -= OnFlower;
            _badgesRef.action.performed  -= OnBadge;
            _scapeRef.action.performed -= OnEscape;
            _inputSystemReference.FindActionMap("Player").Disable();
        }
    }

    private void EnableActions()
    {
        _mapRef.action.performed += OnMap;
        _flowerRef.action.performed += OnFlower;
        _badgesRef.action.performed += OnBadge;
        _scapeRef.action.performed += OnEscape;
        //weakAttackRef.action.performed += OnWeakAttack;
        //strongAttackRef.action.performed += OnStrongAttack;
    }


}
