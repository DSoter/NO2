using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogBox : MonoBehaviour
{
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private GameObject continueIndicator;
    [SerializeField] private float charDelay = 0.03f;

    private List<DialogLine> currentLines;
    private int currentLineIndex;
    private bool isTyping = false;
    private bool isOpen = false;
    private Coroutine typingCoroutine;
    private CharacterInteractable characterReference;

    private InputActionReference _interactRef;
    [Header("Input System")]
    [SerializeField] private InputActionAsset _inputSystemReference;

    private void Awake()
    {
        InitializePrefsActions();
        dialogPanel.SetActive(false);
        continueIndicator.SetActive(false);
    }

    public void StartDialog(List<DialogLine> lines, CharacterInteractable actualCharacter)
    {
        GameManager.Instance.GetComponent<DiverseMenusManager>().DialogIsOpen = true;
        Time.timeScale = 0;
        characterReference = actualCharacter;
        currentLines = lines;
        currentLineIndex = 0;
        isOpen = true;
        dialogPanel.SetActive(true);
        ShowNextValidLine();
    }

    public void Confirm()
    {
        if (!isOpen) return;

        if (isTyping)
        {
            // Si está escribiendo, completa el texto instantáneamente
            CompleteLine();
        }
        else
        {
            // Si ya terminó de escribir, avanza a la siguiente línea
            AdvanceLine();
        }
    }
    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isOpen) return;

            if (isTyping)
            {
                // Si está escribiendo, completa el texto instantáneamente
                CompleteLine();
            }
            else
            {
                // Si ya terminó de escribir, avanza a la siguiente línea
                AdvanceLine();
            }
        }
    }

    private void OnEnable()
    {
        //GameManager.Instance.GetComponent<DiverseMenusManager>().onConfirm += Confirm;
        EnableActions();

    }

    private void OnDisable()
    {
        //GameManager.Instance.GetComponent<DiverseMenusManager>().onConfirm -= Confirm;
        DisposeActions();
    }

    private void InitializePrefsActions()
    {


        string json = PlayerPrefs.GetString("rebinds", "");
        if (!string.IsNullOrEmpty(json))
        {
            _inputSystemReference.LoadBindingOverridesFromJson(json);
        }

        InputActionMap playerMap = _inputSystemReference.FindActionMap("Player");

        _interactRef = InputActionReference.Create(playerMap.FindAction("Interact"));

        EnableActions();
        playerMap.Enable();
    }

    private void DisposeActions()
    {
        if (_interactRef != null)
        {
            _interactRef.action.performed -= OnConfirm;
            _inputSystemReference.FindActionMap("Player").Disable();
        }
    }

    private void EnableActions()
    {
        _interactRef.action.performed += OnConfirm;
    }

    private void AdvanceLine()
    {
        currentLineIndex++;
        ShowNextValidLine();
    }

    private void ShowNextValidLine()
    {
        // Buscamos la siguiente línea cuya condición se cumpla
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

        // No quedan líneas válidas, cerramos
        CloseDialog();
    }

    private void ShowLine(string text)
    {
        continueIndicator.SetActive(false);
        dialogText.text = "";

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(text));
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;

        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSecondsRealtime(charDelay);
        }

        isTyping = false;
        continueIndicator.SetActive(true);
    }

    private void CompleteLine()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        // Ponemos el texto completo de la línea actual directamente
        dialogText.text = currentLines[currentLineIndex].text;
        isTyping = false;
        continueIndicator.SetActive(true);
    }

    private void CloseDialog()
    {
        Time.timeScale = 1;
        characterReference.DialogIsOpen = false;
        characterReference.DialogIsRecentlyOpen = true;
        GameManager.Instance.GetComponent<DiverseMenusManager>().DialogIsOpen = false;
        isOpen = false;
        isTyping = false;
        dialogPanel.SetActive(false);
        continueIndicator.SetActive(false);
        dialogText.text = "";
    }


}
