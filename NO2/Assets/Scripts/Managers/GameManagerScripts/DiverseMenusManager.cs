using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DiverseMenusManager : MonoBehaviour
{
    private int menuIndex;

    private int menuIndexAux;

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
        if (!_isOpen || menuIndex == menuIndexAux)//Si no hay menu abierto o el menú que está abierto es el mismo se llama a open menus 
        {
            menuIndex = menuIndexAux;
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
            menuIndex = menuIndexAux; //esto no hace falta
            switch (menuIndexAux)
            {
                
                case 1:
                    mh.OpenMapMenu(); break;
                case 2:
                    mh.OpenBadgesMenu(); break;
                case 3:
                    mh.OpenFlowerMenu(); break;

            }
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






    



    //Alpargata hacer que si está en el mapa y se pulsa flores 
    public void OnMap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            menuIndexAux = 1;
            ChangeMenus();
        }
    }
    
    public void OnFlower(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            menuIndexAux = 2;
            ChangeMenus();
        }
    }
    public void OnBadge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            menuIndexAux = 3;
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
                switch (menuIndex)
                {
                    //1Mapa
                    //2Badge
                    //3Flower

                    //Al restar uno se queda
                    //1Flower
                    //2Mapa
                    //3Badge
                    case 1:
                        menuIndex = 3;
                        mh.OpenFlowerMenu(); break;
                    case 2:
                        menuIndex = 1;
                        mh.OpenMapMenu(); break;
                    case 3:
                        menuIndex = 2;
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
                switch (menuIndex)
                {
                    //1Mapa
                    //2Badge
                    //3Flower

                    //Al sumar uno se queda
                    //1Badge
                    //2Flower
                    //3Mapa
                    case 1:
                        menuIndex = 2;
                        mh.OpenBadgesMenu(); break;
                    case 2:
                        menuIndex = 3;
                        mh.OpenFlowerMenu(); break;
                    case 3:
                        menuIndex = 1;
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
