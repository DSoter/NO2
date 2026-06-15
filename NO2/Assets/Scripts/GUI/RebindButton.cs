using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindButton : MonoBehaviour
{
    [SerializeField] private InputActionReference actionReference;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private int indexInput;

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
        buttonText.text = InputControlPath.ToHumanReadableString(
                actionReference.action.bindings[indexInput].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );

        
    }
    public void StartRebinding()
    {
        buttonText.text = "...";

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
    public void UpdateDisplayText()
    {
        string path = actionReference.action.bindings[indexInput].effectivePath;
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