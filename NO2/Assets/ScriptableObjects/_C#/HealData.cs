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


    public Action healUsesChanged;
    public Action actualCooldownChanged;

    public enum HealBadgeStat { MaxUses }

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

    public void AddBadgeModifier(HealBadgeStat stat, Modifier modifier)
    {
        if (stat != HealBadgeStat.MaxUses) return;
        if (maxUsesModifiers.Contains(modifier)) return;
        maxUsesModifiers.Add(modifier);
        healUsesChanged?.Invoke();
    }

    public void RemoveBadgeModifier(HealBadgeStat stat, Modifier modifier)
    {
        if (stat != HealBadgeStat.MaxUses) return;
        maxUsesModifiers.Remove(modifier);
        RemainingUses = Mathf.RoundToInt(Modifier.ApplyModifiers(maxUses, maxUsesModifiers));
    }

    public void ClearAllBadgeModifiers()
    {
        maxUsesModifiers.Clear();
        healUsesChanged?.Invoke();
    }
}