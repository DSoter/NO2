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
    [SerializeField] private string nameBadgeHundredRolls = "Acrobata amateur";
    [SerializeField] private string nameBadgeStrongHeart = "Corazon fuerte";
    [SerializeField] private string nameBadgeToughSkin = "Piel dura";
    [SerializeField] private string nameBadgeFullHealthDefense = "Vida plena";
    [SerializeField] private string nameBadgeNO2 = "NO2";
    [SerializeField] private string nameBadgeSnailSlayer = "Snail slayer";
    [SerializeField] private string nameBadgeScoutPrincipiante = "Scout principiante";
    [SerializeField] private string nameBadgeBoxSlayer = "Box slayer";
    [SerializeField] private string nameBadgePrimeraInsignia = "Primera insignia";
    [SerializeField] private string nameBadgeExpertAcrobat = "Acróbata experto";

    // Umbral compartido para las condiciones "un cuarto o menos de X máximo"
    private const float LowResourceThreshold = 0.25f;

    // Modificadores activos por insignias (no se persisten: se reconstruyen a partir de EquippedBadges)
    private readonly Modifier strongHeartModifier = new Modifier(Modifier.ModifierType.Numeric, 10f);
    private readonly Modifier hundredRollsModifier = new Modifier(Modifier.ModifierType.Numeric, 10f);
    private readonly Modifier toughSkinModifier = new Modifier(Modifier.ModifierType.Percentage, 3f);
    private readonly Modifier fullHealthDefenseModifier = new Modifier(Modifier.ModifierType.Percentage, 70f);
    private readonly Modifier no2Modifier = new Modifier(Modifier.ModifierType.Percentage, 20f);

    // ASUNCIÓN: +20% de velocidad (a pie y corriendo) mientras se cumpla la condición de cada insignia.
    private readonly Modifier snailSlayerSpeedModifier = new Modifier(Modifier.ModifierType.Percentage, 20f);
    private readonly Modifier primeraInsigniaSpeedModifier = new Modifier(Modifier.ModifierType.Percentage, 20f);

    // Valor exacto que pediste: +2 de daño plano en cada uno de los tres ataques
    private readonly Modifier scoutPrincipianteDamageModifier = new Modifier(Modifier.ModifierType.Numeric, 2f);

    // Valor exacto que pediste: +10% del oxígeno ganado en cada recogida
    private readonly Modifier boxSlayerOxygenGainModifier = new Modifier(Modifier.ModifierType.Percentage, 10f);

    // ASUNCIÓN: +30% de velocidad de regeneración de resistencia
    private readonly Modifier expertAcrobatStaminaRegenModifier = new Modifier(Modifier.ModifierType.Percentage, 30f);

    private List<Modifier> maxHealthModifiers = new List<Modifier>();
    private List<Modifier> maxStaminaModifiers = new List<Modifier>();
    private List<Modifier> defenseModifiers = new List<Modifier>();
    private List<Modifier> maxOxygenModifiers = new List<Modifier>();
    private List<Modifier> staminaRegenerationSpeedModifiers = new List<Modifier>();
    private bool fullHealthDefenseBadgeEquipped;
    private bool snailSlayerBadgeEquipped;
    private bool scoutPrincipianteBadgeEquipped;
    private bool boxSlayerBadgeEquipped;
    private bool primeraInsigniaBadgeEquipped;

    // Referencias guardadas para poder desuscribirse correctamente
    private Action<string> onEquippedIncreaseHealthHandler;
    private Action<string> onUnequippedDecreaseHealthHandler;
    private Action<string> onEquippedIncreaseStaminaHandler;
    private Action<string> onUnequippedDecreaseStaminaHandler;
    private Action<string> onEquippedIncreaseDefenseHandler;
    private Action<string> onUnequippedDecreaseDefenseHandler;
    private Action<string> onEquippedIncreaseOxygenHandler;
    private Action<string> onUnequippedDecreaseOxygenHandler;
    private Action<string> onEquippedConditionalSpeedHandler;
    private Action<string> onUnequippedConditionalSpeedHandler;
    private Action<string> onEquippedConditionalDamageHandler;
    private Action<string> onUnequippedConditionalDamageHandler;
    private Action<string> onEquippedOxygenGainBonusHandler;
    private Action<string> onUnequippedOxygenGainBonusHandler;
    private Action<string> onEquippedStaminaRegenHandler;
    private Action<string> onUnequippedStaminaRegenHandler;
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
        set
        {
            if (health <= 1 && 0 < health && value > 1)
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

    // "Acróbata experto": bonus permanente mientras esté equipada, no depende de ninguna condición
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
    // su bonus de velocidad aquí. Al evaluarse en el get, se activan/desactivan
    // automáticamente según cambie la vida o el oxígeno, sin necesitar más eventos.
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

    public float MinStrongAttackDamage
    {
        get { return Modifier.ApplyModifiers(minStrongAttackDamage, GetActiveScoutDamageModifiers()); }
    }
    public float MaxStrongAttackDamage
    {
        get { return Modifier.ApplyModifiers(maxStrongAttackDamage, GetActiveScoutDamageModifiers()); }
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

    private void OnEnable()
    {
        SubscribeToBadges();
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
        onEquippedIncreaseDefenseHandler = (badgeName) => IncreaseDefense(badgeName);
        onUnequippedDecreaseDefenseHandler = (badgeName) => DecreaseDefense(badgeName);
        onEquippedIncreaseOxygenHandler = (badgeName) => IncreaseMaxOxygen(badgeName);
        onUnequippedDecreaseOxygenHandler = (badgeName) => DecreaseMaxOxygen(badgeName);
        onEquippedConditionalSpeedHandler = (badgeName) => EquipConditionalSpeedBadge(badgeName);
        onUnequippedConditionalSpeedHandler = (badgeName) => UnequipConditionalSpeedBadge(badgeName);
        onEquippedConditionalDamageHandler = (badgeName) => EquipConditionalDamageBadge(badgeName);
        onUnequippedConditionalDamageHandler = (badgeName) => UnequipConditionalDamageBadge(badgeName);
        onEquippedOxygenGainBonusHandler = (badgeName) => EquipOxygenGainBonusBadge(badgeName);
        onUnequippedOxygenGainBonusHandler = (badgeName) => UnequipOxygenGainBonusBadge(badgeName);
        onEquippedStaminaRegenHandler = (badgeName) => IncreaseStaminaRegenSpeed(badgeName);
        onUnequippedStaminaRegenHandler = (badgeName) => DecreaseStaminaRegenSpeed(badgeName);
        onBadgesResetHandler = HandleBadgesReset;

        equippedBadges.OnEquipped += onEquippedIncreaseHealthHandler;
        equippedBadges.OnUnequipped += onUnequippedDecreaseHealthHandler;
        equippedBadges.OnEquipped += onEquippedIncreaseStaminaHandler;
        equippedBadges.OnUnequipped += onUnequippedDecreaseStaminaHandler;
        equippedBadges.OnEquipped += onEquippedIncreaseDefenseHandler;
        equippedBadges.OnUnequipped += onUnequippedDecreaseDefenseHandler;
        equippedBadges.OnEquipped += onEquippedIncreaseOxygenHandler;
        equippedBadges.OnUnequipped += onUnequippedDecreaseOxygenHandler;
        equippedBadges.OnEquipped += onEquippedConditionalSpeedHandler;
        equippedBadges.OnUnequipped += onUnequippedConditionalSpeedHandler;
        equippedBadges.OnEquipped += onEquippedConditionalDamageHandler;
        equippedBadges.OnUnequipped += onUnequippedConditionalDamageHandler;
        equippedBadges.OnEquipped += onEquippedOxygenGainBonusHandler;
        equippedBadges.OnUnequipped += onUnequippedOxygenGainBonusHandler;
        equippedBadges.OnEquipped += onEquippedStaminaRegenHandler;
        equippedBadges.OnUnequipped += onUnequippedStaminaRegenHandler;
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
        if (onEquippedIncreaseDefenseHandler != null)
            equippedBadges.OnEquipped -= onEquippedIncreaseDefenseHandler;
        if (onUnequippedDecreaseDefenseHandler != null)
            equippedBadges.OnUnequipped -= onUnequippedDecreaseDefenseHandler;
        if (onEquippedIncreaseOxygenHandler != null)
            equippedBadges.OnEquipped -= onEquippedIncreaseOxygenHandler;
        if (onUnequippedDecreaseOxygenHandler != null)
            equippedBadges.OnUnequipped -= onUnequippedDecreaseOxygenHandler;
        if (onEquippedConditionalSpeedHandler != null)
            equippedBadges.OnEquipped -= onEquippedConditionalSpeedHandler;
        if (onUnequippedConditionalSpeedHandler != null)
            equippedBadges.OnUnequipped -= onUnequippedConditionalSpeedHandler;
        if (onEquippedConditionalDamageHandler != null)
            equippedBadges.OnEquipped -= onEquippedConditionalDamageHandler;
        if (onUnequippedConditionalDamageHandler != null)
            equippedBadges.OnUnequipped -= onUnequippedConditionalDamageHandler;
        if (onEquippedOxygenGainBonusHandler != null)
            equippedBadges.OnEquipped -= onEquippedOxygenGainBonusHandler;
        if (onUnequippedOxygenGainBonusHandler != null)
            equippedBadges.OnUnequipped -= onUnequippedOxygenGainBonusHandler;
        if (onEquippedStaminaRegenHandler != null)
            equippedBadges.OnEquipped -= onEquippedStaminaRegenHandler;
        if (onUnequippedStaminaRegenHandler != null)
            equippedBadges.OnUnequipped -= onUnequippedStaminaRegenHandler;
        if (onBadgesResetHandler != null)
            equippedBadges.OnReset -= onBadgesResetHandler;
    }

    private void IncreaseMaxHealth(string badgeName)
    {
        if (nameBadgeStrongHeart == badgeName)
        {
            if (maxHealthModifiers.Contains(strongHeartModifier)) return;

            maxHealthModifiers.Add(strongHeartModifier);
            Health = health + 10;
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
            if (maxStaminaModifiers.Contains(hundredRollsModifier)) return;

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

    private void IncreaseDefense(string badgeName)
    {
        if (nameBadgeToughSkin == badgeName)
        {
            if (defenseModifiers.Contains(toughSkinModifier)) return;
            defenseModifiers.Add(toughSkinModifier);
        }
        else if (nameBadgeFullHealthDefense == badgeName)
        {
            fullHealthDefenseBadgeEquipped = true;
        }
    }
    private void DecreaseDefense(string badgeName)
    {
        if (nameBadgeToughSkin == badgeName)
        {
            defenseModifiers.Remove(toughSkinModifier);
        }
        else if (nameBadgeFullHealthDefense == badgeName)
        {
            fullHealthDefenseBadgeEquipped = false;
        }
    }

    private void IncreaseMaxOxygen(string badgeName)
    {
        if (nameBadgeNO2 == badgeName)
        {
            if (maxOxygenModifiers.Contains(no2Modifier)) return;

            maxOxygenModifiers.Add(no2Modifier);
            Oxygen = oxygen; // reclamp por si el aumento de MaxOxygen debe reflejarse, y dispara OnOxygenChanged
            OnMaxOxygenChanged?.Invoke();
        }
    }
    private void DecreaseMaxOxygen(string badgeName)
    {
        if (nameBadgeNO2 == badgeName)
        {
            maxOxygenModifiers.Remove(no2Modifier);

            if (oxygen > MaxOxygen)
            {
                oxygen = MaxOxygen;
            }
            OnMaxOxygenChanged?.Invoke();
        }
    }

    private void EquipConditionalSpeedBadge(string badgeName)
    {
        if (nameBadgeSnailSlayer == badgeName)
        {
            snailSlayerBadgeEquipped = true;
        }
        else if (nameBadgePrimeraInsignia == badgeName)
        {
            primeraInsigniaBadgeEquipped = true;
        }
    }
    private void UnequipConditionalSpeedBadge(string badgeName)
    {
        if (nameBadgeSnailSlayer == badgeName)
        {
            snailSlayerBadgeEquipped = false;
        }
        else if (nameBadgePrimeraInsignia == badgeName)
        {
            primeraInsigniaBadgeEquipped = false;
        }
    }

    private void EquipConditionalDamageBadge(string badgeName)
    {
        if (nameBadgeScoutPrincipiante == badgeName)
        {
            scoutPrincipianteBadgeEquipped = true;
        }
    }
    private void UnequipConditionalDamageBadge(string badgeName)
    {
        if (nameBadgeScoutPrincipiante == badgeName)
        {
            scoutPrincipianteBadgeEquipped = false;
        }
    }

    private void EquipOxygenGainBonusBadge(string badgeName)
    {
        if (nameBadgeBoxSlayer == badgeName)
        {
            boxSlayerBadgeEquipped = true;
        }
    }
    private void UnequipOxygenGainBonusBadge(string badgeName)
    {
        if (nameBadgeBoxSlayer == badgeName)
        {
            boxSlayerBadgeEquipped = false;
        }
    }

    private void IncreaseStaminaRegenSpeed(string badgeName)
    {
        if (nameBadgeExpertAcrobat == badgeName)
        {
            if (staminaRegenerationSpeedModifiers.Contains(expertAcrobatStaminaRegenModifier)) return;
            staminaRegenerationSpeedModifiers.Add(expertAcrobatStaminaRegenModifier);
        }
    }
    private void DecreaseStaminaRegenSpeed(string badgeName)
    {
        if (nameBadgeExpertAcrobat == badgeName)
        {
            staminaRegenerationSpeedModifiers.Remove(expertAcrobatStaminaRegenModifier);
        }
    }

    private void HandleBadgesReset()
    {
        maxHealthModifiers.Clear();
        maxStaminaModifiers.Clear();
        defenseModifiers.Clear();
        maxOxygenModifiers.Clear();
        staminaRegenerationSpeedModifiers.Clear();
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