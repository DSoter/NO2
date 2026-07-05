using UnityEngine;

using System.Collections;

public class Pickup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PickupPullArea _pickupPullArea;

    [Space(5)]
    [Header("Audio")]
    [SerializeField] private AudioClip _onPickClip;
    [SerializeField] private float _volume = 1f;
    [SerializeField] private float _pitchVariation = 0.3f;

    [Space(5)]
    [Header("Layer Mask")]
    [SerializeField] private LayerMask _playerLayer;

    [Space(5)]
    [Header("Data")]
    [SerializeField] private float _iniPullSpeed = 0;
    [SerializeField] private float _pullAcceleration = 1f;

    [Space(5)]
    [Header("Pickup Effect")]
    [SerializeField] private PickupEffect _effect;

    protected Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _playerLayer) != 0)
        {
            GameManager.Instance.audioManager.PlaySound(_onPickClip, _volume, _pitchVariation);

            if (_effect != null)
            {
                _effect.Apply();

            }

            Destroy(transform.parent.gameObject);
        }
    }

    private void HandlePlayerEnteredPullArea(Transform player)
    {
        StartCoroutine(PullTowardsPlayer(player));
    }

    private IEnumerator PullTowardsPlayer(Transform player)
    {
        while (true)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            _rigidbody.linearVelocity = direction * _iniPullSpeed;
            _iniPullSpeed += _pullAcceleration * Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnEnable()
    {
        _pickupPullArea.OnPlayerEnteredPullArea += HandlePlayerEnteredPullArea;
    }

    private void OnDisable()
    {
        _pickupPullArea.OnPlayerEnteredPullArea -= HandlePlayerEnteredPullArea;
    }
}

