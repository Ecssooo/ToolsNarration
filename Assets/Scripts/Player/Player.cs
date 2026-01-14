using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] float speed;

    CharacterController cc;
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction interactAction;

    PNJ current;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        interactAction = playerInput.actions["Interract"];
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
        var zone = other.GetComponent<PNJ>();
        if (zone == null) return;

        current = zone;
        current.ShowInterract(true);
    }

    void OnTriggerExit(Collider other)
    {
        var zone = other.GetComponent<PNJ>();
        if (zone == null) return;

        if (current == zone)
        {
            current.ShowInterract(false);
            current.HideDialogue(false);
            current = null;
        }
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (current == null) return;
        current.DoInteract();
    }
}