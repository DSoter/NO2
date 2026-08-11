using UnityEngine;

public interface IEffectable
{
    void ApplyBurn(float damagePerSecond, int duration);
    void ApplySevereBurn(float damagePerSecond, int duration);
}