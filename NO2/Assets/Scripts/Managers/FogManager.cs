using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class FogManager : MonoBehaviour
{
    [SerializeField] private GameObject fogPrefab;
    [SerializeField] private Transform topLeft;
    [SerializeField] private Transform topRight;
    [SerializeField] private Transform bottomLeft;
    [SerializeField] private Transform bottomRight;
    [SerializeField] private FogData fogData;
    [SerializeField] private float fogScale = 1f;
    [SerializeField] private float separation = 1f;



    public FogData FogData
    {
        get { return fogData; } 
        set { fogData = value; }
    }

    [ContextMenu("Generate Fog")]
    public void GenerateFog()
    {
        // Limpiamos todos los hijos menos el primero (las esquinas)
        for (int i = transform.childCount - 1; i >= 1; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        if (!fogData.Load())
            fogData.Reset();

        float minX = Mathf.Min(topLeft.position.x, bottomLeft.position.x);
        float maxX = Mathf.Max(topRight.position.x, bottomRight.position.x);
        float minY = Mathf.Min(bottomLeft.position.y, bottomRight.position.y);
        float maxY = Mathf.Max(topLeft.position.y, topRight.position.y);

        int cols = Mathf.RoundToInt((maxX - minX) / separation) + 1;
        int rows = Mathf.RoundToInt((maxY - minY) / separation) + 1;
        
        //// Solo se necesita hacer la primera vez por cada fogData
        //fogData.MinX = minX;
        //fogData.MinY = minY;
        //fogData.Separation = separation;
        //fogData.Cols = cols;
        //fogData.Rows = rows;
        //fogData.Active = new bool[rows, cols];

        //for (int row = 0; row < rows; row++)
        //    for (int col = 0; col < cols; col++)
        //        fogData.Active[row, col] = true;

        for (int row = 0; row < fogData.Rows; row++)
        {
            for (int col = 0; col < fogData.Cols; col++)
            {
                if (!fogData.Active[row, col]) continue; // no instanciar si está inactivo

                float x = minX + col * fogData.Separation;
                float y = minY + row * fogData.Separation;

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