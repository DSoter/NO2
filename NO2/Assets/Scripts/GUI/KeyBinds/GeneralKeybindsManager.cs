using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GeneralKeybindsManager : MonoBehaviour
{
    private string json;
    [SerializeField] private InputActionAsset _inputSystemReference;
    [SerializeField] PopUp popUp;

    private InputAction _goUp;
    private InputAction _goLeft;
    private InputAction _goRight;
    private InputAction _goDown;

    private void Awake()
    {
        InputActionMap uiMap = _inputSystemReference.FindActionMap("UI");
        _goUp = uiMap.FindAction("GoUp");
        _goDown = uiMap.FindAction("GoDown");
        _goLeft = uiMap.FindAction("GoLeft");
        _goRight = uiMap.FindAction("GoRight");
    }
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
        ApplySecretOptionMovement();
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
    private void ApplySecretOptionMovement()
    {
        InputActionMap playerMap = _inputSystemReference.FindActionMap("Player");
        InputAction moveAction = playerMap.FindAction("Move");

        List<string> upPaths = new List<string>();
        List<string> downPaths = new List<string>();
        List<string> leftPaths = new List<string>();
        List<string> rightPaths = new List<string>();

        bool anyArrow = false;

        foreach (InputBinding binding in moveAction.bindings)
        {
            if (!binding.isPartOfComposite) continue;

            string path = binding.effectivePath;
            if (string.IsNullOrEmpty(path)) continue;

            if (path.ToLower().Contains("arrow"))
                anyArrow = true;

            switch (binding.name.ToLower())
            {
                case "up": upPaths.Add(path); break;
                case "down": downPaths.Add(path); break;
                case "left": leftPaths.Add(path); break;
                case "right": rightPaths.Add(path); break;
            }
        }

        ApplyDirectionToGoAction(_goUp, upPaths, anyArrow, "<Keyboard>/upArrow");
        ApplyDirectionToGoAction(_goDown, downPaths, anyArrow, "<Keyboard>/downArrow");
        ApplyDirectionToGoAction(_goLeft, leftPaths, anyArrow, "<Keyboard>/leftArrow");
        ApplyDirectionToGoAction(_goRight, rightPaths, anyArrow, "<Keyboard>/rightArrow");
       
    }

    private void ApplyDirectionToGoAction(InputAction goAction, List<string> sourcePaths,
        bool anyArrowInMove, string arrowFallbackPath)
    {

        if (goAction == null) return;


        // Copiamos cada binding de Move al mismo índice en la acción de menú (0 = set WASD, 1 = set flechas)
        for (int i = 0; i < sourcePaths.Count && i < goAction.bindings.Count; i++)
        {
            goAction.ApplyBindingOverride(i, sourcePaths[i]);
        }

        // Si ninguna dirección del Move usa flechas, forzamos el 3er input (índice 2) a la flecha correspondiente
        if (!anyArrowInMove && goAction.bindings.Count > 2)
        {
            goAction.ApplyBindingOverride(2, arrowFallbackPath);
        }
    }
}
