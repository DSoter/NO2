
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    protected InteractManager _playerController;
    protected string defaultText = "Interactuar";
    protected virtual void Start()
    {
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
                UniqueEnter();
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
                UniqueExit();
            }
            else {
                _playerController.interactableObject = _playerController.interactables[0];
            }
        }
    }
    protected virtual void UniqueEnter()
    {
        _playerController.SetPuedeInteractuar(true);
        _playerController.interactableObject = this;
    }

    protected virtual void UniqueExit()
    {
        _playerController.SetPuedeInteractuar(false);
        _playerController.interactableObject = null;
        _playerController.ChangeDisplayText(defaultText);
    }
    public abstract void Interact();
}
