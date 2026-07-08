
using UnityEngine;
public class CharacterInteractable : Interactable
{
    [SerializeField] FriendlyCharacterData characterData;
    [SerializeField] int conversationNumber; //se usa para acceder a la lista de dialogos pero empieza en uno
    private DialogTextData fullConversation;
    private int timesTalked;

    protected override void Start()
    {
        base.Start();
        fullConversation = characterData.CharacterDialogs.ListOfDialogs[conversationNumber - 1];

    }
    public override void Interact()
    {
        StartDialog();

    }
    protected override void UniqueEnter()
    {
        base.UniqueEnter();
        _playerController.ChangeDisplayText("Hablar");
    }
    private void StartDialog()
    {
        return;
    }

}
