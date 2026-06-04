using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Flower", menuName = "Scriptable Objects/Flower")]
public class Flower : ScriptableObject
{
    public string objectName;
    public UnityEvent basicAttack;
    public UnityEvent chargeAttack;
    public UnityEvent vulnerablePoint;
}
