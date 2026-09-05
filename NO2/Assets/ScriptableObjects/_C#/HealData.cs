using System;
using System.Collections.Generic;
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

    // Modificador aplicado por la insignia (misma instancia reutilizada
    // para poder añadirla/quitarla de la lista por referencia)
    private readonly Modifier fastPotionsModifier = new Modifier(Modifier.ModifierType.Numeric, 2f);
    private List<Modifier> maxUsesModifiers = new List<Modifier>();

    // Referencias guardadas para poder desuscribirse correctamente
    private Action<string> onEquippedHandler;
    private Action<string> onUnequippedHandler;
    private Action onBadgesResetHandler;

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

    // El valor final se calcula aplicando los modificadores activos (insignias)
    // sobre el valor base configurado en el inspector
    public int MaxUses
    {
        get { return Mathf.RoundToInt(Modifier.ApplyModifiers(maxUses, maxUsesModifiers)); }
    }

    public int HealAmount
    {
        get { return healAmount; }
        set { healAmount = value; }
    }
    public float CooldownSeconds
    {
        get { return cooldownSeconds; }
        set { cooldownSeconds = value; }
    }
    public float ActualCooldownSeconds
    {
        get { return actualCooldownSeconds; }
        set
        {
            actualCooldownSeconds = value;
            actualCooldownChanged?.Invoke();
        }
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

        onEquippedHandler = (badgeName) => IncreaseMaxPotions(badgeName);
        onUnequippedHandler = (badgeName) => DecreaseMaxPotions(badgeName);
        onBadgesResetHandler = HandleBadgesReset;

        equippedBadges.OnEquipped += onEquippedHandler;
        equippedBadges.OnUnequipped += onUnequippedHandler;
        equippedBadges.OnReset += onBadgesResetHandler;
    }

    private void UnsubscribeFromBadges()
    {
        if (equippedBadges == null) return;

        if (onEquippedHandler != null)
            equippedBadges.OnEquipped -= onEquippedHandler;
        if (onUnequippedHandler != null)
            equippedBadges.OnUnequipped -= onUnequippedHandler;
        if (onBadgesResetHandler != null)
            equippedBadges.OnReset -= onBadgesResetHandler;
    }

    private void IncreaseMaxPotions(string badgeName)
    {
        if (fastPotionsName == badgeName)
        {
            if (!maxUsesModifiers.Contains(fastPotionsModifier))
                maxUsesModifiers.Add(fastPotionsModifier);

        }
    }
    private void DecreaseMaxPotions(string badgeName)
    {
        if (fastPotionsName == badgeName)
        {
            maxUsesModifiers.Remove(fastPotionsModifier);
            RemainingUses = Mathf.RoundToInt(Modifier.ApplyModifiers(maxUses, maxUsesModifiers));
        }
    }

    private void HandleBadgesReset()
    {
        maxUsesModifiers.Clear();
        healUsesChanged?.Invoke();
    }
}