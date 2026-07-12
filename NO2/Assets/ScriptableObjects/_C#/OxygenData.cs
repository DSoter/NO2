using UnityEngine;

[CreateAssetMenu(fileName = "AlertsFlags", menuName = "Scriptable Objects/AlertsFlags")]
public class OxygenData : ScriptableObject
{
    [Header("Last Seconds")]
    [SerializeField] private float _lastOxygenSeconds = 4f;
    [SerializeField] private float _lastOxygenTimer = 0f;

    [Space(5)]
    [Header("Alert Flags")]
    [SerializeField] private bool _halfOxygenAlertPlayed = false;
    [SerializeField] private bool _lowOxygenAlertPlayed = false;

    public float LastOxygenSeconds
    {
        get { return _lastOxygenSeconds; }
        set { _lastOxygenSeconds = value; }
    }

    public float LastOxygenTimer
    {
        get { return _lastOxygenTimer; }
        set { _lastOxygenTimer = value; }
    }

    public bool HalfOxygenAlertPlayed
    {
        get
        {
            return _halfOxygenAlertPlayed;
        }
        set
        {
            _halfOxygenAlertPlayed = value;
        }
    }
    public bool LowOxygenAlertPlayed
    {
        get
        {
            return _lowOxygenAlertPlayed;
        }
        set
        {
            _lowOxygenAlertPlayed = value;
        }
    }
}
