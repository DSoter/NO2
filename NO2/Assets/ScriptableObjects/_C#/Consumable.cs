using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "Scriptable Objects/Consumable")]
public class Consumable : ScriptableObject
{
    [SerializeField] private string function; //se utiliza para relacionar la función del consumible con el nombre
    [SerializeField] private float value;
}
