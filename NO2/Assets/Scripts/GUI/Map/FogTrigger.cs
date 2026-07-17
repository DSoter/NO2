using System.Collections;
using UnityEngine;

public class FogTrigger : MonoBehaviour
{
    [SerializeField] private GameObject fogImage;
    [SerializeField] private GameObject fogCollider;

    public void OnChildTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {

            Destroy(fogCollider);
            StartCoroutine(ShrinkAndDestroy());

        }
    }

    private IEnumerator ShrinkAndDestroy()
    {
        Vector3 initialScale = transform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            Vector3 newVector = Vector3.Lerp(initialScale, Vector3.zero, t);
            transform.localScale = new Vector3 (newVector.x, newVector.y, 1);
            yield return null;
        }

        Destroy(gameObject);
    }
}
