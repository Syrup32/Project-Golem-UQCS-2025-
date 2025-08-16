using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("HOTAS Input Actions")]
    public InputActionAsset inputActions;

    // HOTAS Actions
    private InputAction throttle;
    private InputAction lookHorizontal;
    private InputAction lookVertical;
    private InputAction jump;

    [Header("Input States")]
    public float throttleValue { get; private set; }
    public Vector2 lookInput { get; private set; }
    public bool jumpPressed { get; private set; }
    private bool jumpPressedThisFrame;

    private void Awake()
    {
        var map = inputActions.FindActionMap("GOLEM Controls", true);

        throttle = map.FindAction("throttle", true);
        lookHorizontal = map.FindAction("hori-look", true);
        lookVertical = map.FindAction("vert-look", true);
        jump = map.FindAction("jump", true);

        map.Enable();
    }

    private void Update()
    {
        throttleValue = throttle.ReadValue<float>();
        lookInput = new Vector2(
            lookHorizontal.ReadValue<float>(),
            lookVertical.ReadValue<float>());

        bool wasJumpPressed = jumpPressed;
        jumpPressed = jump.ReadValue<float>() > 0.5f;
        jumpPressedThisFrame = jumpPressed && !wasJumpPressed;
    }

    public bool ConsumeJumpPressed()
    {
        if (jumpPressedThisFrame)
        {
            jumpPressedThisFrame = false;
            return true;
        }
        return false;
    }
}
