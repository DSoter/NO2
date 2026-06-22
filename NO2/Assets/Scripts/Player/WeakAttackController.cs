using TMPro;
using UnityEngine;

public class WeakAttackController : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private Animator _animator;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void Play(bool shouldFlip)
    {
        // Get the mouse position in world space
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // Set the rotation of the attack based on the mouse position 
        var angle = Vector2.SignedAngle(mousePos - transform.position, transform.position + new Vector3(1, 0, 0) - transform.position);
        transform.rotation = Quaternion.Euler(0, 0, -angle);

        _renderer.flipY = shouldFlip;
        _animator.SetTrigger("Play");
    }
        
}
