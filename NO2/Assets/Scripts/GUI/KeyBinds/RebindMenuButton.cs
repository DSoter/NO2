using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindMenuButton : MonoBehaviour
{
    [SerializeField] private InputActionReference actionReference;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Image buttonIcon;
    [SerializeField] private KeyIconDatabase keyIconDatabase;
    [SerializeField] private int indexInput;

    private string currentEffectivePath;
    private string currentIconLookupPath;

    public InputActionReference ActionReference
    {
        get { return actionReference; }
    }
    public int IndexInput
    {
        get { return indexInput; }
    }
    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;
    private void Start()
    {
        string json = PlayerPrefs.GetString("rebinds", "");
        if (!string.IsNullOrEmpty(json))
        {
            actionReference.action.actionMap.asset.LoadBindingOverridesFromJson(json);
        }

        UpdateDisplayText();
    }
    public void StartRebinding()
    {
        if (buttonIcon.gameObject.activeSelf)
        {
            Sprite pressedIcon = keyIconDatabase.GetIconPressed(currentIconLookupPath);
            if (pressedIcon != null)
                buttonIcon.sprite = pressedIcon;
        }
        else
        {
            buttonText.text = "...";
        }

        actionReference.action.Disable();
        rebindingOperation = actionReference.action
            .PerformInteractiveRebinding(indexInput).WithControlsExcluding("<Keyboard>/enter").WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(operation =>
            {
                operation.Dispose();
                actionReference.action.Enable();
                UpdateBindingText();
            }).OnCancel(operation => UpdateDisplayText());
        rebindingOperation.Start();
    }
    private void UpdateBindingText()
    {
        string newPath = actionReference.action.bindings[indexInput].effectivePath;
        // Buscar conflictos en todos los RebindButton de la escena
        RebindMenuButton[] allButtons = FindObjectsByType<RebindMenuButton>();
        foreach (RebindMenuButton other in allButtons)
        {
            if (other == this) continue;
            string otherPath = other.actionReference.action.bindings[other.indexInput].effectivePath;
            if (otherPath == newPath)
            {
                // Limpiar el binding conflictivo
                InputActionRebindingExtensions.RemoveBindingOverride(
                    other.actionReference.action,
                    other.indexInput
                );
                // Aplicar override vacío para que quede none
                other.actionReference.action.ApplyBindingOverride(other.indexInput, "");
                other.UpdateDisplayText();
            }
        }

        PlayerPrefs.SetString(
            "rebinds",
            actionReference.action.actionMap.asset.SaveBindingOverridesAsJson()
        );

        UpdateDisplayText();
    }
    public void UpdateDisplayText()
    {
        currentEffectivePath = actionReference.action.bindings[indexInput].effectivePath;
        currentIconLookupPath = string.IsNullOrEmpty(currentEffectivePath) ? "empty" : currentEffectivePath;

        Sprite icon = keyIconDatabase.GetIcon(currentIconLookupPath);

        if (icon != null)
        {
            buttonIcon.sprite = icon;
            buttonIcon.gameObject.SetActive(true);
            buttonText.gameObject.SetActive(false);
        }
        else
        {
            buttonIcon.gameObject.SetActive(false);
            buttonText.gameObject.SetActive(true);
            buttonText.text = string.IsNullOrEmpty(currentEffectivePath)
                ? "None"
                : InputControlPath.ToHumanReadableString(
                    currentEffectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice
                  );
        }
    }
}