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
            money = Mathf.Clamp(value, 0, maxAmount);
            OnMoneyChanged?.Invoke();
            
        }
    }

}
