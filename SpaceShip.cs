using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceShip : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] float moveSpeed = 65f;
    [SerializeField] float turnSpeed = 1f;
    [SerializeField] float rollSpeed = 0.15f;
    [SerializeField] bool invertYAxis = false;
    [SerializeField] bool lockCursor = false;
    [SerializeField] float drag = 4f;
    [SerializeField] float angularDrag = 1.5f;
    Rigidbody rb;
    InputAction movementInput;
    InputAction accelerateInput;

    void Awake() {
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
        }
        rb = GetComponent<Rigidbody>();
        if (rb == null) {
            Debug.LogWarning("Don't forget to add a rigid body to the vehicle!");
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        movementInput = InputSystem.actions.FindAction("Move", true);
        accelerateInput = InputSystem.actions.FindAction("Interact", true);
    }

    //FixedUpdate is performed once every physics update (~60 times a second).
    void FixedUpdate() {
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;
        float accel = accelerateInput.ReadValue<float>();
        //If player is not moving forward, exit the function and do nothing.
        if (accel < 0.1f) {
            return;
        }
        Vector2 inputDir = movementInput.ReadValue<Vector2>();

        //Move the spaceship forward based on our moveSpeed variable.
        rb.AddForce(transform.TransformDirection(Vector3.forward) * moveSpeed, ForceMode.Acceleration);

        //Turn the spaceship based on the player's input and the rotationSpeed variable.
        //X Axis Turning
        float inversionMultiplier = invertYAxis ? -1f : 1f;
        rb.AddTorque(transform.right * turnSpeed * inputDir.y * inversionMultiplier, ForceMode.Force);
        //Y Axis Turning
        rb.AddTorque(transform.up * turnSpeed * inputDir.x, ForceMode.Force);

        //Roll the spaceship
        float rollDir = 0f;
        if (Input.GetKey(KeyCode.A)) rollDir += 1f;
        if (Input.GetKey(KeyCode.D)) rollDir -= 1f;
        rb.AddTorque(transform.forward * rollSpeed * rollDir, ForceMode.VelocityChange);
    }
}
