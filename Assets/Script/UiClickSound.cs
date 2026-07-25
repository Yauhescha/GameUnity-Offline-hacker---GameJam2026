using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UiClickSound :  MonoBehaviour, IPointerClickHandler
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button != null && !button.interactable)
            return;

        GameAudio.Instance?.PlayUiClick();
    }
}
