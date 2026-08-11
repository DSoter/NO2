using TMPro;
using UnityEngine;

public class WeakAttackController : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private PlayerData _playerData;

    [Space(5)]
    [Header("Audio Settings")]
    [SerializeField] private AudioClip _weakAttackSound;
    [SerializeField] [Range(0, 1)] private float _volume = 1f;
    [SerializeField] private float _pitchVar = 0.3f;


    private SpriteRenderer _renderer;
    private Animator _animator;
    private Transform _centerTransform;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _centerTransform = transform.parent.transform;
    }

    // Update is called once per frame
    public void Play(bool shouldFlip)
    {
        // Get the mouse position in world space
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Set the rotation of the attack based on the mouse position 
        var angle = Vector2.SignedAngle(mousePos - transform.position, transform.position + new Vector3(1, 0, 0) - transform.position);
        _centerTransform.rotation = Quaternion.Euler(0, 0, -angle);

        _renderer.flipY = shouldFlip;

        GameManager.Instance.audioManager.PlaySound(_weakAttackSound, _volume, _pitchVar);

        _animator.SetTrigger("Play");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IHitable>(out var hitable))
        {
            var direction = (collision.transform.position - transform.position).normalized;
            hitable.Hit(direction, _playerData.WeakAttackDamage, AttackStrength.Weak);

            FlowerEffectResolver.ApplyOnHit(_playerData.EquipedFlower.flowerEffect, collision.gameObject, _playerData, AttackStrength.Weak);
        }
    }

}
