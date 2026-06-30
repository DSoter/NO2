using UnityEngine;
using UnityEngine.UI;

public class LowOxygenController : MonoBehaviour
{
    [SerializeField] private GameObject blackImage;
    [SerializeField] private LowOxygenData loData;
    [SerializeField] private float transparency;

    private Image image;
    private Color color;
    private bool isDying;

    private void Awake()
    {
        image = blackImage.GetComponent<Image>();

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
                color.a = transparency;
                image.color = color;
            }
                
        }
        else
        {
            if (loData.Transparency <= loData.MaxTransparency)
            {
                isDying = false;
                loData.TimePassedOn0 = 0;
                //hacer animación con corutina de ir volviendose clara progresivamente
            }
            else
            {
                loData.TimePassedOn0 += Time.deltaTime;
                float t = Mathf.Clamp01(loData.TimePassedOn0 / loData.MaxTimeOn0);
                color.a = Mathf.Lerp(loData.MaxTransparency, 0.9f, t);
                image.color = color;
            }
        }
    }

}
