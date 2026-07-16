using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class StrongAttackController : MonoBehaviour
{
    [SerializeField] private GameObject _groundCrack;

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
        var angle = Vector2.SignedAngle(_targetPos - transform.position, transform.position + new Vector3(1, 0, 0) - transform.position);
        _centerTransform.rotation = Quaternion.Euler(0, 0, -angle);

        transform.rotation = Quaternion.Euler(0, 0, -_centerTransform.rotation.z);

        // Generate camera shake
        _impulseSource.GenerateImpulse();

        Instantiate(_groundCrack, transform.position, Quaternion.identity);

        _collider.enabled = true;
        yield return new WaitForFixedUpdate();
        _collider.enabled = false;

        _lock = false;
    }

}
