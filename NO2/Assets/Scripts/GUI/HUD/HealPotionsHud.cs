using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealPotionsHud : MonoBehaviour
{
    [SerializeField] HealData healData;

    [SerializeField] TextMeshProUGUI textRemainingPotions;
    [SerializeField] TextMeshProUGUI textMaxPotions;
    [SerializeField] Image mask;

    private void Start()
    {
        textRemainingPotions.text = healData.MaxUses + "";
        textRemainingPotions.text = healData.RemainingUses + "";
        healData.healUsesChanged += UpdateCurrentUses;
    }
    void OnDestroy()
    {
        healData.healUsesChanged -= UpdateCurrentUses;
    }

    private void UpdateCurrentUses()
    {
        textRemainingPotions.text = healData.RemainingUses + "";
    }
   
    private void UpdateMaskFill()
    {

    }



}
