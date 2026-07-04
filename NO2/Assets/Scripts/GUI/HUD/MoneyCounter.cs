using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyCounter : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private TextMeshProUGUI counterText;
    private string displayText;


    private void Start()
    {
        UpdateCurrentMoney();
    }

    private void OnEnable()
    {
        _playerData.OnOxygenChanged += UpdateCurrentMoney;
    }

    private void OnDisable()
    {
        _playerData.OnOxygenChanged -= UpdateCurrentMoney;
    }

    private void UpdateCurrentMoney()
    {
        int numberCoins = (int)(_playerData.Oxygen);
        displayText = "0000" + numberCoins as string;
        counterText.text = displayText;
        
    }

}