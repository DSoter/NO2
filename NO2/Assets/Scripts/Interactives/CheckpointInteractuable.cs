using System.IO;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CheckpointInteractuable : Interactable
{
    public override void Interact()
    {
        CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();

        SceneController sc = FindAnyObjectByType<SceneController>();
        if (cm != null)
        {
            sc.UpdateSpawnPointAfterDeath(transform);
            SpawnId si = GetComponent<SpawnId>();
            cm.IdRespawn = si.IdSpawn;
            cm.SceneWhereRespawn = SceneManager.GetActiveScene().name;
            Debug.Log(SceneManager.GetActiveScene().name);
            //esto es pa reproducir sonido
            //sc.ReproducirCheckPoint(); 
            Debug.Log("Checkpoint alcanzado: " + gameObject.name);
        }
    }
}
