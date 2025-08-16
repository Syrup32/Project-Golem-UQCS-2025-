using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 40f;
    public float fireRate = 0.25f;
    private float fireCooldown = 0f;

#if ENABLE_INPUT_SYSTEM
    public InputActionAsset hotasInputActions;
    private InputAction hotasFireAction;
#endif

    private void Start()
    {
#if ENABLE_INPUT_SYSTEM
        if (hotasInputActions != null)
        {
            var map = hotasInputActions.FindActionMap("GOLEM Controls"); // Replace with your map name
            if (map != null)
            {
                hotasFireAction = map.FindAction("fire"); // Replace with your HOTAS fire action name
                hotasFireAction.Enable();
            }
        }
#endif
    }

    private void Update()
    {
        fireCooldown -= Time.deltaTime;

        bool isMouseFiring = Input.GetButton("Fire1");
        bool isHotasFiring = false;

#if ENABLE_INPUT_SYSTEM
        if (hotasFireAction != null)
        {
            isHotasFiring = hotasFireAction.ReadValue<float>() > 0.5f;
        }
#endif

        if ((isMouseFiring || isHotasFiring) && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = fireRate;
        }
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * projectileSpeed;
    }
}
