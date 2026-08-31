using UnityEngine;

public class CameraBoxClamp : MonoBehaviour
{
    [SerializeField] private Camera targetCamera; // la Camera real que dirige Cinemachine (no el vcam)
    [SerializeField] private Transform boxMin;
    [SerializeField] private Transform boxMax;

    private void LateUpdate()
    {
        if (targetCamera == null || boxMin == null || boxMax == null) return;

        Vector3 pos = targetCamera.transform.position;

        pos.x = Mathf.Clamp(pos.x, boxMin.position.x, boxMax.position.x);
        pos.y = Mathf.Clamp(pos.y, boxMin.position.y, boxMax.position.y);

        targetCamera.transform.position = pos;
    }
}