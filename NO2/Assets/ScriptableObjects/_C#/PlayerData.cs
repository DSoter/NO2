using NUnit.Framework;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{

    [Header("Health")]
    [SerializeField] private float health;
    [SerializeField] private float maxHealth;

    [Space(5)]
    [Header("Stamina")]
    [SerializeField] private float stamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private float secondsUntilStaminaRegeneration;
    [SerializeField] private float staminaRegenerationSpeed;
    [SerializeField] private float runningStaminaCost;
    [SerializeField] private float rollingStaminaCost;
    [SerializeField] private float weakAttackStaminaCost;

    [Space(5)]
    [Header("Oxygen")]
    [SerializeField] private float oxygen;
    [SerializeField] private float maxOxygen;
    [SerializeField] private float oxygenDropingSpeed;
    [SerializeField] private float oxygenRegenerationSpeed;
    [SerializeField] private float lastOxygenSeconds;

    [Space(5)]
    [Header("Speeds")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float iniRollingSpeed;
    [SerializeField] private float endRollingSpeed;

    [Space(5)]
    [Header("Timings")]
    [SerializeField] private float iniRollingSeconds;
    [SerializeField] private float endRollingSeconds;
    [SerializeField] private float weakAttackSeconds;


    [Space(5)]
    [Header("Damages")]
    [SerializeField] private float basicAttackDamage;
    [SerializeField] private float chargeAttackDamage;

    [Space(5)]
    [Header("Flowers")]
    [SerializeField] private Flower equipedFlower;

    // Events

    public event Action OnHealthChanged;
    public event Action OnStaminaChanged;
    public event Action OnOxygenChanged;

    // Read and write properties
    public float Health
    {
        get { return health; }
        set {
            health = value;
            OnHealthChanged?.Invoke();
        }
    }

    public float Stamina
    {
        get { return stamina; }
        set
        {
            stamina = value; 
            OnStaminaChanged?.Invoke();
        }
    }

    public float Oxygen
    {
        get { return oxygen; }
        set 
        {
            oxygen = Mathf.Clamp(value, 0, maxOxygen);
            OnOxygenChanged?.Invoke();
        }
    }


    // Read only properties

    public float MaxHealth 
    { 
        get {  return maxHealth;    }
    }

    public float MaxStamina
    {
        get { return maxStamina; }
    }

    public float MaxOxygen
    {
        get { return maxOxygen; }
    }

    public float SecondsUntilStaminaRegeneration
    {
        get { return secondsUntilStaminaRegeneration; }
    }

    public float StaminaRegenerationSpeed
    {
        get { return staminaRegenerationSpeed; }
    }
    public float RunningStaminaCost
    {
        get { return runningStaminaCost; }
    }
    public float RollingStaminaCost
    {
        get { return rollingStaminaCost; }
    }
    public float WeakAttackStaminaCost
    {
        get { return weakAttackStaminaCost; }
    }

    public float OxygenDropingSpeed
    {
        get { return oxygenDropingSpeed; }
    }

    public float OxygenRegenerationSpeed
    {
        get { return oxygenRegenerationSpeed; }
    }
    public float LastOxygenSeconds
    {
        get { return lastOxygenSeconds; }
        set { lastOxygenSeconds = value; }
    }

    public float WalkingSpeed
    {
        get { return walkingSpeed; }
    }

    public float RunningSpeed
    {
        get { return runningSpeed; }
    }

    public float IniRollingSpeed
    {
        get { return iniRollingSpeed; }
    }

    public float EndRollingSpeed
    {
        get { return endRollingSpeed; }
    }
    public float IniRollingSeconds
    {
        get { return iniRollingSeconds; }
    }

    public float EndRollingSeconds
    {
        get { return endRollingSeconds; }
    }

    public float WeakAttackSeconds
    {
        get { return weakAttackSeconds; }
    }

    public float BasicAttackDamage
    {
        get { return basicAttackDamage; }
    }

    public float ChargeAttackDamage
    {
        get { return chargeAttackDamage; }
    }
    public Flower EquipedFlower
    {
        get { return equipedFlower; }
        set { equipedFlower = value; }
    }

}
