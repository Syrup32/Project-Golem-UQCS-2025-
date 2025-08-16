using UnityEngine;

public interface IWeapon
{
    void OnEquip(Transform firePoint, Transform projectileParent);
    void OnUnequip();

    // Input each frame
    void Tick(bool triggerHeld, bool triggerDown, bool triggerUp, float dt);

    string DisplayName { get; }
}
