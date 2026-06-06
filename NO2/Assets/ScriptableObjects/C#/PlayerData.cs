using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{

    [Header("Health")]
    [SerializeField] private float maxHealth;

    [Space(5)]
    [Header("Stamina")]
    [SerializeField] private float maxStamina;
    [SerializeField] private float secondsUntilStaminaRegeneration;
    [SerializeField] private float staminaRegenerationSpeed;

    [Space(5)]
    [Header("Oxygen")]
    [SerializeField] private float maxOxygen;
    [SerializeField] private float oxigenDropingSpeed;

    [Space(5)]
    [Header("Speeds")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;
    [SerializeField] private float iniRollingSpeed;
    [SerializeField] private float endRollingSpeed;

    [Space(5)]
    [Header("Damages")]
    [SerializeField] private float basicAttackDamage;
    [SerializeField] private float chargeAttackDamage;



    // Access Properties
    public float MaxHealth 
    { 
        get {  return maxHealth; }
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

    public float OxigenDropingSpeed
    {
        get { return oxigenDropingSpeed; }
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

    public float BasicAttackDamage
    {
        get { return basicAttackDamage; }
    }

    public float ChargeAttackDamage
    {
        get { return chargeAttackDamage; }
    }



}
