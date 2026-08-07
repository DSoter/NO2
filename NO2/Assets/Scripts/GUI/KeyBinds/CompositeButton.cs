using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CompositeButton : MonoBehaviour
{
    [SerializeField] private InputActionReference actionReference;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Image buttonIcon;
    [SerializeField] private KeyIconDatabase keyIconDatabase;
    [SerializeField] private int indexInput;
    [SerializeField] private int compositeValue;


    private int dif;
    private string currentEffectivePath;
    private string currentIconLookupPath;

    private InputSystem m_Actions;
    private InputSystem.PlayerActions m_Player;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;


    public InputActionReference ActionReference
    {
        get { return actionReference; }
    }
    public int CompositeValue
    {
        get { return compositeValue; }
    }
    public int Dif
    {
        get { return dif; }
    }

    private void Start()
    {
        string json = PlayerPrefs.GetString("rebinds", "");

        if (indexInput == 0)
        {
            dif = 1;
        }
        else
        {
            dif = 6;
        }

        if (!string.IsNullOrEmpty(json))
        {
            actionReference.action.actionMap.asset.LoadBindingOverridesFromJson(json);
        }

        UpdateDisplayTextComposite();

        if (actionReference is null) { Debug.Log("Falta Action Reference"); }
        if (buttonText is null) { Debug.Log("Falta TMP_Text"); }

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
            .PerformInteractiveRebinding(compositeValue + dif).WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(operation =>
            {
                operation.Dispose();

                actionReference.action.Enable();

                UpdateBindingText();
            }).OnCancel(operation => UpdateDisplayTextComposite());

        rebindingOperation.Start();
    }

    private void UpdateBindingText()
    {
        string newPath = actionReference.action.bindings[compositeValue + dif].effectivePath;

        // Buscar conflictos en todos los RebindButton de la escena
        RebindButton[] allButtons = FindObjectsByType<RebindButton>();
        foreach (RebindButton other in allButtons)
        {
            if (other == this) continue;

            string otherPath = other.ActionReference.action.bindings[other.IndexInput].effectivePath;
            if (otherPath == newPath)
            {
                // Limpiar el binding conflictivo
                InputActionRebindingExtensions.RemoveBindingOverride(
                    other.ActionReference.action,
                    other.IndexInput
                );
                // Aplicar override vacío para que quede none
                other.ActionReference.action.ApplyBindingOverride(other.IndexInput, "");
                other.UpdateDisplayText();
            }
        }

        CompositeButton[] compositeButtons = FindObjectsByType<CompositeButton>();
        foreach (CompositeButton other in compositeButtons)
        {
            if (other == this) continue;

            string otherPath = other.actionReference.action.bindings[other.compositeValue + other.dif].effectivePath;
            if (otherPath == newPath)
            {
                // Limpiar el binding conflictivo
                InputActionRebindingExtensions.RemoveBindingOverride(
                    other.actionReference.action,
                    other.compositeValue + other.dif
                );
                // Aplicar override vacío para que quede none
                other.actionReference.action.ApplyBindingOverride(other.compositeValue + other.dif, "");
                other.UpdateDisplayTextComposite();
            }
        }


        PlayerPrefs.SetString(
            "rebinds",
            actionReference.action.actionMap.asset.SaveBindingOverridesAsJson()
        );

        RefreshUIInteractManager();

        UpdateDisplayTextComposite();
    }

    public void UpdateDisplayTextComposite()
    {
        currentEffectivePath = actionReference.action.bindings[compositeValue + dif].effectivePath;
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