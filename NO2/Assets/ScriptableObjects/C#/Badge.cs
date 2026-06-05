using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Badge", menuName = "Scriptable Objects/Badge")]
public class Badge : ScriptableObject
{
    public string objectName;
    public Logro achievementRelated;
    public string description;
    public string size;
    public UnityEvent effect;
    
}
