using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColectionOfDialogs", menuName = "Scriptable Objects/ColectionOfDialogs")]
public class ColectionOfDialogs : ScriptableObject
{
    [SerializeField] List<DialogTextData> listOfDialogs;
    public List<DialogTextData> ListOfDialogs { get; private set; }
}
