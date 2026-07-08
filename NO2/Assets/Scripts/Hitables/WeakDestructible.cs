using UnityEngine;

public class WeakDestructible : MonoBehaviour, IHitable
{
    [SerializeField] private GameObject[] _content;

    public void Hit(Vector2 direction, float damage, AttackStrength strength)
    {
        Debug.Log("Hit Landed");

        gameObject.SetActive(false);
    }
}
