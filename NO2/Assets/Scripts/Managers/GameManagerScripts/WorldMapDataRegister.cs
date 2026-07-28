using UnityEngine;

public class WorldMapDataRegister : MonoBehaviour
{
    [SerializeField] private WorldMapData worldMapData;
    public WorldMapData WorldMapData => worldMapData;
}