using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IScrollHandler
{
    [SerializeField] private Camera mapCamera;
    [SerializeField] private string mapCameraTag = "MapCamera";
    [SerializeField] private float minFov = 15f;
    [SerializeField] private float maxFov = 100f;
    [SerializeField] private float scrollSensitivity = 5f;

    private bool isHovered = false;

    private float targetFov;

    void Start()
    {
        targetFov = mapCamera.fieldOfView;
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (!isHovered) return;

        float scroll = eventData.scrollDelta.y;
        targetFov = Mathf.Clamp(targetFov - scroll * scrollSensitivity, minFov, maxFov);
    }

    void Update()
    {
        mapCamera.fieldOfView = Mathf.Lerp(mapCamera.fieldOfView, targetFov, Time.unscaledDeltaTime * 10f);
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