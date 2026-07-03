using UnityEngine;

[CreateAssetMenu(fileName = "OxygenEffect", menuName = "Scriptable Objects/OxygenEffect")]
public class OxygenEffect : PickupEffect
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private float _oxygenAmount;

    public override void Apply()
    {
        _playerData.Oxygen += _oxygenAmount;
    }
}
