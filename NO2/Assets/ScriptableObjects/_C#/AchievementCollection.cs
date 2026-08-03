using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementCollection", menuName = "Scriptable Objects/AchievementCollection")]
public class AchievementCollection : ScriptableObject
{
    [SerializeField] private List<Achievement> allAchievements;

    public List<Achievement> AllAchievements => allAchievements;
}