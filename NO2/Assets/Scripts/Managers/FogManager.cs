using UnityEngine;

public class FogManager : MonoBehaviour
{
    [SerializeField] private GameObject fogPrefab;
    [SerializeField] private Transform topLeft;
    [SerializeField] private Transform topRight;
    [SerializeField] private Transform bottomLeft;
    [SerializeField] private Transform bottomRight;
    [SerializeField] private float fogScale = 1f;
    [SerializeField] private float separation = 1f;

    [ContextMenu("Generate Fog")]
    public void GenerateFog()
    {
        // Limpiamos todos los hijos menos el primero (las esquinas)
        for (int i = transform.childCount - 1; i >= 1; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        float minX = Mathf.Min(topLeft.position.x, bottomLeft.position.x);
        float maxX = Mathf.Max(topRight.position.x, bottomRight.position.x);
        float minY = Mathf.Min(bottomLeft.position.y, bottomRight.position.y);
        float maxY = Mathf.Max(topLeft.position.y, topRight.position.y);

        for (float x = minX; x <= maxX; x += separation)
        {
            for (float y = minY; y <= maxY; y += separation)
            {
                GameObject fog = Instantiate(fogPrefab, new Vector3(x, y, 0f), Quaternion.identity, transform);
                fog.transform.localScale = new Vector3(fogScale, fogScale, 1f);
            }
        }
    }

    private void Awake()
    {
        GenerateFog();
    }
}