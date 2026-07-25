using System.IO;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SceneMapData;

public class MapControllerV2 : MonoBehaviour, IScrollHandler, IPointerDownHandler,
                           IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Referencias")]
    [SerializeField] private SceneMapData mapData;
    [SerializeField] private RawImage mapImage;      // capa del mapa
    [SerializeField] private RawImage fogImage;      // capa de niebla
    [SerializeField] private RectTransform mapContainer; // el rect que se mueve y escala

    [Header("Iconos")]
    [SerializeField] private Sprite npcSprite;
    [SerializeField] private Sprite flowerSprite;
    [SerializeField] private Sprite campfireSprite;
    [SerializeField] private Sprite gateSprite;
    [SerializeField] private int iconSize = 16;
    [SerializeField] private int iconTileSize = 3; 

    [Header("Colores")]
    [SerializeField] private Color floorColor = Color.gray;
    [SerializeField] private Color wallColor = Color.white;
    [SerializeField] private Color emptyColor = Color.clear;

    [Header("Zoom y movimiento")]
    [SerializeField] private float scrollSensitivity = 0.1f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 5f;
    [SerializeField] private float panSensitivity = 1f;
    [SerializeField] private float inertiaDeceleration = 5f;
    [SerializeField] private int pixelsPerTile = 16;

    [Header("Jugador")]
    [SerializeField] private Image playerIcon;
    [SerializeField] private Sprite playerSprite;
    [SerializeField] private TilemapToScriptable tilemapToScriptable;

    private Texture2D _mapTexture;
    private Texture2D _fogTexture;
    private float _targetScale = 1f;
    private Vector3 _velocity;
    private bool _isDragging;
    private bool _isHovered;
    private string sceneControllerTag = "SceneController";

    private void Start()
    {
        GenerateMapTexture();
        
        _targetScale = mapContainer.localScale.x;

        UpdatePlayerIcon();
    }
    private void OnEnable()
    {
        GenerateFogTexture();
    }

    public void UpdatePlayerIcon()
    {

        if (playerIcon == null || playerSprite == null) return;

        CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();
        Transform playerTransform = cm.PlayerReference;
        TilemapToScriptable tilemapToScriptable = cm.TilemapToScriptable;

        if (playerTransform == null || tilemapToScriptable == null) return;


        playerIcon.sprite = playerSprite;
        playerIcon.gameObject.SetActive(true);

        // Convertir posición mundo a celda del mapa
        Vector2Int gridPos = tilemapToScriptable.WorldToGrid(playerTransform.position);

        int rows = mapData.MapMatrix.GetLength(0);
        int cols = mapData.MapMatrix.GetLength(1);

        int texWidth = cols * pixelsPerTile;
        int texHeight = rows * pixelsPerTile;

        // Posición en píxeles dentro de la textura
        float pixelX = gridPos.x * pixelsPerTile + pixelsPerTile / 2f;
        float pixelY = gridPos.y * pixelsPerTile + pixelsPerTile / 2f;

        // Convertir a posición normalizada (0-1)
        float normX = pixelX / texWidth;
        float normY = pixelY / texHeight;

        // Convertir a posición local dentro del mapContainer
        RectTransform mapRect = mapImage.rectTransform;
        float localX = (normX - 0.5f) * mapRect.rect.width;
        float localY = (normY - 0.5f) * mapRect.rect.height;

        playerIcon.rectTransform.localPosition = new Vector3(localX, localY, 0f);
    }
    public void GenerateMapTexture()
    {
        UpdateMapData();

        //Se guarda una textura por escena
        string path = GetMapTexturePath();

        if (System.IO.File.Exists(path))
        {
            // Cargar desde disco
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            _mapTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            _mapTexture.filterMode = FilterMode.Point;
            _mapTexture.LoadImage(bytes);
            mapImage.texture = _mapTexture;
            return;
        }


        MapTile[,] matrix = mapData.MapMatrix;
        if (matrix == null) return;


        


        Debug.Log($"Rows: {matrix.GetLength(0)} Cols: {matrix.GetLength(1)}");
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        int texWidth = cols * pixelsPerTile;
        int texHeight = rows * pixelsPerTile;

        _mapTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
        _mapTexture.filterMode = FilterMode.Point;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Color color = GetTileColor(matrix[row, col]);
                FillTile(_mapTexture, col, row, color);
            }
        }

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Sprite icon = GetTileIcon(matrix[row, col]);
                if (icon != null)
                    DrawIcon(_mapTexture, col, row, icon);
            }
        }

        _mapTexture.Apply();
        mapImage.texture = _mapTexture;

        System.IO.File.WriteAllBytes(path, _mapTexture.EncodeToPNG());
        Debug.Log($"Textura guardada en {path}");
    }
    private string GetMapTexturePath()
    {
        return System.IO.Path.Combine(
            Application.persistentDataPath,
            mapData.name + "_mapTexture.png"
        );
    }
    [ContextMenu("Clear Map Texture Cache")]
    public void ClearMapTextureCache()
    {
        string path = GetMapTexturePath();
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            Debug.Log("Caché de textura eliminada");
        }
    }

    public void UpdateFogTexture()
    {
        bool[,] visible = mapData.IsVisibleMatrix;
        if (visible == null || _fogTexture == null) return;

        int rows = visible.GetLength(0);
        int cols = visible.GetLength(1);


        int trues = 0, falses = 0;
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
                if (visible[row, col]) trues++; else falses++;

        Debug.Log($"UpdateFogTexture — Visibles: {trues} No visibles: {falses}");

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Color fogColor = visible[row, col] ? Color.clear : Color.black;
                FillTile(_fogTexture, col, row, fogColor);
            }
        }

        _fogTexture.Apply();
    }
    private void UpdateMapData()
    {

        Scene additiveScene = gameObject.scene; // la escena donde vive este script

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene == additiveScene) continue; // saltamos la aditiva

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                TilemapToScriptable tts = root.GetComponentInChildren<TilemapToScriptable>();
                if (tts != null && tts.CompareTag(sceneControllerTag))
                {
                    //Debug.Log("Se va a actualizar el mapa");
                    tts.SaveMap();
                    mapData = tts.SceneMapData;
                }
            }
        }

}
    private void GenerateFogTexture()
    {
        mapData.Save();
        if(mapData == null)
        {
            Debug.LogError("mapData es null");
            return;
        }
        bool[,] visible = mapData.IsVisibleMatrix;
        if (visible == null)
        {
            Debug.LogError("IsVisibleMatrix es null — FogManager no ha inicializado las matrices todavía");
            return;
        }
        Debug.Log($"Rows: {visible.GetLength(0)} Cols: {visible.GetLength(1)}");
        int rows = visible.GetLength(0);
        int cols = visible.GetLength(1);
        int texWidth = cols * pixelsPerTile;
        int texHeight = rows * pixelsPerTile;

        _fogTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
        _fogTexture.filterMode = FilterMode.Point;

        UpdateFogTexture();
        fogImage.texture = _fogTexture;
    }

    private void FillTile(Texture2D tex, int col, int row, Color color)
    {
        int startX = col * pixelsPerTile;
        int startY = row * pixelsPerTile;
        for (int px = startX; px < startX + pixelsPerTile; px++)
        {
            for (int py = startY; py < startY + pixelsPerTile; py++)
            {
                tex.SetPixel(px, py, color);
            }
        }
    }

    private void DrawIcon(Texture2D tex, int col, int row, Sprite sprite)
    {
        Texture2D iconTex = sprite.texture;
        Rect spriteRect = sprite.textureRect;

        int totalPixels = iconTileSize * pixelsPerTile;
        int startX = col * pixelsPerTile + pixelsPerTile / 2 - totalPixels / 2;
        int startY = row * pixelsPerTile + pixelsPerTile / 2 - totalPixels / 2;

        for (int px = 0; px < totalPixels; px++)
        {
            for (int py = 0; py < totalPixels; py++)
            {
                int texX = startX + px;
                int texY = startY + py;

                // Ignorar pixels fuera de la textura
                if (texX < 0 || texX >= tex.width || texY < 0 || texY >= tex.height)
                    continue;

                float u = spriteRect.x / iconTex.width + (float)px / totalPixels * (spriteRect.width / iconTex.width);
                float v = spriteRect.y / iconTex.height + (float)py / totalPixels * (spriteRect.height / iconTex.height);

                Color c = iconTex.GetPixelBilinear(u, v);
                if (c.a > 0.1f)
                    tex.SetPixel(texX, texY, c);
            }
        }
    }

    private Color GetTileColor(SceneMapData.MapTile tile)
    {
        switch (tile)
        {
            case SceneMapData.MapTile.Floor: return floorColor;
            case SceneMapData.MapTile.Wall: return wallColor;
            default: return floorColor;
        }
    }

    private Sprite GetTileIcon(SceneMapData.MapTile tile)
    {
        switch (tile)
        {
            case SceneMapData.MapTile.Npc: return npcSprite;
            case SceneMapData.MapTile.Flower: return flowerSprite;
            case SceneMapData.MapTile.Campfire: return campfireSprite;
            case SceneMapData.MapTile.Gate: return gateSprite;

            default: return null;
        }
    }

    // Zoom y movimiento
    private void Update()
    {
        float currentScale = mapContainer.localScale.x;
        float newScale = Mathf.Lerp(currentScale, _targetScale, Time.unscaledDeltaTime * 10f);
        mapContainer.localScale = new Vector3(newScale, newScale, 1f);

        if (!_isDragging && _velocity != Vector3.zero)
        {
            mapContainer.localPosition += _velocity;
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, inertiaDeceleration * Time.unscaledDeltaTime);
            if (_velocity.magnitude < 0.001f) _velocity = Vector3.zero;
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (!_isHovered) return;
        float scroll = eventData.scrollDelta.y;
        _targetScale = Mathf.Clamp(_targetScale + scroll * scrollSensitivity, minScale, maxScale);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging) return;
        float scaleRatio = mapContainer.localScale.x / maxScale;
        _velocity = new Vector3(eventData.delta.x, eventData.delta.y, 0f) * panSensitivity * scaleRatio;
        mapContainer.localPosition += _velocity;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && _isHovered)
            _isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            _isDragging = false;
    }

    public void OnPointerEnter(PointerEventData eventData) => _isHovered = true;
    public void OnPointerExit(PointerEventData eventData) => _isHovered = false;
}