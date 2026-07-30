using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogBox : MonoBehaviour
{
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private GameObject continueIndicator;
    [SerializeField] private float charDelay = 0.03f;
    private float charDefault = 0.03f;
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private UnityEngine.UI.Image characterPortrait;
    [SerializeField] private Animator animatorPortrait;

    [Header("Colorines")]
    [SerializeField] private ColorCharPair[] colorCharPairs;
    private Dictionary<char, UnityEngine.Color> colorMap;
    private UnityEngine.Color actualColor = UnityEngine.Color.white;

    [Header("Animaciones letra")]
    [SerializeField] private AnimationCharPair[] animationCharPairs;
    private Dictionary<char, string> animationTextMap;
    private Dictionary<string, Func<int,int,bool?>> functions;
    [SerializeField] private float waveSpeed = 1f;
    [SerializeField] private float waveHeight = 2f;
    private char lastFunctionChar;
    private int firstCharFromAnimationChain;
    private List<Coroutine> animationCoroutines = new List<Coroutine>();


    private List<DialogLine> currentLines;
    private int currentLineIndex;
    private bool isTyping = false;
    private bool isOpen = false;
    private Coroutine typingCoroutine;
    private CharacterInteractable characterReference;
    private FriendlyCharacterData characterData;
    private TMP_TextInfo textInfo;



    private void Awake()
    {
        functions = new Dictionary<string, Func<int, int, bool?>>
        {
            { "wave",   (first,last) => AnimateWave(first,last)},
            { "rainbowWave",   (first,last) => AnimateRainbowWave(first,last)},
            // añadir funciones que se quieran
        };


        GameManager.Instance.gameObject.GetComponent<InputManager>().onInteract += ConfirmWithInteractButton;
        GameManager.Instance.gameObject.GetComponent<InputManager>().onLeftClick += Confirm;
        dialogPanel.SetActive(false);
        continueIndicator.SetActive(false);
        colorMap = new Dictionary<char, UnityEngine.Color>();
        foreach (ColorCharPair pair in colorCharPairs)
            colorMap[pair.character] = pair.color;
        animationTextMap = new Dictionary<char, string>();
        foreach (AnimationCharPair pair in animationCharPairs)
            animationTextMap[pair.character] = pair.keyFunction;
        charDefault = charDelay;
        textInfo = dialogText.textInfo;

        
    }


    [Serializable]
    public class ColorCharPair
    {
        public char character;
        public UnityEngine.Color color;
    }
    [Serializable]
    public class AnimationCharPair
    {
        public char character;
        public string keyFunction;
    }

    public void StartDialog(List<DialogLine> lines, CharacterInteractable actualCharacter)
    {
        GameManager.Instance.GetComponent<DiverseMenusManager>().DialogIsOpen = true;

        characterReference = actualCharacter;
        characterData = characterReference.CharacterData;
        characterPortrait.sprite = characterData.CharacterPortrait;
        characterNameText.text = characterData.CharacterName;
        animatorPortrait.runtimeAnimatorController = characterData.CharacterAnimator;
        animatorPortrait.SetBool("talking", isTyping);

        currentLines = lines;
        currentLineIndex = 0;
        isOpen = true;
        dialogPanel.SetActive(true);
        ShowNextValidLine();
    }

    private bool AnimateWave(int firstCharacter, int lastCharacter)
    {
        Coroutine c = StartCoroutine(DoingWave(firstCharacter, lastCharacter));
        animationCoroutines.Add(c);

        return true;
    }

    private IEnumerator DoingWave(int firstCharacter, int lastCharacter)
    {
        while (true)
        {
            TMP_TextInfo textInfo = dialogText.textInfo;

            for (int i = firstCharacter; i < lastCharacter; i++)
            {
                if (i >= textInfo.characterCount) break;
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                // Obtener posición original de los vértices
                Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;

                float offset = Mathf.Sin(Time.time * waveSpeed + i * 0.5f) * waveHeight;
                Vector3 wave = new Vector3(0, offset, 0);

                // Usar la posición base del carácter como referencia
                Vector3 charCenter = (sourceVertices[vertexIndex + 0] +
                                      sourceVertices[vertexIndex + 2]) / 2f;

                sourceVertices[vertexIndex + 0] = charInfo.bottomLeft + wave;
                sourceVertices[vertexIndex + 1] = charInfo.topLeft + wave;
                sourceVertices[vertexIndex + 2] = charInfo.topRight + wave;
                sourceVertices[vertexIndex + 3] = charInfo.bottomRight + wave;
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                dialogText.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return null;
        }
    }
    private bool AnimateRainbowWave(int firstCharacter, int lastCharacter)
    {
        Coroutine c = StartCoroutine(DoingRainbowWave(firstCharacter, lastCharacter));
        animationCoroutines.Add(c);

        return true;
    }

    private IEnumerator DoingRainbowWave(int firstCharacter, int lastCharacter)
    {
        while (true)
        {
            TMP_TextInfo textInfo = dialogText.textInfo;

            for (int i = firstCharacter; i < lastCharacter; i++)
            {
                if (i >= textInfo.characterCount) break;
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;
                Color32[] colors = textInfo.meshInfo[materialIndex].colors32;

                float offset = Mathf.Sin(Time.time * waveSpeed + i * 0.5f) * waveHeight;
                Vector3 wave = new Vector3(0, offset, 0);

                vertices[vertexIndex + 0] = charInfo.bottomLeft + wave;
                vertices[vertexIndex + 1] = charInfo.topLeft + wave;
                vertices[vertexIndex + 2] = charInfo.topRight + wave;
                vertices[vertexIndex + 3] = charInfo.bottomRight + wave;

                UnityEngine.Color rainbow = UnityEngine.Color.HSVToRGB((Time.time * 0.3f + i * 0.1f) % 1f, 1f, 1f);
                // Preservar el alpha actual para no interferir con la revelación
                byte currentAlpha = colors[vertexIndex].a;
                Color32 rainbowC = rainbow;
                rainbowC.a = currentAlpha;

                colors[vertexIndex + 0] = rainbowC;
                colors[vertexIndex + 1] = rainbowC;
                colors[vertexIndex + 2] = rainbowC;
                colors[vertexIndex + 3] = rainbowC;
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                dialogText.UpdateGeometry(meshInfo.mesh, i);
                dialogText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }

            yield return null;
        }
    }

    public void ConfirmWithInteractButton()
    {
        if (!isOpen) return;

        if (isTyping)
        {
            CompleteLine();
        }
        else
        {
            AdvanceLine();
            characterReference.DialogIsRecentlyOpen = true; //si el utlimo dialogo se ha saltado con la E no se pulsa automaticamente al hablar con un personaje
        }

    }
    public void Confirm()
    {
        if (!isOpen) return;

        if (isTyping)
        {
            CompleteLine();
        }
        else
        {
            AdvanceLine();
            characterReference.DialogIsRecentlyOpen = false;
        }

    }



    private void AdvanceLine()
    {
        currentLineIndex++;
        ShowNextValidLine();
    }

    private void ShowNextValidLine()
    {
        CancelAllCoroutines();
        while (currentLineIndex < currentLines.Count)
        {
            DialogLine line = currentLines[currentLineIndex];
            if (characterReference.Evaluate(line.conditionKey))
            {
                ShowLine(line.text);
                return;
            }
            currentLineIndex++;
        }
        CloseDialog();
    }


    private void ShowLine(string text)
    {
        continueIndicator.SetActive(false);
        dialogText.text = "";
        CancelAllCoroutines();
        animatorPortrait.SetBool("talking", true);
        typingCoroutine = StartCoroutine(TypeLine(text));
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;
        lastFunctionChar = '\0';

        // Primera pasada: construir texto completo invisible y lanzar animaciones
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        int cont = 0;

        foreach (char c in text)
        {
            if (colorMap.ContainsKey(c))
            {
                if (colorMap[c].Equals(actualColor))
                    actualColor = UnityEngine.Color.white;
                else
                    actualColor = colorMap[c];
                continue;
            }
            else if (animationTextMap.ContainsKey(c))
            {
                if (c == lastFunctionChar)
                {
                    string key = animationTextMap[c];
                    if (functions.ContainsKey(key))
                        functions[key].Invoke(firstCharFromAnimationChain, cont);
                    lastFunctionChar = '\0';
                }
                else
                {
                    lastFunctionChar = c;
                    firstCharFromAnimationChain = cont;
                }
                continue;
            }

            // Añadir letra con color pero invisible (alpha 0)
            string hex = UnityEngine.ColorUtility.ToHtmlStringRGB(actualColor);
            sb.Append($"<color=#{hex}00>{c}</color>"); // 00 = alpha 0
            cont++;
        }

        dialogText.text = sb.ToString();
        dialogText.ForceMeshUpdate();

        // Segunda pasada: revelar letra a letra
        actualColor = UnityEngine.Color.white;
        int visibleCount = 0;

        foreach (char c in text)
        {
            if (colorMap.ContainsKey(c) || animationTextMap.ContainsKey(c))
                continue;

            // Hacer visible la letra actual cambiando su alpha
            TMP_TextInfo info = dialogText.textInfo;
            if (visibleCount < info.characterCount)
            {
                TMP_CharacterInfo charInfo = info.characterInfo[visibleCount];
                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Color32[] colors = info.meshInfo[materialIndex].colors32;
                colors[vertexIndex + 0] = SetAlpha(colors[vertexIndex + 0], 255);
                colors[vertexIndex + 1] = SetAlpha(colors[vertexIndex + 1], 255);
                colors[vertexIndex + 2] = SetAlpha(colors[vertexIndex + 2], 255);
                colors[vertexIndex + 3] = SetAlpha(colors[vertexIndex + 3], 255);

                dialogText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }

            visibleCount++;
            if (charDelay > 0)
                yield return new WaitForSeconds(charDelay);
        }
        animatorPortrait.SetBool("talking", false);
        charDelay = charDefault;
        isTyping = false;
        continueIndicator.SetActive(true);
    }

    private Color32 SetAlpha(Color32 color, byte alpha)
    {
        color.a = alpha;
        return color;
    }

    private void CompleteLine()
    {
        //if (typingCoroutine != null)
        //    StopCoroutine(typingCoroutine);

        // Ponemos el texto completo de la línea actual directamente
        //dialogText.text = currentLines[currentLineIndex].text;
        charDelay = 0;
        animatorPortrait.SetBool("talking", false);
        //isTyping = false;
        //continueIndicator.SetActive(true);
    }

    private void CloseDialog()
    {
        characterReference.DialogIsOpen = false;
        GameManager.Instance.GetComponent<DiverseMenusManager>().DialogIsOpen = false;
        isOpen = false;
        isTyping = false;
        dialogPanel.SetActive(false);
        continueIndicator.SetActive(false);
        dialogText.text = "";
    }

    private void CancelAllCoroutines()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        foreach (Coroutine c in animationCoroutines)
            if (c != null) StopCoroutine(c);
        animationCoroutines.Clear();
    }


}
