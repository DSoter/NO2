using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AIManager : MonoBehaviour
{
    [Header("Distance Settings")]
    [SerializeField] private float disableDistance = 40f;
    [SerializeField] private int enemiesPerFrame = 5;

    [Header("References")]
    [SerializeField] private Transform player;

    public static List<EnemyCPU> allEnemies = new List<EnemyCPU>();

    private int currentIndex = 0;
    private float disableDistanceSqr;

    private void Awake()
    {
        SceneManager.sceneUnloaded += OnSceneCleanUp;
    }

    void Start()
    {
        // Precalculate the squared distance to save CPU performance (avoids costly square roots)
        disableDistanceSqr = disableDistance * disableDistance;

        if (player == null)
        {
            FindPlayer();
        }
    }

    void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        if (allEnemies.Count == 0) return;

        for (int i = 0; i < enemiesPerFrame; i++)
        {
            if (currentIndex >= allEnemies.Count)
            {
                currentIndex = 0;
            }

            EnemyCPU enemy = allEnemies[currentIndex];

            if (enemy != null)
            {
                float distanceSqr = (enemy.transform.position - player.position).sqrMagnitude;
                bool isInRange = distanceSqr < disableDistanceSqr;

                enemy.ToggleAI(isInRange);
            }

            currentIndex++;
        }
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    // Automatically triggered by Unity right before the scene reloads
    private void OnSceneCleanUp(Scene currentScene)
    {
        allEnemies.Clear();
        currentIndex = 0;

        // Force the manager to find the new player instance in the newly loaded scene
        player = null;
    }

    private void OnDestroy()
    {
        // Good practice: Unsubscribe from the event if the game closes entirely
        SceneManager.sceneUnloaded -= OnSceneCleanUp;
    }
}
