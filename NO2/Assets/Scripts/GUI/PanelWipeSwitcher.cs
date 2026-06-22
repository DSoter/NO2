using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static DiverseMenusManager;

public class PanelWipeSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] panels; 
    [SerializeField] private RectTransform wipeCurtain;
    [SerializeField] private float wipeDurationSeconds = 0.4f;

    private int currentIndex = 0;
    private bool isTransitioning = false;

    public bool IsTransitioning
    {
        get { return isTransitioning; }
    }

    public void OnSwitchRight()
    {
        UpdateIndex();
        StartCoroutine(Wipe(1));

    }

    public void OnSwitchLeft()
    {
        UpdateIndex();
        StartCoroutine(Wipe(-1));
        
    }

    private void UpdateIndex()
    {
        DiverseMenusManager dm = FindAnyObjectByType<DiverseMenusManager>();
        if (dm != null)
        {
            int id;
            MenuType menuType;
            menuType = dm.GetMenuType();
            switch (menuType)
            {
                case MenuType.Map:
                    currentIndex = 0;
                    break;
                case MenuType.Badges:
                    currentIndex = 1;
                    break;
                case MenuType.Flowers:
                    currentIndex = 2;
                    break;

            }
        }
    }

    private IEnumerator Wipe(int direction)
    {
        isTransitioning = true;
        int nextIndex = (currentIndex + direction + panels.Length) % panels.Length;

        float startX = direction > 0 ? 0f : 1f;
        wipeCurtain.anchorMin = new Vector2(startX, 0f);
        wipeCurtain.anchorMax = new Vector2(startX, 1f);
        wipeCurtain.gameObject.SetActive(true);

        float half = wipeDurationSeconds / 2f;
        float t = 0f;

        while (t < half)
        {
            t += Time.unscaledDeltaTime; // antes: Time.deltaTime
            float p = Mathf.Clamp01(t / half);
            if (direction > 0)
                wipeCurtain.anchorMax = new Vector2(p, 1f);
            else
                wipeCurtain.anchorMin = new Vector2(1f - p, 0f);
            yield return null;
        }

        panels[currentIndex].SetActive(false);
        panels[nextIndex].SetActive(true);
        currentIndex = nextIndex;

        t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime; // antes: Time.deltaTime
            float p = Mathf.Clamp01(t / half);
            if (direction > 0)
                wipeCurtain.anchorMin = new Vector2(p, 0f);
            else
                wipeCurtain.anchorMax = new Vector2(1f - p, 1f);
            yield return null;
        }

        wipeCurtain.gameObject.SetActive(false);
        isTransitioning = false;
    }
}