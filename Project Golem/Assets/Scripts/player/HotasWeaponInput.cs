using UnityEngine;
using UnityEngine.InputSystem;

public class HotasWeaponInput : MonoBehaviour
{
    [Header("Actions (assign InputActionReferences from your asset)")]
    public InputActionReference trigger;       // GOLEM Controls/trigger
    public InputActionReference nextWeapon;    // optional
    public InputActionReference prevWeapon;    // optional
    public InputActionReference slot1;         // optional
    public InputActionReference slot2;         // optional
    public InputActionReference slot3;         // optional

    // Backing fields
    bool _triggerHeld, _triggerDown, _triggerUp;
    bool _nextPressed, _prevPressed, _slot1Pressed, _slot2Pressed, _slot3Pressed;

    // Public read-only properties
    public bool TriggerHeld  => _triggerHeld;
    public bool TriggerDown  => _triggerDown;
    public bool TriggerUp    => _triggerUp;
    public bool NextPressed  => _nextPressed;
    public bool PrevPressed  => _prevPressed;
    public bool Slot1Pressed => _slot1Pressed;
    public bool Slot2Pressed => _slot2Pressed;
    public bool Slot3Pressed => _slot3Pressed;

    void OnEnable()
    {
        Enable(trigger);
        Enable(nextWeapon);
        Enable(prevWeapon);
        Enable(slot1);
        Enable(slot2);
        Enable(slot3);
    }

    void OnDisable()
    {
        Disable(trigger);
        Disable(nextWeapon);
        Disable(prevWeapon);
        Disable(slot1);
        Disable(slot2);
        Disable(slot3);
    }

    void Update()
    {
        // reset edge flags each frame
        _triggerDown = _triggerUp = false;
        _nextPressed = _prevPressed = false;
        _slot1Pressed = _slot2Pressed = _slot3Pressed = false;

        // Trigger states
        if (trigger && trigger.action != null)
        {
            var a = trigger.action;
            _triggerHeld = a.IsPressed();
            if (a.WasPressedThisFrame())  _triggerDown = true;
            if (a.WasReleasedThisFrame()) _triggerUp   = true;
        }
        else
        {
            _triggerHeld = false;
        }

        // One-shot presses
        _nextPressed  = WasPressed(nextWeapon);
        _prevPressed  = WasPressed(prevWeapon);
        _slot1Pressed = WasPressed(slot1);
        _slot2Pressed = WasPressed(slot2);
        _slot3Pressed = WasPressed(slot3);
    }

    static bool WasPressed(InputActionReference aref)
        => aref && aref.action != null && aref.action.WasPressedThisFrame();

    static void Enable(InputActionReference aref)
    {
        if (aref != null && aref.action != null) aref.action.Enable();
    }
    static void Disable(InputActionReference aref)
    {
        if (aref != null && aref.action != null) aref.action.Disable();
    }
}
