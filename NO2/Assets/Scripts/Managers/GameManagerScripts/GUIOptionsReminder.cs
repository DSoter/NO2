using UnityEngine;

public class GUIOptionsReminder : MonoBehaviour
{
    private int lastNotebookOption;

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
