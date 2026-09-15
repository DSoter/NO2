using System.Collections.Generic;
using UnityEngine;

public class BadgeManager : MonoBehaviour
{
    [SerializeField] private EquippedBadges equippedBadges;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private HealData healData;

    [Header("Nombres de insignias")]
    [SerializeField] private string nameBadgeStrongHeart = "Corazon fuerte";
    [SerializeField] private string nameBadgeHundredRolls = "Acrobata amateur";
    [SerializeField] private string nameBadgeToughSkin = "Piel dura";
    [SerializeField] private string nameBadgeFullHealthDefense = "Vida plena";
    [SerializeField] private string nameBadgeNO2 = "NO2";
    [SerializeField] private string nameBadgeSnailSlayer = "Snail slayer";
    [SerializeField] private string nameBadgeScoutPrincipiante = "Scout principiante";
    [SerializeField] private string nameBadgeBoxSlayer = "Box slayer";
    [SerializeField] private string nameBadgePrimeraInsignia = "Primera insignia";
    [SerializeField] private string nameBadgeExpertAcrobat = "Acróbata experto";
    [SerializeField] private string nameBadgeBigCharge = "Peso pesado";
    [SerializeField] private string nameBadgeFastPotions = "Curacion rapida";

    private readonly Modifier strongHeartModifier = new Modifier(Modifier.ModifierType.Numeric, 10f);
    private readonly Modifier hundredRollsModifier = new Modifier(Modifier.ModifierType.Numeric, 10f);
    private readonly Modifier toughSkinModifier = new Modifier(Modifier.ModifierType.Percentage, 3f);
    private readonly Modifier no2Modifier = new Modifier(Modifier.ModifierType.Percentage, 20f);
    private readonly Modifier expertAcrobatStaminaRegenModifier = new Modifier(Modifier.ModifierType.Percentage, 30f);
    private readonly Modifier bigChargeMinModifier = new Modifier(Modifier.ModifierType.Numeric, 2f);
    private readonly Modifier bigChargeMaxModifier = new Modifier(Modifier.ModifierType.Numeric, 2f);
    private readonly Modifier fastPotionsModifier = new Modifier(Modifier.ModifierType.Numeric, 2f);

    private void OnEnable()
    {
        equippedBadges.OnEquipped += HandleEquipped;
        equippedBadges.OnUnequipped += HandleUnequipped;
        equippedBadges.OnReset += HandleReset;
    }

    private void OnDisable()
    {
        equippedBadges.OnEquipped -= HandleEquipped;
        equippedBadges.OnUnequipped -= HandleUnequipped;
        equippedBadges.OnReset -= HandleReset;
    }

    private void HandleEquipped(string badgeName) => ApplyBadge(badgeName, true);
    private void HandleUnequipped(string badgeName) => ApplyBadge(badgeName, false);

    private void HandleReset()
    {
        playerData.ClearAllBadgeModifiers();
        healData.ClearAllBadgeModifiers();
    }

    // Único sitio del proyecto que compara nombres de insignia
    private void ApplyBadge(string badgeName, bool equip)
    {
        if (badgeName == nameBadgeStrongHeart)
        {
            if (equip) playerData.AddBadgeModifier(PlayerData.PlayerBadgeStat.MaxHealth, strongHeartModifier);
            else playerData.RemoveBadgeModifier(PlayerData.PlayerBadgeStat.MaxHealth, strongHeartModifier);
        }
        else if (badgeName == nameBadgeHundredRolls)
        {
            if (equip) playerData.AddBadgeModifier(PlayerData.PlayerBadgeStat.MaxStamina, hundredRollsModifier);
            else playerData.RemoveBadgeModifier(PlayerData.PlayerBadgeStat.MaxStamina, hundredRollsModifier);
        }
        else if (badgeName == nameBadgeToughSkin)
        {
            if (equip) playerData.AddBadgeModifier(PlayerData.PlayerBadgeStat.Defense, toughSkinModifier);
            else playerData.RemoveBadgeModifier(PlayerData.PlayerBadgeStat.Defense, toughSkinModifier);
        }
        else if (badgeName == nameBadgeFullHealthDefense)
        {
            playerData.SetBadgeFlag(PlayerData.PlayerBadgeFlag.FullHealthDefense, equip);
        }
        else if (badgeName == nameBadgeNO2)
        {
            if (equip) playerData.AddBadgeModifier(PlayerData.PlayerBadgeStat.MaxOxygen, no2Modifier);
            else playerData.RemoveBadgeModifier(PlayerData.PlayerBadgeStat.MaxOxygen, no2Modifier);
        }
        else if (badgeName == nameBadgeSnailSlayer)
        {
            playerData.SetBadgeFlag(PlayerData.PlayerBadgeFlag.SnailSlayerSpeed, equip);
        }
        else if (badgeName == nameBadgeScoutPrincipiante)
        {
            playerData.SetBadgeFlag(PlayerData.PlayerBadgeFlag.ScoutPrincipianteDamage, equip);
        }
        else if (badgeName == nameBadgeBoxSlayer)
        {
            playerData.SetBadgeFlag(PlayerData.PlayerBadgeFlag.BoxSlayerOxygenGain, equip);
        }
        else if (badgeName == nameBadgePrimeraInsignia)
        {
            playerData.SetBadgeFlag(PlayerData.PlayerBadgeFlag.PrimeraInsigniaSpeed, equip);
        }
        else if (badgeName == nameBadgeExpertAcrobat)
        {
            if (equip) playerData.AddBadgeModifier(PlayerData.PlayerBadgeStat.StaminaRegenSpeed, expertAcrobatStaminaRegenModifier);
            else playerData.RemoveBadgeModifier(PlayerData.PlayerBadgeStat.StaminaRegenSpeed, expertAcrobatStaminaRegenModifier);
        }
        else if (badgeName == nameBadgeBigCharge)
        {
            if (equip) playerData.AddStrongAttackDamageModifiers(bigChargeMinModifier, bigChargeMaxModifier);
            else playerData.RemoveStrongAttackDamageModifiers(bigChargeMinModifier, bigChargeMaxModifier);
        }
        else if (badgeName == nameBadgeFastPotions)
        {
            if (equip) healData.AddBadgeModifier(HealData.HealBadgeStat.MaxUses, fastPotionsModifier);
            else healData.RemoveBadgeModifier(HealData.HealBadgeStat.MaxUses, fastPotionsModifier);
        }
    }
    public void ResyncModifiers()
    {
        playerData.ClearAllBadgeModifiers();
        healData.ClearAllBadgeModifiers();

        HashSet<Badge> notified = new HashSet<Badge>();
        foreach (Badge badge in equippedBadges.Slots)
        {
            if (badge == null || notified.Contains(badge)) continue;
            notified.Add(badge);
            ApplyBadge(badge.badgeName, true);
        }
    }

    public void ClearAllModifiers()
    {
        playerData.ClearAllBadgeModifiers();
        healData.ClearAllBadgeModifiers();
    }
}