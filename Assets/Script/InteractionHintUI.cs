using TMPro;
using UnityEngine;

public class InteractionHintUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text objectNameText;
    [SerializeField] private TMP_Text actionText;

    private void Awake()
    {
        Hide();
    }

    public void Show(string objectName, string action)
    {
        root.SetActive(true);
        objectNameText.text = objectName;
        actionText.text = action;
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}