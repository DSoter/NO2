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
            "¿Desas aplicar los cambios?" +
            "Se borrará la configuración anterior",
            onConfirm: () => ApplyChanges()
            ,
            onCancel: () => Debug.Log("Cancelado")
        );
        
    }

    public void ApplyChanges()
    {
        SaveAuxBindings();
    }
    public void SaveAuxBindings() {

        PlayerPrefs.SetString(
            "auxRebinds",
            _inputSystemReference.SaveBindingOverridesAsJson()
        );

    }
    public void ApplyAuxBindings()
    {
        json = PlayerPrefs.GetString("auxRebinds", "");
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
