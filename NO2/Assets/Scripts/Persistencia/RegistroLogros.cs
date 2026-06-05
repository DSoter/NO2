using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RegistroLogros
{
    public static RegistroLogros achievementProgress;
    public List<ProgresoLogro> achievementProgressList;
    public enum AchievementName { NO2, Certero };

}
