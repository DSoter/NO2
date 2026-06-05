using UnityEngine;

public class CheckpointInteractuable : Interactable
{
    public override void Interact()
    {
        SceneController sc = FindAnyObjectByType<SceneController>();
        if (sc != null)
        {
            sc.UpdateSpawnPoint(this.transform);

            //esto es pa reproducir sonido
            //sc.ReproducirCheckPoint(); 
            Debug.Log("Checkpoint alcanzado: " + gameObject.name);
        }
    }
}
