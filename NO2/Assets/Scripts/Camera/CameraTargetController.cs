using UnityEngine;

public class CameraTargetController : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Vector3 playerLastPosition = new Vector3 (0,0,0);
    [SerializeField] private float cursorInfluence = 0.5f;
    
    private Camera cam;

    private CheckpointManager cm;

    private bool camaraLocked;


    private void Start()
    {
        cam = Camera.main;
        cm = GameManager.Instance.GetComponent<CheckpointManager>();
    }

    private void LateUpdate()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -cam.transform.position.z;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

        if (cm.CameraLockedPlayer)
        {
            camaraLocked = true;
            transform.position= player.position;
            return;
        } 
        if (player == null)
        {
            
            //transform.position = Vector3.Lerp(playerLastPosition, mouseWorld, cursorInfluence);
            UpdatePlayerReference();
        }
        else 
        {
            if (camaraLocked)
            {
                camaraLocked= false;
                return;
            }
            transform.position =
                Vector3.Lerp(player.position, mouseWorld, cursorInfluence);
            playerLastPosition = player.position;
        }
            
    }

    private void UpdatePlayerReference()
    {
        cm = GameManager.Instance.GetComponent<CheckpointManager>();
        player = cm.PlayerReference;
        if (player == null)
        {
            Debug.Log("No se ha encontrado referencia al jugador");
        }
        else
        {
            Vector3 mouseScreen = Input.mousePosition;
            mouseScreen.z = -cam.transform.position.z;

            Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

            transform.position =
                Vector3.Lerp(player.position, mouseWorld, cursorInfluence);
            playerLastPosition = player.position;
        }

    }
}