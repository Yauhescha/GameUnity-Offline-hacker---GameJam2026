using System;
using UnityEngine;
using UnityEngine.Events;

public enum InteractionActionType
{
    Close,
    Take,
    Use,
    Custom
}

[Serializable]
public class InteractionAction
{
    [SerializeField] private string buttonText = "Close";
    [SerializeField] private InteractionActionType actionType = InteractionActionType.Close;

    [Tooltip("Закрывать ли окно после нажатия кнопки.")]
    [SerializeField] private bool closeAfterAction = true;

    [Tooltip("Сюда можно назначить действие в Inspector: открыть компьютер, дверь и т.п.")]
    [SerializeField] private UnityEvent onAction;

    public string ButtonText => buttonText;
    public InteractionActionType ActionType => actionType;
    public bool CloseAfterAction => closeAfterAction;

    public void InvokeCustomAction()
    {
        onAction?.Invoke();
    }
}