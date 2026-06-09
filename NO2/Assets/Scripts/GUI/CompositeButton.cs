using TMPro;
using UnityEditor.SpeedTree.Importer;
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
            .PerformInteractiveRebinding(compositeValue+dif).WithControlsExcluding("<Keyboard>/escape")
            .OnComplete(operation =>
            {
                operation.Dispose();

                actionReference.action.Enable();

                UpdateBindingText();
            });

        rebindingOperation.Start();
    }

    private void UpdateBindingText()
    {
        buttonText.text =
            InputControlPath.ToHumanReadableString(
                actionReference.action.bindings[compositeValue+dif].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );
        PlayerPrefs.SetString(
    "rebinds",
    actionReference.action.actionMap.asset.SaveBindingOverridesAsJson()
);
    }
}