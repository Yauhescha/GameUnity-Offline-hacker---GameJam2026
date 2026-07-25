using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float mouseSensitivity = 2f;
    private float cameraPitch = 0f;


    [SerializeField] private Camera playerCamera;
    [SerializeField] private float maxDistance = 2.2f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private InteractionHintUI hintUI;

    [Header("Debug")]
    [SerializeField] private bool showDebugRay = true;
    [SerializeField] private float sphereCastRadius = 0.08f;

    private Interactable current;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Блокируем курсор по центру экрана
    }

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera == null)
            Debug.LogError("PlayerInteractor: камера не найдена среди дочерних объектов Player.", this);
    }

    void Update()
    {
        if ((InteractionModal.Instance != null && InteractionModal.Instance.IsOpen) ||
            (Shindows9UI.Instance != null && Shindows9UI.Instance.IsOpen) ||
            (EndGameManager.Instance != null && EndGameManager.Instance.IsFinished))
        {
            GameAudio.Instance?.SetFootstepsPlaying(false);
            return;
        }


        // Движение вперед/назад и в бока (W, A, S, D)
        float moveH = Input.GetAxis("Horizontal");
        float moveV = Input.GetAxis("Vertical");
        Vector3 moveDir = transform.right * moveH + transform.forward * moveV;
        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        GameAudio.Instance?.SetFootstepsPlaying(
            moveDir.sqrMagnitude > 0.01f
        );

        // Вращение камеры мышей (вверх/вниз) и поворот персонажа (влево/вправо)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(0, mouseX, 0); // Поворот самого персонажа

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -50f, 50f); // Ограничение угла обзора камеры
        playerCamera.transform.localEulerAngles = new Vector3(cameraPitch, 0, 0);



        FindInteractable();

        if (current != null && Input.GetKeyDown(KeyCode.E)) {
            current.Interact();
        }
    }

    private void OnDisable()
    {
        GameAudio.Instance?.SetFootstepsPlaying(false);
    }

    private void FindInteractable()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        Interactable next = null;

        bool hitSomething = Physics.SphereCast(
            ray,
            sphereCastRadius,
            out RaycastHit hit,
            maxDistance,
            interactableLayer
        );

        if (hitSomething)
            next = hit.collider.GetComponentInParent<Interactable>();

        if (showDebugRay)
        {
            Debug.DrawRay(
                ray.origin,
                ray.direction * maxDistance,
                next != null ? Color.green : Color.red
            );
        }

        if (next == current)
            return;

        if (current != null)
            current.SetHighlighted(false);

        current = next;

        if (current != null)
        {
            current.SetHighlighted(true);
            hintUI.Show(current.DisplayName, current.InteractionHint);
        }
        else
        {
            hintUI.Hide();
        }
    }
}
