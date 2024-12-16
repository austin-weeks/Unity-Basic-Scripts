using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerJump : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float gravity = 125f;
    [SerializeField] float jumpForce = 20f;
    [SerializeField] float jumpCooldown = 0.5f;
    [Tooltip("Jump Grace Distance determines how close to the ground the player can be before jumping.")]
    [SerializeField] float jumpGraceDistance = 0.25f;
    float jumpTimer = 0f;
    InputAction jumpInput;
    Rigidbody rb;

    void Awake() {
        jumpInput = InputSystem.actions.FindAction("Jump", true);
        jumpTimer = jumpCooldown;
        rb = GetComponent<Rigidbody>();
        if (rb == null) {
            Debug.LogError("No Rigid Body on object with PlayerJump component.");
        }
    }

    void FixedUpdate() {
        HandleJumping();
    }

    void HandleJumping()
    {
        //Increment our jumpTimer.
        jumpTimer += Time.fixedDeltaTime;
        //Check if the player on the ground.
        //radius is used as spherecast radius & offsets position
        const float radius = 0.15f;
        Vector3 castStart = new Vector3(transform.position.x, transform.position.y + radius + 0.1f, transform.position.z);
        // bool grounded = Physics.Raycast(rayStart, Vector3.down, jumpGraceDistance);
        bool grounded = Physics.SphereCast(castStart, radius, Vector3.down, out _, jumpGraceDistance + radius);
        print(grounded);
        if (!grounded)
        {
            if (rb.linearVelocity.y < 0.1f)
            {
                Vector3 gravForce = gravity * Time.fixedDeltaTime * Vector3.down;
                rb.linearVelocity += gravForce;
            }
            return;
        }
        //Check if we can ump again.
        if (jumpTimer < jumpCooldown) return;
        float jumpInputVal = jumpInput.ReadValue<float>();
        //If player is pressing jump button, jump!
        if (jumpInputVal > 0.1f)
        {
            //reset vertical velocity
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            jumpTimer = 0f;
            Vector3 force = Vector3.up * jumpForce;
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

}