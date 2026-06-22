using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class RotatingTest : MonoBehaviour
{
    private Animator _animator;
    private SpriteRenderer _renderer;

    private float _attackCooldown = 0.5f;
    private float _attackTimer = 0;
    private int _attackCounter = 0;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {


        if (Input.GetButtonDown("Fire1") && _attackTimer <= 0)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0;

            var angle = Vector2.SignedAngle(mousePos - transform.position, transform.position + new Vector3(1, 0, 0) - transform.position);

            transform.rotation = Quaternion.Euler(0, 0, -angle);

            _renderer.flipY = _attackCounter % 2 == 1;
            _animator.SetTrigger("Play");
            _attackTimer = _attackCooldown;

            _attackCounter += 1;
        }

        _attackTimer -= Time.deltaTime;
    }

}
