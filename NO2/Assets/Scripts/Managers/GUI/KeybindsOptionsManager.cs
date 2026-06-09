using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public class KeybindsOptionsManager : MonoBehaviour
{
    private InputSystem m_Actions;
    private InputSystem.PlayerActions m_Player;
    
    public void changeKey(string inputName, string inputKey, int indexInput)
    {
        switch(inputName)
        {
            case "Interact":
               
                m_Player.Interact.ApplyBindingOverride( (inputName);
                break;
        }
    }
}
