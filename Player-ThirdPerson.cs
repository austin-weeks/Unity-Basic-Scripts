using UnityEngine;
using UnityEngine.InputSystem;

//Be sure to setup a Cinemachine FreeLookCamera as well!
public class PlayerThirdPerson: MonoBehaviour {
    [Header("Settings")]
    //A rigid body drag of 6 or 7 works well!
    [SerializeField] float moveSpeed = 70f;

    const float rotationSpeed = 600f;
    Rigidbody rb;
    InputAction movementInput;
    Transform cameraTransform;

    //Awake is performed once at the start of the game.
    void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        movementInput = InputSystem.actions.FindAction("Move");
        cameraTransform = Camera.main.transform;
    }

    //FixedUpdate is performed once every physics update (~60 times a second).
    void FixedUpdate() {
        HandleMovement();
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
}