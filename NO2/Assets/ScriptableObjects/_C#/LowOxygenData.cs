using UnityEngine;

[CreateAssetMenu(fileName = "LowOxygenData", menuName = "Scriptable Objects/LowOxygenData")]
public class LowOxygenData : ScriptableObject
{
    [SerializeField] private PlayerData _playerData;

    [SerializeField] private float transparency = 0;
    [SerializeField] private float maxTransparency = 0.55f;
    private float timePassedOn0;
    private float maxTimeOn0;

    public float Transparency
    {
        get { return transparency; }
        set { transparency = value; }
    }
    public float MaxTransparency
    {
        get { return maxTransparency; }
        set { maxTransparency = value; }
    }
    public float TimePassedOn0
    {
        get { return timePassedOn0; }
        set { timePassedOn0 = value; }
    }
    public float MaxTimeOn0
    {
        get { return maxTimeOn0; }
    }


    private void OnEnable()
    {
        maxTimeOn0= _playerData.LastOxygenSeconds;
        _playerData.OnOxygenChanged += UpdateTransparency;

    }

    private void OnDisable()
    {
        _playerData.OnOxygenChanged -= UpdateTransparency;
    }

    private void UpdateTransparency()
    {
        int percentage = (int)(_playerData.Oxygen / _playerData.MaxOxygen * 100);

        if (percentage <= 20)
        {
            float t = 1f - (percentage / 20f);
            transparency = Mathf.Lerp(0.05f, 0.55f, t);
        }
        else
        {
            transparency = 0;
        }
    }
}
