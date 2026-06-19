using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class NotebookOptions : MonoBehaviour
{
    [SerializeField] private GameObject panelVolume;
    [SerializeField] private GameObject panelKeybinds;
    [SerializeField] private UnityEvent saveAuxKeybinds;
    [SerializeField] private UnityEvent applyAuxKeybinds;
    private GUIOptionsReminder _optionsReminder;

    private void Awake()
    {
        _optionsReminder = FindAnyObjectByType<GUIOptionsReminder>();
    }

    public void InitializeOptions()
    {
        applyAuxKeybinds.Invoke();
        saveAuxKeybinds.Invoke();
        switch (_optionsReminder.LastNotebookOption)
        {
            case 0:
                OpenVolume();
                break;
            case 1:
                OpenKeybinds();
                break;
        }
    }

    public void OpenVolume()
    {
        _optionsReminder.LastNotebookOption = 0;
        PlayerPrefs.SetInt("LastNotebookOption", 0);

        panelVolume.SetActive(true);
        CloseKeybinds();
    }

    public void CloseVolume()
    {
        panelVolume.SetActive(false);
        PlayerPrefs.Save(); // guarda en disco
    }

    public void OpenKeybinds()
    {
        saveAuxKeybinds.Invoke();
        _optionsReminder.LastNotebookOption = 1;
        PlayerPrefs.SetInt("LastNotebookOption", 1);

        panelKeybinds.SetActive(true);
        CloseVolume();
    }

    public void CloseKeybinds()
    {
        applyAuxKeybinds.Invoke();
        panelKeybinds.SetActive(false);
        PlayerPrefs.Save(); // guarda en disco
    }
}
