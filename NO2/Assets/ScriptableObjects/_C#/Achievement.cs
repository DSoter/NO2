using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable Objects/Achievement")]
public class Achievement : ScriptableObject
{
    public string achievementName;
    [TextArea] public string description;
    public bool isCounter;
    public int targetCount;

    private int currentCount;
    private bool isCompleted;

    public event Action OnCompleted;
    public event Action OnReset;

    private string SaveKeyCompleted => achievementName + "_completed";
    private string SaveKeyCount => achievementName + "_count";

    public bool IsCompleted => isCompleted;
    public int CurrentCount => currentCount;

    public void Load()
    {
        isCompleted = PlayerPrefs.GetInt(SaveKeyCompleted, 0) == 1;
        currentCount = PlayerPrefs.GetInt(SaveKeyCount, 0);
    }

    public void Save()
    {
        PlayerPrefs.SetInt(SaveKeyCompleted, isCompleted ? 1 : 0);
        PlayerPrefs.SetInt(SaveKeyCount, currentCount);
        PlayerPrefs.Save();
    }

    public void Reset()
    {
        OnReset?.Invoke();
        isCompleted = false;
        currentCount = 0;
        Save();
    }

    public void Complete()
    {
        if (isCompleted) return;
        isCompleted = true;
        Save();
        OnCompleted?.Invoke();
    }

    public void Increment(int amount = 1)
    {
        if (isCompleted) return;
        currentCount += amount;
        if (currentCount >= targetCount)
        {
            currentCount = targetCount;
            Complete();
        }
        else
        {
            Save();
        }
    }
}
