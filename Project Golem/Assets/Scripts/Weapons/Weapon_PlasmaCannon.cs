using UnityEngine;

public class Weapon_PlasmaCannon : MonoBehaviour, IWeapon
{
    [Header("Plasma")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 40f;
    public float minChargeTime = 0.4f;
    public float maxChargeTime = 1.5f;
    public float cooldown = 0.8f;
    public AnimationCurve sizeByCharge = AnimationCurve.EaseInOut(0,1,1,2f); // scale projectile
    public AudioSource sfx;
    public AudioClip chargeLoop;
    public AudioClip fire;

    Transform muzzle, parent;
    float charge;
    float cd;

    public string DisplayName => "Plasma";

    public void OnEquip(Transform firePoint, Transform projectileParent)
    {
        muzzle = firePoint; parent = projectileParent;
        cd = 0f; charge = 0f;
        gameObject.SetActive(true);
    }
    public void OnUnequip()
    {
        StopChargeSfx();
        gameObject.SetActive(false);
    }

    public void Tick(bool held, bool down, bool up, float dt)
    {
        cd = Mathf.Max(0f, cd - dt);

        if (held && cd <= 0f)
        {
            charge = Mathf.Min(maxChargeTime, charge + dt);
            PlayChargeSfx();
        }
        if (up && cd <= 0f)
        {
            var t = Mathf.Clamp01(Mathf.Max(minChargeTime, charge) / maxChargeTime);
            Fire(t);
            charge = 0f;
            cd = cooldown;
            StopChargeSfx();
        }
        if (!held && charge > 0f && cd > 0f) { charge = 0f; StopChargeSfx(); }
    }

    void Fire(float t)
    {
        Vector3 dir = muzzle.forward;
        var rot = Quaternion.LookRotation(dir, Vector3.up);
        var go = Instantiate(projectilePrefab, muzzle.position, rot, parent);
        var rb = go.GetComponent<Rigidbody>();
        if (rb) rb.linearVelocity = dir * projectileSpeed * Mathf.Lerp(1f, 1.2f, t);

        // scale by charge for visuals/damage scripts to read if needed
        float scale = sizeByCharge.Evaluate(t);
        go.transform.localScale *= scale;

        if (sfx && fire) sfx.PlayOneShot(fire);
    }

    void PlayChargeSfx()
    {
        if (!sfx || !chargeLoop) return;
        if (!sfx.isPlaying) { sfx.clip = chargeLoop; sfx.loop = true; sfx.Play(); }
    }
    void StopChargeSfx()
    {
        if (sfx && sfx.isPlaying && sfx.clip == chargeLoop) sfx.Stop();
    }
}
