using UnityEngine;
using UnityEngine.InputSystem;

//Be sure to setup a Cinemachine FreeLookCamera as well!
public class Vehicle: MonoBehaviour {
    [Header("Settings")]
    [SerializeField] float moveSpeed = 65f;
    [SerializeField] float drag = 4f;
    [SerializeField] float rotationSpeed = 200f;
    Rigidbody rb;
    InputAction movementInput;
    Transform cameraTransform;

    //Awake is performed once at the start of the game.
    void Awake() {
        rb = GetComponent<Rigidbody>();
        if (rb == null) {
            Debug.LogWarning("Don't forget to add a rigid body to the vehicle!");
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true;
        movementInput = InputSystem.actions.FindAction("Move", true);
        cameraTransform = Camera.main.transform;
    }

    //FixedUpdate is performed once every physics update (~60 times a second).
    void FixedUpdate() {
        rb.linearDamping = drag;
        Vector2 inputDir = movementInput.ReadValue<Vector2>();
        //If player is not moving, exit the function and do nothing.
        if (inputDir == Vector2.zero) {
            return;
        }

        //Calculate our movement direction using camera position and player input.
        float targetAngle = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        RotatePlayer(moveDir);

        Vector3 movement = moveSpeed * Time.fixedDeltaTime * transform.forward;
        rb.linearVelocity += movement;
    }

    void RotatePlayer(Vector3 moveDir) {
        Quaternion lookRot = Quaternion.LookRotation(moveDir);
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, lookRot, rotationSpeed * Time.fixedDeltaTime);
        transform.rotation = newRotation;
    }
}
