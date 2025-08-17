using UnityEngine;

public class MeleeBatonWeapon : WeaponBase, IWeapon
{
    public float range = 2f;
    public float cooldown = 0.5f;
    public LayerMask hitMask;

    private bool isEquipped = false;
    private bool canSwing = true;

    public string DisplayName => "Melee Baton";

    public void OnEquip(Transform firePoint, Transform projectileParent)
    {
        this.firePoint = firePoint;
        isEquipped = true;
    }

    public void OnUnequip()
    {
        isEquipped = false;
    }

    public void Tick(bool held, bool down, bool up, float deltaTime)
    {
        if (!isEquipped || !canSwing) return;

        if (down)
        {
            Swing();
        }
    }

    void Swing()
    {
        canSwing = false;

        // Draw debug line in Scene view
        Debug.DrawRay(firePoint.position, firePoint.forward * range, Color.red, 1f);
        
        RaycastHit[] hits = Physics.RaycastAll(firePoint.position, firePoint.forward, range, hitMask);
        bool hitAnything = false;

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent(out EnemyAiTutorial enemy))
            {
                enemy.TakeDamage((int)damage);
                Debug.Log($"[Baton] Multi-hit: {enemy.name}, damage: {damage}");
                hitAnything = true;
            }
        }

        if (!hitAnything)
        {
            Debug.Log("Melee raycast missed everything.");
        }
        
        
        Invoke(nameof(ResetCooldown), cooldown);
    }

    void ResetCooldown()
    {
        canSwing = true;
    }
    public override void Fire() { }
}
