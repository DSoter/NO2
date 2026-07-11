using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OxigenBar : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private Image _fill;
    [SerializeField] private TextMeshProUGUI _percentageTMP;
    [SerializeField] private float _updateSpeed = 1f;
    [SerializeField] private float _snapDifference = 0.5f;

    private Animator _percentageAnimator;
    private float _shownPercentage;

    private void Awake()
    {
        _percentageAnimator = _percentageTMP.gameObject.GetComponent<Animator>();
    }
    private void Start()
    {
        _shownPercentage = _playerData.Oxygen / _playerData.MaxOxygen * 100;
        RefreshUI(_shownPercentage);
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
        StopAllCoroutines();
        StartCoroutine(AnimateToTarget(_playerData.Oxygen / _playerData.MaxOxygen * 100));
    }

    private IEnumerator AnimateToTarget(float targetPercentage)
    {
        while (!Mathf.Approximately(_shownPercentage, targetPercentage))
        {
            _shownPercentage = Mathf.Lerp(_shownPercentage, targetPercentage, _updateSpeed * Time.deltaTime);

            // Para que no se quede infinitamente cerca del objetivo, al estar suficientemente cerca forzamos a que se iguale
            if (Mathf.Abs(_shownPercentage - targetPercentage) < _snapDifference)
            {
                _shownPercentage = targetPercentage;
            }

            RefreshUI(_shownPercentage);
            yield return null;
        }
    }

    private void RefreshUI(float percentage)
    {
        _percentageAnimator.SetFloat("Percentage", percentage);
        _percentageTMP.text = (int)percentage + "%";
        _fill.fillAmount = percentage / 100f;
    }
}
