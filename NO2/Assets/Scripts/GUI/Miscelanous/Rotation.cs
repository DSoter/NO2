using System.Runtime.CompilerServices;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float degreesPerSecond;
    private Vector3 rotationVector;

    private void Start()
    {
        rotationVector = new Vector3(0, 0, degreesPerSecond);
    }

    // Update is called once per frame
    void Update()
    {
        transform.transform.Rotate(rotationVector * Time.deltaTime);
    }
}
