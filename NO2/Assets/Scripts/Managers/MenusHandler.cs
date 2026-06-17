using System.Runtime.CompilerServices;
using UnityEngine;

public class MenusHandler : MonoBehaviour
{
    [SerializeField] GameObject _gameOverCanvas;
    [SerializeField] GameObject _flowerCanvas;
    [SerializeField] GameObject _badgesCanvas;
    [SerializeField] GameObject _mapCanvas;

    [SerializeField] PopUpFade _gameOverPopUp;

    private CheckpointManager _checkpointManager;
    private DiverseMenusManager _diverseMenusManager;

    


    public void Awake()
    {
        _diverseMenusManager = GameManager.Instance.GetComponent<DiverseMenusManager>();
        Debug.Log(_diverseMenusManager.GetMenuType());
        switch (_diverseMenusManager.GetMenuType())
        {
            //GameOver 0
            //Map 1
            //Badges 2
            //Flowers 3
            case DiverseMenusManager.MenuType.Gameover:
                OpenGameOver();
                break;
            case DiverseMenusManager.MenuType.Map:
                OpenMapMenu();
                break;
            case DiverseMenusManager.MenuType.Badges:
                OpenBadgesMenu();
                break;
            case DiverseMenusManager.MenuType.Flowers:
                OpenFlowerMenu();
                break;

                

        }
    }

    public void OpenGameOver()
    {
        //_gameOverCanvas.SetActive(true);
        _flowerCanvas.SetActive(false);
        _badgesCanvas.SetActive(false);
        _mapCanvas.SetActive(false);

        _checkpointManager = GameManager.Instance.GetComponent<CheckpointManager>();
        _diverseMenusManager = GameManager.Instance.GetComponent<DiverseMenusManager>();

        //_gameOverCanvas.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _gameOverPopUp.Show(
            "Has muerto",
            onConfirm: () => _checkpointManager.RespawnAfterGameOver()
        );
    }

    public void OpenFlowerMenu()
    {
        _gameOverCanvas.SetActive(false);
        _flowerCanvas.SetActive(true);
        _badgesCanvas.SetActive(false);
        _mapCanvas.SetActive(false);
    }
    public void OpenMapMenu()
    {
        _gameOverCanvas.SetActive(false);
        _flowerCanvas.SetActive(false);
        _badgesCanvas.SetActive(false);
        _mapCanvas.SetActive(true);
    }
    public void OpenBadgesMenu()
    {
        _gameOverCanvas.SetActive(false);
        _flowerCanvas.SetActive(false);
        _badgesCanvas.SetActive(true);
        _mapCanvas.SetActive(false);
    }

    public void PushedE(DiverseMenusManager.MenuType prevMenu, DiverseMenusManager.MenuType nextMenu)
    {

    }

    public void PushedQ(DiverseMenusManager.MenuType prevMenu, DiverseMenusManager.MenuType nextMenu)
    {

    }

}
