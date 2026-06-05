using UnityEngine;

public class SaveAndLoadManager : MonoBehaviour
{
    public EstadisticasJugador prueba;
    public void Awake()
    {
        EstadisticasJugador.current = prueba;
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 80, 20), "Save")) { 
            Debug.Log("Game Saved");
            SaveLoad.Save();
            Debug.Log(EstadisticasJugador.current.objetoRecogido);
        }

        if (GUI.Button(new Rect(160, 20, 80, 20), "Load")) { 
            Debug.Log("Game Loaded");
            SaveLoad.Load();
            Debug.Log(EstadisticasJugador.current.objetoRecogido);
        }
    }

}
