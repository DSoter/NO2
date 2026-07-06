using UnityEngine;
using UnityEngine.UI;

public class LowOxygenController : MonoBehaviour
{
    [SerializeField] private GameObject blackImage;
    [SerializeField] private LowOxygenData loData;
    [SerializeField] private float transparency;

    private Canvas canvas;
    private Camera principalCam;

    private Image image;
    private Color color;
    private bool isDying;

    private void Awake()
    {
        image = blackImage.GetComponent<Image>();
        canvas= gameObject.GetComponent<Canvas>();
        principalCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        if(principalCam != null)
        {
            canvas.worldCamera = principalCam;
        }
        
    }
    private void Update()
    {
        transparency = loData.Transparency;
        if (!isDying)
        {
            if (transparency >= loData.MaxTransparency )
            {
                isDying = true;
            }
            else 
            {
                loData.TimePassedOn0 = 0;
                color.a = transparency;
                image.color = color;
            }
                
        }
        else
        {
            if (loData.Transparency < loData.MaxTransparency)
            {
                loData.TimePassedOn0 = 0;
                isDying = false;
                
                //hacer animaci�n con corutina de ir volviendose clara progresivamente
            }
            else
            {
                loData.TimePassedOn0 += Time.deltaTime;
                float t = Mathf.Clamp01(loData.TimePassedOn0 / loData.MaxTimeOn0);
                color.a = Mathf.Lerp(loData.MaxTransparency, 1.0f, t);
                image.color = color;
            }
        }
    }

}
