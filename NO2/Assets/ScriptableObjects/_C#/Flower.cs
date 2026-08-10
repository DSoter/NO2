using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Flower", menuName = "Scriptable Objects/Flower")]
public class Flower : ScriptableObject
{
    public string objectName;
    public string description;
    public Sprite flowerIcon;
    public FlowerEffect flowerEffect;
}
