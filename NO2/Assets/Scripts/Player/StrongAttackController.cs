using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class StrongAttackController : MonoBehaviour
{
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

    private bool _lock = false;

    private Vector3 _targetPos;

    public Vector3 TargetPos
    {
        set { _targetPos = value; }
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
        if (!_lock)
        {
            StartCoroutine(OnPlayCoroutine());
        }
    }

    private IEnumerator OnPlayCoroutine()
    {
        _lock = true;

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

        _lock = false;
    }

}
