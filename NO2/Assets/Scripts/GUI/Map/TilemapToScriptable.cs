using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.DebugUI.Table;

public class TilemapToScriptable : MonoBehaviour
{
    [SerializeField] SceneMapData sceneMapData;
    private Tilemap floorTileMap;
    private string floorTileMapTag = "MainTileMap";
    private Tilemap wallsTileMap;
    private string wallsTileMapTag = "WallsTileMap";

    //Al ser más grande el de muros lo usamos como referencia
    private BoundsInt boundsWalls;
    private int widthWalls;
    private int heightWalls;

    public SceneMapData SceneMapData
    {
        get { return sceneMapData; }
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

        if(wallsTileMap == null)
        {
            wallsTileMap = GameObject.FindGameObjectWithTag(wallsTileMapTag).GetComponent<Tilemap>();
            floorTileMap = GameObject.FindGameObjectWithTag(floorTileMapTag).GetComponent<Tilemap>();
        }

        Vector3Int tilemapCell = wallsTileMap.WorldToCell(worldPosition);
        boundsWalls = wallsTileMap.cellBounds;

        int col = tilemapCell.x - boundsWalls.xMin;
        int row = tilemapCell.y - boundsWalls.yMin;

        return new Vector2Int(col, row);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        if (wallsTileMap == null)
        {
            wallsTileMap = GameObject.FindGameObjectWithTag(wallsTileMapTag).GetComponent<Tilemap>();
            floorTileMap = GameObject.FindGameObjectWithTag(floorTileMapTag).GetComponent<Tilemap>();
        }
        int tilemapX = boundsWalls.xMin + gridPos.x;
        int tilemapY = boundsWalls.yMin + gridPos.y;
        return wallsTileMap.CellToWorld(new Vector3Int(tilemapX, tilemapY, 0));
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
        
        wallsTileMap = GameObject.FindGameObjectWithTag(wallsTileMapTag).GetComponent<Tilemap>();
        floorTileMap = GameObject.FindGameObjectWithTag(floorTileMapTag).GetComponent<Tilemap>();

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
        

        wallsTileMap = GameObject.FindGameObjectWithTag(wallsTileMapTag).GetComponent<Tilemap>();
        floorTileMap = GameObject.FindGameObjectWithTag(floorTileMapTag).GetComponent<Tilemap>();

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
