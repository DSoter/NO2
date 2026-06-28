using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapRenderer))]
public class YSortTileMap : MonoBehaviour
{
    private TilemapRenderer _renderer;
    private Transform _transformPlayer;

    private CheckpointManager cm;

    private void Awake()
    {
        _renderer = GetComponent<TilemapRenderer>();
        
    }

    private void Start()
    {
        cm = GameManager.Instance.GetComponent<CheckpointManager>();
    }

    private void LateUpdate()
    {
        if (_transformPlayer == null)
        {
            _renderer.sortingOrder = 0;
            UpdatePlayerReference();
        }
        else 
        {
            _renderer.sortingOrder = Mathf.RoundToInt(-_transformPlayer.position.y * 100);
        }
            
    }

    private void UpdatePlayerReference()
    {
        cm = GameManager.Instance.GetComponent<CheckpointManager>();
        _transformPlayer = cm.PlayerReference;
        if (_transformPlayer != null)
        {
            _renderer.sortingOrder = Mathf.RoundToInt(-_transformPlayer.position.y * 100);
        }

    }
}
