using UnityEngine;

public class NoDeathDetector : MonoBehaviour
{
    [SerializeField] private BadgeCollection badgeCollection;
    [SerializeField] private Badge badgePrincipiante;
    [SerializeField] private int perfectZoneNumber;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("patata");
            if (!badgeCollection.UnlockedBadges[badgePrincipiante]) //el jugador no ha muerto
            {
                switch (perfectZoneNumber)
                {
                    case 0:

                        Debug.Log("Ha entrado al cero");
                        GameManager.Instance.GetComponent<AchievementManager>().NotifyEvent("no_death_tutorial");
                        break;
                }
            }
        }
    }
}
