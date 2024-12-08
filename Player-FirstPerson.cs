using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof (Rigidbody))]
public class PlayerFirstPerson: MonoBehaviour {
    [Header("Controls Settings")]
    [SerializeField] float moveSpeed = 70f;
    [SerializeField] float lookSpeed = 1.6f;
    [SerializeField] float vertLookLimitDegrees = 85f;
    [SerializeField] bool invertLookYAxis = false;
    [Header("Jump Settings")]
    [SerializeField] float gravity = 125f;
    [SerializeField] float jumpForce = 20f;
    [SerializeField] float jumpCooldown = 0.5f;

    [Tooltip("Jump Grace Distance determines how close to the ground the player can be before jumping.")]
    [SerializeField] float jumpGraceDistance = 0.3f;
    const float rotationSpeed = 600f;
    Rigidbody rb;
    InputAction lookInput;
    InputAction movementInput;
    InputAction jumpInput;
    Transform camTransform;
    float jumpTimer = 0f;
    float xRotation = 0f;

    //Awake is performed once at the start of the game.
    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        if (rb.linearDamping < 5f) {
            Debug.LogWarning($"Rigid Body's drag is set to {rb.linearDamping}. You may want to increase this to around 6.");
        }
        movementInput = InputSystem.actions.FindAction("Move");
        jumpInput = InputSystem.actions.FindAction("Jump");
        lookInput = InputSystem.actions.FindAction("Look");
        if (movementInput == null || jumpInput == null || lookInput == null) {
            throw new System.Exception("Cannot find required Input Actions!");
        }
        if (Camera.main != null) {
            camTransform = Camera.main.transform;
        }
        if (camTransform == null) {
            camTransform = FindFirstObjectByType<Camera>().transform;
            if (camTransform == null) {
                throw new System.Exception("Cannot find Camera. Please ensure there is camera in the game with the tag 'Main Camera'");
            }
        }
        jumpTimer = jumpCooldown;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //FixedUpdate is performed once every physics update (~60 times a second).
    void FixedUpdate() {
        HandleLookingAndRotation();
        HandleMovement();
        HandleJumping();
    }

    void HandleLookingAndRotation() {
        Vector2 inputLookDir = lookInput.ReadValue<Vector2>();
        if (inputLookDir == Vector2.zero) {
            return;
        }
        // X Rotation rotates around x axis -> tilts camera up and down
        // We have to use a stored xRotation variable, otherwise things get weird
        float inputXRot = inputLookDir.y * lookSpeed;
        if (!invertLookYAxis) {
            inputXRot *= -1;
        }
        // Add input rotation to existing rotation
        xRotation += inputXRot;
        // Clamp rotation between constraint variable
        xRotation = Mathf.Clamp(xRotation, -vertLookLimitDegrees, vertLookLimitDegrees);
        camTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Y Rotation rotates around y axis -> turns player side to side
        float inputYRot = inputLookDir.x * lookSpeed;
        transform.rotation *= Quaternion.Euler(0, inputYRot, 0f);
    }

    void HandleMovement() {
        Vector2 inputDir = movementInput.ReadValue<Vector2>();
        //If player is not moving, exit the function and do nothing.
        if (inputDir == Vector2.zero) {
            return;
        }

        //Calculate our movement direction using current direction and player input.
        //Calculate our movement direction using camera position and player input.
        float targetAngle = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        Vector3 movement = moveSpeed * Time.fixedDeltaTime * moveDir;
        rb.linearVelocity += movement;

    }

    void HandleJumping() {
        //Increment our jumpTimer.
        jumpTimer += Time.fixedDeltaTime;
        //Check if the player on the ground.
        Vector3 rayStartPos = transform.position;
        rayStartPos.y += 0.1f;
        bool grounded = Physics.Raycast(rayStartPos, Vector3.down, jumpGraceDistance);
        if (!grounded) {
            if (rb.linearVelocity.y < 0.1f) {
                Vector3 gravForce = gravity * Time.fixedDeltaTime * Vector3.down;
                rb.linearVelocity += gravForce;
            }
            return;
        }
        //Check if we can ump again.
        if (jumpTimer < jumpCooldown) return;
        float jumpInputVal = jumpInput.ReadValue<float>();
        //If player is pressing jump button, jump!
        if (jumpInputVal > 0.1f) {
            jumpTimer = 0f;
            Vector3 force = Vector3.up * jumpForce;
            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}