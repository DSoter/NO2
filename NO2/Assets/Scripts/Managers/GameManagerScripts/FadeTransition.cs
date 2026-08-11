using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class FadeTransition : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private static FadeTransition instance;

    private void Awake()
    {
        // Singleton simple para poder llamarlo desde cualquier botón sin arrastrar referencias
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public static FadeTransition Instance => instance;

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        yield return StartCoroutine(FadeTo(1f));
        SceneManager.LoadSceneAsync(sceneName);
        //yield return StartCoroutine(FadeTo(0f));
    }
    public void MakeVisible()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        // Bloquea clics durante toda la animación, tanto al oscurecer como al aclarar
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; // unscaledDeltaTime por si el juego pausa el timeScale al abrir el menú
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        // Solo dejamos de bloquear clics cuando queda completamente transparente (alpha 0)
        bool stillOpaque = targetAlpha > 0f;
        canvasGroup.blocksRaycasts = stillOpaque;
        canvasGroup.interactable = stillOpaque;
    }
}