using UnityEngine;

public class InteractableUnlockBadge : Interactable
{
    [SerializeField] private Badge badge;
    [SerializeField] private BadgeCollection badgeCollection;
    public override void Interact()
    {
        if (gameObject is null) { Debug.Log("No se detecta el game Object"); }
        else {
            badgeCollection.Unlock(badge);
            Destroy(gameObject); 
        
        }

    }
}
