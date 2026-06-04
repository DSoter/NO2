using UnityEngine;

[CreateAssetMenu(fileName = "Destroyable", menuName = "Scriptable Objects/Destroyable")]
public class Destroyable : ScriptableObject
{
    public enum DestruibleType { 
        BOX, 
        BARRICADE 
    }

    public string objectName;
    public Sprite sprite;
    public float maxHealth;
    public float health;
    public float amountOfMoney;
   
    public DestruibleType type;
}
