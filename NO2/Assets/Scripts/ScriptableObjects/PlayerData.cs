using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public float maxHealth;
    public float maxStamina;
    public float maxOxygen;
    public float walkingSpeed;
    public float runningSpeed;
    public float staminaRegenerationSpeed;
    public float oxigenDropingSpeed;
    public float rollCooldown;
    public float damageBasicAttack;
    public float cooldoownBasicAttack;
    public float durationBasicAttack;
    public float damageChargeAttack;
    public float cooldoownChargeAttack;
    public float durationChargeOfChargeAttack;
    public float durationChargeAttack;

}
