using System;
using UnityEngine;

[CreateAssetMenu(fileName = "HealData", menuName = "Scriptable Objects/HealData")]
public class HealData : ScriptableObject
{
    [Header("Valores base")]
    [SerializeField] private int baseMaxUses = 3;
    [SerializeField] private int baseHealAmount = 30;
    [SerializeField] private float baseCooldownSeconds = 2f;

    private int maxUses;
    private int remainingUses;
    private int healAmount;
    private float actualCooldownSeconds;
    private float cooldownSeconds;

    [SerializeField] private EquippedBadges equippedBadges;
    private string fastPotionsName = "Curacion rapida";

    private Action<string> onEquippedHandler;
    private Action<string> onUnequippedHandler;

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
    public int MaxUses
    {
        get { return maxUses; }
        set
        {
            maxUses = value;
            healUsesChanged?.Invoke();
            Save();
        }
    }
    public int HealAmount
    {
        get { return healAmount; }
        set { healAmount = value; Save(); }
    }
    public float CooldownSeconds
    {
        get { return cooldownSeconds; }
        set { cooldownSeconds = value; Save(); }
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

    private string SaveKey => name + "_healdata";

    private void OnEnable()
    {
        if (equippedBadges != null)
        {
            onEquippedHandler = (badgeName) => IncreaseMaxPotions(badgeName);
            onUnequippedHandler = (badgeName) => DecreaseMaxPotions(badgeName);

            equippedBadges.OnEquipped += onEquippedHandler;
            equippedBadges.OnUnequipped += onUnequippedHandler;
        }
    }
    private void OnDestroy()
    {
        UnsubscribeFromBadges();
    }
    private void OnDisable()
    {
        UnsubscribeFromBadges();
    }
    private void UnsubscribeFromBadges()
    {
        if (equippedBadges == null) return;

        if (onEquippedHandler != null)
            equippedBadges.OnEquipped -= onEquippedHandler;
        if (onUnequippedHandler != null)
            equippedBadges.OnUnequipped -= onUnequippedHandler;
    }
    private void IncreaseMaxPotions(string badgeName)
    {
        if (fastPotionsName == badgeName)
        {
            MaxUses = maxUses + 2;
            RemainingUses = remainingUses + 2;
        }
    }
    private void DecreaseMaxPotions(string badgeName)
    {
        if (fastPotionsName == badgeName)
        {
            MaxUses = maxUses - 2;
            RemainingUses = maxUses;
        }
    }

    public void Save()
    {
        PlayerPrefs.SetInt(SaveKey + "_maxUses", maxUses);
        PlayerPrefs.SetInt(SaveKey + "_remainingUses", remainingUses);
        PlayerPrefs.SetInt(SaveKey + "_healAmount", healAmount);
        PlayerPrefs.SetFloat(SaveKey + "_actualCooldownSeconds", actualCooldownSeconds);
        PlayerPrefs.SetFloat(SaveKey + "_cooldownSeconds", cooldownSeconds);
        PlayerPrefs.Save();
    }

    public bool Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey + "_maxUses"))
            return false;

        maxUses = PlayerPrefs.GetInt(SaveKey + "_maxUses", baseMaxUses);
        remainingUses = PlayerPrefs.GetInt(SaveKey + "_remainingUses", baseMaxUses);
        healAmount = PlayerPrefs.GetInt(SaveKey + "_healAmount", baseHealAmount);
        actualCooldownSeconds = PlayerPrefs.GetFloat(SaveKey + "_actualCooldownSeconds", baseCooldownSeconds);
        cooldownSeconds = PlayerPrefs.GetFloat(SaveKey + "_cooldownSeconds", baseCooldownSeconds);

        healUsesChanged?.Invoke();
        actualCooldownChanged?.Invoke();
        return true;
    }

    public void Reset()
    {
        maxUses = baseMaxUses;
        remainingUses = baseMaxUses;
        healAmount = baseHealAmount;
        actualCooldownSeconds = baseCooldownSeconds;
        cooldownSeconds = baseCooldownSeconds;

        Save();

        healUsesChanged?.Invoke();
        actualCooldownChanged?.Invoke();
    }
}