using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerFirstPerson : MonoBehaviour
{
    [Header("Controls Settings")]
    [SerializeField] float moveSpeed = 70f;
    [SerializeField] float lookSpeed = 1.6f;
    [SerializeField] float vertLookLimitDegrees = 85f;
    [SerializeField] bool invertLookYAxis = false;

    Rigidbody rb;
    InputAction lookInput;
    InputAction movementInput;
    float xRotation = 0f;

    //Awake is performed once at the start of the game.
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        if (rb.linearDamping < 5f)
        {
            Debug.LogWarning($"Rigid Body's drag is set to {rb.linearDamping}. You may want to increase this to around 6.");
        }
        movementInput = InputSystem.actions.FindAction("Move");
        lookInput = InputSystem.actions.FindAction("Look");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //FixedUpdate is performed once every physics update (~60 times a second).
    void FixedUpdate()
    {
        HandleLookingAndRotation();
        HandleMovement();
    }

    void HandleLookingAndRotation()
    {
        Vector2 inputLookDir = lookInput.ReadValue<Vector2>();
        if (inputLookDir == Vector2.zero)
        {
            return;
        }
        // X Rotation rotates around x axis -> tilts camera up and down
        // We have to use a stored xRotation variable, otherwise things get weird
        float inputXRot = inputLookDir.y * lookSpeed;
        if (!invertLookYAxis)
        {
            inputXRot *= -1;
        }
        // Add input rotation to existing rotation
        xRotation += inputXRot;
        // Clamp rotation between constraint variable
        xRotation = Mathf.Clamp(xRotation, -vertLookLimitDegrees, vertLookLimitDegrees);
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Y Rotation rotates around y axis -> turns player side to side
        float inputYRot = inputLookDir.x * lookSpeed;
        transform.rotation *= Quaternion.Euler(0, inputYRot, 0f);
    }

    void HandleMovement()
    {
        Vector2 inputDir = movementInput.ReadValue<Vector2>();
        //If player is not moving, exit the function and do nothing.
        if (inputDir == Vector2.zero)
        {
            return;
        }

        //Calculate our movement direction using current direction and player input.
        //Calculate our movement direction using camera position and player input.
        float targetAngle = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        Vector3 movement = moveSpeed * Time.fixedDeltaTime * moveDir;
        rb.linearVelocity += movement;

    }
}