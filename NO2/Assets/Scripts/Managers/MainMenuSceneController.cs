using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSceneManager : MonoBehaviour
{

    public Button playButton;
    [SerializeField] private string nameFirstScene;
    public GameObject canvasCredits;
    public GameObject canvasOptions;
    [SerializeField] private AudioClip theme;


    void Update()
    {
        if (canvasCredits.activeSelf || canvasOptions.activeSelf)
        {
            //if (Input.anyKeyDown)
            //{
            //    canvasCredits.SetActive(false);
            //    canvasOptions.SetActive(false);

            //}
        }
    }

    void Start()
    {
        if (theme != null)
        {
            GameManager.Instance.audioManager.PlayMusic(theme);
        }
        //EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
    }

    public void StartGame()
    {
        GameManager.Instance.GetComponent<CheckpointManager>().StartScene(nameFirstScene);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    public void OnEnterName(string playerName)
    {
        Debug.Log("PlayerName: " + playerName);
    }

    public void OnSFXVolume(float volume)
    {
        Debug.Log("Volumen de Efectos: " + volume);
    }

    public void OnMusicVolume(float volume)
    {
        Debug.Log("Volumen de Música: " + volume);
    }

}
