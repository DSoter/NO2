using UnityEngine;

[CreateAssetMenu(fileName = "FriendlyCharacterData", menuName = "Scriptable Objects/FriendlyCharacterData")]
public class FriendlyCharacterData : ScriptableObject
{
    [SerializeField] private Sprite characterPortrait;
    [SerializeField] private  ColectionOfDialogs characterDialogs;

    public ColectionOfDialogs CharacterDialogs {  get; set; }
}
