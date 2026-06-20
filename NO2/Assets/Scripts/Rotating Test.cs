using UnityEngine;
using UnityEngine.UIElements;

public class RotatingTest : MonoBehaviour
{
    private Transform _center;
    private Animator _animator;

    private float _attackCooldown = 0.5f;
    private float _attackTimer = 0;

    private void Start()
    {
        _center = transform.parent.transform;
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        var angle = Vector2.SignedAngle(mousePos - transform.position, transform.position + new Vector3(1, 0, 0) - transform.position);

        Debug.Log(angle);

        transform.rotation = Quaternion.Euler(0, 0, -angle);

        if (Input.GetButtonDown("Fire1") && _attackTimer <= 0)
        {
            _animator.SetTrigger("Play");
            _attackTimer = _attackCooldown;
        }

        _attackTimer -= Time.deltaTime;
    }

}
