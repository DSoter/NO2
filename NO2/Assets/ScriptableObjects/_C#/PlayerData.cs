using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private string nameBadgeBigCharge = "Peso pesado";

    private readonly Modifier strongHeartModifier = new Modifier(Modifier.ModifierType.Numeric, 10f);
    private readonly Modifier hundredRollsModifier = new Modifier(Modifier.ModifierType.Numeric, 10f);
    private readonly Modifier bigChargeMinModifier = new Modifier(Modifier.ModifierType.Numeric, 2f);
    private readonly Modifier bigChargeMaxModifier = new Modifier(Modifier.ModifierType.Numeric, 2f);
    private List<Modifier> maxHealthModifiers = new List<Modifier>();
    private List<Modifier> maxStaminaModifiers = new List<Modifier>();
    private List<Modifier> minStrongAttackDamageModifiers = new List<Modifier>();
    private List<Modifier> maxStrongAttackDamageModifiers = new List<Modifier>();

    //references 
    private Action<string> onEquippedIncreaseHealthHandler;
    private Action<string> onUnequippedDecreaseHealthHandler;
    private Action<string> onEquippedIncreaseStaminaHandler;
    private Action<string> onUnequippedDecreaseStaminaHandler;
    private Action<string> onEquippedIncreaseStrongDamageHandler;
    private Action<string> onUnequippedDecreaseStrongDamageHandler;
    private Action onBadgesResetHandler;

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
        get { return Modifier.ApplyModifiers(maxHealth, maxHealthModifiers); }
        set
        {
            maxHealth = value;
            OnMaxHealthChanged?.Invoke();
        }
    }

    public float MaxStamina
    {
        get { return Modifier.ApplyModifiers(maxStamina, maxStaminaModifiers); }
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
        get {return Modifier.ApplyModifiers(minStrongAttackDamage, minStrongAttackDamageModifiers); }
    }
    public float MaxStrongAttackDamage
    {
        get { return Modifier.ApplyModifiers(maxStrongAttackDamage, maxStrongAttackDamageModifiers); }
    }

    public Flower EquipedFlower
    {
        get { return equipedFlower; }
        set { equipedFlower = value; }
    }

    private void OnEnable()
    {
        SubscribeToBadges();

        //if (!Load())
        //{
        //    Reset();
        //}
    }
    private void OnDestroy()
    {
        UnsubscribeFromBadges();
    }
    private void OnDisable()
    {
        UnsubscribeFromBadges();
    }

    private void SubscribeToBadges()
    {
        if (equippedBadges == null) return;

        onEquippedIncreaseHealthHandler = (badgeName) => IncreaseMaxHealth(badgeName);
        onUnequippedDecreaseHealthHandler = (badgeName) => DecreaseMaxHealth(badgeName);
        onEquippedIncreaseStaminaHandler = (badgeName) => IncreaseMaxStamina(badgeName);
        onUnequippedDecreaseStaminaHandler = (badgeName) => DecreaseMaxStamina(badgeName);
        onEquippedIncreaseStrongDamageHandler = (badgeName) => IncreaseStrongDamage(badgeName);
        onUnequippedDecreaseStrongDamageHandler = (badgeName) => DecreaseStrongDamage(badgeName);
        onBadgesResetHandler = HandleBadgesReset;

        equippedBadges.OnEquipped += onEquippedIncreaseHealthHandler;
        equippedBadges.OnUnequipped += onUnequippedDecreaseHealthHandler;
        equippedBadges.OnEquipped += onEquippedIncreaseStaminaHandler;
        equippedBadges.OnUnequipped += onUnequippedDecreaseStaminaHandler;
        equippedBadges.OnEquipped += onEquippedIncreaseStrongDamageHandler;
        equippedBadges.OnUnequipped += onUnequippedDecreaseStrongDamageHandler;
        equippedBadges.OnReset += onBadgesResetHandler;
    }

    private void UnsubscribeFromBadges()
    {
        if (equippedBadges == null) return;

        if (onEquippedIncreaseHealthHandler != null)
            equippedBadges.OnEquipped -= onEquippedIncreaseHealthHandler;
        if (onUnequippedDecreaseHealthHandler != null)
            equippedBadges.OnUnequipped -= onUnequippedDecreaseHealthHandler;
        if (onEquippedIncreaseStaminaHandler != null)
            equippedBadges.OnEquipped -= onEquippedIncreaseStaminaHandler;
        if (onUnequippedDecreaseStaminaHandler != null)
            equippedBadges.OnUnequipped -= onUnequippedDecreaseStaminaHandler;
        if (onEquippedIncreaseStaminaHandler != null)
            equippedBadges.OnEquipped -= onEquippedIncreaseStrongDamageHandler;
        if (onUnequippedDecreaseStaminaHandler != null) 
            equippedBadges.OnUnequipped -= onUnequippedDecreaseStrongDamageHandler;
        if (onBadgesResetHandler != null)
            equippedBadges.OnReset -= onBadgesResetHandler;
    }

    private void IncreaseMaxHealth(string badgeName)
    {
        if (nameBadgeStrongHeart == badgeName)
        {
            if (!maxHealthModifiers.Contains(strongHeartModifier))
                maxHealthModifiers.Add(strongHeartModifier);

            OnMaxHealthChanged?.Invoke();
        }
    }
    private void DecreaseMaxHealth(string badgeName)
    {

        if (nameBadgeStrongHeart == badgeName)
        {
            maxHealthModifiers.Remove(strongHeartModifier);
            if (health > MaxHealth)
            {
                health = MaxHealth;
            }
            OnMaxHealthChanged?.Invoke();
        }
    }

    private void IncreaseMaxStamina(string badgeName)
    {
        if (nameBadgeHundredRolls == badgeName)
        {
            if (!maxStaminaModifiers.Contains(hundredRollsModifier))
                maxStaminaModifiers.Add(hundredRollsModifier);

            Stamina = stamina + 10;
            OnMaxStaminaChanged?.Invoke();
        }
    }
    private void DecreaseMaxStamina(string badgeName)
    {

        if (nameBadgeHundredRolls == badgeName)
        {
            maxStaminaModifiers.Remove(hundredRollsModifier);

            if (stamina > MaxStamina)
            {
                stamina = MaxStamina;
            }
            OnMaxStaminaChanged?.Invoke();
        }
    }
    private void IncreaseStrongDamage(string badgeName)
    {
        if (nameBadgeBigCharge == badgeName)
        {
            if (!minStrongAttackDamageModifiers.Contains(bigChargeMinModifier))
                minStrongAttackDamageModifiers.Add(bigChargeMinModifier);
            if (!maxStrongAttackDamageModifiers.Contains(bigChargeMaxModifier))
                maxStrongAttackDamageModifiers.Add(bigChargeMaxModifier);
        }
    }
    private void DecreaseStrongDamage(string badgeName)
    {

        if (nameBadgeBigCharge == badgeName)
        {
            minStrongAttackDamageModifiers.Remove(bigChargeMinModifier);
            maxStrongAttackDamageModifiers.Remove(bigChargeMaxModifier);
        }
    }
    private void HandleBadgesReset()
    {
        maxHealthModifiers.Clear();
        maxStaminaModifiers.Clear();

        if (health > MaxHealth) health = MaxHealth;
        if (stamina > MaxStamina) stamina = MaxStamina;

        OnMaxHealthChanged?.Invoke();
        OnMaxStaminaChanged?.Invoke();
    }


}
