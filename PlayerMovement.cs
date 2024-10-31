using UnityEngine;
using UnityEngine.InputSystem;

public enum GameType { Game3D, Game2D }
public class PlayerMovement : MonoBehaviour {
  [SerializeField] float moveSpeed = 10;
  [SerializeField] GameType gameType;
  InputAction movementInput;

  void Start() {
    movementInput = InputSystem.actions.FindAction("Move");
  }

  void Update() {
    Vector2 inputDir = movementInput.ReadValue<Vector2>();

    Vector3 moveDir;
    if (gameType == GameType.Game3D) {
      moveDir = new Vector3(inputDir.x, 0f, inputDir.y);
    } else {
      moveDir = new Vector3(inputDir.x, inputDir.y, 0f);
    }
    moveDir.Normalize();

    print("Movement Vector3: " + moveDir);

    float adjustedSpeed = moveSpeed * Time.deltaTime;

    Vector3 movement = moveDir * adjustedSpeed;

    transform.position += movement;
  }
}
