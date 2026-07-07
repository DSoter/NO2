using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PlayerController;

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
    [SerializeField] private GameObject middleCircunference;
    [SerializeField] private GameObject upArrow;
    [SerializeField] private GameObject downArrow;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    [SerializeField] private PlayerData playerData;

    [SerializeField] private float animationSpeed = 1.0f;

    private int slideDirection; //1 si es hacia arriba -1 si es hacia abajo
    private Animator animatorUp;
    private Animator animatorDown;
    private Animator animatorMiddle;

    private bool isAnimating;


    private bool isActive;
    private int idFlor;

    private PlayerController.PlayerState playerState;

    private void Awake()
    {
        
        animatorUp = upCircunference.GetComponent<Animator>();
        animatorDown = downCircunference.GetComponent <Animator>();
        animatorMiddle = middleCircunference.GetComponent<Animator>();

        animatorDown.speed = animationSpeed;
        animatorMiddle.speed = animationSpeed;
        animatorUp.speed = animationSpeed;
        slideDirection = 0;

        animatorUp.SetInteger("State", 1);
        animatorUp.SetTrigger("WithoutTransition");

        animatorDown.SetInteger("State",-1);
        animatorDown.SetTrigger("WithoutTransition");

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
            if (equipedFlower == null)
            {
                equipedFlower = flowers[0];
            }
            UpdateFlowers(equipedFlower);
        }


        CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();

        if (cm != null)
        {
            PlayerController player = cm.PlayerReference.gameObject.GetComponent<PlayerController>();
            playerState = player.GetState();
        }

    }

    private void Update()
    { //alpargata quitar despues de probar
        animatorDown.speed = animationSpeed;
        animatorMiddle.speed = animationSpeed;
        animatorUp.speed = animationSpeed;
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


        if (slideDirection != 0)
        {
            StartCoroutine(WaitOneSec());
            GameObject goUp = upCircunference;
            GameObject goMid = middleCircunference;
            GameObject goDown = downCircunference;

            Animator animUp = upCircunference.GetComponent<Animator>();
            Animator animMid = middleCircunference.GetComponent<Animator>();
            Animator animDown = downCircunference.GetComponent<Animator>();

            Image imgUp = upFlowerImage;
            Image imgMid = currentFlowerImage;
            Image imgDown = downFlowerImage;

            if (slideDirection == -1) // Direccion -1: middle = up, down = middle, up = down
            {
                upCircunference = goDown; animatorUp = animDown; upFlowerImage = imgDown;
                middleCircunference = goUp; animatorMiddle = animUp; currentFlowerImage = imgUp;
                downCircunference = goMid; animatorDown = animMid; downFlowerImage = imgMid;
            }
            else if (slideDirection == 1) // Direccion 1: los círculos middle = down, up = middle, down = up
            {
                upCircunference = goMid; animatorUp = animMid; upFlowerImage = imgMid;
                middleCircunference = goDown; animatorMiddle = animDown; currentFlowerImage = imgDown;
                downCircunference = goUp; animatorDown = animUp; downFlowerImage = imgUp;
            }

            //StartCoroutine(AnimateFlowers(currentFlower));
        }

        float clar = 0;
        Color colAux= Color.white;

        currentFlower = nextCurrentFlower;

        currentFlowerImage.sprite = currentFlower.flowerIcon;
        if (!flowerCollection.unlockedFlowers[currentFlower])//si no está desbloqueada 
        {
            clar = currentFlowerImage.color.a;
            colAux = new Color(0f, 0f, 0f, clar);
            currentFlowerImage.color = colAux;
         
            descriptionText.text = "Informacion sin descubrir";
            flowerName.text = "Flor ?";
        }
        else
        {

            clar = currentFlowerImage.color.a;
            colAux = new Color(1f, 1f, 1f, clar);
            currentFlowerImage.color = colAux;
            descriptionText.text = nextCurrentFlower.description;
            flowerName.text = nextCurrentFlower.objectName;
        }


        upFlower = ObtainNextFlower(currentFlower);
        upFlowerImage.sprite = upFlower.flowerIcon;
        if (!flowerCollection.unlockedFlowers[upFlower])//si no está desbloqueada 
        {

            clar = upFlowerImage.color.a;
            colAux = new Color(0f, 0f, 0f, clar);
            upFlowerImage.color = colAux;
        }
        else
        {
            clar = upFlowerImage.color.a;
            colAux = new Color(1f, 1f, 1f, clar);
            upFlowerImage.color = colAux;
        }


        downFlower = ObtainAnteriorFlower(currentFlower);
        downFlowerImage.sprite = downFlower.flowerIcon;

        if (!flowerCollection.unlockedFlowers[downFlower])//si no está desbloqueada 
        {
            clar = downFlowerImage.color.a;
            colAux = new Color(0f, 0f, 0f, clar);
            downFlowerImage.color = colAux;
        }
        else
        {
            clar = downFlowerImage.color.a;
            colAux = new Color(1f, 1f, 1f, clar);
            downFlowerImage.color = colAux;
        }
        

    }
    private IEnumerator WaitOneSec()
    {
        isAnimating = true;
        float seconds = 1.0f/animationSpeed;
        yield return new WaitForSecondsRealtime(seconds);
        isAnimating= false;
    }

    //private IEnumerator AnimateFlowers(Flower nextCurrentFlower)
    //{
    //    isAnimating = true;
    //    yield return new WaitForSecondsRealtime(0.5f); //medio segundo, 300 frames

    //    currentFlower = nextCurrentFlower;
    //    currentFlowerImage.sprite = currentFlower.flowerIcon;
    //    if (!flowerCollection.unlockedFlowers[currentFlower])//si no está desbloqueada 
    //    {
    //        currentFlowerImage.color = Color.black;
    //        descriptionText.text = "Informacion sin descubrir";
    //        flowerName.text = "Flor ?";
    //    }
    //    else
    //    {
    //        currentFlowerImage.color = Color.white;
    //        descriptionText.text = nextCurrentFlower.description;
    //        flowerName.text = nextCurrentFlower.objectName;
    //    }


    //    upFlower = ObtainNextFlower(currentFlower);
    //    upFlowerImage.sprite = upFlower.flowerIcon;
    //    if (!flowerCollection.unlockedFlowers[upFlower])//si no está desbloqueada 
    //    {
    //        upFlowerImage.color = Color.black;
    //    }
    //    else
    //    {
    //        upFlowerImage.color = Color.white;
    //    }


    //    downFlower = ObtainAnteriorFlower(currentFlower);
    //    downFlowerImage.sprite = downFlower.flowerIcon;

    //    if (!flowerCollection.unlockedFlowers[downFlower])//si no está desbloqueada 
    //    {
    //        downFlowerImage.color = Color.black;
    //    }
    //    else
    //    {
    //        downFlowerImage.color = Color.white;
    //    }

    //    yield return new WaitForSecondsRealtime(0.5f);

    //    isAnimating = false;
    //}

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


    public void GoUp()
    {
        if (isAnimating) { return; }
        if (!isActive) { return; }
        if (flowers == null) { return; }
        //avanzar y que la current sea la flor siguiente

        slideDirection = 1;
        animatorUp.SetInteger("State", slideDirection);
        animatorUp.SetTrigger("HasToChange");

        animatorDown.SetInteger("State", slideDirection);
        animatorDown.SetTrigger("HasToChange");

        animatorMiddle.SetInteger("State", slideDirection);
        animatorMiddle.SetTrigger("HasToChange");

        UpdateFlowers(downFlower);
    }
    public void GoDown()
    {
        if(isAnimating) { return; }
        if (!isActive) { return; }
        if (flowers == null) { return; }

        slideDirection = -1;
        animatorUp.SetInteger("State", slideDirection);
        animatorUp.SetTrigger("HasToChange");

        animatorDown.SetInteger("State", slideDirection);
        animatorDown.SetTrigger("HasToChange");

        animatorMiddle.SetInteger("State", slideDirection);
        animatorMiddle.SetTrigger("HasToChange");

        UpdateFlowers(upFlower);
        //retroceder y que la current sea la flor anterior
    }

    public void ActivateMenu()
    {
        if(playerState != PlayerState.Rest) { return; }
        if (isAnimating) { return; }
        if (isActive) {
            DeactivateMenu();
            return;
        }
        isActive = true;


        
        upCircunference.SetActive(true);
        downCircunference.SetActive(true);
        animatorUp.SetInteger("State", 1);
        animatorUp.SetTrigger("WithoutTransition");

        animatorDown.SetInteger("State", -1);
        animatorDown.SetTrigger("WithoutTransition");
        upArrow.SetActive(true);
        downArrow.SetActive(true);
        //leftArrow.SetActive(true);
        //rightArrow.SetActive(true);
    }
    public void DeactivateMenu()
    {
        if (isAnimating) { return; }
        isActive = false;

        upCircunference.SetActive(false);
        downCircunference.SetActive(false);
        upArrow.SetActive(false);
        downArrow.SetActive(false);
        //leftArrow.SetActive(false);
        //rightArrow.SetActive(false);

        if (flowerCollection.unlockedFlowers[currentFlower])
        {
            equipedFlower = currentFlower;
            playerData.EquipedFlower = equipedFlower;
            slideDirection = 0;
        }
        else
        {
            currentFlower = equipedFlower;
            slideDirection = 0;
            UpdateFlowers(currentFlower);
        }
        
    }

}
