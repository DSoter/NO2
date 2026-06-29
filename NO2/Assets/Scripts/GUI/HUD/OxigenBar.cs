using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OxigenBar : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private Image _fill;
    [SerializeField] private TextMeshProUGUI _percentageTMP;
    private Animator _percentageAnimator;

    private void Awake()
    {
        _percentageAnimator = _percentageTMP.gameObject.GetComponent<Animator>();
    }
    private void Start()
    {
        UpdateCurrentOxigen();
    }

    private void OnEnable()
    {
         _playerData.OnOxygenChanged += UpdateCurrentOxigen;
    }

    private void OnDisable()
    {
        _playerData.OnOxygenChanged -= UpdateCurrentOxigen;
    }

    private void UpdateCurrentOxigen()
    {
        int percentage = (int)(_playerData.Oxygen / _playerData.MaxOxygen * 100);

        _percentageAnimator.SetFloat("Percentage", percentage);

        _percentageTMP.text = percentage.ToString() + "%";

        _fill.fillAmount = _playerData.Oxygen / _playerData.MaxOxygen;
    }

}
