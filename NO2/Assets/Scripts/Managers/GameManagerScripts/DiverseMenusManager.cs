using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DiverseMenusManager : MonoBehaviour
{


    //GameOver 0
    //Map 1
    //Bandges 2
    //Flowers 3


    private MenuType _menuType;
    private MenuType _futureMenuType;

    public MenuType GetMenuType()
    {
        return _menuType;
    }
    public void SetMenuType(MenuType menuType)
    {
        _menuType= menuType;
    }

    public enum MenuType
    {
        Gameover,
        Map,
        Badges,
        Flowers
    }


    [SerializeField] private string menusSceneName = "MenusAndGameOver";

    // InputSystem 
    private InputActionReference _mapRef, _flowerRef, _badgesRef, _scapeRef, _goLeft, _goRight;
    [Header("Input System")]
    [SerializeField] private InputActionAsset _inputSystemReference;

    private bool _wantsToOpen;
    private bool _isOpen;

    private bool _wantsToPause;

    public bool _WantsToPause
    {
        get { return _wantsToPause; }
        set { _wantsToPause = value; }
    }

    private void Awake()
    {
        InitializePrefsActions();
    }

    private void Update()
    {
        if (_wantsToPause)
        {
            _wantsToPause = false;
            if (_isOpen)
            {
                _wantsToOpen = true;
            }
        }
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

    public void ChangeMenus()
    {
        //if (!CanOpenInCurrentScene()) { return; }
        if (!_isOpen || _menuType == _futureMenuType)//Si no hay menu abierto o el menú que está abierto es el mismo se llama a open menus 
        {
            _menuType = _futureMenuType;
            OpenMenus();    
        }
        else
        {
            MenusHandler mh = FindAnyObjectByType<MenusHandler>();
            if (mh == null) 
            {
                Debug.Log("No se ha encontrado el menu handler");
                return;
            }
            if (_menuType == MenuType.Gameover)
            {
                Debug.Log("Se ha intentado abrir un menú estando gameOver, no se cierra");
            }
            _menuType = _futureMenuType; //esto no hace falta
            switch (_menuType)
            {
                
                case MenuType.Map:
                    mh.OpenMapMenu(); break;
                case MenuType.Badges:
                    mh.OpenBadgesMenu(); break;
                case MenuType.Flowers:
                    mh.OpenFlowerMenu(); break;

            }
        }

    }

    //private bool CanOpenInCurrentScene()
    //{
    //    string escenaActiva = SceneManager.GetActiveScene().name;
    //    // Solo permitimos pausar si NO estamos en menus principales
    //    Scene scene = SceneManager.GetSceneByName("PauseMenu");

    //    // Comprobar si la escena está cargada (incluyendo modo Additive)
    //    if (scene.isLoaded) { return false; }

    //    return escenaActiva != "MenuPrincipal" && escenaActiva != "Splash";
    //}

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






    



    //Alpargata hacer que si está en el mapa y se pulsa flores 
    public void OnMap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _futureMenuType = MenuType.Map;
            ChangeMenus();
        }
    }
    
    public void OnFlower(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _futureMenuType = MenuType.Flowers;
            ChangeMenus();
        }
    }
    public void OnBadge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _futureMenuType = MenuType.Badges;
            ChangeMenus();
        }
    }
    public void OnLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isOpen)
            {
                MenusHandler mh = FindAnyObjectByType<MenusHandler>();
                switch (_menuType)
                {
                    //1Mapa
                    //2Badge
                    //3Flower

                    //Al restar uno se queda
                    //1Flower
                    //2Mapa
                    //3Badge
                    case MenuType.Map:
                        _menuType= MenuType.Flowers;
                        mh.OpenFlowerMenu(); break;
                    case MenuType.Badges:
                        _menuType=MenuType.Map;
                        mh.OpenMapMenu(); break;
                    case MenuType.Flowers:
                        _menuType=MenuType.Badges;
                        mh.OpenBadgesMenu(); break;

                }
            }
        }
    }
    public void OnRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isOpen)
            {
                MenusHandler mh = FindAnyObjectByType<MenusHandler>();
                switch (_menuType)
                {
                    //1Mapa
                    //2Badge
                    //3Flower

                    //Al sumar uno se queda
                    //1Badge
                    //2Flower
                    //3Mapa
                    case MenuType.Map:
                        _menuType = MenuType.Badges;
                        mh.OpenBadgesMenu(); break;
                    case MenuType.Badges:
                        _menuType = MenuType.Flowers;
                        mh.OpenFlowerMenu(); break;
                    case MenuType.Flowers:
                        _menuType = MenuType.Map;
                        mh.OpenMapMenu(); break;

                }
            }
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
        _goLeft = InputActionReference.Create(UIMap.FindAction("GoLeft"));
        _goRight = InputActionReference.Create(UIMap.FindAction("GoRight"));

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
            _goLeft.action.performed -= OnLeft;
            _goRight.action.performed -= OnRight;

            //_scapeRef.action.performed -= OnEscape;


            _inputSystemReference.FindActionMap("Player").Disable();
        }
    }

    private void EnableActions()
    {
        _mapRef.action.performed += OnMap;
        _flowerRef.action.performed += OnFlower;
        _badgesRef.action.performed += OnBadge;
        _goLeft.action.performed += OnLeft;
        _goRight.action.performed += OnRight;

        //_scapeRef.action.performed += OnEscape;



    }


}
