using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuSceneManager : MonoBehaviour
{

	public Button defaultButton;
	[SerializeField] private WorldMapData worldMapData;
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
        UpdateScenesVisited();
        PauseMenuHandler.Instance.QuitToMainMenu();
	}

    private void UpdateScenesVisited()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        WorldMapDataRegister.ScenesVisited scenesVisited = GameManager.Instance.GetComponent<WorldMapDataRegister>().Scenes;
        if (!scenesVisited.references.Contains(sceneName))
        {
            scenesVisited.references.Add(sceneName);
            worldMapData.WorldMapNeedsUpdate = true;
            GameManager.Instance.GetComponent<WorldMapDataRegister>().SaveVisited();
        }

    }

}
