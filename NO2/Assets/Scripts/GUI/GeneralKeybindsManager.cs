using UnityEngine;
using UnityEngine.InputSystem;

public class GeneralKeybindsManager : MonoBehaviour
{
    private string json;
    [SerializeField] private InputActionAsset _inputSystemReference;
    [SerializeField] PopUp popUp;
    public void SaveDefaultKeybinds()
    {   
        popUp.Show(
            "¿Deseas sobrescribir las keybinds por defecto?",
            onConfirm: () => PlayerPrefs.SetString(
            "defaultRebinds",
            _inputSystemReference.SaveBindingOverridesAsJson()
        ),
            onCancel: () => Debug.Log("Cancelado")
        );
        
    }
    public void ResetDefaultKeybinds()
    {
        json = PlayerPrefs.GetString("defaultRebinds", "");
        if (!string.IsNullOrEmpty(json))
        {
           _inputSystemReference.LoadBindingOverridesFromJson(json);
        }
        PlayerPrefs.SetString(
            "rebinds",
            _inputSystemReference.SaveBindingOverridesAsJson()
        );

        RebindButton[] allButtons = FindObjectsByType<RebindButton>();
        foreach (RebindButton other in allButtons)
        {
            other.UpdateDisplayText();

        }

        CompositeButton[] compositeButtons = FindObjectsByType<CompositeButton>();
        foreach (CompositeButton other in compositeButtons)
        {
            other.UpdateDisplayTextComposite();
        }
    }
}
