using System;
using System.Collections;
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
    private InputActionReference _mapRef, _flowerRef, _badgesRef, _scapeRef, _navigateLeft, _navigateRight, _confirm;
    private InputActionReference _goUp, _goDown, _goLeft, _goRight;
    [Header("Input System")]
    [SerializeField] private InputActionAsset _inputSystemReference;

    private bool _wantsToOpen;
    private bool _isOpen;

    private bool _wantsToPause;

    private bool gameOverWithoutFadeIn;



    public event Action onConfirm;



    public bool GameOverWithoutFadeIn
    {
        get { return gameOverWithoutFadeIn; }
        set { gameOverWithoutFadeIn = value; }
    }


    public bool _WantsToPause
    {
        get { return _wantsToPause; }
        set { _wantsToPause = value; }
    }
    public bool _IsOpen
    {
        get { return _isOpen; }
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
        if(CanOpenInCurrentScene()) {
            _isOpen = !_isOpen;

            if (_isOpen)
            {

                Time.timeScale = 0f;
                SceneManager.LoadScene(menusSceneName, LoadSceneMode.Additive);
            }
            else
            {
                
                if(_menuType == MenuType.Gameover)
                {
                    Time.timeScale = 1f;
                    SceneManager.UnloadSceneAsync(menusSceneName);
                    SceneManager.LoadScene("MenuPrincipal");
                }
                else {
                    //if (exitPauseSound != null)
                    //    GameManager.Instance.audioManager.PlaySound(exitPauseSound);
                    CloseFlowers();
                    Time.timeScale = 1f;
                    SceneManager.UnloadSceneAsync(menusSceneName);

                }
            }
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
            //_menuType = _futureMenuType; //esto no hace falta
            switch (_menuType)
            {     
                case MenuType.Map:
                    switch (_futureMenuType)
                    {
                        case MenuType.Badges:
                            SlideRight();
                            break;
                        case MenuType.Flowers:
                            SlideLeft(); break;
                    }
                    break;
                case MenuType.Badges:
                    switch (_futureMenuType)
                    {
                        case MenuType.Flowers:
                            SlideRight();
                            break;
                        case MenuType.Map:
                            SlideLeft(); break;
                    }break;
                case MenuType.Flowers:

                    CloseFlowers();
                    switch (_futureMenuType)
                    {
                        case MenuType.Badges:
                            SlideLeft();
                            break;
                        case MenuType.Map:
                            SlideRight(); break;
                    }
                    break;

            }
        }

    }

    private bool CanOpenInCurrentScene()
    {
        string escenaActiva = SceneManager.GetActiveScene().name;
        Scene scene = SceneManager.GetSceneByName("PauseMenu");

        // Comprobar si la escena está cargada (incluyendo modo Additive)
        if (scene.isLoaded) { return false; }

        return escenaActiva != "MenuPrincipal" && escenaActiva != "Splash";
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
    public void OnNavigateLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isOpen)
            {
                SlideLeft();                
            }
        }
    }
    public void OnNavigateRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isOpen)
            {
                SlideRight();
            }
        }
    }
    public void OnConfirm(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            onConfirm?.Invoke();
            if (_isOpen)
            {
                switch (_menuType) 
                {
                    case MenuType.Map:
                        //ya veremos que se hace
                        break;
                    case MenuType.Flowers:
                        OnActivateFlowers();

                        break;
                    case MenuType.Badges:
                        //ya veremos que se hace
                        break;
                }
                    
            }
        }
    }
    private void OnActivateFlowers()
    {
        ChangeFlowersManager cf = FindAnyObjectByType<ChangeFlowersManager>();
        cf.ActivateMenu();
    }

    public void OnGoUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isOpen)
            {
                switch (_menuType)
                {
                    case MenuType.Map:
                        //ya veremos que se hace
                        break;
                    case MenuType.Flowers:
                        GoUpFlowers();
                        break;
                    case MenuType.Badges:
                        //ya veremos que se hace
                        break;
                }

            }
        }
    }

    public void OnGoDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isOpen)
            {
                switch (_menuType)
                {
                    case MenuType.Map:
                        //ya veremos que se hace
                        break;
                    case MenuType.Flowers:
                        GoDownFlowers();
                        break;
                    case MenuType.Badges:
                        //ya veremos que se hace
                        break;
                }

            }
        }
    }
    public void OnGoLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isOpen)
            {
                switch (_menuType)
                {
                    case MenuType.Map:
                        //ya veremos que se hace
                        break;
                    case MenuType.Flowers:
                        //ya veremos que se hace
                        break;
                    case MenuType.Badges:
                        //ya veremos que se hace
                        break;
                }

            }
        }
    }

    public void OnGoRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_isOpen)
            {
                switch (_menuType)
                {
                    case MenuType.Map:
                        //ya veremos que se hace
                        break;
                    case MenuType.Flowers:
                        //ya veremos que se hace
                        break;
                    case MenuType.Badges:
                        //ya veremos que se hace
                        break;
                }

            }
        }
    }

    public void SlideRight()
    {
        MenusHandler mh = FindAnyObjectByType<MenusHandler>();
        PanelWipeSwitcher pw = FindAnyObjectByType<PanelWipeSwitcher>();
        if (!pw.IsTransitioning)
        {
            pw.OnSwitchRight();
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
                    //mh.OpenBadgesMenu(); 
                    break;
                case MenuType.Badges:
                    _menuType = MenuType.Flowers;
                    //mh.OpenFlowerMenu();
                    break;
                case MenuType.Flowers:
                    //cerramos el menu d flores

                    CloseFlowers();

                    _menuType = MenuType.Map;
                    //mh.OpenMapMenu();
                    break;

            }
        }
    }
    public void SlideLeft()
    {
        MenusHandler mh = FindAnyObjectByType<MenusHandler>();
        PanelWipeSwitcher pw = FindAnyObjectByType<PanelWipeSwitcher>();
        if (!pw.IsTransitioning)
        {
            pw.OnSwitchLeft();
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
                    _menuType = MenuType.Flowers;
                    //mh.OpenFlowerMenu();
                    break;
                case MenuType.Badges:
                    _menuType = MenuType.Map;
                    //mh.OpenMapMenu();
                    break;
                case MenuType.Flowers:
                    CloseFlowers();


                    _menuType = MenuType.Badges;
                    //mh.OpenBadgesMenu(); 
                    break;

            }
        }

    }

    private void CloseFlowers()
    {
        ChangeFlowersManager cf = FindAnyObjectByType<ChangeFlowersManager>();
        if (cf != null)
        {
            cf.DeactivateMenu();
        }
        
        
    }
    private void GoUpFlowers()
    {
        ChangeFlowersManager cf = FindAnyObjectByType<ChangeFlowersManager>();
        if (cf != null)
        {
            cf.GoUp();
        }
    }
    private void GoDownFlowers()
    {
        ChangeFlowersManager cf = FindAnyObjectByType<ChangeFlowersManager>();
        if (cf != null)
        {
            cf.GoDown();
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
        _navigateLeft = InputActionReference.Create(UIMap.FindAction("NavigateLeft"));
        _navigateRight = InputActionReference.Create(UIMap.FindAction("NavigateRight"));
        _confirm = InputActionReference.Create(UIMap.FindAction("Confirm"));
        _goUp = InputActionReference.Create(UIMap.FindAction("GoUp"));
        _goDown = InputActionReference.Create(UIMap.FindAction("GoDown"));
        _goRight = InputActionReference.Create(UIMap.FindAction("GoRight"));
        _goLeft = InputActionReference.Create(UIMap.FindAction("GoLeft"));
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
            _navigateLeft.action.performed -= OnNavigateLeft;
            _navigateRight.action.performed -= OnNavigateRight;
            _confirm.action.performed -= OnConfirm;
            _goUp.action.performed -= OnGoUp;
            _goDown.action.performed -= OnGoDown;
            _goRight.action.performed -= OnGoRight;
            _goLeft.action.performed -= OnGoLeft;
            //_scapeRef.action.performed -= OnEscape;


            _inputSystemReference.FindActionMap("Player").Disable();
        }
    }

    private void EnableActions()
    {
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

        //_scapeRef.action.performed += OnEscape;



    }


}
