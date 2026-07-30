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
    private SceneMapData _lastMapData;
    [Header("Mapa Mundial")]
    [SerializeField] private WorldMapData worldMapData;

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
    [SerializeField] private Color emptyColor = Color.black;

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
       

        
    }
    private void OnEnable()
    {
        mapData.Save();
        //GenerateMapTexture();
        //_targetScale = mapContainer.localScale.x;
        //GenerateFogTexture();
        //UpdatePlayerIcon();


        UpdateMapData();

        Debug.Log("Fog texture update");
        Debug.Log(mapData.FogTextureHasToUpdate);
        GenerateWorldMapTexture();
        _targetScale = mapContainer.localScale.x;
        UpdatePlayerIcon(); 


    }
    public void GenerateWorldMapTexture()
    {
        GenerateWorldMap();
        GenerateWorldFog();
    }

    private void GenerateWorldMap()
    {
        string path = GetWorldMapTexturePath();

        if (!worldMapData.WorldMapNeedsUpdate && System.IO.File.Exists(path))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            _mapTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            _mapTexture.filterMode = FilterMode.Point;
            _mapTexture.LoadImage(bytes);
            mapImage.texture = _mapTexture;
            return;
        }

        (int minX, int minY, int totalCols, int totalRows) = CalculateWorldBounds();
        if (totalCols <= 0 || totalRows <= 0) return;

        int texWidth = totalCols * pixelsPerTile;
        int texHeight = totalRows * pixelsPerTile;

        if (_mapTexture != null) Destroy(_mapTexture);
        _mapTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
        _mapTexture.filterMode = FilterMode.Point;

        Color[] clearPixels = new Color[texWidth * texHeight];
        for (int i = 0; i < clearPixels.Length; i++) clearPixels[i] = Color.clear;
        _mapTexture.SetPixels(clearPixels);

        foreach (WorldMapData.SceneMapEntry entry in worldMapData.scenes)
        {
            if (entry.mapData?.MapMatrix == null) continue;

            int rows = entry.mapData.MapMatrix.GetLength(0);
            int cols = entry.mapData.MapMatrix.GetLength(1);
            int offsetX = entry.offsetInCells.x - minX;
            int offsetY = entry.offsetInCells.y - minY;

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                {
                    Color color = GetTileColor(entry.mapData.MapMatrix[row, col]);
                    FillTile(_mapTexture, offsetX + col, offsetY + row, color);
                }

            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                {
                    Sprite icon = GetTileIcon(entry.mapData.MapMatrix[row, col]);
                    if (icon != null)
                        DrawIcon(_mapTexture, offsetX + col, offsetY + row, icon);
                }
        }

        _mapTexture.Apply();
        mapImage.texture = _mapTexture;
        System.IO.File.WriteAllBytes(path, _mapTexture.EncodeToPNG());
        worldMapData.WorldMapNeedsUpdate = false;
    }

    private void GenerateWorldFog()
    {
        string path = GetWorldFogTexturePath();
        bool anyDirty = false;
        foreach (WorldMapData.SceneMapEntry entry in worldMapData.scenes)
            if (entry.mapData != null && entry.mapData.FogTextureHasToUpdate)
                anyDirty = true;

        if (!anyDirty && System.IO.File.Exists(path))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            if (_fogTexture != null) Destroy(_fogTexture);
            _fogTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            _fogTexture.filterMode = FilterMode.Point;
            _fogTexture.LoadImage(bytes);
            fogImage.texture = _fogTexture;
            return;
        }

        (int minX, int minY, int totalCols, int totalRows) = CalculateWorldBounds();
        if (totalCols <= 0 || totalRows <= 0) return;

        int texWidth = totalCols * pixelsPerTile;
        int texHeight = totalRows * pixelsPerTile;

        if (_fogTexture != null) Destroy(_fogTexture);
        _fogTexture = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
        _fogTexture.filterMode = FilterMode.Point;

        // Fondo negro — todo oculto por defecto
        Color[] blackPixels = new Color[texWidth * texHeight];
        for (int i = 0; i < blackPixels.Length; i++) blackPixels[i] = Color.black;
        _fogTexture.SetPixels(blackPixels);

        foreach (WorldMapData.SceneMapEntry entry in worldMapData.scenes)
        {
            if (entry.mapData?.IsVisibleMatrix == null)
            {
                Debug.Log("Una matriz de visibilidad es nula");
                continue;
            }
            int trues2 = 0;
            for (int r = 0; r < entry.mapData.IsVisibleMatrix.GetLength(0); r++)
                for (int c = 0; c < entry.mapData.IsVisibleMatrix.GetLength(1); c++)
                    if (entry.mapData.IsVisibleMatrix[r, c]) trues2++;
            Debug.Log($"IsVisibleMatrix en GenerateWorldFog escena {entry.sceneName} — Visibles: {trues2}");

            int rows = entry.mapData.IsVisibleMatrix.GetLength(0);
            int cols = entry.mapData.IsVisibleMatrix.GetLength(1);
            int offsetX = entry.offsetInCells.x - minX;
            int offsetY = entry.offsetInCells.y - minY;
            int trues = 0;
            int falses =0;
            for (int row = 0; row < rows; row++)
                for (int col = 0; col < cols; col++)
                {
                    Color fogColor = entry.mapData.IsVisibleMatrix[row, col]
                        ? Color.clear
                        : Color.black;
                    if (entry.mapData.IsVisibleMatrix[row, col])
                    {
                        trues++;
                    }
                    else
                    {
                        falses++;
                    }
                        FillTile(_fogTexture, offsetX + col, offsetY + row, fogColor);
                }
            entry.mapData.FogTextureHasToUpdate = false;
        }

        _fogTexture.Apply();
        fogImage.texture = _fogTexture;
        System.IO.File.WriteAllBytes(path, _fogTexture.EncodeToPNG());
    }

    private (int minX, int minY, int totalCols, int totalRows) CalculateWorldBounds()
    {
        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;

        foreach (WorldMapData.SceneMapEntry entry in worldMapData.scenes)
        {
            if (entry.mapData?.MapMatrix == null) continue;
            int rows = entry.mapData.MapMatrix.GetLength(0);
            int cols = entry.mapData.MapMatrix.GetLength(1);

            minX = Mathf.Min(minX, entry.offsetInCells.x);
            minY = Mathf.Min(minY, entry.offsetInCells.y);
            maxX = Mathf.Max(maxX, entry.offsetInCells.x + cols);
            maxY = Mathf.Max(maxY, entry.offsetInCells.y + rows);
        }

        if (minX == int.MaxValue) return (0, 0, 0, 0);
        return (minX, minY, maxX - minX, maxY - minY);
    }

    public void UpdatePlayerIcon()
    {
        if (playerIcon == null || playerSprite == null) return;

        CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();
        Transform playerTransform = cm.PlayerReference;
        TilemapToScriptable tts = cm.TilemapToScriptable;

        if (playerTransform == null || tts == null) return;

        // Encontrar la entrada del mapa de la escena actual
        string currentScene = SceneManager.GetActiveScene().name;
        WorldMapData.SceneMapEntry currentEntry = worldMapData.scenes.Find(
            s => s.sceneName == currentScene);

        if (currentEntry?.mapData == null) return;

        (int minX, int minY, int totalCols, int totalRows) = CalculateWorldBounds();
        if (totalCols <= 0) return;

        // Posición en celdas locales
        Vector2Int gridPos = tts.WorldToGrid(playerTransform.position);

        // Convertir a coordenadas globales
        int globalCol = currentEntry.offsetInCells.x - minX + gridPos.x;
        int globalRow = currentEntry.offsetInCells.y - minY + gridPos.y;

        int texWidth = totalCols * pixelsPerTile;
        int texHeight = totalRows * pixelsPerTile;

        float pixelX = globalCol * pixelsPerTile + pixelsPerTile / 2f;
        float pixelY = globalRow * pixelsPerTile + pixelsPerTile / 2f;

        float normX = pixelX / texWidth;
        float normY = pixelY / texHeight;

        RectTransform mapRect = mapImage.rectTransform;
        float localX = (normX - 0.5f) * mapRect.rect.width;
        float localY = (normY - 0.5f) * mapRect.rect.height;

        playerIcon.sprite = playerSprite;
        playerIcon.gameObject.SetActive(true);
        playerIcon.rectTransform.localPosition = new Vector3(localX, localY, 0f);
    }
    //public void GenerateWorldMapTexture()
    //{
    //    worldMapData.LoadOffsets();

    //    // Calcular bounds globales
    //    int minX = int.MaxValue, minY = int.MaxValue;
    //    int maxX = int.MinValue, maxY = int.MinValue;

    //    foreach (WorldMapData.SceneMapEntry entry in worldMapData.scenes)
    //    {
    //        if (entry.mapData?.MapMatrix == null) continue;
    //        int rows = entry.mapData.MapMatrix.GetLength(0);
    //        int cols = entry.mapData.MapMatrix.GetLength(1);

    //        minX = Mathf.Min(minX, entry.offsetInCells.x);
    //        minY = Mathf.Min(minY, entry.offsetInCells.y);
    //        maxX = Mathf.Max(maxX, entry.offsetInCells.x + cols);
    //        maxY = Mathf.Max(maxY, entry.offsetInCells.y + rows);
    //    }

    //    int totalCols = maxX - minX;
    //    int totalRows = maxY - minY;
    //    int texWidth = totalCols * pixelsPerTile;
    //    int texHeight = totalRows * pixelsPerTile;

    //    Texture2D worldTex = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
    //    worldTex.filterMode = FilterMode.Point;

    //    // Fondo transparente
    //    Color[] clearPixels = new Color[texWidth * texHeight];
    //    for (int i = 0; i < clearPixels.Length; i++) clearPixels[i] = Color.clear;
    //    worldTex.SetPixels(clearPixels);

    //    // Dibujar cada escena
    //    foreach (WorldMapData.SceneMapEntry entry in worldMapData.scenes)
    //    {
    //        if (entry.mapData?.MapMatrix == null) continue;

    //        int rows = entry.mapData.MapMatrix.GetLength(0);
    //        int cols = entry.mapData.MapMatrix.GetLength(1);
    //        int offsetX = entry.offsetInCells.x - minX;
    //        int offsetY = entry.offsetInCells.y - minY;

    //        for (int row = 0; row < rows; row++)
    //        {
    //            for (int col = 0; col < cols; col++)
    //            {
    //                Color color = GetTileColor(entry.mapData.MapMatrix[row, col]);
    //                int texCol = offsetX + col;
    //                int texRow = offsetY + row;
    //                FillTile(worldTex, texCol, texRow, color);
    //            }
    //        }

    //        // Iconos
    //        for (int row = 0; row < rows; row++)
    //        {
    //            for (int col = 0; col < cols; col++)
    //            {
    //                Sprite icon = GetTileIcon(entry.mapData.MapMatrix[row, col]);
    //                if (icon != null)
    //                    DrawIcon(worldTex, offsetX + col, offsetY + row, icon);
    //            }
    //        }
    //    }

    //    worldTex.Apply();
    //    mapImage.texture = worldTex;
    //}

    private string GetWorldMapTexturePath()
    {
        return System.IO.Path.Combine(
            Application.persistentDataPath,
            worldMapData.name + "_worldMap.png"
        );
    }

    private string GetWorldFogTexturePath()
    {
        return System.IO.Path.Combine(
            Application.persistentDataPath,
            worldMapData.name + "_worldFog.png"
        );
    }

    [ContextMenu("Clear World Map Cache")]
    public void ClearWorldMapCache()
    {
        string path = GetWorldMapTexturePath();
        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
        string fogPath = GetWorldFogTexturePath();
        if (System.IO.File.Exists(fogPath)) System.IO.File.Delete(fogPath);
        worldMapData.WorldMapNeedsUpdate = true;
        Debug.Log("Caché de mapa mundial eliminada");
    }

    //public void UpdatePlayerIcon()
    //{

    //    if (playerIcon == null || playerSprite == null) return;

    //    CheckpointManager cm = GameManager.Instance.GetComponent<CheckpointManager>();
    //    Transform playerTransform = cm.PlayerReference;
    //    TilemapToScriptable tilemapToScriptable = cm.TilemapToScriptable;

    //    if (playerTransform == null || tilemapToScriptable == null) return;


    //    playerIcon.sprite = playerSprite;
    //    playerIcon.gameObject.SetActive(true);

    //    // Convertir posición mundo a celda del mapa
    //    Vector2Int gridPos = tilemapToScriptable.WorldToGrid(playerTransform.position);

    //    int rows = mapData.MapMatrix.GetLength(0);
    //    int cols = mapData.MapMatrix.GetLength(1);

    //    int texWidth = cols * pixelsPerTile;
    //    int texHeight = rows * pixelsPerTile;

    //    // Posición en píxeles dentro de la textura
    //    float pixelX = gridPos.x * pixelsPerTile + pixelsPerTile / 2f;
    //    float pixelY = gridPos.y * pixelsPerTile + pixelsPerTile / 2f;

    //    // Convertir a posición normalizada (0-1)
    //    float normX = pixelX / texWidth;
    //    float normY = pixelY / texHeight;

    //    // Convertir a posición local dentro del mapContainer
    //    RectTransform mapRect = mapImage.rectTransform;
    //    float localX = (normX - 0.5f) * mapRect.rect.width;
    //    float localY = (normY - 0.5f) * mapRect.rect.height;

    //    playerIcon.rectTransform.localPosition = new Vector3(localX, localY, 0f);
    //}
    public void GenerateMapTexture()
    {
        SceneMapData previousMapData = _lastMapData;
        UpdateMapData();

        if (_lastMapData != mapData)
        {
            _lastMapData = mapData;
            //GenerateFogTexture();
        }


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
        SceneMapData auxMapData = mapData;
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
                    Debug.Log($"UpdateMapData — SceneMapData encontrado: {tts.SceneMapData.name}");
                    //tts.SaveMap();
                    mapData = tts.SceneMapData;
                    if (mapData != auxMapData )
                    {
                        tts.FogManager.ResetAndReinitialize();
                    }
                    mapData.Save();
                }
            }
        }

}
    private string GetFogTexturePath()
    {
        return System.IO.Path.Combine(
            Application.persistentDataPath,
            mapData.name + "_fogTexture.png"
        );
    }

    private void GenerateFogTexture()
    {

        UpdateMapData();


        if (_fogTexture != null)
        {
            Destroy(_fogTexture);
            _fogTexture = null;
        }
        if (mapData?.IsVisibleMatrix == null)
        {
            Debug.LogError("IsVisibleMatrix es null");
            return;
        }
        

        string path = GetFogTexturePath();

        if (!mapData.FogTextureHasToUpdate && System.IO.File.Exists(path))
        {
            Debug.Log("Cargando desde disco");
            // Cargar desde disco
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            _fogTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            _fogTexture.filterMode = FilterMode.Point;
            _fogTexture.LoadImage(bytes);
            fogImage.texture = _fogTexture;
            return;
        }

        // Regenerar
        int rows = mapData.IsVisibleMatrix.GetLength(0);
        int cols = mapData.IsVisibleMatrix.GetLength(1);

        _fogTexture = new Texture2D(cols * pixelsPerTile, rows * pixelsPerTile, TextureFormat.RGBA32, false);
        _fogTexture.filterMode = FilterMode.Point;

        // Rellenar manualmente en lugar de llamar a UpdateFogTexture
        bool[,] visible = mapData.IsVisibleMatrix;
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
            {
                Color fogColor = visible[row, col] ? Color.clear : Color.black;
                FillTile(_fogTexture, col, row, fogColor);
            }

        _fogTexture.Apply();
        fogImage.texture = _fogTexture; // asignar

        //UpdateFogTexture();

        // Guardar en disco y marcar como limpia
        System.IO.File.WriteAllBytes(path, _fogTexture.EncodeToPNG());
        mapData.FogTextureHasToUpdate = false;
        Debug.Log("FogTexture guardada");
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