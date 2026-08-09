using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllConditionsDIalog : MonoBehaviour
{
    [SerializeField] FlowerCollection flowerCollection;
    [SerializeField] BadgeCollection badgeCollection;
    private PlayerData _playerData;
    private PlayerController _playerController;

    // --- Estado COMPARTIDO entre todas las instancias (static) ---
    private static bool talkedToTheoAboutFlowersFirstTime;
    private static bool talkedToTheoAboutFlowersSecondTime;
    private static bool talkedToRusellAboutBadgesFirstTime;
    private static bool talkedToRusellAboutBadgesSecondTime;
    private static bool stateLoaded = false; // para cargar de PlayerPrefs solo una vez

    // --- Estado propio de ESTA instancia (no compartido) ---
    private bool destroyedByFlowerUnlock;

    private bool itemPurchased; // TODO: sin implementar todavía, no se persiste

    private const string KeyFirstTheo = "ACD_talkedTheoFlowersFirst";
    private const string KeySecondTheo = "ACD_talkedTheoFlowersSecond";
    private const string KeyFirstRusell = "ACD_talkedRusellBadgesFirst";
    private const string KeySecondRusell = "ACD_talkedRusellBadgesSecond";
    private string KeyDestroyed => "ACD_destroyed_" + gameObject.name; // por instancia

    private const string KeyDestroyedRegistry = "ACD_destroyedKeysRegistry";

    private void Awake()
    {
        LoadSharedStateOnce();
        LoadInstanceState();

        if (destroyedByFlowerUnlock)
        {
            Destroy(gameObject);
        }
    }

    private static void LoadSharedStateOnce()
    {
        if (stateLoaded) return; // ya cargado por otra instancia en esta sesión, no pisar cambios en memoria

        talkedToTheoAboutFlowersFirstTime = PlayerPrefs.GetInt(KeyFirstTheo, 0) == 1;
        talkedToTheoAboutFlowersSecondTime = PlayerPrefs.GetInt(KeySecondTheo, 0) == 1;
        talkedToRusellAboutBadgesFirstTime = PlayerPrefs.GetInt(KeyFirstRusell, 0) == 1;
        talkedToRusellAboutBadgesSecondTime = PlayerPrefs.GetInt(KeySecondRusell, 0) == 1;
        stateLoaded = true;
    }

    private static void SaveSharedState()
    {
        PlayerPrefs.SetInt(KeyFirstTheo, talkedToTheoAboutFlowersFirstTime ? 1 : 0);
        PlayerPrefs.SetInt(KeySecondTheo, talkedToTheoAboutFlowersSecondTime ? 1 : 0);
        PlayerPrefs.SetInt(KeyFirstRusell, talkedToRusellAboutBadgesFirstTime ? 1 : 0);
        PlayerPrefs.SetInt(KeySecondRusell, talkedToRusellAboutBadgesSecondTime ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadInstanceState()
    {
        destroyedByFlowerUnlock = PlayerPrefs.GetInt(KeyDestroyed, 0) == 1;
    }

    private void SaveInstanceState()
    {
        PlayerPrefs.SetInt(KeyDestroyed, destroyedByFlowerUnlock ? 1 : 0);
        if (destroyedByFlowerUnlock)
            RegisterDestroyedKey(KeyDestroyed);
        PlayerPrefs.Save();
    }
    private static void RegisterDestroyedKey(string key)
    {
        string saved = PlayerPrefs.GetString(KeyDestroyedRegistry, "");
        List<string> keys = string.IsNullOrEmpty(saved) ? new List<string>() : new List<string>(saved.Split(','));
        if (!keys.Contains(key))
        {
            keys.Add(key);
            PlayerPrefs.SetString(KeyDestroyedRegistry, string.Join(",", keys));
        }
    }

    [ContextMenu("Reset All Conversations Progress")]
    public void ResetAllConversationsProgress()
    {
        ResetSharedProgress();
    }

    public static void ResetSharedProgress()
    {
        talkedToTheoAboutFlowersFirstTime = false;
        talkedToTheoAboutFlowersSecondTime = false;
        talkedToRusellAboutBadgesFirstTime = false;
        talkedToRusellAboutBadgesSecondTime = false;

        PlayerPrefs.DeleteKey(KeyFirstTheo);
        PlayerPrefs.DeleteKey(KeySecondTheo);
        PlayerPrefs.DeleteKey(KeyFirstRusell);
        PlayerPrefs.DeleteKey(KeySecondRusell);

        string saved = PlayerPrefs.GetString(KeyDestroyedRegistry, "");
        if (!string.IsNullOrEmpty(saved))
        {
            foreach (string key in saved.Split(','))
                PlayerPrefs.DeleteKey(key);
            PlayerPrefs.DeleteKey(KeyDestroyedRegistry);
        }

        PlayerPrefs.Save();
        Debug.Log("Progreso de conversaciones (AllConditionsDIalog) reiniciado, incluyendo NPCs destruidos.");
    }


    public bool FlowerIsUnlocked()
    {
        return flowerCollection.unlockedFlowers.ContainsValue(true);
    }
    public bool BadgeIsUnlocked()
    {
        return badgeCollection.UnlockedBadges.ContainsValue(true);
    }
    public bool FiveBadgesIsUnlocked()
    {
        int badgesUnlockedCount = 0;
        foreach(Badge key in badgeCollection.UnlockedBadges.Keys)
        {
            if (badgeCollection.UnlockedBadges[key])
            {
                badgesUnlockedCount++;
                
            }
        }
        return badgesUnlockedCount>=5;
    }

    public bool FlowerIsUnlockedThenDestroy()
    {
        if (destroyedByFlowerUnlock) return true;

        if (flowerCollection.unlockedFlowers.ContainsValue(true))
        {
            destroyedByFlowerUnlock = true;
            SaveInstanceState();
            StartCoroutine(Bazinga());
            return true;
        }
        return false;
    }

    private IEnumerator Bazinga()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }

    public bool FlowerIsUnlockedThenFinishTheoFirstDialog()
    {

        if (flowerCollection.unlockedFlowers.ContainsValue(true))
        {
            talkedToTheoAboutFlowersFirstTime = true;
            SaveSharedState();
            return true;
        }
        return false;
    }
    public bool FirstTheoDialogIsFinished()
    {
        return talkedToTheoAboutFlowersFirstTime;
    }
    public bool FiveBadgesIsUnlockedThenFinishRusellFirstDialog()
    {
        int badgesUnlockedCount = 0;
        foreach (bool value in badgeCollection.UnlockedBadges.Values)
        {
            if (value)
            {
                badgesUnlockedCount++;
            }
        }
        if(badgesUnlockedCount>=5)
        {
            talkedToRusellAboutBadgesFirstTime = true;
            SaveSharedState();
            return true;
        }
        return false;
    }
    public bool RusellGiveFirstBadge()
    {
        GameManager.Instance.GetComponent<AchievementManager>().NotifyEvent("first_badge");
        return true;
    }
    public bool RusellGiveSecondBadge()
    {
        GameManager.Instance.GetComponent<AchievementManager>().NotifyEvent("second_badge");
        Debug.Log("Segunda insignia otorgada");
        return true;
    }
    public bool FirstRusellDialogIsFinished()
    {
        return talkedToRusellAboutBadgesFirstTime;
    }

    public bool FlowerIsUnlockedThenFinishTheoSecondDialog()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        if (_playerController != null)
        {
            _playerData = _playerController._PlayerData;
            if (_playerData.EquipedFlower != null)
            {
                talkedToTheoAboutFlowersSecondTime = true;
                SaveSharedState();
                return true;
            }
            return false;
        }
        Debug.Log("No se ha encontrado el jugador");
        return false;
    }

    public bool SecondTheoDialogIsFinished()
    {
        return talkedToTheoAboutFlowersSecondTime;
    }

    public bool FireFlowerUnlocked()
    {
        if (flowerCollection != null)
            return flowerCollection.unlockedFlowers[flowerCollection.allFlowers[0]];
        return false;
    }
    public bool BellFlowerUnlocked()
    {
        if (flowerCollection != null)
            return flowerCollection.unlockedFlowers[flowerCollection.allFlowers[1]];
        return false;
    }
    public bool VolatilFlowerUnlocked()
    {
        if (flowerCollection != null)
            return flowerCollection.unlockedFlowers[flowerCollection.allFlowers[2]];
        return false;
    }
    public bool LethalFlowerUnlocked()
    {
        if (flowerCollection != null)
            return flowerCollection.unlockedFlowers[flowerCollection.allFlowers[3]];
        return false;
    }

    public bool ItemPurchased()
    {
        // TODO: sin implementar todavía
        if (itemPurchased)
        {
            itemPurchased = false;
            return true;
        }
        return false;
    }
}