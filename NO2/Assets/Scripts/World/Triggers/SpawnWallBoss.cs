using UnityEngine;

public class SpawnWallBoss : MonoBehaviour
{
    [SerializeField] private GameObject wallDown;
    [SerializeField] private GameObject wallUp;
    [SerializeField] private GameObject boss;
    [SerializeField] private string saveKey = "SnailKingIsDead";
    [SerializeField] private int bossNumber = 0;
    private void Awake()
    {
        if (boss == null)
        {
            Debug.LogWarning("BossPersistence: falta la referencia al jefe");
            enabled = false;
            return;
        }

        bool isDead = PlayerPrefs.GetInt(saveKey, 0) == 1;

        if (isDead)
        {
            //"matamos" al boss
            boss.SetActive(false);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            wallDown.SetActive(true);
            wallUp.SetActive(true);
        }
    }
    private void Update()
    {
        if (!boss.activeSelf)
        {
            switch (bossNumber)
            {
                case 0:
                    GameManager.Instance.GetComponent<AchievementManager>().NotifyEvent("snail_slayer");
                    break;
            }

            wallDown.SetActive(false);
            wallUp.SetActive(false);

            PlayerPrefs.SetInt(saveKey, 1);
            PlayerPrefs.Save();
            Destroy(gameObject);
        }
    }
}
