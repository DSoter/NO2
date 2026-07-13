using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HealData", menuName = "Scriptable Objects/HealData")]
public class HealData : ScriptableObject
{
    [SerializeField] private int maxUses;
    [SerializeField] private int remainingUses;

    [SerializeField] private int healAmount;

    [SerializeField] private float actualCooldownSeconds;
    [SerializeFIeld] private float cooldownSeconds;

    public Action healUsesChanged;

    public int RemainingUses
    {
        get { return remainingUses; }
        set
        { 
            remainingUses = value;
            healUsesChanged.Invoke();
        }
    }
    public int MaxUses
    {
        get { return maxUses; }
        set {  maxUses = value;}
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
        set { ActualCooldownSeconds = value; }
    }

}
