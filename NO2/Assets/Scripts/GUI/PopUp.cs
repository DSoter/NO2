using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class PopUp : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private void Awake()
    {
        confirmButton.onClick.AddListener(Hide);
        cancelButton.onClick.AddListener(Hide);
        panel.SetActive(false);
    }

    public void Show(string message, UnityAction onConfirm, UnityAction onCancel = null)
    {
        messageText.text = message;
        panel.SetActive(true);

        // Limpiar listeners anteriores
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        confirmButton.onClick.AddListener(() => { onConfirm?.Invoke(); Hide(); });
        cancelButton.onClick.AddListener(() => { onCancel?.Invoke(); Hide(); });
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}