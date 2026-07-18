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

    private void Start()
    {
        InitializeMap();
    }

    [ContextMenu("InitializeMap")]
    public void InitializeMap()
    {
        LoadFloorTilemap();
        LoadWallsTilemap();
        AddCampfiresToTileMap();
        AddFlowersToTileMap();
        AddNpcsToTileMap();
    }
        

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        Vector3Int tilemapCell = wallsTileMap.WorldToCell(worldPosition);
        boundsWalls = wallsTileMap.cellBounds;

        int col = tilemapCell.x - boundsWalls.xMin;
        int row = tilemapCell.y - boundsWalls.yMin;

        return new Vector2Int(col, row);
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

    [ContextMenu("Load Floor")]
    public void LoadFloorTilemap()
    {
        
        wallsTileMap = GameObject.FindGameObjectWithTag(wallsTileMapTag).GetComponent<Tilemap>();
        floorTileMap = GameObject.FindGameObjectWithTag(floorTileMapTag).GetComponent<Tilemap>();

        boundsWalls  = wallsTileMap.cellBounds;

        widthWalls = boundsWalls.size.x;
        heightWalls = boundsWalls.size.y;
        Debug.Log($"Floor bounds: xMin={boundsWalls.xMin} yMin={boundsWalls.yMin} width={widthWalls} height={heightWalls}");

        sceneMapData.MapMatrix = new SceneMapData.MapTile[heightWalls, widthWalls];

        for (int row = 0; row < heightWalls; row++)
        {
            for (int col = 0; col < widthWalls; col++)
            {
                int tilemapY = boundsWalls.yMin + row;
                int tilemapX = boundsWalls.xMin + col;
                if(floorTileMap.HasTile(new Vector3Int(tilemapX, tilemapY, 0)))
                {
                    sceneMapData.MapMatrix[row, col] = SceneMapData.MapTile.Floor;
                }                 
            }
        }
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


        for (int row = 0; row < heightWalls; row++)
        {
            for (int col = 0; col < widthWalls; col++)
            {
                int tilemapY = boundsWalls.yMin + row;
                int tilemapX = boundsWalls.xMin + col;
                if (wallsTileMap.HasTile(new Vector3Int(tilemapX, tilemapY, 0)))
                {
                    sceneMapData.MapMatrix[row, col] = SceneMapData.MapTile.Wall;
                }
            }
        }
    }

}
