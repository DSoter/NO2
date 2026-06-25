using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;

    private RectTransform _rectTransform;
    private Image _staminaMask;

    private float _startingHeight;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _staminaMask = transform.GetChild(0).gameObject.GetComponent<Image>();
        _startingHeight = _rectTransform.sizeDelta.y;
    }
    void Start()
    {
        UpdateMaxStamina();
    }

    private void OnEnable()
    {
        _playerData.OnStaminaChanged += UpdateCurrentStamina;
    }

    private void OnDisable()
    {
        _playerData.OnStaminaChanged -= UpdateCurrentStamina;
    }

    public void UpdateMaxStamina()
    {
        _rectTransform.sizeDelta = new Vector2(_playerData.MaxStamina, _startingHeight);
    }

    public void UpdateCurrentStamina()
    {
        _staminaMask.fillAmount = _playerData.Stamina / _playerData.MaxStamina;
    }
}
