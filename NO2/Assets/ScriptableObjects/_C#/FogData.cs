using UnityEngine;

[CreateAssetMenu(fileName = "FogData", menuName = "Scriptable Objects/FogData")]
public class FogData : ScriptableObject
{
    private bool[,] active;
    private float minX;
    private float minY;
    private float separation;
    private int cols;
    private int rows;

    public bool[,] Active
    {
        get {  return active; }
        set { active = value; }
    }
    public float MinX
    {
        get { return minX; }
        set { minX = value; }
    }
    public float MinY
    {
        get { return minY; }
        set { minY = value; }
    }
    public float Separation
    {
        get { return separation; }
        set { separation = value; }
    }

    public int Cols
    {
        get { return cols; }
        set { cols = value; }
    }
    public int Rows
    {
        get { return rows; }
        set { rows = value; }
    }
    private string SaveKey => name + "_fog";

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int col = Mathf.RoundToInt((worldPosition.x - minX) / separation);
        int row = Mathf.RoundToInt((worldPosition.y - minY) / separation);
        return new Vector2Int(col, row);
    }

    public void SetActive(Vector2Int gridPos, bool value)
    {
        if (gridPos.x >= 0 && gridPos.x < cols && gridPos.y >= 0 && gridPos.y < rows)
        {
            active[gridPos.y, gridPos.x] = value;
            Save();
        }
            
    }

    public void Save()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Cols; col++)
                sb.Append(Active[row, col] ? '1' : '0');

        PlayerPrefs.SetString(SaveKey, sb.ToString());
        PlayerPrefs.SetInt(SaveKey + "_rows", Rows);
        PlayerPrefs.SetInt(SaveKey + "_cols", Cols);
        PlayerPrefs.Save();
    }

    public bool Load()
    {
        string saved = PlayerPrefs.GetString(SaveKey, "");
        if (string.IsNullOrEmpty(saved)) return false;

        Rows = PlayerPrefs.GetInt(SaveKey + "_rows", Rows);
        Cols = PlayerPrefs.GetInt(SaveKey + "_cols", Cols);
        Active = new bool[Rows, Cols];

        int i = 0;
        for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Cols; col++)
                Active[row, col] = saved[i++] == '1';

        return true;
    }

    public void Reset()
    {
        Active = new bool[Rows, Cols];
        for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Cols; col++)
                Active[row, col] = true;
        Save();
    }

}
