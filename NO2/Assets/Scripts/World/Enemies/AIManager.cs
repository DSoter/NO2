using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Required to detect scene reloads

public class AIManager : MonoBehaviour
{
    [Header("Distance Settings")]
    [SerializeField] private float disableDistance = 40f;
    [SerializeField] private int enemiesPerFrame = 5;

    [Header("References")]
    [SerializeField] private Transform player;

    // Static list containing all active enemies in the current scene
    public static List<EnemyCPU> allEnemies = new List<EnemyCPU>();

    private int currentIndex = 0;
    private float disableDistanceSqr;

    // Using Awake to subscribe to Unity's scene unload event
    private void Awake()
    {
        // Subscribe to the event: "When a scene unloads, execute this method"
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
        // If the player died and the scene is reloading, we search for the new player instance
        if (player == null)
        {
            FindPlayer();
            return;
        }

        // If there are no enemies registered in the scene, do nothing
        if (allEnemies.Count == 0) return;

        // Distribute CPU load by evaluating only a small group of enemies per frame
        for (int i = 0; i < enemiesPerFrame; i++)
        {
            // Reset index if it exceeds the list count
            if (currentIndex >= allEnemies.Count)
            {
                currentIndex = 0;
            }

            EnemyCPU enemy = allEnemies[currentIndex];

            // Safety check in case the enemy was destroyed by other means (e.g., killed by damage)
            if (enemy != null)
            {
                // Measure distance using sqrMagnitude (extremely fast on CPU)
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
        // Completely clear the list for the next scene
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
