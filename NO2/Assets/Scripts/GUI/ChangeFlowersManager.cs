using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChangeFlowersManager : MonoBehaviour
{
    [SerializeField] private UnlockedFlowers unlockedFlowers;
    
    private List<Flower> flowers;
    private Flower equipedFlower;
    private Flower currentFlower;
    [SerializeField] private Image currentFlowerImage;

    private Flower upFlower;
    [SerializeField] private Image upFlowerImage;


    private Flower downFlower;
    [SerializeField] private Image downFlowerImage;

    [SerializeField] private TextMeshProUGUI flowerName;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private int idFlor;

    private void Awake()
    {
        Debug.Log("Menu flowers awaken");
        flowers = unlockedFlowers.unlockedFlowers;
        if (flowers != null)
        {
            equipedFlower = flowers[0];
            UpdateFlowers(equipedFlower);
        }
        
    }


    private void UpdateFlowers(Flower nextCurrentFlower)
    {
        currentFlower = nextCurrentFlower;
        currentFlowerImage.sprite = currentFlower.flowerIcon;

        upFlower = ObtainNextFlower(currentFlower);
        upFlowerImage.sprite = upFlower.flowerIcon;

        downFlower = ObtainAnteriorFlower(currentFlower);
        downFlowerImage.sprite = downFlower.flowerIcon;

        descriptionText.text = nextCurrentFlower.description;
        flowerName.text = nextCurrentFlower.objectName;
    }

    private Flower ObtainNextFlower(Flower flower)
    {
        if (flowers.Count == 1) { return flower; }
        idFlor = flowers.IndexOf(flower);
        if (idFlor + 1 == flowers.Count)//la ultima flor
        {
            return flowers[0];
        }
        else
        {
            return flowers[idFlor + 1];
        }
    }
    private Flower ObtainAnteriorFlower(Flower flower)
    {
        if (flowers.Count == 1) { return flower; }
        idFlor = flowers.IndexOf(flower);
        if (idFlor == 0)//la primera flor
        {
            return flowers[flowers.Count - 1];
        }
        else
        {
            return flowers[idFlor - 1];
        }
    }
    public void GoDown()
    {

        if (flowers == null) {  return; }
        //avanzar y que la current sea la flor siguiente

        UpdateFlowers(upFlower);
    }
    public void GoUp()
    {

        if (flowers == null) { return; }

        UpdateFlowers(downFlower);
        //retroceder y que la current sea la flor anterior
    }

}
