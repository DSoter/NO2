using UnityEngine;

public class DeathTrigguer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().Death();
            
            // Desactivar el trigger para que no se use más de una vez
            //GetComponent<Collider2D>().enabled = false;
        }
    }
}