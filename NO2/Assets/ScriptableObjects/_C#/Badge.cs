using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Badge", menuName = "Scriptable Objects/Badge")]
public class Badge : ScriptableObject
{
    public string badgeName;
    public Achievement achievement;
    public string description;
    public Sprite icon;
    [Range(1, 4)] public int slotsSize = 1;
    public UnityEvent effect;
    
}
