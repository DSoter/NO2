using System.Collections;
using UnityEngine;

public class AllConditionsDIalog : MonoBehaviour
{
    [SerializeField] FlowerCollection flowerCollection;
    private PlayerData _playerData;
    private PlayerController _playerController;


    private bool talkedToTheoAboutFlowersFirstTime;
    private bool talkedToTheoAboutFlowersSecondTime;


    private bool itemPurchased;
    public bool FlowerIsUnlocked() //Cualquier flor está desbloqueada
    {
        return flowerCollection.unlockedFlowers.ContainsValue(true);
    }
    public bool FlowerIsUnlockedThenDestroy()
    {
        if (flowerCollection.unlockedFlowers.ContainsValue(true))
        {
            StartCoroutine(Bazinga());
            return true;
        }
        return false;
    }

    private IEnumerator Bazinga()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }

    public bool FlowerIsUnlockedThenFinishTheoFirstDialog()
    {
        if (flowerCollection.unlockedFlowers.ContainsValue(true))
        {
            talkedToTheoAboutFlowersFirstTime = true;
            return true;
        }
        return false;
    }
    public bool FirstTheoDialogIsFinished()
    {
        return talkedToTheoAboutFlowersFirstTime;
    }

    public bool FlowerIsUnlockedThenFinishTheoSecondDialog()
    {
        _playerController = FindAnyObjectByType<PlayerController>();
        if (_playerController != null)
        {
            _playerData = _playerController._PlayerData;
            if (_playerData.EquipedFlower != null)
            {

                talkedToTheoAboutFlowersSecondTime = true;
                return true;
            }
            return false;
        }
        Debug.Log("No se ha encontrado el jugador");
        return false;
    }
    public bool SecondTheoDialogIsFinished()
    {
        return talkedToTheoAboutFlowersSecondTime;
    }

    public bool FireFlowerUnlocked()
    {
        if(flowerCollection!= null)
        {
            //alpargata revisar que si se cambia orden de flores esto falla
            if (flowerCollection.unlockedFlowers[flowerCollection.allFlowers[0]])
            {
                return true;
            }
            return false;
        }
        return false;
    }
    public bool BellFlowerUnlocked()
    {
        if (flowerCollection != null)
        {
            //alpargata revisar que si se cambia orden de flores esto falla
            if (flowerCollection.unlockedFlowers[flowerCollection.allFlowers[1]])
            {
                return true;
            }
            return false;
        }
        return false;
    }
    public bool VolatilFlowerUnlocked()
    {
        if (flowerCollection != null)
        {
            //alpargata revisar que si se cambia orden de flores esto falla
            if (flowerCollection.unlockedFlowers[flowerCollection.allFlowers[2]])
            {
                return true;
            }
            return false;
        }
        return false;
    }
    public bool LethalFlowerUnlocked()
    {
        if (flowerCollection != null)
        {
            //alpargata revisar que si se cambia orden de flores esto falla
            if (flowerCollection.unlockedFlowers[flowerCollection.allFlowers[3]])
            {
                return true;
            }
            return false;
        }
        return false;
    }

    public bool ItemPurchased()
    {
        if (itemPurchased)
        {
            itemPurchased = false;
            return true;
        }
        return false;
    }



}
