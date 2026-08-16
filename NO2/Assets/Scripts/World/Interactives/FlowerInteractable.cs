using System.Collections.Generic;
using UnityEngine;

public class FlowerInteractable : Interactable
{
    [SerializeField] private FlowerCollection flowerCollection;
    [SerializeField] private Flower flowerReference;

    private void Awake()
    {
        flowerCollection.Load();
        if(flowerCollection.unlockedFlowers[flowerReference])//Está desbloqueada
        {
            gameObject.SetActive(false);
        }
    }
    public override void Interact()
    {
        if (gameObject is null) { Debug.Log("No se detecta el game Object"); }
        else
        {
            GameManager.Instance.GetComponent<AchievementManager>().NotifyCounter("unlock_1_flower", 1);
            GameManager.Instance.GetComponent<AchievementManager>().NotifyCounter("unlock_3_flowers", 1);
            flowerCollection.unlockedFlowers[flowerReference] = true;
            flowerCollection.Save();
            Destroy(gameObject);
        }

    }
}
