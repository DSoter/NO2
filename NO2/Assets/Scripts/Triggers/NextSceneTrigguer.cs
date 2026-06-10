using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NextSceneTrigguer: MonoBehaviour
{

    [SerializeField] private GateData gateData;

    private void Awake()
    {
        GetComponent<Collider2D>().enabled = true; //cambiar por corutina
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {
            CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();
            if (cm != null)
            {
                if (gateData.sceneName1.Equals(SceneManager.GetActiveScene().name))
                {
                    cm.FinishScene(gateData.sceneName2, gateData.gateId2);//cambiar nombre alpargata
                }
                else
                {
                    cm.FinishScene(gateData.sceneName1, gateData.gateId1);
                }

            }


            // Desactivar el trigger para que no se use más de una vez
            GetComponent<Collider2D>().enabled = false;
        }
    }
}