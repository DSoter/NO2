using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuSceneManager : MonoBehaviour
{

    public Button continueButton;
    public Button newGameButton;
    [SerializeField] private string nameFirstScene;
    public GameObject canvasCredits;
    public GameObject canvasOptions;
    [SerializeField] private AudioClip theme;

    [SerializeField] private FlowerCollection coleccionFlores;

    [SerializeField] private HealData healData;
    [SerializeField] private PlayerData playerData;

    [Header("Administrar nueva partida")]
    [SerializeField] private WorldMapData worldMapData;
    [SerializeField] private WorldMapDataRegister worldMapDataRegister;
    [SerializeField] private FogCollection fogCollection;
    [SerializeField] private AchievementCollection achievementCollection;
    [SerializeField] private FlowerCollection flowerCollection;
    [SerializeField] private BadgeCollection badgeCollection;



    void Start()
    {
        if (theme != null)
        {
            GameManager.Instance.audioManager.PlayMusic(theme);
        }
        continueButton.gameObject.SetActive(HasSavedGame());
        //EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
    }

    public void StartGame()
    {
        InitializePlayerValues();
        coleccionFlores.Load();
        GameManager.Instance.GetComponent<CheckpointManager>().StartSceneWithFade(nameFirstScene);
    }
    public void StartNewGame()
    {
        foreach (WorldMapData.SceneMapEntry entry in worldMapData.scenes)
        {
            if (entry.mapData != null)
                entry.mapData.ResetProgressKeepMap();
        }

        foreach (FogData fog in fogCollection.AllFogData)
        {
            if (fog != null)
                fog.Reset();
        }
        worldMapDataRegister.ResetVisited();
        worldMapData.WorldMapNeedsUpdate = true;
        AllConditionsDIalog.ResetSharedProgress();

        foreach (Achievement achievement in achievementCollection.AllAchievements)
        {
            if (achievement != null)
                achievement.Reset();
        }
        flowerCollection.Load();
        flowerCollection.Reset();
        badgeCollection.Load();
        badgeCollection.Reset();

        Debug.Log("Nueva partida iniciada: progreso reseteado, layout de mapas conservado.");

        StartGame();
    }

    private void InitializePlayerValues()
    {

        healData.RemainingUses = healData.MaxUses;
        playerData.Health = playerData.MaxHealth;
        playerData.Oxygen = playerData.MaxOxygen;
        playerData.Stamina = playerData.MaxStamina;

    }

    public bool HasSavedGame()
    {
        return worldMapDataRegister.HasSavedGame();
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
