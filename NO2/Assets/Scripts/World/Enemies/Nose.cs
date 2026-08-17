using UnityEngine;

public class Nose : Enemy
{

    private Animator _animator;

    private float _toggleTimer = 0;
    private float _toggleSeconds = 4; // Tiempo entre cambio de estados

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

    public override void Hit(Vector2 direction, float damage, AttackStrength strength)
    {
        if (IsVulnerable && strength == AttackStrength.Strong)
        {
            _animator.SetTrigger("Death");
            SpawnContent(direction);
        }
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
    }

    public void ToggleVulnerability()
    {
        IsVulnerable = !IsVulnerable;
    }
}
