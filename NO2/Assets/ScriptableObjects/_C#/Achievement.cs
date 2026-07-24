using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "Scriptable Objects/Achievement")]
public class Achievement : ScriptableObject
{
    public string achievementId;
    [TextArea] public string description;
    public bool isCounter;
    public int targetCount;

    private int currentCount;
    private bool isCompleted;

    public event Action OnCompleted;

    private string SaveKeyCompleted => achievementId + "_completed";
    private string SaveKeyCount => achievementId + "_count";

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
        isCompleted = false;
        currentCount = 0;
        Save();
    }

    // Para logros de un solo evento
    public void Complete()
    {
        if (isCompleted) return;
        isCompleted = true;
        Save();
        OnCompleted?.Invoke();
    }

    // Para logros de contador
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
