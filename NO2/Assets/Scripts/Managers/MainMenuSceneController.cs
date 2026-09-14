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


    [SerializeField] private HealData healData;
    [SerializeField] private PlayerData playerData;

    [Header("Administrar nueva partida")]
    [SerializeField] private WorldMapData worldMapData;
    [SerializeField] private WorldMapDataRegister worldMapDataRegister;
    [SerializeField] private FogCollection fogCollection;
    [SerializeField] private AchievementCollection achievementCollection;
    [SerializeField] private FlowerCollection flowerCollection;
    [SerializeField] private BadgeCollection badgeCollection;
    [SerializeField] private MoneyData moneyData;
    [SerializeField] private RespawnData respawnData;

    [Header("Claves de bosses")]
    [SerializeField] private string saveKeySnailBoss = "SnailKingIsDead";

    private string gameStarted = "HasSavedGame";




    void Start()
    {
        if (theme != null)
        {
            GameManager.Instance.audioManager.PlayMusic(theme);
        }
        continueButton.interactable = HasSavedGame();
        //continueButton.gameObject.SetActive(HasSavedGame());
        //EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
    }

    public void StartGame()
    {
        InitializePlayerValues();

        flowerCollection.Load();

        moneyData.Load();

        respawnData.Load();

        CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();
        cm.SceneWhereRespawn = respawnData.SceneWhereRespawn;
        cm.IdRespawn = respawnData.CheckpointId;
        cm.HasToSpawnPlayerAfterDeath = true;
        cm.ResetAllDeathRespawns();
        cm.ResetAllRestRespawns();

        //GameManager.Instance.GetComponent<CheckpointManager>().StartSceneWithFade(nameFirstScene);
        FadeTransition.Instance.LoadSceneWithFade(respawnData.SceneWhereRespawn);
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
        CharacterInteractable.ResetAllConversationsProgress();
        GameManager.Instance.GetComponent<AchievementManager>().ResetAll();
        respawnData.Reset();

        ResetBossKilled();

        PlayerPrefs.SetInt(gameStarted, 1);

        flowerCollection.Load();
        flowerCollection.Reset();
        badgeCollection.Load();
        badgeCollection.Reset();
        moneyData.Reset();
        playerData.EquipedFlower = null;

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
    private void ResetBossKilled()
    {
        PlayerPrefs.SetInt(saveKeySnailBoss, 0);
        PlayerPrefs.Save();
    }

    public bool HasSavedGame()
    {
        return PlayerPrefs.GetInt(gameStarted, 0) == 1 ;
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
