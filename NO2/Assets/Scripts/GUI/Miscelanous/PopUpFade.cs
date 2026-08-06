using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class PopUpFade : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private float animationDurationOnSeconds;
    private Image fondoPanel;
    private Text textoBoton;

    private bool isFadingIn=false;
    private bool isFadingOut=false;
    private float counterFading;

    private UnityAction pushedConfirm;


    private void Awake()
    {
        confirmButton.onClick.AddListener(Hide);
        //panel.SetActive(false);
        fondoPanel = panel.GetComponent<Image>();
        textoBoton = confirmButton.GetComponentInChildren<Text>();
    }
    private void Update()
    {
        if (isFadingIn)
        {
            
            
            Color panelColorIn = fondoPanel.color;
            panelColorIn.a += Time.deltaTime / animationDurationOnSeconds;
            fondoPanel.color = panelColorIn;
            

            Color buttonColorIn = textoBoton.color;
            buttonColorIn.a += Time.deltaTime / animationDurationOnSeconds;
            textoBoton.color = buttonColorIn;

            Color messageColorIn = messageText.color;
            messageColorIn.a += Time.deltaTime / animationDurationOnSeconds;
            messageText.color = messageColorIn;

            
            if (messageColorIn.a >= 1)
            {
                isFadingIn = false;


                panelColorIn.a =1.0f;
                fondoPanel.color = panelColorIn;

                buttonColorIn.a = 1.0f;
                textoBoton.color = buttonColorIn;

                messageColorIn.a = 1.0f;
                messageText.color = messageColorIn;
            }

        }
        if (isFadingOut)
        {
            isFadingIn = false;
            Color panelColorOut = fondoPanel.color;
            panelColorOut.a -= Time.unscaledDeltaTime / animationDurationOnSeconds;
            fondoPanel.color = panelColorOut;
            Color buttonColorOut = textoBoton.color;
            buttonColorOut.a -= Time.unscaledDeltaTime / animationDurationOnSeconds;
            textoBoton.color = buttonColorOut;

            Color messageColorOut = messageText.color;
            messageColorOut.a -= Time.unscaledDeltaTime / animationDurationOnSeconds;
            messageText.color = messageColorOut;
            if (messageColorOut.a <=0)
            {
                Debug.Log("Terminado de fade out");
                isFadingOut = false;
                pushedConfirm?.Invoke();
                //panel.SetActive(false);
            }
        }
    }

    public void Show(string message, UnityAction onConfirm)
    {
        pushedConfirm = onConfirm;

        messageText.text = message;
        FadeIn();

        // Limpiar listeners anteriores
        confirmButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() => { FadeOut(); });

    }
    public void ShowWithoutFadeIn(string message, UnityAction onConfirm)
    {
        panel.SetActive(true);
        fondoPanel = panel.GetComponent<Image>();
        textoBoton = confirmButton.GetComponentInChildren<Text>();

        

        pushedConfirm = onConfirm;

        messageText.text = message;


        Color panelColorIn = fondoPanel.color;
        panelColorIn.a = 1;
        fondoPanel.color = panelColorIn;

        Color buttonColorIn = textoBoton.color;
        buttonColorIn.a = 1;
        textoBoton.color = buttonColorIn;

        Color messageColorIn = messageText.color;
        messageColorIn.a = 1;
        messageText.color = messageColorIn;


        Debug.Log("Color a tope");
        // Limpiar listeners anteriores
        confirmButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() => { FadeOut(); });
    }

    public void Hide()
    {
        Debug.Log("Hide called it");
        //panel.SetActive(false);
    }

    public void FadeIn()
    {
        Time.timeScale = 1.0f;
        Debug.Log("Show");
        isFadingIn = true;
        panel.SetActive(true);
        Debug.Log($"gameObject activeSelf: {gameObject.activeSelf}, activeInHierarchy: {gameObject.activeInHierarchy}, enabled: {enabled}");


    }
    public void FadeOut()
    {
        isFadingIn = false;
        isFadingOut= true;
    }

}