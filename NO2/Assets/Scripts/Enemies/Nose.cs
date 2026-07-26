using UnityEngine;

public class Nose : MonoBehaviour, IHitable
{

    private Animator _animator;

    private float _toggleTimer = 0;
    private float _toggleSeconds = 4; // Tiempo entre cambio de estados

    private bool _isVulnerable = false;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        _toggleTimer += Time.deltaTime;

        if(_toggleTimer >= _toggleSeconds)
        {
            _animator.SetTrigger("Toggle");
            _toggleTimer = 0;
        }
    }

    public void Hit(Vector2 direction, float damage, AttackStrength strength)
    {
        if (_isVulnerable && strength == AttackStrength.Strong)
        {
            Destroy(gameObject);
        }
    }

    public void ToggleVulnerability()
    {
        _isVulnerable = !_isVulnerable;
    }
}
