using UnityEngine;

[CreateAssetMenu(fileName = "CoinEffect", menuName = "Scriptable Objects/CoinEffect")]
public class CoinEffect : PickupEffect
{
    [SerializeField] private MoneyData _moneyData;
    [SerializeField] private int _moneyAmount;

    public override void Apply()
    {
        _moneyData.Money += _moneyAmount;
    }
}