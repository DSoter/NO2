using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected InteractManager _playerController;
    protected void Start()
    {
        _playerController = FindAnyObjectByType<InteractManager>();
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            if (_playerController == null) 
            {
                _playerController = collision.GetComponent<InteractManager>(); 
            }

            _playerController.interactables.Add(this);

            if (_playerController.interactables.Count == 1)
            {
                _playerController.SetPuedeInteractuar(true);
                _playerController.interactableObject = this;
            }

        }
    }
    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _playerController.interactables.Remove(this);
            if (_playerController.interactables.Count == 0)
            {
                _playerController.SetPuedeInteractuar(false);
                _playerController.interactableObject = null;
            }
            else {
                _playerController.interactableObject = _playerController.interactables[0];
            }
        }
    }
    public abstract void Interact();
}
