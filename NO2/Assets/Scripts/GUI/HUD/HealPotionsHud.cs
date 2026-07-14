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
        healData.actualCooldownChanged += UpdateMaskFill;
    }
    void OnDestroy()
    {
        healData.healUsesChanged -= UpdateCurrentUses;
        healData.actualCooldownChanged -= UpdateMaskFill;
    }

    private void UpdateCurrentUses()
    {
        textRemainingPotions.text = healData.RemainingUses + "";
    }
   
    private void UpdateMaskFill()
    {
        if (mask != null)
        {
            mask.fillAmount = Mathf.Lerp(1, 0, healData.ActualCooldownSeconds / healData.CooldownSeconds);
        }
        
    }



}
