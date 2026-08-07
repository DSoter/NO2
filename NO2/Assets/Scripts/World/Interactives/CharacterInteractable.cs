using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterInteractable : Interactable
{
    [SerializeField] FriendlyCharacterData characterData;
    [SerializeField] int conversationNumber = 1; //se usa para acceder a la lista de dialogos pero empieza en uno

    [Header("Guardado")]
    [Tooltip("Identificador único para esta instancia. Debe ser distinto para cada personaje/instancia en el juego.")]
    [SerializeField] private string saveId;
    private const string SaveIdRegistryKey = "CharacterInteractable_AllSaveIds";

    private AllConditionsDIalog acd;
    private DialogTextData fullConversation;
    private List<DialogSequence> normalTexts;
    private DialogSequence actualConversation;
    private List<DialogSequence> repetitionTexts;
    private int timesTalked;
    private DialogBox dialogBox;
    private bool dialogIsOpen;
    private bool dialogIsRecentlyOpen;

    public string SaveId => saveId;

    private string TimesTalkedKey => saveId + "_timesTalked";

    public bool DialogIsOpen
    {
        get{ return dialogIsOpen; }
        set{ dialogIsOpen = value; }

    }
    public bool DialogIsRecentlyOpen
    {
        get { return dialogIsRecentlyOpen; }
        set { dialogIsRecentlyOpen = value; }
    }
    public FriendlyCharacterData CharacterData
    {
        get { return characterData; }
        set { characterData = value; }
    }

    private Dictionary<string, Func<bool>> conditions;
        
    private void Awake()
    {
        dialogBox = FindAnyObjectByType<DialogBox>();
        acd = GetComponent<AllConditionsDIalog>();
        conditions = new Dictionary<string, Func<bool>>
        {
            //theo
            { "anyFlowerUnlocked",   () => acd.FlowerIsUnlocked()},
            { "anyFlowerUnlockedThenDestroy",   () => acd.FlowerIsUnlockedThenDestroy()},

            {"anyFlowerUnlockedAndNotTheoFirstDialogFinished", () => acd.FlowerIsUnlocked() && !acd.FirstTheoDialogIsFinished() },
            {"anyFlowerUnlockedThenTheoFirstDialogFinished", () =>  !acd.FirstTheoDialogIsFinished() && acd.FlowerIsUnlockedThenFinishTheoFirstDialog()  },
            {"anyFlowerUnlockedAndTheoFirstDialogFinished", () => acd.FlowerIsUnlocked() && acd.FirstTheoDialogIsFinished() },
            {"anyFlowerUnlockedThenTheoSecondDialogFinished", () => acd.FlowerIsUnlockedThenFinishTheoSecondDialog() },
            {"anyFlowerUnlockedAndTheoSecondDialogFinished", () => acd.FlowerIsUnlocked() && acd.SecondTheoDialogIsFinished() },

            { "noneFlowerUnlocked", () => !acd.FlowerIsUnlocked()},

            //Russell
            { "anyBadgeUnlocked",   () => acd.BadgeIsUnlocked()},


            {"fiveBadgeUnlockedAndNotRusellFirstDialogFinished", () => acd.FiveBadgesIsUnlocked() && !acd.FirstRusellDialogIsFinished()},
            {"fiveBadgeUnlockedAndNotRusellFirstDialogFinishedThenGiveSecondBadge", () => acd.FiveBadgesIsUnlocked() && !acd.FirstRusellDialogIsFinished() && acd.RusellGiveSecondBadge()},
            {"fiveBadgesUnlockedThenRusellFirstDialogFinished", () =>  !acd.FirstRusellDialogIsFinished() && acd.FiveBadgesIsUnlockedThenFinishRusellFirstDialog()  },
            {"fiveBadgesUnlockedAndRusellFirstDialogFinished", () => acd.FiveBadgesIsUnlocked() && acd.FirstRusellDialogIsFinished() },

            { "noneBadgeUnlocked",   () => !acd.BadgeIsUnlocked()},
            { "notFiveBadgesUnlocked",   () => !acd.FiveBadgesIsUnlocked()},
            { "fiveBadgesUnlocked",   () => acd.FiveBadgesIsUnlocked()},
            { "noneBadgeUnlockedAndThenUnlockFirstBadge",   () => !acd.BadgeIsUnlocked() && acd.RusellGiveFirstBadge()},
            { "anyBadgeUnlockedAndThenUnlockFirstBadge",   () => acd.BadgeIsUnlocked() && acd.RusellGiveFirstBadge()},

            
            // añadir TODAS las condiciones
        };
        LoadState();
    }
    private void LoadState()
    {
        if (string.IsNullOrEmpty(saveId))
        {
            Debug.LogWarning($"{gameObject.name}: CharacterInteractable no tiene saveId asignado, el progreso no se guardará correctamente.");
            return;
        }
        timesTalked = PlayerPrefs.GetInt(TimesTalkedKey, 0);
        RegisterSaveId(saveId);
    }

    private void SaveState()
    {
        if (string.IsNullOrEmpty(saveId)) return;
        PlayerPrefs.SetInt(TimesTalkedKey, timesTalked);
        PlayerPrefs.Save();
    }

    private static void RegisterSaveId(string id)
    {
        string existing = PlayerPrefs.GetString(SaveIdRegistryKey, "");
        List<string> ids = string.IsNullOrEmpty(existing)
            ? new List<string>()
            : new List<string>(existing.Split(','));

        if (!ids.Contains(id))
        {
            ids.Add(id);
            PlayerPrefs.SetString(SaveIdRegistryKey, string.Join(",", ids));
            PlayerPrefs.Save();
        }
    }

    [ContextMenu("Reset Conversation Progress")]
    public void ResetConversationProgress()
    {
        if (string.IsNullOrEmpty(saveId))
        {
            Debug.LogWarning($"{gameObject.name}: no tiene saveId asignado, no se puede reiniciar el progreso.");
            return;
        }

        timesTalked = 0;
        PlayerPrefs.DeleteKey(TimesTalkedKey);
        PlayerPrefs.Save();

        Debug.Log($"Progreso de conversación reiniciado para '{saveId}'.");
    }

    public static void ResetAllConversationsProgress()
    {
        string existing = PlayerPrefs.GetString(SaveIdRegistryKey, "");
        if (string.IsNullOrEmpty(existing))
        {
            Debug.Log("No hay personajes registrados, nada que reiniciar.");
            return;
        }

        string[] ids = existing.Split(',');
        foreach (string id in ids)
        {
            if (string.IsNullOrEmpty(id)) continue;
            PlayerPrefs.DeleteKey(id + "_timesTalked");
        }
        PlayerPrefs.Save();

        Debug.Log($"Progreso de conversación reiniciado para {ids.Length} personajes.");
    }

    public bool Evaluate(string conditionKey)
    {
        if (string.IsNullOrEmpty(conditionKey)) return true; // sin condición = siempre true
        if (conditions.TryGetValue(conditionKey, out Func<bool> condition))
            return condition.Invoke();

        Debug.LogWarning($"Condición '{conditionKey}' no registrada");
        return false;
    }

    protected override void Start()
    {
        base.Start();
        fullConversation = characterData.CharacterDialogs.ListOfDialogs[conversationNumber-1];
        normalTexts = fullConversation.ListOfTexts;
        repetitionTexts = fullConversation.RepetitionTexts;
        //timesTalked = 0;
    }
    public override void Interact()
    {
        if (dialogIsRecentlyOpen)
        {
            dialogIsRecentlyOpen = false;
            return;
        }
        if(dialogIsOpen) return;
        dialogIsOpen= true;
        StartDialog();
        

    }
    protected override void UniqueEnter()
    {
        base.UniqueEnter();
        _playerController.ChangeDisplayText("Hablar");
    }
    protected override void UpdateDisplayText()
    {
        base.UpdateDisplayText();
        _playerController.ChangeDisplayText("Hablar");
    }
    private void StartDialog()
    {
        if(timesTalked < normalTexts.Count){
            actualConversation = normalTexts[timesTalked];
            dialogBox.StartDialog(actualConversation.lines, this); 
            timesTalked++;
            SaveState();
        }
        else
        {
            int numAl= UnityEngine.Random.Range(0, repetitionTexts.Count);
            actualConversation = repetitionTexts[numAl];
            dialogBox.StartDialog(actualConversation.lines, this);
        }

        //int cont = 0;
        //while (cont < actualConversation.lines.Count)
        //{
        //    DialogLine lineaDialogo = actualConversation.lines[cont];
        //    if(Evaluate(lineaDialogo.conditionKey))
        //    {
        //        Debug.Log(lineaDialogo.text);
        //    }
        //    else
        //    {
        //        Debug.Log("");
        //    }
        //    cont++;
        //}
    }

}
