using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class YSort : MonoBehaviour
{
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        _renderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    [ContextMenu("Sort In Editor")]
    private void SortInEditor()
    {
        var renderer = GetComponent<SpriteRenderer>();
        renderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}
