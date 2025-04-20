using Articy.Unity;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public Transform cameraTransform; // Assign your camera in the Inspector.

    [Header("Ground Detection Settings")]
    [Tooltip("How far down to check for ground")]
    public float groundCheckDistance = 1.5f;

    [Tooltip("Extra offset above the ground")]
    public float groundOffset = 0.1f;

    [Tooltip("Maximum angle (in degrees) considered walkable (e.g., stairs, slopes)")]
    public float maxSlopeAngle = 45f;

    private Rigidbody rb;
    private float colliderHeightOffset; // Cached half-height of the collider

    // Articy vars
    private ArticyObject availableDialogue;
    private DialogueManager dialogueManager;

    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        rb = GetComponent<Rigidbody>();

        // Cache the collider's vertical extent (half-height)
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            colliderHeightOffset = col.bounds.extents.y;
        }
        else
        {
            colliderHeightOffset = 0.5f; // Fallback value if no collider is found.
        }
    }

    void Update()
    {
        // Articy
        DialogueInteraction();
    }

    void FixedUpdate()
    {
        if (dialogueManager.DialogueActive)
        {
            // Disable movement when dialogue is active.
            return;
        }
        // Get input for movement.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Determine camera-relative movement directions.
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        // Build the movement vector relative to the camera.
        Vector3 move = (camForward * vertical + camRight * horizontal);
        Vector3 targetPos = transform.position + move * speed * Time.fixedDeltaTime;

        // --- Ground Snapping ---
        RaycastHit hit;
        // Cast a ray downwards from the player's center.
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance))
        {
            // Calculate the slope of the surface.
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            // Only adjust if the slope is gentle enough.
            if (slopeAngle <= maxSlopeAngle)
            {
                // Adjust target Y so the bottom of the capsule sits slightly above the ground.
                targetPos.y = hit.point.y + colliderHeightOffset + groundOffset;
            }
        }
        // Move the player while preserving collisions.
        rb.MovePosition(targetPos);
    }

    // Articy interactions
    void DialogueInteraction()
    {
        // Key option to start dialogue when near NPC
        if (Input.GetButtonDown("Interact") && availableDialogue != null)
        {
            dialogueManager.StartDialogue(availableDialogue);
        }

        // Key option to abort dialogue
        if (dialogueManager.DialogueActive && Input.GetButtonDown("Cancel"))
        {
            dialogueManager.CloseDialogueBox();
        }
    }

    void OnTriggerEnter(Collider aOther)
    {
        var articyReferenceComp = aOther.GetComponent<ArticyReference>();
        if (articyReferenceComp)
        {
            availableDialogue = articyReferenceComp.reference.GetObject();
        }
    }

    void OnTriggerExit(Collider aOther)
    {
        if (aOther.GetComponent<ArticyReference>() != null)
        {
            availableDialogue = null;
        }
    }
}
