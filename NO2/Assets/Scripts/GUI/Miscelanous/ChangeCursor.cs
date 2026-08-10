using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Texture2D cursorIdle;
    [SerializeField] private Texture2D cursorCanClick;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cursor.SetCursor(cursorCanClick, hotSpot, cursorMode);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        // Pass 'null' to the texture parameter to use the default system cursor.
        Cursor.SetCursor(cursorIdle, Vector2.zero, cursorMode);
    }

}
