using UnityEngine;

public class NotebookOptions : MonoBehaviour
{
    [SerializeField] private GameObject panelVolume;
    [SerializeField] private GameObject panelKeybinds;
    public void OpenVolume()
    {
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
        panelKeybinds.SetActive(true);
        CloseVolume();
    }

    public void CloseKeybinds()
    {
        panelKeybinds.SetActive(false);
        PlayerPrefs.Save(); // guarda en disco
    }
}
