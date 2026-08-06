using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapRenderer))]
public class YSortTileMap : MonoBehaviour
{
    private int sortOrder = 0; 

    private void Awake()
    {
        GetComponent<TilemapRenderer>().sortingOrder = sortOrder;
    }
}
