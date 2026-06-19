using UnityEngine;

public class TransitionController : MonoBehaviour
{

    private Animator _animator; 

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }


    public void StartTransition(Vector2 direction, bool isOut)
    {
        Debug.Log(Time.realtimeSinceStartup);
        if (isOut)
        {
            _animator.SetFloat("DirX", direction.x);
            _animator.SetFloat("DirY", direction.y);
            _animator.SetTrigger("Out");
        }
        else
        {
            _animator.SetFloat("DirX", direction.x);
            _animator.SetFloat("DirY", direction.y);
            _animator.SetTrigger("In");
        }
    }
}
