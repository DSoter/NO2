using UnityEngine;

public class GenericDelegateTrigger : MonoBehaviour
{
    [SerializeField] private FogTrigger _fogTrigger;

    private void OnTriggerEnter2D(Collider2D other)
    {
        _fogTrigger?.OnChildTriggerEnter2D(other);
    }

}
