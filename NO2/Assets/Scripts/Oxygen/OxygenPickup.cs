using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class OxygenPickup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private PickupPullArea _pickupPullArea;

    [Header("Audio")]
    [SerializeField] private AudioClip _onPickClip;

    [Header("Layer Mask")]
    [SerializeField] private LayerMask _playerLayer;

    [Header("Data")]
    [SerializeField] private float _iniPullSpeed = 0;
    [SerializeField] private float _pullAcceleration = 1f;
    [SerializeField] private float _regenAmount = 5f;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _playerLayer) != 0)
        {
            _playerData.Oxygen += _regenAmount;
            GameManager.Instance.audioManager.PlaySound(_onPickClip);
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
