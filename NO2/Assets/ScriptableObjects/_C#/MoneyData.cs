using UnityEngine;
using System;
using Unity.Mathematics;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MoneyData", menuName = "Scriptable Objects/MoneyData")]
public class MoneyData : ScriptableObject
{
    [SerializeField] private int money;
    [SerializeField] private int maxAmount;
    [SerializeField] private EquippedBadges equippedBadges;
    [SerializeField] private string nameBadgeHundredCoins = "Pequeña fortuna";
    [SerializeField] private string nameBadgeFiveThousandCoins = "Codicioso";
    public event Action OnMoneyChanged;


    private readonly Modifier fiveThousandCoinsModifier = new Modifier(Modifier.ModifierType.Percentage, 51f);

    public int Money
    {
        get { return money; }
        set
        {
            int gain = value - money;

            if (gain > 0 && equippedBadges != null && equippedBadges.IsEquipped(nameBadgeFiveThousandCoins))
            {
                gain = Mathf.RoundToInt(fiveThousandCoinsModifier.ApplyModifier(gain));
                value = money + gain;
            }

            if (equippedBadges != null && equippedBadges.IsEquipped(nameBadgeHundredCoins))
            {
                if (value - money > 0)
                {
                    if (UnityEngine.Random.value <= 0.05f)
                    {

                        Debug.Log("Tocó la loteria");
                        value = value + (value - money);
                    }
                }
            }

            money = Mathf.Clamp(value, 0, maxAmount);
            if (money >= 100)
            {
                GameManager.Instance.GetComponent<AchievementManager>().NotifyEvent("100_money");
                if (money >= 1000)
                {
                    GameManager.Instance.GetComponent<AchievementManager>().NotifyEvent("5000_money");

                }
            }
            OnMoneyChanged?.Invoke();
            Save();
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