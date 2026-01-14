using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed;

    [Header("Interaction UI")]
    [SerializeField] GameObject pressECanvas;
    [SerializeField] GameObject dialogueCanvas;

    CharacterController cc;
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction interactAction;

    bool hasInteractTarget;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        interactAction = playerInput.actions["Interract"];

        Player_SetInteractPrompt(false);
        Player_SetDialogue(false);
    }

    void OnEnable()
    {
        moveAction.Enable();

        interactAction.Enable();
        interactAction.performed += OnInteract;
    }

    void OnDisable()
    {
        moveAction.Disable();

        interactAction.performed -= OnInteract;
        interactAction.Disable();
    }

    void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 dir = new Vector3(input.x, 0f, input.y);
        cc.Move(dir * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PNJ>(out _)) return;

        hasInteractTarget = true;
        Player_SetInteractPrompt(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<PNJ>(out _)) return;

        hasInteractTarget = false;
        Player_SetInteractPrompt(false);
        Player_SetDialogue(false);
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!hasInteractTarget) return;

        Player_SetInteractPrompt(false);
        Player_SetDialogue(true);
    }

    void Player_SetInteractPrompt(bool show)
    {
        if (pressECanvas != null) pressECanvas.SetActive(show);
    }

    void Player_SetDialogue(bool show)
    {
        if (dialogueCanvas != null) dialogueCanvas.SetActive(show);
    }
}
