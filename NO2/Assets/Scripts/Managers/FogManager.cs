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
    [SerializeField] private SceneMapData sceneMapData;
    [SerializeField] private TilemapToScriptable tilemapToScriptable;

    [SerializeField] private float fogScale = 1f;
    [SerializeField] private float separation = 1f;



    public FogData FogData
    {
        get { return fogData; } 
        set { fogData = value; }
    }
    public SceneMapData SceneMapData
    {
        get { return sceneMapData; }
        set { sceneMapData = value; }
    }

    public TilemapToScriptable TilemapToScriptable
    {
        get { return tilemapToScriptable; }
        set { tilemapToScriptable = value; }
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
        {
            Debug.Log("Niebla no cargada");
            fogData.Reset();
        }


        float minX = Mathf.Min(topLeft.position.x, bottomLeft.position.x);
        float maxX = Mathf.Max(topRight.position.x, bottomRight.position.x);
        float minY = Mathf.Min(bottomLeft.position.y, bottomRight.position.y);
        float maxY = Mathf.Max(topLeft.position.y, topRight.position.y);

        int cols = Mathf.RoundToInt((maxX - minX) / separation) + 1;
        int rows = Mathf.RoundToInt((maxY - minY) / separation) + 1;

        //// Solo se necesita hacer la primera vez por cada fogData
        fogData.MinX = minX;
        fogData.MinY = minY;
        fogData.Separation = separation;
        if(fogData.Cols == 0)
        {
            fogData.Cols = cols;
            fogData.Rows = rows;
            fogData.Active = new bool[rows, cols];
            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                    fogData.Active[row, col] = true;
        }



        int trues = 0;
        int falses = 0;
        for (int row = 0; row < fogData.Rows; row++)
            for (int col = 0; col < fogData.Cols; col++)
                if (fogData.Active[row, col]) trues++; else falses++;

        Debug.Log($"Nieblas Activas: {trues} Inactivas: {falses}");
        Debug.Log($"MinX: {fogData.MinX} MinY: {fogData.MinY} Separation: {fogData.Separation}");
        int instanciadas = 0;
        for (int row = 0; row < fogData.Rows; row++)
        {
            for (int col = 0; col < fogData.Cols; col++)
            {

                float x = minX + col * fogData.Separation;
                float y = minY + row * fogData.Separation;

                if (!fogData.Active[row, col])
                {
                    // La niebla fue eliminada, revelar sus casillas
                    Vector2Int center = tilemapToScriptable.WorldToGrid(new Vector3(x, y, 0f));
                    float radioMundo = fogScale * fogData.Separation;
                    int radioEnCeldas = Mathf.CeilToInt(radioMundo);

                    if(sceneMapData.IsVisibleMatrix == null)
                    {
                        InitializeVisibilityMatrix();
                    }
                    int mapRows = sceneMapData.IsVisibleMatrix.GetLength(0);
                    int mapCols = sceneMapData.IsVisibleMatrix.GetLength(1);

                    for (int r = -radioEnCeldas; r <= radioEnCeldas; r++)
                    {
                        for (int c = -radioEnCeldas; c <= radioEnCeldas; c++)
                        {
                            int mapRow = center.y + r;
                            int mapCol = center.x + c;

                            if (mapRow < 0 || mapRow >= mapRows || mapCol < 0 || mapCol >= mapCols)
                                continue;

                            Vector3 cellWorld = tilemapToScriptable.GridToWorld(new Vector2Int(mapCol, mapRow));
                            if (Vector2.Distance(new Vector2(x, y), cellWorld) > radioMundo)
                                continue;

                            sceneMapData.FogCoverCount[mapRow, mapCol] =
                                Mathf.Max(0, sceneMapData.FogCoverCount[mapRow, mapCol] - 1);

                            if (sceneMapData.FogCoverCount[mapRow, mapCol] == 0)
                                sceneMapData.IsVisibleMatrix[mapRow, mapCol] = true;
                        }
                    }
                    continue;
                }



                GameObject fog = Instantiate(fogPrefab, new Vector3(x, y, 0f), Quaternion.identity, transform);
                fog.transform.localScale = new Vector3(fogScale, fogScale, 1f);
                instanciadas++; 
            }
        }
        Debug.Log($"Instanciadas: {instanciadas}");
    }
    private void InitializeFromScratch()
    {
        int rows = sceneMapData.MapMatrix.GetLength(0);
        int cols = sceneMapData.MapMatrix.GetLength(1);

        sceneMapData.IsVisibleMatrix = new bool[rows, cols];
        sceneMapData.FogCoverCount = new int[rows, cols];

        // Todas visibles por defecto
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
                sceneMapData.IsVisibleMatrix[row, col] = true;

        float radioMundo = fogScale * fogData.Separation;

        // Marcar no visibles según nieblas activas
        for (int fogRow = 0; fogRow < fogData.Rows; fogRow++)
        {
            for (int fogCol = 0; fogCol < fogData.Cols; fogCol++)
            {
                if (!fogData.Active[fogRow, fogCol]) continue;

                float fogX = fogData.MinX + fogCol * fogData.Separation;
                float fogY = fogData.MinY + fogRow * fogData.Separation;
                Vector3 fogWorldPos = new Vector3(fogX, fogY, 0f);

                Vector2Int center = tilemapToScriptable.WorldToGrid(fogWorldPos);
                int radioEnCeldas = Mathf.CeilToInt(radioMundo);
                int rows2 = sceneMapData.IsVisibleMatrix.GetLength(0);
                int cols2 = sceneMapData.IsVisibleMatrix.GetLength(1);

                for (int r = -radioEnCeldas; r <= radioEnCeldas; r++)
                {
                    for (int c = -radioEnCeldas; c <= radioEnCeldas; c++)
                    {
                        int mapRow = center.y + r;
                        int mapCol = center.x + c;

                        if (mapRow < 0 || mapRow >= rows2 || mapCol < 0 || mapCol >= cols2)
                            continue;

                        Vector3 cellWorld = tilemapToScriptable.GridToWorld(new Vector2Int(mapCol, mapRow));
                        if (Vector2.Distance(fogWorldPos, cellWorld) <= radioMundo)
                        {
                            sceneMapData.FogCoverCount[mapRow, mapCol]++;
                            sceneMapData.IsVisibleMatrix[mapRow, mapCol] = false;
                        }
                    }
                }
            }
        }

        sceneMapData.Save();
    }
    public void InitializeVisibilityMatrix()
    {

        if (!sceneMapData.Load())
        {
            Debug.Log("SceneMapData no cargado, inicializando desde cero");
            InitializeFromScratch();
        }
        //InitializeFromScratch();
    }

    private void Awake()
    {
        GenerateFog();
        
    }
    private void Start()
    {
        InitializeVisibilityMatrix();
    }

    [ContextMenu("Reset")]
    private void ResetFog()
    {
        fogData.Reset();
    }
}