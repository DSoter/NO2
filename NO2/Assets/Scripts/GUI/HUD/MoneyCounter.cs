using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyCounter : MonoBehaviour
{
    [SerializeField] private MoneyData _moneyData;
    [SerializeField] private TextMeshProUGUI counterText;
    private string displayText;


    private void Start()
    {
        UpdateCurrentMoney();
    }

    private void OnEnable()
    {
        _moneyData.OnMoneyChanged += UpdateCurrentMoney;
    }

    private void OnDisable()
    {
        _moneyData.OnMoneyChanged -= UpdateCurrentMoney;
    }

    private void UpdateCurrentMoney()
    {
        int numberCoins = _moneyData.Money;

        //switch (numberCoins)
        //{
        //    case < 10:
        //        displayText = "0000000" + numberCoins as string;
        //        break;
        //    case < 100:
        //        displayText = "000000" + numberCoins as string;
        //        break;
        //    case < 1000:
        //        displayText = "00000" + numberCoins as string;
        //        break;
        //    case < 10000:
        //        displayText = "0000" + numberCoins as string;
        //        break;
        //    case < 100000:
        //        displayText = "000" + numberCoins as string;
        //        break;
        //    case < 1000000:
        //        displayText = "00" + numberCoins as string;
        //        break;
        //    case < 10000000:
        //        displayText = "0" + numberCoins as string;
        //        break;
        //    case < 100000000:
        //        displayText = "" + numberCoins as string;
        //        break;
        //}
        displayText = "" + numberCoins;
        counterText.text = displayText;
        
    }

}