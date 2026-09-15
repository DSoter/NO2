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
    [Header("Defensa")]
    [SerializeField] private float baseDefense = 1f;

    [Space(5)]
    [Header("Flowers")]
    [SerializeField] private Flower equipedFlower;

    [Space(5)]
    [Header("Logros")]
    [SerializeField] private EquippedBadges equippedBadges;

    // Umbral compartido para las condiciones "un cuarto o menos de X máximo"
    private const float LowResourceThreshold = 0.25f;

    // Modificadores activos por insignias (no se persisten: se reconstruyen a partir de EquippedBadges)
    private readonly Modifier fullHealthDefenseModifier = new Modifier(Modifier.ModifierType.Percentage, 70f);

    // Velocidad condicional: tipo Multiplier, se compone en cadena en vez de sumarse
    private readonly Modifier snailSlayerSpeedModifier = new Modifier(Modifier.ModifierType.Multiplier, 20f);
    private readonly Modifier primeraInsigniaSpeedModifier = new Modifier(Modifier.ModifierType.Multiplier, 20f);

    // "Scout principiante": +2 de daño plano en los tres ataques, solo con oxígeno bajo
    private readonly Modifier scoutPrincipianteDamageModifier = new Modifier(Modifier.ModifierType.Numeric, 2f);


    // "Box slayer": +10% del oxígeno ganado en cada recogida
    private readonly Modifier boxSlayerOxygenGainModifier = new Modifier(Modifier.ModifierType.Percentage, 10f);

    private List<Modifier> maxHealthModifiers = new List<Modifier>();
    private List<Modifier> maxStaminaModifiers = new List<Modifier>();
    private List<Modifier> defenseModifiers = new List<Modifier>();
    private List<Modifier> maxOxygenModifiers = new List<Modifier>();
    private List<Modifier> staminaRegenerationSpeedModifiers = new List<Modifier>();
    private List<Modifier> minStrongAttackDamageModifiers = new List<Modifier>();
    private List<Modifier> maxStrongAttackDamageModifiers = new List<Modifier>();
    private bool fullHealthDefenseBadgeEquipped;
    private bool snailSlayerBadgeEquipped;
    private bool scoutPrincipianteBadgeEquipped;
    private bool boxSlayerBadgeEquipped;
    private bool primeraInsigniaBadgeEquipped;


    // Events

    public event Action OnMaxHealthChanged;
    public event Action OnHealthChanged;
    public event Action OnMaxStaminaChanged;
    public event Action OnStaminaChanged;
    public event Action OnMaxOxygenChanged;
    public event Action OnOxygenChanged;
    public event Action OnOxygenIncreased;


    public enum PlayerBadgeStat { MaxHealth, MaxStamina, Defense, MaxOxygen, StaminaRegenSpeed }
    public enum PlayerBadgeFlag { FullHealthDefense, SnailSlayerSpeed, ScoutPrincipianteDamage, BoxSlayerOxygenGain, PrimeraInsigniaSpeed }


    // Read and write properties
    public float Health
    {
        get { return health; }
        set
        {
            if (health <= 10 && 0 < health && value > 10)
            {
                GameManager.Instance.GetComponent<AchievementManager>().NotifyEvent("strong_heart");
            }
            health = Mathf.Clamp(value, 0, MaxHealth);
            OnHealthChanged?.Invoke();
        }
    }

    public float Stamina
    {
        get { return stamina; }
        set
        {
            stamina = Mathf.Clamp(value, 0, MaxStamina);
            OnStaminaChanged?.Invoke();
        }
    }

    public float Oxygen
    {
        get { return oxygen; }
        set
        {
            var aux = oxygen;
            float incomingValue = value;

            if (boxSlayerBadgeEquipped && incomingValue > aux)
            {
                float gain = incomingValue - aux;
                gain = boxSlayerOxygenGainModifier.ApplyModifier(gain);
                incomingValue = aux + gain;
            }

            oxygen = Mathf.Clamp(incomingValue, 0, MaxOxygen);
            OnOxygenChanged?.Invoke();

            if (incomingValue > aux)
            {
                OnOxygenIncreased?.Invoke();
            }
        }
    }

    // El getter aplica los modificadores activos (insignias) sobre el valor base serializado
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
        get { return Modifier.ApplyModifiers(maxOxygen, maxOxygenModifiers); }
        set
        {
            maxOxygen = value;
            OnMaxOxygenChanged?.Invoke();
        }
    }

    // Defensa: arranca en baseDefense (1) y sube por porcentaje con las insignias.
    // "Vida plena" solo se suma mientras la vida esté al máximo, por eso se
    // comprueba en cada get en vez de guardarse siempre en defenseModifiers.
    public float Defense
    {
        get
        {
            if (fullHealthDefenseBadgeEquipped && health >= MaxHealth)
            {
                List<Modifier> withFullHealthBonus = new List<Modifier>(defenseModifiers) { fullHealthDefenseModifier };
                return Modifier.ApplyModifiers(baseDefense, withFullHealthBonus);
            }

            return Modifier.ApplyModifiers(baseDefense, defenseModifiers);
        }
    }

    // Aplica la defensa actual a un daño bruto y devuelve el daño final a restar de Health
    public float CalculateReceivedDamage(float rawDamage)
    {
        float defense = Defense;
        if (defense <= 0f) return rawDamage;
        return rawDamage / defense;
    }

    // Read only properties

    public float SecondsUntilStaminaRegeneration
    {
        get { return secondsUntilStaminaRegeneration; }
    }

    // "Acróbata experto": bonus permanente mientras esté equipada
    public float StaminaRegenerationSpeed
    {
        get { return Modifier.ApplyModifiers(staminaRegenerationSpeed, staminaRegenerationSpeedModifiers); }
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

    // "Snail slayer" (vida <= 25%) y "Primera insignia" (oxígeno <= 25%) suman
    // su bonus de velocidad aquí, evaluado en el get.
    public float WalkingSpeed
    {
        get { return Modifier.ApplyModifiers(walkingSpeed, GetActiveSpeedModifiers()); }
    }

    public float RunningSpeed
    {
        get { return Modifier.ApplyModifiers(runningSpeed, GetActiveSpeedModifiers()); }
    }

    private List<Modifier> GetActiveSpeedModifiers()
    {
        List<Modifier> active = new List<Modifier>();

        if (snailSlayerBadgeEquipped && health <= MaxHealth * LowResourceThreshold)
            active.Add(snailSlayerSpeedModifier);

        if (primeraInsigniaBadgeEquipped && oxygen <= MaxOxygen * LowResourceThreshold)
            active.Add(primeraInsigniaSpeedModifier);

        return active;
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

    // "Scout principiante" (oxígeno <= 25%): +2 de daño plano en los tres ataques
    public float WeakAttackDamage
    {
        get { return Modifier.ApplyModifiers(weakAttackDamage, GetActiveScoutDamageModifiers()); }
    }

    // Combina el bonus permanente de "Peso pesado" con el bonus condicional de "Scout principiante"
    public float MinStrongAttackDamage
    {
        get
        {
            List<Modifier> combined = new List<Modifier>(minStrongAttackDamageModifiers);
            combined.AddRange(GetActiveScoutDamageModifiers());
            return Modifier.ApplyModifiers(minStrongAttackDamage, combined);
        }
    }
    public float MaxStrongAttackDamage
    {
        get
        {
            List<Modifier> combined = new List<Modifier>(maxStrongAttackDamageModifiers);
            combined.AddRange(GetActiveScoutDamageModifiers());
            return Modifier.ApplyModifiers(maxStrongAttackDamage, combined);
        }
    }

    private List<Modifier> GetActiveScoutDamageModifiers()
    {
        List<Modifier> active = new List<Modifier>();

        if (scoutPrincipianteBadgeEquipped && oxygen <= MaxOxygen * LowResourceThreshold)
            active.Add(scoutPrincipianteDamageModifier);

        return active;
    }

    public Flower EquipedFlower
    {
        get { return equipedFlower; }
        set { equipedFlower = value; }
    }

    // Enums públicos para que BadgeManager pida el efecto sin conocer los campos internos
    
  
    public void AddBadgeModifier(PlayerBadgeStat stat, Modifier modifier)
    {
        List<Modifier> list = GetModifierList(stat);
        if (list == null || list.Contains(modifier)) return;
        list.Add(modifier);

        if (stat == PlayerBadgeStat.MaxHealth)
        {
            Health = health + 10; 
            OnMaxHealthChanged?.Invoke();
        }
        else if (stat == PlayerBadgeStat.MaxStamina)
        {
            Stamina = stamina + 10;
            OnMaxStaminaChanged?.Invoke();
        }
        else if (stat == PlayerBadgeStat.MaxOxygen)
        {
            Oxygen = oxygen; 
            OnMaxOxygenChanged?.Invoke();
        }
    }

    public void RemoveBadgeModifier(PlayerBadgeStat stat, Modifier modifier)
    {
        List<Modifier> list = GetModifierList(stat);
        list?.Remove(modifier);

        if (stat == PlayerBadgeStat.MaxHealth)
        {
            if (health > MaxHealth) health = MaxHealth;
            OnMaxHealthChanged?.Invoke();
        }
        else if (stat == PlayerBadgeStat.MaxStamina)
        {
            if (stamina > MaxStamina) stamina = MaxStamina;
            OnMaxStaminaChanged?.Invoke();
        }
        else if (stat == PlayerBadgeStat.MaxOxygen)
        {
            if (oxygen > MaxOxygen) oxygen = MaxOxygen;
            OnMaxOxygenChanged?.Invoke();
        }
    }

    private List<Modifier> GetModifierList(PlayerBadgeStat stat)
    {
        switch (stat)
        {
            case PlayerBadgeStat.MaxHealth: return maxHealthModifiers;
            case PlayerBadgeStat.MaxStamina: return maxStaminaModifiers;
            case PlayerBadgeStat.Defense: return defenseModifiers;
            case PlayerBadgeStat.MaxOxygen: return maxOxygenModifiers;
            case PlayerBadgeStat.StaminaRegenSpeed: return staminaRegenerationSpeedModifiers;
            default: return null;
        }
    }

    public void SetBadgeFlag(PlayerBadgeFlag flag, bool equipped)
    {
        switch (flag)
        {
            case PlayerBadgeFlag.FullHealthDefense: fullHealthDefenseBadgeEquipped = equipped; break;
            case PlayerBadgeFlag.SnailSlayerSpeed: snailSlayerBadgeEquipped = equipped; break;
            case PlayerBadgeFlag.ScoutPrincipianteDamage: scoutPrincipianteBadgeEquipped = equipped; break;
            case PlayerBadgeFlag.BoxSlayerOxygenGain: boxSlayerBadgeEquipped = equipped; break;
            case PlayerBadgeFlag.PrimeraInsigniaSpeed: primeraInsigniaBadgeEquipped = equipped; break;
        }
    }

    public void AddStrongAttackDamageModifiers(Modifier minModifier, Modifier maxModifier)
    {
        if (!minStrongAttackDamageModifiers.Contains(minModifier)) minStrongAttackDamageModifiers.Add(minModifier);
        if (!maxStrongAttackDamageModifiers.Contains(maxModifier)) maxStrongAttackDamageModifiers.Add(maxModifier);
    }
    public void RemoveStrongAttackDamageModifiers(Modifier minModifier, Modifier maxModifier)
    {
        minStrongAttackDamageModifiers.Remove(minModifier);
        maxStrongAttackDamageModifiers.Remove(maxModifier);
    }

    public void ClearAllBadgeModifiers()
    {
        maxHealthModifiers.Clear();
        maxStaminaModifiers.Clear();
        defenseModifiers.Clear();
        maxOxygenModifiers.Clear();
        staminaRegenerationSpeedModifiers.Clear();
        minStrongAttackDamageModifiers.Clear();
        maxStrongAttackDamageModifiers.Clear();
        fullHealthDefenseBadgeEquipped = false;
        snailSlayerBadgeEquipped = false;
        scoutPrincipianteBadgeEquipped = false;
        boxSlayerBadgeEquipped = false;
        primeraInsigniaBadgeEquipped = false;

        if (health > MaxHealth) health = MaxHealth;
        if (stamina > MaxStamina) stamina = MaxStamina;
        if (oxygen > MaxOxygen) oxygen = MaxOxygen;

        OnMaxHealthChanged?.Invoke();
        OnMaxStaminaChanged?.Invoke();
        OnMaxOxygenChanged?.Invoke();
    }

}