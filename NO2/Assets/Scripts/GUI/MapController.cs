using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IScrollHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Camera mapCamera;
    private string mapCameraTag = "MapCamera";
    [SerializeField] private float minFov = 5f;
    [SerializeField] private float maxFov = 50f;

    [SerializeField] private float scrollSensitivity = 1f;

    [SerializeField] private float panSensitivity = 0.1f;
    [SerializeField] private float inertiaDeceleration = 9f;
    private bool isDragging = false;
    private Vector3 velocity = Vector3.zero;

    private Vector3 initialPosition = Vector3.zero;


    private Tilemap sceneTileMap;
    private string tileMapTag = "MainTileMap";


    private bool isHovered = false;

    private float targetFov;

    private int xMax;
    private int yMax;
    private int xMin;
    private int yMin;

    void Start()
    {
        if (sceneTileMap != null)
        {
            
            sceneTileMap.CompressBounds();
            CalculateBounds();

        }
        if(mapCamera == null)
        {
            return;
        }
        targetFov = mapCamera.orthographicSize;
        velocity = Vector3.zero;
        initialPosition = mapCamera.transform.position;


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
        velocity = new Vector3(
            -delta.x * panSensitivity * fovScale,
            -delta.y * panSensitivity * fovScale,
            0f
        );

        mapCamera.transform.position += velocity;
    }

    void Update()
    {
        if(mapCamera == null)
        {
            return;
        }
        //mapCamera.fieldOfView = Mathf.Lerp(mapCamera.fieldOfView, targetFov, Time.unscaledDeltaTime * 10f);
        mapCamera.orthographicSize = Mathf.Lerp(mapCamera.orthographicSize, targetFov, Time.unscaledDeltaTime * 10f);
        float maxSizeByHeight = (yMax - yMin) / 2f;
        float maxSizeByWidth = (xMax - xMin) / 2f / mapCamera.aspect;

        if (mapCamera.orthographicSize > maxSizeByHeight)
        {
            mapCamera.orthographicSize = Mathf.Lerp(mapCamera.orthographicSize, maxSizeByHeight, Time.unscaledDeltaTime * 10f);
            targetFov = mapCamera.orthographicSize;
        }
        if (mapCamera.orthographicSize > maxSizeByWidth)
        {
            mapCamera.orthographicSize = Mathf.Lerp(mapCamera.orthographicSize, maxSizeByWidth, Time.unscaledDeltaTime * 10f);
            targetFov = mapCamera.orthographicSize;
        }
        //mapCamera.orthographicSize = Mathf.Min(mapCamera.orthographicSize, Mathf.Min(maxSizeByHeight, maxSizeByWidth));

        if (isDragging || velocity == Vector3.zero) {
            return;
        }

        mapCamera.transform.position += velocity;
        velocity = Vector3.Lerp(velocity, Vector3.zero, inertiaDeceleration * Time.unscaledDeltaTime);

        if (velocity.magnitude < 0.001f)
        {
            velocity = Vector3.zero;
        }

        FixCameraPosition();
    }
    private void Awake()
    {
        if(mapCamera == null) 
        {
            mapCamera =FindCamera();
            if(mapCamera == null) { Debug.Log("No se ha encontrado la camara"); }
        }
        if (sceneTileMap == null)
        {
            sceneTileMap = FindTileMap();
            if(sceneTileMap == null) { Debug.Log("No se ha encontrado el tilemap"); }
        }
        
    }
    private void OnDisable()
    {
        ResetPosition();
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
    private Tilemap FindTileMap()
    {
        Scene additiveScene = gameObject.scene; // la escena donde vive este script

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene == additiveScene) continue; // saltamos la aditiva

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                Tilemap tm= root.GetComponentInChildren<Tilemap>();
                if (tm != null && tm.CompareTag(tileMapTag))
                {
                    return tm;
                }
            }
        }
        return null;
    }



    public void ResetPosition()
    {
        if(mapCamera!= null)
        {
            mapCamera.transform.position = initialPosition;
        }
        
    }
    private void CalculateBounds()
    {
        BoundsInt bounds = sceneTileMap.cellBounds;
        xMin = bounds.xMin;
        xMax = bounds.xMax;
        yMin = bounds.yMin;
        yMax = bounds.yMax;
    }
    private void FixCameraPosition()
    {
        if( mapCamera != null && sceneTileMap != null)
        {


            float xAct = mapCamera.transform.position.x;
            float yAct = mapCamera.transform.position.y;
            float xLimInf = xMin + mapCamera.orthographicSize * mapCamera.aspect;
            float xLimSup = xMax - mapCamera.orthographicSize * mapCamera.aspect;
            float yLimInf = yMin + mapCamera.orthographicSize;
            float yLimSup = yMax - mapCamera.orthographicSize;

            xAct = Mathf.Clamp(xAct, xLimInf, xLimSup);
            yAct = Mathf.Clamp(yAct, yLimInf, yLimSup);
            
            xAct = Mathf.Lerp(mapCamera.transform.position.x, xAct, Time.unscaledDeltaTime * 3);
            yAct = Mathf.Lerp(mapCamera.transform.position.y, yAct, Time.unscaledDeltaTime * 3);


            mapCamera.transform.position = new Vector3(xAct, yAct, mapCamera.transform.position.z);
        }
    }
}