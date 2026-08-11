using System.Collections;
using UnityEngine;

public class SpriteFlash : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private float _flashDuration = 0.15f;
    [SerializeField] private float _fadeOutDuration = 0.2f;

    private SpriteRenderer _renderer;
    private MaterialPropertyBlock _propertyBlock;
    private Coroutine _flashCoroutine;

    private const string PROPIEDAD_FLASH = "_FlashAmount";

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    public void ApplyEffect()
    {
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
        }

        _flashCoroutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        SetFlash(1f);

        yield return new WaitForSeconds(_flashDuration);

        float elapsed = 0f;
        float startValue = 1f;

        while (elapsed < _fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _fadeOutDuration;
            SetFlash(Mathf.Lerp(startValue, 0f, t));
            yield return null;
        }

        SetFlash(0f);

        _flashCoroutine = null;
    }

    private void SetFlash(float valor)
    {
        // Obtiene el estado actual del SpriteRenderer para no pisar otros datos
        _renderer.GetPropertyBlock(_propertyBlock);

        // Modifica únicamente el valor de FlashAmount para este objeto
        _propertyBlock.SetFloat(PROPIEDAD_FLASH, valor);

        // Aplica el cambio de vuelta al SpriteRenderer
        _renderer.SetPropertyBlock(_propertyBlock);
    }
}
