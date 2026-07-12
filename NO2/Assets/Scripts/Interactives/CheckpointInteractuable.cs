using System.IO;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CheckpointInteractuable : Interactable
{
    private ParticleSystem sistemaParts;

    protected override void Start()
    {
        base.Start();
        if (transform.GetChild(0) != null) { 
            sistemaParts = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        }
    }
    public override void Interact()
    {
        CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();

        
        SceneController sc = FindAnyObjectByType<SceneController>();
        if (cm != null)
        {
            PlayerController player = cm.PlayerReference.gameObject.GetComponent<PlayerController>();
            if (player.GetState() == PlayerController.PlayerState.Rest)
            {

                //Se levanta
                _playerController.ChangeDisplayText("Descansar");
                player.SetState(PlayerController.PlayerState.Move);
            }
            else
            {
                //Se sienta
                _playerController.ChangeDisplayText("Levantarse");
                player.SetState(PlayerController.PlayerState.Rest);
                if(sistemaParts != null)
                {
                    sistemaParts.Emit(48);
                }
                sc.UpdateSpawnPointAfterDeath(transform);
                SpawnId si = GetComponent<SpawnId>();
                int id = si.IdSpawn;
                cm.IdRespawn = id;
                cm.SceneWhereRespawn = SceneManager.GetActiveScene().name;
                //esto es pa reproducir sonido
                //sc.ReproducirCheckPoint(); 
                Debug.Log("Checkpoint alcanzado: " + gameObject.name);

            }
        }
        
    }
    protected override void UniqueEnter()
    {
        base.UniqueEnter();
        _playerController.ChangeDisplayText("Descansar");
    }

    protected override void UpdateDisplayText()
    {
        base.UpdateDisplayText();
        _playerController.ChangeDisplayText("Descansar");
    }

}
