using UnityEngine;

[CreateAssetMenu(fileName = "FireFlowerEffect", menuName = "Scriptable Objects/FlowerEffects/FireFlowerEffect")]
public class FireFlowerEffect : FlowerEffect
{
    public int burnSeconds = 5; // Duración del efecto de quemadura en segundos
    public float burnDamagePerSecond = 5f; // Daño por segundo del efecto de quemadura
    public float vulnableDamageMultiplier = 2f; // Multiplicador de daño de quemadura para ataques vulnerables

    public override void OnWeakAttackHitEnemy(GameObject enemy, PlayerData playerData)
    {
        if(enemy.TryGetComponent<IEffectable>(out var enemyScript))
        {
            enemyScript.ApplyBurn(burnDamagePerSecond, burnSeconds);
        }
    }

    public override void OnStrongAttackHitEnemy(GameObject enemy, PlayerData playerData)
    {
        if (enemy.TryGetComponent<IEffectable>(out var enemyScript))
        {
            enemyScript.ApplyBurn(burnDamagePerSecond, burnSeconds);
        }
    }

    public override void OnVulnerableHitEnemy(GameObject enemy, PlayerData playerData)
    {
        if (enemy.TryGetComponent<IEffectable>(out var enemyScript))
        {
            enemyScript.ApplySevereBurn(burnDamagePerSecond * vulnableDamageMultiplier, burnSeconds);
        }
    }
}
