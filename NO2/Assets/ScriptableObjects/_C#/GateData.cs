using UnityEngine;

[CreateAssetMenu(fileName = "GateData", menuName = "Scriptable Objects/GateData")]
public class GateData : ScriptableObject
{
    public string sceneName1;
    public int gateId1;
    public Vector2 gateDirection1;
    public string sceneName2;
    public int gateId2;
    public Vector2 gateDirection2;
    
}
