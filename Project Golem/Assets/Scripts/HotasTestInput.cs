using UnityEngine;
using UnityEngine.InputSystem;

public class HotasTestInput : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    private InputAction pitch;
    private InputAction roll;
    private InputAction throttle;

    void OnEnable()
    {
        var map = inputActions.FindActionMap("GOLEM Controls"); // Use your Action Map name

        roll = map.FindAction("hori-look");
        pitch = map.FindAction("vert-look");
        throttle = map.FindAction("throttle");

        map.Enable();
    }

    void Update()
    {
        float pitchVal = pitch.ReadValue<float>();
        float rollVal = roll.ReadValue<float>();
        float throttleVal = throttle.ReadValue<float>();

        //Debug.Log($"Pitch: {pitchVal} | Roll: {rollVal} | Throttle: {throttleVal}");
    }
}
