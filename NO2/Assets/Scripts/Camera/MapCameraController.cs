using UnityEngine;

public class MapCameraController : MonoBehaviour
{
    [SerializeField] private GameObject camara;
    private DiverseMenusManager dm;
    private void Awake()
    {
        camara.SetActive(false);
        dm = GameManager.Instance.GetComponent<DiverseMenusManager>();
    }
    private void Update()
    {
        camara.SetActive(dm._IsOpen);
    }
}
