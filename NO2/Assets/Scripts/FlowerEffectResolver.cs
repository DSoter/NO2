using UnityEngine;

public static class FlowerEffectResolver
{
    public static void ApplyOnHit(FlowerEffect flower, GameObject enemy, PlayerData playerData, AttackStrength attackType)
    {
        if (flower == null) return;

        bool isVulnerable = enemy.TryGetComponent<IVulnerable>(out var vulnerable) && vulnerable.IsVulnerable;

        if (isVulnerable && attackType == AttackStrength.Strong)
        {
            flower.OnVulnerableHitEnemy(enemy, playerData);
            flower.OnVulnerableHitPlayer(playerData);
            return;
        }

        switch (attackType)
        {
            case AttackStrength.Weak:
                flower.OnWeakAttackHitEnemy(enemy, playerData);
                flower.OnWeakAttackHitPlayer(playerData);
                break;

            case AttackStrength.Strong:
                flower.OnStrongAttackHitEnemy(enemy, playerData);
                flower.OnStrongAttackHitPlayer(playerData);
                break;
        }
    }
}
