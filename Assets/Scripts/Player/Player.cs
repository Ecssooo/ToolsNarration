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

    NpcDialogueSelector currentTarget;
    bool isInDialogue;

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
        if (isInDialogue) return;

        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 dir = new Vector3(input.x, 0f, input.y);
        cc.Move(dir * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<NpcDialogueSelector>(out var selector)) return;

        currentTarget = selector;
        Player_SetInteractPrompt(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<NpcDialogueSelector>(out var selector)) return;
        if (currentTarget != selector) return;

        currentTarget = null;

        Player_SetInteractPrompt(false);
        Player_SetDialogue(false);
        isInDialogue = false;
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (currentTarget == null) return;

        Player_SetInteractPrompt(false);
        Player_SetDialogue(true);
        isInDialogue = true;

        currentTarget.Interact();
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
