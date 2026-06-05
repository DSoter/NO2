using UnityEngine;
using UnityEngine.SceneManagement;
public class CheckpointInteractuable : Interactable
{
    public override void Interact()
    {
        CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();
        if (cm != null)
        {
            cm.UpdateSpawnPoint(transform);
            cm.SetCurrentSceneName(SceneManager.GetActiveScene().name);

            //esto es pa reproducir sonido
            //sc.ReproducirCheckPoint(); 
            Debug.Log("Checkpoint alcanzado: " + gameObject.name);
        }
    }
}
