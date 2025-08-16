using UnityEngine;

public class Weapon_RapidFire : MonoBehaviour, IWeapon
{
    [Header("Rifle")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 55f;
    public float fireRate = 12f;        // shots per second
    public float spreadDegrees = 1.2f;  // small bloom
    public AudioSource sfx;
    public AudioClip shot;

    Transform muzzle, parent;
    float fireTimer;

    public string DisplayName => "Rifle";

    public void OnEquip(Transform firePoint, Transform projectileParent)
    {
        muzzle = firePoint; parent = projectileParent;
        fireTimer = 0f;
        gameObject.SetActive(true);
    }

    public void OnUnequip() => gameObject.SetActive(false);

    public void Tick(bool held, bool down, bool up, float dt)
    {
        fireTimer -= dt;
        if (held && fireTimer <= 0f)
        {
            Fire();
            fireTimer = 1f / Mathf.Max(0.01f, fireRate);
        }
    }

    void Fire()
    {
        Vector3 dir = muzzle.forward;
        dir = Quaternion.Euler(Random.Range(-spreadDegrees, spreadDegrees),
                               Random.Range(-spreadDegrees, spreadDegrees), 0f) * dir;

        var rot = Quaternion.LookRotation(dir, Vector3.up);
        var go = Instantiate(projectilePrefab, muzzle.position, rot, parent);
        var rb = go.GetComponent<Rigidbody>();
        if (rb) rb.linearVelocity = dir * projectileSpeed;  // rb.velocity if needed
        if (sfx && shot) sfx.PlayOneShot(shot);
    }
}
