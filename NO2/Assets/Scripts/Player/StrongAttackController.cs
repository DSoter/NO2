using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class StrongAttackController : MonoBehaviour
{
    [Header("PlayerData")]
    [SerializeField] private PlayerData _playerData;

    [Space(5)]
    [Header("Ground Effect")]
    [SerializeField] private GameObject _groundCrack;

    [Space(5)]
    [Header("Audio")]
    [SerializeField] private AudioClip _smashClip;
    [SerializeField] [Range(0,1)] private float _volume;
    [SerializeField] private float _pitchVar;

    private Transform _centerTransform;
    private Collider2D _collider;
    private CinemachineImpulseSource _impulseSource;

    private Vector3 _targetPos;

    public Vector3 TargetPos
    {
        set { _targetPos = value; }
    }

    private float _damage;

    public float Damage
    {
        set { _damage = value; }
    }

    void Start()
    {
        _centerTransform = transform.parent.transform;
        _collider = GetComponent<Collider2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    [ContextMenu("Play")]
    public void Play()
    {
            StartCoroutine(OnPlayCoroutine());
    }

    private IEnumerator OnPlayCoroutine()
    {

        // Set the rotation of the attack based on the mouse position 
        var angle = Vector2.SignedAngle(_targetPos - _centerTransform.position, _centerTransform.position + new Vector3(1, 0, 0) - _centerTransform.position);
        _centerTransform.rotation = Quaternion.Euler(0, 0, -angle);

        transform.rotation = Quaternion.Euler(0, 0, -_centerTransform.rotation.z);

        // Generate camera shake
        _impulseSource.GenerateImpulse();

        Instantiate(_groundCrack, transform.position, Quaternion.identity);

        _collider.enabled = true;
        yield return new WaitForFixedUpdate();

        _collider.enabled = false;
        GameManager.Instance.audioManager.PlaySound(_smashClip,_volume,_pitchVar);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IHitable>(out var hitable))
        {
            if (collision.TryGetComponent<IEffectable>(out var effectable) && _playerData.EquipedFlower != null)
            {
                FlowerEffectResolver.ApplyOnHit(_playerData.EquipedFlower.flowerEffect, collision.gameObject, _playerData, AttackStrength.Strong);
            }

            var direction = (collision.transform.position - transform.position).normalized;
            hitable.Hit(direction, _damage, AttackStrength.Strong);
        }
    }

}
