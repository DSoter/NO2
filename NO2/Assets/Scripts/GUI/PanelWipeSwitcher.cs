using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static DiverseMenusManager;

public class PanelWipeSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] panels; 
    [SerializeField] private GameObject[] insidePanels;
    [SerializeField] private RectTransform wipeCurtain;
    [SerializeField] private float wipeDurationSeconds = 0.4f;

    [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);


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

        // Panel actual colapsa de 1 a 0
        float t = 0f;
        while (t < wipeDurationSeconds)
        {
            t += Time.unscaledDeltaTime;
            float p = easeCurve.Evaluate(Mathf.Clamp01(t / wipeDurationSeconds));
            insidePanels[currentIndex].transform.localScale = new Vector3(1f - p, 1f, 1f);
            yield return null;
        }
        insidePanels[currentIndex].transform.localScale = Vector3.zero;
        panels[currentIndex].SetActive(false);

        // Panel siguiente expande de 0 a 1
        insidePanels[nextIndex].transform.localScale = new Vector3(0f, 1f, 1f);
        panels[nextIndex].SetActive(true);
        t = 0f;
        while (t < wipeDurationSeconds)
        {
            t += Time.unscaledDeltaTime;
            float p = easeCurve.Evaluate(Mathf.Clamp01(t / wipeDurationSeconds));
            insidePanels[nextIndex].transform.localScale = new Vector3(p, 1f, 1f);
            yield return null;
        }
        insidePanels[nextIndex].transform.localScale = Vector3.one;

        currentIndex = nextIndex;
        isTransitioning = false;

        //isTransitioning = true;
        //int nextIndex = (currentIndex + direction + panels.Length) % panels.Length;

        //float startX = direction > 0 ? 0f : 1f;
        //wipeCurtain.anchorMin = new Vector2(startX, 0f);
        //wipeCurtain.anchorMax = new Vector2(startX, 1f);
        //wipeCurtain.gameObject.SetActive(true);

        //float half = wipeDurationSeconds / 2f;
        //float t = 0f;

        //while (t < half)
        //{
        //    t += Time.unscaledDeltaTime; // antes: Time.deltaTime
        //    float p = Mathf.Clamp01(t / half);
        //    if (direction > 0)
        //        wipeCurtain.anchorMax = new Vector2(p, 1f);
        //    else
        //        wipeCurtain.anchorMin = new Vector2(1f - p, 0f);
        //    yield return null;
        //}

        //panels[currentIndex].SetActive(false);
        //panels[nextIndex].SetActive(true);
        //currentIndex = nextIndex;

        //t = 0f;
        //while (t < half)
        //{
        //    t += Time.unscaledDeltaTime; // antes: Time.deltaTime
        //    float p = Mathf.Clamp01(t / half);
        //    if (direction > 0)
        //        wipeCurtain.anchorMin = new Vector2(p, 0f);
        //    else
        //        wipeCurtain.anchorMax = new Vector2(1f - p, 1f);
        //    yield return null;
        //}

        //wipeCurtain.gameObject.SetActive(false);
        //isTransitioning = false;
    }
}