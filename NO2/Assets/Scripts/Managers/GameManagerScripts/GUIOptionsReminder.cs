using UnityEngine;

public class GUIOptionsReminder : MonoBehaviour
{
    private int lastNotebookOption;
    private int lastMenuOpen; //esto es para lo del menu de las flores y todo el rollo

    private void Awake()
    {
        lastNotebookOption = PlayerPrefs.GetInt("LastNotebookOption", 0);
    }
    
    public int LastNotebookOption
    {  
        get {
            return lastNotebookOption; 
        } 
        set { 
            lastNotebookOption = value; 
        } 
    }
}
