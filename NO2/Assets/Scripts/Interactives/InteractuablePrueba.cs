using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InteractuablePrueba : MonoBehaviour
{
    private InteractManager _playerController;
    public void interact()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            if (_playerController == null) { _playerController = collision.GetComponent<InteractManager>();}
            if (!_playerController.GetPuedeInteractuar())
            {
                _playerController.SetPuedeInteractuar(true);
                _playerController.objetoInteractuable = gameObject;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            if (_playerController.GetPuedeInteractuar())
            {
                _playerController.SetPuedeInteractuar(false);
                _playerController.objetoInteractuable = null;
            }
        }
    }
}
