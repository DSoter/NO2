using UnityEngine;
using static UnityEngine.Tilemaps.TilemapRenderer;

[RequireComponent(typeof(SpriteRenderer))]
public class YSort : MonoBehaviour
{
    private int sortOrder = 0;
    private void Awake()
    {
        GetComponent<SpriteRenderer>().sortingOrder = sortOrder;
    }
}
