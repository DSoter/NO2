using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoSceneController: MonoBehaviour
{

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }



    public void RellenarEncuesta()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSf_d3JCZnVc0GImSZvZY9u02MC7pFQGBsZbxSTeeeX09ukmXA/viewform?usp=dialog");

    }

    public void BotonSalir()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }


}
