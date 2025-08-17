using UnityEngine;

public class PlasmaCannonWeapon : WeaponBase, IWeapon
{
    public GameObject projectilePrefab;
    public AnimationCurve sizeByCharge = AnimationCurve.Linear(0, 1, 1, 2);
    public float minChargeTime = 0.4f;
    public float maxChargeTime = 3f;
    public float projectileSpeed = 20f;
    public float cooldown = 3f;

    public GameObject chargingPreviewPrefab;
    private GameObject currentPreview;

    private float chargeStartTime;
    private bool isCharging = false;
    private bool canFire = true;

    private bool isEquipped = false;

    public string DisplayName => "Plasma Cannon";

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
        if (!isEquipped || !canFire) return;

        if (down)
            StartCharging();

        if (held && isCharging)
            UpdateCharging();

        if (up && isCharging)
            Fire();
    }

    void StartCharging()
    {
        isCharging = true;
        chargeStartTime = Time.time;
        currentPreview = Instantiate(chargingPreviewPrefab, firePoint.position, firePoint.rotation, firePoint);
        currentPreview.transform.localScale = Vector3.zero;
    }

    void UpdateCharging()
    {
        float chargeDuration = Mathf.Clamp(Time.time - chargeStartTime, 0, maxChargeTime);
        float scale = sizeByCharge.Evaluate(chargeDuration / maxChargeTime);
        if (currentPreview != null)
        currentPreview.transform.localScale = Vector3.one * scale;
        // Optional: add charging VFX, sounds, UI etc. here
    }

    public override void Fire()
    {
        if (currentPreview != null) Destroy(currentPreview);
        isCharging = false;
        canFire = false;

        float chargeTime = Mathf.Clamp(Time.time - chargeStartTime, 0, maxChargeTime);
        float scale = sizeByCharge.Evaluate(chargeTime / maxChargeTime);

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        proj.transform.localScale *= scale;

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * projectileSpeed;
        }

        float scaledDamage = damage * scale;
        proj.GetComponent<PlayerProjectile>().Initialize(scaledDamage);

        Invoke(nameof(ResetCooldown), cooldown);
    }

    void ResetCooldown()
    {
        canFire = true;
    }
}
