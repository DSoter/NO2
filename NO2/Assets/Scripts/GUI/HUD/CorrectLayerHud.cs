using UnityEngine;
using UnityEngine.UI;

public class CorrectLayerHud : MonoBehaviour
{

    private Canvas canvas;
    private Camera principalCam;

    private void Awake()
    {
        //canvas = gameObject.GetComponent<Canvas>();
        //principalCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        //if (principalCam != null)
        //{
        //    canvas.worldCamera = principalCam;
        //}

    }
}
