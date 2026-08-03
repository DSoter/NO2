using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FogCollection", menuName = "Scriptable Objects/FogCollection")]
public class FogCollection : ScriptableObject
{
    [SerializeField] private List<FogData> allFogData;

    public List<FogData> AllFogData => allFogData;
}