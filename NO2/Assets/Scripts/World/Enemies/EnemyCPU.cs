using UnityEngine;
using UnityEngine.AI;

public class EnemyCPU : MonoBehaviour
{
    [SerializeField] private float disableDistance = 40f;
    [SerializeField] private float checkInterval = 0.25f;

    private static Transform _player;
    private NavMeshAgent _agent;
    private Enemy _enemyScript;
    private float _timer;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _enemyScript = GetComponent<Enemy>();
        // Escalonado: reparte la carga entre frames sin necesitar un manager
        _timer = Random.Range(0f, checkInterval);
    }

    void OnEnable() => _timer = 0f;   // comprueba inmediatamente al activarse

    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer > 0f) return;
        _timer = checkInterval;

        if (_player == null)
        {
            var obj = GameObject.FindGameObjectWithTag("Player");
            if (obj == null) { ToggleAI(true); return; }  // fail-safe: encendido
            _player = obj.transform;
        }

        bool inRange = (transform.position - _player.position).sqrMagnitude
                       < disableDistance * disableDistance;
        ToggleAI(inRange);
    }

    private void ToggleAI(bool activar)
    {
        if (_enemyScript != null && _enemyScript.enabled != activar)
            _enemyScript.enabled = activar;

        if (_agent != null && _agent.enabled != activar)
        {
            _agent.enabled = activar;
            if (activar) ReattachToNavMesh();
        }
    }

    private void ReattachToNavMesh()
    {
        if (_agent.isOnNavMesh) return;
        if (NavMesh.SamplePosition(transform.position, out var hit, 2f, NavMesh.AllAreas))
            _agent.Warp(hit.position);
    }
}
