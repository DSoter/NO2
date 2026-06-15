
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuHandler : MonoBehaviour
{
	public static PauseMenuHandler Instance;

	[SerializeField] private string pauseSceneName = "PauseMenu";
	[SerializeField] private AudioClip enterPauseSound, exitPauseSound;

    // InputSystem 
    private InputSystem m_Actions;
    private InputSystem.UIActions m_UI;

	private bool _wantsToPause;
    public bool isPaused { get; private set; }

	void Awake()
	{
		if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
		else { Destroy(gameObject); }
        m_Actions = new InputSystem();
        m_UI = m_Actions.UI;

        m_UI.Escape.performed += OnEscape;
    }

	void Update()
	{
		if (_wantsToPause)
		{
			_wantsToPause = false;
			if (!CanPauseInCurrentScene()) return;
			TogglePause();
		}

		//if (Input.GetKeyDown(KeyCode.R))
		//{
		//	if (!CanRestartCurrentScene()) return;
		//	RestartCurrentScene();
		//}
	}

	bool CanPauseInCurrentScene()
	{
		string escenaActiva = SceneManager.GetActiveScene().name;
        // Solo permitimos pausar si NO estamos en menus principales
        Scene scene = SceneManager.GetSceneByName("MenusAndGameOver");

        // Comprobar si la escena está cargada (incluyendo modo Additive)
        if (scene.isLoaded) { return false; }

        return escenaActiva != "MenuPrincipal" && escenaActiva != "Splash" && escenaActiva != "MenusAndGameOver" ;
	}

	bool CanRestartCurrentScene()
	{
		return !isPaused && CanPauseInCurrentScene();
	}

	//void RestartCurrentScene()
	//{
	//	Time.timeScale = 1f;
	//	Scene escenaActiva = SceneManager.GetActiveScene();
	//	SceneManager.LoadScene(escenaActiva.name);
	//}

	public void TogglePause()
	{
		isPaused = !isPaused;

		if (isPaused)
		{
			if (enterPauseSound != null)
				GameManager.Instance.audioManager.PlaySound(enterPauseSound);

			Time.timeScale = 0f;
			SceneManager.LoadScene(pauseSceneName, LoadSceneMode.Additive);
		}
		else
		{
			if (exitPauseSound != null)
				GameManager.Instance.audioManager.PlaySound(exitPauseSound);
			Time.timeScale = 1f;
			SceneManager.UnloadSceneAsync(pauseSceneName);

			
		}
	}

	public void QuitToMainMenu()
	{
		isPaused = false;
		Time.timeScale = 1f;
		SceneManager.LoadScene("MenuPrincipal");
	}

    public void OnEscape(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _wantsToPause = true;
            DiverseMenusManager _menuManager = GameManager.Instance.GetComponent<DiverseMenusManager>();
			_menuManager._WantsToPause = true;
        }
    }

    void OnDestroy()
    {
        m_Actions.Dispose();
    }
	void OnEnable()
	{
		m_UI.Enable();
	}
	//void OnDisable()
	//{
	//	m_UI.Disable();
	//}

	

}
