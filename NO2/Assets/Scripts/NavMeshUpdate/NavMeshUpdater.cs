using UnityEngine;
using NavMeshPlus.Components;

public class NavMeshUpdater : MonoBehaviour
{
    private NavMeshSurface _surface2D;
    private bool _isDirty = false;

    private void Awake()
    {
        _surface2D = GetComponent<NavMeshSurface>();
    }

    public void MarkDirty() 
    { 
        _isDirty = true;
    }

    private void LateUpdate()
    {
        if (_isDirty)
        {
            _surface2D.UpdateNavMesh(_surface2D.navMeshData);
            _isDirty = false;
        }
    }
}
