using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private PlayerData _playerData;

    private RectTransform _rectTransform;
    private Image _staminaMask;

    private float _startingHeight;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _staminaMask = transform.GetChild(0).gameObject.GetComponent<Image>();

        _startingHeight = _rectTransform.sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMaxStamina(_playerData.MaxStamina);
        UpdateCurrentStamina();
    }

    public void UpdateMaxStamina(float amount)
    {
        _rectTransform.sizeDelta = new Vector2(amount, _startingHeight);
    }

    public void UpdateCurrentStamina()
    {
        _staminaMask.fillAmount = _playerData.Stamina / _playerData.MaxStamina;
    }
}
