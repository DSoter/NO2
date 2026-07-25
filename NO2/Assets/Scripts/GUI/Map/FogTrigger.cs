using System.Collections;
using UnityEngine;

public class FogTrigger : MonoBehaviour
{
    private FogData fogData;
    private SceneMapData sceneMapData;
    [SerializeField] private TilemapToScriptable tilemapToScriptable;
    [SerializeField] private GameObject fogImage;
    [SerializeField] private GameObject fogCollider;


    private void Awake()
    {
        fogData = GetComponentInParent<FogManager>().FogData;
        sceneMapData = GetComponentInParent<FogManager>().SceneMapData;
        tilemapToScriptable = GetComponentInParent<FogManager>().TilemapToScriptable;
    }
    public void OnChildTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            Vector2Int gridPos = fogData.WorldToGrid(transform.position);
            Debug.Log("Niebla cruzada");
            fogData.SetActive(gridPos, false);

            RevealMapCells();
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
    private void RevealMapCells()
    {
        if (sceneMapData?.IsVisibleMatrix == null)
        {
            Debug.Log("Matriz de visibilidad nula");
            return;
        }
            

        int rows = sceneMapData.IsVisibleMatrix.GetLength(0);
        int cols = sceneMapData.IsVisibleMatrix.GetLength(1);

        float radioMundo = fogData.Separation;
        int radioEnCeldas = Mathf.CeilToInt(radioMundo);
        Vector2Int center = tilemapToScriptable.WorldToGrid(transform.position);

        int celdasReveladas = 0;

        for (int r = -radioEnCeldas; r <= radioEnCeldas; r++)
        {
            for (int c = -radioEnCeldas; c <= radioEnCeldas; c++)
            {
                int mapRow = center.y + r;
                int mapCol = center.x + c;

                if (mapRow < 0 || mapRow >= rows || mapCol < 0 || mapCol >= cols)
                    continue;

                Vector3 cellWorld = tilemapToScriptable.GridToWorld(new Vector2Int(mapCol, mapRow));
                if (Vector2.Distance(transform.position, cellWorld) > radioMundo)
                    continue;

                int countBefore = sceneMapData.FogCoverCount[mapRow, mapCol];
                sceneMapData.FogCoverCount[mapRow, mapCol] =
                    Mathf.Max(0, sceneMapData.FogCoverCount[mapRow, mapCol] - 1);

               

                if (sceneMapData.FogCoverCount[mapRow, mapCol] == 0)
                {
                    sceneMapData.IsVisibleMatrix[mapRow, mapCol] = true;
                    celdasReveladas++;
                }
                Debug.Log($"Celda [{mapRow},{mapCol}] — FogCount antes: {countBefore} después: {sceneMapData.FogCoverCount[mapRow, mapCol]} visible: {sceneMapData.IsVisibleMatrix[mapRow, mapCol]}");
                Debug.Log($"Celdas reveladas: {celdasReveladas}");
            }
        }
    }
}
