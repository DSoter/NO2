using System;
using UnityEngine;



public class WeakDestructible : MonoBehaviour, IHitable
{
    [Header("Content")]
    [SerializeField] private GameObject[] _content;
    [SerializeField] private float _maxAngleDegrees = 30f;
    [SerializeField] private float _minForce = 2f;
    [SerializeField] private float _maxForce = 6f;


    [Space(5)]
    [Header("Audio")]
    [SerializeField] private AudioClip _onDestructClip;
    [SerializeField] [Range(0,1)] private float _volume = 1f;
    [SerializeField] private float _pitchVariation = 0.3f;
    

    public void Hit(Vector2 direction, float damage, AttackStrength strength)
    {
        Debug.Log(damage);

        GameManager.Instance.audioManager.PlaySound(_onDestructClip, _volume, _pitchVariation);
        SpawnContent(direction);

        PersistentRespawnObject pro = gameObject.GetComponent<PersistentRespawnObject>();
        if (pro != null)
        {
            gameObject.GetComponent<PersistentRespawnObject>().RegisterDestroy();
        }

        GameManager.Instance.GetComponent<AchievementManager>().NotifyCounter("box_slayer", 1);


        gameObject.SetActive(false);
    }

    private void SpawnContent(Vector2 direction)
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
    private Vector2 AddNoise(Vector2 direction, float maxAngleDegrees, float minForce, float maxForce)
    {
        float noise = UnityEngine.Random.Range(-maxAngleDegrees, maxAngleDegrees);
        float rad = noise * Mathf.Deg2Rad;

        Vector2 noisyDirection = new Vector2(
            direction.x * Mathf.Cos(rad) - direction.y * Mathf.Sin(rad),
            direction.x * Mathf.Sin(rad) + direction.y * Mathf.Cos(rad)
        ).normalized;

        return noisyDirection * UnityEngine.Random.Range(minForce, maxForce);
    }

    
}
