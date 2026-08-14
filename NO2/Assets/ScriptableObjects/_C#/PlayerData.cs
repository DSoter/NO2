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
    [SerializeField] private float healthRegenerationSpeed;
    [SerializeField] private float healthDropingSpeed;

    [Space(5)]
    [Header("Stamina")]
    [SerializeField] private float stamina;
    [SerializeField] private float maxStamina;
    [SerializeField] private float secondsUntilStaminaRegeneration;
    [SerializeField] private float staminaRegenerationSpeed;
    [SerializeField] private float runningStaminaCost;
    [SerializeField] private float rollingStaminaCost;
    [SerializeField] private float weakAttackStaminaCost;
    [SerializeField] private float strongAttackStaminaCost;

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
    [SerializeField] private float invulnerabilitySeconds;
    [SerializeField] private float hurtSeconds;


    [Space(5)]
    [Header("Damages")]
    [SerializeField] private float weakAttackDamage;
    [SerializeField] private float minStrongAttackDamage;
    [SerializeField] private float maxStrongAttackDamage;

    [Space(5)]
    [Header("Flowers")]
    [SerializeField] private Flower equipedFlower;

    [Space(5)]
    [Header("Logros")]
    [SerializeField] private EquippedBadges equippedBadges;
    [SerializeField] private string nameBadgeHundredRolls = "Acrobata amateur";
    [SerializeField] private string nameBadgeStrongHeart = "Corazon fuerte";

    // Events

    public event Action OnMaxHealthChanged;
    public event Action OnHealthChanged;
    public event Action OnMaxStaminaChanged;
    public event Action OnStaminaChanged;
    public event Action OnMaxOxygenChanged;
    public event Action OnOxygenChanged;
    public event Action OnOxygenIncreased;



    // Read and write properties
    public float Health
    {
        get { return health; }
        set {
            if( health <= 1 && 0 < health && value > 1){
                GameManager.Instance.GetComponent<AchievementManager>().NotifyEvent("strong_heart");
            }
            health = Mathf.Clamp(value, 0, maxHealth);
            OnHealthChanged?.Invoke();
        }
    }

    public float Stamina
    {
        get { return stamina; }
        set
        {
            stamina = Mathf.Clamp(value, 0, maxStamina);
            OnStaminaChanged?.Invoke();
        }
    }

    public float Oxygen
    {
        get { return oxygen; }
        set 
        {
            var aux = oxygen;

            oxygen = Mathf.Clamp(value, 0, maxOxygen);
            OnOxygenChanged?.Invoke();

            if(value > aux)
            {
                OnOxygenIncreased?.Invoke();
            }
        }
    }

    public float MaxHealth 
    { 
        get {  return maxHealth;    }
        set
        {
            maxHealth = value;
            OnMaxHealthChanged?.Invoke();
        }
    }

    public float MaxStamina
    {
        get { return maxStamina; }
        set
        {
            maxStamina = value;
            OnMaxStaminaChanged?.Invoke();
        }
    }

    public float MaxOxygen
    {
        get { return maxOxygen; }
        set
        {
            maxOxygen = value;
            OnMaxOxygenChanged?.Invoke();
        }
    }

    // Read only properties

    public float SecondsUntilStaminaRegeneration
    {
        get { return secondsUntilStaminaRegeneration; }
    }

    public float StaminaRegenerationSpeed
    {
        get { return staminaRegenerationSpeed; }
    }
    public float HealthRegenerationSpeed
    {
        get { return healthRegenerationSpeed; }
    }
    public float HealthDropingSpeed
    {
        get { return healthDropingSpeed; }
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

    public float StrongAttackStaminaCost
    {
        get { return strongAttackStaminaCost; }
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

    public float InvulnerabilitySeconds
    {
        get { return invulnerabilitySeconds; }
    }

    public float HurtSeconds
    {
        get { return hurtSeconds; }
    }

    public float WeakAttackDamage
    {
        get { return weakAttackDamage; }
    }

    public float MinStrongAttackDamage
    {
        get { return minStrongAttackDamage; }
    }
    public float MaxStrongAttackDamage
    {
        get { return maxStrongAttackDamage; }
    }

    public Flower EquipedFlower
    {
        get { return equipedFlower; }
        set { equipedFlower = value; }
    }

    private void Awake()
    {
        equippedBadges.OnEquipped += (badgeName) => IncreaseMaxHealth(badgeName);
        equippedBadges.OnUnequipped += (badgeName) => DecreaseMaxHealth(badgeName);
        equippedBadges.OnEquipped += (badgeName) => IncreaseMaxStamina(badgeName);
        equippedBadges.OnUnequipped += (badgeName) => DecreaseMaxStamina(badgeName);
    }
    private void OnEnable()
    {
        if(equippedBadges != null) {
            equippedBadges.OnEquipped += (badgeName) => IncreaseMaxHealth(badgeName);
            equippedBadges.OnUnequipped += (badgeName) => DecreaseMaxHealth(badgeName);
            equippedBadges.OnEquipped += (badgeName) => IncreaseMaxStamina(badgeName);
            equippedBadges.OnUnequipped += (badgeName) => DecreaseMaxStamina(badgeName);
        }
    }
    private void OnDestroy()
    {
        equippedBadges.OnEquipped -= (badgeName) => IncreaseMaxHealth(badgeName);
        equippedBadges.OnUnequipped -= (badgeName) => DecreaseMaxHealth(badgeName);
        equippedBadges.OnEquipped -= (badgeName) => IncreaseMaxStamina(badgeName);
        equippedBadges.OnUnequipped -= (badgeName) => DecreaseMaxStamina(badgeName);
    }
    private void OnDisable()
    {
        equippedBadges.OnEquipped -= (badgeName) => IncreaseMaxHealth(badgeName);
        equippedBadges.OnUnequipped -= (badgeName) => DecreaseMaxHealth(badgeName);
        equippedBadges.OnEquipped -= (badgeName) => IncreaseMaxStamina(badgeName);
        equippedBadges.OnUnequipped -= (badgeName) => DecreaseMaxStamina(badgeName);
    }

    private void IncreaseMaxHealth(string badgeName)
    {
        if (nameBadgeStrongHeart == badgeName)
        {
            MaxHealth = maxHealth + 10;
            Health = health + 10;
        }
    }
    private void DecreaseMaxHealth(string badgeName)
    {

        if (nameBadgeStrongHeart == badgeName)
        {
            MaxHealth = maxHealth - 10;
            if(health > maxHealth)
            {
                health=maxHealth;
            }
        }
    }

    private void IncreaseMaxStamina(string badgeName)
    {
        if (nameBadgeHundredRolls == badgeName)
        {
            MaxStamina = maxStamina + 10;
            Stamina = stamina + 10;
        }
    }
    private void DecreaseMaxStamina(string badgeName)
    {

        if (nameBadgeHundredRolls == badgeName)
        {
            MaxStamina = maxStamina - 10;
            if (stamina > maxStamina)
            {
                stamina = maxStamina;
            }
        }
    }


}
