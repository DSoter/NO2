using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerData _playerData;

    [Header("Settings")]
    [SerializeField] private float _updateSpeed = 5f;
    [SerializeField] private float _snapDifference = 0.005f;

    private RectTransform _rectTransform;
    private Image _healthMask;

    private float _startingHeight;
    private float _shownFill;

    private void Awake()
    {

        _rectTransform = GetComponent<RectTransform>();
        _healthMask = transform.GetChild(0).gameObject.GetComponent<Image>();

        _startingHeight = _rectTransform.sizeDelta.y;
    }

    void Start()
    {
        UpdateMaxHealth();
        _shownFill = _playerData.Health / _playerData.MaxHealth;
        _healthMask.fillAmount = _shownFill;
    }

    private void OnEnable()
    {
        _playerData.OnHealthChanged += UpdateCurrentHealth;
        _playerData.OnMaxHealthChanged += UpdateMaxHealth;
    }

    private void OnDisable()
    {
        _playerData.OnHealthChanged -= UpdateCurrentHealth;
        _playerData.OnHealthChanged -= UpdateMaxHealth;
    }

    public void UpdateMaxHealth()
    {
        if (_rectTransform!=null)
        {
            _rectTransform.sizeDelta = new Vector2(_playerData.MaxHealth, _startingHeight);
        }
        
    }

    public void UpdateCurrentHealth()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateToTarget(_playerData.Health / _playerData.MaxHealth));
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

            _healthMask.fillAmount = _shownFill;
            yield return null;
        }
    }
}
