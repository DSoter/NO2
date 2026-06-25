using UnityEngine;

public class FlowerInteractable : Interactable
{
    [SerializeField] private UnlockedFlowers flowersUnlocked;
    [SerializeField] private Flower flowerReference;

    private void Awake()
    {
        if(flowersUnlocked.unlockedFlowers.IndexOf(flowerReference) != -1)//Está desbloqueada
        {
            gameObject.SetActive(false);
        }
    }
    public override void Interact()
    {
        if (gameObject is null) { Debug.Log("No se detecta el game Object"); }
        else
        { 
            flowersUnlocked.unlockedFlowers.Add(flowerReference);
            Destroy(gameObject);
        }

    }
}
