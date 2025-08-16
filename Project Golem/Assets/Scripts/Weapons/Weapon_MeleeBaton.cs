using UnityEngine;

public class Weapon_MeleeBaton : MonoBehaviour, IWeapon
{
    [Header("Melee")]
    public float swingRate = 2.5f;          // swings per second
    public float range = 2.2f;
    public float radius = 0.6f;
    public int damage = 30;
    public LayerMask hitMask;               // set to Enemy in Inspector (or leave blank to use default from switcher)
    public AudioSource sfx;
    public AudioClip swingClip;
    public AudioClip hitClip;

    Transform origin;
    float cooldown;

    public string DisplayName => "Baton";

    public void OnEquip(Transform firePoint, Transform projectileParent)
    {
        origin = firePoint; // use same "muzzle" as approximate hand/forward
        cooldown = 0f;
        gameObject.SetActive(true);
    }
    public void OnUnequip() => gameObject.SetActive(false);

    public void Tick(bool held, bool down, bool up, float dt)
    {
        cooldown -= dt;
        if (down && cooldown <= 0f)
        {
            DoSwing();
            cooldown = 1f / Mathf.Max(0.01f, swingRate);
        }
    }

    void DoSwing()
    {
        Debug.Log("Swung!");
        if (sfx && swingClip) sfx.PlayOneShot(swingClip);

        Vector3 start = origin.position;
        Vector3 dir   = origin.forward;

        // sphere cast in front
        if (Physics.SphereCast(start, radius, dir, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            // Try damage via your existing enemy script first
            var enemy = hit.collider.GetComponentInParent<EnemyAiTutorial>();
            if (enemy) { enemy.TakeDamage(damage); if (sfx && hitClip) sfx.PlayOneShot(hitClip); return; }

            // Or a generic interface if you add one later:
            //var dmg = hit.collider.GetComponentInParent<IDamageable>();
            //if (dmg != null) { dmg.TakeDamage(damage); if (sfx && hitClip) sfx.PlayOneShot(hitClip); }
        }
    }
}
