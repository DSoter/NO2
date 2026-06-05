using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InteractuablePrueba : Interactable
{
    Interactable interactable;
    public override void Interact()
    {
        if(gameObject is null) { Debug.Log("No se detecta el game Object"); }
        else { Destroy(gameObject); }
            
    }
}
