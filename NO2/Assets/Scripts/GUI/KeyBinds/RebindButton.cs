using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindButton : MonoBehaviour
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
        RebindButton[] allButtons = FindObjectsByType<RebindButton>();
        foreach (RebindButton other in allButtons)
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

        CompositeButton[] compositeButtons = FindObjectsByType<CompositeButton>();
        foreach (CompositeButton other in compositeButtons)
        {
            if (other == this) continue;

            string otherPath = other.ActionReference.action.bindings[other.CompositeValue + other.Dif].effectivePath;
            if (otherPath == newPath)
            {
                // Limpiar el binding conflictivo
                InputActionRebindingExtensions.RemoveBindingOverride(
                    other.ActionReference.action,
                    other.CompositeValue + other.Dif
                );
                // Aplicar override vacío para que quede none
                other.ActionReference.action.ApplyBindingOverride(other.CompositeValue + other.Dif, "");
                other.UpdateDisplayTextComposite();
            }
        }

        PlayerPrefs.SetString(
            "rebinds",
            actionReference.action.actionMap.asset.SaveBindingOverridesAsJson()
        );
        RefreshUIInteractManager();

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


    private void RefreshUIInteractManager()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.GetComponent<InteractManager>().RefreshInteractBinding();
        }
    }
}