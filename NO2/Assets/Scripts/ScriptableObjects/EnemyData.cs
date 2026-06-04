using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string objectName;
    public float maxHealth;
    public float health;
    public float walkingSpeed;
    public float oxygenAtDeath;
    public int coinsAtDeath;
    public float basicDamage;
}
