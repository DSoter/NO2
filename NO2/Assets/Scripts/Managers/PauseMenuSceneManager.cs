using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuSceneManager : MonoBehaviour
{

	public Button defaultButton;
	void Start()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
	void Update()
	{

	}



	public void Reanudar()
	{
		PauseMenuHandler.Instance.TogglePause();

	}

	public void BotonSalir()
	{
		PauseMenuHandler.Instance.QuitToMainMenu();
	}

}
