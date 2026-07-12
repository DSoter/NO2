using TMPro;
using UnityEngine;

public class HealPotionsHud : MonoBehaviour
{
    [SerializeField] HealData healData;

    [SerializeField] TextMeshProUGUI textRemainingPotions;
    [SerializeField] TextMeshProUGUI textMaxPotions;

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
    


}
