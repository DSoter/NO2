using UnityEngine;
using UnityEngine.AI;

public class EnemyCPU : MonoBehaviour
{
    private NavMeshAgent agent;
    private Enemy enemyScript; // Reemplaza por tu script de comportamiento

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyScript = GetComponent<Enemy>();
    }

    void OnEnable() => AIManager.allEnemies.Add(this);
    void OnDisable() => AIManager.allEnemies.Remove(this);

    public void ToggleAI(bool activar)
    {
        if (agent != null && agent.enabled != activar) agent.enabled = activar;
        if (enemyScript != null && enemyScript.enabled != activar) enemyScript.enabled = activar;
    }
}
