using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;

    private RectTransform _rectTransform;
    private Image _healthMask;

    private float _startingHeight;

    void Start()
    {

        _rectTransform = GetComponent<RectTransform>();
        _healthMask = transform.GetChild(0).gameObject.GetComponent<Image>();

        _startingHeight = _rectTransform.sizeDelta.y;

        UpdateMaxHealth();
    }

    private void OnEnable()
    {
        _playerData.OnHealthChanged += UpdateCurrentHealth;
    }

    private void OnDisable()
    {
        _playerData.OnHealthChanged -= UpdateCurrentHealth;
    }

    public void UpdateMaxHealth()
    {
        _rectTransform.sizeDelta = new Vector2(_playerData.MaxHealth, _startingHeight);
    }

    public void UpdateCurrentHealth()
    {
        _healthMask.fillAmount = _playerData.Health / _playerData.MaxHealth;
    }
}
