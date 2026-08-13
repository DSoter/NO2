using UnityEngine;

public class NoDeathDetector : MonoBehaviour
{
    [SerializeField] private EquippedBadges equippedBadges;
    private string nameBadgePrincipiante = "Principiante";
    [SerializeField] private int perfectZoneNumber;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("patata");
            if (!equippedBadges.IsEquipped(nameBadgePrincipiante)) //el jugador no ha muerto
            {
                Debug.Log("is locked");
                switch (perfectZoneNumber)
                {
                    case 0:
                        GameManager.Instance.GetComponent<AchievementManager>().NotifyEvent("no_death_tutorial");
                        break;
                }
            }
        }
    }
}
