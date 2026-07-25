using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [field: SerializeField]
    public string DisplayName { get; private set; } = "Object";

    [field: SerializeField, TextArea(3, 8)]
    public string Description { get; private set; }

    [Header("Pickup")]
    [SerializeField] private string itemId;

    [Header("Interaction")]
    [SerializeField] private string interactionHint = "E — Details";

    [Header("Highlight")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color highlightColor = Color.cyan;

    [Header("Buttons")]
    [SerializeField] private List<InteractionAction> actions = new();


    private Material materialInstance;
    private Color initialBaseColor;
    private string baseColorProperty;

    public virtual string InteractionHint => interactionHint;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null)
        {
            materialInstance = targetRenderer.material;
            if (materialInstance.HasProperty("_BaseColor"))
            {
                // URP Lit
                baseColorProperty = "_BaseColor";
            }
            else if (materialInstance.HasProperty("_Color"))
            {
                // Standard / Built-in / многие другие шейдеры
                baseColorProperty = "_Color";
            }

            if (!string.IsNullOrEmpty(baseColorProperty))
                initialBaseColor = materialInstance.GetColor(baseColorProperty);
        }
    }

    public virtual void Interact()
    {
        InteractionModal.Instance.Open(this);
    }

    public IReadOnlyList<InteractionAction> GetActions()
    {
        return actions;
    }

    public void ExecuteAction(InteractionAction action)
    {
        switch (action.ActionType)
        {
            case InteractionActionType.Take:
                PlayerInventory.Instance.AddItem(itemId);
                action.InvokeCustomAction();
                gameObject.SetActive(false);
                break;

            case InteractionActionType.Use:
            case InteractionActionType.Custom:
                action.InvokeCustomAction();
                break;
        }

        if (action.CloseAfterAction || action.ActionType == InteractionActionType.Close)
            InteractionModal.Instance.Close();
    }

    public void SetHighlighted(bool isHighlighted)
    {
        if (materialInstance == null || string.IsNullOrEmpty(baseColorProperty))
            return;

        Color targetColor = isHighlighted
            ? Color.Lerp(initialBaseColor, highlightColor, 0.75f)
            : initialBaseColor;

        materialInstance.SetColor(baseColorProperty, targetColor);
    }
}