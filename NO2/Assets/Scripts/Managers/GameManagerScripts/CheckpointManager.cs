using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    private Transform currentSpawnPoint;
    private string currentSceneName;
    private bool hasToSpawnPlayer;
    private bool hasToSpawnPlayerAfterDeath;
    public void UpdateSpawnPoint(Transform newSpawn)
    {
        currentSpawnPoint = newSpawn;
    }
    public Transform GetCurrentSpawnPoint()
    {
        return currentSpawnPoint;
    }
    public void SetCurrentSpawnPoint(Transform newSpawn)
    {
        currentSpawnPoint = newSpawn;
    }

    public string GetCurrentSceneName()
    {
        return currentSceneName;
    }
    public void SetCurrentSceneName(string newName)
    {
        currentSceneName = newName;
    }

    public void FinishScene(string sceneName, Transform newSpawnPoint)
    {
        SceneController sc = FindAnyObjectByType<SceneController>();
        if (sc != null)
        {
            sc.UpdateSpawnPoint(newSpawnPoint);
        }
        currentSceneName= sceneName;
        SceneManager.LoadScene(sceneName);
        SpawnPlayer();
    }
    public void StartScene(string sceneName)
    {
        hasToSpawnPlayer = true;
        SceneManager.LoadScene(sceneName);
        
        
    }
    public void SpawnPlayer()
    {
        if (currentSceneName is null) {
            currentSceneName = SceneManager.GetActiveScene().name;
        }
        if (!(SceneManager.GetActiveScene().name.Equals(currentSceneName))) {
            hasToSpawnPlayer = true;
            SceneManager.LoadScene(currentSceneName);
        }
        SceneController sc = FindAnyObjectByType<SceneController>(); 
        sc.SpawnPlayer();

    }
    public void SpawnPlayerAfterDeath()
    {
        if (currentSceneName is null) {
            currentSceneName = SceneManager.GetActiveScene().name;
        }
        if (!SceneManager.GetActiveScene().name.Equals(currentSceneName))
        {
            hasToSpawnPlayerAfterDeath = true;
            SceneManager.LoadScene(currentSceneName);
        }
        SceneController sc = FindAnyObjectByType<SceneController>();
        sc.SpawnPlayerAfterDeath();
    }
    public bool GetHasToSpawn()
    {
        return hasToSpawnPlayer;
    }
    public void SetHasToSpawn(bool value)
    {
        hasToSpawnPlayer= value;
    }

    public bool GetHasToSpawnAfterDeath()
    {
        return hasToSpawnPlayerAfterDeath;
    }
    public void SetHasToSpawnAfterDeath(bool value)
    {
        hasToSpawnPlayerAfterDeath = value;
    }

}
