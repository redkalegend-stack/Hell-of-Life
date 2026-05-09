using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {
    private CameraController cameraController;

    private Rigidbody2D rb;
    private int playerSpeed = 6;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();

        cameraController = FindAnyObjectByType<CameraController>();
    }

    private void Update() {
        if (Mouse.current.rightButton.isPressed) {
            cameraController.useLookAhead = true;
        }
        else {
            cameraController.useLookAhead = false;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame) {
            GameObject weapon = transform.GetChild(0).gameObject;
            if (weapon != null) {
                if (weapon.CompareTag("Weapon")) {
                    weapon.GetComponent<Weapon>().Shoot();
                }
            }
        }
    }

    private void LateUpdate() {
        RotateTowardsMouse();
    }

    private void FixedUpdate() {
        HandleMovement();
    }

    private void HandleMovement() {
        Vector3 moveDir = Vector3.zero;

        // Vertical Movements
        if (Keyboard.current.wKey.isPressed) moveDir += Vector3.up;
        else if (Keyboard.current.sKey.isPressed) moveDir += Vector3.down;

        // Horizontal Movement
        if (Keyboard.current.aKey.isPressed) moveDir += Vector3.left;
        else if (Keyboard.current.dKey.isPressed) moveDir += Vector3.right;

        moveDir = moveDir.normalized;
        rb.linearVelocity = moveDir * playerSpeed;
    }

    private void RotateTowardsMouse() {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 direction = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);

        transform.right = direction;
    }
}