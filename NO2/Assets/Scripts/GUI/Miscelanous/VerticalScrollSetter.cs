using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VerticalScrollSetter : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(SetScrollToTop());
    }

    private IEnumerator SetScrollToTop()
    {
        yield return new WaitForEndOfFrame();
        GetComponent<Scrollbar>().value = 1f;
    }
    
}
