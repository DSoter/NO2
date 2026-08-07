using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI.Table;

public class TilemapToScriptable : MonoBehaviour
{
    [SerializeField] SceneMapData sceneMapData;
    [SerializeField] FogManager fogManager;
    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorTileMap;
    [SerializeField] private Tilemap wallsTileMap;

    //Al ser más grande el de muros lo usamos como referencia
    private BoundsInt boundsWalls;
    private int widthWalls;
    private int heightWalls;

    private bool boundsReady = false;
    public SceneMapData SceneMapData
    {
        get { return sceneMapData; }
    }
    public FogManager FogManager
    {
        get { return fogManager; }
    }

    private void Awake()
    {
        //InitializeMap();
    }

    [ContextMenu("InitializeMap")]
    public void InitializeMap()
    {
        LoadFloorTilemap();
        LoadWallsTilemap();
        AddCampfiresToTileMap();
        AddFlowersToTileMap();
        AddNpcsToTileMap();
        AddGatesToTileMap();
        AddOxygenZonesToTileMap();


        sceneMapData.SaveMapOnly();
    }

    [ContextMenu("SaveMap")]
    public void SaveMap()
    {
        InitializeMap();
        sceneMapData.Save();
    }
    [ContextMenu("Reset map")]
    public void ResetMap()
    {
        sceneMapData.Reset();
    }
        

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        EnsureBoundsCompressed();

        Vector3Int tilemapCell = wallsTileMap.WorldToCell(worldPosition);
        boundsWalls = wallsTileMap.cellBounds;

        int col = tilemapCell.x - boundsWalls.xMin;
        int row = tilemapCell.y - boundsWalls.yMin;

        return new Vector2Int(col, row);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        EnsureBoundsCompressed();

        int tilemapX = boundsWalls.xMin + gridPos.x;
        int tilemapY = boundsWalls.yMin + gridPos.y;
        return wallsTileMap.CellToWorld(new Vector3Int(tilemapX, tilemapY, 0));
    }



    private void EnsureBoundsCompressed()
    {
        if (boundsReady) return;

        wallsTileMap.CompressBounds();
        floorTileMap.CompressBounds();
        boundsWalls = wallsTileMap.cellBounds;
        widthWalls = boundsWalls.size.x;
        heightWalls = boundsWalls.size.y;


        boundsReady = true;
    }
    public void AddOxygenZonesToTileMap()
    {
        OxygenArea[] zonas = FindObjectsByType<OxygenArea>();

        foreach (OxygenArea zona in zonas)
        {
            PolygonCollider2D col = zona.GetComponent<PolygonCollider2D>();

            if (col == null) continue;

            Bounds worldBounds = col.bounds;

            Vector2Int minCell = WorldToGrid(worldBounds.min);
            Vector2Int maxCell = WorldToGrid(worldBounds.max);

            int rows = sceneMapData.MapMatrix.GetLength(0);
            int cols = sceneMapData.MapMatrix.GetLength(1);

            for (int row = Mathf.Max(0, minCell.y); row <= Mathf.Min(rows - 1, maxCell.y); row++)
            {
                for (int col2 = Mathf.Max(0, minCell.x); col2 <= Mathf.Min(cols - 1, maxCell.x); col2++)
                {
                    SceneMapData.MapTile current = sceneMapData.MapMatrix[row, col2];

                    // cuenta como casilla de terreno aunque se dibuje despues
                    if (current != SceneMapData.MapTile.Floor && current != SceneMapData.MapTile.Wall)
                        continue;

                    Vector3 cellWorldCenter = GridToWorld(new Vector2Int(col2, row));
                    if (col.OverlapPoint(cellWorldCenter))
                    {
                        sceneMapData.MapMatrix[row, col2] = SceneMapData.MapTile.Oxygen;
                    }
                }
            }
        }
    }

    public void AddFlowersToTileMap()
    {
        FlowerInteractable[] flowers = FindObjectsByType<FlowerInteractable>();
        foreach (FlowerInteractable flower in flowers)
        {
            Transform t = flower.transform;
            Vector2Int gridPos = WorldToGrid(t.position);
            sceneMapData.MapMatrix[gridPos.y, gridPos.x] = SceneMapData.MapTile.Flower;
        }
    }
    public void AddNpcsToTileMap()
    {
        CharacterInteractable[] flowers = FindObjectsByType<CharacterInteractable>();
        foreach (CharacterInteractable flower in flowers)
        {
            Transform t = flower.transform;
            Vector2Int gridPos = WorldToGrid(t.position);
            sceneMapData.MapMatrix[gridPos.y, gridPos.x] = SceneMapData.MapTile.Npc;
        }
    }
    public void AddCampfiresToTileMap()
    {
        CheckpointInteractuable[] flowers = FindObjectsByType<CheckpointInteractuable>();
        foreach (CheckpointInteractuable flower in flowers)
        {
            Transform t = flower.transform;
            Vector2Int gridPos = WorldToGrid(t.position);
            sceneMapData.MapMatrix[gridPos.y, gridPos.x] = SceneMapData.MapTile.Campfire;
        }
    }
    public void AddGatesToTileMap()
    {
        NextSceneTrigguer[] flowers = FindObjectsByType<NextSceneTrigguer>();
        foreach (NextSceneTrigguer flower in flowers)
        {
            Transform t = flower.transform;
            Vector2Int gridPos = WorldToGrid(t.position);
            sceneMapData.MapMatrix[gridPos.y, gridPos.x] = SceneMapData.MapTile.Gate;
        }
    }

    [ContextMenu("Load Floor")]
    public void LoadFloorTilemap()
    {
        
        wallsTileMap.CompressBounds(); 
        floorTileMap.CompressBounds();

        boundsWalls  = wallsTileMap.cellBounds;

        widthWalls = boundsWalls.size.x;
        heightWalls = boundsWalls.size.y;
        Debug.Log($"Floor bounds: xMin={boundsWalls.xMin} yMin={boundsWalls.yMin} width={widthWalls} height={heightWalls}");

        sceneMapData.Load();
        sceneMapData.MapMatrix = new SceneMapData.MapTile[heightWalls, widthWalls];
        int numberOfFloors = 0;
        for (int row = 0; row < heightWalls; row++)
        {
            for (int col = 0; col < widthWalls; col++)
            {
                int tilemapY = boundsWalls.yMin + row;
                int tilemapX = boundsWalls.xMin + col;
                if(floorTileMap.HasTile(new Vector3Int(tilemapX, tilemapY, 0)))
                {
                    sceneMapData.MapMatrix[row, col] = SceneMapData.MapTile.Floor;
                    numberOfFloors++;
                }                 
            }
        }
        Debug.Log($"Number of floors {numberOfFloors}");
    }
    [ContextMenu("Load Walls")]
    public void LoadWallsTilemap()
    {
        

        wallsTileMap.CompressBounds();
        floorTileMap.CompressBounds();

        boundsWalls = wallsTileMap.cellBounds;

        widthWalls = boundsWalls.size.x;
        heightWalls = boundsWalls.size.y;
        Debug.Log($"Walls bounds: xMin={boundsWalls.xMin} yMin={boundsWalls.yMin} width={widthWalls} height={heightWalls}");

        int numberOfWalls = 0;

        for (int row = 0; row < heightWalls; row++)
        {
            for (int col = 0; col < widthWalls; col++)
            {
                int tilemapY = boundsWalls.yMin + row;
                int tilemapX = boundsWalls.xMin + col;
                if (wallsTileMap.HasTile(new Vector3Int(tilemapX, tilemapY, 0)))
                {
                    sceneMapData.MapMatrix[row, col] = SceneMapData.MapTile.Wall;
                    numberOfWalls++;
                }
            }
        }
        Debug.Log($"Number of Walls {numberOfWalls}");
    }

}
