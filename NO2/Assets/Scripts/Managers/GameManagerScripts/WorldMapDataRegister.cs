using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapDataRegister : MonoBehaviour
{
    [SerializeField] private WorldMapData worldMapData;
    [SerializeField] private ScenesVisited scenesVisited;
    public WorldMapData WorldMapData => worldMapData;
    public ScenesVisited Scenes => scenesVisited;
    //[SerializeField] private 
    [Serializable]
    public class ScenesVisited
    {
        public List<string> references;
    }
}