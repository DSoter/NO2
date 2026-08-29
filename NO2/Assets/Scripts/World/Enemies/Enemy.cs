using UnityEngine;

public abstract class Enemy : MonoBehaviour, IHitable, IEffectable, IVulnerable
{
    [Header("Content")]
    [SerializeField] protected GameObject[] _content;
    [SerializeField] protected float _maxAngleDegrees = 30f;
    [SerializeField] protected float _minForce = 2f;
    [SerializeField] protected float _maxForce = 6f;

    [Header("Health")]
    [SerializeField] protected float _healthPoints = 1;
    [SerializeField] protected float _vulnerableMult = 1.5f;

    [Space(10)]
    [Header("AUDIO")]
    [SerializeField] protected AudioClip _onHitAudioClip;
    [SerializeField][Range(0, 1)] protected float _onHitVolume = 1;
    [SerializeField] protected float _onHitPitchVar = 0.3f;
    [Space(5)]
    [SerializeField] protected AudioClip _onBreachAudioClip;
    [SerializeField][Range(0, 1)] protected float _onBreachVolume = 1;
    [SerializeField] protected float _onBreachPitchVar = 0.3f;

    public virtual bool IsVulnerable { get; set; } = false;

    public virtual void Hit(Vector2 direction, float damage, AttackStrength strength)
    {
        if(_onHitAudioClip != null)
            GameManager.Instance.audioManager.PlaySound(_onHitAudioClip, _onHitVolume, _onHitPitchVar);

        if (IsVulnerable && strength == AttackStrength.Strong)
        {
            Debug.Log("Enemy - Vulnerable Damage Taken: " + damage * _vulnerableMult);
            _healthPoints -= damage * _vulnerableMult;

            if(_onBreachAudioClip != null)
                GameManager.Instance.audioManager.PlaySound(_onBreachAudioClip, _onBreachVolume, _onBreachPitchVar);
        }
        else
        {
            Debug.Log("Enemy - Damage Taken: " + damage);
            _healthPoints -= damage;
        }
    }

    public virtual void ApplyBurn(float damagePerSecond, int duration)
    {
        Debug.Log("BURN - Not Implemented");
    }

    public virtual void ApplySevereBurn(float damagePerSecond, int duration)
    {
        Debug.Log("SEVERE BURN - Not Implemented");
    }

    protected void SpawnContent(Vector2 direction)
    {
        foreach (var item in _content)
        {
            GameObject spawnedItem = Instantiate(item, transform.position, Quaternion.identity);
            Rigidbody2D rb = spawnedItem.GetComponentInChildren<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 force = AddNoise(direction, _maxAngleDegrees, _minForce, _maxForce);
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }

    // Añade un factor aleatorio a la dirección y fuerza con la que salen disparados los pickups
    protected Vector2 AddNoise(Vector2 direction, float maxAngleDegrees, float minForce, float maxForce)
    {
        float noise = Random.Range(-maxAngleDegrees, maxAngleDegrees);
        float rad = noise * Mathf.Deg2Rad;

        Vector2 noisyDirection = new Vector2(
            direction.x * Mathf.Cos(rad) - direction.y * Mathf.Sin(rad),
            direction.x * Mathf.Sin(rad) + direction.y * Mathf.Cos(rad)
        ).normalized;

        return noisyDirection * Random.Range(minForce, maxForce);
    }
}
