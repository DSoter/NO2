using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockedFlowers", menuName = "Scriptable Objects/UnlockedFlowers")]
public class UnlockedFlowers : ScriptableObject
{
    public List<Flower> unlockedFlowers;
}
