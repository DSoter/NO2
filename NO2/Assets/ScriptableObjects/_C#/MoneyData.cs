using UnityEngine;
using System;
using Unity.Mathematics;

[CreateAssetMenu(fileName = "MoneyData", menuName = "Scriptable Objects/MoneyData")]
public class MoneyData : ScriptableObject
{
    [SerializeField] private int money;
    [SerializeField] private int maxAmount;
    [SerializeField] private EquippedBadges equippedBadges;
    [SerializeField] private string nameBadgeHundredCoins = "Pequeña fortuna";
    [SerializeField] private string nameBadgeFiveThousandCoins = "Codicioso";
    public event Action OnMoneyChanged;

    public int Money
    {
        get { return money; }
        set
        {
            if (equippedBadges.IsEquipped(nameBadgeHundredCoins))
            {
                
                if (UnityEngine.Random.value <= 0.05f)
                {

                    Debug.Log("Tocó la loteria");
                    value = value + (value-money);
                }
            }
            GameManager.Instance.GetComponent<AchievementManager>().NotifyCounter("100_money", value-money);
            GameManager.Instance.GetComponent<AchievementManager>().NotifyCounter("5000_money", value-money);
            money = Mathf.Clamp(value, 0, maxAmount);
            OnMoneyChanged?.Invoke();
            Save();
        }
    }
    private void Awake()
    {
        equippedBadges.OnEquipped += (badgeName) => IncreaseMoney(badgeName);
        equippedBadges.OnUnequipped += (badgeName) => DecreaseMoney(badgeName);
    }
    private void OnEnable()
    {
        equippedBadges.OnEquipped += (badgeName) => IncreaseMoney(badgeName);
        equippedBadges.OnUnequipped += (badgeName) => DecreaseMoney(badgeName);
    }
    private void OnDestroy()
    {
        equippedBadges.OnEquipped -= (badgeName) => IncreaseMoney(badgeName);
        equippedBadges.OnUnequipped -= (badgeName) => DecreaseMoney(badgeName);
    }
    private void OnDisable()
    {
        equippedBadges.OnEquipped -= (badgeName) => IncreaseMoney(badgeName);
        equippedBadges.OnUnequipped -= (badgeName) => DecreaseMoney(badgeName);
    }
    private void IncreaseMoney(string badgeName) {
        if (nameBadgeHundredCoins == badgeName) {
            Debug.Log("Gana dinero");
            Money = money + 50;
        }
    }
    private void DecreaseMoney(string badgeName)
    {
        
        if (nameBadgeHundredCoins == badgeName)
        {
            Debug.Log("Pierde dinero");
            Money = money - 50;
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