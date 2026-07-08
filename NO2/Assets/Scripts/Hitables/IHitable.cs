using UnityEngine;

public enum AttackStrength
{
    Weak,
    Strong
}

public interface IHitable
{
    public void Hit(Vector2 direction, float damage, AttackStrength strength);
}
