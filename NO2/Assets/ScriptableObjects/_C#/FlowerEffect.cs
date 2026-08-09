using UnityEngine;

[CreateAssetMenu(fileName = "FlowerEffect", menuName = "Scriptable Objects/FlowerEffect")]

public abstract class FlowerEffect : ScriptableObject
{
    // Ataque debil
    public virtual void OnWeakAttackHitEnemy(GameObject enemy, PlayerData playerData) { }
    public virtual void OnWeakAttackHitPlayer(PlayerData playerData) { }

    // Ataque cargado
    public virtual void OnStrongAttackHitEnemy(GameObject enemy, PlayerData playerData) { }
    public virtual void OnStrongAttackHitPlayer(PlayerData playerData) { }

    // Ataque cargado en momento vulnerable
    public virtual void OnVulnerableHitEnemy(GameObject enemy, PlayerData playerData) { }
    public virtual void OnVulnerableHitPlayer(PlayerData playerData) { }
}
