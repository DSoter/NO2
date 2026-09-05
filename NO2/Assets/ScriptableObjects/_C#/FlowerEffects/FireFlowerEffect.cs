using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireFlowerEffect", menuName = "Scriptable Objects/FlowerEffects/FireFlowerEffect")]
public class FireFlowerEffect : FlowerEffect
{
    public int burnSeconds = 5; // Duración del efecto de quemadura en segundos
    public float burnDamagePerSecond = 5f; // Daño por segundo del efecto de quemadura (base)
    public float vulnableDamageMultiplier = 2f; // Multiplicador de daño de quemadura para ataques vulnerables

    [Header("Logros")]
    [SerializeField] private EquippedBadges equippedBadges;
    [SerializeField] private string nameBadgePrimeraFlor = "Primera flor";

    // +1 de daño al quemado
    private readonly Modifier primeraFlorModifier = new Modifier(Modifier.ModifierType.Numeric, 1f);
    private List<Modifier> normalBurnModifiers = new List<Modifier>();

    private Action<string> onEquippedHandler;
    private Action<string> onUnequippedHandler;
    private Action onBadgesResetHandler;


    private float BurnDamagePerSecond
    {
        get { return Modifier.ApplyModifiers(burnDamagePerSecond, normalBurnModifiers); }
    }
       
    private void OnEnable()
    {
        SubscribeToBadges();
    }
    private void OnDisable()
    {
        UnsubscribeFromBadges();
    }
    private void OnDestroy()
    {
        UnsubscribeFromBadges();
    }

    private void SubscribeToBadges()
    {
        if (equippedBadges == null) return;

        onEquippedHandler = (badgeName) => IncreaseNormalBurnDamage(badgeName);
        onUnequippedHandler = (badgeName) => DecreaseNormalBurnDamage(badgeName);
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

    private void IncreaseNormalBurnDamage(string badgeName)
    {
        if (nameBadgePrimeraFlor == badgeName)
        {
            if (normalBurnModifiers.Contains(primeraFlorModifier)) return;
            normalBurnModifiers.Add(primeraFlorModifier);
        }
    }
    private void DecreaseNormalBurnDamage(string badgeName)
    {
        if (nameBadgePrimeraFlor == badgeName)
        {
            normalBurnModifiers.Remove(primeraFlorModifier);
        }
    }

    private void HandleBadgesReset()
    {
        normalBurnModifiers.Clear();
    }

    public override void OnWeakAttackHitEnemy(GameObject enemy, PlayerData playerData)
    {
        if (enemy.TryGetComponent<IEffectable>(out var enemyScript))
        {
            enemyScript.ApplyBurn(BurnDamagePerSecond, burnSeconds);
        }
    }

    public override void OnStrongAttackHitEnemy(GameObject enemy, PlayerData playerData)
    {
        if (enemy.TryGetComponent<IEffectable>(out var enemyScript))
        {
            enemyScript.ApplyBurn(BurnDamagePerSecond, burnSeconds);
        }
    }

    public override void OnVulnerableHitEnemy(GameObject enemy, PlayerData playerData)
    {
        if (enemy.TryGetComponent<IEffectable>(out var enemyScript))
        {
            enemyScript.ApplySevereBurn(BurnDamagePerSecond * vulnableDamageMultiplier, burnSeconds);
        }
    }
}