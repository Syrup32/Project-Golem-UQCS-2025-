using UnityEngine;
using UnityEngine.InputSystem;

public class HotasCharacterController : MonoBehaviour
{
    public InputActionAsset inputActions;
    public Transform cameraTarget;  // This is the Cinemachine target the camera follows
    public float moveSpeed = 5f;
    public float rotationSpeed = 90f; // degrees/sec

    private CharacterController controller;
    private InputAction throttle;
    private InputAction stick;

    private float pitch;
    private float yaw;

    void OnEnable()
    {
        var map = inputActions.FindActionMap("GOLEM Controls"); // Your action map name
        throttle = map.FindAction("throttle");
        stick = map.FindAction("look"); // This should be a Vector2 (x = yaw, y = pitch)

        map.Enable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector2 lookInput = stick.ReadValue<Vector2>();
        float throttleInput = throttle.ReadValue<float>();

        // Apply movement: Negative throttle = forward
        Vector3 moveDir = transform.forward * -throttleInput * moveSpeed;
        controller.Move(moveDir * Time.deltaTime);

        // Apply rotation (X stick = yaw, Y stick = pitch)
        yaw += lookInput.x * rotationSpeed * Time.deltaTime;
        pitch += lookInput.y * rotationSpeed * Time.deltaTime;

        // Clamp pitch to avoid flipping the camera
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        // Apply to transforms
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (cameraTarget != null)
            cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
