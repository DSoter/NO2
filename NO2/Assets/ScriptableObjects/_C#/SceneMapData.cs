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
        Campfire
    }
    private MapTile[,] mapMatrix;
    private bool[,] isVisibleMatrix;

    public MapTile[,] MapMatrix
    { 
        get { return mapMatrix; }
        set {  mapMatrix = value; }
    }
    public bool[,] IsVisibleMatrix
    {
        get { return isVisibleMatrix; }
        set { isVisibleMatrix = value; }    
    }
}
