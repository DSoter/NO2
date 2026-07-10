using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CharacterInteractable : Interactable
{
    [SerializeField] FriendlyCharacterData characterData;
    [SerializeField] int conversationNumber = 1; //se usa para acceder a la lista de dialogos pero empieza en uno
    private AllConditionsDIalog acd;
    private DialogTextData fullConversation;
    private List<DialogSequence> normalTexts;
    private DialogSequence actualConversation;
    private List<DialogSequence> repetitionTexts;
    private int timesTalked;
    private DialogBox dialogBox;
    private bool dialogIsOpen;

    public bool DialogIsOpen
    {
        get{ return dialogIsOpen; }
        set{ dialogIsOpen = value; }

    }

    private Dictionary<string, Func<bool>> conditions;
        
    private void Awake()
    {
        dialogBox = FindAnyObjectByType<DialogBox>();
        acd = GetComponent<AllConditionsDIalog>();
        conditions = new Dictionary<string, Func<bool>>
        {
            { "anyFlowerUnlocked",   () => acd.FlowerIsUnlocked()},
            { "anyFlowerUnlockedThenDestroy",   () => acd.FlowerIsUnlockedThenDestroy()},
            {"anyFlowerUnlockedAndNotTheoFirstDialogFinished", () => acd.FlowerIsUnlocked() && !acd.FirstTheoDialogIsFinished() },
            {"anyFlowerUnlockedThenTheoFirstDialogFinished", () =>  !acd.FirstTheoDialogIsFinished() & acd.FlowerIsUnlockedThenFinishTheoFirstDialog()  },
            {"anyFlowerUnlockedAndTheoFirstDialogFinished", () => acd.FlowerIsUnlocked() && acd.FirstTheoDialogIsFinished() },
            {"anyFlowerUnlockedThenTheoSecondDialogFinished", () => acd.FlowerIsUnlockedThenFinishTheoSecondDialog() },
            {"anyFlowerUnlockedAndTheoSecondDialogFinished", () => acd.FlowerIsUnlocked() && acd.SecondTheoDialogIsFinished() },
            { "noneFlowerUnlocked", () => !acd.FlowerIsUnlocked()},
            // añadir TODAS las condiciones
        };
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
        timesTalked = 0;
    }
    public override void Interact()
    {
        if(dialogIsOpen) return;
        dialogIsOpen= true;
        StartDialog();
        

    }
    protected override void UniqueEnter()
    {
        base.UniqueEnter();
        _playerController.ChangeDisplayText("Hablar");
    }
    private void StartDialog()
    {
        if(timesTalked < normalTexts.Count){
            actualConversation = normalTexts[timesTalked];
            dialogBox.StartDialog(actualConversation.lines, this); 
            timesTalked++;
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
