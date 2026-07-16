using System.Collections;
using System.Threading;
using UnityEngine;

public class CleanWithTime : MonoBehaviour
{
    [SerializeField] private float _timeBeforeClean = 0.5f;
    [SerializeField] private float _timeToClean = 1f;

    private SpriteRenderer _renderer;

    private Color _color;
    private float _initialAlpha;
    private float _timer = 0f;


    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _color = _renderer.color;
        _initialAlpha = _color.a;

        StartCoroutine(CleanCoroutine());
    }

    private IEnumerator CleanCoroutine()
    {
        yield return new WaitForSeconds(_timeToClean);

        while(_timer < _timeToClean)
        {
            _color.a = Mathf.Lerp(_initialAlpha, 0f, _timer / _timeToClean);
            _renderer.color = _color;
            _timer += Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }

}
