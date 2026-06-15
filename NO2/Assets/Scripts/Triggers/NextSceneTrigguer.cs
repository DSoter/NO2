using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NextSceneTrigguer: MonoBehaviour
{

    [SerializeField] private GateData gateData;

    private void Awake()
    {
        GetComponent<Collider2D>().enabled = false;
        CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();
        StartCoroutine(WaitAndActivateTriger(cm.GateTransitionSeconds));
    }
    private IEnumerator WaitAndActivateTriger(float waitDurationSeconds)
    {
        yield return new WaitForSeconds(waitDurationSeconds);
        GetComponent<Collider2D>().enabled = true; 
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {
            CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();
            if (cm != null)
            {
                // Desactivar el trigger para que no se use más de una vez
                GetComponent<Collider2D>().enabled = false;
                if (gateData.sceneName1.Equals(SceneManager.GetActiveScene().name))
                {

                    cm.EnterGateDirection = gateData.gateDirection1;

                    cm.ExitGateDirection = gateData.gateDirection2;

                    cm.GoNextScene(gateData.sceneName2, gateData.gateId2);//cambiar por una corrutina

                    
                }
                else
                {

                    cm.EnterGateDirection = gateData.gateDirection2;

                    cm.ExitGateDirection = gateData.gateDirection1;

                    cm.GoNextScene(gateData.sceneName1, gateData.gateId1);

                    

                }

            }


            
        }
    }
}