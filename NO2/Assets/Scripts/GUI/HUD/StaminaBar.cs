using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerData _playerData;

    [Space(5)]
    [Header("Settings")]
    [SerializeField] private float _updateSpeed = 5f;
    [SerializeField] private float _snapDifference = 0.005f;

    private RectTransform _rectTransform;
    private Image _staminaMask;

    private float _startingHeight;
    private float _shownFill;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _staminaMask = transform.GetChild(0).gameObject.GetComponent<Image>();
        _startingHeight = _rectTransform.sizeDelta.y;
    }
    void Start()
    {
        UpdateMaxStamina();
        _shownFill = _playerData.Stamina / _playerData.MaxStamina;
        _staminaMask.fillAmount = _shownFill;
    }

    private void OnEnable()
    {
        _playerData.OnStaminaChanged += UpdateCurrentStamina;
        _playerData.OnMaxStaminaChanged += UpdateMaxStamina;
    }

    private void OnDisable()
    {
        _playerData.OnStaminaChanged -= UpdateCurrentStamina;
        _playerData.OnMaxStaminaChanged -= UpdateMaxStamina;
    }

    public void UpdateMaxStamina()
    {
        _rectTransform.sizeDelta = new Vector2(_playerData.MaxStamina, _startingHeight);
    }

    public void UpdateCurrentStamina()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateToTarget(_playerData.Stamina / _playerData.MaxStamina));
    }

    private IEnumerator AnimateToTarget(float targetFill)
    {
        while (!Mathf.Approximately(_shownFill, targetFill))
        {
            _shownFill = Mathf.Lerp(_shownFill, targetFill, _updateSpeed * Time.deltaTime);

            if (Mathf.Abs(_shownFill - targetFill) < 0.001f)
            {
                _shownFill = targetFill;
            }

            _staminaMask.fillAmount = _shownFill;
            yield return null;
        }
    }
}
