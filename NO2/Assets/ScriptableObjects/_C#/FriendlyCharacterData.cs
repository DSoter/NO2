using UnityEngine;

[CreateAssetMenu(fileName = "FriendlyCharacterData", menuName = "Scriptable Objects/FriendlyCharacterData")]
public class FriendlyCharacterData : ScriptableObject
{
    [SerializeField] private Sprite characterPortrait;
    [SerializeField] private string characterName;
    [SerializeField] private  ColectionOfDialogs characterDialogs;

    public ColectionOfDialogs CharacterDialogs { 
        get { return characterDialogs; }
        set { characterDialogs = value; }
    }
    public Sprite CharacterPortrait
    {
        get { return characterPortrait; }
        set { characterPortrait = value; }
    }
    public string CharacterName
    {
        get { return characterName; }
        set { characterName = value; }
    }
}
