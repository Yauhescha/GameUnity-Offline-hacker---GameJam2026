using System.Collections;
using UnityEngine;

public class OfficeDoor : Interactable
{
    [Header("Door")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openRotationY = 90f; // по часовой стрелке
    [SerializeField] private Vector3 openPositionOffset = new(0.5f, 0f, 0.5f);
    [SerializeField] private float openingSpeed = 4f;

    [Header("Lock")]
    [SerializeField] private bool isLocked;
    [SerializeField] private string requiredItemId = "manager_keycard";
    [SerializeField, TextArea]
    private string lockedMessage =
        "The manager office is locked. You need a keycard.";

    private bool isOpen;
    private bool isAnimating;
    private Quaternion closedRotation;
    private Quaternion openedRotation;
    private Vector3 closedLocalPosition;
    private Vector3 openedLocalPosition;


       private void Start()
    {
        if (doorPivot == null)
            doorPivot = transform;

        closedLocalPosition = doorPivot.localPosition;
        openedLocalPosition = closedLocalPosition + openPositionOffset;

        closedRotation = doorPivot.localRotation;
        openedRotation = closedRotation * Quaternion.Euler(0f, openRotationY, 0f);
    
    }

    public override string InteractionHint
    {
        get
        {
            if (isAnimating)
                return "";

            return isOpen ? "E — Close" : "E — Open";
        }
    }

    public override void Interact()
    {
        if (isAnimating)
            return;

        if (isLocked && !PlayerInventory.Instance.HasItem(requiredItemId))
        {
            InteractionModal.Instance.ShowMessage(
                DisplayName,
                lockedMessage
            );

            return;
        }

        bool shouldOpen = !isOpen;

        if (shouldOpen)
            GameAudio.Instance?.PlayDoorOpen(doorPivot.position);
        else
            GameAudio.Instance?.PlayDoorClose(doorPivot.position);

        StartCoroutine(AnimateDoor(shouldOpen));
    }

    private IEnumerator AnimateDoor(bool shouldOpen)
    {
        isAnimating = true;

        Vector3 fromPosition = doorPivot.localPosition;
        Vector3 toPosition = shouldOpen
            ? openedLocalPosition
            : closedLocalPosition;

        Quaternion fromRotation = doorPivot.localRotation;
        Quaternion toRotation = shouldOpen
            ? openedRotation
            : closedRotation;

        float progress = 0f;

        while (progress < 1f)
        {
            progress += Time.deltaTime * openingSpeed;

            doorPivot.localPosition  = Vector3.Lerp(
                fromPosition,
                toPosition,
                progress
            );

            doorPivot.localRotation = Quaternion.Slerp(
                fromRotation,
                toRotation,
                progress
            );

            yield return null;
        }

        doorPivot.localPosition = toPosition;
        doorPivot.localRotation = toRotation;

        isOpen = shouldOpen;
        isAnimating = false;
    }
}
