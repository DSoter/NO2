using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IScrollHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Camera mapCamera;
    [SerializeField] private string mapCameraTag = "MapCamera";
    [SerializeField] private float minFov = 5f;
    [SerializeField] private float maxFov = 50f;

    [SerializeField] private float scrollSensitivity = 1f;

    [SerializeField] private float panSensitivity = 0.1f;
    private bool isDragging = false;


    private bool isHovered = false;

    private float targetFov;

    void Start()
    {
        targetFov = mapCamera.orthographicSize;
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (!isHovered) return;

        float scroll = eventData.scrollDelta.y;
        targetFov = Mathf.Clamp(targetFov - scroll * scrollSensitivity, minFov, maxFov);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || mapCamera == null) return;

        Vector2 delta = eventData.delta;
        float fovScale = mapCamera.orthographicSize / maxFov;
        Vector3 move = new Vector3(
            -delta.x * panSensitivity * fovScale,
            -delta.y * panSensitivity * fovScale,
            0f
        );

        mapCamera.transform.position += move;
    }

    void Update()
    {
        //mapCamera.fieldOfView = Mathf.Lerp(mapCamera.fieldOfView, targetFov, Time.unscaledDeltaTime * 10f);
        mapCamera.orthographicSize = Mathf.Lerp(mapCamera.orthographicSize, targetFov, Time.unscaledDeltaTime * 10f);
        
    }
    private void Awake()
    {
        if(mapCamera == null) 
        {
            mapCamera =FindCamera();
            if(mapCamera == null) { Debug.Log("No se ha encontrado la camara"); }
        }
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Boton clicado");
        Debug.Log($"Hovering {isHovered}");
        if (eventData.button == PointerEventData.InputButton.Left && isHovered)
        {
            isDragging = true;
            Debug.Log($"Dragging {isDragging}");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = false;
        }
    }


    private Camera FindCamera()
    {
        Scene additiveScene = gameObject.scene; // la escena donde vive este script

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene == additiveScene) continue; // saltamos la aditiva

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Camera cam = root.GetComponentInChildren<Camera>();
                if (cam != null && cam.CompareTag(mapCameraTag))
                {
                    return cam;
                }
            }
        }
        return null;
    }
}