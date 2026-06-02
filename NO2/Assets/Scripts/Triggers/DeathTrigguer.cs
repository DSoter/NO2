using UnityEngine;

public class DeathTrigguer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Buscamos el SceneController y le pasamos nuestra posición
            SceneController sc = FindAnyObjectByType<SceneController>();
            if (sc != null)
            {
                sc.KillPlayer();

                //esto es pa reproducir sonido
                //sc.ReproducirCheckPoint(); 
                Debug.Log("Muerte alcanzada " + gameObject.name);
            }

            // Desactivar el trigger para que no se use más de una vez
            //GetComponent<Collider2D>().enabled = false;
        }
    }
}