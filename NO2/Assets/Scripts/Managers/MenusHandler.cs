using System.Runtime.CompilerServices;
using UnityEngine;

public class MenusHandler : MonoBehaviour
{
    [SerializeField] GameObject _gameOverCanvas;
    [SerializeField] GameObject _flowerCanvas;
    [SerializeField] GameObject _badgesCanvas;
    [SerializeField] GameObject _mapCanvas;

    [SerializeField] PopUp _gameOverPopUp;

    private CheckpointManager _checkpointManager;
    private DiverseMenusManager _diverseMenusManager;




    public void Awake()
    {
        _diverseMenusManager = GameManager.Instance.GetComponent<DiverseMenusManager>();

        switch (_diverseMenusManager.MenuIndex)
        {
            //GameOver 0
            //Map 1
            //Badges 2
            //Flowers 3
            case 0:
                OpenGameOver();
                break;
            case 1:
                OpenMapMenu();
                break;
            case 2:
                OpenBadgesMenu();
                break;
            case 3:
                OpenFlowerMenu();
                break;

                

        }
    }

    public void OpenGameOver()
    {
        _flowerCanvas.SetActive(false);
        _badgesCanvas.SetActive(false);
        _mapCanvas.SetActive(false);

        _checkpointManager = GameManager.Instance.GetComponent<CheckpointManager>();
        _diverseMenusManager = GameManager.Instance.GetComponent<DiverseMenusManager>();

        //_gameOverCanvas.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _gameOverPopUp.Show(
            "Has muerto \n ¿Deseas volver a intentarlo?",
            onConfirm: () => _checkpointManager.RespawnAfterGameOver(),
            onCancel: () => _diverseMenusManager.QuitToMainMenu()
        );
    }

    public void OpenFlowerMenu()
    {
        _flowerCanvas.SetActive(true);
        _badgesCanvas.SetActive(false);
        _mapCanvas.SetActive(false);
    }
    public void OpenMapMenu()
    {
        _flowerCanvas.SetActive(false);
        _badgesCanvas.SetActive(false);
        _mapCanvas.SetActive(true);
    }
    public void OpenBadgesMenu()
    {
        _flowerCanvas.SetActive(false);
        _badgesCanvas.SetActive(true);
        _mapCanvas.SetActive(false);
    }

}
