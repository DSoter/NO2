using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CompositeButton : MonoBehaviour
{
    [SerializeField] private InputActionReference actionReference;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private int indexInput;
    [SerializeField] private int compositeValue;


    private int dif;

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
        buttonText.text = InputControlPath.ToHumanReadableString(
                actionReference.action.bindings[compositeValue+dif].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );

        if (actionReference is null) { Debug.Log("Falta Action Reference"); }
        if (buttonText is null) { Debug.Log("Falta TMP_Text"); }

    }
    public void StartRebinding()
    {
        buttonText.text = "...";

        actionReference.action.Disable();

        rebindingOperation = actionReference.action
            .PerformInteractiveRebinding(compositeValue+dif).WithCancelingThrough("<Keyboard>/escape")
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


        buttonText.text = InputControlPath.ToHumanReadableString(
            newPath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );

        PlayerPrefs.SetString(
            "rebinds",
            actionReference.action.actionMap.asset.SaveBindingOverridesAsJson()
        );

        RefreshUIInteractManager();
    }

    public void UpdateDisplayTextComposite()
    {
        string path = actionReference.action.bindings[compositeValue + dif].effectivePath;
        buttonText.text = string.IsNullOrEmpty(path)
            ? "None"
            : InputControlPath.ToHumanReadableString(
                path,
                InputControlPath.HumanReadableStringOptions.OmitDevice
              );
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