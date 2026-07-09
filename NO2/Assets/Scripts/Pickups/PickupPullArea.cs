using System;
using System.Collections;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

public class PickupPullArea : MonoBehaviour
{
    public event Action<Transform> OnPlayerEnteredPullArea;

    [SerializeField] private Transform _coreTransform;

    [SerializeField] private CircleCollider2D _collider;
    [SerializeField] private LayerMask _playerLayer;

    [SerializeField] private float _inactiveSeconds = 1f;

    private void Start()
    {
        StartCoroutine(InitialCoroutine());
    }

    private void Update()
    {
        transform.position = _coreTransform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & _playerLayer) != 0)
        {
            Debug.Log("Player Entered Pull Area");
            OnPlayerEnteredPullArea?.Invoke(collision.GetComponent<PlayerController>().Center);
            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _collider.radius);
    }

    private IEnumerator InitialCoroutine()
    {
        yield return new WaitForSeconds(_inactiveSeconds);
        _collider.enabled = true;
    }
}
