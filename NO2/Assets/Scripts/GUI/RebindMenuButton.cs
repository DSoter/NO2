using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindMenuButton : MonoBehaviour
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



        buttonText.text = InputControlPath.ToHumanReadableString(
            newPath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );

        PlayerPrefs.SetString(
            "rebinds",
            actionReference.action.actionMap.asset.SaveBindingOverridesAsJson()
        );
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


}