using UnityEngine;

public class NavDestructible : MonoBehaviour
{
    private NavMeshUpdater _updater;

    private void Start()
    {
        var navMesh = GameObject.FindGameObjectWithTag("NavMesh");

        if (navMesh != null)
        {
            navMesh.TryGetComponent<NavMeshUpdater>(out _updater);
        }
    }

    private void OnDisable()
    {
        if (_updater != null)
        {
            _updater.MarkDirty();
        }
    }
}
