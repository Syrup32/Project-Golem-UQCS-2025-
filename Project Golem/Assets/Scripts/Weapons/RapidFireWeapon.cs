using UnityEngine;

public class RapidFireWeapon : WeaponBase, IWeapon
{
    public GameObject projectilePrefab;
    public float fireRate = 0.1f;

    private float nextFireTime = 0f;
    private bool isEquipped = false;

    public string DisplayName => "Rapid Fire";

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
        if (!isEquipped) return;

        if (held && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    public override void Fire()
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        proj.GetComponent<PlayerProjectile>().Initialize(damage);

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * 50f;  // Make sure it's `velocity`, not `linearVelocity`
        }
    }
}
