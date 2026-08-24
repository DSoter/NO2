using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HealData", menuName = "Scriptable Objects/HealData")]
public class HealData : ScriptableObject
{
    [SerializeField] private int maxUses;
    [SerializeField] private int remainingUses;

    [SerializeField] private int healAmount;

    [SerializeField] private float actualCooldownSeconds;
    [SerializeField] private float cooldownSeconds;

    [SerializeField] private EquippedBadges equippedBadges;
    private string fastPotionsName = "Curacion rapida";

    public Action healUsesChanged;

    public Action actualCooldownChanged;
    public int RemainingUses
    {
        get { return remainingUses; }
        set
        { 
            remainingUses = value;
            healUsesChanged?.Invoke();
        }
    }
    public int MaxUses
    {
        get { return maxUses; }
        set
        { 
            maxUses = value;
            healUsesChanged?.Invoke();
        }
    }
    public int HealAmount
    {
        get { return healAmount; }
        set {  healAmount = value;}
    }
    public float CooldownSeconds
    {
        get { return cooldownSeconds; }
        set { cooldownSeconds = value; }
    }
    public float ActualCooldownSeconds
    {
        get { return actualCooldownSeconds; }
        set {
                actualCooldownSeconds = value; 
                actualCooldownChanged?.Invoke();
        }
    }

    private void OnEnable()
    {
        if (equippedBadges != null)
        {
            equippedBadges.OnEquipped += (badgeName) => IncreaseMaxPotions(badgeName);
            equippedBadges.OnUnequipped += (badgeName) => DecreaseMaxPotions(badgeName);
        }
    }
    private void OnDestroy()
    {
        equippedBadges.OnEquipped -= (badgeName) => IncreaseMaxPotions(badgeName);
        equippedBadges.OnUnequipped -= (badgeName) => DecreaseMaxPotions(badgeName);
    }
    private void OnDisable()
    {
        equippedBadges.OnEquipped -= (badgeName) => IncreaseMaxPotions(badgeName);
        equippedBadges.OnUnequipped -= (badgeName) => DecreaseMaxPotions(badgeName);
    }

    private void IncreaseMaxPotions(string badgeName)
    {
        if (fastPotionsName== badgeName)
        {
            MaxUses = maxUses + 2;
            RemainingUses = remainingUses + 2;

        }
    }
    private void DecreaseMaxPotions(string badgeName)
    {
        if (fastPotionsName == badgeName)
        {
            MaxUses = maxUses - 2;
            RemainingUses = maxUses;
        }
    }

}
