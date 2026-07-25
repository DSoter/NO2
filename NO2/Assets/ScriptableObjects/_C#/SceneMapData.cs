using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneMapData", menuName = "Scriptable Objects/SceneMapData")]
public class SceneMapData : ScriptableObject
{
    public enum MapTile
    {
        Floor,
        Wall,
        Npc,
        Flower,
        Campfire,
        Gate,

    }
    private MapTile[,] mapMatrix;
    private bool[,] isVisibleMatrix;
    public int[,] fogCoverCount; 

    public MapTile[,] MapMatrix
    { 
        get { return mapMatrix; }
        set {  
            mapMatrix = value;
            //Save();  
        }
    }
    public bool[,] IsVisibleMatrix
    {
        get { return isVisibleMatrix; }
        set { 
            isVisibleMatrix = value;
            //Save();
        }    
    }
    public int[,] FogCoverCount
    {
        get { return fogCoverCount; }
        set 
        {  
            fogCoverCount = value; 
            //Save(); 
        }
    }

    private string SaveKey => name + "_mapdata";

    public void Save()
    {
        int rows = mapMatrix.GetLength(0);
        int cols = mapMatrix.GetLength(1);

        PlayerPrefs.SetInt(SaveKey + "_rows", rows);
        PlayerPrefs.SetInt(SaveKey + "_cols", cols);

        // Guardar MapMatrix como string de numeros
        System.Text.StringBuilder sbMap = new System.Text.StringBuilder();
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
                sbMap.Append((int)mapMatrix[row, col]);
        PlayerPrefs.SetString(SaveKey + "_map", sbMap.ToString());

        // Guardar IsVisibleMatrix como string de 0 y 1

        if(isVisibleMatrix == null) { isVisibleMatrix = new bool[rows, cols];  }
        
        System.Text.StringBuilder sbVisible = new System.Text.StringBuilder();
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
                sbVisible.Append(isVisibleMatrix[row, col] ? '1' : '0');
        PlayerPrefs.SetString(SaveKey + "_visible", sbVisible.ToString());

        if(fogCoverCount == null) { fogCoverCount = new int[rows, cols]; }
        // Guardar FogCoverCount como string separado por comas
        System.Text.StringBuilder sbFog = new System.Text.StringBuilder();
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
            {
                sbFog.Append(fogCoverCount[row, col]);
                if (row < rows - 1 || col < cols - 1) sbFog.Append(',');
            }
        PlayerPrefs.SetString(SaveKey + "_fog", sbFog.ToString());

        PlayerPrefs.Save();
    }
    public void SaveMapOnly()
    {
        if (mapMatrix == null) return;

        int rows = mapMatrix.GetLength(0);
        int cols = mapMatrix.GetLength(1);

        PlayerPrefs.SetInt(SaveKey + "_rows", rows);
        PlayerPrefs.SetInt(SaveKey + "_cols", cols);

        System.Text.StringBuilder sbMap = new System.Text.StringBuilder();
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
                sbMap.Append((int)mapMatrix[row, col]);

        PlayerPrefs.SetString(SaveKey + "_map", sbMap.ToString());
        PlayerPrefs.Save();
        Debug.Log($"Mapa guardado: {rows}x{cols}");
    }

    public bool Load()
    {
        string savedMap = PlayerPrefs.GetString(SaveKey + "_map", "");
        Debug.Log($"SaveKey: {SaveKey}");
        Debug.Log($"savedMap longitud: {savedMap.Length}");

        if (string.IsNullOrEmpty(savedMap)) return false;

        int rows = PlayerPrefs.GetInt(SaveKey + "_rows", 0);
        int cols = PlayerPrefs.GetInt(SaveKey + "_cols", 0);
        Debug.Log($"Rows: {rows} Cols: {cols}");
        if (rows == 0 || cols == 0) return false;

        // Cargar MapMatrix
        mapMatrix = new MapTile[rows, cols];
        int i = 0;
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
                mapMatrix[row, col] = (MapTile)(savedMap[i++] - '0');

        // Cargar IsVisibleMatrix

        isVisibleMatrix = new bool[rows, cols];
        string savedVisible = PlayerPrefs.GetString(SaveKey + "_visible", "");
        Debug.Log($"savedVisible longitud: {savedVisible.Length}");
        int visibles = 0;
        int noVisibles = 0;

        i = 0;
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
            {
                isVisibleMatrix[row, col] = savedVisible[i++] == '1';
                if (isVisibleMatrix[row, col]) visibles++; else noVisibles++;
            }
        Debug.Log($"Visibles cargados: {visibles} No visibles: {noVisibles}");

        // Cargar FogCoverCount
        string savedFog = PlayerPrefs.GetString(SaveKey + "_fog", "");
        Debug.Log($"savedFog longitud: {savedFog.Length}");
        FogCoverCount = new int[rows, cols];


        string[] fogValues = savedFog.Split(',');
        i = 0;
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
                FogCoverCount[row, col] = int.Parse(fogValues[i++]);



        return true;
    }

    public void Reset()
    {
        PlayerPrefs.DeleteKey(SaveKey + "_map");
        PlayerPrefs.DeleteKey(SaveKey + "_visible");
        PlayerPrefs.DeleteKey(SaveKey + "_fog");
        PlayerPrefs.DeleteKey(SaveKey + "_rows");
        PlayerPrefs.DeleteKey(SaveKey + "_cols");
        PlayerPrefs.Save();
        mapMatrix = null;
        isVisibleMatrix = null;
        fogCoverCount = null;
    }
}
