using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public float damage = 10f; // Default value, can be overridden in Inspector
    public Transform firePoint;

    public abstract void Fire();
}
