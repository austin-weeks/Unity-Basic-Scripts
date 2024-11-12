using UnityEngine;
using UnityEngine.InputSystem;

//Be sure to setup a Cinemachine FreeLookCamera as well!
public class PlayerThirdPerson: MonoBehaviour {
    [Header("Settings")]
    //A rigid body drag of 6 or 7 works well!
    [SerializeField] float moveSpeed = 70f;
    [SerializeField] float gravity = 125f;
    [SerializeField] float jumpForce = 20f;
    [SerializeField] float jumpCooldown = 0.5f;

    [Tooltip("Jump Grace Distance determines how close to the ground the player can be before jumping.")]
    [SerializeField] float jumpGraceDistance = 0.2f;
    const float rotationSpeed = 600f;
    Rigidbody rb;
    InputAction movementInput;
    InputAction jumpInput;
    Transform cameraTransform;
    float jumpTimer;

    //Awake is performed once at the start of the game.
    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        movementInput = InputSystem.actions.FindAction("Move");
        jumpInput = InputSystem.actions.FindAction("Jump");
        cameraTransform = Camera.main.transform;
        jumpTimer = jumpCooldown;
    }

    //FixedUpdate is performed once every physics update (~60 times a second).
    void FixedUpdate() {
        HandleMovement();
        HandleJumping();
    }

    void HandleMovement() {
        Vector2 inputDir = movementInput.ReadValue<Vector2>();
        //If player is not moving, exit the function and do nothing.
        if (inputDir == Vector2.zero) {
            return;
        }

        //Calculate our movement direction using camera position and player input.
        float targetAngle = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        RotatePlayer(moveDir);

        Vector3 movement = moveSpeed * Time.fixedDeltaTime * moveDir;
        rb.linearVelocity += movement;
    }

    void RotatePlayer(Vector3 moveDir) {
        Quaternion lookRot = Quaternion.LookRotation(moveDir);
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, lookRot, rotationSpeed * Time.fixedDeltaTime);
        transform.rotation = newRotation;
    }

    void HandleJumping() {
        //Increment our jumpTimer.
        jumpTimer += Time.fixedDeltaTime;
        //Check if the player on the ground.
        bool grounded = Physics.Raycast(transform.position, Vector3.down, jumpGraceDistance);
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