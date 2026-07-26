using UnityEngine;

public class Nose : MonoBehaviour
{

    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            _animator.SetTrigger("Reveal");
        }
    }
}
