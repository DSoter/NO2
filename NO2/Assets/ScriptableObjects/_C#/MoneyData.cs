using UnityEngine;
using System;

[CreateAssetMenu(fileName = "MoneyData", menuName = "Scriptable Objects/MoneyData")]
public class MoneyData : ScriptableObject
{
    [SerializeField] private int money;
    [SerializeField] private int maxAmount;
    public event Action OnMoneyChanged;

    public int Money
    {
        get { return money; }
        set
        {
            GameManager.Instance.GetComponent<AchievementManager>().NotifyCounter("100_money", value-money);
            GameManager.Instance.GetComponent<AchievementManager>().NotifyCounter("5000_money", value-money);
            money = Mathf.Clamp(value, 0, maxAmount);
            OnMoneyChanged?.Invoke();
            
        }
    }

    private string SaveKey => "actual_money";

    public void Save()
    {
        PlayerPrefs.SetInt(SaveKey, money);
        PlayerPrefs.Save();
    }

    public bool Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return false;

        money = Mathf.Clamp(PlayerPrefs.GetInt(SaveKey, 0), 0, maxAmount);
        OnMoneyChanged?.Invoke();
        return true;
    }

    public void Reset()
    {
        money = 0;
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        OnMoneyChanged?.Invoke();
    }
}