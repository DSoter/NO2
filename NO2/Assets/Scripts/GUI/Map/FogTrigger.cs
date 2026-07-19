using System.Collections;
using UnityEngine;

public class FogTrigger : MonoBehaviour
{
    private FogData fogData;
    [SerializeField] private GameObject fogImage;
    [SerializeField] private GameObject fogCollider;


    private void Awake()
    {
        fogData = GetComponentInParent<FogManager>().FogData;
    }
    public void OnChildTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            Vector2Int gridPos = fogData.WorldToGrid(transform.position);
            Debug.Log("Niebla cruzada");
            fogData.SetActive(gridPos, false);
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
            Vector3 newVector = Vector3.Lerp(initialScale, initialScale/5, t);
            transform.localScale = new Vector3 (newVector.x, newVector.y, 1);
            yield return null;
        }

        Destroy(gameObject);
    }
}
