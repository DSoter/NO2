using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChangeFlowersManager : MonoBehaviour
{
    [SerializeField] private FlowerCollection flowerCollection;
    
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

    [SerializeField] private GameObject upCircunference;
    [SerializeField] private GameObject downCircunference;
    [SerializeField] private GameObject upArrow;
    [SerializeField] private GameObject downArrow;

    [SerializeField] private PlayerData playerData;



    private bool isActive;
    private int idFlor;

    private void Awake()
    {
        flowerCollection.Load();
        isActive = false;
        equipedFlower = playerData.EquipedFlower;
        flowers = flowerCollection.allFlowers;
        if (flowers != null)
        {
            //if (equipedFlower  == null)
            //{
            //    equipedFlower = flowers[0];
            //}
            //if(!flowerCollection.unlockedFlowers.GetValueOrDefault(equipedFlower)){
            //    Debug.Log("Error, la flor equipada no está desbloqueada");
            //}
            //else
            //{
            //    UpdateFlowers(equipedFlower);
            //}
            if (equipedFlower != null)
            {
                equipedFlower = flowers[0];
            }
            UpdateFlowers(equipedFlower);
        }
        
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(240, 20, 80, 20), "Reset"))
        {
           flowerCollection.Reset();
        }

    }
    private void UpdateFlowers(Flower nextCurrentFlower)
    {
        currentFlower = nextCurrentFlower;
        currentFlowerImage.sprite = currentFlower.flowerIcon;
        if (!flowerCollection.unlockedFlowers[currentFlower])//si no está desbloqueada 
        {
            currentFlowerImage.color = Color.gray7;
            descriptionText.text = "Información sin descubrir";
            flowerName.text = "Flor ?";
        }
        else
        {
            currentFlowerImage.color = Color.white;
            descriptionText.text = nextCurrentFlower.description;
            flowerName.text = nextCurrentFlower.objectName;
        }


        upFlower = ObtainNextFlower(currentFlower);
        upFlowerImage.sprite = upFlower.flowerIcon;
        if (!flowerCollection.unlockedFlowers[upFlower])//si no está desbloqueada 
        {
            upFlowerImage.color = Color.gray7;
        }
        else
        {
            upFlowerImage.color = Color.white;
        }


        downFlower = ObtainAnteriorFlower(currentFlower);
        downFlowerImage.sprite = downFlower.flowerIcon;

        if (!flowerCollection.unlockedFlowers[downFlower])//si no está desbloqueada 
        {
            downFlowerImage.color = Color.gray7;
        }
        else
        {
            downFlowerImage.color = Color.white;
        }

        
        
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
        if (!isActive) { return; }
        if (flowers == null) {  return; }
        //avanzar y que la current sea la flor siguiente

        UpdateFlowers(upFlower);
    }
    public void GoUp()
    {
        if (!isActive) { return; }
        if (flowers == null) { return; }

        UpdateFlowers(downFlower);
        //retroceder y que la current sea la flor anterior
    }

    public void ActivateMenu()
    {
        if (isActive) {
            DeactivateMenu();
            return;
        }
        isActive = true;

        upCircunference.SetActive(true);
        downCircunference.SetActive(true);
        upArrow.SetActive(true);
        downArrow.SetActive(true);

    }
    public void DeactivateMenu()
    {
        isActive = false;

        upCircunference.SetActive(false);
        downCircunference.SetActive(false);
        upArrow.SetActive(false);
        downArrow.SetActive(false);

        if (flowerCollection.unlockedFlowers[currentFlower])
        {
            equipedFlower = currentFlower;
            playerData.EquipedFlower = equipedFlower;
        }
        else
        {
            currentFlower = equipedFlower;
            UpdateFlowers(currentFlower);
        }
        
    }

}
